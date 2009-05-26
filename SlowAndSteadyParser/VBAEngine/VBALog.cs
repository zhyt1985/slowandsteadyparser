using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace SlowAndSteadyParser
{
    [Serializable]
    public class VBALog : IVBAObject, IVBALog
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private VBALogLevelFlag m_loglevelstartard = VBALogLevelFlag.Debug;

        #region 公共属性和方法 用于VBA调用

        public virtual void Debug(String d)
        {
            log.Debug(d);
            LogListLog(VBALogLevelFlag.Debug, d);
        }

        public virtual void Debug(String d, Exception e)
        {
            log.Debug(d, e);
            LogListLog(VBALogLevelFlag.Debug, d + " " + e.Message);
        }

        public virtual void Info(String info)
        {
            log.Info(info);
            LogListLog(VBALogLevelFlag.Info, info);
        }

        public virtual void Info(String info, Exception e)
        {
            log.Info(info, e);
            LogListLog(VBALogLevelFlag.Info, info + " " + e.Message);
        }

        public virtual void Warn(String w)
        {
            log.Warn(w);
            LogListLog(VBALogLevelFlag.Warn, w);
        }

        public virtual void Warn(String w, Exception e)
        {
            log.Warn(w, e);
            LogListLog(VBALogLevelFlag.Warn, w + " " + e.Message);
        }

        public virtual void Error(String r)
        {
            log.Error(r);
            LogListLog(VBALogLevelFlag.Error, r);
        }

        public virtual void Error(String r, Exception e)
        {
            log.Error(r, e);
            LogListLog(VBALogLevelFlag.Error, r + " " + e.Message);
        }

        public virtual void Fatal(String f)
        {
            log.Fatal(f);
            LogListLog(VBALogLevelFlag.Fatal, f);
        }

        public virtual void Fatal(String f, Exception e)
        {
            log.Fatal(f, e);
            LogListLog(VBALogLevelFlag.Fatal, f + " " + e.Message);
        }

        #endregion

        #region Loglist Construction

        [Serializable]
        public struct LogList
        {
            public DateTime errtime;
            public VBALogLevelFlag errlevel;
            public string errmessage;
        }

        private List<LogList> m_loglist = new List<LogList>();

        public virtual List<LogList> Loglists
        {
            get { return m_loglist; }
        }

        public virtual void LogListLog(VBALogLevelFlag level, string message)
        {
            if (m_loglevelstartard <= level)
            {
                LogList l = new LogList();
                l.errtime = DateTime.Now;
                l.errlevel = level;
                l.errmessage = message;
                m_loglist.Add(l);
            }
        }

        #endregion

        #region 本模块的一些属性和方法 ************************************************************


        #endregion

        #region 构造函数 **************************************************************************

        public VBALog()
        {
        }

        public VBALog(VBALogLevelFlag loglevel)
        {
            m_loglevelstartard = loglevel;
        }

        /// <summary>
        /// 用来反序列化的构造函数
        /// </summary>
        protected VBALog(System.Runtime.Serialization.SerializationInfo s, System.Runtime.Serialization.StreamingContext c)
        {
            m_loglist = s.GetValue("Log", m_loglist.GetType()) as List<LogList>;
            m_loglevelstartard = (VBALogLevelFlag)s.GetValue("LogLevel", m_loglevelstartard.GetType());
        }

		#endregion

        #region IVBAObject 成员

        public string Name
        {
            get { return "Log"; }
        }

        public bool BeSerializable
        {
            get { return false; }
        }

        public VBAObjectLife Life
        {
            get { return VBAObjectLife.Task; }
        }

        public virtual void Reset()
        {
            m_loglist = new List<LogList>();
        }

        #endregion

        #region ISerializable 成员

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("Log", m_loglist);
            info.AddValue("LogLevel", m_loglevelstartard);
        }

        #endregion

        #region IDisposable 成员

        private bool IsDisposed = false;

        public virtual void Dispose()  
        {  
            Dispose(true);  
            GC.SuppressFinalize(this);  
        }

        protected void Dispose(bool Disposing)  
        {  
            if(!IsDisposed)  
            {  
                if(Disposing)  
                {
                    //清理托管资源          
                }  
                //清理非托管资源
            }  
            IsDisposed=true;  
        }

        ~VBALog()  
        {  
            Dispose(false);  
        } 

        #endregion
    }
}
