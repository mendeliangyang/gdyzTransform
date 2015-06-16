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

        internal static uint ByteToInt(byte[] bytes)
        {
            return (uint)(bytes[0] | bytes[1] << 8 | bytes[2] << 16 | bytes[3] << 24);
        }



        /// <summary>
        /// 根据格式检查报文是否正确，并返回out byteData数据消息buf
        /// </summary>
        /// <param name="bytes">source</param>
        /// <param name="byteHead">报文头</param>
        /// <param name="byteOperator">操作类型</param>
        /// <param name="byteDealCode">交易码</param>
        /// <param name="byteData">报文内容</param>
        /// <returns>false报文格式错误，true  byteDate中保存数据</returns>
        internal static bool CheckMsgForm(byte[] bytes, byte byteHead, byte byteOperator, byte[] byteDealCode,out byte[] byteData)
        {
            byte[] byteTemp=null;
            byteData = null;
            int iEffect = 0;
            if (bytes == null || bytes.Length < 2)
            {
                return false;
            }
            if (bytes[iEffect] != byteHead)
            {
                return false;
            }
            iEffect++;
            if (bytes[1] != byteOperator)
            {
                return false;
            }
            iEffect++;
            if (byteDealCode != null)
            {
                int iDealCodeLen = byteDealCode.Length;
                byteTemp = new byte[iDealCodeLen];
                Array.Copy(bytes, iEffect, byteTemp, 0, iDealCodeLen);
                for (int i = 0; i < iDealCodeLen; i++)
                {
                    if (byteTemp[i]!=byteDealCode[i])
                    {
                        return false;
                    }
                }
                byteTemp = null;
                iEffect += iDealCodeLen;
            }
            //TODO 检查数据体和长度是否吻合，换有结束符 0x03
            byteTemp = new byte[4];

            byteTemp[0] = bytes[iEffect + 4];
            byteTemp[1] = bytes[iEffect + 3];
            byteTemp[2] = bytes[iEffect + 2];
            byteTemp[3] = bytes[iEffect + 1];
            iEffect += 4;
            int iDataLen = BitConverter.ToInt32(byteTemp, 0);
            byteTemp = null;
            byteData = new byte[iDataLen];
            Array.Copy(bytes, iEffect, byteData, 0, iDataLen);
            iEffect += iDataLen;
            if (bytes[iEffect]==0x03)
            {
                return true;
            }
            return false;
        }
    }
}