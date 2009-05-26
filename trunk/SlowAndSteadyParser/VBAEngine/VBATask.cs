using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SlowAndSteadyParser
{
    [Serializable]
    public class VBATask : IVBAObject, IVBATask
    {
        //log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Random ms_rand = new Random();

        private Hashtable m_taskht = new Hashtable();
        private string m_url = null;
        private VBATaskStatus m_status = VBATaskStatus.Ready;
        

        //面向Task, 不要从脚本中直接调用该方法
        public virtual VBATaskStatus CurrentVBATaskStatus 
        {
            get { return m_status; }
            set { m_status = value; }
        }

        #region 公共属性 用于VBA调用

        public virtual string URL
        {
            get { return m_url; }
            set { m_url = value; }
        }

        public virtual Random Random
        {
            get { return ms_rand; }
        }

        public virtual string LocalDirectory
        {
            get { return Environment.CurrentDirectory; }
        }

        #endregion

        #region 公共方法 用于VBA调用 - Data 数据储存

        public virtual object GetHashValue(string key)
        {
            if (m_taskht.Contains(key))
                return m_taskht[key];
            return null;
        }

        public virtual string GetHashString(string key)
        {
            if (m_taskht.Contains(key))
                return (string)m_taskht[key];
            return null;
        }

        public virtual int GetHashInt(string key)
        {
            if (m_taskht.Contains(key))
                return (int)m_taskht[key];
            return 0;
        }

        public virtual bool SetHashValue(string key, object value)
        {
            if (m_taskht.Contains(key))
                m_taskht[key] = value;
            else
                m_taskht.Add(key, value);
            return true;
        }

        public virtual void RemoveHashValue(string key)
        {
            m_taskht.Remove(key);
        }

        #endregion

        #region 公共方法 用于VBA调用 - Task 任务控制

        public virtual void TaskMessage(string message)
        {
            MessageMonitor.Update(this.GetHashCode().ToString(), message);
        }

        public virtual void TaskMessage(string uniquename, string message)
        {
            MessageMonitor.Update(uniquename, message);
        }

        public virtual void TaskFail()
        {
            m_status = VBATaskStatus.Failure;
        }

        public virtual void TaskRestartADSL()
        {
            m_status = VBATaskStatus.RestartADSL ;
        }

        public virtual void TaskClose()
        {
            m_status = VBATaskStatus.Close;
        }

        public virtual void TaskCloseAndTurnOnNextDomain()
        {
            m_status = VBATaskStatus.TurnToNext;
        }
        #endregion

		#region 构造函数 **************************************************************************

		/// <summary>
		/// 初始化对象
		/// </summary>
		public VBATask()
        {
        }

        /// <summary>
        /// 用来反序列化的构造函数
        /// </summary>
        public VBATask(System.Runtime.Serialization.SerializationInfo s, System.Runtime.Serialization.StreamingContext c)
        {
            m_url = s.GetString("URL");
            m_taskht = (Hashtable)s.GetValue("HashTable", m_taskht.GetType());
        }
		#endregion

		#region IVBAObject 成员

        public string Name
        {
            get { return "Task"; }
        }

        public bool BeSerializable
        {
            get { return true; }
        }

        public VBAObjectLife Life
        {
            get { return VBAObjectLife.TaskChain; }
        }

        public virtual void Reset()
        {
            m_taskht.Clear();
            m_url = null;
            m_status = VBATaskStatus.Ready;
        }

        #endregion

        #region ISerializable 成员

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("HashTable", m_taskht, m_taskht.GetType());
            info.AddValue("URL", m_url);
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

        ~VBATask()  
        {  
            Dispose(false);  
        } 

        #endregion
    }
}
