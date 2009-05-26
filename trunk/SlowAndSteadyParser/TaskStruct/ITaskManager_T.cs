using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public interface ITaskManager<T> : IDisposable
    {
        string Name { get;set;}

        bool IsRunning { get;}

        int MaxRetryTime { get;set;}

        int MaxRunningThread { get;set;}

        bool AddTask(T whichtask);

        bool RemoveTask(T whichtask);

        void Start();

        void Stop();

    }
}
