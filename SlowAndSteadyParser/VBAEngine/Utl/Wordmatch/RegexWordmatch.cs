using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SlowAndSteadyParser
{
    public class RegexWordmatch : AbstractWordmatch
    {
        Regex m_featherfirst;
        Regex m_feathersecond;
        Regex m_featherthird;
        bool m_IsSingleMatch = true;

        public RegexWordmatch(string category, Regex feather)
            : base(category)
        {
            m_featherfirst = feather;
        }

        public RegexWordmatch(string category, Regex feather, bool issinglematch)
            : base(category)
        {
            m_featherfirst = feather;
            m_IsSingleMatch = issinglematch;
        }

        public RegexWordmatch(string category, Regex featherfirst, Regex feathersecond, bool issinglematch)
            : base(category)
        {
            m_featherfirst = featherfirst;
            m_feathersecond = feathersecond;
            m_IsSingleMatch = issinglematch;
        }

        public RegexWordmatch(string category, Regex featherfirst, Regex feathersecond, Regex featherthird, bool issinglematch)
            : base(category)
        {
            m_featherfirst = featherfirst;
            m_feathersecond = feathersecond;
            m_featherthird = featherthird;
            m_IsSingleMatch = issinglematch;
        }

        protected override bool resolve(MatchingString ms)
        {
            MatchCollection mc;
            foreach (MatchingBlock mb in ms.GetAvaiableMatchingBlock())
            {
                if (m_featherfirst != null)
                {
                    mc = m_featherfirst.Matches(ms.MatchingBlockGetString(mb));
                    foreach (Match m in mc)
                    {
                        if (m.Success)
                        {
                            ms.AddMatchingBlock(this, mb, m.Index, m.Length);
                            if (m_IsSingleMatch)
                                return false;
                        }
                    }
                }

                if (m_feathersecond != null)
                {
                    mc = m_feathersecond.Matches(ms.MatchingBlockGetString(mb));
                    foreach (Match m in mc)
                    {
                        if (m.Success)
                        {
                            ms.AddMatchingBlock(this, mb, m.Index, m.Length);
                            if (m_IsSingleMatch)
                                return false;
                        }
                    }
                }

                if (m_featherthird != null)
                {
                    mc = m_featherthird.Matches(ms.MatchingBlockGetString(mb));
                    foreach (Match m in mc)
                    {
                        if (m.Success)
                        {
                            ms.AddMatchingBlock(this, mb, m.Index, m.Length);
                            if (m_IsSingleMatch)
                                return false;
                        }
                    }
                }
            }
            return false;
        }
    }
}
