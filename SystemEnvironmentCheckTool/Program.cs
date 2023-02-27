using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace SystemEnvironmentCheckTool
{
    public class Program
    {
        private static readonly ILog m_Log = LogManager.GetLogger("Program");
        public static bool isSystemEnvironmentOk = true;

        //exe的名称                    注册表中的关联名称
        //vcredist_2005_SP1_x86.exe   Microsoft Visual C++ 2005 Redistributable
        //vcredist_2008_SP1_x86.exe   Microsoft Visual C++ 2008 Redistributable - x86 9.0.30729.17
        //vcredist_2010_x86.exe       Microsoft Visual C++ 2010  x86 Redistributable - 10.0.30319
        //vcredist_2013_x86.exe       Microsoft Visual C++ 2013 x86 Minimum Runtime - 12.0.21005
        //vcredist_2015_x86_u3.exe    Microsoft Visual C++ 2015-2019 Redistributable (x86) - 14.29.30133
        //SDK320.msi                  XFS 3.20 SDK
        //NDP46-KB3045557-x86-x64-AllOS-ENU.exe  Microsoft .NET Framework

        static void Main(string[] args)
        {
            m_Log.Debug("====================获取自助应用所需环境依赖====================");

            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            var dependencies = new List<string>();
            var frameworkDependencies = new List<string>();
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        var name = subkey.GetValue("DisplayName");
                        if (name == null)
                        {
                            continue;
                        }

                        if (name.ToString().StartsWith("Microsoft Visual C++") || name.ToString().StartsWith("XFS"))
                        {
                            dependencies.Add(name.ToString());
                            m_Log.Debug($"系统已安装自助应用所需依赖：{name}");
                        }

                        if(name.ToString().StartsWith("Microsoft .NET Framework"))
                        {
                            frameworkDependencies.Add(name.ToString());
                            m_Log.Debug($"系统已安装自助应用所需依赖：{name}");
                        }
                    }
                }
            }

            m_Log.Debug("====================检查自助应用所需环境依赖====================");

            if (!dependencies.Contains("Microsoft Visual C++ 2005 Redistributable"))
            {
               m_Log.Error("请检查系统环境是否安装 Microsoft Visual C++ 2005 (x86)");
               isSystemEnvironmentOk = false;
            }
            if (!dependencies.Contains("Microsoft Visual C++ 2008 Redistributable - x86 9.0.30729.17"))
            {
               m_Log.Error("请检查系统环境是否安装 Microsoft Visual C++ 2008 (x86)");
               isSystemEnvironmentOk = false;
            }
            if (!dependencies.Contains("Microsoft Visual C++ 2010  x86 Redistributable - 10.0.30319"))
            {
               m_Log.Error("请检查系统环境是否安装 Microsoft Visual C++ 2010 (x86)");
               isSystemEnvironmentOk = false;
            }
            if (!dependencies.Contains("Microsoft Visual C++ 2013 x86 Minimum Runtime - 12.0.21005"))
            {
               m_Log.Error("请检查系统环境是否安装 Microsoft Visual C++ 2013 (x86)");
               isSystemEnvironmentOk = false;
            }
            if (!dependencies.Contains("Microsoft Visual C++ 2015-2019 Redistributable (x86) - 14.29.30133"))
            {
               m_Log.Error("请检查系统环境是否安装 Microsoft Visual C++ 2015 (x86)");
               isSystemEnvironmentOk = false;
            }
            if (!dependencies.Contains("XFS 3.20 SDK"))
            {
               m_Log.Error("请检查系统环境是否安装 XFS 3.20 SDK");
               isSystemEnvironmentOk = false;
            }
            if (frameworkDependencies.Count == 0)
            {
               m_Log.Error("请检查系统环境是否安装 .NET Framework");
               isSystemEnvironmentOk = false;
            }

            m_Log.Debug("====================检查自助应用所需环境依赖的结果展示====================");

            if (isSystemEnvironmentOk == false)
            {
                MessageBox.Show("环境依赖有问题，请参考日志报错信息检查环境依赖");
                m_Log.Error("环境依赖有问题，请参考日志报错信息检查环境依赖");
            }
            else
            {
                MessageBox.Show("环境依赖没有问题");
                m_Log.Debug("环境依赖没有问题");
            }
        }
    }
}
