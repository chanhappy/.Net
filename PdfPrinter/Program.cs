using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using log4net;

namespace PdfPrinter
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                foreach (var item in args)
                {
                    Logger.Debug($"arg:{item}");
                }
            }

            //隐藏黑窗口
            Console.Title = "PdfPrinter";
            IntPtr intptr = FindWindow(null, "PdfPrinter");
            if (intptr != IntPtr.Zero)
            {
                ShowWindow(intptr, 0);
            }

            PdfPrint(args[0]);
        }

        public static void PdfPrint(string filePath)
        {
            Logger.Debug($"[pdfPrint]filePath:{filePath}");

            PrintDocument pd = new PrintDocument();
            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true,
                FileName = filePath,
                Verb = "print",
                Arguments = @"/p /h \" + filePath + "\"\"" + pd.PrinterSettings.PrinterName + "\""
            };

            p.StartInfo = startInfo;
            try
            {
                p.Start();
                p.WaitForExit();
                p.Close();
                p.Dispose();
                Logger.Debug($"[pdfPrint]print {filePath} done!");
            }
            catch (Exception ex)
            {
                Logger.Debug("[pdfPrint]打印文件异常：" + ex.Message);
                p.Close();
            }
        }
    }
}
