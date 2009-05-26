using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public class VBATaskStop : VBATask
    {
        #region IVBATask 成员

        public override VBATaskStatus CurrentVBATaskStatus
        {
            get
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
            set
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
        }

        public override int GetHashInt(string key)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string GetHashString(string key)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override object GetHashValue(string key)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void RemoveHashValue(string key)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool SetHashValue(string key, object value)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void TaskMessage(string message)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void TaskMessage(string uniquename, string message)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void TaskFail()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void TaskRestartADSL()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void TaskClose()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void TaskCloseAndTurnOnNextDomain()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string URL
        {
            get
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
            set
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
        }

        public override Random Random
        {
            get
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
        }

        public override string LocalDirectory
        {
            get
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
        }

        #endregion

        #region IVBAObject 成员

        public override void Reset()
        {

        }

        #endregion

    }
}
