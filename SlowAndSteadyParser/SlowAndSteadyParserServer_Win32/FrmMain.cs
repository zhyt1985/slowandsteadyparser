using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Specialized;
using AIMS.Libraries.CodeEditor.SyntaxFiles;

namespace SlowAndSteadyParser
{
    public partial class FrmMain : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private FrmWelcome m_frmWelcome;
        private FrmLog m_frmLog;
        private FrmMonitor m_frmMonitor;
        private TreeViewDisplay m_tvd;
        private TreeNode m_LastSelectedTreeNode = null; //上一个选中的树形节点
        private Control m_LastWindowControl = null;
        private Dictionary<VBALogLevelFlag, Image> m_levelpics = null; //DomainLogPics
        private Button[] m_SendingTimeButtons = new Button[24];

        #region 窗体基础函数

        public FrmMain()
        {
            m_frmWelcome = new FrmWelcome();
            m_frmWelcome.Show(this);
            InitializeComponent();            
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            // Log Start!
            log.Info("Application [" + System.Reflection.Assembly.GetEntryAssembly().GetName().Name + "] Start");

            //Init Forms
            m_frmMonitor = new FrmMonitor(this, this.notifyIconServer);
            m_frmLog = new FrmLog();

            //Init!!!
            ServerControlManager.Init(this);            

            //Init TreeViewDisplay
            m_tvd = new TreeViewDisplay(this.treeViewMain);            

            //Re-Init Components!
            ReInitializeComponent();

            //Refresh Components
            RefreshServerWorkingComponents();
            RefreshDomainOperationComponents();
            RefreshWindowComponent();                   

            //dispose welcome form
            m_frmWelcome.Dispose();
            this.Activate();

            //Show Vision
            this.Text = this.Text + " - " + Application.ProductVersion;            
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //判断用户退出
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult r = MessageBox.Show("确实要退出SAS服务器程序?", "确认退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            //Dispose!!!
            ServerControlManager.Dispose(this);
            TestManager.Dispose();

            // Log End!
            log.Info("Application [" + System.Reflection.Assembly.GetEntryAssembly().GetName().Name + "] End");
        }

        private void FrmMain_SizeChanged(object sender, EventArgs e)
        {            
            WindowSizeChange();            
        } 

        #endregion

        #region 公用函数

        private void ReInitializeComponent()
        {
            //SendingTimeButton
            m_SendingTimeButtons[0] = this.buttonTimer0;
            m_SendingTimeButtons[1] = this.buttonTimer1;
            m_SendingTimeButtons[2] = this.buttonTimer2;
            m_SendingTimeButtons[3] = this.buttonTimer3;
            m_SendingTimeButtons[4] = this.buttonTimer4;
            m_SendingTimeButtons[5] = this.buttonTimer5;
            m_SendingTimeButtons[6] = this.buttonTimer6;
            m_SendingTimeButtons[7] = this.buttonTimer7;
            m_SendingTimeButtons[8] = this.buttonTimer8;
            m_SendingTimeButtons[9] = this.buttonTimer9;
            m_SendingTimeButtons[10] = this.buttonTimer10;
            m_SendingTimeButtons[11] = this.buttonTimer11;
            m_SendingTimeButtons[12] = this.buttonTimer12;
            m_SendingTimeButtons[13] = this.buttonTimer13;
            m_SendingTimeButtons[14] = this.buttonTimer14;
            m_SendingTimeButtons[15] = this.buttonTimer15;
            m_SendingTimeButtons[16] = this.buttonTimer16;
            m_SendingTimeButtons[17] = this.buttonTimer17;
            m_SendingTimeButtons[18] = this.buttonTimer18;
            m_SendingTimeButtons[19] = this.buttonTimer19;
            m_SendingTimeButtons[20] = this.buttonTimer20;
            m_SendingTimeButtons[21] = this.buttonTimer21;
            m_SendingTimeButtons[22] = this.buttonTimer22;
            m_SendingTimeButtons[23] = this.buttonTimer23;

            for (int i = 0; i < 24; i++)
            {
                m_SendingTimeButtons[i].Tag = false;
            }

            //datagridview
            this.dataGridViewDomainLog.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            this.dataGridViewDomainLog.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            //清空DummyWindow
            this.panelDummyWindow.Controls.Clear();

            //设定panelProperty是否修改过
            this.panelProperty.Tag = false;

            //codeeditor
            CodeEditorSyntaxLoader.SetSyntax(this.codeEditorControlWindow, SyntaxLanguage.VBNET);
            CodeEditorSyntaxLoader.SetSyntax(this.codeEditorControlTest, SyntaxLanguage.VBNET);


            //DomainLog
            m_levelpics = new Dictionary<VBALogLevelFlag, Image>();
            m_levelpics.Add(VBALogLevelFlag.Debug, global::SlowAndSteadyParser.Properties.Resources.debug);
            m_levelpics.Add(VBALogLevelFlag.Info, global::SlowAndSteadyParser.Properties.Resources.info);
            m_levelpics.Add(VBALogLevelFlag.Warn, global::SlowAndSteadyParser.Properties.Resources.warn);
            m_levelpics.Add(VBALogLevelFlag.Error, global::SlowAndSteadyParser.Properties.Resources.err);
            m_levelpics.Add(VBALogLevelFlag.Fatal, global::SlowAndSteadyParser.Properties.Resources.fatal);

            //初始化TestManager
            TestManager.Init(this.dataGridViewTestResult,this.syntaxDocumentTest, m_levelpics,
                this.StartTest_toolStripButton, this.SingleStepTest_toolStripButton, this.StopTest_toolStripButton);

            //init ballon
            
        }

        private void NotifyIconShowBallon()
        {
            string s = "";
            s = s + "服务器状态:    ";
            if (ServerControlManager.IsServerRunning == true )
                s = s + "运行"+Environment.NewLine;
            else
                s = s + "停止" + Environment.NewLine;
            s = s + "缓冲区任务:    " + ServerControlManager.ReservedPoolTask.ToString()+Environment.NewLine;
            s = s + DomainManager.DomainInfoToString();
            this.notifyIconServer.ShowBalloonTip(5, "SAS.Parser", s,ToolTipIcon.Info);
        }

        private void RefreshServerWorkingComponents()
        {
            //刷新TreeView
            m_tvd.SetServerNodeStatus();

            //刷新工具栏 - 服务器运行按钮
            if (ServerControlManager.IsServerRunning)
            {
                this.StartServer_toolStripButton.Enabled = false;
                this.StopServer_toolStripButton.Enabled = true;
                this.RestartServer_toolStripButton.Enabled = true;
            }
            else
            {
                this.StartServer_toolStripButton.Enabled = true;
                this.StopServer_toolStripButton.Enabled = false;
                this.RestartServer_toolStripButton.Enabled = false;
            }

            //刷新菜单栏 - 操作菜单
            if (ServerControlManager.IsServerRunning)
            {
                this.StartServer_ToolStripMenuItem.Enabled = false;
                this.StopServer_ToolStripMenuItem.Enabled = true;
                this.RestartServer_ToolStripMenuItem.Enabled = true;
            }
            else
            {
                this.StartServer_ToolStripMenuItem.Enabled = true;
                this.StopServer_ToolStripMenuItem.Enabled = false;
                this.RestartServer_ToolStripMenuItem.Enabled = false;
            }
        }

        private void RefreshDomainOperationComponents()
        {
            //刷新工具栏、菜单栏 - DeleteDomain按钮
            if (m_tvd.FocusdNodeType == TreeViewFocus.Domain)
            {
                DeleteDomain_toolStripButton.Enabled = true;
                DeleteDomain_ToolStripMenuItem.Enabled = true;
            }
            else
            {
                DeleteDomain_toolStripButton.Enabled = false;
                DeleteDomain_ToolStripMenuItem.Enabled = false;
            }

            //刷新工具栏、菜单栏 - SaveDomain按钮
            if (m_tvd.FocusdNodeType == TreeViewFocus.Domain)
            {
                SaveDomain_ToolStripButton.Enabled = true;
                SaveDomain_toolStripMenuItem.Enabled = true;
            }
            else
            {
                SaveDomain_ToolStripButton.Enabled = false;
                SaveDomain_toolStripMenuItem.Enabled = false;
            }

            //刷新工具栏 - DomainLogShowAll按钮
            if (m_tvd.FocusdNodeType == TreeViewFocus.Log)
            {
                toolStripButtonDomianLogShowAll.Visible = true;
            }
            else
            {
                toolStripButtonDomianLogShowAll.Visible = false;
            }



        }

        private void RefreshWindowHeadLine()
        {
            TreeViewFocus t = m_tvd.TreeNodeGetType(m_LastSelectedTreeNode);
            switch (t)
            {
                case TreeViewFocus.None:
                    break;
                case TreeViewFocus.Server:
                    this.Headline_toolStripLabel.Text = "服务器配置";
                    break;
                case TreeViewFocus.Domain:
                    Domain d = m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode);
                    this.Headline_toolStripLabel.Text = "解决方案: " + d.Name;
                    break;
                case TreeViewFocus.Script_Preparation:
                    Domain d1 = m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent);
                    this.Headline_toolStripLabel.Text = "解决方案: " + d1.Name + " >>> 数据初始化脚本";
                    break;
                case TreeViewFocus.Script_Parser:
                    Domain d2 = m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent);
                    this.Headline_toolStripLabel.Text = "解决方案: " + d2.Name + " >>> 数据采集脚本";
                    break;
                case TreeViewFocus.Script_Storage:
                    Domain d3 = m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent);
                    this.Headline_toolStripLabel.Text = "解决方案: " + d3.Name + " >>> 数据入库脚本";
                    break;
                case TreeViewFocus.Log:
                    Domain d4 = m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent);
                    this.Headline_toolStripLabel.Text = "解决方案: " + d4.Name + " >>> 日志  当前最低记录等级:"+d4.LogLevelFlag.ToString();
                    break;
                case TreeViewFocus.Test:
                    Domain d5 = m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent);
                    this.Headline_toolStripLabel.Text = "解决方案: " + d5.Name + " >>> 测试";
                    break;
            }
        }

        private void RefreshDomainPropertyControl(Domain d)
        {
            if (d.Enable)
            {
                this.buttonEnableDomain.Enabled = false;
                this.buttonDisableDomain.Enabled = true;
            }
            else
            {
                this.buttonEnableDomain.Enabled = true;
                this.buttonDisableDomain.Enabled = false;
            }
        }

        private void RefreshDomainPropertySTATs(Domain d)
        {
            //统计信息
            int total = d.TotalTask;
            int succeed = d.TotalSucceedTask;
            int runningtime = d.TotalRunningTime;
            double sp = (double)succeed / (double)total;
            double ef = (double)total / (double)runningtime;
            this.labelTotalTask.Text = total.ToString();
            this.labelSucceedTask.Text = succeed.ToString();
            this.labelRunningTime.Text = runningtime.ToString();
            this.labelSucceedPercentage.Text = sp.ToString("p");
            this.labelEfficiency.Text = ef.ToString("p");
        }

        private void RefreshConfiguration()
        {
            //统计
            this.labelServerRunningTask.Text = ServerControlManager.ServerRunningTask.ToString();
            this.labelServerWaitingTask.Text = ServerControlManager.ServerWaitingTask.ToString();
            this.labelReservedPoolTask.Text = ServerControlManager.ReservedPoolTask.ToString();

            //基础
            if (ServerControlManager.IsServerModeRemote)
            {
                this.buttonModeServer.Enabled = false;
                this.buttonModeSingle.Enabled = true;
            }
            else
            {
                this.buttonModeServer.Enabled = true;
                this.buttonModeSingle.Enabled = false;
            }

            //消息
            if (ServerControlManager.IsUsingFetion)
            {
                this.textBoxFetionNumber.Enabled = true;
                this.textBoxFetionPSD.Enabled = true;
                this.groupBoxSendingTime.Enabled = true;
                this.buttonTestFetion.Enabled = true;
            }
            else
            {
                this.textBoxFetionNumber.Enabled = false;
                this.textBoxFetionPSD.Enabled = false;
                this.groupBoxSendingTime.Enabled = false;
                this.buttonTestFetion.Enabled = false;
            }

            for (int i = 0; i < 24; i++)
            {
                if ((bool)m_SendingTimeButtons[i].Tag == true)
                {
                    m_SendingTimeButtons[i].ForeColor = Color.Red;
                }
                else
                {
                    m_SendingTimeButtons[i].ForeColor = Color.Black;
                }
            }
        }

        //处理上个窗体的结束任务
        private bool DealLastWindowComponent()
        {
            //不考虑失去焦点的情况
            if (m_LastWindowControl == null || treeViewMain.SelectedNode == null)
                return true;

            if (treeViewMain.SelectedNode != m_LastSelectedTreeNode)
            {
                TreeViewFocus t = m_tvd.TreeNodeGetType(m_LastSelectedTreeNode);
                switch (t)
                {
                    case TreeViewFocus.None:
                        return true;
                    case TreeViewFocus.Server:
                        return JudgeAndSaveModifiedConfiguration();
                    case TreeViewFocus.Domain:
                        return JudgeModifiedDomainProperty();
                    case TreeViewFocus.Log:
                        return true;
                    case TreeViewFocus.Script_Parser:
                    case TreeViewFocus.Script_Preparation:
                    case TreeViewFocus.Script_Storage:
                        return JudgeAndSaveModifiedScript(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent), t);
                    case TreeViewFocus.Test:
                        return JudgeTest();
                }
            }
            return true;
        }
        
        private void RefreshWindowComponent()
        {
            //如果没有选中任何Node
            if (treeViewMain.SelectedNode == null || treeViewMain.SelectedNode == m_LastSelectedTreeNode)
            {
                return;
            }

            //如果检查未通过,则返回上个位置
            if (!DealLastWindowComponent())
            {
                treeViewMain.SelectedNode = m_LastSelectedTreeNode;
                return;
            }

            //设定变量
            m_LastSelectedTreeNode = treeViewMain.SelectedNode;

            //显示Loading图片
            this.toolStripLabelLoading.Visible = true;

            //刷新右侧窗体
            TreeViewFocus t = m_tvd.FocusdNodeType;            
            switch (t)
            {
                case TreeViewFocus.None:
                    break;
                case TreeViewFocus.Server:
                    if (m_LastWindowControl !=null)
                        this.panelDummyWindow.Controls.Remove(m_LastWindowControl);
                    this.panelDummyWindow.Controls.Add(this.panelConfiguration);
                    m_LastWindowControl = this.panelConfiguration;
                    this.panelConfiguration.Dock = DockStyle.Fill;
                    WindowLoadConfiguration();
                    break;
                case TreeViewFocus.Domain:
                    if (m_LastWindowControl != null)
                        this.panelDummyWindow.Controls.Remove(m_LastWindowControl);
                    this.panelDummyWindow.Controls.Add(this.panelProperty);
                    m_LastWindowControl = this.panelProperty;
                    this.panelProperty.Dock = DockStyle.Fill;
                    Domain d = m_tvd.TreeViewGetFocusdDomain();                    
                    this.WindowLoadDomainProperty(d);
                    break;
                case TreeViewFocus.Script_Preparation:
                case TreeViewFocus.Script_Parser:
                case TreeViewFocus.Script_Storage:
                    if (m_LastWindowControl != null)
                        this.panelDummyWindow.Controls.Remove(m_LastWindowControl);
                    this.panelDummyWindow.Controls.Add(this.codeEditorControlWindow);
                    m_LastWindowControl = this.codeEditorControlWindow;
                    this.codeEditorControlWindow.Dock = DockStyle.Fill;
                    this.WindowLoadScript(m_tvd.TreeNodeGetDomain(this.treeViewMain.SelectedNode.Parent), t);
                    break;
                case TreeViewFocus.Log:
                    if (m_LastWindowControl != null)
                        this.panelDummyWindow.Controls.Remove(m_LastWindowControl);
                    this.panelDummyWindow.Controls.Add(this.panelLog);
                    m_LastWindowControl = this.panelLog;
                    this.panelLog.Dock = DockStyle.Fill;
                    this.WindowLoadDomainLog(m_tvd.TreeNodeGetDomain(this.treeViewMain.SelectedNode.Parent));
                    break;
                case TreeViewFocus.Test :
                    if (m_LastWindowControl != null)
                        this.panelDummyWindow.Controls.Remove(m_LastWindowControl);
                    this.panelDummyWindow.Controls.Add(this.panelTest);
                    m_LastWindowControl = this.panelTest;
                    this.panelTest.Dock = DockStyle.Fill;
                    this.WindowLoadTest(m_tvd.TreeNodeGetDomain(this.treeViewMain.SelectedNode.Parent));
                    break;
            }

            //刷新显示
            m_tvd.RefreshAllDomainNode();

            //隐藏Loading图片
            this.toolStripLabelLoading.Visible = false;

            //刷新Headline
            RefreshWindowHeadLine();
        }

        private void OpenDomainFile()
        {
            Domain d = ServerControlManager.OpenDomainFile();
            if (d != null)
                m_tvd.TreeViewAddDomain(d, true, true);
        }

        private void SaveDomainFile()
        {
            ServerControlManager.SaveDomainFile(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode));
        }

        private void AddDomainNode()
        {
            m_tvd.TreeViewAddDomain(DomainManager.NewDomain(), true, true);
        }

        private void DeleteDomainNode()
        {
            if (m_tvd.FocusdNodeType == TreeViewFocus.Domain)
            {

                DialogResult r = MessageBox.Show("确定要删除解决方案 " + m_tvd.TreeViewGetFocusdDomain().Name + "?", "删除解决方案", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    Domain d = m_tvd.TreeViewRemoveFocusdDomainNode();
                    DomainManager.RemoveDomain(d);
                }
            }
        }

        private bool JudgeModifiedDomainProperty()
        {
            if ((bool)(this.panelProperty.Tag) == true)
            {
                DialogResult r = MessageBox.Show("是否需要保存对该解决方案属性的修改?", "保存确认", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    SaveModifiedDomainProperty(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode));
                    m_tvd.SetDomainNodeStatus(m_LastSelectedTreeNode);
                    RefreshWindowHeadLine();
                    return true;
                }
                else
                {
                    if (r == DialogResult.Cancel)
                    {
                        return false;
                    }
                    else
                        return true;

                }
            }
            return true;
        }

        private bool JudgeTest()
        {
            if (TestManager.IsTesting())
            {
                DialogResult r = MessageBox.Show("存在启动的测试, 是否需要停止?", "测试停止确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    TestManager.TooltripStopTest();
                    return true;
                }
                else
                    return false;
            }
            else
            {
                return true;
            }
        }

        private int LevelFlagToInt(VBALogLevelFlag dflag)
        {
            int i = 0;
            //Debug
            if ((VBALogLevelFlag.Debug & dflag) != 0)
                i = 0;
            //Info
            if ((VBALogLevelFlag.Info & dflag) != 0)
                i = 1;            
            //Warn
            if ((VBALogLevelFlag.Warn & dflag) != 0)
                i = 2;            
            //Error
            if ((VBALogLevelFlag.Error & dflag) != 0)
                i = 3;
            //Fatal
            if ((VBALogLevelFlag.Fatal & dflag) != 0)
                i = 4;
            return i;
        }

        private VBALogLevelFlag IntToLevelFlag(int i)
        {
            if (i < 0 || i > 4)
                return VBALogLevelFlag.None;

            return (VBALogLevelFlag)(Math.Pow(2, i));
        }

        private void SaveModifiedDomainProperty(Domain d)
        {
            d.Name = this.textBoxDomainName.Text;            
            d.Priority = (DomainPriority)(this.comboBoxPriority.SelectedIndex);
            d.LogLevelFlag = IntToLevelFlag(this.comboBoxDomainLogLevel.SelectedIndex);

            //更新条件
            ServerControlManager.CheckForReservedPool();
        }

        private void SaveModifiedConfiguration()
        {
            //基础
            ServerControlManager.ServerPort = (int)this.numericUpDownPort.Value;
            ServerControlManager.IsMinToSystemTray = this.checkBoxSystemTray.Checked;

            //运行
            ServerControlManager.MinWaitingClientTask =(int)this.numericUpDownMinReservedTask.Value;
            ServerControlManager.MaxRunningTask = (int)this.numericUpDownMaxRunningTask.Value;
            ServerControlManager.MaxRetryTime = (int)this.numericUpDownMaxRetryTime.Value;

            //ADSL
            ServerControlManager.ADSLEntry = this.textBoxADSLEntryName.Text;
            ServerControlManager.ADSLUserName = this.textBoxADSLUserName.Text;
            ServerControlManager.ADSLPassword = this.textBoxADSLPassword.Text;

            //日志
            ServerControlManager.LogReservedDays = (int)this.numericUpDownLogReservedDay.Value;
            ServerControlManager.LogLoadingLimit = (int)this.numericUpDownMaxLogLoadingLimit.Value;

            //消息
            ServerControlManager.MessagePeriod = (int)this.numericUpDownMessagePeriod.Value;
            ServerControlManager.FetionNumber = this.textBoxFetionNumber.Text;
            ServerControlManager.FetionPsd = this.textBoxFetionPSD.Text;
            for (int i = 0; i < 24; i++)
            {
                ServerControlManager.FetionSendingTime[i] = (bool)m_SendingTimeButtons[i].Tag;   
            }             

            //更新条件
            ServerControlManager.CheckForReservedPool();
        }

        private bool JudgeAndSaveModifiedScript(Domain d, TreeViewFocus t)
        {
            if (this.syntaxDocumentWindow.Modified)
            {
                DialogResult r = MessageBox.Show("是否需要保存对该脚本的修改?", "保存确认", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {                    
                    switch (t)
                    {
                        case TreeViewFocus.Script_Preparation:
                            d.PreparationScript = this.syntaxDocumentWindow.Text;
                            break;
                        case TreeViewFocus.Script_Parser:
                            d.ParserScript = this.syntaxDocumentWindow.Text;
                            break;
                        case TreeViewFocus.Script_Storage:
                            d.StorageScript = this.syntaxDocumentWindow.Text;
                            break;
                    }
                    return true;
                }
                else
                {
                    if (r == DialogResult.Cancel)
                    {                        
                        return false;
                    }
                    else
                        return true;

                }
            }
            else
                return true;
            
        }

        private bool JudgeAndSaveModifiedConfiguration()
        {
            if ((bool)this.panelConfiguration.Tag == true)
            {
                DialogResult r = MessageBox.Show("是否需要保存对服务器配置的修改?", "保存确认", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    SaveModifiedConfiguration();
                    return true;
                }
                else
                {
                    if (r == DialogResult.Cancel)
                    {
                        return false;
                    }
                    else
                        return true;

                }
            }
            else
                return true;
        }

        private void WindowLoadConfiguration()
        {
            //基础
            this.numericUpDownPort.Value = ServerControlManager.ServerPort;
            this.checkBoxSystemTray.Checked = ServerControlManager.IsMinToSystemTray;

            //运行
            this.numericUpDownMinReservedTask.Value = ServerControlManager.MinWaitingClientTask;
            this.numericUpDownMaxRunningTask.Value = ServerControlManager.MaxRunningTask;
            this.numericUpDownMaxRetryTime.Value = ServerControlManager.MaxRetryTime;

            //ADSL
            this.textBoxADSLEntryName.Text = ServerControlManager.ADSLEntry;
            this.textBoxADSLUserName.Text = ServerControlManager.ADSLUserName;
            this.textBoxADSLPassword.Text = ServerControlManager.ADSLPassword;

            //消息
            this.checkBoxFetionUsage.Checked = ServerControlManager.IsUsingFetion;
            this.numericUpDownMessagePeriod.Value = ServerControlManager.MessagePeriod;
            this.textBoxFetionNumber.Text = ServerControlManager.FetionNumber;
            this.textBoxFetionPSD.Text = ServerControlManager.FetionPsd;
            for (int i = 0; i < 24; i++)
            {
                m_SendingTimeButtons[i].Tag = ServerControlManager.FetionSendingTime[i];
            }

            //日志
            this.numericUpDownLogReservedDay.Value = ServerControlManager.LogReservedDays;
            this.numericUpDownMaxLogLoadingLimit.Value = ServerControlManager.LogLoadingLimit;

            //刷新窗体
            RefreshConfiguration();

            this.panelConfiguration.Tag = false;
        }

        private void WindowLoadDomainProperty(Domain d)
        {
            //控制
            RefreshDomainPropertyControl(d);

            //统计信息
            RefreshDomainPropertySTATs(d);

            //属性
            this.textBoxDomainName.Text = d.Name;            
            this.comboBoxPriority.SelectedIndex = (int)d.Priority;
            this.comboBoxDomainLogLevel.SelectedIndex = LevelFlagToInt(d.LogLevelFlag);

            //设定修改值
            this.panelProperty.Tag = false;
        }

        private void WindowLoadScript(Domain d, TreeViewFocus t)
        {
            switch (t)
            {
                case TreeViewFocus.Script_Preparation:
                    this.syntaxDocumentWindow.Text = d.PreparationScript;
                    this.syntaxDocumentWindow.Modified = false;
                    
                    break;
                case TreeViewFocus.Script_Parser:
                    this.syntaxDocumentWindow.Text = d.ParserScript;
                    this.syntaxDocumentWindow.Modified = false;
                    
                    break;
                case TreeViewFocus.Script_Storage:
                    this.syntaxDocumentWindow.Text = d.StorageScript;
                    this.syntaxDocumentWindow.Modified = false;
                    
                    break;
            }
        }

        private void WindowLoadDomainLog(Domain d)
        {
            //取log数据            
            this.dataGridViewDomainLog.Rows.Clear();
            LogDatabaseManager.DataGridViewLoadLog(d, this.dataGridViewDomainLog, m_levelpics, ButtonStatusToLevelFlags(),ServerControlManager.LogLoadingLimit);
            this.toolStripButtonDomianLogShowAll.Enabled = false;
            this.dataGridViewDomainLog.Tag = null;
        }

        private void WindowLoadDomainLog(Domain d, string taskchainGUID)
        {
            //取log数据            
            this.dataGridViewDomainLog.Rows.Clear();
            LogDatabaseManager.DataGridViewLoadLog(d, taskchainGUID, this.dataGridViewDomainLog, m_levelpics, ButtonStatusToLevelFlags(), ServerControlManager.LogLoadingLimit);
            this.toolStripButtonDomianLogShowAll.Enabled = true;
            this.dataGridViewDomainLog.Tag = taskchainGUID;
        }

        private void WindowLoadTest(Domain d)
        {
            TestManager.WindowRefresh();
        }

        public VBALogLevelFlag ButtonStatusToLevelFlags()
        {
            //得到levelflags
            VBALogLevelFlag levelflag = 0;
            if (debug_toolStripButton.Checked) levelflag |= VBALogLevelFlag.Debug;
            if (info_toolStripButton.Checked) levelflag |= VBALogLevelFlag.Info;
            if (warn_toolStripButton.Checked) levelflag |= VBALogLevelFlag.Warn;
            if (error_toolStripButton.Checked) levelflag |= VBALogLevelFlag.Error;
            if (fatal_toolStripButton.Checked) levelflag |= VBALogLevelFlag.Fatal;
            return levelflag;
        }

        public void LevelFlagsToButtonStatus(VBALogLevelFlag dflags)
        {
            //Debug
            if ((VBALogLevelFlag.Debug & dflags) != 0)
                debug_toolStripButton.Checked = true;
            else
                debug_toolStripButton.Checked = false;
            //Info
            if ((VBALogLevelFlag.Info & dflags) != 0)
                info_toolStripButton.Checked = true;
            else
                info_toolStripButton.Checked = false;
            //Warn
            if ((VBALogLevelFlag.Warn & dflags) != 0)
                warn_toolStripButton.Checked = true;
            else
                warn_toolStripButton.Checked = false;
            //Error
            if ((VBALogLevelFlag.Error & dflags) != 0)
                error_toolStripButton.Checked = true;
            else
                error_toolStripButton.Checked = false;
            //Fatal
            if ((VBALogLevelFlag.Fatal & dflags) != 0)
                fatal_toolStripButton.Checked = true;
            else
                fatal_toolStripButton.Checked = false;
        }

        private void WindowSizeChange()
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                if (ServerControlManager.IsMinToSystemTray)
                {
                    if (this.Visible)
                    {
                        this.Hide();
                        this.notifyIconServer.Visible = true;
                        NotifyIconShowBallon();
                    }
                    else
                    {
                        this.Show();
                        this.notifyIconServer.Visible = false;
                        this.WindowState = FormWindowState.Normal;
                        m_tvd.RefreshAllDomainNode();
                    }                
                }
            }
            else
            {             
                m_tvd.RefreshAllDomainNode();
            }
        }

        #endregion

        #region 事件处理函数

        private void StartServer_toolStripButton_Click(object sender, EventArgs e)
        {
            ServerControlManager.Start();
            RefreshServerWorkingComponents();
            RefreshConfiguration();
        }

        private void StopServer_toolStripButton_Click(object sender, EventArgs e)
        {
            ServerControlManager.Stop();
            RefreshServerWorkingComponents();
            RefreshConfiguration();
        }

        private void RestartServer_toolStripButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("开始重新启动服务器...", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            ServerControlManager.Stop();
            Thread.Sleep(500);
            ServerControlManager.Start();
            RefreshServerWorkingComponents();
            RefreshConfiguration();
        }

        private void StartServer_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerControlManager.Start();
            RefreshServerWorkingComponents();
            RefreshConfiguration();
        }

        private void StopServer_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerControlManager.Stop();
            RefreshServerWorkingComponents();
            RefreshConfiguration();
        }

        private void RestartServer_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("开始重新启动服务器...", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            ServerControlManager.Stop();
            Thread.Sleep(500);
            ServerControlManager.Start();
            RefreshServerWorkingComponents();
            RefreshConfiguration();
        }

        private void NewDomain_ToolStripButton_Click(object sender, EventArgs e)
        {
            AddDomainNode();
        }

        private void Newdomain_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddDomainNode();
        }

        private void DeleteDomain_toolStripButton_Click(object sender, EventArgs e)
        {
            DeleteDomainNode();
        }

        private void treeViewMain_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RefreshDomainOperationComponents();
            RefreshWindowComponent();
        }

        private void DeleteDomain_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteDomainNode();
        }

        private void buttonDomainPropertyConfirm_Click(object sender, EventArgs e)
        {
            Domain d = m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode);
            SaveModifiedDomainProperty(d);
            m_tvd.SetDomainNodeStatus(m_LastSelectedTreeNode);
            RefreshWindowHeadLine();
            MessageBox.Show("保存到解决方案:" + d.Name, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.panelProperty.Tag = false;
        }

        private void textBoxDomainName_TextChanged(object sender, EventArgs e)
        {
            this.panelProperty.Tag = true;
        }

        private void buttonDomainPropertyRecover_Click(object sender, EventArgs e)
        {
            WindowLoadDomainProperty(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode));
        }

        private void ServerLog_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!m_frmLog.Visible)
                m_frmLog.Show(this);
        }
        
        private void buttonEnableDomain_Click(object sender, EventArgs e)
        {
            Domain d = m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode);
            d.Enable = true;
            m_tvd.SetDomainNodeStatus(m_LastSelectedTreeNode);
            RefreshDomainPropertyControl(d);
            //更新条件
            ServerControlManager.CheckForReservedPool();
        }

        private void buttonDisableDomain_Click(object sender, EventArgs e)
        {
            Domain d = m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode);
            d.Enable = false;
            m_tvd.SetDomainNodeStatus(m_LastSelectedTreeNode);
            RefreshDomainPropertyControl(d);
            //更新条件
            ServerControlManager.CheckForReservedPool();
        }

        private void TestDomain_toolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void Quit_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridViewDomainLog_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridViewDomainLog.Columns[e.ColumnIndex] is DataGridViewLinkColumn
                && e.RowIndex != -1
                && this.toolStripButtonDomianLogShowAll.Enabled == false)
            {

                object value = dataGridViewDomainLog.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (value is DBNull) { return; }
                string taskguid = value.ToString();
                WindowLoadDomainLog(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent), taskguid);

            }
        }

        private void debug_toolStripButton_Click(object sender, EventArgs e)
        {
            if (m_tvd.TreeNodeGetType(m_LastSelectedTreeNode) == TreeViewFocus.Log)
            {
                if (dataGridViewDomainLog.Tag == null)
                    WindowLoadDomainLog(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent));
                else
                    WindowLoadDomainLog(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent), (string)dataGridViewDomainLog.Tag);
            }
        }

        private void info_toolStripButton_Click(object sender, EventArgs e)
        {
            if (m_tvd.TreeNodeGetType(m_LastSelectedTreeNode) == TreeViewFocus.Log)
            {
                if (dataGridViewDomainLog.Tag == null)
                    WindowLoadDomainLog(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent));
                else
                    WindowLoadDomainLog(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent), (string)dataGridViewDomainLog.Tag);
            }
        }

        private void warn_toolStripButton_Click(object sender, EventArgs e)
        {
            if (m_tvd.TreeNodeGetType(m_LastSelectedTreeNode) == TreeViewFocus.Log)
            {
                if (dataGridViewDomainLog.Tag == null)
                    WindowLoadDomainLog(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent));
                else
                    WindowLoadDomainLog(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent), (string)dataGridViewDomainLog.Tag);
            }
        }

        private void error_toolStripButton_Click(object sender, EventArgs e)
        {
            if (m_tvd.TreeNodeGetType(m_LastSelectedTreeNode) == TreeViewFocus.Log)
            {
                if (dataGridViewDomainLog.Tag == null)
                    WindowLoadDomainLog(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent));
                else
                    WindowLoadDomainLog(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent), (string)dataGridViewDomainLog.Tag);
            }
        }

        private void fatal_toolStripButton_Click(object sender, EventArgs e)
        {
            if (m_tvd.TreeNodeGetType(m_LastSelectedTreeNode) == TreeViewFocus.Log)
            {
                if (dataGridViewDomainLog.Tag == null)
                    WindowLoadDomainLog(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent));
                else
                    WindowLoadDomainLog(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent), (string)dataGridViewDomainLog.Tag);
            }
        }

        private void notifyIconServer_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WindowSizeChange();
            }
            else
            {
                NotifyIconShowBallon();
            }
                
        }

        private void notifyIconServer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WindowSizeChange();
            }
        }

        private void buttonPropertySTATClear_Click(object sender, EventArgs e)
        {
            Domain d = m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode);
            d.ResetStats();
            RefreshDomainPropertySTATs(d);
        }

        private void comboBoxPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.panelProperty.Tag = true;
        }

        private void comboBoxDomainLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.panelProperty.Tag = true;
        }

        private void toolStripButtonDomianLogShowAll_Click(object sender, EventArgs e)
        {
            WindowLoadDomainLog(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent));
        }

        private void StartTest_toolStripButton_Click(object sender, EventArgs e)
        {
            TestManager.TooltripStartTest(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent));
        }

        private void SingleStepTest_toolStripButton_Click(object sender, EventArgs e)
        {
            TestManager.TooltripSinglestepTest(m_tvd.TreeNodeGetDomain(m_LastSelectedTreeNode.Parent));
        }

        private void StopTest_toolStripButton_Click(object sender, EventArgs e)
        {
            TestManager.TooltripStopTest();
        }

        private void numericUpDownMinReservedTask_ValueChanged(object sender, EventArgs e)
        {
            this.panelConfiguration.Tag = true;
        }

        private void numericUpDownMaxRunningTask_ValueChanged(object sender, EventArgs e)
        {
            this.panelConfiguration.Tag = true;
        }

        private void numericUpDownMaxRetryTime_ValueChanged(object sender, EventArgs e)
        {
            this.panelConfiguration.Tag = true;
        }

        private void numericUpDownMaxLogLength_ValueChanged(object sender, EventArgs e)
        {
            this.panelConfiguration.Tag = true;
        }

        private void numericUpDownMaxLogLoadingLimit_ValueChanged(object sender, EventArgs e)
        {
            this.panelConfiguration.Tag = true;
            NumericUpDown n = sender as NumericUpDown;
            if (n.Value < 1)
                n.Value = 1;
            if (n.Value > 10000)
                n.Value = 10000;
        }

        private void buttonConfigurationSave_Click(object sender, EventArgs e)
        {
            SaveModifiedConfiguration();
            MessageBox.Show("保存到服务器配置", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.panelConfiguration.Tag = false;
        }

        private void buttonConfigurationReload_Click(object sender, EventArgs e)
        {
            WindowLoadConfiguration();
        }

        private void buttonConfigurationClearLog_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("确实要删除所有日志(操作不可恢复)?", "确认日志清除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r == DialogResult.Yes)
                LogDatabaseManager.ClearLogAll();
        }

        private void Configuration_toolStripButton_Click(object sender, EventArgs e)
        {
            m_tvd.MoveToRoot();
        }

        private void Configuration_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_tvd.MoveToRoot();
        }

        private void numericUpDownPort_ValueChanged(object sender, EventArgs e)
        {
            this.panelConfiguration.Tag = true;
        }

        private void buttonClearAllWaitingTask_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("确定要删除所有等待中的任务(不可恢复)?", "危险操作", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r == DialogResult.Yes)
            {
                ServerControlManager.ClearAllWaitingTask();
            }
            RefreshConfiguration();
        }

        private void OpenDomain_ToolStripButton_Click(object sender, EventArgs e)
        {
            OpenDomainFile();
        }

        private void OpenDomain_toolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDomainFile();
        }

        private void SaveDomain_toolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDomainFile();
        }

        private void SaveDomain_ToolStripButton_Click(object sender, EventArgs e)
        {
            SaveDomainFile();
        }

        private void test1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChineseAddress address = ChineseAddressParser.Parse(@"XXX-333-X宁波市慈溪市定福区美丽大街33号天一广场三楼(近朝阳北路)");
            MessageBox.Show(address.ToString());
        }

        private void test2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String ss = @"隐泉日本料理的名字很不错．第一次去那儿是听朋友介绍才去的． 寿司都做的很有意思，名字也起的都怪怪的...但口味还是很好的．而且，居然见到了２０００元一瓶的特级大吟酿，差点动心买下来．．．最后没舍得钱。";
            List<string> ls = CommentDimensionReduction.Reduce(ss);
            foreach (string s in ls)
                MessageBox.Show(s);
        }

        private void 同义词替换测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thesaurus ts = new Thesaurus(System.IO.Path.Combine(Environment.CurrentDirectory, "SegmentDict") + System.IO.Path.DirectorySeparatorChar + "tyc.mdb");
            string s = @"味道很不错，很好吃，很开心，一点也不贵，很便宜，人很多，很挤，空间很狭窄";
            string d = ts.Replace(s);
            MessageBox.Show(s + Environment.NewLine + d);
        }

        private void 随机用户名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                   UserNameCreator.getInstanse().getRandomUserName() + Environment.NewLine +
                   UserNameCreator.getInstanse().getRandomUserName() + Environment.NewLine +
                   UserNameCreator.getInstanse().getRandomUserName() + Environment.NewLine +
                   UserNameCreator.getInstanse().getRandomUserName() + Environment.NewLine +
                   UserNameCreator.getInstanse().getRandomUserName() + Environment.NewLine
                   );
        }

        private void checkBoxSystemTray_CheckedChanged(object sender, EventArgs e)
        {
            this.panelConfiguration.Tag = true;
        }

        private void ToolStripMenuItemMessageMonitor_Click(object sender, EventArgs e)
        {
            if (!m_frmMonitor.Visible)
            {
                m_frmMonitor.Show(this);
                m_frmMonitor.RefreshMessage();
            }
        }

        private void ToolStripMenuItemMessageTest_Click(object sender, EventArgs e)
        {
            MessageMonitor.Update("t1", UserNameCreator.getInstanse().getRandomUserName());
            MessageMonitor.Update("t2", UserNameCreator.getInstanse().getRandomUserName());
            MessageMonitor.Update("t3", UserNameCreator.getInstanse().getRandomUserName());
        }              

        private void buttonModeServer_Click(object sender, EventArgs e)
        {
            ServerControlManager.IsServerModeRemote = true;
            RefreshConfiguration();
            m_tvd.SetServerNodeStatus();
            ServerControlManager.CheckForReservedPool();
        }

        private void buttonModeSingle_Click(object sender, EventArgs e)
        {
            ServerControlManager.IsServerModeRemote = false;
            RefreshConfiguration(); 
            m_tvd.SetServerNodeStatus();
            ServerControlManager.CheckForReservedPool();
        }

        #endregion

        private void checkBoxFetionUsage_CheckedChanged(object sender, EventArgs e)
        {
            ServerControlManager.IsUsingFetion = this.checkBoxFetionUsage.Checked;
            RefreshConfiguration();
        }

        private void numericUpDownMessagePeriod_ValueChanged(object sender, EventArgs e)
        {
            this.panelConfiguration.Tag = true;
        }

        private void textBoxFetionNumber_TextChanged(object sender, EventArgs e)
        {
            this.panelConfiguration.Tag = true;
        }

        private void textBoxFetionPSD_TextChanged(object sender, EventArgs e)
        {
            this.panelConfiguration.Tag = true;
        }

        private void buttonTestFetion_Click(object sender, EventArgs e)
        {
            string returnmessage;
            if (this.textBoxFetionNumber.Text == "" || this.textBoxFetionPSD.Text == "")
                MessageBox.Show("必须填入移动电话的号码和密码", "缺少参数", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                returnmessage = FetionManager.TemperorySendSelfSMS(this.textBoxFetionNumber.Text, this.textBoxFetionPSD.Text, "这是SAS.Parser 服务器向您发送的测试信息"+Environment.NewLine+"现在是服务器时间 "+DateTime.Now.ToString());
                if (returnmessage != "success")
                    MessageBox.Show(returnmessage);
            }

        }

        private void messageStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(m_frmMonitor.ToString());
        }

        private int m_lasthour = DateTime.Now.Hour;

        private void timerMain_Tick(object sender, EventArgs e)
        {
            string sms;
            if (m_lasthour != DateTime.Now.Hour)
            {
                m_lasthour = DateTime.Now.Hour;
                if (ServerControlManager.IsUsingFetion && ServerControlManager.FetionSendingTime[DateTime.Now.Hour])
                {
                    sms = m_frmMonitor.ToString();
                    if (sms == "")
                    {
                        sms = sms + "[暂时没有服务器消息]" + Environment.NewLine;
                        sms = sms + "服务器状态:    ";
                        if (ServerControlManager.IsServerRunning == true)
                            sms = sms + "运行" + Environment.NewLine;
                        else
                            sms = sms + "停止" + Environment.NewLine;
                        sms = sms + DomainManager.DomainInfoToString();
                    }
                    log.Debug(FetionManager.TemperorySendSelfSMS(ServerControlManager.FetionNumber, ServerControlManager.FetionPsd, sms));

                }
            }
        }

        private void buttonTimer0_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer1_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer2_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer3_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer4_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer5_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer6_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer7_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer23_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer22_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer21_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer20_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer19_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer18_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer17_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer16_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer15_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer14_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer13_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer12_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer11_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer10_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer9_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void buttonTimer8_Click(object sender, EventArgs e)
        {
            (sender as Button).Tag = !(bool)(sender as Button).Tag;
            this.panelConfiguration.Tag = true;
            RefreshConfiguration();
        }

        private void aDSLTESTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ADSLFactory.Reconnect();
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

        private void textBoxADSLEntryName_TextChanged(object sender, EventArgs e)
        {
            this.panelConfiguration.Tag = true;
        }

        private void textBoxADSLUserName_TextChanged(object sender, EventArgs e)
        {
            this.panelConfiguration.Tag = true;
        }

        private void textBoxADSLPassword_TextChanged(object sender, EventArgs e)
        {
            this.panelConfiguration.Tag = true;
        }
 
    }
}