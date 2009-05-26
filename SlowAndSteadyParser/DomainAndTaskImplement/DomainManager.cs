using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace SlowAndSteadyParser
{
    public static class DomainManager
    {
        private static bool ms_IsServerModeRemote = true;

        public static void SetServerModeRemote()
        {
            ms_IsServerModeRemote = true;
        }

        public static void SetServerModeLocal()
        {
            ms_IsServerModeRemote = false;
        }

        private static Dictionary<string,Domain> m_DomainList = new Dictionary<string, Domain>();
        
        public static Dictionary<string,Domain> DomainList
        {
            get{return m_DomainList;}
            set
            {
                if (value !=null)
                    m_DomainList = value;
            }
        }

        public static Domain NewDomain()
        {
            Domain d = new Domain();
            d.Name = d.DomainGUID;
            AddDomain(d);
            return d;
        }

        public static void AddDomain(Domain whichdomain)
        {
            lock (m_DomainList)
            {
                if (!m_DomainList.ContainsKey(whichdomain.DomainGUID))
                    m_DomainList.Add(whichdomain.DomainGUID, whichdomain);
                else
                    m_DomainList[whichdomain.DomainGUID] = whichdomain;
            }
        }

        public static void RemoveDomain(Domain whichdomain)
        {
            lock (m_DomainList)
            {
                if (m_DomainList.ContainsKey(whichdomain.DomainGUID))
                    m_DomainList.Remove(whichdomain.DomainGUID);
            }
            
        }

        public static Domain GetDomain(string DomainGUID)
        {
            if (m_DomainList.ContainsKey(DomainGUID))
                    return m_DomainList[DomainGUID];
            return null;
        }

        public static bool IsDomainNeedUpdated(string domainGUID, DateTime timestamp)
        {
            if (m_DomainList.ContainsKey(domainGUID))
            {
                Domain d = m_DomainList[domainGUID];
                if (d.TimeStamp >= timestamp)
                    return false;
                else
                    return true;
            }
            else
                return true;
        }

        public static void InitDomainForTaskManager(ITaskManager<IRemoteTask> tm, Domain whichdomain)
        {
            if (!m_DomainList.ContainsValue(whichdomain))
                AddDomain(whichdomain);

            if (ms_IsServerModeRemote)
                tm.AddTask(new RemotePreparationVBAScriptTask(whichdomain));         
            else
                tm.AddTask(new LocalPreparationVBAScriptTask(whichdomain));

        }

        public static bool IsDomainExist(string DomainGUID)
        {
            return m_DomainList.ContainsKey(DomainGUID);
        }

        public static void TaskManagerAddTaskFromAvaibleDomains(ITaskManager<IRemoteTask> tm)
        {
            foreach (Domain d in m_DomainList.Values)
                if (d.Enable)
                {
                    Random r = new Random();
                    switch (d.Priority)
                    {
                        case DomainPriority.Emergency:
                            InitDomainForTaskManager(tm, d);
                            break;
                        case DomainPriority.Important:
                            if (r.NextDouble() < 0.8)
                                InitDomainForTaskManager(tm, d);
                            break;
                        case DomainPriority.General:
                            if (r.NextDouble() < 0.6)
                                InitDomainForTaskManager(tm, d);
                            break;
                        case DomainPriority.Subordinary:                            
                            if (r.NextDouble() < 0.4)
                                InitDomainForTaskManager(tm, d);
                            break;
                        case DomainPriority.Insignificant:
                            if (r.NextDouble() < 0.2)
                                InitDomainForTaskManager(tm, d);
                            break;
                        case DomainPriority.OneTime:
                            InitDomainForTaskManager(tm, d);
                            d.Enable = false;
                            break;
                    }
                }                
        }

        public static bool IsHavingAvaibleDomain
        {
            get
            {
                int i = 0;
                foreach (Domain d in m_DomainList.Values)
                    if (d.Enable) i++;
                return (i>0);
            }
        }

        public static string DomainInfoToString()
        {
            int i = 0;
            string s = "启用的解决方案:"+Environment.NewLine;
            foreach (Domain d in m_DomainList.Values)
            {
                if (d.Enable)
                {
                    i++;
                    s = s + d.Name +" / "+d.TotalRunningTime+ Environment.NewLine;
                }

            }
            if (i == 0)
                s = s + "   无" + Environment.NewLine;
            return s;
        }

        public static void TurnToNextDomain(string DomainGUID)
        {
            lock (m_DomainList)
            {
                Domain currentDomain = GetDomain(DomainGUID);
                currentDomain.Enable = false;
                //domain
                SortedDictionary<string, Domain> sortedbyname = new SortedDictionary<string, Domain>();
                foreach (Domain d in m_DomainList.Values)
                {
                    sortedbyname.Add(d.Name, d);
                }
                IEnumerator domainenum = sortedbyname.Values.GetEnumerator();
                while (domainenum.MoveNext())
                {
                    if (domainenum.Current == currentDomain)
                    {
                        if (domainenum.MoveNext())
                        {
                            (domainenum.Current as Domain).Enable = true;
                        }
                        break;
                    }
                }                
            }
        }

    }
}
