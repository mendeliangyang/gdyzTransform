using gdyzTransform.Common;
using gdyzTransform.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Web.Http;

namespace gdyzTransform.Controllers
{
    public class ChangePasswordController : ApiController
    {
        FormationResult fr = new FormationResult();

        [HttpPost]
        public JToken ChangePassword(dynamic strParam)
        {
            NLogHelper.DefalutInfo(string.Format("ChangePassword: {0}", strParam.ToString()));

            #region json description msgBuf

            //json 格式
            //   {
            //    "head":
            //    {
            //    },
            //    "body":
            //    {
            //        "CardNo":"12121212","OldPasswd":"12121212","NewPasswd":"12121212","IdCardType":"12121212","IdCardNo":"12121212"
            //    }
            //}
            //char CardNo[150]         //银行卡号：客户需办理业务的卡号
            //char OldPasswd[30]  		//旧密码：修改前密码
            //char NewPasswd[30]		//新密码：修改后密码
            //char IdCardType[3]		   //证件类型：办理人证件类型
            //char IdCardNo[20]			//证件号码：办理人证件号码


            //            数据包格式：040402
            //报文头（0x02） +操作类型（0x25）+ 交易码（6个字节）+ 数据长度（4个字节）+ 报文内容（ buf ）+ 报文尾（0x03）

            //txn_040402Entity txn = Common.JsonHelper.JsonDeserialize<Models.txn_040402Entity>(jobject.GetValue("body").ToString());
            //txn_040402 structTxn1 = Common.JsonHelper.JsonDeserialize<Models.txn_040402>(jobject.GetValue("body").ToString());
            #endregion

            byte[] byteMsg = null ,byteRet=null;
            List<byte> listByte;
            SocketClientHelper scHelper = new SocketClientHelper();
            try
            {
                JObject jobject = JObject.Parse(strParam.ToString());
                JToken jBody = jobject.GetJTokenFromJToken("body");
                txn_040402 structTxn = new txn_040402();
                structTxn.CardNo = jBody.GetStringFromJToken("CardNo").ToString().PadRight(150, '\0').ToCharArray();
                structTxn.OldPasswd = jBody.GetStringFromJToken("OldPasswd").ToString().PadRight(30, '\0').ToCharArray();
                structTxn.NewPasswd = jBody.GetStringFromJToken("NewPasswd").ToString().PadRight(30, '\0').ToCharArray();
                structTxn.IdCardType = jBody.GetStringFromJToken("IdCardType").ToString().PadRight(3, '\0').ToCharArray();
                structTxn.IdCardNo = jBody.GetStringFromJToken("IdCardNo").ToString().PadRight(20, '\0').ToCharArray();
                byteMsg = TakeMsgByte.HandleMsgToByte(0x02, 0x81, "040402", structTxn);

#if DEBUG
                System.IO.FileStream aFile = new System.IO.FileStream(@"D:\buf.txt", System.IO.FileMode.OpenOrCreate);
                // StreamWriter sw = new StreamWriter(aFile);
                aFile.Write(byteMsg, 0, byteMsg.Length);
                aFile.Flush();
                aFile.Close();
#endif

            }
            catch (Exception ex)
            {
                NLogHelper.DefalutError("ERROR: analyze request msg.", ex);
                return fr.FormationJToken(ResponseResultCode.Error, ex.Message, "", null);
            }
            try
            {
                listByte = scHelper.DealOnce(byteMsg);
                //获取到单号， 如果单号长度为0表示交易失败
                //报文头（0x02） +操作类型（0x81）+ 单号长度（4个字节）+ 单号+ 报文尾（0x03）
                //如果返回单号长度为0 表示数据后台保存失败
                byte[] byteData;
                //判断报文格式是否正确
                if(!TakeMsgByte.CheckMsgForm(listByte.ToArray(), 0x02, 0x81, null, out byteData))
                    return fr.FormationJToken(ResponseResultCode.Error, "获取申请数据失败。", "", null);
                //直接判断单号长度，如果不是0表示成功，发送请求接口，交易，否则返回错误给pad
                byte[] byteOddNum = new byte[4];
                byteOddNum[0] = listByte[5];
                byteOddNum[1] = listByte[4];
                byteOddNum[2] = listByte[3];
                byteOddNum[3] = listByte[2];
                int iOddNum = BitConverter.ToInt32(byteOddNum,0);
                //第一次申请出错，直接返回给pad错误
                if (iOddNum==0)
                {
                    return fr.FormationJToken(ResponseResultCode.Error, "申请改密失败。", "", null);
                }
                //定时发送    报文头（0x02） +操作类型（0x84）+单号长度（4个字节）+单号+ 报文尾（0x03）
                //把第一次返回的信息直接替换操作类型在返回给服务端
                byteMsg = listByte.ToArray();
                byteMsg[1] = 0x84;
                int iRequestCount = 0;
                while (true)
                {
                    //请求 24次，2分钟，如果没有返回表示处理失败
                    if (iRequestCount>24)
                    {
                        return fr.FormationJToken(ResponseResultCode.Error, "改密处理失败。", "", null);
                    }
                    //接收：报文头（0x02） +操作类型（0x84）+交易状态长度（4个字节）+交易状态+ 报文尾（0x03）
                    listByte = scHelper.DealOnce(byteMsg);
                    //判断报文格式是否正确
                    if (!TakeMsgByte.CheckMsgForm(listByte.ToArray(), 0x02, 0x84, null, out byteData))
                        return fr.FormationJToken(ResponseResultCode.Error, "获取处理结果失败。", "", null);
                    //有0和1  不考虑处理失败
                    byte[] byteNum = new byte[4];
                    byteNum[0] = listByte[5];
                    byteNum[1] = listByte[4];
                    byteNum[2] = listByte[3];
                    byteNum[3] = listByte[2];
                    int iMsgLen = BitConverter.ToInt32(byteNum, 0);
                    byteRet = new byte[iMsgLen];
                    Array.Copy(listByte.ToArray(), 6, byteRet, 0, iMsgLen);
                    //todo 不清楚具体的数据
                    //System.Text.Encoding.ASCII.GetString(byteRet);
                    //int iRetCode= BitConverter.ToInt32(byteRet,0);
                    if (byteRet[0]==0x01)
                    {
                        return fr.FormationJToken(ResponseResultCode.Success, "修改密码成功。", "", null);
                    }
                    //5s 询问一次  一定时间内都是0就更新为失败
                    System.Threading.Thread.Sleep(5000);
                    iRequestCount++;
                }

            }
            catch (Exception ex)
            {
                NLogHelper.DefalutError("ERROR: deal failed with backSvc.", ex);
                return fr.FormationJToken(ResponseResultCode.Error, ex.Message, "", null);
            }

            #region don't delete socket
            //Socket socketClient = null;
            //try
            //{
            //    IPAddress ipAddress = IPAddress.Parse(gdyzTransform.Properties.Settings.Default.ServiceIp);
            //    socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    //set timeout
            //    socketClient.ReceiveTimeout = 10000;
            //    socketClient.SendTimeout = 10000;
            //    //connect
            //    socketClient.Connect(new IPEndPoint(ipAddress, gdyzTransform.Properties.Settings.Default.ServicePort)); 
            //    //send
            //    socketClient.Send(byteMsg);
            //    //receive
            //    int receiveLen = 0, receiveTotal = 0;
            //    byte[] byteResult = new byte[1024];
            //    int available = socketClient.Available;
            //    List<byte> listByte = new List<byte>();
            //    do
            //    {
            //        // 如果客户端Socket关闭，_Client会接受到receiveLength=0
            //        receiveLen = socketClient.Receive(byteResult);
            //        receiveTotal += receiveLen;
            //        if (receiveLen > 0)
            //        {
            //            listByte.AddRange(byteResult);
            //        }
            //        Array.Clear(byteResult, 0, byteResult.Length);

            //    } while (receiveLen > 0 && receiveTotal < available);


            //    //respone
            //    //Console.WriteLine("连接服务器成功");
            //    return fr.FormationJToken(ResponseResultCode.Success,"","",null);
            //        // JObject.Parse("{\"head\":{\"resultCode\":\"Success\",\"errMsg\":\"\",\"token\":\"\"},\"body\":null}");
            //}
            //catch (Exception ex)
            //{
            //    //send pad error  
            //    NLogHelper.DefalutInfo("ERROR: deal failed with backSvc.");
            //    NLogHelper.ExceptionInfo("deal failed with backSvc. ", ex);

            //    //return JObject.Parse(string.Format("{\"head\":{\"resultCode\":\"Error\",\"errMsg\":\"{0}\",\"token\":\"\"},\"body\":null}", ex.Message));

            //    return fr.FormationJToken(ResponseResultCode.Error, ex.Message, "", null);
            //}
            //finally
            //{
            //    try
            //    {
            //        if (socketClient != null && socketClient.Connected)
            //        {
            //            socketClient.Shutdown(SocketShutdown.Both);
            //            socketClient.Close();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        NLogHelper.DefalutInfo("ERROR: shutdown connection .");
            //        NLogHelper.ExceptionInfo("shutdown connection. ", ex);
            //    }

            //}
            #endregion


        }
    }
}
