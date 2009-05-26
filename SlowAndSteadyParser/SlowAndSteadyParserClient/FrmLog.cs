using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SlowAndSteadyParser
{
    public partial class FrmLog : Form
    {
        public FrmLog()
        {
            InitializeComponent();
            //log4net.Appender.RichTextBoxAppender.SetRichTextBox(richTextBoxLog, "RichTextBoxAppender");
            log4net.Appender.TextBoxAppender.SetTextBox(textBoxLog);
        }

        private void FrmLog_Load(object sender, EventArgs e)
        {
           
        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}