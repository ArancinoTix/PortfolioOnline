using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

namespace U9.Network
{
	public abstract class U9Thread : MonoBehaviour
	{

		//--------------------------------------------------------------------------------------------------------------------------------//
		// Variables
		//--------------------------------------------------------------------------------------------------------------------------------//

		protected Thread m_Thread = null;
		protected bool m_IsThreadOpen = false;

		//--------------------------------------------------------------------------------------------------------------------------------//
		// Getters
		//--------------------------------------------------------------------------------------------------------------------------------//

		public bool IsThreadOpen
		{
			get
			{
				return m_IsThreadOpen;
			}
		}

		//--------------------------------------------------------------------------------------------------------------------------------//
		// Thread
		//--------------------------------------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Starts the serial thread
		/// </summary>
		public virtual void Open()
		{
			try
			{
				if (!m_IsThreadOpen)
				{
					m_IsThreadOpen = true;

					m_Thread = new Thread(new ThreadStart(ThreadBehaviour));
					m_Thread.Start();
				}
			}
			catch (Exception ex)
			{
				// Failed to start thread
				Debug.LogError("Error 3: " + ex.Message.ToString());
			}
		}

		/// <summary>
		/// The behaviour that occurs in the thread
		/// </summary>
		protected abstract void ThreadBehaviour();

		/// <summary>
		/// Stops the serial thread
		/// </summary>
		public virtual void Close()
		{
			if (m_IsThreadOpen)
			{
				m_IsThreadOpen = false;

				Thread.Sleep(100);

				if (m_Thread != null)
				{
					m_Thread.Abort();
					Thread.Sleep(100);
					m_Thread = null;
				}

				Debug.Log("Ended Serial Loop Thread!");
			}
		}

		void OnApplicationQuit()
		{
			Close();
		}

	}
}
