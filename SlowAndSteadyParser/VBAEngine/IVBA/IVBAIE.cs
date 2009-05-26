using System;
namespace SlowAndSteadyParser
{
    interface IVBAIE
    {
        bool IsNavigationError { get;}
        int ErrorCode { get;}
        int ErrorCode_NET_NOT_FOUND { get;}
        bool ClearSessionCookies();
        string CurrentCookies { get; set; }
        bool DownloadActiveX { get; set; }
        bool DownloadFrames { get; set; }
        bool DownloadImages { get; set; }
        bool DownloadJava { get; set; }
        bool DownloadScripts { get; set; }
        bool DownloadSounds { get; set; }
        bool DownloadVideo { get; set; }
        bool Redirection { get; set; }
        System.Collections.Generic.Stack<string> RedirectUrl { get;}
        string NodeClassName { get; set; }
        string NodeHref { get; }
        string NodeId { get; set; }
        string NodeInnerHtml { get; set; }
        string NodeInnerText { get; set; }
        bool NodeIsNull();
        bool NodeMoveFirstChild();
        bool NodeMoveNext();
        bool NodeMoveNext(int time);
        bool NodeMoveNextCondition(NodeSeekConditions cs);
        bool NodeMoveParent();
        string NodeOuterHtml { get; set; }
        string NodeOuterText { get; set; }
        bool NodePopRange();
        bool NodePushRangeAll();
        bool NodePushRangeById(string id);
        bool NodePushRangeByName(string elementname);
        bool NodePushRangeByTagName(string tagname);
        bool NodePushRangeChildren();
        bool NodePushRangeImages();
        bool NodePushRangeLinks();
        bool NodePushRangeScripts();
        bool NodeRemoveHidden();
        string NodeTagName { get; }
        NodeSeekConditions NodeSeekClassName(string classname);
        NodeSeekConditions NodeSeekId(string id);
        NodeSeekConditions NodeSeekInnerHTML(string innerHTML, bool exactmatch);
        NodeSeekConditions NodeSeekInnerText(string innerText, bool exactmatch);
        NodeSeekConditions NodeSeekTagName(string tagname);
        string GetCacheCookie(string domain, string cookiename);
        string GetCurrentURL();
        string GetCurrentWebReferer();
        void Navigate(string url);
        void Navigate(string url, int retrytime);
        void NavigateDelay(string url, int delaymilliseconds);
        bool PerformClickButton(string btnnameorid);
        bool PerformClickLink(string linknameorid);
        bool PerformEnterData(string inputnameorid, string strValue);
        bool PerformEnterDataTextArea(string inputnameorid, string strValue);
        bool PerformSelectRadio(string radionameorid);
        void RemoveAllHiddenElements();
        bool RunActiveX { get; set; }
        bool SetCacheCookie(string domain, string cookiename, string cookievalue);
        bool Silent { get; set; }
        void SimulateKeyStroke(System.Windows.Forms.Keys keycode);
        string GetCurrentTitle();
        string NodeGetAttribute(string attribute);
        bool NodeClick();
        int AJAXDELAY { get; set; }
        int AJAXTIMEOUT { get; set; }
        int ErrorCode_TIMEOUT { get; }
        int NAVIGATIONTIMEOUT { get; set; }
    }
}
