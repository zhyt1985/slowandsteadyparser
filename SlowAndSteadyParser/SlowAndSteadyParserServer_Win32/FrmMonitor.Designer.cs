namespace SlowAndSteadyParser
{
    partial class FrmMonitor
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewMonitor = new System.Windows.Forms.DataGridView();
            this.ColumnMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDatetime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timerMonitor = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMonitor)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewMonitor
            // 
            this.dataGridViewMonitor.AllowUserToAddRows = false;
            this.dataGridViewMonitor.AllowUserToDeleteRows = false;
            this.dataGridViewMonitor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMonitor.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnMessage,
            this.ColumnDatetime});
            this.dataGridViewMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewMonitor.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewMonitor.Name = "dataGridViewMonitor";
            this.dataGridViewMonitor.ReadOnly = true;
            this.dataGridViewMonitor.RowTemplate.Height = 23;
            this.dataGridViewMonitor.Size = new System.Drawing.Size(354, 181);
            this.dataGridViewMonitor.TabIndex = 0;
            // 
            // ColumnMessage
            // 
            this.ColumnMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ColumnMessage.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColumnMessage.HeaderText = "消息";
            this.ColumnMessage.MinimumWidth = 50;
            this.ColumnMessage.Name = "ColumnMessage";
            this.ColumnMessage.ReadOnly = true;
            this.ColumnMessage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnMessage.Width = 50;
            // 
            // ColumnDatetime
            // 
            this.ColumnDatetime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnDatetime.HeaderText = "发布时间";
            this.ColumnDatetime.Name = "ColumnDatetime";
            this.ColumnDatetime.ReadOnly = true;
            this.ColumnDatetime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.ColumnDatetime.Width = 78;
            // 
            // timerMonitor
            // 
            this.timerMonitor.Interval = 60000;
            this.timerMonitor.Tick += new System.EventHandler(this.timerMonitor_Tick);
            // 
            // FrmMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 181);
            this.Controls.Add(this.dataGridViewMonitor);
            this.Name = "FrmMonitor";
            this.Text = "SAS.Parser 服务器 - 消息监视器";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMonitor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewMonitor;
        private System.Windows.Forms.Timer timerMonitor;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDatetime;
    }
}