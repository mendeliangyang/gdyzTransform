using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace gdyzTransform.Models
{
    public class txn_040402Entity
    {
        public string CardNo;
        public string OldPasswd;
        public string NewPasswd;
        public string IdCardType;
        public string IdCardNo;
    }

    public struct txn_040402
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
        public char[] CardNo;         //银行卡号：客户需办理业务的卡号
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public char[] OldPasswd;  		//旧密码：修改前密码
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public char[] NewPasswd;	//新密码：修改后密码
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public char[] IdCardType;	   //证件类型：办理人证件类型
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] IdCardNo;		//证件号码：办理人证件号码
    }
}


//数据包格式：
//报文头（0x02） +操作类型（0x25）+ 交易码（6个字节）+ 数据长度（4个字节）+ 报文内容（ buf ）+ 报文尾（0x03）

//1、更改密印（040402）：
//Buf内容：
//为结构体
//Struct  txn_040402
//{
//char CardNo[150]         //银行卡号：客户需办理业务的卡号
//char OldPasswd[30]  		//旧密码：修改前密码
//char NewPasswd[30]		//新密码：修改后密码
//char IdCardType[3]		   //证件类型：办理人证件类型
//char IdCardNo[20]			//证件号码：办理人证件号码
//}
