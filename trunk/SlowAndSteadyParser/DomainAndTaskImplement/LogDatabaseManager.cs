using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace SlowAndSteadyParser
{


    public static class LogDatabaseManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string ms_connstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+Path.Combine(Environment.CurrentDirectory, "Log.MDB");

        public static bool TestConnection(ref string errmessage)
        {
            try
            {
                using (OleDbConnection logdbcon = new OleDbConnection(ms_connstring))
                {
                    string cmdstr = null;
                    try
                    {
                        logdbcon.Open();
                        cmdstr = "SELECT TOP 1 *  FROM [Log]";
                        OleDbCommand cmd = new OleDbCommand(cmdstr, logdbcon);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (OleDbException e)
                    {
                        errmessage = "基本日志表不存在, 将重新创建!";
                        log.Error("数据表Log打开错误:" + cmdstr, e);
                        //表
                        cmdstr = @"CREATE TABLE [Log] (
   [DomainGUID] nvarchar(50) null ,
   [TaskChainGUID] nvarchar(50) null ,
   [LogTime] datetime null ,
   [LogLevel] nvarchar(10) null ,
   [LogMessage] ntext null 
  )";
                        OleDbCommand cmdnew = new OleDbCommand(cmdstr, logdbcon);
                        cmdnew.ExecuteNonQuery();
                        //索引
                        cmdstr = "CREATE INDEX [Time] on [Log]([LogTime])";
                        cmdnew = new OleDbCommand(cmdstr, logdbcon);
                        cmdnew.ExecuteNonQuery();
                        return false;
                    }
                    catch (Exception e)
                    {
                        errmessage = e.ToString();
                        log.Error(e);
                        return false;
                    }
                }
            }
            catch(Exception e)
            {
                errmessage = "数据库连接字符串错误:"+Environment.NewLine +e.ToString();
                log.Error(e);
                return false;
            }
        }

        public static void WriteVBALogIntoDB(string DomainGUID, string TaskChainGUID, string TaskName, VBALog vlog)
        {
            using (OleDbConnection logdbcon = new OleDbConnection(ms_connstring))
            {
                string cmdstr = null;
                try
                {
                    logdbcon.Open();
                    
                    foreach (VBALog.LogList l in vlog.Loglists)
                    {
                        cmdstr = "INSERT INTO Log(DomainGUID,TaskChainGUID,LogTime,LogLevel,LogMessage) VALUES('" + DomainGUID + "','" + TaskChainGUID + "','" +l.errtime + "','" + l.errlevel.ToString() + "','" + l.errmessage + "')";
                        OleDbCommand cmd = new OleDbCommand(cmdstr, logdbcon);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (OleDbException e)
                {
                    log.Error("写入Log数据库错误:" + cmdstr, e);
                }
                catch (Exception e)
                {
                    log.Error(e);
                }

            }
        }

        //public static void DataGridViewLoadLog(Domain d, System.Windows.Forms.DataGridView dgv, Dictionary<string, Image> levelpics)
        //{
        //    using (OleDbConnection logdbcon = new OleDbConnection(ms_connstring))
        //    {
        //        string cmdstr = null;
        //        OleDbDataReader dr = null;
        //        Image levelpic = null;
        //        string levelstring = null;
        //        try
        //        {
        //            logdbcon.Open();
        //            cmdstr = "SELECT TOP 1000 TaskChainGUID,LogTime,LogLevel,LogMessage FROM Log WHERE DomainGUID = '" + d.DomainGUID + "' ORDER BY LogTime DESC";
        //            OleDbCommand cmd = new OleDbCommand(cmdstr, logdbcon);
        //            dr = cmd.ExecuteReader();
                    
        //            while (dr.Read())
        //            {
        //                //得到level图片
        //                switch (((string)dr["LogLevel"]).ToLower())
        //                {
        //                    case "debug":
        //                        levelpic = levelpics["Debug"];
        //                        levelstring = "调试";
        //                        break;
        //                    case "info":
        //                        levelpic = levelpics["Info"];
        //                        levelstring = "信息";
        //                        break;
        //                    case "warn":
        //                        levelpic = levelpics["Warn"];
        //                        levelstring = "警告";
        //                        break;
        //                    case "error":
        //                        levelpic = levelpics["Error"];
        //                        levelstring = "错误";
        //                        break;
        //                    case "fatal":
        //                        levelpic = levelpics["Fatal"];
        //                        levelstring = "致命错误";
        //                        break;
        //                }

        //                DataGridViewRow r = new DataGridViewRow();                        
        //                dgv.Rows.Add(new object[] { levelpic, levelstring, dr["LogTime"], dr["TaskChainGUID"], dr["LogMessage"] });
        //            }
        //        }
        //        catch (OleDbException e)
        //        {
        //            log.Error("读取Log数据库错误:" + cmdstr, e);
        //        }
        //        catch (Exception e)
        //        {
        //            log.Error(e);
        //        }

        //    }
        //}

        private static void LoadLogBase(Domain d, string TaskGUID, System.Windows.Forms.DataGridView dgv, Dictionary<VBALogLevelFlag, Image> levelpics, VBALogLevelFlag dflags, int logloadlimit)
        {

            using (OleDbConnection logdbcon = new OleDbConnection(ms_connstring))
            {
                string cmdstr = null;
                OleDbDataReader dr = null;
                Image levelpic = null;
                string levelstring = null;
                string wherestring = null;
                List<string> levelflagstring = new List<string>();

                //一个都没的情况
                if (dflags == 0)
                    return;
                else
                {
                    //全有的情况
                    if (dflags == (VBALogLevelFlag.Debug | VBALogLevelFlag.Error | VBALogLevelFlag.Fatal |
                                  VBALogLevelFlag.Info | VBALogLevelFlag.Warn))
                        if (TaskGUID == null)
                        {
                            wherestring = "WHERE DomainGUID = '" + d.DomainGUID + "'";
                        }
                        else
                        {
                            wherestring = "WHERE DomainGUID = '" + d.DomainGUID + "' AND TaskChainGUID = '" + TaskGUID + "'";
                        }
                    else
                    {
                        //Debug
                        if ((VBALogLevelFlag.Debug & dflags) != 0)
                            levelflagstring.Add("LogLevel = 'Debug'");
                        //Info
                        if ((VBALogLevelFlag.Info & dflags) != 0)
                            levelflagstring.Add("LogLevel = 'Info'");
                        //Warn
                        if ((VBALogLevelFlag.Warn & dflags) != 0)
                            levelflagstring.Add("LogLevel = 'Warn'");
                        //Error
                        if ((VBALogLevelFlag.Error & dflags) != 0)
                            levelflagstring.Add("LogLevel = 'Error'");
                        //Fatal
                        if ((VBALogLevelFlag.Fatal & dflags) != 0)
                            levelflagstring.Add("LogLevel = 'Fatal'");

                        //构造查询WHERE字段
                        int i = 1;

                        if (TaskGUID == null)
                        {
                            wherestring = "WHERE DomainGUID = '" + d.DomainGUID + "' AND (";
                        }
                        else
                        {
                            wherestring = "WHERE DomainGUID = '"+ d.DomainGUID+ "' AND TaskChainGUID = '" + TaskGUID + "' AND (";
                        }

                        foreach (string s in levelflagstring)
                        {
                            wherestring = wherestring + s;
                            if (i != levelflagstring.Count)
                                wherestring = wherestring + " OR ";
                            i++;
                        }
                        wherestring = wherestring + ")";
                    }
                }

                try
                {
                    //关闭复杂排版
                    dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    dgv.Columns["DomainLogLevel"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgv.Columns["DomainLogDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

                    //读取Log数据
                    logdbcon.Open();
                    cmdstr = "SELECT TOP " + logloadlimit.ToString() + " TaskChainGUID,LogTime,LogLevel,LogMessage FROM Log " + wherestring + " ORDER BY logtime DESC;";
                    OleDbCommand cmd = new OleDbCommand(cmdstr, logdbcon);
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        //得到level图片
                        switch (((string)dr["LogLevel"]).ToLower())
                        {
                            case "debug":
                                levelpic = levelpics[VBALogLevelFlag.Debug];
                                levelstring = "调试";
                                break;
                            case "info":
                                levelpic = levelpics[VBALogLevelFlag.Info];
                                levelstring = "信息";
                                break;
                            case "warn":
                                levelpic = levelpics[VBALogLevelFlag.Warn];
                                levelstring = "警告";
                                break;
                            case "error":
                                levelpic = levelpics[VBALogLevelFlag.Error];
                                levelstring = "错误";
                                break;
                            case "fatal":
                                levelpic = levelpics[VBALogLevelFlag.Fatal];
                                levelstring = "致命错误";
                                break;
                        }
                        dgv.Rows.Add(new object[] { levelpic, levelstring, dr["LogTime"], dr["TaskChainGUID"], dr["LogMessage"] });
                        Application.DoEvents();

                    }

                    //排版
                    dgv.Columns["DomainLogLevel"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dgv.Columns["DomainLogDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                    dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
                }
                catch (OleDbException e)
                {
                    log.Error("读取Log数据库错误:" + cmdstr, e);
                }
                catch (Exception e)
                {
                    log.Error(e);
                }

            }
        }

        public static void DataGridViewLoadLog(Domain d, System.Windows.Forms.DataGridView dgv, Dictionary<VBALogLevelFlag, Image> levelpics, VBALogLevelFlag dflags, int logloadlimit)
        {
            LoadLogBase(d, null, dgv, levelpics, dflags, logloadlimit);
        }

        public static void DataGridViewLoadLog(Domain d, string TaskGUID, System.Windows.Forms.DataGridView dgv, Dictionary<VBALogLevelFlag, Image> levelpics, VBALogLevelFlag dflags, int logloadlimit)
        {
            LoadLogBase(d, TaskGUID, dgv, levelpics, dflags ,logloadlimit);
        }

        public static void ClearLogByDate(DateTime deadlineday)
        {
            using (OleDbConnection logdbcon = new OleDbConnection(ms_connstring))
            {
                string cmdstr = null;                
                try
                {
                    logdbcon.Open();
                    cmdstr = "DELETE FROM Log WHERE DATEDIFF('d',LogTime,'" + deadlineday.ToString() + "')>0";          
                    OleDbCommand cmd = new OleDbCommand(cmdstr, logdbcon);
                    int r = cmd.ExecuteNonQuery();
                }
                catch (OleDbException e)
                {
                    log.Error("写入Log数据库错误:" + cmdstr, e);
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }
        }

        public static void ClearLogAll()
        {
            using (OleDbConnection logdbcon = new OleDbConnection(ms_connstring))
            {
                string cmdstr = null;
                try
                {
                    logdbcon.Open();
                    cmdstr = "DELETE FROM [Log]";
                    OleDbCommand cmd = new OleDbCommand(cmdstr, logdbcon);
                    int r = cmd.ExecuteNonQuery();
                }
                catch (OleDbException e)
                {
                    log.Error("写入Log数据库错误:" + cmdstr, e);
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }
        }
    }
}
