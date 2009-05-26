using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public enum DomainPriority
    {
        Emergency,          //紧急
        Important,          //重要
        General,            //普通
        Subordinary,        //次要
        Insignificant,      //无关紧要的
        OneTime
    }
    
    [Serializable]
    public class Domain
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Private Attribute
        
        private bool m_IsEnable = false;

        private string m_GUID = null;
        private string m_name = null;
        private DateTime m_timestamp;
        private VBALogLevelFlag m_levelflag = VBALogLevelFlag.Debug;
        private DomainPriority m_priority = DomainPriority.General;

        //STATs
        private int m_totalrunningtime = 0;
        private int m_totalerrortime = 0;

        private int m_totaltask = 0;
        private int m_totalsucceedtask = 0;        

        #endregion

        #region Private Attribute - PreparationTask

        private bool m_IsGenerateNewTaskTriggerEnable = false;

        //一次性添加2个BeforeTask任务
        private int m_PreparationTaskIncrement = 2;

        private string m_PreparationScript = null;        
        #endregion

        #region Private Attribute - Parser
        private string m_ParserScript = null;
        #endregion

        #region Private Attribute - Storage
        private string m_StorageScript = null;
        
        
        #endregion

        #region Private Methods

        private string MD5String(string value)
        {
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] d = System.Text.Encoding.ASCII.GetBytes(value);
                byte[] dc = x.ComputeHash(d);
                return System.Text.Encoding.ASCII.GetString(dc);
            }
            catch (Exception e)
            {
                log.Error("MD5 Compute Error!", e);
                return null;
            }
        }

        private void NewTimeStamp()
        {
            m_timestamp = DateTime.Now;
        }

        #endregion

        #region Public Attribute & Methods

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public string DomainGUID { get{return m_GUID;}}

        public bool Enable
        {
            get { return m_IsEnable; }
            set { m_IsEnable = value; }
        }

        public DomainPriority Priority
        {
            get { return m_priority; }
            set { m_priority = value; }
        }

        public DateTime TimeStamp { get { return m_timestamp; } }

        public VBALogLevelFlag LogLevelFlag
        {
            get { return m_levelflag; }
            set { m_levelflag = value; }
        }

        public int TotalRunningTime { get { return m_totalrunningtime; } }

        public int TotalErrorTime { get { return m_totalerrortime; } }

        public int TotalTask { get { return m_totaltask; } }

        public int TotalSucceedTask { get { return m_totalsucceedtask; } }

        public void IncreaseRunningTime(int runningtime)
        {
            m_totalrunningtime += runningtime;
        }

        public void IncreaseErrorTime(int errortime)
        {
            m_totalerrortime += errortime;
        }

        public void IncreaseTotalTask()
        {
            m_totaltask++;
        }

        public void IncreaseSucceedTask()
        {
            m_totalsucceedtask++;
        }

        public void ResetStats()
        {
            m_totalerrortime = 0;
            m_totalrunningtime = 0;
            m_totalsucceedtask = 0;
            m_totaltask = 0;
        }

        public void NewDomainGUID()
        {
            m_GUID = Guid.NewGuid().ToString();
            NewTimeStamp();
        }

        #endregion

        #region Public Methods - PreparationTask

        public bool GenerateNewTaskTriggeEnable
        {
            get { return m_IsGenerateNewTaskTriggerEnable; }
            set { m_IsGenerateNewTaskTriggerEnable = value; }
        }

        public string PreparationScript
        {
            get { return m_PreparationScript; }
            set 
            { 
                m_PreparationScript = value;
                NewTimeStamp();
            }
        }

        public int PreparationTaskIncrement
        {
            get { return m_PreparationTaskIncrement; }
            set 
            { 
                if (value > 0) m_PreparationTaskIncrement = value;
                NewTimeStamp();
            }
        }

        #endregion

        #region Public Methods - Parsing

        public string ParserScript
        {
            get { return m_ParserScript; }
            set 
            {
                m_ParserScript = value;
                NewTimeStamp();
            }
        }
       
        #endregion

        #region Public Methods - Storage

        public string StorageScript
        {
            get { return m_StorageScript; }
            set 
            { 
                m_StorageScript = value;
                NewTimeStamp();
            }
        }

        #endregion

        public Domain()
        {
            NewDomainGUID();
        }

        public Domain(string name)
        {            
            m_name = name;
            NewDomainGUID();
        }

        public Domain(string name, string preparationscript, string parserscript, string storagescript)
        {            
            m_name = name;
            m_PreparationScript = preparationscript;
            m_ParserScript = parserscript;
            m_StorageScript = storagescript;
            NewDomainGUID();
        }
    }
}
