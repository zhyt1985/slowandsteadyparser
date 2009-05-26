using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SlowAndSteadyParser
{
    //自定义搜索条件的结构
    public struct NodeSeekCondition
    {
        public bool IsIncluding;
        public NodeSeekConditonType ConditionAttribute;
        public string ConditionValue;
    }

    public enum NodeSeekConditonType
    {
        TagName,
        ClassName,
        Id,
        InnerTextExactMatch,
        InnerTextPartialMatch,
        InnerHTMLExactMatch,
        InnerHTMLPartialMatch,
    }

    public class NodeSeekConditions : IEnumerable
    {

        private List<NodeSeekCondition> cs = new List<NodeSeekCondition>();

        public NodeSeekConditions(NodeSeekConditonType ConditonAttruibute, string ConditionValue)
        {
            NodeSeekCondition c = new NodeSeekCondition();
            c.ConditionAttribute = ConditonAttruibute;
            c.ConditionValue = ConditionValue;
            c.IsIncluding = true;
            cs.Add(c);
        }

        public NodeSeekConditions(List<NodeSeekCondition> cnew)
        {
            cs = cnew;
        }

        public static NodeSeekConditions operator +(NodeSeekConditions c1, NodeSeekConditions c2)
        {
            List<NodeSeekCondition> cnew = new List<NodeSeekCondition>();
            cnew.AddRange(c1.cs);
            cnew.AddRange(c2.cs);
            return new NodeSeekConditions(cnew);
        }

        public static NodeSeekConditions operator -(NodeSeekConditions c1, NodeSeekConditions c2)
        {
            List<NodeSeekCondition> cnew = new List<NodeSeekCondition>();
            cnew.AddRange(c1.cs);
            IEnumerator i = c2.cs.GetEnumerator();
            NodeSeekCondition e;
            while (i.MoveNext())
            {
                e = (NodeSeekCondition)(i.Current);
                e.IsIncluding = false;
            }
            cnew.AddRange(c2.cs);
            return new NodeSeekConditions(cnew);
        }

        public static NodeSeekConditions operator -(NodeSeekConditions c1)
        {
            List<NodeSeekCondition> cnew = new List<NodeSeekCondition>();
            IEnumerator i = c1.cs.GetEnumerator();
            NodeSeekCondition e;
            while (i.MoveNext())
            {
                e = (NodeSeekCondition)(i.Current);
                e.IsIncluding = false;
            }
            cnew.AddRange(c1.cs);
            return new NodeSeekConditions(cnew);
        }

        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.cs.GetEnumerator();
        }

        #endregion
    }
}
