using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

//If you receive an error from this line, you need to change "API Compatibility Level" to .Net 2.0
//HOWEVER if you do not need serial ports, delete this file instead.
using System.IO.Ports;
using System;
using System.Threading;

namespace U9.Network
{

	public enum BaudRate
	{
		_300 = 300,
		_600 = 600,
		_1200 = 1200,
		_2400 = 2400,
		_4800 = 4800,
		_9600 = 9600,
		_14400 = 14400,
		_19200 = 19200,
		_28800 = 28800,
		_38400 = 38400,
		_57600 = 57600,
		_115200 = 115200,
		_250000 = 250000,
		_1MB = 1 * 1024 * 1204 * 8,
		_2MB = 2 * 1024 * 1204 * 8,
		_4MB = 4 * 1024 * 1204 * 8,
		_12MB = 12 * 1024 * 1204 * 8,
	}

	public class U9SerialPort : U9Thread
	{

		//--------------------------------------------------------------------------------------------------------------------------------//
		// Variables
		//--------------------------------------------------------------------------------------------------------------------------------//

		SerialPort m_SerialPort;                                                    // The serial port for the devices.
		[SerializeField] string m_ComPort = "COM5";             // Port address for the serial port.
		[SerializeField] BaudRate m_BaudRate = BaudRate._57600;     // Baudrate that the data is being sent at.
		[SerializeField] bool m_OpenPortOnStart = false;                // Do we open the port on the start of the scene?

		// The thread for getting data.
		[SerializeField] int m_ReadTimeout = 10;                    // 
		[SerializeField] int m_WriteTimeout = 10;                   //
		[SerializeField] int m_ReadInterval = 1000;

		bool m_IsRunning = false;               // Is the port up and running?
		string m_DataLine = "";                 // The data string that is built from the serial port.

		//--------------------------------------------------------------------------------------------------------------------------------//
		// ID
		//--------------------------------------------------------------------------------------------------------------------------------//

		public const char k_MESSAGE_START = '~';            // The first digit that defines the start of a message.
		public const char k_MESSAGE_END = '\n';         // The new line character that seperates data.
		public const char k_MESSAGE_SPLIT = '#';

		//--------------------------------------------------------------------------------------------------------------------------------//
		// Getters
		//--------------------------------------------------------------------------------------------------------------------------------//

		public bool IsRunning
		{
			get { return m_IsRunning; }
			set { m_IsRunning = value; }
		}

		public string ComPort
		{
			get { return m_ComPort; }
			set { m_ComPort = value; }
		}

		public BaudRate BaudRate
		{
			get { return m_BaudRate; }
			set { m_BaudRate = value; }
		}

		public bool OpenPortOnStart
		{
			get { return m_OpenPortOnStart; }
		}

		public SerialPort SerialPort
		{
			get { return m_SerialPort; }
		}


		//--------------------------------------------------------------------------------------------------------------------------------//
		// Init
		//--------------------------------------------------------------------------------------------------------------------------------//

		void Start()
		{
			if (m_OpenPortOnStart)
				Open();
		}

		/// <summary>
		/// The behaviour that occurs in the thread
		/// </summary>
		protected override void ThreadBehaviour()
		{
			try
			{
				m_SerialPort = new SerialPort(m_ComPort, (int)m_BaudRate);
				SerialPort.ReadTimeout = m_ReadTimeout;
				SerialPort.WriteTimeout = m_WriteTimeout;
				SerialPort.Open();

				while (true)
				{
					Thread.Sleep(m_ReadInterval);
					Read();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Error 1: " + ex.Message.ToString());
			}
		}

		/// <summary>
		/// Loop that reads the data as a chars
		/// </summary>
		private void Read()
		{
			try
			{
				//Debug.Log("1");

				//Need 1-2 bytes before we can read a char.
				if (SerialPort.IsOpen)// && SerialPort.BytesToRead>= 1)
				{
					// STRING BASED
					//*
					// Read line blocks until new line char has appeared.

					string message = SerialPort.ReadLine();
					ReadLine(message);
					//*/
					// BYTE BASED
					/*
						int newReadChar = SerialPort.ReadByte();

						if (newReadChar == -1)
							return;
						else if (newReadChar == 255)	// message end sign
						{
							// if (m_DataLine.Length < 3)
							//    throw new UnityException("BOOM");
							ReadLine(m_DataLine);
							m_DataLine = "";
						}
						else
						{
							// Add the char to the data string
							m_DataLine += ((char)newReadChar).ToString();
						}
						//*/

					// CHAR BASED
					/*
						//return;
					//Debug.Log("2");

					char newReadChar = (char)SerialPort.ReadChar();
					//Debug.Log("C: " + newReadChar);

					if(newReadChar == k_MESSAGE_END)
					{
						// If we had read '\n', parse the previous recorded string.
						//Debug.Log(m_DataLine);
						ReadLine(m_DataLine);
						m_DataLine = "";
					}
					else
					{
						// Add the char to the data string
						m_DataLine += newReadChar.ToString();
					}
					//*/
				}
			}
			catch (TimeoutException timeout)
			{
				// This will be triggered lots with the coroutine method
			}
			catch (Exception ex)
			{
				// This could be thrown if we close the port whilst the thread 
				// is reading data. So check if this is the case!
				if (SerialPort.IsOpen)
				{
					// Something has gone wrong!
					Debug.LogError("Error 4: " + ex.Message.ToString());
				}
				else
				{
					// Error caused by closing the port whilst in use! This is 
					// not really an error but uncomment if you wish.

					//Debug.Log("Error 5: Port Closed Exception!");
				}
			}

		}

		//--------------------------------------------------------------------------------------------------------------------------------//
		// Send and Receive
		//--------------------------------------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Sends data to through the serial port
		/// </summary>
		public void SendData(byte[] data)
		{
			if (SerialPort != null)
			{
				SerialPort.Write(data, 0, data.Length);
			}
		}

		public void SendData(string data)
		{
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);

			SendData(bytes);
		}


		/// <summary>
		/// Takes a read line from the serial port, and splits it into IMU lines
		/// </summary>
		void ReadLine(string line)
		{
			if (line != null && line != "")
			{
				Debug.Log(line);
				//if (line[0] == k_MESSAGE_START)
				//{
				ParseLine(line);
				//}
			}
		}

		protected virtual void ParseLine(string lineToRead)
		{
			string[] lines = lineToRead.Split(k_MESSAGE_SPLIT);

			foreach (string line in lines)
			{
				//for each line, if it is long enough, parse it
				if (line.Length > 3)
				{
					string opening = line.Substring(0, 3);

				}
			}
		}

		/// <summary>
		/// Stops the serial thread
		/// </summary>
		public override void Close()
		{
			base.Close();

			Thread.Sleep(100);

			if (m_SerialPort != null)
				m_SerialPort.Close();

			m_SerialPort = null;

			m_DataLine = "";
		}





	}
}