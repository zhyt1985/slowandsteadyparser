using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    class VBAHtmlStop : VBAHtml
    {
        public override string NodeClassName
        {
            get
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
            set
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
        }

        public override string NodeHref
        {
            get { throw new Exception("VBAEngine Stopping Exception!"); }
        }

        public override string NodeId
        {
            get
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
            set
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
        }

        public override string NodeInnerHtml
        {
            get
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
            set
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
        }

        public override string NodeInnerText
        {
            get
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
            set
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
        }

        public override bool NodeIsNull()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool NodeMoveNext()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool NodeMoveNext(int time)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool NodeMoveNextCondition(NodeSeekConditions cs)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string NodeOuterHTML
        {
            get
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
            set
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
        }

        public override string NodeOuterText
        {
            get
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
            set
            {
                throw new Exception("VBAEngine Stopping Exception!");
            }
        }

        public override bool NodePopRange()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool NodePushRangeAll()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool NodePushRangeById(string id)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool NodePushRangeByName(string elementname)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool NodePushRangeByTagName(string tagname)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool NodePushRangeChildren()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool NodePushRangeImages()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool NodePushRangeLinks()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool NodePushRangeScripts()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string NodeTagName
        {
            get { throw new Exception("VBAEngine Stopping Exception!"); }
        }

        public override NodeSeekConditions NodeSeekClassName(string classname)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override NodeSeekConditions NodeSeekId(string id)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override NodeSeekConditions NodeSeekInnerHTML(string innerHTML, bool exactmatch)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override NodeSeekConditions NodeSeekInnerText(string innerText, bool exactmatch)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override NodeSeekConditions NodeSeekTagName(string tagname)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string GetCurrentURL()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void Navigate(string url)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void Navigate(string url, int retrytime)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void NavigateDelay(string url, int delaymilliseconds)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string GetCurrentTitle()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string NodeGetAttribute(string attribute)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        #region IVBAObject 成员

        public override void Reset()
        {

        }

        #endregion
    }
}
