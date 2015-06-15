using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gdyzTransform.Common
{
    internal class NLogHelper
    {
        private static readonly NLog.Logger DefalutLogger = NLog.LogManager.GetLogger("DefaultLogger");

        private static readonly NLog.Logger ExceptionLogger = NLog.LogManager.GetLogger("ExceptionLogger");





        internal static void DefalutInfo(string info)
        {
#if DEBUG
            Console.WriteLine(info);
#endif

            DefalutLogger.Info(info);
        }

        internal static void DefalutError(string info,Exception ex)
        {
#if DEBUG
            Console.WriteLine(info);
#endif

            DefalutLogger.Info(info);
            ExceptionLogger.Error(info, ex);
        }
        internal static void ExceptionInfo(string info, Exception ex)
        {
#if DEBUG
            Console.WriteLine(info);
#endif
            ExceptionLogger.Error(info, ex);
        }


    }
}