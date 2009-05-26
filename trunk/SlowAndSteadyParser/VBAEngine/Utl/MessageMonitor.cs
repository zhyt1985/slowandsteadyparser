using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SlowAndSteadyParser
{
    public static class MessageMonitor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);        

        public static event UpdateMessageEventHandle UpdateMessage = null;

        public delegate void UpdateMessageEventHandle(string uniname, string message);

        public static void Update(string uniname, string message)
        {
            if (UpdateMessage != null)
            {
                UpdateMessage(uniname,message);
            }
        }
    }

    //public class UpdateMessageEventArgs : EventArgs
    //{
    //    public string UniqueName;
    //    public string Message;

    //    public UpdateMessageEventArgs() { }
    //    public UpdateMessageEventArgs(string uniname, string message) 
    //    {
    //        this.UniqueName = uniname;
    //        this.Message = message;
    //    }
    //}
}
