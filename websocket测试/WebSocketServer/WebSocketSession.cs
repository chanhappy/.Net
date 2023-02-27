using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using NHMakeCard_tlb;
using Newtonsoft.Json;

namespace WebSocketServer
{
    public class WebSocketSession : WebSocketBehavior
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WebSocketSession));

        protected override void OnOpen()
        {
            base.OnOpen();
            Logger.Debug($"[OnOpen]websocket server open");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try{
                base.OnMessage(e);
                Logger.Debug($"[OnMessage]e.Data{ e.Data}");
                Send($"服务端收到了信息{e.Data}");

                //var jsonObj = JsonConvert.DeserializeObject<dynamic>(e.Data);

                //if (jsonObj["methodName"] == "iWrite")
                //{
                //    var iHtml = jsonObj["iHtml"];
                //    var iUser = jsonObj["iUser"];
                //    var iPass = jsonObj["iPass"];
                //    var iOprt = jsonObj["iOprt"];
                //    var iAAB301 = jsonObj["iAAB301"];
                //    var iAAC002 = jsonObj["iAAC002"];
                //    var iIprt = jsonObj["iIprt"];
                //    Logger.Debug($"[OnMessage]iHtml：{iHtml},iUser:{iUser},iPass:{iPass},iOprt:{iOprt},iAAB301:{iAAB301}, iAAC002:{iAAC002},iIprt:{iIprt}");

                //    var makeCard = new MAKECARD();
                //    var result = makeCard.iWrite(iHtml, iUser, iPass, iOprt, iAAB301, iAAC002, iIprt);
                //    var methodName = "iWrite";
                //    var sendData = JsonConvert.SerializeObject(new { methodName, result });
                //    Logger.Debug($"[OnMessage]sendData{ sendData}");
                //    Send(sendData);
                //}
            }
            catch (Exception ex) {
                Logger.Debug($"ex.Message：{ ex.Message }");
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            Logger.Warn($"[OnClose]websocket server close{ e.Reason}");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            Logger.Error($"[OnError]websocket server error{ e.Message}");
        }
    }
}
