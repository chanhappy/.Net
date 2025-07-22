using log4net;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;        // 窗口左上角的 x 坐标
        public int Top;         // 窗口左上角的 y 坐标
        public int Right;      // 窗口右下角的 x 坐标
        public int Bottom;     // 窗口右下角的 y 坐标
    }

    static void Main()
    {
        Console.WriteLine("====================已经打开的窗口=========start===================================" + Environment.NewLine);
        Logger.Debug("已经打开的窗口=========start=========" + Environment.NewLine);
        foreach (var process in Process.GetProcesses())
        {
            if (process.MainWindowTitle.Length != 0)
            {
                Logger.Debug(process.MainWindowTitle);
                Console.WriteLine(process.MainWindowTitle);
            }
        }
        Console.WriteLine(Environment.NewLine + "===========已经打开的窗口=========end====================================" + Environment.NewLine);
        Logger.Debug("已经打开的窗口=========end=========" + Environment.NewLine);

        GetWindowInfo("togoface.txt - 记事本");

        while(true)
        {
            var input = Console.ReadLine();
            GetWindowInfo(input);
        }
    }

    public static void GetWindowInfo(string windowName)
    {
        IntPtr hWnd = FindWindow(null, windowName);
        if (hWnd == IntPtr.Zero)
        {
            Console.WriteLine("未找到窗口");
            return;
        }

        RECT rect;
        GetWindowRect(hWnd, out rect);

        Console.WriteLine($"窗口  <  {windowName}  >  位置: X = {rect.Left}, Y = {rect.Top}");
        Logger.Debug($"窗口  <  {windowName}  >  位置: X = {rect.Left}, Y = {rect.Top}");

        Console.WriteLine($"窗口  <  {windowName}  >  大小: Width = {rect.Right - rect.Left}, Height = {rect.Bottom - rect.Top}");
        Logger.Debug($"窗口  <  {windowName}  >  大小: Width = {rect.Right - rect.Left}, Height = {rect.Bottom - rect.Top}");
    }
}