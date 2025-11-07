using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityOSC;

namespace U9.OSC
{
    public class OSCController : OSCBase
    {
        //-----------------------------------------------------------------------------------------------------//
        // Avatar Events
        //-----------------------------------------------------------------------------------------------------//
        public event Action<string> OnConnected;
        public event Action<string> OnTest;
        public event Action OnDisconnected;
        private Coroutine timeoutCoroutine = null;

        public static OSCController Instance { get; private set; }

        // Add Events here!
        public event Action<string> OnCurrentState;
        public event Action<string> OnPlayerRegistered;
        public event Action OnStartRecieved;
        public event Action OnStopRecieved;
        public event Action OnResetRecieved;
        public event Action OnFullResetRecieved;
        public event Action<string, double> OnRoundTimeUpdate;

        public string ClientIP => settings.ClientIPAddress;

        private void Awake()
        {
            Instance = this;
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Processes the given packet
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        protected override void ProcessPacket(OSCPacket packet)
        {
            base.ProcessPacket(packet);

            // A packet is not Valid if it is a message with the same OSCCommand but older than the latest version of that OSCCommand
            // This ignores old (previously lost) messages from breaking logic
            // A packet is always valid if not using the OSCHandshake system
            if (packet.Valid)
            {
                // If using Handshake Method then the packed.Address will contian additional information
                // However here we are not interested in any of that (As the handshake service is all done in base.ProcessPacket())
                string address = packet.Address.Split('-')[0];

                try
                {
                    switch (address)
                    {
                        case OSCCommands.k_Connected:
                            OnConnected?.Invoke(OSCHelper.DataToString(packet.Data));
                            if (timeoutCoroutine != null)
                            {
                                StopCoroutine(timeoutCoroutine);
                                timeoutCoroutine = null;
                            }
                            break;
                        case OSCCommands.k_CurrentState:
                            OnCurrentState?.Invoke(OSCHelper.DataToString(packet.Data));
                            break;
                        case OSCCommands.k_TestCommand:
                            OnTest?.Invoke(OSCHelper.DataToString(packet.Data));
                            break;
                        case OSCCommands.k_RoundTime:
                            string[] tokens = OSCHelper.DataToString(packet.Data).Split(';');
                            OnRoundTimeUpdate?.Invoke(tokens[0], double.Parse(tokens[1]));
                            break;
                        case OSCCommands.k_PlayerRegistered:
                            OnPlayerRegistered?.Invoke(OSCHelper.DataToString(packet.Data));
                            break;
                        case OSCCommands.k_Started:
                            OnStartRecieved?.Invoke();
                            break;
                        case OSCCommands.k_Stopped:
                            OnStopRecieved?.Invoke();
                            break;
                        case OSCCommands.k_HasReset:
                            OnResetRecieved?.Invoke();
                            break;
                        case OSCCommands.k_HasFullReset:
                            OnFullResetRecieved?.Invoke();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message + " " + ex.StackTrace);
                }
            }
        }

        private IEnumerator StartTimeout()
        {
            yield return new WaitForSeconds(settings.PauseTimeout);

            OnDisconnected?.Invoke();
        }

        protected override void SendHeartbeat()
        {
            SendMessageToClient(k_Heartbeat, false);
        }

        protected override void SendConnectionMessage()
        {
            SendMessageToClient(OSCCommands.k_Connect, false, string.Format("{0};{1}", OSCHelper.GetIP(), settings.OwnPort));

            if (timeoutCoroutine != null)
            {
                StopCoroutine(timeoutCoroutine);
                timeoutCoroutine = null;
            }

            timeoutCoroutine = StartCoroutine(StartTimeout());
        }

        protected override void Initialize()
        {
            m_ClientIPAddress = PlayerPrefs.GetString("ClientIP", settings.ClientIPAddress);
            settings.ClientIPAddress = m_ClientIPAddress;
            m_ClientPort = settings.ClientPort1;

            base.StartServer();
        }

        public void ConnectAfterDelay(float delay)
        {
            if(!IsConnected)
                StartCoroutine(Connect(delay));
        }

        private IEnumerator Connect(float delay)
        {
            yield return new WaitForSeconds(delay);

            Initialize();
        }

        public void SetClientIP(string ip)
        {
            m_ClientIPAddress = settings.ClientIPAddress = ip;

            PlayerPrefs.SetString("ClientIP", ip);

            base.StartServer();
        }
    }

    public class OSCCommands
    {
        public const string k_TestCommand = "/test";

        // Tablet(s) to PC messages
        public const string k_Connect = "/connect";
        public const string k_RegisterPlayer = "/register_player";
        public const string k_Start = "/start";
        public const string k_Stop = "/stop";
        public const string k_Reset = "/reset";
        public const string k_FullReset = "/full_reset";
        public const string k_RequestSync = "/request_sync";

        // PC to Tablet(s) messages
        public const string k_Connected = "/connected";
        public const string k_CurrentState = "/current_state";
        public const string k_PlayerRegistered = "/player_registered";
        public const string k_Started = "/started";
        public const string k_Stopped = "/stopped";
        public const string k_HasReset = "/has_reset";
        public const string k_HasFullReset = "/has_full_reset";
        public const string k_RoundTime = "/roundtime";
    }
}