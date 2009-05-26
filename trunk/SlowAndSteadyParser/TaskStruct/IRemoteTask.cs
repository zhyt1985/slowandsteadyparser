using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public interface IRemoteTask : ITask, ICloneable
    {
        TaskPosition Position { get;}
    }
}
