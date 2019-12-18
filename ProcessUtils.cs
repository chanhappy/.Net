using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Awp.Logging;

namespace Awp.SS.Cordova.Plugin
{
    /// <summary>
    /// 进程操作工具类
    /// </summary>
    [CordovaPlugin]
    public class ProcessUtils : AttributedCordovaPlugin
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(ProcessUtils));

        /// <summary>
        /// 开启一个进程
        /// </summary>
        /// <param name="fileName">可执行应用程序的文件名</param>
        /// <param name="arguments">启动进程时命令行参数</param>
        /// <param name="isSystemProcess">是否是系统进程调用</param>
        /// <param name="isWaitForExit">是否等待退出</param>
        /// <param name="callbackContext">回调上下文</param>
        [CordovaPluginAction]
        public async void StartProcessAsync(string fileName, string[] arguments, bool isSystemProcess, bool isWaitForExit, CallbackContext callbackContext)
        {
            m_Logger.Debug($"[startProcessArgument]fileName:{fileName},Arguments:{arguments}");

            Process startProcess = new Process();
            try
            {
                await Task.Factory.StartNew(() => {
                    startProcess.StartInfo.UseShellExecute = true;
                    startProcess.StartInfo.FileName = Path.GetFullPath(fileName);
                    if (isSystemProcess)
                        startProcess.StartInfo.FileName = fileName;
                    if (arguments.Length > 0 && arguments != null)
                        startProcess.StartInfo.Arguments = string.Join(" ", arguments);

                    startProcess.Start();

                    if (isWaitForExit)
                    {
                        startProcess.WaitForExit();
                        callbackContext.Success(startProcess.ExitCode);
                    }
                    else
                    {
                        callbackContext.Success();
                    }
                });
            }
            catch (Exception e)
            {
                m_Logger.Error(e);
                callbackContext.Error(new
                {
                    type = e.GetType().Name,
                    code = "",
                    message = e.Message,
                    details = e.StackTrace
                });
            }
        }
    }
}
