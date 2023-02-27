using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace WebSocketServer
{
    public class WebSocket
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WebSocket));

        private static WebSocketSharp.Server.WebSocketServer _server;

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="url"></param>
        public static void Start(string url)
        {
            Logger.Debug($"[Start]url:{url}");
            _server = new WebSocketSharp.Server.WebSocketServer(url);
            _server.AddWebSocketService<WebSocketSession>("/");
            _server.Start();
            Logger.Debug($"[Stop]websocket server start");
        }

        public static void Stop()
        {
            if (_server != null)
            {
                _server.Stop();
                Logger.Debug($"[Stop]websocket server stop");
                _server.RemoveWebSocketService("/");
            }
        }
    }
}
