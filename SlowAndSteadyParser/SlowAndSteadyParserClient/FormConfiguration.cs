using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SlowAndSteadyParser
{
    [Serializable]
    public struct ConfigurationDetailInMode
    {
        public int QueryInterval;
        public int SingleTransportation;
        public int MaxRunningTask;
        public int MaxRetryTime;
        public int MaxCPUUsage;
    }

    [Serializable]
    public struct Configuration
    {
        public string ServerAddress;
        public int ServerPort;

        public string ADSLEntryName;
        public string ADSLUserName;
        public string ADSLPassword;

        public ClientMode CurrentMode;
        public Dictionary<ClientMode, ConfigurationDetailInMode> Configurations;            
    }

    public partial class FrmConfiguration : Form
    {
        private bool m_IsModified = false;
        
        public FrmConfiguration()
        {
            InitializeComponent();

            this.Shown += new EventHandler(frm_FrmShown);
            this.FormClosing += new FormClosingEventHandler(this.frm_FrmClosing);
            
        }

        private void frm_FrmShown(object sender, EventArgs e)
        {
            LoadConfiguration();
        }

        private void frm_FrmClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                SaveAndClose();
            }
        }

        private void SaveConfiguration()
        {
            if (!m_IsModified)
                return;

            Configuration m_c = new Configuration();

            m_c.ServerAddress = this.textBoxServerAddress.Text;
            m_c.ServerPort = (int)this.numericUpDownPort.Value;

            m_c.ADSLEntryName = this.textBoxADSLEntryName.Text;
            m_c.ADSLUserName = this.textBoxADSLUserName.Text;
            m_c.ADSLPassword = this.textBoxADSLPassword.Text;

            m_c.CurrentMode = ClientControlManager.Mode;

            m_c.Configurations = new Dictionary<ClientMode, ConfigurationDetailInMode>();            

            ConfigurationDetailInMode low = new ConfigurationDetailInMode();
            low.QueryInterval = (int)this.numericUpDownQueryIntervalLow.Value;
            low.SingleTransportation = (int)this.numericUpDownSingleTranspotationLow.Value;
            low.MaxRunningTask = (int)this.numericUpDownMaxRunningTaskLow.Value;
            low.MaxRetryTime = (int)this.numericUpDownMaxRetryTimeLow.Value;
            low.MaxCPUUsage = (int)this.numericUpDownCPUUsageLow.Value;            
            m_c.Configurations.Add(ClientMode.LowspeedMode, low);

            ConfigurationDetailInMode standard = new ConfigurationDetailInMode();
            standard.QueryInterval = (int)this.numericUpDownQueryIntervalStandard.Value;
            standard.SingleTransportation = (int)this.numericUpDownSingleTranspotationStandard.Value;
            standard.MaxRunningTask = (int)this.numericUpDownMaxRunningTaskStandard.Value;
            standard.MaxRetryTime = (int)this.numericUpDownMaxRetryTimeStandard.Value;
            standard.MaxCPUUsage = (int)this.numericUpDownCPUUsageStandard.Value;
            m_c.Configurations.Add(ClientMode.StandardspeedMode, standard);

            ConfigurationDetailInMode high = new ConfigurationDetailInMode();
            high.QueryInterval = (int)this.numericUpDownQueryIntervalHigh.Value;
            high.SingleTransportation = (int)this.numericUpDownSingleTranspotationHigh.Value;
            high.MaxRunningTask = (int)this.numericUpDownMaxRunningTaskHigh.Value;
            high.MaxRetryTime = (int)this.numericUpDownMaxRetryTimeHigh.Value;
            high.MaxCPUUsage = (int)this.numericUpDownCPUUsageHigh.Value;
            m_c.Configurations.Add(ClientMode.HighspeedMode, high);

            ClientControlManager.Configuration = m_c;

            m_IsModified = false;
        }

        private void LoadConfiguration()
        {
            Configuration m_c = ClientControlManager.Configuration;

            this.textBoxServerAddress.Text = m_c.ServerAddress;
            this.numericUpDownPort.Value = m_c.ServerPort;

            this.textBoxADSLEntryName.Text = m_c.ADSLEntryName;
            this.textBoxADSLUserName.Text = m_c.ADSLUserName;
            this.textBoxADSLPassword.Text = m_c.ADSLPassword;

            if (m_c.Configurations != null)
            {
                ConfigurationDetailInMode low = m_c.Configurations[ClientMode.LowspeedMode];
                this.numericUpDownQueryIntervalLow.Value = low.QueryInterval;
                this.numericUpDownSingleTranspotationLow.Value = low.SingleTransportation;
                this.numericUpDownMaxRunningTaskLow.Value = low.MaxRunningTask;
                this.numericUpDownMaxRetryTimeLow.Value = low.MaxRetryTime;
                this.numericUpDownCPUUsageLow.Value = low.MaxCPUUsage;

                ConfigurationDetailInMode standard = m_c.Configurations[ClientMode.StandardspeedMode];
                this.numericUpDownQueryIntervalStandard.Value = standard.QueryInterval;
                this.numericUpDownSingleTranspotationStandard.Value = standard.SingleTransportation;
                this.numericUpDownMaxRunningTaskStandard.Value = standard.MaxRunningTask;
                this.numericUpDownMaxRetryTimeStandard.Value = standard.MaxRetryTime;
                this.numericUpDownCPUUsageStandard.Value = standard.MaxCPUUsage;

                ConfigurationDetailInMode high = m_c.Configurations[ClientMode.HighspeedMode];
                this.numericUpDownQueryIntervalHigh.Value = high.QueryInterval;
                this.numericUpDownSingleTranspotationHigh.Value = high.SingleTransportation;
                this.numericUpDownMaxRunningTaskHigh.Value = high.MaxRunningTask;
                this.numericUpDownMaxRetryTimeHigh.Value = high.MaxRetryTime;
                this.numericUpDownCPUUsageHigh.Value = high.MaxCPUUsage;
            }

            m_IsModified = false;
        }

        private void SaveAndClose()
        {
            if (m_IsModified)
            {
                DialogResult r = MessageBox.Show("是否要保存修改过的配置?", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    SaveConfiguration();
                }                
            }

            this.Hide();
        }

        private void numericUpDownPort_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownMaxRetryTime_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void buttonTestADSL_Click(object sender, EventArgs e)
        {
            if (this.textBoxADSLEntryName.Text == "" || this.textBoxADSLUserName.Text == "")
            {
                MessageBox.Show("请输入ADSL连接名、账号、密码后再按测试钮", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ADSLFactory.Init(this.textBoxADSLEntryName.Text, this.textBoxADSLUserName.Text, this.textBoxADSLPassword.Text);
            ADSLFactory.Reconnect();
        }

        private void textBoxServerAddress_TextChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void textBoxADSLEntryName_TextChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void textBoxADSLUserName_TextChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void textBoxADSLPassword_TextChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownQueryIntervalLow_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownSingleTranspotationLow_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownMaxRunningTaskLow_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownCPUUsageLow_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownQueryIntervalStandard_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownSingleTranspotationStandard_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownMaxRunningTaskStandard_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownMaxRetryTimeStandard_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownCPUUsageStandard_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownQueryIntervalHigh_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownSingleTranspotationHigh_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownMaxRunningTaskHigh_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownMaxRetryTimeHigh_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void numericUpDownCPUUsageHigh_ValueChanged(object sender, EventArgs e)
        {
            m_IsModified = true;
        }

        private void buttonConfigurationSave_Click(object sender, EventArgs e)
        {
            SaveConfiguration();
            this.Hide();
        }

        private void buttonConfigurationReload_Click(object sender, EventArgs e)
        {
            LoadConfiguration();
        }

    }
}