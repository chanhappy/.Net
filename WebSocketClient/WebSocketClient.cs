using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Web.Helpers;
using WebSocketSharp;
using System.Timers;
using System.Text;

namespace WebSocketClient
{
    public class WebSocketClient
    {
        private static WebSocket _webSocket;
        public static Timer _timer = new Timer();

        public static void Start()
        {
            _timer.Elapsed += new ElapsedEventHandler(HeartbeatMessage);
            _timer.Start();
            _timer.Interval = 1000 * 5;
             var url = "ws://127.0.0.1:53001/";

            _webSocket = new WebSocket(url);

            //事件
            _webSocket.OnMessage += Client_OnMessage;
            _webSocket.OnError += Client_OnError;
            _webSocket.OnOpen += Client_OnOpen;
            _webSocket.OnClose += Client_OnClose;

            //连接
            _webSocket.Connect();

            //推送注册信息
            Register();
        }

        private static void Register()
        {
            var initObj = new Dictionary<string, object>();
            var initData = new Dictionary<string, object>();
            initObj.Add("type", "init");
            initObj.Add("id", "newId11");
            initData.Add("branch", "b1");
            initObj.Add("data", initData);
            _webSocket.Send(JsonConvert.SerializeObject(initObj));
        }

        private static void Client_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("open event 捂手成功");
            var methodName = "iWrite";
            var iHtml = "iHtml";
            var iUser = "iUser";
            var iPass = "iPass";
            var iOprt = "iOprt";
            var iAAB301 = "iAAB301";
            var iAAC002 = "iAAC002";
            var iIprt = "iIprt";

            _webSocket.Send(JsonConvert.SerializeObject(new { methodName , iHtml, iUser, iPass, iOprt, iAAB301 , iAAC002 , iIprt }));
        }

        private static void Client_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine($"message Data: {e.Data}");
            if(e.Data == "heartbeat")
            {
                Console.WriteLine("收到心跳返回消息");
                return;
            }

            if (e.Data== "{\"code\":200,\"msg\":\"success\"}")
            {
                Console.WriteLine("收到注册消息");
                return;
            }

            var responseMsg = JsonConvert.DeserializeObject<ResponseMsg>(e.Data);
            Console.WriteLine("----------------");
            //推送回执信息
            var ackObj = new Dictionary<string, object>
            {
                { "type", "ack" },
                { "msgId", responseMsg.MsgId}
            };
            Console.WriteLine($"responseMsg.MsgId:{responseMsg.MsgId}");
            _webSocket.Send(JsonConvert.SerializeObject(ackObj));

            Console.WriteLine(JsonConvert.SerializeObject(responseMsg));
        }

        private static void Client_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("Error event");
        }

        public static void Stop()
        {
            _webSocket.Close();
            _timer.Elapsed -= new ElapsedEventHandler(HeartbeatMessage);
            _webSocket.OnClose -= Client_OnClose;
            _webSocket.OnOpen -= Client_OnOpen;
            _webSocket.OnError -= Client_OnError;
            _webSocket.OnMessage -= Client_OnMessage;
        }
        private static void Client_OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine("close event");
        }
        private static void HeartbeatMessage(object sender, ElapsedEventArgs e)
        {
            _webSocket.Send("heartbeat");
        }
    }
}
