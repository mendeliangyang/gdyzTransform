using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace gdyzTransform.Common
{
    public class TakeMsgByte
    {
        //            数据包格式：040402
        //报文头（0x02） +操作类型（0x25）+ 交易码（6个字节）+ 数据长度（4个字节）+ 报文内容（ buf ）+ 报文尾（0x03）

        internal static byte[] HandleMsgToByte(byte msgHead, byte operationType, String transactCode, dynamic obj)
        {
            if (null == transactCode || transactCode.Length != 6)
            {
                throw new Exception("交易码必须为6个字节");
            }

            List<byte> listByte = new List<byte>();
            listByte.Add(msgHead);
            listByte.Add(operationType);
            listByte.AddRange(Common.DatagramCoder.abcStrToToHexByte(transactCode));
            //listByte.AddRange(transactCode);
            int structSize = Marshal.SizeOf(obj);
            //string strSize= structSize.ToString("X").PadLeft(4, '0');
            //structSize = 288;
            //byte[]  byteStructSize= BitConverter.GetBytes(structSize);
            byte[] arry = new byte[4];
            arry[3] = (byte)(structSize & 0xFF);
            arry[2] = (byte)((structSize & 0xFF00) >> 8);
            arry[1] = (byte)((structSize & 0xFF0000) >> 16);
            arry[0] = (byte)((structSize >> 24) & 0xFF);

            if (true)
            {
                //memcpy(sbuf,"\x\x\x\x",4);  
            }
            //int u = (int)(byteStructSize[0] | byteStructSize[1] << 8 | byteStructSize[2] << 16 | byteStructSize[3] << 24);
            //byteStructSize[0] = (byte)(u);
            //byteStructSize[1] = (byte)(u >> 8);
            //byteStructSize[2] = (byte)(u >> 16);
            //byteStructSize[3] = (byte)(u >> 24);

            //for (int i = byteStructSize.Length; i < byteStructSize.Length; i--)
            //{
            //    if (true)
            //    {

            //    }
            //}
            //if (byteStructSize.Length<4)
            //{
            //    int whileFlag = 4 - byteStructSize.Length;
            //    for (int i = 0; i < whileFlag; i++)
            //    {
            //        byteStructSize.
            //    }
            //}
            listByte.AddRange(arry);
            listByte.AddRange(Common.DatagramCoder.StructToBytes(obj));
            listByte.Add(0x03);
            return listByte.ToArray();

        }
    }
}