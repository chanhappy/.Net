using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Awp.Logging;
using Awp.SS.Cordova;
using ZXing;
using System.Drawing;
using System;
using System.Drawing.Imaging;
using ZXing.QrCode;
using ZXing.Multi.QrCode;
using ZXing.Common;
using System.Collections.Generic;

namespace Awp.SS.Cordova.Plugin.QRCodeUtils
{
    /// <summary>
    /// 二维码操作工具
    /// </summary>
    [CordovaPlugin]
    public class QRCodeUtils : AttributedCordovaPlugin
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(QRCodeUtils));

        /// <summary>
        /// 识别二维码
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="callbackContext"></param>
        [CordovaPluginAction]
        public void ReadQRCode(string fileName, CallbackContext callbackContext)
        {
            try
            {
                m_Logger.Debug($"[QRCodeUtils]ReadRQCode=>fileName:{fileName}");

                var qrCodeResult = new QRCodeResult();
                List<string> textList = new List<string>();

                if (!File.Exists(fileName))
                {
                    qrCodeResult.IsSuccess = false;
                    textList.Add("文件路径不存在");
                    qrCodeResult.Content = textList.ToArray();
                    callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, qrCodeResult));
                    return;
                }

                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    QRCodeMultiReader qc = new QRCodeMultiReader();
                    Image image = Image.FromStream(fs);
                    Bitmap bitmap = new Bitmap(image);
                    LuminanceSource source = new BitmapLuminanceSource(bitmap);
                    BinaryBitmap binarybitmap = new BinaryBitmap(new HybridBinarizer(source));
                    IDictionary<DecodeHintType, object> hints = new Dictionary<DecodeHintType, object>();
                    hints.Add(DecodeHintType.CHARACTER_SET, "UTF-8");
                    hints.Add(DecodeHintType.TRY_HARDER, "3");
                    Result[] decodeResult = qc.decodeMultiple(binarybitmap, hints);

                    m_Logger.Debug($"[QRCodeUtils]ReadRQCode=>decodeResult == null:{decodeResult == null}");
                    if(decodeResult == null || decodeResult.Length == 0)
                    {
                        qrCodeResult.IsSuccess = false;
                        textList.Add("无法识别图片中的二维码");
                        qrCodeResult.Content = textList.ToArray();
                        callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, qrCodeResult));
                        return;
                    }

                    foreach (Result res in decodeResult)
                    {
                        if(res.Text != null)
                        {
                            textList.Add(res.Text);
                        }
                    }
                }

                qrCodeResult.Content = textList.ToArray();

                m_Logger.Debug($"[QRCodeUtils]ReadRQCode=>textList.ToArray().Length:{textList.ToArray().Length}");

                if (textList.ToArray().Length == 0)
                {
                    qrCodeResult.IsSuccess = false;
                    callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, qrCodeResult));
                    return;
                }
                qrCodeResult.IsSuccess = true;
                callbackContext.SendPluginResult(new PluginResult(PluginResult.Status.OK, qrCodeResult));
            }
            catch (Exception err)
            {
                m_Logger.Error(err);
                callbackContext.Error(new
                {
                    type = err.GetType().Name,
                    code = "",
                    message = err.Message,
                    details = err.StackTrace
                });
            }
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="qrCodeOptions">生成二维码的选项</param>
        /// <param name="callbackContext"></param>
        [CordovaPluginAction]
        public void GenerateQRCode(QRCodeOptions qrCodeOptions, CallbackContext callbackContext)
        {
            try
            {
                BarcodeWriter writer = new BarcodeWriter();
                writer.Format = BarcodeFormat.QR_CODE;
                QrCodeEncodingOptions options = new QrCodeEncodingOptions();
                options.DisableECI = true;
                //设置内容编码
                options.CharacterSet = "UTF-8";
                //设置二维码的宽度和高度
                options.Width = qrCodeOptions.QRCodeWidth;
                options.Height = qrCodeOptions.QRCodeHeight;
                //设置二维码的边距,单位不是固定像素
                options.Margin = 1;
                writer.Options = options;
                Bitmap map = writer.Write(qrCodeOptions.QRCodeContent);
                map.Save(qrCodeOptions.QRCodePath, ImageFormat.Png);
                map.Dispose();
                callbackContext.Success();
            }
            catch (Exception err)
            {
                m_Logger.Error(err);
                callbackContext.Error(new
                {
                    type = err.GetType().Name,
                    code = "",
                    message = err.Message,
                    details = err.StackTrace
                });
            }
        }
    }

    /// <summary>
    /// 生成二维码的选项
    /// </summary>
    public class QRCodeOptions
    {
        /// <summary>
        /// 二维码的内容
        /// </summary>
        public string QRCodeContent;

        /// <summary>
        /// 二维码生成的路径
        /// </summary>
        public string QRCodePath;

        /// <summary>
        /// 二维码的宽
        /// </summary>
        public int QRCodeWidth;

        /// <summary>
        /// 二维码的高
        /// </summary>
        public int QRCodeHeight;
    }

    public class QRCodeResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess;

        /// <summary>
        /// 二维码识别结果
        /// </summary>
        public string[] Content;
    }
}