using UnityEngine;

namespace U9.OSC
{
    [CreateAssetMenu(fileName = "OSCSettings", menuName = "OSC/Settings")]
    public class OSCSettings : ScriptableObject
    {
        [Header("Server")]
        [SerializeField] bool m_IsVerbose = false;
        [SerializeField] bool m_InitServerOnStart = false;
        [SerializeField] int m_OwnPort = 8004;
        [Header("Client")]
        [SerializeField] int m_ClientPort1 = 8000;
        [SerializeField] int m_ClientPort2 = 8001;
        [SerializeField] string m_ClientIPAddress = "192.168.1.3";
        [Header("Heartbeat")]
        [SerializeField] float m_ReceiveHeartbeatTime = 2;
        [SerializeField] bool m_ResendHeartbeatOnReceive = false;
        [SerializeField] [HideInInspector] float m_SendHeartbeatTime = 3;
        [Tooltip("Minutes, set -1 to disable timeout")] [Range(-1, 1440)] [SerializeField] float m_PauseTimeout = 30;

        public bool IsVerbose => m_IsVerbose;
        public bool InitServerOnStart => m_InitServerOnStart;
        public int OwnPort { get => m_OwnPort; set => m_OwnPort = value; }
        public int ClientPort1 { get => m_ClientPort1; set => m_ClientPort1 = value; }
        public int ClientPort2 { get => m_ClientPort2; set => m_ClientPort2 = value; }
        public string ClientIPAddress { get => m_ClientIPAddress; set => m_ClientIPAddress = value; }
        public float SendHeartbeatTime { get => m_SendHeartbeatTime; set => m_SendHeartbeatTime = value; }
        public float ReceiveHeartbeatTime => m_ReceiveHeartbeatTime;
        public bool ResendHeartbeatOnReceive => m_ResendHeartbeatOnReceive;
        public float PauseTimeout => m_PauseTimeout;
    }
}