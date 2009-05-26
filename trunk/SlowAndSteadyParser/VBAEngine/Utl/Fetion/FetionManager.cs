using System;
using System.Collections.Generic;
using System.Text;
using NullStudio.Fetion_SDK;
using Imps.Client;
using Imps.Client.Core;
using NullStudio.Fetion_SDK.Event;
using System.Threading;

namespace SlowAndSteadyParser
{
    public static class FetionManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static bool ms_IsLog = true;
        private static bool ms_IsLoginIn;
        private static string ms_errormessage;
        private static FetionSDK sdk;

        static FetionManager()
        {
            sdk = new FetionSDK();

            sdk.SDK_UserSatusChange += new FetionSDK.SDK_UserSatusChangedEventHandler(sdk_SDK_UserSatusChange);
            sdk.SDK_ReceiveMessage += new FetionSDK.SDK_ReceiveMessageEventHandler(sdk_SDK_ReceiveMessage);
            sdk.SDK_Error += new FetionSDK.SDK_ErrorEventHandler(sdk_SDK_Error);
        }

        public static void Dispose()
        {
            if (sdk != null)
            {
                sdk.ActiveMainWindow();
            }
        }

        public static string TemperorySendSelfSMS(string fetionnumber, string fetionpsd, string message)
        {
            TemperorySendSelfSMSDelegator td = new TemperorySendSelfSMSDelegator(TemperorySendSelfSMSRunner);
            IAsyncResult ar = td.BeginInvoke(fetionnumber, fetionpsd, message, null, null);
            return td.EndInvoke(ar);
        }

        private delegate string TemperorySendSelfSMSDelegator(string fetionnumber, string fetionpsd, string message);

        private static string TemperorySendSelfSMSRunner(string fetionnumber, string fetionpsd, string message)
        {
            bool iscancle = false;
            ms_IsLoginIn = false;
            sdk.AccountManager.FillUserIdAndPassword(fetionnumber, fetionpsd, false);

            sdk.ActiveMainWindow();

            if (message == "")
                return "消息不能为空";

            CWBPool.NavigationBegin();

            //等待登陆结束
            lock (sdk)
            {
                sdk.AccountManager.Login();
                Monitor.Wait(sdk, 10000);
            }
            if (ms_IsLoginIn)
            {
                Thread.Sleep(1000);
                if (sdk.ContactControl.SendSMS.SendSMS(sdk.ContactControl.getMyself().Uri.Id.ToString(), message) != 1)
                {
                    Thread.Sleep(5000);
                    sdk.ContactControl.SendSMS.SendSMS(sdk.ContactControl.getMyself().Uri.Id.ToString(), message);
                }

                Thread.Sleep(1000);
                sdk.AccountManager.Logout(ref iscancle);

                CWBPool.NavigationEnd();

                return "success";
            }
            else
            {
                CWBPool.NavigationEnd();

                return ms_errormessage;
            }
        }

        static private void sdk_SDK_UserSatusChange(object send, UserSatusChangedEventArgs e)
        {
            if (e.NewStatus == UserAccountStatus.Loginning)
            {
                if (ms_IsLog) log.Debug("Fetion - 飞信:正在登陆...");
            }
            else if (e.NewStatus == UserAccountStatus.Logon)
            {
                if (ms_IsLog) log.Debug("Fetion - 飞信:登陆成功");
                ms_IsLoginIn = true;
                lock (sdk)
                {
                    Monitor.Pulse(sdk);
                }
            }
            else if (e.NewStatus == UserAccountStatus.Logouting)
            {
                if (ms_IsLog) log.Debug("Fetion - 飞信:正在退出登陆..");
            }
            else if (e.NewStatus == UserAccountStatus.Logoff)
            {
                if (ms_IsLog) log.Debug("Fetion - 飞信:退出登陆成功！");
                ms_IsLoginIn = false;
            }
            else if (e.NewStatus == UserAccountStatus.None)
            {
                if (ms_IsLog) log.Debug("用户名或密码错误！");
                ms_errormessage = "用户名或密码错误！";
                lock (sdk)
                {
                    Monitor.Pulse(sdk);
                }
            }
            else
            {
                if (ms_IsLog) log.Debug("Fetion - 飞信 " + e.NewStatus.ToString());
                ms_errormessage = e.NewStatus.ToString();
                lock (sdk)
                {
                    Monitor.Pulse(sdk);
                }
            }
        }

        static private void sdk_SDK_ReceiveMessage(object send, SDK_ReceiveMessageEventArgs e)
        {
            if (ms_IsLog) log.Debug(e.Contact.DisplayName + " " + e.DateTime.ToString("yyyy-MM-dd HH:mm:ss  \r\n") + e.Message + "\r\n");
        }

        static private void sdk_SDK_Error(object sender, SDK_ErrorEventArgs fe)
        {
            if (ms_IsLog) log.Debug("[Error:" + fe.ErrorType.ToString() + "] " + fe.Message);
        }
    }
}
