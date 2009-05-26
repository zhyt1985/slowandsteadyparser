using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;

namespace SlowAndSteadyParser
{
    [Serializable]
    public class TaskManagerServer : TaskManager<IRemoteTask>, IDisposable, ISerializable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        protected TaskTransmissionManager m_ttm = null;

        #region TaskManagerServer专属

        private void ProcessBeforDisposedSucceedTask(BaseVBAScriptTask basetask)
        {
            //将Log写入数据库
            basetask.DoDBLogging();

            //更新统计信息
            DomainManager.GetDomain(basetask.DomainGUID).IncreaseTotalTask();
            DomainManager.GetDomain(basetask.DomainGUID).IncreaseSucceedTask();
            DomainManager.GetDomain(basetask.DomainGUID).IncreaseRunningTime(basetask.RunningTime);
            DomainManager.GetDomain(basetask.DomainGUID).IncreaseErrorTime(basetask.ErrorTime);
        }

        private void ProcessBeforDisposedFailureTask(BaseVBAScriptTask basetask)
        {
            //将Log写入数据库
            basetask.DoDBLogging();

            //更新统计信息
            DomainManager.GetDomain(basetask.DomainGUID).IncreaseTotalTask();
            DomainManager.GetDomain(basetask.DomainGUID).IncreaseRunningTime(basetask.RunningTime);
            DomainManager.GetDomain(basetask.DomainGUID).IncreaseErrorTime(basetask.ErrorTime);
        }

        public TaskTransmissionManager TaskTransmissionManager
        {
            get { return m_ttm; }
            set { m_ttm = value; }
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

            //如果执行有错误的话,考虑其是否超过最大重试次数,如果没有超过就重新入队
            switch (whichtask.Status)
            {
                case TaskStatus.Error:
                    if (whichtask.ErrorTime <= m_maxretrytime && whichtask.Position == TaskPosition.Server)
                    {
                        log.Info(whichtask.Name + " Error, Retry " + whichtask.RunningTime);
                        base.UnattachEventHandler(whichtask);
                        AddTask(whichtask);
                    }
                    else
                    {
                        log.Info(whichtask.Name + " Error Too Much Times...Abandoning..");
                        //销毁前处理
                        ProcessBeforDisposedFailureTask(whichtask as BaseVBAScriptTask);
                        //注销该任务
                        base.UnattachEventHandler(whichtask);
                        whichtask.Dispose();
                    }
                    break;
                case TaskStatus.Restart:
                    log.Info(whichtask.Name + " Restart...");
                    base.UnattachEventHandler(whichtask);
                    AddTask(whichtask);
                    break;
                case TaskStatus.Succeed:
                    log.Info(whichtask.Name + " Succeed...");
                    //销毁前处理
                    ProcessBeforDisposedSucceedTask(whichtask as BaseVBAScriptTask);
                    List<ITask> tnexts = whichtask.NextTasks();
                    if (tnexts != null)
                    {
                        foreach (ITask t in tnexts)
                        {
                            log.Info("Further Task Spawning..." + t.Name);
                            AddTask((IRemoteTask)t);
                        }
                    }
                    base.UnattachEventHandler(whichtask);
                    whichtask.Dispose();
                    break;
                case TaskStatus.Failure:
                    log.Info(whichtask.Name + " Fail...");
                    //销毁前处理
                    ProcessBeforDisposedFailureTask(whichtask as BaseVBAScriptTask);
                    base.UnattachEventHandler(whichtask);
                    whichtask.Dispose();
                    break;
                case TaskStatus.Closing:
                    //销毁前处理
                    ProcessBeforDisposedFailureTask(whichtask as BaseVBAScriptTask);
                    base.UnattachEventHandler(whichtask);
                    whichtask.Dispose();
                    break;
                case TaskStatus.Abandoning:
                    //销毁前处理
                    ProcessBeforDisposedFailureTask(whichtask as BaseVBAScriptTask);
                    base.UnattachEventHandler(whichtask);
                    whichtask.Dispose();
                    break;
            }   
        }

        public override bool AddTask(IRemoteTask task)
        {
            if (task.Position == TaskPosition.Server)
                return base.AddTask(task);
            else if (task.Position == TaskPosition.Client)
                return m_ttm.AddClientTask(task);
            else
                throw new Exception("远程对象IRemoteTask.Position位置错误: " + task.Position);
        }

        //public void RemoveTaskFromDomainGUID(string domainGUID)
        //{
        //    lock (locker)
        //    {
        //        IRemoteTask[] r = m_WaitingLocalTasks.ToArray();
        //        m_WaitingLocalTasks.Clear();
        //        foreach (IRemoteTask t in r)
        //            if ((t as BaseVBAScriptTask).DomainGUID != domainGUID)
        //                m_WaitingLocalTasks.Enqueue(t);
        //    }
        //}

        public void RemoveAllWaitingTask()
        {
            lock (tasklocker)
            {
                while (m_WaitingLocalTasks.Count > 0)
                {
                    m_WaitingLocalTasks.Dequeue().Dispose();
                }                
            }
        }    

        public int CountOfAllTask
        {
            get
            {
                lock (tasklocker)
                {
                    return m_WaitingLocalTasks.Count + m_RunningLocalTasks.Count;
                }
            }
        }

        public int CountOfWaitingTask
        {
            get
            {
                return m_WaitingLocalTasks.Count;                
            }
        }

        public int CountOfRunningTask
        {
            get
            {
                return m_RunningLocalTasks.Count;                
            }
        }

        #endregion

        #region 构造函数

        public TaskManagerServer():base()
        {
        }

        protected TaskManagerServer(SerializationInfo info, StreamingContext context)
        {
            lock (tasklocker)
            {
                m_threadnumber = info.GetInt32("ThreadNum");
                m_maxretrytime = info.GetInt32("RetryTime");
                m_WaitingLocalTasks = info.GetValue("LocalWaitingTasks", m_WaitingLocalTasks.GetType()) as Queue<IRemoteTask>;                
            }
        }

        #endregion

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            lock (tasklocker)
            {
                info.AddValue("ThreadNum", m_threadnumber);
                info.AddValue("RetryTime", m_maxretrytime);
                info.AddValue("LocalWaitingTasks", m_WaitingLocalTasks);
            }
        }

        #endregion

        
    }
}
