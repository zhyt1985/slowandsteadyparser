using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public class VBALogHost : IVBAObjectHost, IVBALog
    {
        private VBALog m_vbalog = null;

        #region IVBALog 成员

        public void Debug(string d, Exception e)
        {
            m_vbalog.Debug(d, e);
        }

        public void Debug(string d)
        {
            m_vbalog.Debug(d);
        }

        public void Error(string r)
        {
            m_vbalog.Error(r);
        }

        public void Error(string r, Exception e)
        {
            m_vbalog.Error(r, e);
        }

        public void Fatal(string f, Exception e)
        {
            m_vbalog.Fatal(f, e);
        }

        public void Fatal(string f)
        {
            m_vbalog.Fatal(f);
        }

        public void Info(string info, Exception e)
        {
            m_vbalog.Info(info, e);
        }

        public void Info(string info)
        {
            m_vbalog.Info(info);
        }

        public void LogListLog(VBALogLevelFlag level, string message)
        {
            m_vbalog.LogListLog(level, message);
        }

        public List<VBALog.LogList> Loglists
        {
            get { return m_vbalog.Loglists; }
        }

        public void Warn(string w, Exception e)
        {
            m_vbalog.Warn(w, e);
        }

        public void Warn(string w)
        {
            m_vbalog.Warn(w);
        }

        #endregion

        #region IVBAObjectHost 成员

        public string Name
        {
            get { return "Log"; }
        }

        public IVBAObject Tenant
        {
            get
            {
                return m_vbalog;
            }
            set
            {
                m_vbalog = (VBALog)value;
            }
        }

        #endregion
    }
}
