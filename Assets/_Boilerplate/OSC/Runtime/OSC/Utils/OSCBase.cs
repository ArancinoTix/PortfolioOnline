using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityOSC;

namespace U9.OSC
{
    public abstract class OSCBase : MonoBehaviour, ISender
    {
        [SerializeField] protected OSCSettings settings;

        public bool IsConnected => m_HasHeartbeat;
        public bool ShouldReset { get; private set; } = false;
        public event Action<bool> OnConnectionChanged;
        public event Action<bool> OnPause;
        public event Action<List<object>> OnHeartbeatReceived;
        public bool IgnoreHeartbeat { get; private set; }
        public OSCSettings Settings { get => settings; }

        //initialization
        private bool m_IsServerInitted = false;
        private bool m_IsClientInitted = false;
        private OSCServer m_Server;
        private OSCClient m_Client;
        private string m_CurrentIP = "";
        protected string m_ClientIPAddress;
        protected int m_ClientPort = 8000;
        private bool isPaused = false;

        //heartbeat
        protected const string k_Heartbeat = "/heartbeat";
        private bool m_HasHeartbeat = false;
        private bool m_lastHasHeartbeat = false;
        private float m_InternalSendHeartbeatTime;
        private float m_InternalReceiveHeartbeatTime;
        private bool m_ExpectingToReceiveHeartbeat = false;

        //packets
        private List<OSCPacket> m_ReceivedPackets;

        //handshake
        [Header("Handshake (Optional)")]
        [Range(0.01f, 0.5f)] public float ResendMessageDelay = 0.25f;
        private Dictionary<string, uint> m_SentMessageIDs = new Dictionary<string, uint>();
        private Dictionary<string, uint> m_ReceivedMessageIDs = new Dictionary<string, uint>();
        protected List<OSCHandshake> m_MessagesWaitingForHandshake = new List<OSCHandshake>();

        // EXTRA!!!
        private bool m_TriedConnection1 = false;

        #region Unity methods
        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Attempts to start the server on start if enabled.
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        void Start()
        {
            if (settings.InitServerOnStart)
                Initialize();
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Checks the last packet that was received and parses it
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        void Update()
        {
            if (m_IsServerInitted)
            {
                if (m_ReceivedPackets.Count > 0)
                {
                    OSCPacket packet = m_ReceivedPackets[0];
                    m_ReceivedPackets.RemoveAt(0);

                    if (packet != null)
                    {
                        ProcessPacket(packet);
                    }
                }
                if (isPaused) return;

                //If we are expecting to get a heartbeat from the PC
                if (m_ExpectingToReceiveHeartbeat || settings.ResendHeartbeatOnReceive)
                {
                    //If the heartbeat time has expired
                    if (m_InternalReceiveHeartbeatTime <= 0)
                    {
                        //We have not received anything from the PC, so reconnect
                        m_HasHeartbeat = false;
                        StopClient();

                        // Try connecting to one of the two servers (if we failed then try the other)
                        if (!m_TriedConnection1)
                            StartClient(m_ClientIPAddress, settings.ClientPort1);
                        else
                            StartClient(m_ClientIPAddress, settings.ClientPort2);

                        m_TriedConnection1 = !m_TriedConnection1;
                    }
                    else
                    {
                        //We still have time, so decrement it
                        m_InternalReceiveHeartbeatTime -= Time.deltaTime;
                    }
                }
                // We aren't expecting the heartbeat, so check if we should send
                else
                {
                    if (m_InternalSendHeartbeatTime <= 0)
                    {
                        ResetSendHeartbeat();
                        //Send a heartbeat
                        m_ExpectingToReceiveHeartbeat = true;

                        SendHeartbeat();
                    }
                    else
                    {
                        m_InternalSendHeartbeatTime -= Time.deltaTime;
                    }
                }

            }

            UpdateConnectionStatus();
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// When the application quits, shut down the client and server
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        void OnApplicationQuit()
        {
            Debug.Log("quit");
            Close();
        }

        public void Close()
        {
            StopServer();
            StopClient();
        }

        #endregion

        #region Virtual

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Processes the given packet
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        protected virtual void ProcessPacket(OSCPacket packet)
        {
            // Handshake:
            // Check if message is a response, that the target recieved a message sent from this device.
            // Or that it is a message from the Target and requires a response to say this device has got the message.

            string[] packetAddressTokens = packet.Address.Split('-');

            // Handshake only interested in message addresses with 3 parameters seperated by "-"
            if (packetAddressTokens.Length == 3)
            {
                // An ECHO of a messsage that was send out previously
                if (packetAddressTokens[2].Equals("ECHO"))
                {
                    // If it is this device that is expecting the response, can now stop and remove the Hanshhake service
                    CheckResponse(packet);
                }
                else if (packetAddressTokens[2].Equals(settings.ClientPort1.ToString()))
                {
                    // Check the list of received messages,
                    // if there was a oscCommand with a higher count sent before this one, then this packet should be marked as invalid
                    // Doing so will ignore old messages that never got through.
                    if (!CheckMessageIsValid(packetAddressTokens[0], uint.Parse(packetAddressTokens[1])))
                        packet.Valid = false;

                    // This is a message from the other device, Respond by sending with ECHO in Address
                    // Even if not valid anymore as we don't want it to be sent again either way
                    SendResponseToClient(packetAddressTokens[0] + "-" + packetAddressTokens[1] + "-ECHO");
                }
            }

            ResetReceiveHeartbeat();
            m_HasHeartbeat = true;
            if (IgnoreHeartbeat) return;

            if (packet != null && packet.Address == k_Heartbeat)
            {
                OnHeartbeatReceived?.Invoke(packet.Data);
            }

            if (!settings.ResendHeartbeatOnReceive) return;

            if (packet != null && packet.Address == k_Heartbeat)
            {
                SendHeartbeat();
            }
        }
        protected abstract void SendConnectionMessage();

        protected virtual void SendHeartbeat()
        {
            SendMessageToClient(new U9OscMessage(k_Heartbeat), false);
        }

        protected virtual void Initialize()
        {
            m_ClientIPAddress = settings.ClientIPAddress;
            m_ClientPort = settings.ClientPort1;

            if (settings.InitServerOnStart)
            {
                StartServer();
            }
        }

        protected void InitClient(string ip, int port)
        {
            StopClient();
            StartClient(ip, port);
        }
        #endregion

        #region Private
        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Updates te connection status
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        private void UpdateConnectionStatus()
        {
            if (m_HasHeartbeat != m_lastHasHeartbeat)
            {
                m_lastHasHeartbeat = m_HasHeartbeat;
                OnConnectionChanged?.Invoke(m_lastHasHeartbeat);
            }
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Starts the server
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        protected void StartServer()
        {
            if (ShouldReset)
                return;

            try
            {
                if (!m_IsServerInitted)
                {
                    m_CurrentIP = OSCHelper.GetIP();

                    if (m_CurrentIP == "")
                    {
                        Debug.Log("IP is not ready");
                        return;
                    }

                    m_Server = new UnityOSC.OSCServer(settings.OwnPort);
                    m_Server.Connect();

                    m_ReceivedPackets = new List<OSCPacket>();
                    m_Server.PacketReceivedEvent += OnPacketReceived;

                    m_IsServerInitted = true;

                    if (settings.IsVerbose)
                        Debug.Log("Starting the OSC Server");
                }
            }
            catch (Exception ex)
            {
                m_IsServerInitted = false;
                if (settings.IsVerbose)
                    Debug.Log("Couldn't start Server on port: " + settings.OwnPort + " message: " + ex);
            }
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Stops the server
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        private void StopServer()
        {
            try
            {
                if (m_Server != null)
                {
                    m_Server.Close();
                    m_IsServerInitted = false;

                    if (settings.IsVerbose)
                        Debug.Log("Stopping OSC Server");
                }
                m_IsServerInitted = false;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Called when our server receives a packet from a client.
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        private void OnPacketReceived(UnityOSC.OSCServer sender, UnityOSC.OSCPacket packet)
        {
            if (ShouldReset)
            {
                Debug.Log("Discarded packed");
                return;
            }

            m_ReceivedPackets.Add(packet);
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Connects the tablet client to the given ip and port
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        private void StartClient(string ipAddress, int port)
        {
            if (ShouldReset)
                return;
            m_ClientIPAddress = ipAddress;

            ResetReceiveHeartbeat();

            // Reset the received message IDs (To allow ones from 0 again as these are now valid)
            ResetReceivedMessageIDs();

            try
            {
                if (!m_IsClientInitted)
                {
                    IPAddress address;
                    bool parsed = IPAddress.TryParse(ipAddress, out address);

                    if (parsed)
                    {
                        m_Client = new OSCClient(address, port);

                        // For any handshakes happening, make sure to update with new client to send to
                        for (int I = 0; I < m_MessagesWaitingForHandshake.Count; I++)
                            m_MessagesWaitingForHandshake[I].UpdateClient(ref m_Client);

                        m_Client.Connect();

                        if (settings.IsVerbose)
                            Debug.Log("Starting the Client: " + ipAddress);

                        SendConnectionMessage();

                        m_IsClientInitted = true;
                    }
                    else
                        Debug.LogError("Failed to parse IP Address: " + ipAddress);
                }
                else
                {
                    SendConnectionMessage();
                }
            }
            catch (Exception ex)
            {
                m_IsClientInitted = false;
                if (settings.IsVerbose)
                    Debug.Log("Couldn't start Client on address: " + ipAddress + " message: " + ex.Message);
            }
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Stops the client
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        private void StopClient()
        {
            try
            {
                if (m_Client != null)
                {
                    m_Client.Close();
                    m_IsClientInitted = false;

                    if (settings.IsVerbose)
                        Debug.Log("Stopping Tablet Client");
                }
                m_IsClientInitted = false;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Reset the heartbeat timer
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        private void ResetSendHeartbeat()
        {
            m_InternalSendHeartbeatTime = settings.SendHeartbeatTime;
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Reset the heartbeat timer
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        private void ResetReceiveHeartbeat()
        {
            m_ExpectingToReceiveHeartbeat = false;
            m_InternalReceiveHeartbeatTime = settings.ReceiveHeartbeatTime;
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a OSCHandshake service that will keep trying to send until 
        /// the target sends back a response that it received the messsage
        /// </summary>
        /// <param name="message">The message to send</param>
        //-----------------------------------------------------------------------------------------------------//
        private void SendMessageToServer_HandshakeVersion(OSCMessage message)
        {
            // These messages are not supported by Handshake
            if (message.Address == k_Heartbeat ||
                message.Address == OSCCommands.k_Connected)
            {
                m_Client.Send(message);

                if (settings.IsVerbose)
                    Debug.Log("Send " + message.Address);
            }
            else
            {
                // Creates or updates the unique message count of specific command being sent, returns its value
                uint messageCount = IncrementMessageCount(message.Address);

                // Check and remove any older OSCHandshakes already trying to send a message with this same oscCommand
                RemoveAnyOldHandshakeServices(message.Address);

                // Create a OSCHandshake object, this will keep sending the message until a correct response or timeout
                GameObject newHandshakeGO = new GameObject("[ Message " + message.Address + " #" + messageCount + " ]");
                newHandshakeGO.transform.parent = transform;
                OSCHandshake newHandshake = newHandshakeGO.AddComponent<OSCHandshake>();

                // Start sending the message
                newHandshake.SendMessage(message, messageCount, ref m_Client, ref settings, ResendMessageDelay);

                // To keep track of the messages waiting to receive responses
                // These are checked when we receive a message
                // If it's an ECHO of a message this device sent, then its Handshake service will be destroyed (Message Sent and Received Successfully)
                m_MessagesWaitingForHandshake.Add(newHandshake);
            }
        }

        /// <summary>
        /// Increments sent message count for specified oscCommand
        /// <para>Create one if it doesn't exist</para>
        /// </summary>
        /// <param name="oscCommand"></param>
        /// <returns></returns>
        private uint IncrementMessageCount(string oscCommand)
        {
            if (m_SentMessageIDs.ContainsKey(oscCommand))
                return ++m_SentMessageIDs[oscCommand];
            else
                m_SentMessageIDs.Add(oscCommand, 0);
            return 0;
        }

        /// <summary>
        /// Checks messages and their count per oscCommand
        /// </summary>
        /// <param name="oscCommand"></param>
        /// <param name="receivedMessageID"></param>
        /// <returns>Whether the message is the latest of its kind</returns>
        private bool CheckMessageIsValid(string oscCommand, uint messageID)
        {
            if (m_ReceivedMessageIDs.ContainsKey(oscCommand))
            {
                if (m_ReceivedMessageIDs[oscCommand] < messageID)
                {
                    m_ReceivedMessageIDs[oscCommand]++;
                    return true;
                }
                else
                    return false;
            }
            else
                m_ReceivedMessageIDs.Add(oscCommand, 0);

            return true;
        }

        private void ResetReceivedMessageIDs()
        {
            m_ReceivedMessageIDs.Clear();
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Check received packet to see if it was a response to one of the messages being send by this device
        /// </summary>
        /// <param name="response"></param>
        //-----------------------------------------------------------------------------------------------------//
        private void CheckResponse(OSCPacket response)
        {
            for (int I = 0; I < m_MessagesWaitingForHandshake.Count; I++)
            {
                if (m_MessagesWaitingForHandshake[I].CheckReceivedMessage(response))
                {
                    m_MessagesWaitingForHandshake.RemoveAt(I);
                    break;
                }
            }
            m_MessagesWaitingForHandshake.TrimExcess();
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Removes any messages that are out of date (same as oscCommand but older)
        /// Also removes any handshakes in the list, that have destroyed themselves
        /// </summary>
        /// <param name="oscCommand"></param>
        //-----------------------------------------------------------------------------------------------------//
        private void RemoveAnyOldHandshakeServices(string oscCommand)
        {
            for (int I = 0; I < m_MessagesWaitingForHandshake.Count; I++)
            {
                // Remove any that have already been stopped
                if (!m_MessagesWaitingForHandshake[I])
                    m_MessagesWaitingForHandshake.RemoveAt(I);
                // Remove any that have same command
                else if (m_MessagesWaitingForHandshake[I].OscCommand.Equals(oscCommand))
                {
                    m_MessagesWaitingForHandshake[I].RemoveService();
                    m_MessagesWaitingForHandshake.RemoveAt(I);
                }
            }
            m_MessagesWaitingForHandshake.TrimExcess();
        }
        #endregion

        #region Public


        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Sends a response to the client that sent the message to begin with
        /// </summary>
        /// <param name="stringMessage"></param>
        //-----------------------------------------------------------------------------------------------------//
        public void SendResponseToClient(string stringMessage)
        {
            U9OscMessage message = new U9OscMessage(stringMessage);

            if (!m_IsClientInitted) return;
            if (settings.IsVerbose)
                Debug.Log("echo " + message.Address);

            if (m_Client == null || !m_Client.IsConnected)// || m_ClientPaused)
            {
                Debug.LogWarning("No client connected");
                return;
            }

            m_Client.Send(message.ToOSC());
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Sends a message to the PC/Tablet
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        public void SendMessageToClient(U9OscMessage message, bool handshake)
        {
            if (!m_IsClientInitted) return;
            if (settings.IsVerbose)
                Debug.Log("Send " + message.Address);

            if (m_Client == null || !m_Client.IsConnected)// || m_ClientPaused)
            {
                Debug.LogWarning("No client connected");
                return;
            }

            if (handshake)
                SendMessageToServer_HandshakeVersion(message.ToOSC());
            else
                m_Client.Send(message.ToOSC());
        }

        public void SendMessageToClient(string message, bool handshake)
        {
            SendMessageToClient(new U9OscMessage(message), handshake);
        }

        public void SendMessageToClient(string command, object payload, bool handshake)
        {
            if (settings.IsVerbose)
                Debug.Log("Send " + command);

            if (m_Client == null || !m_Client.IsConnected)// || m_ClientPaused)
            {
                Debug.LogWarning("No client connected");
                return;
            }

            OSCMessage oSCMessage = new OSCMessage(command, payload);
            if (!m_IsClientInitted)
                return;

            if (handshake)
                SendMessageToServer_HandshakeVersion(oSCMessage);
            else
                m_Client.Send(oSCMessage);
        }

        public void SendMessageToClient(string messageCommand, bool handshake, params string[] message)
        {
            if (settings.IsVerbose)
                Debug.Log("Send " + messageCommand);

            if (m_Client == null || !m_Client.IsConnected)// || m_ClientPaused)
            {
                Debug.LogWarning("No client connected");
                return;
            }

            OSCMessage oSCMessage = new OSCMessage(messageCommand);
            for (int i = 0; i < message.Length; i++)
            {
                string currMessage = message[i];
                oSCMessage.Append(currMessage);
            }

            if (handshake)
                SendMessageToServer_HandshakeVersion(oSCMessage);
            else
                m_Client.Send(oSCMessage);
        }

        public void Pause()
        {
            isPaused = true;
            OnPause?.Invoke(isPaused);
        }

        public void Unpause()
        {
            isPaused = false;
            OnPause?.Invoke(isPaused);
        }

        public void DisableHeartbeat()
        {
            IgnoreHeartbeat = true;
        }

        public void EnableHeartbeat()
        {
            IgnoreHeartbeat = false;
        }
        #endregion
    }
}