using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using RAJ.GrayLog.Interfaces;
using NLog;

namespace RAJ.GrayLog.Utils
{
    public class GelfConverter : IGelfConverter
    {
        private const int ShortMessageMaxLength = 250;
        private const string GelfVersion = "1.0";

        public JObject Convert(LogEventInfo logEventInfo, string facility)
        {
            var gelfMessageBuilder = new GelfMessageBuilder(logEventInfo.Properties);
            var logEventMessage = logEventInfo.FormattedMessage;
            if (logEventMessage == null)
                return null;

            if (logEventInfo.Exception != null)
            {
                string exceptionDetail;
                string stackDetail;

                GetExceptionMessages(logEventInfo.Exception, out exceptionDetail, out stackDetail);
                gelfMessageBuilder.AddProperty("ExceptionSource", logEventInfo.Exception.Source)
                    .AddProperty("ExceptionMessage", exceptionDetail)
                    .AddProperty("StackTrace", stackDetail);
            }

            var shortMessage = logEventMessage;
            if (shortMessage.Length > ShortMessageMaxLength)
            {
                shortMessage = shortMessage.Substring(0, ShortMessageMaxLength);
            }

            return gelfMessageBuilder.AddProperty("facility", (string.IsNullOrEmpty(facility) ? "GELF" : facility))
                    .AddProperty("LoggerName", logEventInfo.LoggerName)
                    .AddVersion(GelfVersion)
                    .AddHost(Dns.GetHostName())
                    .AddShortMessage(shortMessage)
                    .AddFullMessage(logEventMessage)
                    .AddTimestamp(logEventInfo.TimeStamp)
                    .AddLevel(GetSeverityLevel(logEventInfo.Level))
                    .AddLine((logEventInfo.UserStackFrame != null)
                        ? logEventInfo.UserStackFrame.GetFileLineNumber().ToString(CultureInfo.InvariantCulture)
                        : string.Empty)
                    .AddFile((logEventInfo.UserStackFrame != null)
                        ? logEventInfo.UserStackFrame.GetFileName()
                        : string.Empty)
                    .Build();
        }

        /// <summary>
        /// Values from SyslogSeverity enum here: http://marc.info/?l=log4net-dev&m=109519564630799
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static int GetSeverityLevel(LogLevel level)
        {
            if (level == LogLevel.Debug)
            {
                return 7;
            }
            if (level == LogLevel.Fatal)
            {
                return 2;
            }
            if (level == LogLevel.Info)
            {
                return 6;
            }
            if (level == LogLevel.Trace)
            {
                return 6;
            }
            if (level == LogLevel.Warn)
            {
                return 4;
            }

            return 3; //LogLevel.Error
        }

        /// <summary>
        /// Get the message details from all nested exceptions, up to 10 in depth.
        /// </summary>
        /// <param name="ex">Exception to get details for</param>
        /// <param name="exceptionDetail">Exception message</param>
        /// <param name="stackDetail">Stacktrace with inner exceptions</param>
        private void GetExceptionMessages(Exception ex, out string exceptionDetail, out string stackDetail)
        {
            var exceptionSb = new StringBuilder();
            var stackSb = new StringBuilder();
            var nestedException = ex;
            stackDetail = null;

            int counter = 0;
            do
            {
                exceptionSb.Append(nestedException.Message + " - ");
                if (nestedException.StackTrace != null)
                    stackSb.Append(nestedException.StackTrace + "--- Inner exception stack trace ---");
                nestedException = nestedException.InnerException;
                counter++;
            }
            while (nestedException != null && counter < 11);

            exceptionDetail = exceptionSb.ToString().Substring(0, exceptionSb.Length - 3);
            if (stackSb.Length > 0)
                stackDetail = stackSb.ToString().Substring(0, stackSb.Length - 35);
        }
    }
}