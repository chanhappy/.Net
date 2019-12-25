using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SM2Tool
{
    /// <summary>
    /// 常用工具
    /// </summary>
    public static class Utils
    {
        #region Bytes<=>Char

        /// <summary>
        /// Bytes转Char
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static char BytesToChar(params byte[] b)
        {
            return (char)(((b[0] & 0xFF) << 8) | (b[1] & 0xFF));
        }

        /// <summary>
        /// Char转Bytes
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static byte[] CharToBytes(char c)
        {
            var result = new byte[2];
            result[0] = Convert.ToByte((c >> 8) & 0xFF);
            result[1] = Convert.ToByte(c & 0xFF);
            return result;
        }

        #endregion

        #region Bytes<=>HexString

        /// <summary>
        /// Bytes转HexString
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(params byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        /// <summary>
        /// HexString转Bytes
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// byte转换成HexString
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(this byte bytes)
        {
            return ByteArrayToHexString(bytes);
        }

        /// <summary>
        /// byte[]转换成HexString
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] bytes)
        {
            return ByteArrayToHexString(bytes);
        }

        /// <summary>
        /// HexString转换成byte
        /// </summary>
        /// <param name="hexString">2位字符串</param>
        /// <returns></returns>
        public static byte ToByte(this string hexString)
        {
            return HexStringToByteArray(hexString)[0];
        }

        /// <summary>
        /// HexString转换成byte[]
        /// </summary>
        /// <param name="hexString">2位字符串</param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string hexString)
        {
            return HexStringToByteArray(hexString);
        }
        #endregion

        #region  String<=>HexString

        /// <summary>
        /// 字符串转换为16进制字符串
        /// 1PAY.SYS.DDF01=>315041592E5359532E4444463031
        /// </summary>
        /// <param name="asciiString"></param>
        /// <returns></returns>
        public static string StringToHexString(string asciiString)
        {
            var hexSb = new StringBuilder();
            foreach (char c in asciiString)
            {
                var value = Convert.ToInt32(c);
                var chr = string.Format("{0:X2}", value);
                hexSb.Append(chr);
            }
            return hexSb.ToString();
        }

        /// <summary>
        /// 16进制字符串转换为字符串
        /// 315041592E5359532E4444463031=>1PAY.SYS.DDF01
        /// </summary>
        /// <param name="mHex"></param>
        /// <returns></returns>
        public static string HexStringToString(string mHex)
        {
            if (mHex == null)
                return null;

            if (string.IsNullOrWhiteSpace(mHex))
                return "";

            if (Convert.ToBoolean(mHex.Length & 1))
                throw new ArgumentException($"param length must be an even number。param length:{mHex.Length}");

            var sb = new StringBuilder();
            for (int i = 0; i < mHex.Length; i += 2)
            {
                var c = Convert.ToChar(int.Parse(mHex.Substring(i, 2), NumberStyles.HexNumber));
                sb.Append(c);
            }

            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// 判断指定bit位值是否为1
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index">位索引(b8索引为8)</param>
        /// <returns></returns>
        public static bool HasValue(this byte b, int index)
        {
            if (index > 8 || index < 1)
                throw new ArgumentOutOfRangeException();

            return Convert.ToBoolean((b >> (index - 1)) & 0x01);
        }

        /// <summary>
        /// 设置指定bit位值
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index">位索引(b8索引为8)</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte SetBit(this byte b, int index, bool value)
        {
            if (index > 8 || index < 1)
                throw new ArgumentOutOfRangeException();

            int v = index < 2 ? index : (2 << (index - 2));
            return value ? (byte)(b | v) : (byte)(b & ~v);
        }

        /// <summary>
        /// 获取指定位置的二进制字符串表示形式
        /// 例如：1E=>"00011110"
        /// </summary>
        /// <param name="b"></param>
        /// <param name="begin">bit高位</param>
        /// <param name="end">bit低位</param>
        /// <returns></returns>
        public static string GetBinaryString(this byte b, int begin = 8, int end = 1)
        {
            var sb = new StringBuilder();

            for (int i = begin; i >= end; i--)
            {
                var bChar = b.HasValue(i) ? "1" : "0";
                sb.Append(bChar);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取二进制的字符串表示形式
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetBinaryString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.GetBinaryString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// 16进制字符串转int
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static int HexStringToInt(string hexString)
        {
            var bytes = HexStringToByteArray(hexString).Reverse().ToArray();
            if (bytes.Length == 1)
            {
                return (int)bytes[0];
            }
            else if (bytes.Length == 2)
            {
                return BitConverter.ToInt16(bytes, 0);
            }
            else if (bytes.Length == 4)
            {
                return BitConverter.ToInt32(bytes, 0);
            }
            throw new ArgumentOutOfRangeException("hexString length is illegal.");
        }

        /// <summary>
        /// Int32转4字节Byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] IntToByteArray(int value)
        {
            var result = new List<byte>();
            var b0 = (byte)(value & 0xFF);
            result.Insert(0, b0);

            var b1 = (byte)(value >> 8 & 0xFF);
            if (b1 != 0) result.Insert(0, b1);

            var b2 = (byte)(value >> 16 & 0xFF);
            if (b2 != 0) result.Insert(0, b2);

            var b3 = (byte)(value >> 24 & 0xFF);
            if (b3 != 0) result.Insert(0, b3);

            return result.ToArray();
        }

        /// <summary>
        /// Int32转Hex字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string IntToHexString(int value)
        {
            return ByteArrayToHexString(IntToByteArray(value));
        }

        /// <summary>
        /// Int字符串转Hex字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string IntStringToHexString(string value)
        {
            return ByteArrayToHexString(IntToByteArray(int.Parse(value)));
        }
    }
}
