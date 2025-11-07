using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

namespace U9.OSC
{
	[System.Serializable]
	public class U9OscMessage
	{

		[SerializeField]
		protected string m_Address;


		public string Address
		{
			get { return m_Address; }
		}

		public U9OscMessage()
		{
			m_Address = "/blank";
		}

		public U9OscMessage(string address)
		{
			m_Address = address;
		}

		public virtual OSCMessage ToOSC()
		{
			return new OSCMessage(m_Address);
		}

		public virtual string ToString()
		{
			return m_Address.ToString();
		}
	}
}


