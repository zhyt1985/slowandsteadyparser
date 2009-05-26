using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Threading;

namespace SlowAndSteadyParser
{
    public static class ClientControlManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Balancer balancer = PerformanceBalancer.GetBalancer(System.Reflection.Assembly.GetEntryAssembly().GetName().Name);

        private static TaskManagerClient ms_tmc = null;
        //private static ITaskTransmissionManager ms_ttm = null;      
        private static bool ms_IsClientRunning = false;
        private static Configuration ms_c;
        private static Form ms_ui;

        public static bool IsClientRunning
        {
            get { return ms_IsClientRunning; }
        }

        public static ClientMode Mode
        {
            get { return ms_c.CurrentMode; }
            set 
            { 
                ms_c.CurrentMode = value;
                SetConfigurationToManager();
                SetConfigurationToPerformanceBalancer();
            }
        }        

        public static Configuration Configuration
        {
            get
            {
                return ms_c;
            }

            set
            {
                ms_c = value;
                SetConfigurationToServerInfo();
                SetConfigurationToManager();
                SetConfigurationToADSL();
                SetConfigurationToPerformanceBalancer();
                ConnectionManagerClient.FetchServerStatus();
            }
        }

        public static void SetConfigurationToManager()
        {
            ms_tmc.MaxRetryTime = ms_c.Configurations[ms_c.CurrentMode].MaxRetryTime;
            ms_tmc.MaxRunningThread = ms_c.Configurations[ms_c.CurrentMode].MaxRunningTask;
            ms_tmc.QueryServerInterval = ms_c.Configurations[ms_c.CurrentMode].QueryInterval*1000;
            ms_tmc.SingleTranspotation = ms_c.Configurations[ms_c.CurrentMode].SingleTransportation;
        }

        public static void SetConfigurationToADSL()
        {
            ADSLFactory.Init(ms_c.ADSLEntryName, ms_c.ADSLUserName, ms_c.ADSLPassword);
        }

        public static void SetConfigurationToPerformanceBalancer()
        {
            balancer.AnticapateCPUUsage = ms_c.Configurations[ms_c.CurrentMode].MaxCPUUsage;
        }

        public static void SetConfigurationToServerInfo()
        {
            ConnectionManagerClient.Address = ms_c.ServerAddress;
            ConnectionManagerClient.Port = ms_c.ServerPort;
        }

        public static void Start()
        {
            if (!ms_IsClientRunning)
            {
                ms_tmc.Start();
                
                ms_IsClientRunning = true;
            }
        }

        public static void Stop()
        {
            if (ms_IsClientRunning)
            {
                ms_tmc.Stop();

                ms_IsClientRunning = false;
            }
        }

        private static void StoreManagerIntoFile(FrmMain uiform)
        {
            LocalStorage store = new LocalStorage();           
            store.DomainList = DomainManager.DomainList;
            store.CurrentTaskManager = ms_tmc;
            store.Configuration = ms_c;
            IFormatter formatter = new BinaryFormatter();
            try
            {
                Stream stream = new FileStream("SlowAndSteadyParser.store", FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, store);
                stream.Close();
                
            }
            catch (Exception e)
            {
                MessageBox.Show("写入本地数据文件失败,客户端信息无法保存", "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    ms_tmc = store.CurrentTaskManager;
                    DomainManager.DomainList = store.DomainList;
                    ms_c = store.Configuration;                    
                }
                catch (SerializationException e)
                {
                    log.Warn("本地数据文件格式错误", e);
                }
                finally
                {                    
                    stream.Close();
                }

            }
            catch (System.IO.FileNotFoundException e)
            {
                log.Warn("未找到本地数据文件", e);
            }
            finally
            {            
                if (ms_tmc == null) ms_tmc = new TaskManagerClient();
                RecheckConfiguration();
                SetConfigurationToServerInfo();
                SetConfigurationToManager();
                SetConfigurationToPerformanceBalancer();
                SetConfigurationToADSL();
            }
        }

        private static void RecheckConfiguration()
        {
            if (ms_c.ServerPort < 1 || ms_c.ServerPort > 32768) ms_c.ServerPort = 1371;

            if (ms_c.Configurations == null)
            {
                ms_c.Configurations = new Dictionary<ClientMode, ConfigurationDetailInMode>();
            }

            if (ms_c.CurrentMode == 0)
                ms_c.CurrentMode = ClientMode.StandardspeedMode;

            //Low
            ConfigurationDetailInMode Low;
            if (ms_c.Configurations.ContainsKey(ClientMode.LowspeedMode))
                Low = ms_c.Configurations[ClientMode.LowspeedMode];
            else
                Low = new ConfigurationDetailInMode();
            if (Low.SingleTransportation < 1 || Low.SingleTransportation > 20) Low.SingleTransportation = 3;
            if (Low.QueryInterval < 10 || Low.QueryInterval > 3600) Low.QueryInterval = 30;
            if (Low.MaxRunningTask < 1 || Low.MaxRunningTask > 20) Low.MaxRunningTask = 2;
            if (Low.MaxRetryTime < 0 || Low.MaxRetryTime > 5) Low.MaxRetryTime = 0;
            if (Low.MaxCPUUsage < 1 || Low.MaxCPUUsage > 100) Low.MaxCPUUsage = 50;
            ms_c.Configurations[ClientMode.LowspeedMode] = Low;

            //Standard
            ConfigurationDetailInMode Standard;
            if (ms_c.Configurations.ContainsKey(ClientMode.StandardspeedMode))
                Standard = ms_c.Configurations[ClientMode.StandardspeedMode];
            else
                Standard = new ConfigurationDetailInMode();
            if (Standard.SingleTransportation < 1 || Standard.SingleTransportation > 20) Standard.SingleTransportation = 5;
            if (Standard.QueryInterval < 10 || Standard.QueryInterval > 3600) Standard.QueryInterval = 30;
            if (Standard.MaxRunningTask < 1 || Standard.MaxRunningTask > 20) Standard.MaxRunningTask = 4;
            if (Standard.MaxRetryTime < 0 || Standard.MaxRetryTime > 5) Standard.MaxRetryTime = 1;
            if (Standard.MaxCPUUsage < 1 || Standard.MaxCPUUsage > 100) Standard.MaxCPUUsage = 75;
            ms_c.Configurations[ClientMode.StandardspeedMode] = Standard;

            //High
            ConfigurationDetailInMode High;
            if (ms_c.Configurations.ContainsKey(ClientMode.HighspeedMode))
                High = ms_c.Configurations[ClientMode.HighspeedMode];
            else
                High = new ConfigurationDetailInMode();
            if (High.SingleTransportation < 1 || High.SingleTransportation > 20) High.SingleTransportation = 7;
            if (High.QueryInterval < 10 || High.QueryInterval > 3600) High.QueryInterval = 30;
            if (High.MaxRunningTask < 1 || High.MaxRunningTask > 20) High.MaxRunningTask = 5;
            if (High.MaxRetryTime < 0 || High.MaxRetryTime > 5) High.MaxRetryTime = 1;
            if (High.MaxCPUUsage < 1 || High.MaxCPUUsage > 100) High.MaxCPUUsage = 100;
            ms_c.Configurations[ClientMode.HighspeedMode] = High;

        }

        public static void RegisterTcpChannelForClient()
        {
            if (ConnectionManagerClient.IsTcpChannelRegistered)
                ConnectionManagerClient.UnregisterChannel();

            //Init Tcp Channel
            log.Info("Init Tcp Channel...");
            ConnectionManagerClient.RegisterTcpChannel();
        }

        public static void Init(FrmMain uiform)
        {
            Thread.CurrentThread.Name = "UI";
            ms_ui = uiform;
            
            //Init PerformanceBalancer
            PerformanceBalancer.Init(true);

            //Init Web Browser Warehouse
            log.Info("Init Web Browser Pool...");
            CWBPool.Init(ms_ui);

            //Init Channel
            RegisterTcpChannelForClient();

            //Init TaskManager & TaskTransmissionManager
            LoadManagerFromFile(uiform);

        }

        public static void Dispose(FrmMain uiform)
        {
            if (ms_IsClientRunning)
                Stop();

            Thread.Sleep(500);

            ConnectionManagerClient.UnregisterChannel();         

            StoreManagerIntoFile(uiform);

            ms_tmc.Dispose();

            //Static Manager Disposing...
            VBAStaticEngine.DisposeAll();
            CWBPool.Dispose();
            PerformanceBalancer.Dispose();
        }
    }

    [Serializable]
    public struct LocalStorage
    {
        public Configuration Configuration;                //配置数据
        public Dictionary<string, Domain> DomainList;      //所有本地Domain信息
        public TaskManagerClient CurrentTaskManager;       //TaskManagerClinet
        
    }
}
