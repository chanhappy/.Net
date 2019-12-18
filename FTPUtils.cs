using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Awp.Common;
using Awp.Logging;
using Newtonsoft.Json;

namespace Awp.SS.Cordova.Plugin
{
    /// <summary>
    /// FTP工具类
    /// </summary>
    [CordovaPlugin]
    public class FTPUtils : AttributedCordovaPlugin
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(FTPUtils));

        /// <summary>
        /// 从FTP服务器下载文件
        /// </summary>
        /// <param name="ip">远程地址</param>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <param name="localFileRelativePath">本地存放相对应用Temp的路径</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="isUsePassive">是否被动模式</param>
        /// <param name="callbackContext"></param>
        [CordovaPluginAction]
        public void DownloadFileToAppTempDirectoryAsync(string ip, string remoteFilePath, string localFileRelativePath, string userName, string password, bool isUsePassive, CallbackContext callbackContext)
        {
            var localFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp", localFileRelativePath);
            DownloadFileAsync(ip, remoteFilePath, localFilePath, userName, password, isUsePassive, callbackContext);
        }

        /// <summary>
        /// 在FTP服务器上创建文件夹
        /// </summary>
        /// <param name="ip">远程地址</param>
        /// <param name="remoteFilePath">需要创建文件夹的名称或者目录</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="isUsePassive">是否被动模式</param>
        /// <param name="callbackContext"></param>
        [CordovaPluginAction]
        public async void MakeSubDirectoryAsync(string ip, string remoteFilePath, string userName, string password, bool isUsePassive, CallbackContext callbackContext)
        {
            try
            {
                m_Logger.Debug($"ip:{ip},remoteFilePath:{remoteFilePath}");
                FTPClient ftpClient = new FTPClient($"ftp://{ip}");
                await ftpClient.CheckDirectoryAsync(remoteFilePath, userName, password, isUsePassive);
                callbackContext.Success();
            }
            catch (Exception e)
            {
                m_Logger.Error(e);
                callbackContext.Error(new
                {
                    type = "Exception",
                    code = "",
                    message = e.Message,
                    details = e.StackTrace
                });
            }
        }

        /// <summary>
        /// 列出子目录
        /// </summary>
        /// <param name="ip">远程地址</param>
        /// <param name="remoteFilePath">需要创建文件夹的名称或者目录</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="isUsePassive">是否被动模式</param>
        /// <param name="callbackContext">回掉上下文</param>
        [CordovaPluginAction]
        public async void ListDirectoryAsync(string ip, string remoteFilePath, string userName, string password, bool isUsePassive, CallbackContext callbackContext)
        {
            m_Logger.Debug($"ip:{ip},remoteFilePath:{remoteFilePath}");
            try
            {
                FTPClient ftpClient = new FTPClient($"ftp://{ip}");
                string[] ftpResult = await ftpClient.ListDirectoryAsync(remoteFilePath, userName, password, isUsePassive);
                callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, ftpResult));
            }
            catch (Exception e)
            {
                m_Logger.Error(e);
                callbackContext.Error(new
                {
                    type = "Exception",
                    code = "",
                    message = e.Message,
                    details = e.StackTrace
                });
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="ip">远程地址</param>
        /// <param name="remoteFilePath">需要创建文件夹目录</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="isUsePassive">是否被动模式</param>
        /// <param name="callbackContext">回掉上下文</param>
        [CordovaPluginAction]
        public async void DeleteFtpDirectoryAsync(string ip, string remoteFilePath, string userName, string password, bool isUsePassive, CallbackContext callbackContext)
        {
            m_Logger.Debug($"ip:{ip},remoteFilePath:{remoteFilePath}");
            try
            {
                FTPClient ftpClient = new FTPClient($"ftp://{ip}");
                var ftpResult = await ftpClient.DeleteDirectoryAsync(remoteFilePath, userName, password, isUsePassive);
                callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, ftpResult));
            }
            catch (Exception e)
            {
                m_Logger.Error(e);
                callbackContext.Error(new
                {
                    type = "Exception",
                    code = "",
                    message = e.Message,
                    details = e.StackTrace
                });
            }
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="ip">远程地址</param>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <param name="newFileName">重命名文件新名字</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="isUsePassive">是否被动模式</param>
        /// <param name="callbackContext">回掉上下文</param>
        [CordovaPluginAction]
        public async void RenameFileAsync(string ip, string remoteFilePath, string newFileName, string userName, string password, bool isUsePassive, CallbackContext callbackContext)
        {
            try
            {
                m_Logger.Debug($"ip:{ip},remoteFilePath:{remoteFilePath},newFileName:{newFileName}");

                var ftpClient = new FTPClient($"ftp://{ip}");
                var ftpResult = await ftpClient.RenameFileAsync(remoteFilePath, newFileName, userName, password, isUsePassive);
                callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, ftpResult));
            }
            catch (Exception e)
            {
                m_Logger.Error(e);
                callbackContext.Error(new
                {
                    type = "Exception",
                    code = "",
                    message = e.Message,
                    details = e.StackTrace
                });
            }
        }

        /// <summary>
        /// 从FTP服务器下载文件
        /// </summary>
        /// <param name="ip">远程地址</param>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <param name="localFilePath">本地存放路径</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="isUsePassive">是否被动模式</param>
        /// <param name="callbackContext">回掉上下文</param>
        [CordovaPluginAction]
        public async void DownloadFileAsync(string ip, string remoteFilePath, string localFilePath, string userName, string password, bool isUsePassive, CallbackContext callbackContext)
        {
            try
            {
                m_Logger.Debug($"ip:{ip},remoteFilePath:{remoteFilePath},localFilePath:{localFilePath}");

                var ftpClient = new FTPClient($"ftp://{ip}");
                var ftpResult = await ftpClient.DownloadFileAsync(remoteFilePath, localFilePath, userName, password, isUsePassive);
                callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, ftpResult));
            }
            catch (Exception e)
            {
                m_Logger.Error(e);
                callbackContext.Error(new
                {
                    type = "Exception",
                    code = "",
                    message = e.Message,
                    details = e.StackTrace
                });
            }
        }

        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="isNeedCheckDirectory">是否需要检查目录</param>        
        /// <param name="ip">远程地址</param>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <param name="localFilePath">本地文件路径</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="isUsePassive">是否被动模式</param>
        /// <param name="callbackContext">回掉上下文</param>        
        [CordovaPluginAction]
        public async void UploadFileAsync(bool isNeedCheckDirectory, string ip, string remoteFilePath, string localFilePath, string userName, string password, bool isUsePassive, CallbackContext callbackContext)
        {
            try
            {
                m_Logger.Debug($"ip:{ip},remoteFilePath:{remoteFilePath},localFilePath:{localFilePath}");

                var ftpClient = new FTPClient($"ftp://{ip}");
                var ftpResult = await ftpClient.UploadFileAsync(isNeedCheckDirectory, remoteFilePath, localFilePath, userName, password, isUsePassive);
                callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, ftpResult));
            }
            catch (Exception e)
            {
                m_Logger.Error(e);
                callbackContext.Error(new
                {
                    type = "Exception",
                    code = "",
                    message = e.Message,
                    details = e.StackTrace
                });
            }
        }

        #region 新组织结构Js SDK对应的Cordova插件API

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="directoryArgument">创建文件夹参数</param>
        /// <param name="callbackContext">回调上下文</param>
        [CordovaPluginAction]
        public void MakeDirectoryAsync(DirectoryArgument directoryArgument, CallbackContext callbackContext)
        {
            try
            {
                Requires.NotNull(directoryArgument, "directoryArgument");
                Requires.NotNullOrEmpty(directoryArgument.IP, "IP");
                Requires.NotNullOrEmpty(directoryArgument.RemoteFilePath, "RemoteFilePath");
                Requires.NotNullOrEmpty(directoryArgument.UserName, "UserName");
                Requires.NotNullOrEmpty(directoryArgument.Password, "Password");
                m_Logger.Debug($"makeDirectoryArgument:{directoryArgument}");
                this.MakeSubDirectoryAsync(directoryArgument.IP, directoryArgument.RemoteFilePath,
                       directoryArgument.UserName, directoryArgument.Password, directoryArgument.IsUsePassive, callbackContext);
            }
            catch (Exception e)
            {
                m_Logger.Error(e);
                callbackContext.Error(new
                {
                    type = "Exception",
                    code = "",
                    message = e.Message,
                    details = e.StackTrace
                });
            }

        }

        /// <summary>
        /// 列出子目录
        /// </summary>
        /// <param name="directoryArgument">列出子文件夹参数</param>
        /// <param name="callbackContext">回调上下文</param>
        [CordovaPluginAction]
        public void ListSubDirectoryAsync(DirectoryArgument directoryArgument, CallbackContext callbackContext)
        {
            try
            {
                Requires.NotNull(directoryArgument, "directoryArgument");
                Requires.NotNullOrEmpty(directoryArgument.IP, "IP");
                Requires.NotNullOrEmpty(directoryArgument.RemoteFilePath, "remoteFilePath");
                Requires.NotNullOrEmpty(directoryArgument.UserName, "UserName");
                Requires.NotNullOrEmpty(directoryArgument.Password, "Password");
                m_Logger.Debug($"makeDirectoryArgument:{directoryArgument}");
                this.ListDirectoryAsync(directoryArgument.IP, directoryArgument.RemoteFilePath,
                       directoryArgument.UserName, directoryArgument.Password, directoryArgument.IsUsePassive, callbackContext);
            }
            catch (Exception e)
            {
                m_Logger.Error(e);
                callbackContext.Error(new
                {
                    type = "Exception",
                    code = "",
                    message = e.Message,
                    details = e.StackTrace
                });
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="directoryArgument">删除文件夹参数</param>
        /// <param name="callbackContext">回掉上下文</param>
        [CordovaPluginAction]
        public void DeleteDirectoryAsync(DirectoryArgument directoryArgument, CallbackContext callbackContext)
        {
            try
            {
                Requires.NotNull(directoryArgument, "directoryArgument");
                Requires.NotNullOrEmpty(directoryArgument.IP, "IP");
                Requires.NotNullOrEmpty(directoryArgument.RemoteFilePath, "remoteFilePath");
                Requires.NotNullOrEmpty(directoryArgument.UserName, "UserName");
                Requires.NotNullOrEmpty(directoryArgument.Password, "Password");
                m_Logger.Debug($"makeDirectoryArgument:{directoryArgument}");
                this.DeleteFtpDirectoryAsync(directoryArgument.IP, directoryArgument.RemoteFilePath,
                       directoryArgument.UserName, directoryArgument.Password, directoryArgument.IsUsePassive, callbackContext);
            }
            catch (Exception e)
            {
                m_Logger.Error(e);
                callbackContext.Error(new
                {
                    type = "Exception",
                    code = "",
                    message = e.Message,
                    details = e.StackTrace
                });
            }
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="renameArgument">重命名参数</param>
        /// <param name="callbackContext">回调上下文</param>
        [CordovaPluginAction]
        public void RenameAsync(FTPRenameArgument renameArgument, CallbackContext callbackContext)
        {
            try
            {
                Requires.NotNull(renameArgument, "renameArgument");
                Requires.NotNullOrEmpty(renameArgument.IP, "IP");
                Requires.NotNullOrEmpty(renameArgument.RemoteFilePath, "RemoteFilePath");
                Requires.NotNullOrEmpty(renameArgument.NewFileName, "NewFileName");
                Requires.NotNullOrEmpty(renameArgument.UserName, "UserName");
                Requires.NotNullOrEmpty(renameArgument.Password, "Password");

                this.RenameFileAsync(renameArgument.IP, renameArgument.RemoteFilePath, renameArgument.NewFileName,
                   renameArgument.UserName, renameArgument.Password, renameArgument.IsUsePassive, callbackContext);
            }
            catch (Exception e)
            {
                m_Logger.Error(e);
                callbackContext.Error(new
                {
                    type = "Exception",
                    code = "",
                    message = e.Message,
                    details = e.StackTrace
                });
            }
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="ftpArgument">ftp通信参数</param>
        /// <param name="callbackContext">回调上下文</param>
        [CordovaPluginAction]
        public void DownloadAsync(FTPClientArgument ftpArgument, CallbackContext callbackContext)
        {
            try
            {
                Requires.NotNull(ftpArgument, "ftpArgument");

                Requires.NotNullOrEmpty(ftpArgument.IP, "IP");
                Requires.NotNullOrEmpty(ftpArgument.LocalFilePath, "LocalFilePath");
                Requires.NotNullOrEmpty(ftpArgument.RemoteFilePath, "RemoteFilePath");
                Requires.NotNullOrEmpty(ftpArgument.UserName, "UserName");
                Requires.NotNullOrEmpty(ftpArgument.Password, "Password");

                ftpArgument.LocalFilePath = Path.GetFullPath(ftpArgument.LocalFilePath);//相对路径转换成本地运行程序的绝对路径
                this.DownloadFileAsync(ftpArgument.IP, ftpArgument.RemoteFilePath, ftpArgument.LocalFilePath,
                   ftpArgument.UserName, ftpArgument.Password, ftpArgument.IsUsePassive, callbackContext);
            }
            catch (Exception e)
            {
                m_Logger.Error(e);
                callbackContext.Error(new
                {
                    type = "Exception",
                    code = "",
                    message = e.Message,
                    details = e.StackTrace
                });
            }
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="ftpArgument">ftp通信参数</param>
        /// <param name="callbackContext">回调上下文</param>
        [CordovaPluginAction]
        public void UploadAsync(FTPClientArgument ftpArgument, CallbackContext callbackContext)
        {
            try
            {
                Requires.NotNull(ftpArgument, "ftpArgument");

                Requires.NotNullOrEmpty(ftpArgument.IP, "IP");
                Requires.NotNullOrEmpty(ftpArgument.LocalFilePath, "LocalFilePath");
                Requires.NotNullOrEmpty(ftpArgument.RemoteFilePath, "RemoteFilePath");
                Requires.NotNullOrEmpty(ftpArgument.UserName, "UserName");
                Requires.NotNullOrEmpty(ftpArgument.Password, "Password");

                ftpArgument.LocalFilePath = Path.GetFullPath(ftpArgument.LocalFilePath);//相对路径转换成本地运行程序的绝对路径
                this.UploadFileAsync(ftpArgument.NeedCheckDirectory, ftpArgument.IP, ftpArgument.RemoteFilePath, ftpArgument.LocalFilePath,
                   ftpArgument.UserName, ftpArgument.Password, ftpArgument.IsUsePassive, callbackContext);
            }
            catch (Exception e)
            {
                m_Logger.Error(e);
                callbackContext.Error(new
                {
                    type = "Exception",
                    code = "",
                    message = e.Message,
                    details = e.StackTrace
                });
            }
        }

        /// <summary>
        /// 批量上传文件
        /// </summary>
        /// <param name="ftpArguments">ftp通信参数</param>
        /// <param name="callbackContext">回调上下文</param>
        [CordovaPluginAction]
        public async void UploadFilesAsync(FTPClientArgument[] ftpArguments, CallbackContext callbackContext)
        {
            try
            {
                Requires.NotNull(ftpArguments, nameof(ftpArguments));
                var ftpArgumentGroups = ftpArguments.GroupBy(x => x.IP);

                foreach (var ftpArgumentGroup in ftpArgumentGroups)
                {
                    var ftpClient = new FTPClient($"ftp://{ftpArgumentGroup.First().IP}");
                    foreach (var ftpArgument in ftpArgumentGroup)
                    {
                        await ftpClient.UploadFileAsync(ftpArgument.NeedCheckDirectory, ftpArgument.RemoteFilePath,
                            ftpArgument.LocalFilePath, ftpArgument.UserName, ftpArgument.Password, ftpArgument.IsUsePassive);
                    }
                }

                callbackContext.Success();
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

        #endregion

    }

    /// <summary>
    /// 创建文件或者列出子目录的参数
    /// </summary>
    public class DirectoryArgument
    {
        /// <summary>
        /// IP地址
        /// </summary>
        [JsonProperty("ip")]
        public string IP { get; set; }

        /// <summary>
        /// 远程服务器文件路径
        /// </summary>
        [JsonProperty("remoteFilePath")]
        public string RemoteFilePath { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        [JsonProperty("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// 是否被动模式
        /// </summary>
        [JsonProperty("isUsePassive")]
        public bool IsUsePassive { get; set; }
    }

    /// <summary>
    /// 重命名参数
    /// </summary>
    public class FTPRenameArgument
    {
        /// <summary>
        /// IP地址
        /// </summary>
        [JsonProperty("ip")]
        public string IP { get; set; }

        /// <summary>
        /// 远程服务器文件路径
        /// </summary>
        [JsonProperty("remoteFilePath")]
        public string RemoteFilePath { get; set; }

        /// <summary>
        /// 重命名文件新名字
        /// </summary>
        [JsonProperty("newFileName")]
        public string NewFileName { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        [JsonProperty("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// 是否被动模式
        /// </summary>
        [JsonProperty("isUsePassive")]
        public bool IsUsePassive { get; set; }
    }

    /// <summary>
    /// FTP通信客户端参数
    /// </summary>
    public class FTPClientArgument
    {
        /// <summary>
        /// 是否需要检查目录
        /// </summary>
        [JsonProperty("needCheckDirectory")]
        public bool NeedCheckDirectory { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        [JsonProperty("ip")]
        public string IP { get; set; }

        /// <summary>
        /// 远程服务器文件路径
        /// </summary>
        [JsonProperty("remoteFilePath")]
        public string RemoteFilePath { get; set; }

        /// <summary>
        /// 本地文件路径
        /// </summary>
        [JsonProperty("localFilePath")]
        public string LocalFilePath { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        [JsonProperty("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// 是否被动模式
        /// </summary>
        [JsonProperty("isUsePassive")]
        public bool IsUsePassive { get; set; }
    }
}
