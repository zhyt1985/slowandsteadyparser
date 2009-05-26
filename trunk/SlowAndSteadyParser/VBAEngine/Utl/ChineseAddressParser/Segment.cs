using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SlowAndSteadyParser
{
    [Serializable]
    public class Segment
    {
        private string m_srcsegment;
        private Regex m_matchregex;

        public Regex Matchregex
        {
            get { return m_matchregex;}
            set { m_matchregex = value;}
        }

        public string StringSegment
        {
            get { return m_srcsegment; }
            set { m_srcsegment = value; }
        }

        public Segment(string srcsegment, Regex matchregex)
        {
            m_srcsegment = srcsegment;
            m_matchregex = matchregex;
        }
    }
}
