using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WooliesXAPI.Models;

namespace WooliesXAPI.Logging
{
    public static class LoggingExtensions
    {
        static readonly log4net.Core.Level authLevel = new log4net.Core.Level(68000, "AUTH");
        static readonly log4net.Core.Level invalidModelLevel = new log4net.Core.Level(69000, "INVALIDMODEL");
        static readonly log4net.Core.Level requestLevel = new log4net.Core.Level(66000, "REQUEST");
        static readonly log4net.Core.Level responseLevel = new log4net.Core.Level(67000, "RESPONSE");

        public static void Auth(this ILog log, string message)
        {
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                authLevel, message, null);
        }

        public static void AuthFormat(this ILog log, string message, params object[] args)
        {
            string formattedMessage = string.Format(message, args);
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                authLevel, formattedMessage, null);
        }

        public static void InvalidModel(this ILog log, string message)
        {
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                invalidModelLevel, message, null);
        }

        public static void InvalidModel(this ILog log, string message, List<CustomErrorModel> errors)
        {
            LogicalThreadContext.Properties["modelerrors"] = errors;
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, invalidModelLevel, message, null);
        }

        public static void InvalidModelFormat(this ILog log, string message, params object[] args)
        {
            string formattedMessage = string.Format(message, args);
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                invalidModelLevel, formattedMessage, null);
        }

        public static void Request(this ILog log, string message)
        {
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                requestLevel, message, null);
        }

        public static void RequestFormat(this ILog log, string message, params object[] args)
        {
            string formattedMessage = string.Format(message, args);
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                requestLevel, formattedMessage, null);
        }

        public static void Response(this ILog log, string message)
        {
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                responseLevel, message, null);
        }

        public static void ResponseFormat(this ILog log, string message, params object[] args)
        {
            string formattedMessage = string.Format(message, args);
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                responseLevel, formattedMessage, null);
        }
    }
}
