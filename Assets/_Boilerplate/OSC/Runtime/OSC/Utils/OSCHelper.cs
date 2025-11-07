using System.Collections.Generic;
using System.Net;

namespace U9.OSC
{
    public class OSCHelper
    {
        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Converts the given data buffer to a string
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        public static string DataToString(List<object> data)
        {
            string buffer = "";

            for (int i = 0; i < data.Count; i++)
            {
                buffer += data[i].ToString();
            }

            return buffer;
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Converts the given data buffer into a list of byte arrays
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        public static byte[] DataToByte(List<object> data)
        {
            List<byte> bytes = new List<byte>();

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].GetType() == typeof(byte[]))
                {
                    byte[] tmpBytes = data[i] as byte[];
                    if (tmpBytes.Length > 0)
                        bytes.AddRange(tmpBytes);
                }
            }

            return bytes.ToArray();
        }

        //-----------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Retreives the IP of this computer
        /// </summary>
        //-----------------------------------------------------------------------------------------------------//
        public static string GetIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                //Debug.Log("Found IP: " + ip.ToString());

                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }

                string[] split = ip.ToString().Split('.');
                if (split.Length == 4)
                {
                    int main = 0;
                    if (int.TryParse(split[0], out main))
                    {
                        if (main >= 100)
                        {
                            return ip.ToString();
                        }
                    }
                }
            }

            return "";
        }
    }
}