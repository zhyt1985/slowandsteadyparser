using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SlowAndSteadyParser
{
    public static class ServerStatusMonitor
    {
        private static System.Windows.Forms.Label ms_serverstatus = null;
        private static System.Windows.Forms.Form ms_ui = null;

        private delegate void SetLabelTextInvokor(Color c, string text);

        public static void Init(System.Windows.Forms.Form ui,  System.Windows.Forms.Label l)
        {
            ms_serverstatus = l;
            ms_ui = ui;
        }

        private static void SetLabelText(Color c, string text)
        {
            if (ms_ui != null && ms_ui.IsDisposed == false)
            {
                if (ms_ui.InvokeRequired)
                {
                    SetLabelTextInvokor s1 = new SetLabelTextInvokor(SetLabelText);
                    try
                    {
                        ms_ui.Invoke(s1, new object[] { c, text });
                    }
                    catch
                    {
                    }
                }
                else
                {
                    ms_serverstatus.ForeColor = c;
                    ms_serverstatus.Text = text;
                }
            }
        }

        public static void SetServerStatusErrorAddressAndPort()
        {
            SetLabelText(Color.Red, "服务器地址或端口号错误");            
        }

        public static void SetServerStatusEnable()
        {
            SetLabelText(Color.Black, "运行");      
        }


        public static void SetServerStatusDiabled()
        {
            SetLabelText(Color.Black, "停止");      
        }
    }
}
