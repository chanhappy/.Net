using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketClient
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                WebSocketClient.Start();
                Console.WriteLine(DateTime.Now);
                Thread.Sleep(1000 * 600);
                WebSocketClient.Stop();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
