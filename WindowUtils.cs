using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;//调用DLLIMPORT
using Awp.SS.Cordova;
using Awp.Logging;

namespace Awp.SS.Cordova.Plugin.WindowUtils
{
    /// <summary>
    /// window调用工具
    /// </summary>
    [CordovaPlugin]
    public class WindowUtils : AttributedCordovaPlugin
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(WindowUtils));

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(int hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 前置窗口
        /// </summary>
        /// <param name="downloadArguments"></param>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="callbackContext"></param>
        [CordovaPluginAction]
        public void FrontWindow(string windowName, CallbackContext callbackContext)
        {
            try
            {
                IntPtr ParenthWnd = new IntPtr(0);
                ParenthWnd = FindWindow(null, windowName);

                if (ParenthWnd == IntPtr.Zero)
                {
                    m_Logger.Error($"[WindowUtils]FrontWindow:没有找到窗口");
                    callbackContext.Error(new PluginResult(PluginResult.Status.OK, false));
                    return;
                }

                var result = SetForegroundWindow((int)ParenthWnd);
                m_Logger.Error($"[WindowUtils]FrontWindow:{result}");
                callbackContext.Success(new PluginResult(PluginResult.Status.OK, result));
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex);
                callbackContext.Error(new
                {
                    type = "Exception",
                    code = "",
                    message = ex.Message,
                    details = ex.StackTrace
                });
            }
        }
    }
}
