using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Threading;
using System.Runtime.Serialization;

namespace SlowAndSteadyParser
{
    [Serializable]
    public class VBATestLog : VBALog, ISerializable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private object locker = new object();

        private bool m_IsPaused = false;

        private bool m_IsSinglestepMode = true;

        public delegate void ScriptPauseEventHandler(VBALogLevelFlag lflag, string message);

        public event ScriptPauseEventHandler MessageEvent = null;

        private void WaitOne()
        {
            lock (locker)
            {                
                if (m_IsSinglestepMode)
                {
                    m_IsPaused = true;
                    Monitor.Wait(locker);
                }
            }
        }
        
        #region 公共属性和方法 用于VBA调用

        public bool IsPaused
        {
            get { return m_IsPaused; }
        }

        public void PulseOne()
        {
            lock (locker)
            {
                Monitor.PulseAll(locker);
                m_IsPaused = false;
            }
        }

        public void TurnPersistance()
        {
            m_IsSinglestepMode = false;
        }

        public override void Debug(String d)
        {
            if (MessageEvent != null)
                MessageEvent(VBALogLevelFlag.Debug, d);            
            WaitOne();
        }

        public override void Debug(String d, Exception e)
        {
             if (MessageEvent != null)
                MessageEvent(VBALogLevelFlag.Debug, d+e.ToString());            
            WaitOne();
        }

        public override void Info(String info)
        {
            if (MessageEvent != null)
                MessageEvent(VBALogLevelFlag.Info, info);            
            WaitOne();
        }

        public override void Info(String info, Exception e)
        {
           if (MessageEvent != null)
                MessageEvent(VBALogLevelFlag.Info, info+e.ToString());            
            WaitOne();
        }

        public override void Warn(String w)
        {
            if (MessageEvent != null)
                MessageEvent(VBALogLevelFlag.Warn , w);            
            WaitOne();
        }

        public override void Warn(String w, Exception e)
        {
             if (MessageEvent != null)
                MessageEvent(VBALogLevelFlag.Warn , w+e.ToString());            
            WaitOne();
        }

        public override void Error(String r)
        {
           if (MessageEvent != null)
                MessageEvent(VBALogLevelFlag.Error, r);            
            WaitOne();
        }

        public override void Error(String r, Exception e)
        {
            if (MessageEvent != null)
                MessageEvent(VBALogLevelFlag.Error, r+e.ToString());            
            WaitOne();
        }

        public override void Fatal(String f)
        {
            if (MessageEvent != null)
                MessageEvent(VBALogLevelFlag.Fatal , f);            
            WaitOne();
        }

        public override void Fatal(String f, Exception e)
        {
            if (MessageEvent != null)
                MessageEvent(VBALogLevelFlag.Fatal , f+e.ToString());            
            WaitOne();
        }

        #endregion

        public override void LogListLog(VBALogLevelFlag level, string message)
        {
            if (MessageEvent != null)
                MessageEvent(level, message);
            WaitOne();
        }

        #region 构造函数 **************************************************************************


        /// <summary>
		/// 初始化对象
		/// </summary>
        public VBATestLog(bool IsSingleStepMode)
        {
            m_IsSinglestepMode = IsSingleStepMode;
        }
        
        /// <summary>
        /// 用来反序列化的构造函数
        /// </summary>
        protected VBATestLog(System.Runtime.Serialization.SerializationInfo s, System.Runtime.Serialization.StreamingContext c)
        {

        }

		#endregion

        #region IVBAObject 成员

        public override void Reset()
        {

        }
        #endregion

        #region ISerializable 成员

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
        }

        #endregion
    }
}
