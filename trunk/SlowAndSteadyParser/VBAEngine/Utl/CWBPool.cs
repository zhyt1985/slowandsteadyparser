using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SlowAndSteadyParser
{
    public static class CWBPool
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static object locker = new object();
        
        private const int m_PreparedCWBNum = 1;
        private static System.Windows.Forms.Form m_form = null;
        private const int m_CWBLIFECOUNT = 8;

        private static Queue<csExWB.cEXWB> m_AvailableCWB= new Queue<csExWB.cEXWB>();
        private static List<csExWB.cEXWB> m_UsedCWB = new List<csExWB.cEXWB>();
        private static Dictionary<csExWB.cEXWB, int> m_CWBCOUNTDICT = new Dictionary<csExWB.cEXWB, int>();

        private delegate void PrepareCWBDelegator();
        private delegate void DisposeCWBDelegator(csExWB.cEXWB c);

        private static bool m_Enable = true;

        public static void Init(System.Windows.Forms.Form form)
        {
            if (form!=null)
            {
                m_form = form;
                PrepareCWB();
            }

            ms_NCThread = new Thread(NCThreadRunner);
            ms_NCThread.Name = "NavigationCenter";
            ms_NCThread.Start();
        }

        public static bool Enable
        {
            get { return m_Enable; }
        }

        private static void PrepareCWB()
        {
            if (m_form.InvokeRequired)
            {
                m_form.Invoke(new PrepareCWBDelegator(PrepareCWB));
            }
            else
            {
                for (int i = 1; i <= m_PreparedCWBNum; i++)
                {
                    csExWB.cEXWB c = new csExWB.cEXWB();
                    c.Size = new System.Drawing.Size(50, 50);
                    c.NavToBlank();
                    m_CWBCOUNTDICT.Add(c, 0);
                    m_AvailableCWB.Enqueue(c);
                }
            }
        }

        private static void DisposeCWB(csExWB.cEXWB c)
        {
            if (m_form.InvokeRequired)
            {
                log.Debug("Dispose CWB...");
                m_form.BeginInvoke(new DisposeCWBDelegator(DisposeCWB), new object[] {c});
            }
            else
            {
                try
                {
                    c.Dispose();
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }
        }

        private static bool CWBLifeCheck(csExWB.cEXWB c)
        {
            if (m_CWBCOUNTDICT.ContainsKey(c))
            {
                if (m_CWBCOUNTDICT[c] >= m_CWBLIFECOUNT)
                {
                    m_CWBCOUNTDICT.Remove(c);
                    return false;
                }
                else
                {
                    m_CWBCOUNTDICT[c] = m_CWBCOUNTDICT[c] + 1;
                    return true;
                }
            }
            else
            {
                log.Error("某浏览器对象的计数器元件未初始化!");
                return false;
            }

        }

        public static csExWB.cEXWB RentCWB()
        {
            lock (locker)
            {
                if (m_Enable)
                {
                    if (m_AvailableCWB.Count <= 0)
                        PrepareCWB();
                    csExWB.cEXWB c = m_AvailableCWB.Dequeue();
                    m_UsedCWB.Add(c);
                    return c;
                }
                return null;
            }
        }

        public static void ReturnCWB(csExWB.cEXWB c)
        {
            lock (locker)
            {
                m_UsedCWB.Remove(c);
                if (CWBLifeCheck(c))
                    m_AvailableCWB.Enqueue(c);
                else
                    DisposeCWB(c);
            }
        }
        
        #region Static 静态成员 - NavigationCenter

        private static Thread ms_NCThread;
        private static object nclockerbegin = new object();
        private static object nclockerend = new object();
        private static object nclockeradsl = new object();
        private static int ms_navigationcount = 0;
        //private static bool flag_isadslchanging = false;
        private const int CHANGEIPFREEZINGTIME = 8000; //冰封期时长 = 8sec
        private const int CONNECTIONOUTOFTIME = 60000; //连接超时时长 = 60sec

        private static void NCThreadRunner()
        {
            Monitor.Enter(nclockeradsl);
            Monitor.Enter(nclockerbegin);
            Monitor.Enter(nclockerend);

            while (true)
            {
                //IP切换冰封期
                Monitor.Exit(nclockerbegin);
                Monitor.Exit(nclockerend);
                Monitor.Exit(nclockeradsl); //释放ADSL线程
                Monitor.Enter(nclockeradsl);
                Thread.Sleep(CHANGEIPFREEZINGTIME);

                //自由导航模式
                Monitor.Wait(nclockeradsl);

                //IP切换准备模式
                Monitor.Enter(nclockerbegin);
                Monitor.Enter(nclockerend);
                while (ms_navigationcount > 0)
                {
                    if (!Monitor.Wait(nclockerend, CONNECTIONOUTOFTIME))
                        break;
                }

                //IP切换模式
                ADSLFactory.Reconnect();
            }

            Monitor.Exit(nclockerbegin);
            Monitor.Exit(nclockerend);
            Monitor.Exit(nclockeradsl);
        }

        public static void NavigationBegin()
        {
            lock (nclockerbegin)
            {
                ms_navigationcount++;
            }
        }

        public static void NavigationEnd()
        {
            lock (nclockerend)
            {
                if (ms_navigationcount > 0)
                {
                    ms_navigationcount--;
                    Monitor.Pulse(nclockerend);
                }
            }
        }

        public static void ApplyChangeIP()
        {
            //通知ADSL切换
            Monitor.Enter(nclockeradsl);
            Monitor.Pulse(nclockeradsl);                
            Monitor.Exit(nclockeradsl);
        } 

        #endregion

        #region Static 静态成员 - 单站连接限制器

        private static Dictionary<string, DateTime> ms_datetimemap = new Dictionary<string, DateTime>();
        private static Dictionary<string, object> ms_lockermap = new Dictionary<string, object>();
        private const double mc_MinimalHostInterval = 100;

        private static object GetLockerByHost(string host)
        {
            lock (ms_lockermap)
            {
                object locker = null;
                if (!ms_lockermap.ContainsKey(host))
                {
                    locker = new object();
                    ms_lockermap.Add(host, locker);
                    ms_datetimemap.Add(host, DateTime.Now);
                    return locker;
                }
                else
                    return ms_lockermap[host];
            }
        }
        public static void WebBrowserNavigationDelayer(string url, double theoreticaldelay)
        {
            DateTime newtimepoint;
            string host;
            TimeSpan practicaldelay = TimeSpan.Zero;

            if (theoreticaldelay <= 0) theoreticaldelay = mc_MinimalHostInterval;
            try
            {
                System.Uri u = new Uri(url);
                host = u.Host;
            }
            catch
            {
                host = "default";
            }

            lock (GetLockerByHost(host))
            {
                newtimepoint = ms_datetimemap[host].AddMilliseconds(theoreticaldelay);
                if (newtimepoint > DateTime.Now)
                {
                    practicaldelay = newtimepoint - DateTime.Now;
                    if (practicaldelay.TotalMilliseconds <= theoreticaldelay)
                    {
                        //延迟
                        log.Info("Navigation Delay..." + practicaldelay.TotalMilliseconds.ToString());
                        Thread.Sleep(practicaldelay);
                    }
                }
                //更新时间戳
                ms_datetimemap[host] = DateTime.Now;
            }
        }

        #endregion       

        public static void Dispose()
        {
            lock (locker)
            {
                m_Enable = false;
                foreach (csExWB.cEXWB c in m_UsedCWB)
                    c.Dispose();

                foreach (csExWB.cEXWB c in m_AvailableCWB)
                    c.Dispose();
            }

            //消灭线程
            if (ms_NCThread != null)
            {
                ms_NCThread.Abort();
                ms_NCThread = null;
            }
        }

    }
}
