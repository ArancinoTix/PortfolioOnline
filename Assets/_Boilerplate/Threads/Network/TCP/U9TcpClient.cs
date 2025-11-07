using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace U9.Network
{
	public class U9TcpClient : U9Thread
	{


		[SerializeField] int m_ReadInterval = 1000;
		[SerializeField] bool m_UseString = true;

		TcpClient m_Client;

		bool m_LockedToString;
		StreamWriter m_Writer;
		StreamReader m_Reader;

		public event EventHandler<U9TcpServer.DataReceivedEventArgs> OnDataReceived;
		public System.Action<int, byte[]> OnRawDataReceived;
		public event EventHandler<EventArgs> OnClientConnected;
		public event EventHandler<EventArgs> OnClientDisconnected;

		/// <summary>
		/// Starts the serial thread
		/// </summary>
		public override void Open()
		{
			Debug.Log("Please use Open with port and ipAddress, client not started");
		}

		public void Open(string ipAddress, int port)
		{
			try
			{
				m_LockedToString = m_UseString;

				m_Client = new TcpClient(ipAddress, port);

				if (m_LockedToString)
				{
					NetworkStream stream = m_Client.GetStream(); ;
					m_Reader = new StreamReader(stream);
					m_Writer = new StreamWriter(stream);
				}

				if (OnClientConnected != null)
					OnClientConnected(this, new EventArgs());
				base.Open();
			}
			catch (SocketException e)
			{
				Debug.Log("SocketException:" + e);
			}
		}

		/// <summary>
		/// The behaviour that occurs in the thread
		/// </summary>
		protected override void ThreadBehaviour()
		{
			try
			{
				while (true)
				{
					Thread.Sleep(m_ReadInterval);

					bool closed = false;

					// Detect if client disconnected
					if (m_Client.Client.Poll(0, SelectMode.SelectRead))
					{
						byte[] buff = new byte[1];
						if (m_Client.Client.Receive(buff, SocketFlags.Peek) == 0)
						{
							// Client disconnected
							closed = true;
						}
					}


					if (closed)
					{
						if (OnClientDisconnected != null)
							OnClientDisconnected(this, new EventArgs());

						Close();
					}
					else if (m_Client.Available > 0)
					{
						Debug.Log("Data is available");

						if (m_LockedToString)
						{
							while (m_Reader.Peek() >= 0)
							{
								HandleReceivedData(m_Reader.ReadLine());
							}
						}
						else
						{
							byte[] data = new byte[65000];
							int count = m_Client.Client.Receive(data);

							HandleReceivedData(count, data);
						}

					}

				}
			}
			catch (SocketException e)
			{
				Debug.Log("SocketException:" + e);
				if (OnClientDisconnected != null)
					OnClientDisconnected(this, new EventArgs());
				Close();
			}
		}

		/// <summary>
		/// Stops the serial thread
		/// </summary>
		public override void Close()
		{
			base.Close();

			Thread.Sleep(100);
			CloseClient();
		}

		void CloseClient()
		{
			if (m_Writer != null)
			{
				m_Writer.Close();
				m_Writer = null;
			}

			if (m_Reader != null)
			{
				m_Reader.Close();
				m_Reader = null;
			}

			if (m_Client != null)
			{
				m_Client.Close();
				m_Client = null;
			}
		}

		void HandleReceivedData(string data)
		{
			if (OnDataReceived != null)
				OnDataReceived(this, new U9TcpServer.DataReceivedEventArgs() { Data = data });
		}

		void HandleReceivedData(int count, byte[] data)
		{
			if (OnRawDataReceived != null)
				OnRawDataReceived(count, data);
		}

		public void Send(string data)
		{
			bool closed = false;
			if (!m_LockedToString)
				return;

			// Detect if client disconnected
			if (m_Client.Client.Poll(0, SelectMode.SelectRead))
			{
				byte[] buff = new byte[1];
				if (m_Client.Client.Receive(buff, SocketFlags.Peek) == 0)
				{
					// Client disconnected
					closed = true;
				}
			}

			if (closed)
			{
				if (OnClientDisconnected != null)
					OnClientDisconnected(this, new EventArgs());

				Close();
			}
			else
			{
				m_Writer.WriteLine(data);
				m_Writer.Flush();
			}
		}

		public void Send(byte[] data)
		{
			bool closed = false;
			if (m_LockedToString)
				return;

			// Detect if client disconnected
			if (m_Client.Client.Poll(0, SelectMode.SelectRead))
			{
				byte[] buff = new byte[1];
				if (m_Client.Client.Receive(buff, SocketFlags.Peek) == 0)
				{
					// Client disconnected
					closed = true;
				}
			}

			if (closed)
			{
				if (OnClientDisconnected != null)
					OnClientDisconnected(this, new EventArgs());

				Close();
			}
			else
			{
				m_Client.Client.Send(data);
			}
		}
	}
}