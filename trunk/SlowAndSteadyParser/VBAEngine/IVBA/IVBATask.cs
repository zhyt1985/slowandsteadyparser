using System;
namespace SlowAndSteadyParser
{
    interface IVBATask
    {
        SlowAndSteadyParser.VBATaskStatus CurrentVBATaskStatus { get;set; }
        int GetHashInt(string key);
        string GetHashString(string key);
        object GetHashValue(string key);
        void RemoveHashValue(string key);
        bool SetHashValue(string key, object value);
        void TaskMessage(string message);
        void TaskMessage(string uniname, string message);
        void TaskFail();
        void TaskRestartADSL();
        void TaskClose();
        void TaskCloseAndTurnOnNextDomain();        
        string URL { get; set; }
        Random Random { get; }
        string LocalDirectory { get; }
    }
}
