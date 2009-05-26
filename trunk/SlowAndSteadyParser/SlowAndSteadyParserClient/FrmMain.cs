using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Specialized;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;

namespace SlowAndSteadyParser
{
    public partial class FrmMain : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private Form m_frmLog = new FrmLog();
        private Form m_frmConfig = new FrmConfiguration();
                
        public FrmMain()
        {
            InitializeComponent();                  
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClientControlManager.Dispose(this);

            // Log End!
            log.Info("Application [" + System.Reflection.Assembly.GetEntryAssembly().GetName().Name + "] End");
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            // Log an info level message
            log.Info("Application [" + System.Reflection.Assembly.GetEntryAssembly().GetName().Name + "] Start");

            //Init
            ClientControlManager.Init(this);
            
            //Init ServerStatusMonitor
            ServerStatusMonitor.Init(this, this.labelServerStatus);

            //Refresh Application Interface
            RefreshClientControl();
            RefreshMode();

            //Refresh Server Status
            ConnectionManagerClient.FetchServerStatus();

            //Show Vision
            this.Text = this.Text + " - " + Application.ProductVersion;

        }

        private void RefreshClientControl()
        {
            if (ClientControlManager.IsClientRunning)
            {
                this.StartClient_button.Enabled = false;
                this.StopClient_button.Enabled = true;

                this.StartClient_ToolStripMenuItem.Enabled = false;
                this.StopClient_ToolStripMenuItem.Enabled = true;

                this.labelClientStatus.Text = "运行";
            }
            else
            {
                this.StartClient_button.Enabled = true;
                this.StopClient_button.Enabled = false;

                this.StartClient_ToolStripMenuItem.Enabled = true;
                this.StopClient_ToolStripMenuItem.Enabled = false;

                this.labelClientStatus.Text = "停止";
            }

        }

        private void RefreshMode()
        {
            switch (ClientControlManager.Mode)
            {
                case ClientMode.LowspeedMode:
                    this.LowMode_ToolStripMenuItem.Checked = true;
                    this.StandardMode_ToolStripMenuItem.Checked = false;
                    this.HighMode_ToolStripMenuItem.Checked = false;
                    this.labelMode.Text = "低速模式";
                    break;
                case ClientMode.StandardspeedMode:
                    this.LowMode_ToolStripMenuItem.Checked = false;
                    this.StandardMode_ToolStripMenuItem.Checked = true;
                    this.HighMode_ToolStripMenuItem.Checked = false;
                    this.labelMode.Text = "标准模式";
                    break;
                case ClientMode.HighspeedMode:
                    this.LowMode_ToolStripMenuItem.Checked = false;
                    this.StandardMode_ToolStripMenuItem.Checked = false;
                    this.HighMode_ToolStripMenuItem.Checked = true;
                    this.labelMode.Text = "高速模式";
                    break;
            }
        }

        private void Quit_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Log_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!m_frmLog.Visible)
                m_frmLog.Show(this);
        }

        private void StartClient_button_Click(object sender, EventArgs e)
        {
            if (!ClientControlManager.IsClientRunning)
                ClientControlManager.Start();
            RefreshClientControl();
        }

        private void StopClient_button_Click(object sender, EventArgs e)
        {
            if (ClientControlManager.IsClientRunning)
                ClientControlManager.Stop();
            RefreshClientControl();
        }

        private void Configuration_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!m_frmConfig.Visible)
                m_frmConfig.Show(this);
        }

        private void Main_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshMode();
        }

        private void StartClient_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ClientControlManager.IsClientRunning)
                ClientControlManager.Start();
            RefreshClientControl();
        }

        private void StopClient_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClientControlManager.IsClientRunning)
                ClientControlManager.Stop();
            RefreshClientControl();
        }

        private void LowMode_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientControlManager.Mode = ClientMode.LowspeedMode;
            RefreshMode();
        }

        private void StandardMode_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientControlManager.Mode = ClientMode.StandardspeedMode;
            RefreshMode();
        }

        private void HighMode_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientControlManager.Mode = ClientMode.HighspeedMode;
            RefreshMode();
        }

      
    }
}