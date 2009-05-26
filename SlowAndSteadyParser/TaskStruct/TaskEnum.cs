using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public enum TaskStatus 
    { 
        Ready,          //任务就绪(默认状态)
        Failure,        //任务失败(不进行后续任务)
        Succeed,        //任务成功(进行后续任务)
        Error,          //任务出错(在重试次数以内的重启,重试次数以外的放弃)
        Restart,        //任务重启(无条件重启,慎用!不向用户直接开放)
        Closing,        //用户手动关闭任务
        Abandoning      //错误太多次数,被强制关闭任务
    };

    public enum TaskPosition
    {
        Client,
        Server,
    };
}
