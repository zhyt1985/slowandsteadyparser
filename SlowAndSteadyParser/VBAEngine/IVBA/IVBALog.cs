using System;
namespace SlowAndSteadyParser
{
    interface IVBALog
    {
        void Debug(string d, Exception e);
        void Debug(string d);
        void Error(string r);
        void Error(string r, Exception e);
        void Fatal(string f, Exception e);
        void Fatal(string f);
        void Info(string info, Exception e);
        void Info(string info);
        void LogListLog(SlowAndSteadyParser.VBALogLevelFlag level, string message);
        System.Collections.Generic.List<SlowAndSteadyParser.VBALog.LogList> Loglists { get; }
        void Warn(string w, Exception e);
        void Warn(string w);
    }
}
