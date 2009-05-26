using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public class NoiseCollector : AbstractWordmatch
    {
        public NoiseCollector() : base("NoiseCollector") { }
        
        protected override bool resolve(MatchingString ms)
        {
            foreach (MatchingBlock mb in ms.GetAvaiableMatchingBlock())
            {
                mb.wordmatch = this;                
            }

            return true;
        }
    }
}
