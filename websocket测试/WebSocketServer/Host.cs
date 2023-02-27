using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace WebSocketServer
{
    public class Host
    {
        public static void Main(string[] args)
        {
            WebSocket.Start("ws://127.0.0.1:53001");
            Application.Run();
        }
    }
}
