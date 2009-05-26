using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DotRas;
using System.Net;
using System.Collections.ObjectModel;

namespace SlowAndSteadyParser
{
    public class ADSLFactory
    {
        //log
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //locker
        private static object locker = new object();
        private static object connectionlocker = new object();

        //ip change
        //private const int LEASTCHANGEIPINTERVAL = 3;
        private const int CONNECTIONOUTMILLIONSECOND = 25000;
        //private static string m_lastip;

        //ras
        private static RasDialer ms_rasdialer;         
        //private static RASDisplay ms_rasdisplay;
        //private static RasManager ms_rasmanager;
        private static NetworkCredential ms_nc = null;
        //private static RasHandle ms_rashandle = null;
        private static string ms_entryname = null;
        private static bool ms_IsInitialed = false;
        private static int ms_visitor = 0;

        public static bool IsInitialed
        {
            get { return ms_IsInitialed; }
        }

        public static void Init(string entryname, string username, string password)
        {
            bool IsFoundEntry = false;

            //判断是否从电话簿中调出已有拨号连接
            if (username == null || password == null || username == "" || password == "")
            {
                using (RasPhoneBook phoneBook = new RasPhoneBook())
                {
                    phoneBook.Open();
                    foreach (RasEntry entry in phoneBook.Entries)
                    {
                        if (entry.Name == entryname)
                        {
                            IsFoundEntry = true;
                        }
                    }
                    //没有找到同名的就用第一个
                    if (!IsFoundEntry)
                    {
                        entryname = phoneBook.Entries[0].Name;
                    }
                }
            }
            else
            {
                //本地生成
                ms_nc = new NetworkCredential(username, password);
            }
            ms_entryname = entryname;
            ms_IsInitialed = true;
            log.Debug("ADSLManager: Initial " + entryname);
        }

        public static bool Connect()
        {
            Thread t;
            if (ms_IsInitialed)
            {
                lock (connectionlocker)
                {
                    t = new Thread(ConnectThread);
                    t.Start();                    
                    if (Monitor.Wait(connectionlocker, CONNECTIONOUTMILLIONSECOND))
                    {
                        return true;
                    }
                    else
                    {
                        t.Abort();
                        t.Join(CONNECTIONOUTMILLIONSECOND/2);
                        log.Debug("ADSLManager: Connecting Out Of Time");
                        return false;                        
                    }

                }
            }
            log.Debug("ADSLManager: Not Been Initialed!");
            return false;
        }

        public static void ConnectThread()
        {
            try
            {
                ms_rasdialer = new RasDialer();
                ms_rasdialer.EntryName = ms_entryname;
                //判断是否带用户名连接
                if (ms_nc == null)
                {
                    ms_rasdialer.Dial();
                }
                else
                {
                    ms_rasdialer.Dial(ms_nc);
                }

            }
            catch (Exception rde)
            {
                log.Error("ADSLManger: ConnectionThread", rde);
            }
            finally
            {
                lock (connectionlocker)
                {
                    Monitor.Pulse(connectionlocker);
                }
            }
        }

        public static void Disconnect()
        {
            try
            {
                if (ms_rasdialer != null)
                {
                    ms_rasdialer.Dispose();                    
                }
                ms_rasdialer = new RasDialer();
                ReadOnlyCollection<RasConnection> connections = ms_rasdialer.GetActiveConnections();
                if (connections != null && connections.Count > 0)
                {
                    foreach (RasConnection connection in connections)
                    {
                        if (ms_entryname == connection.EntryName)
                        {
                            connection.HangUp();
                        }
                    }
                }
                ms_rasdialer.Dispose();
            }
            catch (RasException re)
            {
                log.Error("ADSLManger: Disconnect", re);
            }
        }

        public static bool Reconnect()
        {
            if (!ms_IsInitialed)
            {
                log.Debug("ADSLManager: Not Been Initialed!");
                return false;
            }

            if (Monitor.TryEnter(locker))
            {
                log.Debug("ADSLManager: Reconnection...Start");
                Disconnect();
                System.Threading.Thread.Sleep(500);
                Connect();
                System.Threading.Thread.Sleep(1000);
                Monitor.Exit(locker);
                log.Debug("ADSLManager: Reconnection...Succeed");
                return true;
            }
            else
            {
                log.Debug("ADSLManager: Has Been Reconnecting, Waiting..");
                Monitor.Enter(locker); 
                Monitor.Exit(locker);
                return true;
            }
        }
    }
}
