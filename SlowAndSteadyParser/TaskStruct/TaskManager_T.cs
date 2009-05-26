using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;

namespace SlowAndSteadyParser
{
    public class TaskManager<T> : ITaskManager<T> where T:ITask
    {
        //多线程锁相关
        protected object tasklocker = new object();
        private Thread m_managerthread = null;
        private Thread m_finishthread = null;
        protected int m_threadnumber = 5;
        private Random sleeprand = new Random();

        //log
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Attributes
        private string m_name = "Standard Task Manager";
        protected int m_maxretrytime = 1;
        protected bool m_IsTaskManagerRunning = false;

        //Queue for tasks
        protected Queue<T> m_WaitingLocalTasks = new Queue<T>();
        protected List<T> m_RunningLocalTasks = new List<T>();
        protected Queue<T> m_FinishedTasks = new Queue<T>();

        #region Construction
        public TaskManager()
        {
            log.Debug("Init TaskManager.Watcher Thread");
            if (m_managerthread == null)
            {
                m_managerthread = new Thread(PassLocalTaskQueueThread);
                m_managerthread.Name = "Watcher";
                m_managerthread.Start();
            }

            if (m_finishthread == null)
            {
                m_finishthread = new Thread(PassFinishTaskQueueThread);
                m_finishthread.Name = "Sweaper";
                m_finishthread.Start();
            }
        }
        #endregion

        #region Private Functions

        private void PassLocalTaskQueueThread()
        {
            while (true)
            {
                lock (tasklocker)
                {
                    //当运行任务数超过m_threadnumber 或者 排队中的任务数为0的时候,就暂停
                    while (!m_IsTaskManagerRunning || m_RunningLocalTasks.Count >= m_threadnumber || m_WaitingLocalTasks.Count == 0)
                        Monitor.Wait(tasklocker);

                    T t = m_WaitingLocalTasks.Dequeue();
                    m_RunningLocalTasks.Add(t);
                    //给当前Task绑定事件处理
                    AttachEventHandler(t);
                    t.Run();
                }
                    //Thread.Sleep(sleeprand.Next(1000, 2000));             
            }
        }

        private void PassFinishTaskQueueThread()
        {
            while (true)
            {
                lock (m_FinishedTasks)
                {
                    //当运行任务数超过m_threadnumber 或者 排队中的任务数为0的时候,就暂停
                    while (m_FinishedTasks.Count == 0)
                        Monitor.Wait(m_FinishedTasks);

                    AfterTaskFinish();
                }
            }
        }

        protected void AttachEventHandler(T whichtask)
        {            
            whichtask.BeforeScript += new EventHandler(BeforeScriptEventHandler);            
            whichtask.AfterScript += new EventHandler(AfterScriptEventHandler);
        }

        protected void UnattachEventHandler(T whichtask)
        {
            whichtask.BeforeScript -= BeforeScriptEventHandler;
            whichtask.AfterScript -= AfterScriptEventHandler;
        }

        protected virtual void BeforeScriptEventHandler(object sender, EventArgs e)
        {

        }

        protected virtual void AfterScriptEventHandler(object sender, EventArgs e)
        {
            AddProcessedTask((T)sender);
        }

        protected virtual void AfterTaskFinish()
        {
            T whichtask = default(T);

            lock (m_FinishedTasks)
            {
                whichtask = m_FinishedTasks.Dequeue();
                //放行任务队列
                Monitor.Pulse(m_FinishedTasks);
            }
    
            switch (whichtask.Status)
            {
                case TaskStatus.Error:
                    if (whichtask.ErrorTime < m_maxretrytime)
                    {
                        log.Info(whichtask.Name + " Error, Retry " + whichtask.RunningTime);
                        UnattachEventHandler(whichtask);
                        AddTask(whichtask);
                    }
                    else
                    {
                        log.Info(whichtask.Name + " Error Too Much Times...Abandoning..");
                        whichtask.Dispose();
                    }
                    break;
                case TaskStatus.Restart:
                    log.Info(whichtask.Name + " Restart...");
                    UnattachEventHandler(whichtask);
                    AddTask(whichtask);
                    break;
                case TaskStatus.Succeed:
                    log.Info(whichtask.Name + " Succeed...");
                    List<ITask> tnexts = whichtask.NextTasks();
                    if (tnexts != null)
                    {
                        foreach (ITask t in tnexts)
                        {
                            log.Info("Further Task Spawning..." + t.Name);
                            AddTask((T)t);
                        }
                    }
                    whichtask.Dispose();
                    break;
                case TaskStatus.Failure:
                    log.Info(whichtask.Name + " Fail...");
                    whichtask.Dispose();
                    break;
                case TaskStatus.Closing:
                    whichtask.Dispose();
                    break;
            } 
        }

        public virtual void AddProcessedTask(T task)
        {
            lock (tasklocker)
            {
                if (m_RunningLocalTasks.Contains(task))
                    m_RunningLocalTasks.Remove(task);
                //放行任务队列
                Monitor.Pulse(tasklocker);
            }

            lock (m_FinishedTasks)
            {
                m_FinishedTasks.Enqueue(task);
                //放行任务队列
                Monitor.Pulse(m_FinishedTasks);
            }
        }

        #endregion

        #region Public Functions

        public virtual bool AddTask(T task)
        {
            lock (tasklocker)
            {
                if (!m_WaitingLocalTasks.Contains(task))
                {                   
                    //Task入队
                    m_WaitingLocalTasks.Enqueue(task);
                    //发出信号,通知Task处理线程工作
                    Monitor.Pulse(tasklocker);
                    return true;
                }
                return false;
            }
        }

        public virtual bool RemoveTask(T task)
        {
            return false;
        }
            
        public void Start()
        {
            lock (tasklocker)
            {
                if (!m_IsTaskManagerRunning)
                {
                    m_IsTaskManagerRunning = true;
                    Monitor.Pulse(tasklocker);
                }
            }
        }    

        public void Stop()
        {
            lock (tasklocker)
            {
                m_IsTaskManagerRunning = false;
                foreach (T t in m_RunningLocalTasks)
                {
                    t.Stop();
                }
            }
        }

        #endregion     

        #region Public Attribute

        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        public bool IsRunning
        {
            get { return m_IsTaskManagerRunning; }
        }

        public int MaxRetryTime
        {
            get { return m_maxretrytime; }
            set { if (value >= 0) m_maxretrytime = value; }
        }

        public int MaxRunningThread
        {
            get { return m_threadnumber; }
            set { if (value > 0) m_threadnumber = value; }
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
                //清理非托管资源
                if (m_managerthread != null)
                    m_managerthread.Abort();
                m_managerthread = null;

                if (m_finishthread != null)
                    m_finishthread.Abort();
                m_finishthread = null;

                if (Disposing)
                {
                    //清理托管资源                    
                    log.Info("TaskManager Disposing...");
                }
            }  
            IsDisposed=true;  
        }

        ~TaskManager()
        {  
            Dispose(false);  
        }
        #endregion

    }
}
