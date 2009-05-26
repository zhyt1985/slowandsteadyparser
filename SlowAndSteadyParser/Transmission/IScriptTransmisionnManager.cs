using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public interface IDomainTransmissionManager
    {
        Domain GetDomain(string domainGUID);
    }
}
