using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.IO;

namespace SlowAndSteadyParser
{
    public class AggregateWordmatch : AbstractWordmatch
    {
        List<String> m_aggregate;
        bool m_IsSingleMatch = true;

        public List<String> Aggregate
        {
            get { return m_aggregate; }
        }

        public AggregateWordmatch(string category, List<String> aggregate)
            : base(category)
        {
            m_aggregate = aggregate;
        }

        public AggregateWordmatch(string category, List<String> aggregate, bool issinglematch)
            : base(category)
        {
            m_aggregate = aggregate;
            m_IsSingleMatch = issinglematch;
        }

        public AggregateWordmatch(string category, string filepath, bool issinglematch) : base(category)
        {
            System.IO.StreamReader reader = new StreamReader(filepath);
            m_aggregate = new List<String>();
            string line;
            while (reader.Peek() >= 0)
            {
                line = reader.ReadLine();
                m_aggregate.Add(line);
            }
        }

        protected override bool resolve(MatchingString ms)
        {
            int index = 0;
            bool IsContinue;
            string str;

            foreach (MatchingBlock mb in ms.GetAvaiableMatchingBlock())
            {
                foreach (string a in m_aggregate)
                {
                    IsContinue = true;
                    index = 0;
                    while (IsContinue)
                    {
                        str = ms.MatchingBlockGetString(mb);
                        index = str.IndexOf(a,index);
                        if (index > -1)
                        {
                            ms.AddMatchingBlock(this, mb, index, a.Length);
                            if (m_IsSingleMatch)
                                return false;                            
                            index++;
                        }
                        else
                            IsContinue = false;
                    }
                    
                }
            }
            return false;
        }
    }
}
