using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NLog.Config;
using NLog.Targets;
using NLog;

namespace DVG.WIS.Utilities.Logs
{
    public class Logger
    {
        private static void Init()
        {
            LoggingConfiguration configuration = new LoggingConfiguration();

            FileTarget target = new FileTarget();
            configuration.AddTarget("file", target);
            target.FileName = LogPath + "${date:format=yyyyMMdd}/tracelog.txt";
            target.Layout = "${date:format=HH\\:mm\\:ss}\t${message}\t${logger}\t${stacktrace}";
            LoggingRule item = new LoggingRule("*", target);
            item.EnableLoggingForLevel(LogLevel.Trace);
            configuration.LoggingRules.Add(item);

            FileTarget target2 = new FileTarget();
            configuration.AddTarget("file", target2);
            target2.FileName = LogPath + "${date:format=yyyyMMdd}/debuglog.txt";
            target2.Layout = "${date:format=HH\\:mm\\:ss}\t${message}\t${logger}\t${stacktrace}";
            LoggingRule rule2 = new LoggingRule("*", target2);
            rule2.EnableLoggingForLevel(LogLevel.Debug);
            configuration.LoggingRules.Add(rule2);

            FileTarget target3 = new FileTarget();
            configuration.AddTarget("file", target3);
            target3.FileName = LogPath + "${date:format=yyyyMMdd}/infolog.txt";
            target3.Layout = "${date:format=HH\\:mm\\:ss}\t${message}\t${logger}\t${stacktrace}";
            LoggingRule rule3 = new LoggingRule("*", target3);
            rule3.EnableLoggingForLevel(LogLevel.Info);
            configuration.LoggingRules.Add(rule3);

            FileTarget target4 = new FileTarget();
            configuration.AddTarget("file", target4);
            target4.FileName = LogPath + "${date:format=yyyyMMdd}/warnlog.txt";
            target4.Layout = "${date:format=HH\\:mm\\:ss}\t${message}\t${logger}\t${stacktrace}";
            LoggingRule rule4 = new LoggingRule("*", target4);
            rule4.EnableLoggingForLevel(LogLevel.Warn);
            configuration.LoggingRules.Add(rule4);

            FileTarget target5 = new FileTarget();
            configuration.AddTarget("file", target5);
            target5.FileName = LogPath + "${date:format=yyyyMMdd}/errorlog.txt";
            target5.Layout = "${date:format=HH\\:mm\\:ss}\t${message}\t${logger}\t${stacktrace}";
            LoggingRule rule5 = new LoggingRule("*", target5);
            rule5.EnableLoggingForLevel(LogLevel.Error);
            configuration.LoggingRules.Add(rule5);

            FileTarget target6 = new FileTarget();
            configuration.AddTarget("file", target6);
            target6.FileName = LogPath + "${date:format=yyyyMMdd}/fatallog.txt";
            target6.Layout = "${date:format=HH\\:mm\\:ss}\t${message}\t${logger}\t${stacktrace}";
            LoggingRule rule6 = new LoggingRule("*", target6);
            rule6.EnableLoggingForLevel(LogLevel.Fatal);
            configuration.LoggingRules.Add(rule6);

            LogManager.Configuration = configuration;
        }
        public static void TraceLog(object content)
        {
            if (LogManager.Configuration == null)
            {
                Init();
            }
            NLog.Logger logger = LogManager.GetLogger(new StackFrame(1).GetMethod().Name);
            logger.Trace(content);
        }
        public static void WriteLog(LogType logType, object content)
        {
            if (LogManager.Configuration == null)
            {
                Init();
            }
            NLog.Logger logger = LogManager.GetLogger(new StackFrame(1).GetMethod().Name);
            switch (logType)
            {
                case LogType.Trace:
                    logger.Trace(content);
                    return;

                case LogType.Debug:
                    logger.Debug(content);
                    return;

                case LogType.Warning:
                    logger.Warn(content);
                    return;

                case LogType.Error:
                    logger.Error(content);
                    return;

                case LogType.Fatal:
                    logger.Fatal(content);
                    return;
            }
            logger.Info(content);
        }

        public static void WriteLog(LogType logType, string content)
        {
            if (LogManager.Configuration == null)
            {
                Init();
            }
            NLog.Logger logger = LogManager.GetLogger(new StackFrame(1).GetMethod().Name);
            switch (logType)
            {
                case LogType.Trace:
                    logger.Trace(content);
                    return;

                case LogType.Debug:
                    logger.Debug(content);
                    return;

                case LogType.Warning:
                    logger.Warn(content);
                    return;

                case LogType.Error:
                    logger.Error(content);
                    return;

                case LogType.Fatal:
                    logger.Fatal(content);
                    return;
            }
            logger.Info(content);
        }

        public static void WriteLog(LogType logType, string content, LogParam logParam)
        {
            if (LogManager.Configuration == null)
            {
                Init();
            }
            NLog.Logger logger = LogManager.GetLogger(new StackFrame(1).GetMethod().Name);
            switch (logType)
            {
                case LogType.Trace:
                    logger.Trace(content);
                    foreach (KeyValuePair<string, object> pair in logParam.Attribute)
                    {
                        logger.Trace("$[{pair.Key}:{pair.Value}]");
                    }
                    return;

                case LogType.Debug:
                    logger.Debug(content);
                    foreach (KeyValuePair<string, object> pair2 in logParam.Attribute)
                    {
                        logger.Debug("$[{pair2.Key}:{pair2.Value}]");
                    }
                    return;

                case LogType.Warning:
                    logger.Warn(content);
                    foreach (KeyValuePair<string, object> pair3 in logParam.Attribute)
                    {
                        logger.Warn("$[{pair3.Key}:{pair3.Value}]");
                    }
                    return;

                case LogType.Error:
                    logger.Error(content);
                    foreach (KeyValuePair<string, object> pair4 in logParam.Attribute)
                    {
                        logger.Error("$[{pair4.Key}:{pair4.Value}]");
                    }
                    return;

                case LogType.Fatal:
                    logger.Fatal(content);
                    foreach (KeyValuePair<string, object> pair5 in logParam.Attribute)
                    {
                        logger.Fatal("$[{pair5.Key}:{pair5.Value}]");
                    }
                    return;
            }
            logger.Info(content);
            foreach (KeyValuePair<string, object> pair6 in logParam.Attribute)
            {
                logger.Info("$[{pair6.Key}:{pair6.Value}]");
            }
        }

        public static void Info(object content)
        {
            WriteLog(LogType.Info, content);
        }

        public static void ErrorLog(object content)
        {
            WriteLog(LogType.Error, content);
        }

        public static void ErrorLog(string content)
        {
            WriteLog(LogType.Error, content);
        }

        public static void FatalLog(object content)
        {
            WriteLog(LogType.Fatal, content);
        }

        public static void FatalLog(string content)
        {
            WriteLog(LogType.Fatal, content);
        }
        private static string LogPath
        {
            get
            {
                string absolutePath = new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase).AbsolutePath;
                absolutePath = !string.IsNullOrEmpty(AppSettings.Instance.GetString("LogPath")) ? AppSettings.Instance.GetString("LogPath") : (absolutePath.Substring(0, absolutePath.IndexOf("/bin") + 1) + "logs/");
                try
                {
                    if (!Directory.Exists(absolutePath))
                    {
                        Directory.CreateDirectory(absolutePath);
                    }
                }
                catch (Exception)
                {
                }
                return absolutePath;
            }
        }

        public static void Error(Exception ex)
        {
            WriteLog(LogType.Error, ex);
        }

        [Serializable]
        public class LogParam
        {
            private Dictionary<string, object> _attribute;

            public LogParam()
            {
                this.Attribute = new Dictionary<string, object>();
            }

            public Dictionary<string, object> Attribute
            {
                get { return _attribute; }
                set
                {
                    this._attribute = value;
                }
            }

            public object this[string attribute]
            {
                get
                {
                    if (!this.Attribute.ContainsKey(attribute))
                    {
                        return null;
                    }
                    return this.Attribute[attribute];
                }
                set
                {
                    if (this.Attribute.ContainsKey(attribute))
                    {
                        this.Attribute[attribute] = value;
                    }
                    else
                    {
                        this.Attribute.Add(attribute, value);
                    }
                }
            }
        }

        public enum LogType
        {
            Trace,
            Debug,
            Info,
            Warning,
            Error,
            Fatal,
            TraceOveride,
        }
    }
}

