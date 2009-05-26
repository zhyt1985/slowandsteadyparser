using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SlowAndSteadyParser
{
    [Serializable]
    public class Splitter
    {
        private Regex m_splitregex;
        private Regex[] m_matchregexs;
        bool m_issinglesplitter = true;

        public Regex SplitRegex
        {
            get { return m_splitregex; }
            set { m_splitregex = value; }
        }

        public Regex[] MatchRegexs
        {
            get { return m_matchregexs; }
            set { m_matchregexs = value; }
        }

        public bool IsSingleSplit
        {
            get { return m_issinglesplitter; }
            set { m_issinglesplitter = value; }
        }

        public Splitter(Regex splitregex, Regex[] matchregexs)
        {
            m_splitregex = splitregex;
            m_matchregexs = matchregexs;
        }

        public Splitter(Regex splitregex, Regex[] matchregexs, bool issinglesplitter)
        {
            m_splitregex = splitregex;
            m_matchregexs = matchregexs;
            m_issinglesplitter = issinglesplitter;
        }
    }
}
