using System;
using System.Collections.Generic;
using Awp.Common;
using Awp.Logging;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace Awp.SS.Cordova.Plugin
{
    /// <summary>
    /// SFTP工具类
    /// </summary>
    [CordovaPlugin]
    public class SFTPUtils : AttributedCordovaPlugin
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(SFTPUtils));
        private static SFTPClient _sftpClient;
        private static string _host;
        private static string _username;
        private static string _password;

        /// <summary>
        /// SFTP重命名参数
        /// </summary>
        public class SFTPRenameArgument
        {
            /// <summary>
            /// sftp的host
            /// </summary>
            [JsonProperty("remoteHost")]
            public string RemoteHost { get; set; }

            /// <summary>
            /// 旧文件路径，包括文件名及后缀
            /// </summary>
            [JsonProperty("remotePathWithOldFile")]
            public string RemotePathWithOldFile { get; set; }

            /// <summary>
            /// 新文件路径，包括文件名及后缀
            /// </summary>
            [JsonProperty("remotePathWithNewFile")]
            public string RemotePathWithNewFile { get; set; }

            /// <summary>
            /// 用户名
            /// </summary>
            [JsonProperty("userName")]
            public string UserName { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            [JsonProperty("password")]
            public string Password { get; set; }
        }

        /// <summary>
        /// SFTP下载文件参数
        /// </summary>
        public class SFTPDownloadArgument
        {
            /// <summary>
            /// sftp的host
            /// </summary>
            [JsonProperty("remoteHost")]
            public string RemoteHost { get; set; }

            /// <summary>
            /// 下载文件的完整路径，包括文件名称及后缀
            /// </summary>
            [JsonProperty("remotePathWithFileName")]
            public string RemotePathWithFileName { get; set; }

            /// <summary>
            /// 下载文件存放的本地路径
            /// </summary>
            [JsonProperty("localPathWithoutFileName")]
            public string LocalPathWithoutFileName { get; set; }

            /// <summary>
            /// 用户名
            /// </summary>
            [JsonProperty("userName")]
            public string UserName { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            [JsonProperty("password")]
            public string Password { get; set; }
        }

        /// <summary>
        /// SFTP上传文件参数
        /// </summary>
        public class SFTPUploadArgument
        {
            /// <summary>
            /// 是否需要检查目录
            /// </summary>
            [JsonProperty("isNeedCheckDirectory")]
            public bool IsNeedCheckDirectory { get; set; }

            /// <summary>
            /// sftp的host
            /// </summary>
            [JsonProperty("remoteHost")]
            public string RemoteHost { get; set; }

            /// <summary>
            /// 上传的资源文件
            /// </summary>
            [JsonProperty("localPathWithFileName")]
            public string LocalPathWithFileName { get; set; }

            /// <summary>
            /// 基础目录:"/home/ctm/voucherimages/"
            /// </summary>
            [JsonProperty("baseDirectory")]
            public string BaseDirectory { get; set; }

            /// <summary>
            /// 上传文件的远程路径（不含基础路径）
            /// </summary>
            [JsonProperty("targetPath")]
            public string TargetPath { get; set; }

            /// <summary>
            /// 用户名
            /// </summary>
            [JsonProperty("userName")]
            public string UserName { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            [JsonProperty("password")]
            public string Password { get; set; }

            /// <summary>
            /// 上传的文件名字
            /// </summary>
            [JsonProperty("upFileName")]
            public string UpFileName { get; set; }

            /// <summary>
            /// 是否需要修改文件权限， 0-不修改， 其他-修改
            /// </summary>
            [JsonProperty("needChange")]
            public string NeedChage { get; set; }

            /// <summary>
            /// 需要修改的权限，如"777", "755"等
            /// </summary>
            [JsonProperty("permission")]
            public string Permission { get; set; }
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="renameArgument">SFTP重命名参数</param>
        /// <param name="callbackContext">回调上下文</param>
        [CordovaPluginAction]
        public void RenameFile(SFTPRenameArgument renameArgument, CallbackContext callbackContext)
        {
            try
            {
                var sftpResult = RenameFile(renameArgument);
                callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, sftpResult));
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

        private FtpStatusCode RenameFile(SFTPRenameArgument renameArgument)
        {
            m_Logger.Debug($"renameArgument:{renameArgument}");

            Requires.NotNull(renameArgument, "renameArgument");
            Requires.NotNullOrEmpty(renameArgument.RemoteHost, "remoteHost");
            Requires.NotNullOrEmpty(renameArgument.RemotePathWithOldFile, "remotePathWithOldFile");
            Requires.NotNullOrEmpty(renameArgument.RemotePathWithNewFile, "remotePathWithNewFile");
            Requires.NotNullOrEmpty(renameArgument.UserName, "userName");
            Requires.NotNullOrEmpty(renameArgument.Password, "password");
            _sftpClient = GetInstance(renameArgument.RemoteHost, renameArgument.UserName, renameArgument.Password);
            var sftpResult = _sftpClient.RenameFileName(renameArgument.RemotePathWithOldFile, renameArgument.RemotePathWithNewFile);
            return sftpResult;
        }
        
        /// <summary>
        /// 重命名文件(兼容旧版本)
        /// </summary>
        /// <param name="renameArgument"></param>
        /// <param name="callbackContext"></param>
        [CordovaPluginAction]
        public void RenameFileLegacy(SFTPRenameArgument renameArgument, CallbackContext callbackContext)
        {
            try
            {
                var sftpResult = RenameFile(renameArgument);
                _sftpClient.Close();
                _sftpClient = null;
                callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, sftpResult));
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

        /// <summary>
        /// SFTP下载文件
        /// </summary>
        /// <param name="downloadArgument">SFTP下载文件参数</param>
        /// <param name="callbackContext"></param>
        [CordovaPluginAction]
        public void DownloadFile(SFTPDownloadArgument downloadArgument, CallbackContext callbackContext)
        {
            try
            {
                var sftpResult = this.DownloadFile(downloadArgument);
                callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, sftpResult));
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

        private FtpStatusCode DownloadFile(SFTPDownloadArgument downloadArgument)
        {
            m_Logger.Debug($"downloadArgument:{downloadArgument}");
            string[] paths = downloadArgument.RemotePathWithFileName.Split('/');
            string fileName = paths[paths.Length - 1];
            Requires.NotNull(downloadArgument, "downloadArgument");
            Requires.NotNullOrEmpty(downloadArgument.RemoteHost, "remoteHost");
            Requires.NotNullOrEmpty(downloadArgument.RemotePathWithFileName, "remotePathWithFileName");
            Requires.NotNullOrEmpty(downloadArgument.LocalPathWithoutFileName, "localPathWithoutFileName");
            Requires.NotNullOrEmpty(downloadArgument.UserName, "userName");
            Requires.NotNullOrEmpty(downloadArgument.Password, "password");
            m_Logger.Debug("本地文件路径:" + downloadArgument.LocalPathWithoutFileName + "\\" + fileName);
            if (File.Exists(downloadArgument.LocalPathWithoutFileName + "\\" + fileName))
            {
                File.Delete(downloadArgument.LocalPathWithoutFileName + "\\" + fileName);
            }
            _sftpClient = GetInstance(downloadArgument.RemoteHost, downloadArgument.UserName, downloadArgument.Password);
            var sftpResult = _sftpClient.DownloadFile(downloadArgument.RemotePathWithFileName, downloadArgument.LocalPathWithoutFileName);
            return sftpResult;
        }

        /// <summary>
        /// SFTP下载单个文件(兼容旧版本)
        /// </summary>
        /// <param name="downloadArgument"></param>
        /// <param name="callbackContext"></param>
        [CordovaPluginAction]
        public void DownloadFileLegacy(SFTPDownloadArgument downloadArgument, CallbackContext callbackContext)
        {
            try
            {
                var sftpResult = DownloadFile(downloadArgument);
                _sftpClient.Close();
                _sftpClient = null;
                callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, sftpResult));
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

        /// <summary>
        /// SFTP批量下载文件
        /// </summary>
        /// <param name="downloadArguments"></param>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="callbackContext"></param>
        [CordovaPluginAction]
        public void DownloadFiles(SFTPDownloadArgument[] downloadArguments, string host, string userName, string password, CallbackContext callbackContext)
        {
            try
            {
                Requires.NotNullOrEmpty(host, "remoteHost");
                Requires.NotNullOrEmpty(userName, "userName");
                Requires.NotNullOrEmpty(password, "password");
                foreach (SFTPDownloadArgument downloadArgument in downloadArguments)
                {
                    string[] paths = downloadArgument.RemotePathWithFileName.Split('/');
                    string fileName = paths[paths.Length - 1];
                    m_Logger.Debug($"downloadArgument:{downloadArgument}");
                    Requires.NotNull(downloadArgument, "downloadArgument");
                    Requires.NotNullOrEmpty(downloadArgument.RemotePathWithFileName, "remotePathWithFileName");
                    Requires.NotNullOrEmpty(downloadArgument.LocalPathWithoutFileName, "localPathWithoutFileName");
                    if (File.Exists(downloadArgument.LocalPathWithoutFileName + "\\" + fileName))
                    {
                        File.Delete(downloadArgument.LocalPathWithoutFileName + "\\" + fileName);
                    }
                }
                List<FtpStatusCode> result = new List<FtpStatusCode>();
                _sftpClient = GetInstance(host, userName, password);
                foreach (SFTPDownloadArgument downloadArgument in downloadArguments)
                {
                    var sftpResult = _sftpClient.DownloadFile(downloadArgument.RemotePathWithFileName, downloadArgument.LocalPathWithoutFileName);
                    result.Add(sftpResult);
                }
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

        /// <summary>
        /// SFTP上传文件
        /// </summary>
        /// <param name="uploadArgument">SFTP上传文件参数</param>
        /// <param name="callbackContext"></param>
        [CordovaPluginAction]
        public void UploadFile(SFTPUploadArgument uploadArgument, CallbackContext callbackContext)
        {
            try
            {
                var sftpResult = UploadFile(uploadArgument);
                callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, sftpResult));
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
        /// SFTP上传单个文件(兼容旧版本)
        /// </summary>
        /// <param name="uploadArgument"></param>
        /// <param name="callbackContext"></param>
        [CordovaPluginAction]
        public void UploadFileLegacy(SFTPUploadArgument uploadArgument, CallbackContext callbackContext)
        {
            try
            {
                var sftpResult = UploadFile(uploadArgument);
                _sftpClient.Close();
                _sftpClient = null;
                callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, sftpResult));
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

        private FtpStatusCode UploadFile(SFTPUploadArgument uploadArgument)
        {
            m_Logger.Debug($"uploadArgument:{uploadArgument}");

            Requires.NotNull(uploadArgument, "uploadArgument");
            Requires.NotNullOrEmpty(uploadArgument.RemoteHost, "remoteHost");
            Requires.NotNullOrEmpty(uploadArgument.LocalPathWithFileName, "localPathWithFileName");
            Requires.NotNullOrEmpty(uploadArgument.BaseDirectory, "baseDirectory");
            Requires.NotNullOrEmpty(uploadArgument.TargetPath, "targetPath");
            Requires.NotNullOrEmpty(uploadArgument.UserName, "userName");
            Requires.NotNullOrEmpty(uploadArgument.Password, "password");
            _sftpClient = GetInstance(uploadArgument.RemoteHost, uploadArgument.UserName, uploadArgument.Password);
            var sftpResult = _sftpClient.UploadFile(uploadArgument.IsNeedCheckDirectory, uploadArgument.LocalPathWithFileName, uploadArgument.BaseDirectory, uploadArgument.TargetPath, uploadArgument.UpFileName);
            if (!string.IsNullOrEmpty(uploadArgument.NeedChage) && uploadArgument.NeedChage != "0")
            {
                short newPermission = Convert.ToInt16(uploadArgument.Permission);
                string _baseDir = uploadArgument.BaseDirectory + "/";
                string[] paths = uploadArgument.TargetPath.Split('/');
                foreach (string path in paths)
                {
                    _baseDir = _baseDir + path + "/";
                    _sftpClient.changePermission(_baseDir, newPermission);
                }
                //  如果未传入上传后的文件名，使用源文件的名称
                string fileName = string.IsNullOrWhiteSpace(uploadArgument.UpFileName) ? Path.GetFileName(uploadArgument.LocalPathWithFileName) : uploadArgument.UpFileName;
                _sftpClient.changePermission(_baseDir + "/" + fileName, newPermission);
            }
            return sftpResult;
        }

        /// <summary>
        /// SFTP批量上传文件
        /// </summary>
        /// <param name="uploadArguments"></param>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="callbackContext"></param>
        [CordovaPluginAction]
        public void UploadFiles(SFTPUploadArgument[] uploadArguments, string host, string username, string password, CallbackContext callbackContext)
        {
            try
            {
                Requires.NotNullOrEmpty(host, "remoteHost");
                Requires.NotNullOrEmpty(username, "userName");
                Requires.NotNullOrEmpty(password, "password");
                foreach (SFTPUploadArgument uploadArgument in uploadArguments)
                {
                    m_Logger.Debug($"uploadArgument:{uploadArgument}");
                    Requires.NotNull(uploadArgument, "uploadArgument");
                    Requires.NotNullOrEmpty(uploadArgument.LocalPathWithFileName, "localPathWithFileName");
                    Requires.NotNullOrEmpty(uploadArgument.BaseDirectory, "baseDirectory");
                    Requires.NotNullOrEmpty(uploadArgument.TargetPath, "targetPath");
                }
                List<FtpStatusCode> result = new List<FtpStatusCode>();
                _sftpClient = GetInstance(host, username, password);
                foreach (SFTPUploadArgument uploadArgument in uploadArguments)
                {
                    var sftpResult = _sftpClient.UploadFile(uploadArgument.IsNeedCheckDirectory, uploadArgument.LocalPathWithFileName, uploadArgument.BaseDirectory, uploadArgument.TargetPath, uploadArgument.UpFileName);
                    if (!string.IsNullOrEmpty(uploadArgument.NeedChage) && uploadArgument.NeedChage != "0")
                    {
                        short newPermission = Convert.ToInt16(uploadArgument.Permission);
                        string _baseDir = uploadArgument.BaseDirectory + "/";
                        string[] paths = uploadArgument.TargetPath.Split('/');
                        foreach (string path in paths)
                        {
                            _baseDir = _baseDir + path + "/";
                            _sftpClient.changePermission(_baseDir, newPermission);
                        }
                        _sftpClient.changePermission(_baseDir + uploadArgument.UpFileName, newPermission);
                    }
                    result.Add(sftpResult);
                }
                callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, result));
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

        private SFTPClient GetInstance(string host, string username, string password)
        {
            if (_sftpClient == null || _host != host || _username != username || _password != password)
            {
                _host = host;
                _username = username;
                _password = password;
                _sftpClient = new SFTPClient(host, username, password);
            }
            return _sftpClient;
        }

        /// <summary>
        /// 断开SFTP连接
        /// </summary>
        [CordovaPluginAction]
        public void Disconnect(CallbackContext callbackContext)
        {
            if (_sftpClient != null)
            {
                _host = "";
                _username = "";
                _password = "";
                _sftpClient.Close();
            }
            callbackContext.Success();
        }
    }
}
