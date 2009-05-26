using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Lifetime;

namespace SlowAndSteadyParser
{
    public class DomainTransmissionManager : MarshalByRefObject, IDomainTransmissionManager
    {

        #region IScriptTransmisionnManager 成员

        public Domain GetDomain(string domainGUID)
        {
            Domain d = DomainManager.GetDomain(domainGUID);
            if (d == null)
                throw new NullReferenceException(domainGUID + "不存在!");
            else
                return d;
        }

        #endregion

        #region MarshalByRefObject 成员
        public override object InitializeLifetimeService()
        {
            ILease lease = (ILease)base.InitializeLifetimeService();
            if (lease.CurrentState == LeaseState.Initial)
            {
                lease.InitialLeaseTime = TimeSpan.Zero;
            }
            return lease;
        }
        #endregion
    }
}
