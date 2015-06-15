using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace gdyzTransform.Common
{

    public class DatagramCoder
    {
        public enum SerializationType
        {
            XmlSerialize,
            BinaryFormatter,
            SoapFormatter
        }
        public enum EncodingMothord
        {
            Default,
            Unicode,
            UTF8,
            ASCII
        }
        [Flags]
        public enum ObjectTypeMark
        {
            CharacterString = 85,
            FileIdentification = 102,
            ObjectBinaryFormatter = 119,
            ObjectXmlSerialize = 120,
            ObjectSoapFormatter = 121,
            StructObject = 135,
            XmlDocumentInnerText = 136
        }
        private DatagramCoder.EncodingMothord encodingMothord;
        internal DatagramCoder(DatagramCoder.EncodingMothord mothord)
        {
            this.encodingMothord = mothord;
        }
        public virtual string BytesToString(byte[] dataBytes, int start, int size)
        {
            string @string;
            switch (this.encodingMothord)
            {
                case DatagramCoder.EncodingMothord.Default:
                    @string = Encoding.Default.GetString(dataBytes, start, size);
                    break;
                case DatagramCoder.EncodingMothord.Unicode:
                    @string = Encoding.Unicode.GetString(dataBytes, start, size);
                    break;
                case DatagramCoder.EncodingMothord.UTF8:
                    @string = Encoding.UTF8.GetString(dataBytes, start, size);
                    break;
                case DatagramCoder.EncodingMothord.ASCII:
                    @string = Encoding.ASCII.GetString(dataBytes, start, size);
                    break;
                default:
                    throw new IndexOutOfRangeException("未定义的编码格式");
            }
            return @string;
        }
        public void SaveFile(string fileName, byte[] recvData, int start, int size)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
            fileStream.Write(recvData, start, size);
            fileStream.Flush();
            fileStream.Close();
        }
        public virtual object BytesToObjectByBinaryFormatter(byte[] dataBytes, int start, int size)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(dataBytes, start, size);
            memoryStream.Position = 0L;
            object result = binaryFormatter.Deserialize(memoryStream);
            memoryStream.Close();
            return result;
        }
        public virtual object BytesToObjectByXmlSerializer(byte[] dataBytes, int start, int size, Type type)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(type);
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(dataBytes, start, size);
            memoryStream.Position = 0L;
            object result = xmlSerializer.Deserialize(memoryStream);
            memoryStream.Close();
            return result;
        }
        public virtual XmlDocument BytesToXmlDocument(byte[] dataBytes, int start, int size)
        {
            XmlDocument result;
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Write(dataBytes, start, size);
                memoryStream.Position = 0L;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlDocument));
                XmlDocument xmlDocument = (XmlDocument)xmlSerializer.Deserialize(memoryStream);
                memoryStream.Flush();
                memoryStream.Close();
                result = xmlDocument;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        internal static int CharToInt(byte[] length, int start, int size)
        {
            return int.Parse(Encoding.Default.GetString(length, start, size));
        }
        internal static int CharToIntBoEing(byte[] length, int start, int size)
        {
            return int.Parse(Encoding.UTF8.GetString(length, start, size));
        }
        internal static int CharToInt(char[] length)
        {
            return int.Parse(new string(length));
        }
        internal static char[] IntToChar(int length, int charLength)
        {
            return length.ToString().PadLeft(charLength, '0').ToCharArray();
        }
        internal static byte[] StructToBytes(object obj)
        {
            int num = Marshal.SizeOf(obj);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr(obj, intPtr, false);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }
        internal static object BytesToStruct(byte[] bytes, Type type)
        {
            int num = Marshal.SizeOf(type);
            object result;
            if (num > bytes.Length)
            {
                result = null;
            }
            else
            {
                IntPtr intPtr = Marshal.AllocHGlobal(num);
                Marshal.Copy(bytes, 0, intPtr, num);
                object obj = Marshal.PtrToStructure(intPtr, type);
                Marshal.FreeHGlobal(intPtr);
                result = obj;
            }
            return result;
        }
        internal virtual byte[] StringToBytes(string datagram)
        {
            int num = Encoding.Default.GetBytes(datagram).Length;
            byte[] array = new byte[num + 5];
            array[0] = 85;
            array[1] = 1;
            array[2] = (byte)(num / 65536);
            array[3] = (byte)(num / 256);
            array[4] = (byte)(num % 256);
            byte[] result;
            switch (this.encodingMothord)
            {
                case DatagramCoder.EncodingMothord.Default:
                    Encoding.Default.GetBytes(datagram, 0, datagram.Length, array, 5);
                    result = array;
                    break;
                case DatagramCoder.EncodingMothord.Unicode:
                    Encoding.Unicode.GetBytes(datagram, 0, datagram.Length, array, 5);
                    result = array;
                    break;
                case DatagramCoder.EncodingMothord.UTF8:
                    Encoding.UTF8.GetBytes(datagram, 0, datagram.Length, array, 5);
                    result = array;
                    break;
                case DatagramCoder.EncodingMothord.ASCII:
                    Encoding.ASCII.GetBytes(datagram, 0, datagram.Length, array, 5);
                    result = array;
                    break;
                default:
                    throw new Exception("未定义的编码格式");
            }
            return result;
        }
        internal virtual byte[] XmlDatagramToBytes(string datagram)
        {
            int num = Encoding.Default.GetBytes(datagram).Length;
            byte[] array = new byte[num];
            byte[] result;
            switch (this.encodingMothord)
            {
                case DatagramCoder.EncodingMothord.Default:
                    Encoding.Default.GetBytes(datagram, 0, datagram.Length, array, 0);
                    result = array;
                    break;
                case DatagramCoder.EncodingMothord.Unicode:
                    Encoding.Unicode.GetBytes(datagram, 0, datagram.Length, array, 0);
                    result = array;
                    break;
                case DatagramCoder.EncodingMothord.UTF8:
                    {
                        int num2 = Encoding.UTF8.GetBytes(datagram).Length;
                        byte[] array2 = new byte[num2];
                        Encoding.UTF8.GetBytes(datagram, 0, datagram.Length, array2, 0);
                        result = array2;
                        break;
                    }
                case DatagramCoder.EncodingMothord.ASCII:
                    Encoding.ASCII.GetBytes(datagram, 0, datagram.Length, array, 0);
                    result = array;
                    break;
                default:
                    throw new Exception("未定义的编码格式");
            }
            return result;
        }
        internal virtual byte[] FileToBytes(string filePath)
        {
            if (File.Exists(filePath))
            {
                string fileName = Path.GetFileName(filePath);
                byte[] array = this.StringToBytes(fileName);
                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                byte[] array2 = new byte[fileStream.Length + 2L + (long)array.Length];
                array2[0] = 102;
                array2[1] = 1;
                array.CopyTo(array2, 2);
                fileStream.Read(array2, 2 + array.Length, (int)fileStream.Length);
                fileStream.Flush();
                fileStream.Close();
                return array2;
            }
            throw new FileNotFoundException("指定的文件路径不存在！", filePath);
        }
        internal virtual byte[] ObjectToBytesByBinaryFormatter(object myObject, byte byteObjectIndex)
        {
            byte[] result;
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                MemoryStream memoryStream = new MemoryStream();
                binaryFormatter.Serialize(memoryStream, myObject);
                memoryStream.Position = 0L;
                byte[] array = new byte[(int)memoryStream.Length + 2];
                array[0] = 119;
                array[1] = byteObjectIndex;
                memoryStream.Read(array, 2, (int)memoryStream.Length);
                memoryStream.Flush();
                memoryStream.Close();
                result = array;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        internal virtual byte[] ObjectToBytesByXmlSerializer(object obj, Type objType, byte byteObjectIndex)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(objType);
            MemoryStream memoryStream = new MemoryStream();
            xmlSerializer.Serialize(memoryStream, obj);
            memoryStream.Position = 0L;
            byte[] array = new byte[(int)memoryStream.Length + 2];
            array[0] = 120;
            array[1] = byteObjectIndex;
            memoryStream.Read(array, 2, (int)memoryStream.Length);
            memoryStream.Flush();
            memoryStream.Close();
            return array;
        }
        internal virtual byte[] XmlTextToBytes(string xmlText)
        {
            int num = Encoding.Default.GetBytes(xmlText).Length;
            byte[] array = new byte[num + 2];
            array[0] = 136;
            array[1] = 1;
            byte[] result;
            switch (this.encodingMothord)
            {
                case DatagramCoder.EncodingMothord.Default:
                    Encoding.Default.GetBytes(xmlText, 0, xmlText.Length, array, 2);
                    result = array;
                    break;
                case DatagramCoder.EncodingMothord.Unicode:
                    Encoding.Unicode.GetBytes(xmlText, 0, xmlText.Length, array, 2);
                    result = array;
                    break;
                case DatagramCoder.EncodingMothord.UTF8:
                    Encoding.UTF8.GetBytes(xmlText, 0, xmlText.Length, array, 2);
                    result = array;
                    break;
                case DatagramCoder.EncodingMothord.ASCII:
                    Encoding.ASCII.GetBytes(xmlText, 0, xmlText.Length, array, 2);
                    result = array;
                    break;
                default:
                    throw new Exception("未定义的编码格式");
            }
            return result;
        }

        /// <summary> 
        /// 字符串转16进制字节数组 
        /// </summary> 
        /// <param name="hexString"></param> 
        /// <returns></returns> 
       public static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }


       private string StringToHexString(string s, Encoding encode)
       {
           byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
           string result = string.Empty;
           for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符，以%隔开
           {
               result += "%" + Convert.ToString(b[i], 16);
           }
           return result;
       }
       private string HexStringToString(string hs, Encoding encode)
       {
           //以%分割字符串，并去掉空字符
           string[] chars = hs.Split(new char[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
           byte[] b = new byte[chars.Length];
           //逐个字符变为16进制字节数据
           for (int i = 0; i < chars.Length; i++)
           {
               b[i] = Convert.ToByte(chars[i], 16);
           }
           //按照指定编码将字节数组变为字符串
           return encode.GetString(b);
       }




       /// <summary>
       /// 字符串转16进制字节数组
       /// </summary>
       /// <param name="hexString"></param>
       /// <returns></returns>
       public static byte[] abcStrToToHexByte(string hexString)
       {
           hexString = hexString.Replace(" ", "");
           if ((hexString.Length % 2) != 0)
               hexString += " ";
           byte[] returnBytes = new byte[hexString.Length];
           for (int i = 0; i < returnBytes.Length; i++)
               returnBytes[i] = Convert.ToByte(hexString.Substring(i , 1), 16);
           return returnBytes;
       }






       /// <summary>
       /// 字节数组转16进制字符串
       /// </summary>
       /// <param name="bytes"></param>
       /// <returns></returns>
       public static string byteToHexStr(byte[] bytes)
       {
           string returnStr = "";
           if (bytes != null)
           {
               for (int i = 0; i < bytes.Length; i++)
               {
                   returnStr += bytes[i].ToString("X2");
               }
           }
           return returnStr;
       }

 





        /// <summary> 
        /// 从汉字转换到16进制 
        /// </summary> 
        /// <param name="s"></param> 
        /// <param name="charset">编码,如"utf-8","gb2312"</param> 
        /// <param name="fenge">是否每字符用逗号分隔</param> 
        /// <returns></returns> 
        public static string ToHex(string s, string charset, bool fenge)
        {
            if ((s.Length % 2) != 0)
            {
                s += " ";//空格 
                //throw new ArgumentException("s is not valid chinese string!"); 
            }
            System.Text.Encoding chs = System.Text.Encoding.GetEncoding(charset);
            byte[] bytes = chs.GetBytes(s);
            string str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                str += string.Format("{0:X}", bytes[i]);
                if (fenge && (i != bytes.Length - 1))
                {
                    str += string.Format("{0}", ",");
                }
            }
            return str.ToLower();
        }

        ///<summary> 
        /// 从16进制转换成汉字 
        /// </summary> 
        /// <param name="hex"></param> 
        /// <param name="charset">编码,如"utf-8","gb2312"</param> 
        /// <returns></returns> 
        public static string UnHex(string hex, string charset)
        {
            if (hex == null)
                throw new ArgumentNullException("hex");
            hex = hex.Replace(",", "");
            hex = hex.Replace("\n", "");
            hex = hex.Replace("\\", "");
            hex = hex.Replace(" ", "");
            if (hex.Length % 2 != 0)
            {
                hex += "20";//空格 
            }
            // 需要将 hex 转换成 byte 数组。 
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    // 每两个字符是一个 byte。 
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2),
                    System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    // Rethrow an exception with custom message. 
                    throw new ArgumentException("hex is not a valid hex number!", "hex");
                }
            }
            System.Text.Encoding chs = System.Text.Encoding.GetEncoding(charset);
            return chs.GetString(bytes);
        }

    }
}
