using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public class VBAIEHost : IVBAIE, IVBAObjectHost
    {
        private VBAIE m_vbaobject = null;

        #region IVBAIE 成员        

        public virtual bool IsNavigationError
        {
            get { return m_vbaobject.IsNavigationError; }
        }

        public virtual int ErrorCode
        {
            get { return m_vbaobject.ErrorCode; }
        }

        public virtual int ErrorCode_NET_NOT_FOUND
        {
            get { return m_vbaobject.ErrorCode_NET_NOT_FOUND; }
        }

        public bool ClearSessionCookies()
        {
            return m_vbaobject.ClearSessionCookies();
        }

        public string CurrentCookies
        {
            get
            {
                return m_vbaobject.CurrentCookies;
            }
            set
            {
                m_vbaobject.CurrentCookies = value;
            }
        }

        public bool DownloadActiveX
        {
            get
            {
                return m_vbaobject.DownloadActiveX;
            }
            set
            {
                m_vbaobject.DownloadActiveX = value;
            }
        }

        public bool DownloadFrames
        {
            get
            {
                return m_vbaobject.DownloadFrames;
            }
            set
            {
                m_vbaobject.DownloadFrames = value;
            }
        }

        public bool DownloadImages
        {
            get
            {
                return m_vbaobject.DownloadImages;
            }
            set
            {
                m_vbaobject.DownloadImages = value;
            }
        }

        public bool DownloadJava
        {
            get
            {
                return m_vbaobject.DownloadJava;
            }
            set
            {
                m_vbaobject.DownloadJava = value;
            }
        }

        public bool DownloadScripts
        {
            get
            {
                return m_vbaobject.DownloadScripts;
            }
            set
            {
                m_vbaobject.DownloadScripts = value;
            }
        }

        public bool DownloadSounds
        {
            get
            {
                return m_vbaobject.DownloadSounds;
            }
            set
            {
                m_vbaobject.DownloadSounds = value;
            }
        }

        public bool DownloadVideo
        {
            get
            {
                return m_vbaobject.DownloadVideo;
            }
            set
            {
                m_vbaobject.DownloadVideo = value;
            }
        }

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

        public bool NodeMoveFirstChild()
        {
            return m_vbaobject.NodeMoveFirstChild();
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

        public bool NodeMoveParent()
        {
            return m_vbaobject.NodeMoveParent();
        }

        public string NodeOuterHtml
        {
            get
            {
                return m_vbaobject.NodeOuterHtml;
            }
            set
            {
                m_vbaobject.NodeOuterHtml = value;
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

        public bool NodeRemoveHidden()
        {
            return m_vbaobject.NodeRemoveHidden();
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

        public string GetCacheCookie(string domain, string cookiename)
        {
            return m_vbaobject.GetCacheCookie(domain, cookiename);
        }

        public string GetCurrentURL()
        {
            return m_vbaobject.GetCurrentURL();
        }

        public string GetCurrentWebReferer()
        {
            return m_vbaobject.GetCurrentWebReferer();
        }

        public void Navigate(string url)
        {
            m_vbaobject.Navigate(url);
        }

        public void Navigate(string url, int retrytime)
        {
            if (retrytime <0)
                m_vbaobject.Navigate(url);
            else
                m_vbaobject.Navigate(url, retrytime);
        }

        public void NavigateDelay(string url, int delaymilliseconds)
        {
            m_vbaobject.NavigateDelay(url, delaymilliseconds);
        }

        public bool PerformClickButton(string btnnameorid)
        {
            return m_vbaobject.PerformClickButton(btnnameorid);
        }

        public bool PerformClickLink(string linknameorid)
        {
            return m_vbaobject.PerformClickLink(linknameorid);
        }

        public bool PerformEnterData(string inputnameorid, string strValue)
        {
            return m_vbaobject.PerformEnterData(inputnameorid, strValue);
        }

        public bool PerformEnterDataTextArea(string inputnameorid, string strValue)
        {
            return m_vbaobject.PerformEnterDataTextArea(inputnameorid, strValue);
        }

        public bool PerformSelectRadio(string radionameorid)
        {
            return m_vbaobject.PerformSelectRadio(radionameorid);
        }

        public void RemoveAllHiddenElements()
        {
            m_vbaobject.RemoveAllHiddenElements();
        }

        public bool RunActiveX
        {
            get
            {
                return m_vbaobject.RunActiveX;
            }
            set
            {
                m_vbaobject.RunActiveX = value;
            }
        }

        public bool SetCacheCookie(string domain, string cookiename, string cookievalue)
        {
            return m_vbaobject.SetCacheCookie(domain, cookiename, cookievalue);
        }

        public bool Silent
        {
            get
            {
                return m_vbaobject.Silent;
            }
            set
            {
                m_vbaobject.Silent = value;
            }
        }

        public void SimulateKeyStroke(System.Windows.Forms.Keys keycode)
        {
            m_vbaobject.SimulateKeyStroke(keycode);
        }

        public string GetCurrentTitle()
        {
           return m_vbaobject.GetCurrentTitle();
        }

        #endregion

        #region IVBAObjectHost 成员

        public string Name
        {
            get
            {
                return "IE";
            }
        }

        public IVBAObject Tenant
        {
            get
            {
                return m_vbaobject;
            }
            set
            {
                m_vbaobject = (VBAIE)value;
            }
        }

        #endregion

        #region IVBAIE 成员

        public bool NodeClick()
        {
            return m_vbaobject.NodeClick();
        }

        public bool Redirection
        {
            get
            {
                return m_vbaobject.Redirection;
            }
            set
            {
                m_vbaobject.Redirection = value;
            }
        }

        public Stack<string> RedirectUrl
        {
            get { return m_vbaobject.RedirectUrl; }
        }

        #endregion


        #region IVBAIE 成员


        public int AJAXDELAY
        {
            get
            {
                return m_vbaobject.AJAXDELAY;
            }
            set
            {
                m_vbaobject.AJAXDELAY = value;
            }
        }

        public int AJAXTIMEOUT
        {
            get
            {
                return m_vbaobject.AJAXTIMEOUT;
            }
            set
            {
                m_vbaobject.AJAXTIMEOUT = value;
            }
        }

        public int ErrorCode_TIMEOUT
        {
            get { return m_vbaobject.ErrorCode_TIMEOUT; }
        }

        public int NAVIGATIONTIMEOUT
        {
            get
            {
                return m_vbaobject.NAVIGATIONTIMEOUT;
            }
            set
            {
                m_vbaobject.NAVIGATIONTIMEOUT = value;
            }
        }

        #endregion
    }
}
