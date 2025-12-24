using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FileWatcherTool
{
    class FileWatcher
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FileWatcher));
        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public static readonly IntPtr HFILE_ERROR = new IntPtr(-1);

        private static string _filePath = "";
        static void Main(string[] args)
        {
            //Console.WriteLine($"收到参数:{args[0]}");

            var filePath = @"C:\Users\auus\Desktop\Data\version.json";
            _filePath = filePath;

            Logger.Debug("启动文件监听！");
            MonitorDirectory(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));

            Logger.Debug("检查文件占用！");
            Thread t = new Thread(CheckOccupied);
            t.Start();

            Console.WriteLine("按q退出！");
        }

        private static void MonitorDirectory(string path, string filter)
        {
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = path;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite
                                 | NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size; 

            //文件类型，支持通配符，“*.txt”只监视文本文件
            fileSystemWatcher.Filter = filter;    // 监控的文件格式
            fileSystemWatcher.IncludeSubdirectories = true;  // 监控子目录
            fileSystemWatcher.Changed += new FileSystemEventHandler(OnProcess);
            fileSystemWatcher.Created += new FileSystemEventHandler(OnProcess);
            fileSystemWatcher.Renamed += new RenamedEventHandler(OnRenamed);
            fileSystemWatcher.Deleted += new FileSystemEventHandler(OnProcess);
            //表示当前的路径正式开始被监控，一旦监控的路径出现变更，FileSystemWatcher 中的指定事件将会被触发。
            fileSystemWatcher.EnableRaisingEvents = true;
        }
        private static void OnProcess(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                OnCreated(source, e);
            }
            else if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                OnChanged(source, e);
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                OnDeleted(source, e);
            }
        }

        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            Logger.Debug($"[OnCreated]File created=> FullPath:{e.FullPath}");
        }

        /// <summary>
        /// 查看文件是否被占用
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsFileOccupied(string filePath)
        {
            IntPtr vHandle = _lopen(filePath, OF_READWRITE | OF_SHARE_DENY_NONE);
            CloseHandle(vHandle);
            return vHandle == HFILE_ERROR ? true : false;
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                var content = File.ReadAllText(e.FullPath);
                var isJson = content.StartsWith("{");
                if (isJson)
                {
                    Logger.Debug($"[OnChanged]文件内容是否JSON格式：{isJson},内容：{content}");
                }
                else
                {
                    Logger.Error($"[OnChanged]文件内容是否JSON格式：{isJson},内容：{content}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[OnChanged]err：{ex.Message}");
                if (File.Exists(_filePath))
                {
                    var result = IsFileOccupied(_filePath);
                    if (result)
                    {
                        Logger.Error($"[OnChanged]================={_filePath}文件被占用=================");
                        Process tool = new Process();
                        tool.StartInfo.FileName = "handle.exe";
                        tool.StartInfo.Arguments = _filePath + " /accepteula";
                        tool.StartInfo.UseShellExecute = false;
                        tool.StartInfo.RedirectStandardOutput = true;
                        tool.Start();
                        tool.WaitForExit();
                        string outputTool = tool.StandardOutput.ReadToEnd();
                        string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
                        foreach (Match match in Regex.Matches(outputTool, matchPattern))
                        {
                            Logger.Error($"[OnChanged]占用{_filePath}文件的进程是：{Process.GetProcessById(int.Parse(match.Value))}");
                        }
                    }
                }
            }
        }
        private static void CheckOccupied()
        {
            while (File.Exists(_filePath))
            {
                var result = IsFileOccupied(_filePath);
                if (result)
                {
                    Logger.Error($"================={_filePath}文件被占用=================");
                    Process tool = new Process();
                    tool.StartInfo.FileName = "handle.exe";
                    tool.StartInfo.Arguments = _filePath + " /accepteula";
                    tool.StartInfo.UseShellExecute = false;
                    tool.StartInfo.RedirectStandardOutput = true;
                    tool.Start();
                    tool.WaitForExit();
                    string outputTool = tool.StandardOutput.ReadToEnd();
                    string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
                    foreach (Match match in Regex.Matches(outputTool, matchPattern))
                    {
                        Logger.Error($"[CheckOccupied]占用{_filePath}文件的进程是：{Process.GetProcessById(int.Parse(match.Value))}");
                    }
                }
            }
        }

        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            Logger.Debug($"[OnDeleted]File deleted=> FullPath:{e.FullPath}");
        }

        private static void OnRenamed(object source, FileSystemEventArgs e)
        {
            Logger.Debug($"[OnRenamed]File renamed=> FullPath:{e.FullPath}");
        }
    }
}
