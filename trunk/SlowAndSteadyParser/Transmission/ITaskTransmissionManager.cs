using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public interface ITaskTransmissionManager
    {
        ServerStatus GetServerStatus();

        IRemoteTask FetchAClientTask();

        List<IRemoteTask> FetchClientTasks(int maxnumber);

        bool SendbackAClientTask(IRemoteTask whichtask);

        bool SendbackClientTasks(List<IRemoteTask> tasks);
    }
}
