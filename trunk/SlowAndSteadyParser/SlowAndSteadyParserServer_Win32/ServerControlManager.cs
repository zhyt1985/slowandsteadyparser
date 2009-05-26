using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Serialization.Formatters.Soap;

namespace SlowAndSteadyParser
{
    public static class ServerControlManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static bool ms_IsServerRunning = false;
        private static TaskManagerServer m_tms = null;
        private static TaskTransmissionManager m_ttm = null;
        private static int ms_socketport = 1371; //默认TCP端口
        private static int ms_LogLoadingLimit = 800;
        private static int ms_LogReservedDay = 7;
        private static bool ms_IsMinToSystemTray = true;
        private static bool ms_IsServerModeRemote = true;

        public static void Start()
        {
            if (!ms_IsServerRunning)          
            {
                //Enable TaskTransmissionManager
                m_ttm.Start();

                //Start TaskManager
                m_tms.Start();

                //check available resource
                m_ttm.CheckAvailableClientTaskResource();

                ms_IsServerRunning = true;
            }

            
        }

        public static void Stop()
        {
            if (ms_IsServerRunning)
            {
                //Stop TaskTransmissionManager
                m_ttm.Stop();

                //Stop TaskManager
                m_tms.Stop();

                ms_IsServerRunning = false;
            }

            //clear log!
            LogDatabaseManager.ClearLogByDate(DateTime.Now.AddDays(-1 * ms_LogReservedDay));
        }

        private static void StoreManagerIntoFile(FrmMain uiform)
        {
            LocalStorage store = new LocalStorage();
            store.LogLevelFlags = uiform.ButtonStatusToLevelFlags();
            store.DomainList = DomainManager.DomainList;
            store.CurrentTaskManager = m_tms;
            store.CurrentTransmissionManager = m_ttm;
            store.ServerTcpPort = ServerPort;
            store.LogReservedDays = LogReservedDays;
            store.LogLoadingLimit = LogLoadingLimit;
            store.IsMinToSystemTray = IsMinToSystemTray;
            store.IsServerModeLocal = !IsServerModeRemote;
            store.MessagePeriod = MessagePeriod;
            store.IsUsingFetion = IsUsingFetion;
            store.FetionNumber = FetionNumber;
            store.FetionPsd = FetionPsd;
            store.FetionSendingTime = FetionSendingTime;
            store.ADSLEntryName = ADSLEntry;
            store.ADSLUserName = ADSLUserName;
            store.ADSLPassword = ADSLPassword;

            IFormatter formatter = new BinaryFormatter();
            try
            {
                Stream stream = new FileStream("SlowAndSteadyParser.store", FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, store);
                stream.Close();
                
            }
            catch (Exception e)
            {
                MessageBox.Show("写入本地数据文件失败,当前任务信息无法保存", "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.Error("写入本地数据文件失败", e);
            }
            finally
            {
                
            }
        }

        private static void LoadManagerFromFile(FrmMain uiform)
        {
            
            IFormatter formatter = new BinaryFormatter();
            try
            {
                Stream stream = new FileStream("SlowAndSteadyParser.store", FileMode.Open, FileAccess.Read, FileShare.Read);
                try
                {
                    LocalStorage store = (LocalStorage)formatter.Deserialize(stream);
                    uiform.LevelFlagsToButtonStatus(store.LogLevelFlags);
                    m_tms = store.CurrentTaskManager;
                    m_ttm = store.CurrentTransmissionManager;
                    DomainManager.DomainList = store.DomainList;
                    IsMinToSystemTray = store.IsMinToSystemTray;
                    IsServerModeRemote = !store.IsServerModeLocal;
                    if (store.ServerTcpPort != 0) ServerPort = store.ServerTcpPort;
                    if (store.LogLoadingLimit != 0) LogLoadingLimit = store.LogLoadingLimit;
                    if (store.LogReservedDays != 0) LogReservedDays = store.LogReservedDays;
                    if (store.MessagePeriod != 0) MessagePeriod = store.MessagePeriod;
                    IsUsingFetion = store.IsUsingFetion;
                    if (store.FetionNumber != null) FetionNumber = store.FetionNumber;
                    if (store.FetionPsd != null) FetionPsd = store.FetionPsd;
                    if (store.FetionSendingTime != null) FetionSendingTime = store.FetionSendingTime;
                    if (store.ADSLEntryName != null) ADSLEntry = store.ADSLEntryName;
                    if (store.ADSLUserName != null) ADSLUserName = store.ADSLUserName;
                    if (store.ADSLPassword != null) ADSLPassword = store.ADSLPassword;
                }
                catch (SerializationException e)
                {
                    m_tms = new TaskManagerServer();
                    m_ttm = new TaskTransmissionManager();
                    log.Warn("本地数据文件格式错误", e);
                }
                catch (Exception e)
                {
                    m_tms = new TaskManagerServer();
                    m_ttm = new TaskTransmissionManager();
                    log.Warn("本地数据文件格式错误", e);
                }
                finally
                {
                    stream.Close();
                }

            }
            catch (System.IO.FileNotFoundException e)
            {
                m_tms = new TaskManagerServer();
                m_ttm = new TaskTransmissionManager();
                log.Warn("未找到本地数据文件", e);
            }
            finally
            {
                if (m_ttm == null) m_ttm = new TaskTransmissionManager();
                if (m_tms == null) m_tms = new TaskManagerServer();
            }
        }

        public static void Init(FrmMain uiform)
        {
            //Init Thread Name
            Thread.CurrentThread.Name = "UI";

            //Init WebBrowser Pool
            CWBPool.Init(uiform);

            //Init PerformanceBalancer
            PerformanceBalancer.Init(false);
            
            //Init FetionTimer
            uiform.timerMain.Enabled = true;

            //Init TaskManager & TaskTransmissionManager
            ms_FetionSendingTime = new Dictionary<int, bool>();
            for (int i = 0; i < 24; i++)
            {
                ms_FetionSendingTime.Add(i, false);
            }
            LoadManagerFromFile(uiform);
            m_tms.TaskTransmissionManager = m_ttm;
            m_ttm.TaskManagerServer = m_tms;
            
            //open tcp server
            try
            {
                ConnectionManagerServer.RegisterTcpChannel(ms_socketport);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message+Environment.NewLine+"请重新配置端口号", "注册端口错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.Error("注册端口错误: ", e);
            }

            //Test Log DB
            string errmessage = null;
            if (LogDatabaseManager.TestConnection(ref errmessage) == false)
                MessageBox.Show(errmessage, "日志数据库", MessageBoxButtons.OK, MessageBoxIcon.Error);

            //Start Tcp Service
            string errormessage = null;
            if (!ConnectionManagerServer.StartTcpServer(m_ttm, ref errormessage))
            {
                MessageBox.Show(errormessage, "启动服务错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }           

        }

        public static void Dispose(FrmMain uiform)
        {
            if (ms_IsServerRunning)
                Stop();

            Thread.Sleep(500);

            //Stop Tcp Service
            ConnectionManagerServer.StopServer();

            ConnectionManagerServer.UnregisterChannel();

            StoreManagerIntoFile(uiform);

            m_tms.Dispose();
            m_ttm.Dispose();

            //Static Manager Disposing...
            VBAStaticEngine.DisposeAll();
            CWBPool.Dispose();
            PerformanceBalancer.Dispose();
        }

        public static Domain OpenDomainFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "解决方案文件|*.domain|所有文件|*.*";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        IFormatter formatter = new SoapFormatter();
                        using (Stream stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            try
                            {
                                Domain d = (Domain)formatter.Deserialize(stream);
                                if (DomainManager.IsDomainExist(d.DomainGUID))
                                {
                                    d.NewDomainGUID();                                                                        
                                }
                                DomainManager.AddDomain(d);
                                return d;
                            }
                            catch (SerializationException e)
                            {
                                log.Error("反序列化失败: " + openFileDialog.FileName + "不是解决方案文件!", e);
                                MessageBox.Show(openFileDialog.FileName + "不是解决方案文件!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return null;
                            }
                            catch (Exception e)
                            {
                                log.Error("未知错误: " + openFileDialog.FileName, e);
                                MessageBox.Show(openFileDialog.FileName + "出现未知错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return null;
                            }
                        }
                    }
                    catch(System.IO.FileNotFoundException e)
                    {
                        log.Error("打开文件失败: " + openFileDialog.FileName, e);
                        MessageBox.Show("打开文件失败: " + openFileDialog.FileName, "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return null;
                    }
                }
                return null;
            }            
        }

        public static void SaveDomainFile(Domain d)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "解决方案文件|*.domain|所有文件|*.*";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FilterIndex = 1;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        IFormatter formatter = new SoapFormatter();
                        using (Stream stream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                        {                           
                            formatter.Serialize(stream, d);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("Domain文件保存失败: " + saveFileDialog.FileName, e);
                        MessageBox.Show("保存解决方案文件失败: " + saveFileDialog.FileName, "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }            
        }

        public static int LogLoadingLimit
        {
            get { return ms_LogLoadingLimit; }
            set { ms_LogLoadingLimit = value; }
        }

        public static int LogReservedDays
        {
            get { return ms_LogReservedDay; }
            set { ms_LogReservedDay = value; }
        }

        public static void ClearAllWaitingTask()
        {
            m_tms.RemoveAllWaitingTask();
            m_ttm.RemoveAllWaitingTask();
        }

        public static void CheckForReservedPool()
        {
            m_ttm.CheckAvailableClientTaskResource();
        }

        #region Server Basic

        public static bool IsServerRunning
        {
            get { return ms_IsServerRunning; }
        }

        public static bool IsServerModeRemote
        {
            get { return ms_IsServerModeRemote; }
            set
            {
                ms_IsServerModeRemote = value;
                if (ms_IsServerModeRemote)
                    DomainManager.SetServerModeRemote();
                else
                    DomainManager.SetServerModeLocal();
            }
        }

        public static int ServerWaitingTask
        {
            get { return m_tms.CountOfWaitingTask; }
        }

        public static int ServerRunningTask
        {
            get { return m_tms.CountOfRunningTask; }
        }

        public static int ReservedPoolTask
        {
            get { return m_ttm.CountOfWaitingClientTask; }
        }

        public static bool IsMinToSystemTray
        {
            get { return ms_IsMinToSystemTray; }
            set { ms_IsMinToSystemTray = value; }
        }

        public static int MaxRetryTime
        {
            get { return m_tms.MaxRetryTime; }
            set { m_tms.MaxRetryTime = value; }
        }

        public static int MaxRunningTask
        {
            get { return m_tms.MaxRunningThread; }
            set { m_tms.MaxRunningThread = value; }
        }

        public static int MinWaitingClientTask
        {
            get { return m_ttm.MinimalAcceptableWatingClientTask; }
            set { m_ttm.MinimalAcceptableWatingClientTask = value; }
        }

        public static int ServerPort
        {
            get { return ms_socketport; }
            set { ms_socketport = value; }
        }

        #endregion

        #region Fetion & Message

        private static bool ms_IsUsingFetion = false;
        private static int ms_MessagePeriod = 10;
        private static string ms_FetionNumber = "";
        private static string ms_FetionPsd = "";
        private static Dictionary<int, bool> ms_FetionSendingTime;

        public static bool IsUsingFetion
        {
            get { return ms_IsUsingFetion; }
            set { ms_IsUsingFetion = value; }
        }
        public static int MessagePeriod
        {
            get { return ms_MessagePeriod; }
            set { ms_MessagePeriod = value; }
        }
        public static string FetionNumber
        {
            get { return ms_FetionNumber; }
            set { ms_FetionNumber = value; }
        }
        public static string FetionPsd
        {
            get { return ms_FetionPsd; }
            set { ms_FetionPsd = value; }
        }        
        public static Dictionary<int, bool> FetionSendingTime
        {
            get { return ms_FetionSendingTime; }
            set { ms_FetionSendingTime = value; }
        }

        #endregion

        #region ADSL

        private static string ms_ADSLEntry = "ADSL";
        private static string ms_ADSLUserName = "";
        private static string ms_ADSLPassword = "";

        public static string ADSLEntry
        {
            get { return ms_ADSLEntry; }
            set { ms_ADSLEntry = value; }
        }
        public static string ADSLUserName
        {
            get { return ms_ADSLUserName; }
            set { ms_ADSLUserName = value; }
        }
        public static string ADSLPassword
        {
            get { return ms_ADSLPassword; }
            set
            {
                ms_ADSLPassword = value;
                ADSLFactory.Init(ADSLEntry, ADSLUserName, ADSLPassword);
            }
        }

        #endregion

    }

    [Serializable]
    public struct LocalStorage
    {
        public bool IsServerModeLocal;                     //服务器模式是本地模式么
        public int ServerTcpPort;                          //服务器端口号
        public int LogReservedDays;
        public int LogLoadingLimit;
        public bool IsMinToSystemTray;
        public int MessagePeriod;
        public bool IsUsingFetion;
        public string FetionNumber;
        public string FetionPsd;
        public string ADSLEntryName;
        public string ADSLUserName;
        public string ADSLPassword;
        public Dictionary<int,bool> FetionSendingTime;
        public VBALogLevelFlag LogLevelFlags;              //当前LevelFlags
        public Dictionary<string, Domain> DomainList;      //所有Domain信息
        public TaskManagerServer CurrentTaskManager;       //TaskManager
        public TaskTransmissionManager CurrentTransmissionManager;
    }
}
