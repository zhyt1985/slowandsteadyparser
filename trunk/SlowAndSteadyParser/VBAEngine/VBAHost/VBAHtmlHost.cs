using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public class VBAHtmlHost : IVBAObjectHost, IVBAHtml
    {
        private VBAHtml m_vbaobject = null;

        #region IVBAHtml 成员        

        public string NodeClassName
        {
            get
            {
                return m_vbaobject.NodeClassName;
            }
            set
            {
                m_vbaobject.NodeClassName = value;
            }
        }

        public string NodeHref
        {
            get { return m_vbaobject.NodeHref; }
        }

        public string NodeId
        {
            get
            {
                return m_vbaobject.NodeId;
            }
            set
            {
                m_vbaobject.NodeId = value;
            }
        }

        public string NodeInnerHtml
        {
            get
            {
                return m_vbaobject.NodeInnerHtml;
            }
            set
            {
                m_vbaobject.NodeInnerHtml = value;
            }
        }

        public string NodeInnerText
        {
            get
            {
                return m_vbaobject.NodeInnerText;
            }
            set
            {
                m_vbaobject.NodeInnerText = value;
            }
        }

        public bool NodeIsNull()
        {
            return m_vbaobject.NodeIsNull();
        }

        public bool NodeMoveNext()
        {
            return m_vbaobject.NodeMoveNext();
        }

        public bool NodeMoveNext(int time)
        {
            return m_vbaobject.NodeMoveNext(time);
        }

        public bool NodeMoveNextCondition(NodeSeekConditions cs)
        {
            return m_vbaobject.NodeMoveNextCondition(cs);
        }

        public string NodeOuterHTML
        {
            get
            {
                return m_vbaobject.NodeOuterHTML;
            }
            set
            {
                m_vbaobject.NodeOuterHTML = value;
            }
        }

        public string NodeOuterText
        {
            get
            {
                return m_vbaobject.NodeOuterText;
            }
            set
            {
                m_vbaobject.NodeOuterText = value;
            }
        }

        public bool NodePopRange()
        {
            return m_vbaobject.NodePopRange();
        }

        public bool NodePushRangeAll()
        {
            return m_vbaobject.NodePushRangeAll();
        }

        public bool NodePushRangeById(string id)
        {
            return m_vbaobject.NodePushRangeById(id);
        }

        public bool NodePushRangeByName(string elementname)
        {
            return m_vbaobject.NodePushRangeByName(elementname);
        }

        public bool NodePushRangeByTagName(string tagname)
        {
            return m_vbaobject.NodePushRangeByTagName(tagname);
        }

        public bool NodePushRangeChildren()
        {
            return m_vbaobject.NodePushRangeChildren();
        }

        public bool NodePushRangeImages()
        {
            return m_vbaobject.NodePushRangeImages();
        }

        public bool NodePushRangeLinks()
        {
            return m_vbaobject.NodePushRangeLinks();
        }

        public bool NodePushRangeScripts()
        {
            return m_vbaobject.NodePushRangeScripts();
        }

        public string NodeTagName
        {
            get { return m_vbaobject.NodeTagName; }
        }

        public string NodeGetAttribute(string attribute)
        {
            return m_vbaobject.NodeGetAttribute(attribute);
        }

        public NodeSeekConditions NodeSeekClassName(string classname)
        {
            return m_vbaobject.NodeSeekClassName(classname);
        }

        public NodeSeekConditions NodeSeekId(string id)
        {
            return m_vbaobject.NodeSeekId(id);
        }

        public NodeSeekConditions NodeSeekInnerHTML(string innerHTML, bool exactmatch)
        {
            return m_vbaobject.NodeSeekInnerHTML(innerHTML, exactmatch);
        }

        public NodeSeekConditions NodeSeekInnerText(string innerText, bool exactmatch)
        {
            return m_vbaobject.NodeSeekInnerText(innerText, exactmatch);
        }

        public NodeSeekConditions NodeSeekTagName(string tagname)
        {
            return m_vbaobject.NodeSeekTagName(tagname);
        }

        public string GetCurrentURL()
        {
            return m_vbaobject.GetCurrentURL();
        }

        public void Navigate(string url)
        {
            m_vbaobject.Navigate(url);
        }

        public void Navigate(string url, int retrytime)
        {
            if (retrytime < 0)
                m_vbaobject.Navigate(url);
            else
                m_vbaobject.Navigate(url, retrytime);
        }

        public void NavigateDelay(string url, int delaymilliseconds)
        {
            m_vbaobject.NavigateDelay(url, delaymilliseconds);
        }

        public string GetCurrentTitle()
        {
            return m_vbaobject.GetCurrentTitle();
        }

        #endregion

        #region IVBAObjectHost 成员

        public string Name
        {
            get { return "Html"; }
        }

        public IVBAObject Tenant
        {
            get
            {
                return m_vbaobject;
            }
            set
            {
                m_vbaobject = (VBAHtml)value;
            }
        }

        #endregion
    }
}
