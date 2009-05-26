using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace SlowAndSteadyParser
{
    [Serializable]
    public class BaseVBAScriptTask : ITask , IVBAScript, ISerializable
    {
        #region 模块内部属性

        //log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //locker
        private object locker = new object();

        //resource
        protected DateTime m_timestamp;
        protected bool m_IsClosing = false;
        protected string m_domainGUID = null;
        protected string m_taskchainGUID = null;
        protected string m_script = null;
        protected Dictionary<string, IVBAObject> m_vbaobjs = new Dictionary<string,IVBAObject>();
        protected TaskStatus m_taskstatus = TaskStatus.Ready;
        protected int m_runningtime = 0;
        protected int m_errortime = 0;

        //engine
        protected VBAStaticEngine m_engine = null;

        //Thread
        protected Thread m_scriptthread = null;

        #endregion

        #region 私有方法

        private void EngineClose()
        {
            //关闭引擎
            if (m_engine != null)
            {
                VBAStaticEngine.ReturnVBAEngine(m_engine);
                m_engine = null;
            }
        }

        /// <summary>
        /// 分析错误文本, 找出有用的信息存入Log
        /// </summary>  
        private string ParserErrorString(Exception e)
        {
            Regex r1 = new Regex(@"在.*Module1:行号\s+(?<num>\S+)", RegexOptions.Multiline|RegexOptions.Compiled);
            string err = e.ToString();
            if (r1.IsMatch(err))
            {
                string errline = r1.Match(err).Value;
                string numstr = r1.Match(err).Groups["num"].Value;
                int n = VBAEngineBase.FixLineNumber(Convert.ToInt32(numstr.Trim()));
                string newerrline = errline.Replace(numstr,n.ToString());
                Regex r2 = new Regex(@".*", RegexOptions.Compiled | RegexOptions.Multiline);
                string errhead = r2.Match(err).Value;
                return errhead + Environment.NewLine + newerrline;
            }
            else
                return err;            
        }

        protected void ScriptThreadRunner()
        {
            //实现BeforeTask事件
            if (BeforeScript != null)
                BeforeScript(this, null);

            try
            {
                //Log开始
                GetLogger().LogListLog(VBALogLevelFlag.Debug, this.Name + " start...[running time: "+m_runningtime.ToString()+"] [at "+System.Net.Dns.GetHostName()+"]");

                //执行脚本,带空脚本处理
                if (m_script != null && m_script != "")
                    m_engine.Run();

                //关闭引擎
                EngineClose();

                //从VBATask中得到Task状态
                VBATaskStatus ts = (m_vbaobjs["Task"] as VBATask).CurrentVBATaskStatus;
                switch (ts)
                {
                    case VBATaskStatus.Failure:
                        GetLogger().LogListLog(VBALogLevelFlag.Debug, this.Name + " fail...");                        
                        m_taskstatus = TaskStatus.Failure;
                        break;
                    case VBATaskStatus.RestartADSL:
                        //重置ADSL(Client)
                        if (ADSLFactory.Reconnect())
                        {
                            GetLogger().LogListLog(VBALogLevelFlag.Debug, this.Name + " restart ADSL...succeed!");
                            m_taskstatus = TaskStatus.Restart;
                        }
                        else
                        {
                            GetLogger().LogListLog(VBALogLevelFlag.Debug, this.Name + " restarting ADSL failed... setting this task failure!");
                            m_taskstatus = TaskStatus.Failure;
                        }
                        break;                     
                    case VBATaskStatus.Ready:
                        m_taskstatus = TaskStatus.Succeed;
                        GetLogger().LogListLog(VBALogLevelFlag.Debug, this.Name + " succeed...");
                        break;
                    case VBATaskStatus.Error:
                        m_taskstatus = TaskStatus.Error;
                        GetLogger().LogListLog(VBALogLevelFlag.Error, this.Name + " set error by customer...");
                        break;
                    case VBATaskStatus.Close:
                        m_taskstatus = TaskStatus.Succeed;
                        GetLogger().LogListLog(VBALogLevelFlag.Warn, this.Name + " close whole domain!");
                        Domain d = DomainManager.GetDomain(m_domainGUID);
                        if (d.Enable) d.Enable = false;
                        break;
                    case VBATaskStatus.TurnToNext:
                        m_taskstatus = TaskStatus.Succeed;
                        GetLogger().LogListLog(VBALogLevelFlag.Warn, this.Name + " close and turn to next domain!");
                        Domain dd = DomainManager.GetDomain(m_domainGUID);
                        if (dd.Enable || dd.Priority == DomainPriority.OneTime) DomainManager.TurnToNextDomain(m_domainGUID);
                        break;
                    default:
                        m_taskstatus = TaskStatus.Succeed;
                        GetLogger().LogListLog(VBALogLevelFlag.Debug, this.Name + " succeed...");
                        break;
                }
            }
            catch (Exception e)
            {
                if (m_IsClosing)
                {
                    log.Warn(this.Name + " closed by force...");
                    GetLogger().LogListLog(VBALogLevelFlag.Warn, this.Name + " closed...");                   
                    m_taskstatus = TaskStatus.Closing;
                }
                else
                {
                    GetLogger().LogListLog(VBALogLevelFlag.Error, this.Name + " runtime error..." + ParserErrorString(e));
                    log.Error(this.Name + " runtime error: ", e);
                    m_errortime++;                    
                    m_taskstatus = TaskStatus.Error;
                }
            }
            finally
            {
                //实现AfterTask事件
                if (AfterScript != null)
                    AfterScript(this, null);
            }
        }

        /// <summary>
        ///得到内部的VBALog对象
        /// </summary>
        protected VBALog GetLogger()
        {
            return m_vbaobjs["Log"] as VBALog;
        }

        /// <summary>
        ///重写该方法,得到运行用Script
        /// </summary>
        protected virtual string GetScript()
        {
            return "";
        }

        /// <summary>
        ///重写该方法,得到时间戳
        /// </summary>
        protected virtual DateTime GetTimeStamp()
        {
            return DomainManager.GetDomain(m_domainGUID).TimeStamp;
        }

        /// <summary>
        ///判断VBAobject的生命周期
        /// </summary>
        protected virtual void RefreshVBAObjects()
        {
            foreach (IVBAObject o in m_vbaobjs.Values)
                if (o.Life == VBAObjectLife.Task)
                    o.Reset();
        }
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseVBAScriptTask()
        {

        }

        /// <summary>
        /// 构造函数,输入站点DomainGUID, 脚本Script, IVBAObject数组对象
        /// </summary>
        public BaseVBAScriptTask(string dGUID, Dictionary<string, IVBAObject> vobjs)
        {
            m_domainGUID = dGUID;            
            m_vbaobjs = vobjs;
            m_timestamp = GetTimeStamp();
        }

        public BaseVBAScriptTask(string domainGUID, string taskchainGUID, Dictionary<string, IVBAObject> vobjs)
        {
            m_domainGUID = domainGUID;
            m_taskchainGUID = taskchainGUID;
            m_vbaobjs = vobjs;
            m_timestamp = GetTimeStamp();
        }

        protected BaseVBAScriptTask(System.Runtime.Serialization.SerializationInfo s, System.Runtime.Serialization.StreamingContext c)
        {
            m_runningtime = s.GetInt32("RunningTime");
            m_errortime = s.GetInt32("ErrorTime");
            m_domainGUID = s.GetString("DomainGUID");
            m_taskchainGUID = s.GetString("TaskChainGUID");
            m_timestamp = (DateTime)s.GetValue("TimeStamp", m_timestamp.GetType());
            m_taskstatus  = (TaskStatus)s.GetValue("Status", m_taskstatus.GetType());
            m_vbaobjs = (Dictionary<string, IVBAObject>)s.GetValue("VBAObjects", m_vbaobjs.GetType());
        }
        #endregion

        #region 新增公有成员

        /// <summary>
        /// 将VBALog中的日志写入数据库, 只在服务器端执行
        /// </summary>  
        public virtual void DoDBLogging()
        {
            //保存log到数据库
            LogDatabaseManager.WriteVBALogIntoDB(m_domainGUID, m_taskchainGUID, this.Name, GetLogger());
        }

        /// <summary>
        ///Task自带的时间戳, 来自构造它的Domain
        /// </summary>  
        public DateTime TimeStamp
        {
            get { return m_timestamp; }
            set { m_timestamp = value; }
        }

        #endregion

        #region ITask 成员

        public virtual string Name { get { return "Base Task"; } }

        public string DomainGUID
        {
            get
            {
                return m_domainGUID;
            }
            set
            {
                m_domainGUID = value;
            }
        }

        public string TaskChainGUID
        {
            get { return m_taskchainGUID; }
        }

        public Dictionary<string, IVBAObject> VBAObjs
        {
            get
            {
                return m_vbaobjs;
            }
            set
            {
                m_vbaobjs = value;
            }
        }

        public int RunningTime
        {
            get
            {
                return m_runningtime;
            }

        }

        public int ErrorTime
        {
            get
            {
                return m_errortime;
            }
            set
            {
                m_errortime = value;
            }
        }

        public TaskStatus Status
        {
            get
            {
                return m_taskstatus;
            }
        }

        public virtual void Run()
        {
            try
            {
                m_taskstatus = TaskStatus.Ready;
                (m_vbaobjs["Task"] as VBATask).CurrentVBATaskStatus = VBATaskStatus.Ready;
                m_runningtime++;

                m_script = GetScript();
                
                if (m_script != null && m_script != "")
                {

                    EngineClose();

                    //实例化一个新引擎
                    m_engine = VBAStaticEngine.RentVBAEngine(m_script);
                    
                    //实现VBAObject注入
                    m_engine.Injection(m_vbaobjs);                    
                }

                //新开一个线程                   
                m_scriptthread = new Thread(ScriptThreadRunner);
                m_scriptthread.Start();
            }
            catch (Exception err)
            {
                GetLogger().Fatal(this.Name + err.Message);
                m_taskstatus = TaskStatus.Failure;
                //实现AfterTask事件
                if (AfterScript != null)
                    AfterScript(this, null);
            }
        }

        public virtual void Stop()
        {
            lock (locker)
            {
                if (m_engine != null)
                {
                    //关闭引擎
                    m_IsClosing = true;
                    m_engine.Stop();
                }
            }
        }

        /// <summary>
        ///重写该类,得到应该执行的任务(包括后续任务和新任务链)
        /// </summary>        
        public virtual List<ITask> NextTasks()
        {
            RefreshVBAObjects();
            return new List<ITask>();
        }

        public event EventHandler BeforeScript = null;

        public event EventHandler AfterScript = null;

        #endregion

        #region ISerializable 成员

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("RunningTime", m_runningtime);
            info.AddValue("ErrorTime", m_errortime);
            info.AddValue("DomainGUID", m_domainGUID);
            info.AddValue("TaskChainGUID", m_taskchainGUID);
            info.AddValue("TimeStamp", m_timestamp);
            info.AddValue("VBAObjects", m_vbaobjs);
            info.AddValue("Status", m_taskstatus);
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

                //关闭引擎
                EngineClose();
                if (m_vbaobjs != null)
                    foreach (IVBAObject o in m_vbaobjs.Values)
                        o.Dispose();
            }  
            IsDisposed=true;  
        }

        ~BaseVBAScriptTask()  
        {  
            Dispose(false);  
        } 

        #endregion

     }
}
