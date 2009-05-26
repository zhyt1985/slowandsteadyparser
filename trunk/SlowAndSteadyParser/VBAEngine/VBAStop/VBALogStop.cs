using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public class VBALogStop : VBALog
    {
        #region IVBALog 成员

        public override void Debug(string d, Exception e)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void Debug(string d)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void Error(string r)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void Error(string r, Exception e)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void Fatal(string f, Exception e)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void Fatal(string f)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void Info(string info, Exception e)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void Info(string info)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void LogListLog(VBALogLevelFlag level, string message)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override List<VBALog.LogList> Loglists
        {
            get { throw new Exception("VBAEngine Stopping Exception!"); }
        }

        public override void Warn(string w, Exception e)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void Warn(string w)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        #endregion

        #region IVBAObject 成员

        public override void Reset()
        {

        }
        #endregion

    }
}
