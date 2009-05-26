using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Lifetime;
using System.Threading;

namespace SlowAndSteadyParser
{
    [Serializable]
    public class TaskTransmissionManager : MarshalByRefObject, ITaskTransmissionManager, ISerializable, IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Queue<IRemoteTask> m_waitingclienttasks = new Queue<IRemoteTask>();
        private TaskManagerServer m_taskmanager = null;
        private bool m_enable = false;
        private Thread m_monitorthread = null;
        private int m_MinimalAcceptableWaitingclienttask = 10;

        public bool Enable
        {
            get { return m_enable; }
        }

        public void Start()
        {
            if (!m_enable)
                m_enable = true;
        }

        public void Stop()
        {
            if (m_enable)
            {
                m_enable = false;
            }
        }

        private void CheckAvailableClientTaskResourceThread()
        {
            lock (m_waitingclienttasks)
            {
                while (true)
                {
                    if (ServerControlManager.IsServerModeRemote)
                    {
                        while ( 
                                (ServerControlManager.IsServerModeRemote == true) &&
                                (
                                    !m_enable ||
                                    !DomainManager.IsHavingAvaibleDomain ||
                                    m_taskmanager.CountOfAllTask + m_waitingclienttasks.Count >= m_MinimalAcceptableWaitingclienttask 
                                 )
                              )

                            Monitor.Wait(m_waitingclienttasks, 30000);                        
                    }
                    else
                    {
                        while (
                                (ServerControlManager.IsServerModeRemote == false) && 
                                (
                                    !m_enable || 
                                    !DomainManager.IsHavingAvaibleDomain || 
                                    m_taskmanager.CountOfAllTask >= m_MinimalAcceptableWaitingclienttask
                                )
                              )

                            Monitor.Wait(m_waitingclienttasks,3000);                    
                    }

                    DomainManager.TaskManagerAddTaskFromAvaibleDomains(m_taskmanager);
                }
            }
        }

        public void CheckAvailableClientTaskResource()
        {
            lock (m_waitingclienttasks)
            {
                Monitor.Pulse(m_waitingclienttasks);
            }
        }

        public int MinimalAcceptableWatingClientTask
        {
            get { return m_MinimalAcceptableWaitingclienttask; }
            set { if (value>0) m_MinimalAcceptableWaitingclienttask = value; }
        }

        public int CountOfWaitingClientTask
        {
            get
            {
                return m_waitingclienttasks.Count;
            }
        }

        public void RemoveAllWaitingTask()
        {
            lock (m_waitingclienttasks)
            {
                while (m_waitingclienttasks.Count > 0)
                {
                    m_waitingclienttasks.Dequeue().Dispose();
                }         
            }
        }

        #region ITaskTransmissionManager 成员

        public ServerStatus GetServerStatus()
        {
            CheckAvailableClientTaskResource();
            if (ServerControlManager.IsServerRunning)
                return ServerStatus.Online;
            else
                return ServerStatus.Offline;
        }
        
        public TaskManagerServer TaskManagerServer
        {
            get { return m_taskmanager; }
            set { m_taskmanager = value; }
        }

        public IRemoteTask FetchAClientTask()
        {
            CheckAvailableClientTaskResource();
            lock (m_waitingclienttasks)
            {
                if (m_enable && m_waitingclienttasks.Count > 0)
                {
                    IRemoteTask t = m_waitingclienttasks.Dequeue();
                    return t;
                }
                else
                    return null;
            }
        }

        public List<IRemoteTask> FetchClientTasks(int maxnumber)
        {
            CheckAvailableClientTaskResource();

            lock (m_waitingclienttasks)
            {
                List<IRemoteTask> lrt = new List<IRemoteTask>();
                if (m_enable)
                {
                    for (int i = 1; i <= maxnumber; i++)
                    {
                        if (m_waitingclienttasks.Count > 0)
                            lrt.Add(m_waitingclienttasks.Dequeue());
                    }
                }
                return lrt;
            }
        }

        public bool SendbackAClientTask(IRemoteTask whichtask)
        {
            try
            {
                if (m_enable)
                {
                    IRemoteTask t1 = whichtask.Clone() as IRemoteTask;
                    m_taskmanager.AddProcessedTask(t1);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                log.Error("Sending Back", e);
                return false;
            }
        }

        public bool SendbackClientTasks(List<IRemoteTask> tasks)
        {
            try
            {
                if (m_enable)
                {
                    IRemoteTask t1;
                    foreach (IRemoteTask r in tasks)
                    {
                        t1 = r.Clone() as IRemoteTask;
                        m_taskmanager.AddProcessedTask(t1);
                    }
                    return true;
                }
                return false;
            }
            catch(Exception e)
            {
                log.Error("Sending Back", e);
                return false;
            }
        }

        public bool AddClientTask(IRemoteTask whichtask)
        {
            if (m_enable)
            {
                m_waitingclienttasks.Enqueue(whichtask);
                return true;
            }
            return false;
        }

        #endregion

        #region MarshalByRefObject 成员
        public override object InitializeLifetimeService()
        {
            ILease lease = (ILease)base.InitializeLifetimeService();
            if (lease.CurrentState == LeaseState.Initial)
            {
                lease.InitialLeaseTime = TimeSpan.Zero;                
            }
            return lease;
        }
        #endregion

        #region 构造函数

        public TaskTransmissionManager()
        {
            log.Debug("Init TaskTransmissionManager.Monitor Thread");
            if (m_monitorthread == null)
            {
                m_monitorthread = new Thread(CheckAvailableClientTaskResourceThread);
                m_monitorthread.Name = "Monitor";
                m_monitorthread.Start();
            }
        }

        protected TaskTransmissionManager(SerializationInfo info, StreamingContext context)
        {
            m_waitingclienttasks = info.GetValue("WaitingTasks", m_waitingclienttasks.GetType()) as Queue<IRemoteTask>;
            m_MinimalAcceptableWaitingclienttask = info.GetInt32("ReservedPoolTask");

            log.Debug("Init TaskTransmissionManager.Monitor Thread");
            if (m_monitorthread == null)
            {
                m_monitorthread = new Thread(CheckAvailableClientTaskResourceThread);
                m_monitorthread.Name = "Local/Remote Task Feeding Thread";
                m_monitorthread.Start();
            }
        }

        #endregion

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("WaitingTasks", m_waitingclienttasks);
            info.AddValue("ReservedPoolTask", m_MinimalAcceptableWaitingclienttask);
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (m_monitorthread != null)
                m_monitorthread.Abort();
            m_monitorthread = null;
            m_waitingclienttasks = null;
            log.Info("TaskTransmissionManager Disposing...");
        }

        #endregion

    }
}
