using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using AIMS.Libraries.CodeEditor.Syntax;

namespace SlowAndSteadyParser
{
    public static class TestManager
    {
        private static object TestProccessStatusLocker = new object();

        private static DataGridView ms_dgv = null;
        private static Dictionary<VBALogLevelFlag, Image> ms_levelpics = null;
        private static SyntaxDocument ms_scripteditor = null;
        private static ToolStripButton ms_starttest = null;
        private static ToolStripButton ms_singlesteptest = null;
        private static ToolStripButton ms_stoptest = null;

        private static bool ms_IsClosed = false;
        private static TestPhase ms_tp = TestPhase.Nonstarted;
        private static TestProcessStatus ms_tps = TestProcessStatus.Ready;
        private static Dictionary<string, IVBAObject> ms_vbaobjects = null;
        private static BaseVBAScriptTask ms_task = null;
        private static Domain ms_domain = null;
        private static VBATestLog ms_testlog = null;

        private delegate void MessageHandlerInvokor(VBALogLevelFlag vflag, string message);
        private delegate void ToolStripInvokor();
        private delegate void SyntaxDocumentInvokor(string script);

        private static void SetSyntaxDocumentText(string script)
        {
            if (ms_dgv.InvokeRequired)
            {
                SyntaxDocumentInvokor t1 = new SyntaxDocumentInvokor(SetSyntaxDocumentText);
                ms_dgv.Invoke(t1, new object[] { script });
            }
            else
            {
                ms_scripteditor.Text = script;
            }
        }

        private static void RefreshTestToolButtons()
        {
            if (ms_dgv.InvokeRequired)
            {
                ToolStripInvokor t1 = new ToolStripInvokor(RefreshTestToolButtons);
                ms_dgv.Invoke(t1);
            }
            else
            {
                switch (ms_tps)
                {
                    case TestProcessStatus.Ready:
                        ms_starttest.Enabled = true;
                        ms_singlesteptest.Enabled = true;
                        ms_stoptest.Enabled = false;
                        break;
                    case TestProcessStatus.Singlestep_Pause:
                        ms_starttest.Enabled = true;
                        ms_singlesteptest.Enabled = true;
                        ms_stoptest.Enabled = true;
                        break;
                    case TestProcessStatus.Singlestep_Running:
                        ms_starttest.Enabled = false;
                        ms_singlesteptest.Enabled = false;
                        ms_stoptest.Enabled = true;
                        break;
                    case TestProcessStatus.Persistence:
                        ms_starttest.Enabled = false;
                        ms_singlesteptest.Enabled = false;
                        ms_stoptest.Enabled = true;
                        break;
                }
            }
        }

        private static void StopTestThread()
        {
            lock (TestProccessStatusLocker)
            {
                if (ms_task != null)
                {
                    ms_IsClosed = true;
                    switch (ms_tps)
                    {
                        case TestProcessStatus.Ready:
                            ms_task.Stop();
                            MessageBox.Show("ready!");
                            break;
                        case TestProcessStatus.Singlestep_Pause:
                            ms_task.Stop();
                            ms_testlog.PulseOne();
                            break;
                        case TestProcessStatus.Persistence:
                            ms_task.Stop();
                            break;
                        case TestProcessStatus.Singlestep_Running:
                            ms_task.Stop();
                            break;
                    }                    
                }
            }
        }

        public static void BeforeTestHandler()
        {
            if (ms_tp == TestPhase.Nonstarted)
            {
                SetSyntaxDocumentText(ms_domain.PreparationScript);
                ms_task = new LocalPreparationVBAScriptTask(ms_domain.DomainGUID, ms_vbaobjects);
                ms_task.AfterScript += new EventHandler(AfterTestHandler);
                ms_tp = TestPhase.Preparation;
                ms_task.Run();
               
            }
        }

        public static void AfterTestHandler(object sender, EventArgs e)
        {
            BaseVBAScriptTask whichtask = (BaseVBAScriptTask)sender;

            if (whichtask.Status == TaskStatus.Succeed && ms_IsClosed == false)
            {
                switch (ms_tp)
                {
                    case TestPhase.Preparation:
                        SetSyntaxDocumentText(ms_domain.ParserScript);
                        ms_task = new LocalParserVBAScriptTask(ms_domain.DomainGUID, whichtask.TaskChainGUID, ms_vbaobjects);
                        whichtask.VBAObjs = null;
                        ms_task.AfterScript += new EventHandler(AfterTestHandler);
                        ms_task.Run();
                        ms_tp = TestPhase.Parser;
                        whichtask.Dispose();
                        break;
                    case TestPhase.Parser:
                        SetSyntaxDocumentText(ms_domain.StorageScript);
                        ms_task = new LocalStorageVBAScriptTask(ms_domain.DomainGUID, whichtask.TaskChainGUID, ms_vbaobjects);
                        whichtask.VBAObjs = null;
                        ms_task.AfterScript += new EventHandler(AfterTestHandler);
                        ms_task.Run();
                        ms_tp = TestPhase.Storage;
                        whichtask.Dispose();
                        break;
                    case TestPhase.Storage:
                        MessageBox.Show("测试结束,请检查日志以确定代码是否正确", "通知", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        whichtask.Dispose();
                        ms_task = null;
                        ms_tps = TestProcessStatus.Ready;
                        RefreshTestToolButtons();                        
                        break;
                }
            }
            else
            {
                whichtask.Dispose();
                ms_task = null;
                ms_tps = TestProcessStatus.Ready;
                RefreshTestToolButtons();              
            }

            
        }

        public static void MessageHandler(VBALogLevelFlag vflag, string message)
        {
            if (ms_dgv.InvokeRequired)
            {
                MessageHandlerInvokor m1 = new MessageHandlerInvokor(MessageHandler);
                ms_dgv.BeginInvoke(m1, new object[] { vflag, message });
            }
            else
            {
                //加入dataview
                ms_dgv.Rows.Insert(0, new object[] { ms_levelpics[vflag], vflag, message });

                lock (TestProccessStatusLocker)
                {
                    if (ms_tps == TestProcessStatus.Singlestep_Running)
                    {
                        //改变tps
                        ms_tps = TestProcessStatus.Singlestep_Pause;
                        RefreshTestToolButtons();
                    }
                }
            }
        }

        public static void Init(DataGridView dgv, 
            SyntaxDocument s, 
            Dictionary<VBALogLevelFlag, Image> levelpics,
            ToolStripButton starttest,
            ToolStripButton singlesteptest,
            ToolStripButton stoptest
            )
        {
            ms_dgv = dgv;
            ms_scripteditor = s;
            ms_levelpics = levelpics;
            ms_starttest = starttest;
            ms_singlesteptest = singlesteptest;
            ms_stoptest = stoptest;
        }

        public static void Dispose()
        {

        }

        private static void StartSinglestepTest(Domain d)
        {
            lock (TestProccessStatusLocker)
            {
                if (ms_tps == TestProcessStatus.Ready)
                {
                    
                    //Domain
                    ms_domain = d;

                    //清空log区
                    ms_dgv.Rows.Clear();

                    //重置Close Flag
                    ms_IsClosed = false;

                    //准备vba脚本对象
                    ms_vbaobjects = new Dictionary<string, IVBAObject>();
                    IVBAObject ivo = new VBATask();
                    ms_vbaobjects.Add(ivo.Name, ivo);

                    //小心处理伪Log对象
                    ms_testlog = new VBATestLog(true);
                    ms_testlog.MessageEvent += new VBATestLog.ScriptPauseEventHandler(MessageHandler);
                    ms_vbaobjects.Add(ms_testlog.Name, ms_testlog);

                    ivo = new VBATestIE();
                    ms_vbaobjects.Add(ivo.Name, ivo);
                    ivo = new VBAUtility();
                    ms_vbaobjects.Add(ivo.Name, ivo);
                    ivo = new VBAHtml();
                    ms_vbaobjects.Add(ivo.Name, ivo);

                    ms_tp = TestPhase.Nonstarted;
                    Thread t = new Thread(BeforeTestHandler);
                    t.Name = "Thread For Singlestep Test";
                    t.Start();


                }
                else if (ms_tps == TestProcessStatus.Singlestep_Pause)
                {
                    //放行
                    ms_testlog.PulseOne();
                }

                ms_tps = TestProcessStatus.Singlestep_Running;
                RefreshTestToolButtons();
            }
        }

        private static void StartPersistantTest(Domain d)
        {
            lock (TestProccessStatusLocker)
            {
                if (ms_tps == TestProcessStatus.Ready)
                {
                    //Domain
                    ms_domain = d;

                    //清空log区
                    ms_dgv.Rows.Clear();

                    //重置Close Flag
                    ms_IsClosed = false;

                    //准备vba脚本对象
                    ms_vbaobjects = new Dictionary<string, IVBAObject>();
                    IVBAObject ivo = new VBATask();
                    ms_vbaobjects.Add(ivo.Name, ivo);

                    //小心处理伪Log对象
                    ms_testlog = new VBATestLog(false);
                    ms_testlog.MessageEvent += new VBATestLog.ScriptPauseEventHandler(MessageHandler);
                    ms_vbaobjects.Add(ms_testlog.Name, ms_testlog);

                    ivo = new VBATestIE();
                    ms_vbaobjects.Add(ivo.Name, ivo);
                    ivo = new VBAUtility();
                    ms_vbaobjects.Add(ivo.Name, ivo);
                    ivo = new VBAHtml();
                    ms_vbaobjects.Add(ivo.Name, ivo);

                    ms_tp = TestPhase.Nonstarted;
                    Thread t = new Thread(BeforeTestHandler);
                    t.Name = "Thread For Persistant Test";
                    t.Start();
                }
                else if (ms_tps == TestProcessStatus.Singlestep_Pause)
                {
                    //改成持续运行模式
                    ms_testlog.TurnPersistance();
                    //放行
                    ms_testlog.PulseOne();
                }

                ms_tps = TestProcessStatus.Persistence;
                RefreshTestToolButtons();
            }
  
        }

        public static void TooltripStartTest(Domain d)
        {
            StartPersistantTest(d);
        }

        public static void TooltripSinglestepTest(Domain d)
        {            
            StartSinglestepTest(d);
        }

        public static void TooltripStopTest()
        {
            StopTestThread();
        }

        public static void WindowRefresh()
        {
            RefreshTestToolButtons();
            ms_dgv.Rows.Clear();
            SetSyntaxDocumentText("");
        }

        public static bool IsTesting()
        {
            if (ms_tps == TestProcessStatus.Ready)
                return false;
            else
                return true;
        }

    }

    public enum TestPhase
    {
        Nonstarted,
        Preparation,
        Parser,
        Storage
    }

    public enum TestProcessStatus
    {
        Ready,
        Singlestep_Running,
        Singlestep_Pause,
        Persistence
    }
}
