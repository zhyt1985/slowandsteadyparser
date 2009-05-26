using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;

namespace SlowAndSteadyParser
{
    [Serializable]
    public class TaskManagerClient : TaskManager<IRemoteTask>, IDisposable, ISerializable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Thread m_connectionthread = null;
        private string m_name = "Task Manager Client";
        protected int m_SingerTransportation = 3;
        protected int m_QueryServerInterval = 8000;
        protected Queue<IRemoteTask> m_WaitingServerTasks = new Queue<IRemoteTask>();

        private Domain GetDomainFromServer(string domainGUID)
        {
            Domain d = null;
            try
            {
                IDomainTransmissionManager mgr = ConnectionManagerClient.GetDomainTransmissionManager();
                d = mgr.GetDomain(domainGUID);
                return d;
            }
            catch (NullReferenceException e)
            {
                log.Error("尝试下载一个不存在的Domian", e);
                return d;
            }
            catch (Exception e)
            {
                log.Error("GetDomainFromServer未知错误: ", e);
                return d;
            }
        }

        public int QueryServerInterval
        {
            get { return m_QueryServerInterval; }
            set { if (value > 0) m_QueryServerInterval = value; }
        }

        public int SingleTranspotation
        {
            get { return m_SingerTransportation; }
            set { if (value > 0) m_SingerTransportation = value; }
        }

        private void RefreshServerStatus()
        {
            ConnectionManagerClient.FetchServerStatus();
        }

        private void DownloadTasks()
        {
            BaseVBAScriptTask bt = null;
            if (m_WaitingLocalTasks.Count <= m_SingerTransportation)
            {
                log.Debug("Start Download...");
                List<IRemoteTask> ts = ConnectionManagerClient.TTMFeachTasks(m_SingerTransportation);
                if (ts != null && ts.Count > 0)
                {
                    foreach (IRemoteTask t in ts)
                    {
                        bt = t as BaseVBAScriptTask;
                        if (DomainManager.IsDomainNeedUpdated(bt.DomainGUID, bt.TimeStamp))
                        {
                            Domain d = GetDomainFromServer(bt.DomainGUID);
                            if (d != null)
                            {
                                DomainManager.AddDomain(d);
                                log.Debug("Fetching A Domain for " + t.Name);
                            }
                            else
                            {
                                log.Debug("Weird DomainGUID With No Domian IN SERVER!:" + bt.DomainGUID);
                            }
                            
                        }
                        this.AddTask(t);
                        log.Debug("Fetching one " + t.Name);
                    }
                }
                else
                    log.Debug("No avaible remotetask for downloading...");         
            }

        }

        private void UploadTasks()
        {
            lock (m_WaitingServerTasks)
            {                
                if (m_WaitingServerTasks.Count > 0)
                {
                    log.Debug("Start Uploading..."+m_WaitingServerTasks.Count.ToString());

                    List<IRemoteTask> tl = new List<IRemoteTask>();

                    while (m_WaitingServerTasks.Count > 0)
                        tl.Add(m_WaitingServerTasks.Dequeue());

                    if (!ConnectionManagerClient.TTMSendBackTasks(tl))
                        foreach (IRemoteTask t in tl)
                            m_WaitingServerTasks.Enqueue(t);
                    else
                        foreach (IRemoteTask t in tl)
                            t.Dispose();
                }
            }
        }

        private void PassServerTaskQueueThread()
        {
            while (true)
            {
                //刷新服务器状态
                RefreshServerStatus();

                if (m_IsTaskManagerRunning)
                {
                    //下载任务
                    DownloadTasks();
                    //上传任务
                    UploadTasks();

                    lock (tasklocker)
                    {
                        Monitor.Pulse(tasklocker);
                    }
                }
                Thread.Sleep(m_QueryServerInterval);
            }
        }

        private void SetTaskSent(IRemoteTask t)
        {
            lock (m_WaitingServerTasks)
            {
                m_WaitingServerTasks.Enqueue(t);
            }            
        }

        protected override void AfterTaskFinish()
        {
            IRemoteTask whichtask = null;

            lock (m_FinishedTasks)
            {
                whichtask = m_FinishedTasks.Dequeue();
                //放行任务队列
                Monitor.Pulse(m_FinishedTasks);
            }
            
            switch (whichtask.Status)
            {
                case TaskStatus.Error:
                    //如果执行有错误的话,考虑其是否超过最大重试次数,如果没有超过就重新入队
                    if (whichtask.ErrorTime <= m_maxretrytime)
                    {
                        log.Info(whichtask.Name + " Error, Retry " + whichtask.RunningTime);
                        base.UnattachEventHandler(whichtask);
                        AddTask(whichtask);
                    }
                    else
                    {
                        log.Info(whichtask.Name + " Error Too Much Times...Abandoning..");
                        base.UnattachEventHandler(whichtask);
                        SetTaskSent(whichtask);                                
                    }
                    break;
                case TaskStatus.Restart:
                    log.Info(whichtask.Name + " Restart...");
                    base.UnattachEventHandler(whichtask);
                    AddTask(whichtask);
                    break;
                case TaskStatus.Succeed:
                    log.Info(whichtask.Name + " Succeed...Sending back...");                    
                    base.UnattachEventHandler(whichtask);
                    SetTaskSent(whichtask);  
                    break;
                case TaskStatus.Failure:
                    log.Info(whichtask.Name + " Fail...");
                    base.UnattachEventHandler(whichtask);
                    SetTaskSent(whichtask);
                    break;
                case TaskStatus.Closing:
                    log.Info(whichtask.Name + " is Closing...");
                    base.UnattachEventHandler(whichtask);
                    SetTaskSent(whichtask);
                    break;
            }                
        }

        public override bool AddTask(IRemoteTask task)
        {
            if (task.Position == TaskPosition.Client)
                return base.AddTask(task);
            else if (task.Position == TaskPosition.Server)
            {
                lock (m_WaitingServerTasks)
                {
                    m_WaitingServerTasks.Enqueue(task);
                }
                return true;
            }
            else
                throw new Exception("远程对象IRemoteTask.Position位置错误: " + task.Position);
        }

        #region Construction

        public TaskManagerClient() : base()
        {
            log.Debug("Init TaskManagerClient.Connection Thread");
            if (m_connectionthread == null)
            {
                m_connectionthread = new Thread(PassServerTaskQueueThread);
                m_connectionthread.Name = "Conntection";
                m_connectionthread.Start();
            }
        }

        protected TaskManagerClient(SerializationInfo info, StreamingContext context)
        {
            //m_WaitingServerTasks = info.GetValue("ServerWaitingTasks", m_WaitingLocalTasks.GetType()) as Queue<IRemoteTask>;         

            log.Debug("Init TaskManagerClient.Connection Thread");
            if (m_connectionthread == null)
            {
                m_connectionthread = new Thread(PassServerTaskQueueThread);
                m_connectionthread.Name = "Conntection";
                m_connectionthread.Start();
            }
        }

        #endregion

        #region IDisposable 成员

        public new void Dispose()
        {
            //销毁Conntection线程
            if (m_connectionthread != null)
                m_connectionthread.Abort();
            m_connectionthread = null;

            //调用基类Dispose方法
            base.Dispose();
        }

        #endregion

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue("ServerWaitingTasks", m_WaitingServerTasks);
        }

        #endregion
    }
}
