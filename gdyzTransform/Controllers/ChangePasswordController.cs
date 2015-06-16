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
            NLogHelper.DefalutInfo(string.Format( "ChangePassword: {0}",strParam.ToString()));

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

            byte[] byteMsg = null;
            SocketClientHelper scHelper = new SocketClientHelper();
            try
            {
                JObject jobject = JObject.Parse(strParam.ToString());
                JToken jBody = jobject.GetJTokenFromJToken("body");
                txn_040402 structTxn = new txn_040402();
                structTxn.CardNo = jBody.GetStringFromJToken("CardNo").ToString().PadLeft(150, '\0').ToCharArray();
                structTxn.OldPasswd = jBody.GetStringFromJToken("OldPasswd").ToString().PadLeft(30, '\0').ToCharArray();
                structTxn.NewPasswd = jBody.GetStringFromJToken("NewPasswd").ToString().PadLeft(30, '\0').ToCharArray();
                structTxn.IdCardType = jBody.GetStringFromJToken("IdCardType").ToString().PadLeft(3, '\0').ToCharArray();
                structTxn.IdCardNo = jBody.GetStringFromJToken("IdCardNo").ToString().PadLeft(20, '\0').ToCharArray();
                byteMsg = TakeMsgByte.HandleMsgToByte(0x02, 0x25, "040402", structTxn);
                //获取到结果保存到本地，轮询查询
            }
            catch (Exception ex)
            {
                NLogHelper.DefalutError("ERROR: analyze request msg.",ex);
                return fr.FormationJToken(ResponseResultCode.Error, ex.Message, "", null);
            }
            try
            {
               List<byte> listByte= scHelper.DealOnce(byteMsg);
               return fr.FormationJToken(ResponseResultCode.Success, "", "", null);
            }
            catch (Exception ex)
            {
                NLogHelper.DefalutError("ERROR: deal failed with backSvc.",ex);
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
