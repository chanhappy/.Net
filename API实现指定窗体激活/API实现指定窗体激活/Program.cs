using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;//调用DLLIMPORT
using System.Diagnostics;

namespace EmuWindowInfor
{
    /// <summary>
    /// 调用API的EnumWindows来枚举窗口
    /// </summary>
    class Program
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(int hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        static void Main(string[] args)
        {
            foreach (var p in Process.GetProcesses())
            {
                Console.WriteLine(p.MainWindowTitle);
            }

            IntPtr ParenthWnd = new IntPtr(0);

            ParenthWnd = FindWindow(null, "微信");
            //判断这个窗体是否有效
            if (ParenthWnd != IntPtr.Zero)
            {
                Console.WriteLine("找到窗口");
                var test = SetForegroundWindow((int)ParenthWnd);
                Console.WriteLine(test);
            }
            else
            {
                Console.WriteLine("没有找到窗口");
                var test = SetForegroundWindow((int)ParenthWnd);
                Console.WriteLine(test);
            }
                
            Console.ReadKey();
        }
    }
}