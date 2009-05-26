using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public interface IVBAObjectHost
    {
        string Name { get;}
        IVBAObject Tenant { get; set; }
    }
}
