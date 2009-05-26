using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public class DoNothingWordmatch : AbstractWordmatch
    {
        public DoNothingWordmatch(string category)
            : base(category)
        {
            
        }
        
        protected override bool resolve(MatchingString ms)
        {
            return false;
        }
    }
}
