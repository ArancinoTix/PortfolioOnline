using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Network
{
	public class U9TcpHostClientManager : MonoSingleton<U9TcpHostClientManager>
	{

		[SerializeField] U9TcpServer m_Host;
		[SerializeField] U9TcpClient m_Client;

		static bool m_IsHost = true;
		bool m_Connected = false;
		int m_ClientCount = 0;

		List<string> m_ReceivedMessages;

		public System.Action<string> OnMessageReceived;
		bool m_Opened = false;

		public int ClientCount
		{
			get
			{
				return m_ClientCount;
			}
		}

		public bool Connected
		{
			get
			{
				return m_Connected;
			}
		}

		public static bool IsHost
		{
			get
			{
				return m_IsHost;
			}
		}

		// Use this for initialization
		void Start()
		{
			Instance = this;
		}

		// Use this for initialization
		public void OpenAsHost()
		{
			m_ReceivedMessages = new List<string>();
			m_IsHost = true;

			m_Host.OnDataReceived += OnClientMessageReceived;
			m_Host.OnClientConnected += OnClientConnected;
			m_Host.OnClientDisconnected += OnClientDisconnected;
			m_Host.Open();
			m_Connected = true;


			m_Opened = true;
		}

		public void OpenAsClient(string ip, int port)
		{
			m_ReceivedMessages = new List<string>();
			m_IsHost = false;
			
			m_Client.OnDataReceived += OnHostMessageReceived;
			m_Client.OnClientConnected += OnClientConnected;
			m_Client.OnClientDisconnected += OnClientDisconnected;
			m_Client.Open(ip, port);

			m_Opened = true;
		}


		public void Close()
		{
			if (m_Opened)
			{
				m_Host.OnDataReceived -= OnClientMessageReceived;
				m_Host.OnClientConnected -= OnClientConnected;
				m_Host.OnClientDisconnected -= OnClientDisconnected;


				m_Client.OnDataReceived -= OnHostMessageReceived;
				m_Client.OnClientConnected -= OnClientConnected;
				m_Client.OnClientDisconnected -= OnClientDisconnected;

				m_Host.Close();
				m_Client.Close();
				m_ReceivedMessages.Clear();
				m_Connected = false;
				m_ClientCount = 0;

			}
			m_Opened = false;
		}

		void Update()
		{
			if (m_Connected)
			{
				int ni = m_ReceivedMessages.Count;

				if (ni > 0)
				{
					for (int i = 0; i < ni; i++)
					{
						string message = m_ReceivedMessages[i];

						if (OnMessageReceived != null)
							OnMessageReceived(message);
					}
					m_ReceivedMessages.Clear();
				}
			}
		}

		/*
		void OnHostDiscovered (object sender, NetworkDiscoverer.ServerEventArgs e)
		{
			if (!m_Connected) {
				Debug.Log ("Server discovered from: " + e.Address + ", " + e.Port);
				//NetworkDiscoverer.Instance.StopBroadcast ();

				m_Client.Open (e.Address, e.Port);
			}
		}
		*/

		void OnClientMessageReceived(object sender, U9TcpServer.DataReceivedEventArgs e)
		{
			Debug.Log("<color=cyan>Received Client Message: </color>" + e.Data);
			m_ReceivedMessages.Add(e.Data);
		}

		void OnHostMessageReceived(object sender, U9TcpServer.DataReceivedEventArgs e)
		{
			Debug.Log("<color=cyan>Received Client Message: </color>" + e.Data);
			m_ReceivedMessages.Add(e.Data);
		}

		void OnClientConnected(object sender, System.EventArgs e)
		{
			Debug.Log("<color=green>Client connected</color>");
			m_ClientCount++;

			if (!m_IsHost)
				m_Connected = true;
		}

		void OnClientDisconnected(object sender, System.EventArgs e)
		{
			Debug.Log("<color=red>Client disconnected</color>");
			m_ClientCount--;

			if (!m_IsHost)
				m_Connected = false;
		}

		public void SendMessage(string message)
		{
			if (m_IsHost)
				m_Host.Send(message);
			else
				m_Client.Send(message);
		}
	}
}