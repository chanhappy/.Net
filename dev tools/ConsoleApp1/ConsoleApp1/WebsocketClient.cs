using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Threading;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    class WebsocketClient
    {
        public static int TakeHeapSnapshotID { get; set; }
        private static List<string> HeapSnapshotContent = new List<string>();
        public static void Main(string[] args)
        {
            var ws = new WebSocket("ws://localhost:9222/devtools/page/E039658B3E4F35558B1ACE9EA3C6EA94");
            var content = "";
            ws.OnMessage += (sender, e) =>
            {
                //Console.WriteLine($"aaaaaaaaaaaaaaaaaaaaaaaa:{e.Data}");
                var data = JsonConvert.DeserializeObject(e.Data) as JObject;

                if (data["id"] != null)
                {
                    //Console.WriteLine($"data:" + data);
                }
                if (data["method"] != null)
                {
                    Console.WriteLine($"method:" + data["method"].ToString());
                }
                if (data["method"] != null && data["method"].ToString() == "HeapProfiler.addHeapSnapshotChunk")
                {
                    content += data["params"]["chunk"].ToString();
                }

                if (data["id"] != null && int.Parse(data["id"].ToString()) == TakeHeapSnapshotID)
                {
                    File.WriteAllText($"C:/Users/Administrator/Desktop/profile{data["id"]}.heapsnapshot", content);
                    Console.WriteLine("end");
                }
            };

            ws.Connect();

            ////垃圾回收
            //ws.Send(JsonConvert.SerializeObject(new Command() { ID = 1, Method = "HeapProfiler.collectGarbage", Params = { } }));

            ////获取快照
            var commandParams = new CommandParams() { ReportProgress = false, TreatGlobalObjectsAsRoots = false };
            TakeHeapSnapshotID = new Random().Next();
            ws.Send(JsonConvert.SerializeObject(new Command() { ID = 1, Method = "HeapProfiler.takeHeapSnapshot", Params = commandParams }));
            Console.ReadKey(true);
        }
}
}