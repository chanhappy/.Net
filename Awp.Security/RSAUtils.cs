using Awp.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Awp.Security
{
    public class RSAUtils
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(SM2Utils));

        /// <summary>
        /// rsa加密
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name= "plainText">加密数据的明文</param>
        /// <param name= "plainText">密钥长度</param>
        public string RSAEncrypt(string publicKey, string plainText, int modulusLenth)
        {
            if (string.IsNullOrWhiteSpace(publicKey))
            {
                m_Logger.Debug("");
                throw new ArgumentException("[RSAEncrypt]PublicKey is empty");
            }

            if (string.IsNullOrWhiteSpace(plainText))
            {
                throw new ArgumentException("[RSAEncrypt]plainText is empty");
            }

            var hexString = Convert.ToBase64String(Utils.HexStringToByteArray(publicKey.Substring(14, modulusLenth/4)));
            var publicKeyXml = $"<RSAKeyValue><Modulus>{hexString}</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            byte[] data = Utils.HexStringToByteArray(plainText);
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKeyXml);
                var result = rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
                return Utils.ByteArrayToHexString(result);
            }
        }

        /// <summary>
        /// rsa解密
        /// </summary>
        /// <param name="privateKey">xml格式的私钥</param>
        /// <param name= "cipherText">加密数据的密文</param>
        public string RSADecrypt(string privateKeyXml, string cipherText)
        {
            if (string.IsNullOrWhiteSpace(privateKeyXml))
            {
                throw new ArgumentException("[RSAEncrypt]privateKeyXml is empty");
            }

            if (string.IsNullOrWhiteSpace(cipherText))
            {
                throw new ArgumentException("[RSAEncrypt]cipherText is empty");
            }
            
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKeyXml);
                var result = rsa.Decrypt(Utils.HexStringToByteArray(cipherText), RSAEncryptionPadding.Pkcs1);
                return Utils.ByteArrayToHexString(result);
            }
        }
    }
}
