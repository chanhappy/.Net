using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MachineUtils
{
    class Program
    {
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);

        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(int dest, int host, byte[] mac, ref int length);

        static void Main(string[] args)
        {
            Console.WriteLine(GetLocalIp());
            Console.WriteLine(GetMACByIP(GetLocalIp()));
        }

        public static string GetLocalIp() {
            System.Net.IPAddress localIP = null;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                var endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address;
            }
            return localIP.ToString();
        }
        public static string GetMACByIP(string ip)
        {
            try
            {
                byte[] aa = new byte[6];
                var ldest = inet_addr(ip); //目的地的ip
                var len = 6;
                int res = SendARP(ldest, 0, aa, ref len);
                return BitConverter.ToString(aa, 0, 6); ;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
