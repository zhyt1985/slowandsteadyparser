using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Threading;

namespace SlowAndSteadyParser
{
    public class SafeOledb
    {
        //log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<string, OleDbConnection> ms_safedbconn = new Dictionary<string, OleDbConnection>();
        private static object locker = new object();
        private const int DBCONNOUTOFTIME = 60000;

        public static OleDbConnection OpenSafeOleDbConnection(string dbstring)
        {
            lock (ms_safedbconn)
            {
                if (!ms_safedbconn.ContainsKey(dbstring))
                {
                    ms_safedbconn.Add(dbstring, new OleDbConnection(dbstring));
                }
            }

                OleDbConnection dbconn = ms_safedbconn[dbstring];
                if (Monitor.TryEnter(dbconn, DBCONNOUTOFTIME))
                {
                    if (dbconn.State == System.Data.ConnectionState.Open)
                    {
                        log.Error("Safeoledb: 发现未关闭连接, 延时一秒后尝试关闭");
                        Thread.Sleep(1000);
                        dbconn.Close();
                        Thread.Sleep(100);
                    }

                    dbconn.Open();
                    return dbconn;
                }
                else
                {
                    log.Error("Safeoledb: 安全连接超时 " + dbstring);
                    throw new Exception("Safeoledb: 安全连接超时 " + dbstring);
                }            
        }

        public static void CloseSafeOleDbConnection(string dbstring)
        {            
            if (ms_safedbconn.ContainsKey(dbstring))
            {
                OleDbConnection dbconn = ms_safedbconn[dbstring];
                dbconn.Close();
                Monitor.Exit(dbconn);
            }            
        }

        public static void CloseSafeOleDbConnection(OleDbConnection oledbconn)
        {
            if (oledbconn != null)
            {
                oledbconn.Close();
            }
            if (ms_safedbconn.ContainsValue(oledbconn))
            {
                Monitor.Exit(oledbconn);
            }
        }

        public static void CloseAllSafeOleDbConnection()
        {
            foreach (OleDbConnection oledbconn in ms_safedbconn.Values)
                if (oledbconn.State == System.Data.ConnectionState.Open)
                {                    
                    oledbconn.Close();
                }
        }
    }
}
