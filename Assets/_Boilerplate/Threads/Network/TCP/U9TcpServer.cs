using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.Networking;
using System.Net;
using System.Threading;
using System.IO;
using System;

namespace U9.Network
{
	public class U9TcpServer : U9Thread
	{

		[SerializeField] int m_Port = 3333;
		[SerializeField] int m_MaxClients = 3;
		[SerializeField] int m_ReadInterval = 1000;

		[SerializeField] bool m_UseString = true;

		public int Port
		{
			get
			{
				return m_Port;
			}
		}

		int m_NoOfConnectedClients = 0;
		TcpListener m_Server;
		List<TcpClient> m_Clients;
		List<StreamReader> m_Readers;
		List<StreamWriter> m_Writers;

		bool m_LockedToString;

		public event EventHandler<DataReceivedEventArgs> OnDataReceived;
		public System.Action<int, byte[]> OnRawDataReceived;
		public event EventHandler<EventArgs> OnClientConnected;
		public event EventHandler<EventArgs> OnClientDisconnected;

		public class DataReceivedEventArgs : EventArgs
		{
			public string Data { get; set; }
		}

		/// <summary>
		/// The behaviour that occurs in the thread
		/// </summary>
		protected override void ThreadBehaviour()
		{
			try
			{
				m_LockedToString = m_UseString;

				m_Clients = new List<TcpClient>();
				m_Writers = new List<StreamWriter>();
				m_Readers = new List<StreamReader>();

				m_Server = new TcpListener(IPAddress.Any, m_Port);
				m_Server.Start();

				while (true)
				{
					Thread.Sleep(m_ReadInterval);
					//Debug.Log("Waiting for a connection... ");

					//Try to accept clients
					if (m_NoOfConnectedClients < m_MaxClients && m_Server.Pending())
					{
						TcpClient client = m_Server.AcceptTcpClient();
						if (client != null)
						{
							m_Clients.Add(client);

							if (m_LockedToString)
							{
								NetworkStream stream = client.GetStream();
								m_Readers.Add(new StreamReader(stream));
								m_Writers.Add(new StreamWriter(stream));
							}

							m_NoOfConnectedClients++;

							if (OnClientConnected != null)
								OnClientConnected(this, new EventArgs());
						}
					}


					for (int i = 0, ni = m_Clients.Count; i < ni; i++)
					{
						TcpClient c = m_Clients[i];

						bool closed = false;

						// Detect if client disconnected
						if (c.Client.Poll(0, SelectMode.SelectRead))
						{
							byte[] buff = new byte[1];
							if (c.Client.Receive(buff, SocketFlags.Peek) == 0)
							{
								// Client disconnected
								closed = true;
							}
						}


						if (closed)
						{
							CloseClient(i);
							m_Clients.RemoveAt(i);

							if (m_LockedToString)
							{
								m_Readers.RemoveAt(i);
								m_Writers.RemoveAt(i);
							}

							i--;
							m_NoOfConnectedClients--;

							if (OnClientDisconnected != null)
								OnClientDisconnected(this, new EventArgs());
						}
						else if (c.Available > 0)
						{
							if (m_LockedToString)
							{
								StreamReader reader = m_Readers[i];
								while (reader.Peek() >= 0)
								{
									HandleReceivedData(reader.ReadLine());
								}
							}
							else
							{
								byte[] data = new byte[65000];
								int count = m_Clients[i].Client.Receive(data);

								HandleReceivedData(count, data);
							}
						}
					}
				}
			}
			catch (SocketException e)
			{
				Debug.Log("SocketException:" + e);
			}
		}

		/// <summary>
		/// Stops the serial thread
		/// </summary>
		public override void Close()
		{
			base.Close();

			Thread.Sleep(100);

			if (m_Clients != null)
			{
				for (int i = 0, ni = m_Clients.Count; i < ni; i++)
				{
					CloseClient(i);
				}

				m_Clients.Clear();
			}
			m_NoOfConnectedClients = 0;

			if (m_Server != null)
				m_Server.Stop();
			m_Server = null;

		}

		void CloseClient(int index)
		{
			if (m_LockedToString)
			{
				StreamWriter w = m_Writers[index];
				w.Close();
				w = null;

				StreamReader r = m_Readers[index];
				r.Close();
				r = null;
			}

			TcpClient c = m_Clients[index];
			c.Close();
			c = null;
		}

		void HandleReceivedData(string data)
		{
			if (OnDataReceived != null)
				OnDataReceived(this, new DataReceivedEventArgs() { Data = data });
		}

		void HandleReceivedData(int count, byte[] data)
		{
			if (OnRawDataReceived != null)
				OnRawDataReceived(count, data);
		}

		public void Send(string data)
		{
			if (!m_LockedToString)
				return;

			for (int i = 0, ni = m_Clients.Count; i < ni; i++)
			{
				TcpClient c = m_Clients[i];

				bool closed = false;

				// Detect if client disconnected
				if (c.Client.Poll(0, SelectMode.SelectRead))
				{
					byte[] buff = new byte[1];
					if (c.Client.Receive(buff, SocketFlags.Peek) == 0)
					{
						// Client disconnected
						closed = true;
					}
				}

				if (closed)
				{
					CloseClient(i);
					m_Clients.RemoveAt(i);
					m_Readers.RemoveAt(i);
					m_Writers.RemoveAt(i);

					i--;
					m_NoOfConnectedClients--;

					if (OnClientDisconnected != null)
						OnClientDisconnected(this, new EventArgs());
				}
				else
				{
					StreamWriter writer = m_Writers[i];
					writer.WriteLine(data);
					writer.Flush();
				}
			}
		}

		public void Send(byte[] data)
		{
			if (m_LockedToString)
				return;

			for (int i = 0, ni = m_Clients.Count; i < ni; i++)
			{
				TcpClient c = m_Clients[i];

				bool closed = false;

				// Detect if client disconnected
				if (c.Client.Poll(0, SelectMode.SelectRead))
				{
					byte[] buff = new byte[1];
					if (c.Client.Receive(buff, SocketFlags.Peek) == 0)
					{
						// Client disconnected
						closed = true;
					}
				}

				if (closed)
				{
					CloseClient(i);
					m_Clients.RemoveAt(i);

					if (m_LockedToString)
					{
						m_Readers.RemoveAt(i);
						m_Writers.RemoveAt(i);
					}

					i--;
					m_NoOfConnectedClients--;

					if (OnClientDisconnected != null)
						OnClientDisconnected(this, new EventArgs());
				}
				else
				{
					m_Clients[i].Client.Send(data);
				}
			}
		}

		void OnApplicationQuit()
		{
			Close();
		}

	}
}
