using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SlowAndSteadyParser
{
    [Serializable]
    public class LocalPreparationVBAScriptTask : BaseVBAScriptTask , IRemoteTask
    {
        protected virtual Dictionary<string, IVBAObject> GetVBAObjectForNewTaskChain(Domain d)
        {
            Dictionary<string, IVBAObject> l = new Dictionary<string, IVBAObject>();
            IVBAObject ivo = new VBATask();
            l.Add(ivo.Name, ivo);
            ivo = new VBALog(d.LogLevelFlag);
            l.Add(ivo.Name, ivo);
            ivo = new VBAIE();
            l.Add(ivo.Name, ivo);
            ivo = new VBAUtility();
            l.Add(ivo.Name, ivo);
            ivo = new VBAHtml();
            l.Add(ivo.Name, ivo);
            return l;
        }

        public override List<ITask> NextTasks()
        {
            List<ITask> ltask = base.NextTasks();
            Domain d = DomainManager.GetDomain(m_domainGUID);
            if (d!=null)
            {
                ltask.Add(new LocalParserVBAScriptTask(m_domainGUID, m_taskchainGUID, m_vbaobjs));
                m_vbaobjs = null;
            }
            return ltask;
        }

        protected override string GetScript()
        {
            return DomainManager.GetDomain(this.DomainGUID).PreparationScript;
        }

        public override string Name { get { return "Local Preparation Task"; } }

        #region 构造函数 **********************************************************

        /// <summary>
        /// 构造函数(输入站点domain, 将自动从站点中抽取PreparationScript)
        /// </summary>
        public LocalPreparationVBAScriptTask(Domain d)
        {
            m_domainGUID = d.DomainGUID;
            m_script = d.PreparationScript;
            m_vbaobjs = GetVBAObjectForNewTaskChain(d);
            m_taskchainGUID = Guid.NewGuid().ToString();
            m_timestamp = GetTimeStamp();
        }

        /// <summary>
        /// 构造函数(输入站点domainGUID, 将自动从站点中抽取PreparationScript)
        /// </summary>
        public LocalPreparationVBAScriptTask(string domainGUID)
        {
            Domain d = DomainManager.GetDomain(domainGUID);
            if (d != null)
            {
                m_domainGUID = d.DomainGUID;
                m_script = d.PreparationScript;
                m_vbaobjs = GetVBAObjectForNewTaskChain(d);
                m_taskchainGUID = Guid.NewGuid().ToString();
                m_timestamp = GetTimeStamp();
            }
            else
                throw new Exception("无效的DomainGUID: " + domainGUID);
        }

        /// <summary>
        /// 构造函数(无参数)
        /// </summary>
        public LocalPreparationVBAScriptTask() : base()
        {

        }

        /// <summary>
        /// 构造函数(输入站点DomainGUID, IVBAObject数组对象)
        /// </summary>
        public LocalPreparationVBAScriptTask(string dGUID, Dictionary<string, IVBAObject> vobjs) : base(dGUID, vobjs)
        {
            if (vobjs == null)
                m_vbaobjs = GetVBAObjectForNewTaskChain(DomainManager.GetDomain(dGUID));
            else
                if (vobjs.Count == 0)
                    m_vbaobjs = GetVBAObjectForNewTaskChain(DomainManager.GetDomain(dGUID));
            m_taskchainGUID = Guid.NewGuid().ToString();
        }


        /// <summary>
        /// 构造函数(输入站点DomainGUID, 任务链TaskChainGUID, IVBAObject数组对象)
        /// </summary>
        public LocalPreparationVBAScriptTask(string dGUID, string tcGUID,Dictionary<string, IVBAObject> vobjs): base(dGUID, tcGUID, vobjs)
        {
            if (vobjs == null)
                m_vbaobjs = GetVBAObjectForNewTaskChain(DomainManager.GetDomain(dGUID));
            else
                if (vobjs.Count == 0)
                    m_vbaobjs = GetVBAObjectForNewTaskChain(DomainManager.GetDomain(dGUID));
        }

        protected LocalPreparationVBAScriptTask(SerializationInfo s, StreamingContext c) : base(s, c)
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
            LocalPreparationVBAScriptTask t = new LocalPreparationVBAScriptTask();
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
