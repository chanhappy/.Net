using log4net;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Text;
using System.Windows;

namespace SM2Tool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(MainWindow));

        SM2Utils sm2Utils;
        public MainWindow()
        {
            InitializeComponent();
            sm2Utils = new SM2Utils();
        }

        private void GenerateKeysBtn_Click(object sender, RoutedEventArgs e)
        {
            var sm2 = new SM2Utils();
            AsymmetricCipherKeyPair key = sm2.pairGenerator.GenerateKeyPair();
            ECPrivateKeyParameters ecpriv = (ECPrivateKeyParameters)key.Private;
            ECPublicKeyParameters ecpub = (ECPublicKeyParameters)key.Public;
            var priv = ecpriv.D;
            var pub = ecpub.Q;

            var publicKeyStr = Utils.ByteArrayToHexString(pub.GetEncoded());
            //publicKey.Text = publicKeyStr + Environment.NewLine + "publicKey:Length:" + publicKeyStr.Length;
            publicKey.Text = publicKeyStr;

            var privateKeyStr = Utils.ByteArrayToHexString(priv.ToByteArray());
            //privateKey.Text = privateKeyStr + Environment.NewLine + "privateKey:Length:" + privateKeyStr.Length;
            privateKey.Text = privateKeyStr;
            tbPrivateKey.Text = privateKey.Text;
            tbPublicKey.Text = publicKey.Text;

            m_Logger.Debug("公钥: " + Encoding.ASCII.GetString(Hex.Encode(pub.GetEncoded())).ToUpper());
            m_Logger.Debug("私钥: " + Encoding.ASCII.GetString(Hex.Encode(priv.ToByteArray())).ToUpper());
        }

        private void EncryptBtn_Click(object sender, RoutedEventArgs e)
        {
            var publicKey = tbPublicKey.Text;
            var plainText = tbPlainText.Text;
            tbEncryptResult.Text = sm2Utils.Sm2Encrypt(publicKey, plainText);
        }

        private void DecryptBtn_Click(object sender, RoutedEventArgs e)
        {
            var privateKey = tbPrivateKey.Text;
            var cipherText = tbCipherText.Text;
            tbDecryptResult.Text = sm2Utils.Sm2Decrypt(privateKey, cipherText);
        }
    }
}
