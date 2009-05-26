using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SlowAndSteadyParser
{
    [Serializable]
    public class RemoteParserVBAScriptTask : BaseVBAScriptTask, IRemoteTask, ISerializable
    {
        public override List<ITask> NextTasks()
        {
            List<ITask> ltask = base.NextTasks() ;
            Domain d = DomainManager.GetDomain(m_domainGUID);
            if (d!=null)
            {
                ltask.Add(new RemoteStorageVBAScriptTask(m_domainGUID, m_taskchainGUID, m_vbaobjs));
                m_vbaobjs = null;
            }
            return ltask;
        }

        protected override string GetScript()
        {
            return DomainManager.GetDomain(this.DomainGUID).ParserScript;
        }

        public override string Name { get { return "Remote Parser Task"; } }

        #region 构造函数 **********************************************************
        /// <summary>
        /// 构造函数(无参数)
        /// </summary>
        public RemoteParserVBAScriptTask() : base()
        {

        }

        /// <summary>
        /// 构造函数(输入站点DomainGUID, IVBAObject数组对象)
        /// </summary>
        public RemoteParserVBAScriptTask(string dGUID, Dictionary<string, IVBAObject> vobjs) : base(dGUID,vobjs)
        {

        }


        /// <summary>
        /// 构造函数(输入站点DomainGUID, 任务链TaskChainGUID, IVBAObject数组对象)
        /// </summary>
        public RemoteParserVBAScriptTask(string dGUID, string tcGUID, Dictionary<string, IVBAObject> vobjs)
            : base(dGUID, tcGUID, vobjs)
        {
        }


        protected RemoteParserVBAScriptTask(SerializationInfo s, StreamingContext c) : base(s, c)
        {

        }

        #endregion

        #region IRemoteTask 成员

        public TaskPosition Position
        {
            get { return TaskPosition.Client; }
        }

        #endregion

        #region ICloneable 成员

        public object Clone()
        {
            RemoteParserVBAScriptTask t = new RemoteParserVBAScriptTask();
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
