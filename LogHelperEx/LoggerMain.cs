using log4net;
using log4net.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogHelperEx
{
    /// <summary>
    /// 日志消息
    /// </summary>
    public struct LogMessage
    {
        public string Message { get; set; }
        public LogType MessageLogType { get; set; }
        public Exception Exceptions { get; set; }
    }

    public class LoggerMain
    {
        private readonly ConcurrentQueue<LogMessage> _que;
        private readonly ManualResetEvent _mre;
        /// <summary>
        /// lognet4日志
        /// </summary>
        private readonly ILog _log;
        private static LoggerMain _mainLog = new LoggerMain();
        public delegate void DelegateShowMessage(LogMessage msg);
        public event DelegateShowMessage ShowMessageEvent;
       
        /// <summary>
        /// 获取实例对象
        /// </summary>
        /// <returns></returns>
        public static LoggerMain GetInstance()
        {
            return _mainLog;
        }

        public LoggerMain()
        {
            var configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));
            if (!configFile.Exists)
            {
                //throw new Exception("未配置log4net配置文件！");
            }
            XmlConfigurator.Configure(configFile);
            _que = new ConcurrentQueue<LogMessage>();
            _mre = new ManualResetEvent(false);
            _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void RunMonitor()
        {
            Thread t = new Thread(new ThreadStart(WriteLog));
            t.Name = "Logger";
            t.IsBackground = true;
            t.Start();
            
        }

        private void WriteLog()
        {
            while (true)
            {
                // 等待信号通知
                _mre.WaitOne();
                LogMessage msg;
                // 判断是否有内容需要如磁盘 从列队中获取内容，并删除列队中的内容
                while (_que.Count > 0 && _que.TryDequeue(out msg))
                {
                    // 判断日志等级，然后写日志
                    switch (msg.MessageLogType)
                    {
                        case LogType.Debug:
                            _log.Debug(msg.Message, msg.Exceptions);
                            break;
                        case LogType.Info:
                            _log.Info(msg.Message, msg.Exceptions);
                            break;
                        case LogType.Error:
                            _log.Error(msg.Message, msg.Exceptions);
                            break;
                        case LogType.Warn:
                            _log.Warn(msg.Message, msg.Exceptions);
                            break;
                        case LogType.Fatal:
                            _log.Fatal(msg.Message, msg.Exceptions);
                            break;
                    }
                    if (ShowMessageEvent!=null)
                    {
                        ShowMessageEvent(msg);
                    }

                }
                // 重新设置信号
                _mre.Reset();
                Thread.Sleep(5);
            }
        }

        public void EnqueueMessage(string message, LogType logtype, Exception ex = null)
        {
            if ((logtype == LogType.Debug && _log.IsDebugEnabled)
             || (logtype == LogType.Error && _log.IsErrorEnabled)
             || (logtype == LogType.Fatal && _log.IsFatalEnabled)
             || (logtype == LogType.Info && _log.IsInfoEnabled)
             || (logtype == LogType.Warn && _log.IsWarnEnabled))
            {
                _que.Enqueue(new LogMessage
                {
                    Message = message,
                    MessageLogType = logtype,
                    Exceptions = ex
                });
                // 通知线程往磁盘中写日志
                _mre.Set();
            }
        }
        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void Debug(string msg, Exception ex = null)
        {
            GetInstance().EnqueueMessage(msg, LogType.Debug, ex);
        }
        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void Error(string msg, Exception ex = null)
        {
            GetInstance().EnqueueMessage(msg, LogType.Error, ex);
        }
        /// <summary>
        /// 写入致命信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void Fatal(string msg, Exception ex = null)
        {
            GetInstance().EnqueueMessage(msg, LogType.Fatal, ex);
        }
        /// <summary>
        /// 写入一般信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void Info(string msg, Exception ex = null)
        {
            GetInstance().EnqueueMessage(msg, LogType.Info, ex);
        }
        /// <summary>
        /// 写入警告信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void Warn(string msg, Exception ex = null)
        {
            GetInstance().EnqueueMessage(msg, LogType.Warn, ex);
        }

    }
}
