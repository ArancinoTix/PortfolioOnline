using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

namespace U9.OSC
{
    /// <summary>
    /// When sending messages create an instance of one of these 
    /// <para>This acts like a service, sending the message again and again until the target receives 
    /// the message and sends a message back saying it has received the message. Once the message is recieved
    /// this instance will destroy itself.</para>
    /// <br>Note: Assign the Prefab "Handshaker" to an OSCBase for this service</br>
    /// </summary>
    public class OSCHandshake : MonoBehaviour
    {
        [SerializeField] private OSCSettings m_Settings;

        private OSCMessage m_Message;
        private OSCClient m_Client;

        private bool m_HandshakeReceived = false;

        public string OscCommand { get; private set; }

        /// <summary>
        /// A response should include in its Address <b>The Original OSCCommand string + Unique MessageID + ECHO</b>
        /// </summary>
        /// <param name="message">Message to check</param>
        /// <returns></returns>
        public bool CheckReceivedMessage(OSCPacket message)
        {
            string[] messageSent = m_Message.Address.Split('-');
            string[] messageFromResponce = message.Address.Split('-');

            if (messageSent.Length == 3 &&                  // Original message that was sent from here is of valid length
                messageFromResponce.Length == 3 &&          // Response messsage is of valid length
                messageSent[0] == messageFromResponce[0] && // OSCCommand are the same?
                messageSent[1] == messageFromResponce[1] && // Unique MesssageIDs are the same?
                messageFromResponce[2].Equals("ECHO"))      // Is the message an ECHO?
            {
                m_HandshakeReceived = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Instantly removes the Handshake service
        /// <para>Use this to stop an older version of a message from sending</para>
        /// </summary>
        public void RemoveService()
        {
            StopAllCoroutines();

            Destroy(gameObject);
        }

        /// <summary>
        /// Starts a coroutine to send a message
        /// Appends the message count and this devices own port number
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="messageID">The number of times this message has been sent (To determine if the one received is latest)</param>
        /// <param name="client">The client service used to send the message</param>
        /// <param name="resendDelay">How long to wait between retries</param>
        public void SendMessage(OSCMessage message, uint messageID, ref OSCClient client, ref OSCSettings settings, float resendDelay = 0.1f)
        {
            m_Message = message;

            m_Client = client;
            m_Settings = settings;

            // This is the command, saved for comparing, any new version of the same command still trying to get through will be stopped
            OscCommand = m_Message.Address;

            m_Message.Address += "-" + messageID + "-" + m_Settings.OwnPort.ToString();

            StartCoroutine(SendMessageToClientUntilResponse(m_Message, resendDelay));
        }

        public void UpdateClient(ref OSCClient client)
        {
            m_Client = client;
        }

        /// <summary>
        /// Keeps trying to send the message until it gets a response 
        /// or is superseded by a newer message with the same oscCommand
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ResendMessageDelay"></param>
        /// <returns></returns>
        private IEnumerator SendMessageToClientUntilResponse(OSCMessage message, float resendDelay = 0.1f)
        {
            if (m_Settings.IsVerbose)
                Debug.Log("Send " + message.Address);

            // Send the message once
            m_Client.Send(message);

            // Check for as long as SendHeartbeatTime + ReceiveHeartbeatTime (Expected timeout time)
            float currentWaitTime = 0.0f;
            while (currentWaitTime < m_Settings.SendHeartbeatTime + m_Settings.ReceiveHeartbeatTime && !m_HandshakeReceived)
            {
                // Wait resendDelay seconds, then try again
                float currentRetryTime = 0.0f;
                while (currentRetryTime < resendDelay && !m_HandshakeReceived)
                {
                    currentRetryTime += Time.deltaTime;
                    yield return null;
                }

                // If still no handshake then resend message
                if (!m_HandshakeReceived)
                {
                    if (m_Settings.IsVerbose)
                        Debug.Log("Resend " + message.Address);

                    m_Client.Send(message);
                }

                currentWaitTime += currentRetryTime;
            }

            // If still no handshake then we have timed out (something is wrong)
            if (!m_HandshakeReceived)
            {
                Debug.LogWarning("No Response " + m_Message.Address);

                // Disconnect?
                // NOTE: You could add a function here, like to Restart Experience
                // or something where you can retry connecting the devices
            }

            // Remove thy self, sevice no longer required
            Destroy(gameObject);
        }
    }
}