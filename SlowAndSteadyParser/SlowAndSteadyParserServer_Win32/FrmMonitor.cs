using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SlowAndSteadyParser
{
    public partial class FrmMonitor : Form
    {
        private Dictionary<string, ServerMessage> m_servermessages = new Dictionary<string, ServerMessage>();
        private Form m_frmParent = null;
        private NotifyIcon m_notifyicon = null;
        private bool m_IsFirstShow = true;

        private delegate void Delegator();

        public FrmMonitor(Form frmParent, NotifyIcon notifyicon)
        {
            InitializeComponent();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMonitor_FormClosing);
            this.timerMonitor.Enabled = true;
            MessageMonitor.UpdateMessage += new SlowAndSteadyParser.MessageMonitor.UpdateMessageEventHandle(this.UpdateMessage);
            m_frmParent = frmParent;
            m_notifyicon = notifyicon;
        }

        private void frmMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void timerMonitor_Tick(object sender, EventArgs e)
        {
            lock (m_servermessages)
            {
                Dictionary<string, ServerMessage> tempmessage = new Dictionary<string, ServerMessage>();
                foreach (KeyValuePair<string, ServerMessage> kv in m_servermessages)
                {
                    if (kv.Value.TimeStamp.AddMinutes(ServerControlManager.MessagePeriod) > DateTime.Now)
                        tempmessage.Add(kv.Key, kv.Value);
                }
                m_servermessages = tempmessage;
            }
            RefreshMessage();
        }

        public void RefreshMessage()
        {
            if (this.Visible)
            {
                if (this.dataGridViewMonitor.InvokeRequired)
                {
                    this.dataGridViewMonitor.BeginInvoke(new Delegator(RefreshMessage));
                }
                else
                {
                    lock (m_servermessages)
                    {
                        foreach (DataGridViewRow dr in dataGridViewMonitor.Rows)
                            dr.Dispose();
                        this.dataGridViewMonitor.Rows.Clear();
                        foreach (ServerMessage sm in m_servermessages.Values)
                        {
                            this.dataGridViewMonitor.Rows.Add(new object[] { sm.Message, sm.TimeStamp.ToShortTimeString() });
                        }
                        this.dataGridViewMonitor.Sort(this.dataGridViewMonitor.Columns[1], ListSortDirection.Descending);
                    }
                }
            }


        }

        private void UpdateMessage(string uniname, string message)
        {
            ServerMessage ms;
            CheckMonitorWindow();
            ms = new ServerMessage();
            ms.Message = message;
            ms.TimeStamp = DateTime.Now;
            lock (m_servermessages)
            {
                if (m_servermessages.ContainsKey(uniname))
                {
                    m_servermessages[uniname] = ms;
                }
                else
                {
                    m_servermessages.Add(uniname, ms);
                }
            }
            //if (m_notifyicon.Visible)
            //{
            //    m_notifyicon.ShowBalloonTip(2, "新的消息", message, ToolTipIcon.Info);
            //}

            RefreshMessage();
        }

        private void CheckMonitorWindow()
        {
            if (m_IsFirstShow)
            {
                if (m_frmParent.InvokeRequired)
                {
                    m_frmParent.Invoke(new Delegator(CheckMonitorWindow));
                }
                else
                {
                    m_IsFirstShow = false;
                    this.Show(m_frmParent);
                }
            }
        }

        public override string ToString()
        {
            string sms = "";

            foreach (ServerMessage sm in m_servermessages.Values)
                sms = sms + sm.Message + Environment.NewLine;
            return sms;
        }
    }

    public struct ServerMessage
    {
        public string Message;
        public DateTime TimeStamp;
    }
}