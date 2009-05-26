using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public class VBATaskHost: IVBAObjectHost, IVBATask
    {
        private VBATask m_vbatask = null;

        #region IVBAObjectHost 成员

        public string Name
        {
            get { return "Task"; }
        }

        public IVBAObject Tenant
        {
            get
            {
                return m_vbatask;
            }
            set
            {
                m_vbatask = (VBATask)value;
            }
        }

        #endregion

        #region IVBATask 成员

        public VBATaskStatus CurrentVBATaskStatus
        {
            get { return m_vbatask.CurrentVBATaskStatus; }
            set { m_vbatask.CurrentVBATaskStatus = value; }
        }

        public int GetHashInt(string key)
        {
            return m_vbatask.GetHashInt(key);
        }

        public string GetHashString(string key)
        {
            return m_vbatask.GetHashString(key);
        }

        public object GetHashValue(string key)
        {
            return m_vbatask.GetHashValue(key);
        }

        public void RemoveHashValue(string key)
        {
            m_vbatask.RemoveHashValue(key);
        }

        public bool SetHashValue(string key, object value)
        {
            return m_vbatask.SetHashValue(key, value);
        }

        public void TaskFail()
        {
            m_vbatask.TaskFail();
        }

        public void TaskRestartADSL()
        {
            m_vbatask.TaskRestartADSL();
        }

        public string URL
        {
            get
            {
                return m_vbatask.URL;
            }
            set
            {
                m_vbatask.URL = value;
            }
        }

        #endregion

        #region IVBATask 成员

        public void TaskClose()
        {
            m_vbatask.TaskClose();
        }

        public void TaskCloseAndTurnOnNextDomain()
        {
            m_vbatask.TaskCloseAndTurnOnNextDomain();
        }

        #endregion

        #region IVBATask 成员


        public void TaskMessage(string message)
        {
            m_vbatask.TaskMessage(message);
        }

        public void TaskMessage(string uniname, string message)
        {
            m_vbatask.TaskMessage(uniname, message);
        }

        #endregion

        #region IVBATask 成员


        public Random Random
        {
            get { return m_vbatask.Random; }
        }

        public string LocalDirectory
        {
            get { return m_vbatask.LocalDirectory; }
        }

        #endregion
    }
}
