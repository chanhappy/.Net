using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Automation;

namespace WhereFocus
{
    class Program
    {
        /// <summary>
        /// 设置前置窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(int hWnd);

        [DllImport("user32.dll")]
        public static extern int GetFocus();
        

        static void Main(string[] args)
        {
            Automation.AddAutomationFocusChangedEventHandler(OnFocusChangedHandler);

            Console.WriteLine("Monitoring... Hit enter to end.");
    
            Console.ReadLine();

        }

        private static void OnFocusChangedHandler(object src, AutomationFocusChangedEventArgs args)
        {
            try
            {
                Console.WriteLine("Focus changed!");
                AutomationElement element = src as AutomationElement;
                if (element != null)
                {
                    string name = element.Current.Name;
                    string id = element.Current.AutomationId;
                    int processId = element.Current.ProcessId;
                    using (Process process = Process.GetProcessById(processId))
                    {
                        Console.WriteLine($"用户界面元素名称:{name},启动时间:{process.StartTime},进程ID:{processId},进程名称:{process.ProcessName},");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex:{ex.Message}");
            }
        }
    }
}
