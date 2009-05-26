using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public abstract class AbstractWordmatch
    {
        //该词组识别器的分类名
        protected string m_category;
        private AbstractWordmatch next; 


        public AbstractWordmatch(string category)
        {
            m_category = category;
        }

        public AbstractWordmatch SetNext(AbstractWordmatch next)
        {
            this.next = next;
            return next;
        }

        public void Process(MatchingString ms)
        {
            if (resolve(ms))
                done(ms);
            else
                if (next != null)
                    next.Process(ms);
                else
                    fail(ms);
        }

        protected void fail(MatchingString ms)
        {
            
        }

        protected void done(MatchingString ms)
        {
            
        }

        protected abstract bool resolve(MatchingString ms);

        public override string ToString()
        {
            return "["+m_category+"]";
        }
    }
}
