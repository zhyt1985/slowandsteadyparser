using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public enum NAVIGATESTATUS
    {
        Ready = 0,
        Navigating = 1,
        Done = 2,
        GenralError = 3,    //一般错误
        NET_RESOURCE_NOT_FOUND = 4,   //没有可用网络连接
        HTTP_STATUS_NOT_FOUND = 5,  //404 错误
    }

    public enum VBATaskStatus 
    { 
        Ready,          //任务就绪(默认状态)
        RestartADSL,    //任务重启,ADSL重启,切换IP
        Failure,        //任务失败
        Error,          //任务出错,由管理器重启
        Close,          //关闭当前解决方案
        TurnToNext      //关闭当前解决方案,启动下一个解决方案
    };

    public enum VBAObjectLife
    {
        Task,           //只在任务之中存活
        TaskChain       //在整条任务链中存活
    };

    [Flags]
    public enum VBALogLevelFlag
    {
        None = 0,
        Debug = 1,
        Info = 2,
        Warn = 4,
        Error = 8,
        Fatal = 16
    };
}
