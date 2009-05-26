using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SlowAndSteadyParser
{
    public partial class FrmWelcome : Form
    {
        public FrmWelcome()
        {
            InitializeComponent(); 
            this.labelVersion.Text = this.labelVersion.Text + Application.ProductVersion;                       
        }

        private void FrmWelcome_Load(object sender, EventArgs e)
        {

        }
    }
}