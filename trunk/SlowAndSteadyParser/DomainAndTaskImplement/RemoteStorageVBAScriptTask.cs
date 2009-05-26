using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SlowAndSteadyParser
{
    [Serializable]
    public class RemoteStorageVBAScriptTask : BaseVBAScriptTask, IRemoteTask
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override string GetScript()
        {
            return DomainManager.GetDomain(this.DomainGUID).StorageScript;
        }

        public override string Name { get { return "Remote Storage Task"; } }

        #region 构造函数 **********************************************************
        /// <summary>
        /// 构造函数(无参数)
        /// </summary>
        public RemoteStorageVBAScriptTask() : base()
        {

        }

        /// <summary>
        /// 构造函数(输入站点DomainGUID, IVBAObject数组对象)
        /// </summary>
        public RemoteStorageVBAScriptTask(string dGUID, Dictionary<string, IVBAObject> vobjs) : base(dGUID, vobjs)
        {

        }

        /// <summary>
        /// 构造函数(输入站点DomainGUID, 任务链TaskChainGUID, IVBAObject数组对象)
        /// </summary>
        public RemoteStorageVBAScriptTask(string dGUID, string tcGUID, Dictionary<string, IVBAObject> vobjs)
            : base(dGUID, tcGUID, vobjs)
        {
        }

        protected RemoteStorageVBAScriptTask(SerializationInfo s, StreamingContext c)
            : base(s, c)
        {

        }

        #endregion


        #region IRemoteTask 成员

        public TaskPosition Position
        {
            get { return TaskPosition.Server; }
        }

        #endregion


        #region ICloneable 成员

        public object Clone()
        {
            RemoteStorageVBAScriptTask t = new RemoteStorageVBAScriptTask();
            t.m_domainGUID = m_domainGUID;
            t.m_engine = m_engine;
            t.m_errortime = m_errortime;
            t.m_IsClosing = m_IsClosing;
            t.m_runningtime = m_runningtime;
            t.m_script = m_script;
            t.m_taskchainGUID = m_taskchainGUID;
            t.m_taskstatus = m_taskstatus;
            t.m_timestamp = m_timestamp;
            t.m_vbaobjs = m_vbaobjs;
            return t;
        }

        #endregion
    }
}
