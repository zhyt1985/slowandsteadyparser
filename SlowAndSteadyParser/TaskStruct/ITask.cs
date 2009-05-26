using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SlowAndSteadyParser
{
    public interface ITask : IDisposable
    {
        /// <summary>
        /// 用来识别该任务的类型
        /// </summary>
        string Name { get;}

        /// <summary>
        /// 任务状态
        /// </summary>
        TaskStatus Status { get;}
        
        /// <summary>
        /// 运行次数
        /// </summary>
        int RunningTime { get;}

        /// <summary>
        /// 出错次数
        /// </summary>
        int ErrorTime { get;set;}

        #region event

        /// <summary>
        /// 脚本执行前的事件
        /// </summary>
        event System.EventHandler BeforeScript;

        /// <summary>
        /// 脚本执行后的事件
        /// </summary>
        event System.EventHandler AfterScript;

        #endregion

        #region Methods

        /// <summary>
        /// 运行这个ITask
        /// </summary>
        void Run();

        /// <summary>
        /// 强制停止正在运行的ITask
        /// </summary>
        void Stop();

        /// <summary>
        /// 得到后续任务,输出null表示没有后续任务
        /// </summary>
        List<ITask> NextTasks();

        #endregion
    }
}
