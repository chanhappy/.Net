using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Automation;

namespace FocusTools
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            Automation.AddAutomationFocusChangedEventHandler(OnFocusChangedHandler);
            Console.ReadLine();
        }

        private static void OnFocusChangedHandler(object src, AutomationFocusChangedEventArgs args)
        {
            try
            {
                Logger.Warn("[OnFocusChangedHandler]Focus changed!");
                Console.WriteLine("[OnFocusChangedHandler]Focus changed!");
                AutomationElement element = src as AutomationElement;
                if (element != null)
                {
                    string name = element.Current.Name;
                    string id = element.Current.AutomationId;
                    int processId = element.Current.ProcessId;
                    using (var process = Process.GetProcessById(processId))
                    {
                        Logger.Info($"[OnFocusChangedHandler]总的处理时间:{process.TotalProcessorTime},启动时间:{process.StartTime},进程ID:{processId},进程名称:{process.ProcessName}");
                        Console.WriteLine($"[OnFocusChangedHandler]总的处理时间:{process.TotalProcessorTime},启动时间:{process.StartTime},进程ID:{processId},进程名称:{process.ProcessName}"+ Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[OnFocusChangedHandler]ex:{ex.Message}");
                Console.WriteLine($"[OnFocusChangedHandler]ex:{ex.Message}");
            }
        }
    }
}
