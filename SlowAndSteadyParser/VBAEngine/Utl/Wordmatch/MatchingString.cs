using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace SlowAndSteadyParser
{
    public class MatchingBlock
    {
        public int startpoint;
        public int length;
        public AbstractWordmatch wordmatch;

        public MatchingBlock(AbstractWordmatch wm, int startp, int len)
        {
            this.wordmatch = wm;
            this.startpoint = startp;
            this.length = len;
        }

    }

    public class MatchingBlockStartpointComparer : IComparer<MatchingBlock>
    {
        #region IComparer<MatchingBlock> 成员

        public int Compare(MatchingBlock x, MatchingBlock y)
        {
            return x.startpoint.CompareTo(y.startpoint);
        } 

        #endregion
    }

    public class SplitPoint
    {
        public int splitpoint;        
    }

    public class MatchingString
    {
        protected string m_string;
        protected List<MatchingBlock> m_matchblocklist;

        public bool AddMatchingBlock(AbstractWordmatch wordmatch, int absolutestartpoint, int length)
        {
            if (absolutestartpoint < 0 || length > this.m_string.Length)
                return false;

            //查找它在哪个匹配块内
            foreach (MatchingBlock mpexist in m_matchblocklist)
            {
                if (mpexist.startpoint <= absolutestartpoint && absolutestartpoint + length <= mpexist.startpoint + mpexist.length)
                {
                    //拆分该块
                    SplitMatchingBlock(mpexist, absolutestartpoint, length, wordmatch);                    
                    return true;
                }
                
            }
            return false;            
        }

        public bool AddMatchingBlock(AbstractWordmatch wordmatch, MatchingBlock oldmb,  int relativestartpoint, int length)
        {
            return this.AddMatchingBlock(wordmatch, oldmb.startpoint + relativestartpoint, length);            
        }

        private void SplitMatchingBlock(MatchingBlock oldmb, int startpoint, int length, AbstractWordmatch wordmatch)
        {
            int oldstart = oldmb.startpoint;
            int oldlenth = oldmb.length;

            m_matchblocklist.Remove(oldmb);
            m_matchblocklist.Add(new MatchingBlock(wordmatch, startpoint, length));            

            if (oldstart < startpoint)
            {
                m_matchblocklist.Add(new MatchingBlock(null, oldstart, startpoint-oldstart));
            }

            if ((oldstart + oldlenth) > (startpoint+length))
            {
                m_matchblocklist.Add(new MatchingBlock(null, startpoint + length, oldstart + oldlenth - startpoint - length));
            }
        }

        public string MatchingBlockGetString(MatchingBlock mb)
        {
            return m_string.Substring(mb.startpoint, mb.length);
        }

        public List<MatchingBlock> GetAvaiableMatchingBlock()
        {
            List<MatchingBlock> listmb = new List<MatchingBlock>();
            foreach(MatchingBlock mb in m_matchblocklist)
            {
                if (mb.wordmatch == null)
                    listmb.Add(mb);
            }
            return listmb;
        }

        public StringCollection GetStringsByWordmatch(AbstractWordmatch wm)
        {
            StringCollection sc = new StringCollection();
            foreach (MatchingBlock mb in m_matchblocklist)
            {
                if (mb.wordmatch == wm)
                {
                    sc.Add(this.MatchingBlockGetString(mb));
                }
            }
            return sc;
        }

        public string GetStringByWordmatch(AbstractWordmatch wm)
        {            
            foreach (MatchingBlock mb in m_matchblocklist)
            {
                if (mb.wordmatch == wm)
                {
                    return MatchingBlockGetString(mb);
                }
            }
            return null;
        }

        public MatchingString(string str)
        {
            if (str != null && str != "")
            {
                m_string = str;
                m_matchblocklist = new List<MatchingBlock>();
                m_matchblocklist.Add(new MatchingBlock(null, 0, str.Length));
            }
            else
                throw new ArgumentNullException("MatchingString的值不能初始化为null或空值");

        }

        public int GetMatchingBlockNumber()
        {
            if (m_matchblocklist != null)
                return m_matchblocklist.Count;
            else
                return 0;
        }

        public override string ToString()
        {
            string s;
            s = "string: " + m_string + Environment.NewLine;
            foreach (MatchingBlock mb in m_matchblocklist)
            {
                s = s + "match:" + m_string.Substring(mb.startpoint, mb.length) + Environment.NewLine;
                s = s + "start:" + mb.startpoint+" length:" + mb.length + Environment.NewLine;
            }
            return s;
        }

    }
}
