using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public class VBAIEStop : VBAIE
    {
        #region IVBAIE 成员

        public override bool IsNavigationError
        {
            get { throw new Exception("VBAEngine Stopping Exception!"); }
        }

        public override int ErrorCode
        {
            get { throw new Exception("VBAEngine Stopping Exception!"); }
        }

        public override int ErrorCode_NET_NOT_FOUND
        {
            get { throw new Exception("VBAEngine Stopping Exception!"); }
        }

        public override bool ClearSessionCookies()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string CurrentCookies
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

        public override bool DownloadActiveX
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

        public override bool DownloadFrames
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

        public override bool DownloadImages
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

        public override bool DownloadJava
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

        public override bool DownloadScripts
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

        public override bool DownloadSounds
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

        public override bool DownloadVideo
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

        public override bool Redirection
        {
            get { throw new Exception("VBAEngine Stopping Exception!"); }
            set { throw new Exception("VBAEngine Stopping Exception!"); }
        }

        public override Stack<string> RedirectUrl
        {
            get { throw new Exception("VBAEngine Stopping Exception!"); }
        } 

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

        public override string NodeGetAttribute(string attribute)
        {
            throw new Exception("VBAEngine Stopping Exception!");
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

        public override bool NodeMoveFirstChild()
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

        public override bool NodeMoveParent()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string NodeOuterHtml
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

        public override bool NodeRemoveHidden()
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

        public override string GetCacheCookie(string domain, string cookiename)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string GetCurrentURL()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string GetCurrentWebReferer()
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

        public override bool PerformClickButton(string btnnameorid)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool PerformClickLink(string linknameorid)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool PerformEnterData(string inputnameorid, string strValue)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool PerformEnterDataTextArea(string inputnameorid, string strValue)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool PerformSelectRadio(string radionameorid)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void RemoveAllHiddenElements()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool NodeClick()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }        

        public override bool RunActiveX
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

        public override bool SetCacheCookie(string domain, string cookiename, string cookievalue)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override bool Silent
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

        public override void SimulateKeyStroke(System.Windows.Forms.Keys keycode)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string GetCurrentTitle()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        /// <summary>
        /// VBA属性:Navigation超时设置(秒)
        /// </summary>  
        public override int NAVIGATIONTIMEOUT
        {
            get { throw new Exception("VBAEngine Stopping Exception!"); }
            set { throw new Exception("VBAEngine Stopping Exception!"); }
        }

        /// <summary>
        /// VBA属性:AJAX超时设置(秒)
        /// </summary>  
        public override int AJAXTIMEOUT
        {
            get { throw new Exception("VBAEngine Stopping Exception!"); }
            set { throw new Exception("VBAEngine Stopping Exception!"); }
        }

        /// <summary>
        /// VBA属性:AJAX检测延时(秒)
        /// </summary>  
        public override int AJAXDELAY
        {
            get { throw new Exception("VBAEngine Stopping Exception!"); }
            set { throw new Exception("VBAEngine Stopping Exception!"); }
        }

        /// <summary>
        /// VBA属性:IE错误代码_连接超时
        /// </summary>  
        public override int ErrorCode_TIMEOUT
        {
            get { throw new Exception("VBAEngine Stopping Exception!"); }
        }

        #endregion

        #region IVBAObject 成员

        public override void Reset()
        {

        }

        #endregion
    }
}
