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
            listByte.AddRange(Common.DatagramCoder.abcStrToToHexByte(Marshal.SizeOf(obj).ToString().PadLeft(4, '0')));
            listByte.AddRange(Common.DatagramCoder.StructToBytes(obj));
            listByte.Add(0x03);
            return listByte.ToArray();

        }
    }
}