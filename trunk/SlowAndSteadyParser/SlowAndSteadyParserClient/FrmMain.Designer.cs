namespace SlowAndSteadyParser
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.Main_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ModeSelected_toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LowMode_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StandardMode_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HighMode_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartClient_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StopClient_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Configuration_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Log_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Quit_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.labelClientStatus = new System.Windows.Forms.Label();
            this.StartClient_button = new System.Windows.Forms.Button();
            this.StopClient_button = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.labelMode = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelServerStatus = new System.Windows.Forms.Label();
            this.notifyIconMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Main_ToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(242, 24);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // Main_ToolStripMenuItem
            // 
            this.Main_ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ModeSelected_toolStripMenuItem,
            this.StartClient_ToolStripMenuItem,
            this.StopClient_ToolStripMenuItem,
            this.toolStripSeparator1,
            this.Configuration_ToolStripMenuItem,
            this.Log_ToolStripMenuItem,
            this.toolStripSeparator2,
            this.Quit_ToolStripMenuItem});
            this.Main_ToolStripMenuItem.Name = "Main_ToolStripMenuItem";
            this.Main_ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.Main_ToolStripMenuItem.Text = "控制";
            this.Main_ToolStripMenuItem.Click += new System.EventHandler(this.Main_ToolStripMenuItem_Click);
            // 
            // ModeSelected_toolStripMenuItem
            // 
            this.ModeSelected_toolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LowMode_ToolStripMenuItem,
            this.StandardMode_ToolStripMenuItem,
            this.HighMode_ToolStripMenuItem});
            this.ModeSelected_toolStripMenuItem.Name = "ModeSelected_toolStripMenuItem";
            this.ModeSelected_toolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.ModeSelected_toolStripMenuItem.Text = "模式切换";
            // 
            // LowMode_ToolStripMenuItem
            // 
            this.LowMode_ToolStripMenuItem.CheckOnClick = true;
            this.LowMode_ToolStripMenuItem.Name = "LowMode_ToolStripMenuItem";
            this.LowMode_ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.LowMode_ToolStripMenuItem.Text = "低速模式";
            this.LowMode_ToolStripMenuItem.Click += new System.EventHandler(this.LowMode_ToolStripMenuItem_Click);
            // 
            // StandardMode_ToolStripMenuItem
            // 
            this.StandardMode_ToolStripMenuItem.CheckOnClick = true;
            this.StandardMode_ToolStripMenuItem.Name = "StandardMode_ToolStripMenuItem";
            this.StandardMode_ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.StandardMode_ToolStripMenuItem.Text = "标准模式";
            this.StandardMode_ToolStripMenuItem.Click += new System.EventHandler(this.StandardMode_ToolStripMenuItem_Click);
            // 
            // HighMode_ToolStripMenuItem
            // 
            this.HighMode_ToolStripMenuItem.CheckOnClick = true;
            this.HighMode_ToolStripMenuItem.Name = "HighMode_ToolStripMenuItem";
            this.HighMode_ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.HighMode_ToolStripMenuItem.Text = "高速模式";
            this.HighMode_ToolStripMenuItem.Click += new System.EventHandler(this.HighMode_ToolStripMenuItem_Click);
            // 
            // StartClient_ToolStripMenuItem
            // 
            this.StartClient_ToolStripMenuItem.Name = "StartClient_ToolStripMenuItem";
            this.StartClient_ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.StartClient_ToolStripMenuItem.Text = "启动";
            this.StartClient_ToolStripMenuItem.Click += new System.EventHandler(this.StartClient_ToolStripMenuItem_Click);
            // 
            // StopClient_ToolStripMenuItem
            // 
            this.StopClient_ToolStripMenuItem.Name = "StopClient_ToolStripMenuItem";
            this.StopClient_ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.StopClient_ToolStripMenuItem.Text = "停止";
            this.StopClient_ToolStripMenuItem.Click += new System.EventHandler(this.StopClient_ToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(119, 6);
            // 
            // Configuration_ToolStripMenuItem
            // 
            this.Configuration_ToolStripMenuItem.Name = "Configuration_ToolStripMenuItem";
            this.Configuration_ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.Configuration_ToolStripMenuItem.Text = "配置";
            this.Configuration_ToolStripMenuItem.Click += new System.EventHandler(this.Configuration_ToolStripMenuItem_Click);
            // 
            // Log_ToolStripMenuItem
            // 
            this.Log_ToolStripMenuItem.Name = "Log_ToolStripMenuItem";
            this.Log_ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.Log_ToolStripMenuItem.Text = "查看日志";
            this.Log_ToolStripMenuItem.Click += new System.EventHandler(this.Log_ToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(119, 6);
            // 
            // Quit_ToolStripMenuItem
            // 
            this.Quit_ToolStripMenuItem.Name = "Quit_ToolStripMenuItem";
            this.Quit_ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.Quit_ToolStripMenuItem.Text = "退出";
            this.Quit_ToolStripMenuItem.Click += new System.EventHandler(this.Quit_ToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "客户端状态:";
            // 
            // labelClientStatus
            // 
            this.labelClientStatus.AutoSize = true;
            this.labelClientStatus.Location = new System.Drawing.Point(128, 45);
            this.labelClientStatus.Name = "labelClientStatus";
            this.labelClientStatus.Size = new System.Drawing.Size(47, 12);
            this.labelClientStatus.TabIndex = 3;
            this.labelClientStatus.Text = "running";
            // 
            // StartClient_button
            // 
            this.StartClient_button.Location = new System.Drawing.Point(27, 126);
            this.StartClient_button.Name = "StartClient_button";
            this.StartClient_button.Size = new System.Drawing.Size(69, 22);
            this.StartClient_button.TabIndex = 4;
            this.StartClient_button.Text = "启动";
            this.StartClient_button.UseVisualStyleBackColor = true;
            this.StartClient_button.Click += new System.EventHandler(this.StartClient_button_Click);
            // 
            // StopClient_button
            // 
            this.StopClient_button.Location = new System.Drawing.Point(130, 126);
            this.StopClient_button.Name = "StopClient_button";
            this.StopClient_button.Size = new System.Drawing.Size(69, 22);
            this.StopClient_button.TabIndex = 4;
            this.StopClient_button.Text = "停止";
            this.StopClient_button.UseVisualStyleBackColor = true;
            this.StopClient_button.Click += new System.EventHandler(this.StopClient_button_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "当前模式:";
            // 
            // labelMode
            // 
            this.labelMode.AutoSize = true;
            this.labelMode.Location = new System.Drawing.Point(128, 98);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(29, 12);
            this.labelMode.TabIndex = 3;
            this.labelMode.Text = "低速";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "服务器状态:";
            // 
            // labelServerStatus
            // 
            this.labelServerStatus.AutoSize = true;
            this.labelServerStatus.Location = new System.Drawing.Point(128, 72);
            this.labelServerStatus.Name = "labelServerStatus";
            this.labelServerStatus.Size = new System.Drawing.Size(47, 12);
            this.labelServerStatus.TabIndex = 3;
            this.labelServerStatus.Text = "running";
            // 
            // notifyIconMain
            // 
            this.notifyIconMain.Text = "notifyIcon1";
            this.notifyIconMain.Visible = true;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(242, 176);
            this.Controls.Add(this.StopClient_button);
            this.Controls.Add(this.StartClient_button);
            this.Controls.Add(this.labelMode);
            this.Controls.Add(this.labelServerStatus);
            this.Controls.Add(this.labelClientStatus);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "FrmMain";
            this.Text = "SAS.Parser 客户端";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem Main_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StopClient_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StartClient_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem Configuration_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Log_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem Quit_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ModeSelected_toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LowMode_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StandardMode_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HighMode_ToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelClientStatus;
        private System.Windows.Forms.Button StartClient_button;
        private System.Windows.Forms.Button StopClient_button;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelServerStatus;
        private System.Windows.Forms.NotifyIcon notifyIconMain;

    }
}

