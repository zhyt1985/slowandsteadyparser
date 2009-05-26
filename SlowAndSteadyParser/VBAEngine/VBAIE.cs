using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using csExWB;
using IfacesEnumsStructsClasses;
using System.Collections;


namespace SlowAndSteadyParser
{
    [Serializable]
    public class VBAIE : IVBAObject, IVBAIE
    {
        #region 模块内部属性和委托*******************************************************

        //log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Balancer balancer = PerformanceBalancer.GetBalancer(System.Reflection.Assembly.GetEntryAssembly().GetName().Name);

        //超时限制20秒
        private int NAVIGATINGOUTOFSECOND = 30;
        private double AJAXOUTOFSECOND = 20;
        private double AJAXDELAYSECOND = 5;

        //浏览器指针
        protected csExWB.cEXWB pWB = null;
        private object navigatelocker = new object();
        private object ajaxlocker = new object();

        //当前HTMLElement对象
        protected IHTMLElement pNode = null;

        //当前HTMLElementCollection的枚举对象
        protected IEnumerator pNodesEnum = null;

        //当前HTMLelementCollection对象
        //protected IHTMLElementCollection pNodes= null;
        protected List<IHTMLElement> pNodeList = null;

        //用来保存游标各种状态的栈
        protected Stack m_NodeStack = new Stack();

        //浏览器WBDOCDOWNLOADCTLFLAG对应内部属性
        protected DOCDOWNLOADCTLFLAG m_DLCtlFlags = DOCDOWNLOADCTLFLAG.DLIMAGES | DOCDOWNLOADCTLFLAG.BGSOUNDS | DOCDOWNLOADCTLFLAG.VIDEOS;
        private bool m_IsNavigationCompleted = false;
        private bool m_IsNavigationError = false;
        private WinInetErrors m_IEErrorCode;
        //private NAVIGATESTATUS m_NaviStatus = NAVIGATESTATUS.Ready;
        private bool m_IsRedirctionEnable = false;
        private bool m_IsFlashEnable = false;
        private static Guid ms_guidFlash = new Guid("D27CDB6E-AE6D-11cf-96B8-444553540000");
        private Stack<string> m_RedirectUrl = new Stack<string>();

        //内部时钟
        private DateTime m_NavigatingStartTimepoint;  //开始连接的时间点
        private DateTime m_AjaxStartTimepoint;  //开始Ajax的时间点
        private DateTime m_ScriptStartTimepoint;   //最后一次脚本运行的时间点

        //委托群
        protected delegate void CsEXWBNaviInvoker(string url);

        #endregion

        #region 模块内部方法 **************************************************************

        protected csExWB.cEXWB GetWebBrowser()
        {
            if (pWB == null)
            {
                pWB = CWBPool.RentCWB();
                RegiserHandler();                
                return pWB;
            }
            else
                return pWB;
        }

        protected void Navi(string url)
        {
            if (!GetWebBrowser().InvokeRequired)
            {
                pWB.WBDOCDOWNLOADCTLFLAG = (int)m_DLCtlFlags;
                pWB.Navigate(url);
            }

            else
            {
                //清空重定向URL容器
                m_RedirectUrl.Clear();
                //设置连接状态
                m_IsNavigationCompleted = false;
                m_IsNavigationError = false;
                m_IEErrorCode = 0;
                //设置时间戳
                m_NavigatingStartTimepoint = DateTime.Now;

                pWB.Invoke(new CsEXWBNaviInvoker(Navi), new object[] { url });
                lock (navigatelocker)
                {
                    Monitor.Wait(navigatelocker, NAVIGATINGOUTOFSECOND * 1000);
                }

                Thread.Sleep(200);
            }

        }

        private void AjaxWatch()
        {
            //设置时间戳
            m_ScriptStartTimepoint = DateTime.Now;
            m_AjaxStartTimepoint = DateTime.Now;
            log.Debug("AJAX Detection Start...");

            while (m_ScriptStartTimepoint.AddSeconds(AJAXDELAYSECOND) > DateTime.Now)
            {
                System.Threading.Thread.Sleep(300);

                if (m_AjaxStartTimepoint.AddSeconds(AJAXOUTOFSECOND) < DateTime.Now)
                {
                    if (IsWebFilled())
                    {
                        log.Debug("Navigation Done(Out of Time Through) In AJAX");                        
                    }
                    else
                    {
                        log.Debug("Navigation Out Of Time In AJAX");
                        m_IsNavigationError = true;
                        m_IEErrorCode = WinInetErrors.ERROR_INTERNET_TIMEOUT;
                    }
                    break;
                }

                if (m_IsRedirctionEnable && m_RedirectUrl.Count > 0)
                {
                    log.Debug("Navigation Stopped In AJAX Because Of Redirection");
                    break;
                }
            }
            log.Debug("AJAX Detection End...");
        }

        private bool IsWebFilled()
        {
            try
            {
                string src = GetWebBrowser().DocumentTitle;
                if (src != null && src != "")
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        //只加入当前层元素
        private List<IHTMLElement> SingleLayerTransefer(IHTMLElementCollection ec)
        {
            List<IHTMLElement> elist = new List<IHTMLElement>();
            foreach (IHTMLElement e in ec)
                elist.Add(e);            
            return elist;
        }

        //加入所有元素(包括子对象)
        private List<IHTMLElement> AllLayerTransefer(IHTMLElementCollection ec)
        {
            List<IHTMLElement> elist = new List<IHTMLElement>();
            AllLayerTransferRecursion(ref elist, ec);
            return elist;
        }

        private void AllLayerTransferRecursion(ref List<IHTMLElement> elist, IHTMLElementCollection ec)
        {            
            foreach (IHTMLElement e in ec)
            {
                elist.Add(e);
                if (ec != null && ec.length > 0)
                    AllLayerTransferRecursion(ref elist, (IHTMLElementCollection)e.children);
            }
        }

        //private void FilterHiddenText(IHTMLElement element, ref string srcStr)
        //{
        //    IHTMLCurrentStyle cstyle = (IHTMLCurrentStyle)((IHTMLElement2)element).currentStyle;
        //    if (
        //            null != element &&
        //            null != element.innerText &&
        //            null != element.style &&
        //            (
        //                (null != cstyle.visibility && cstyle.visibility.Equals("hidden")) ||
        //                (null != cstyle.display && cstyle.display.Equals("none"))
        //            )
        //        )
        //    {
        //        int pos = srcStr.IndexOf(element.innerText);
        //        srcStr = srcStr.Remove(pos, element.innerText.Length);
        //        return;
        //    }

        //    IHTMLElementCollection collection = (IHTMLElementCollection)element.children;
        //    if (collection.length != 0)
        //    {
        //        foreach (IHTMLElement elem in collection)
        //        {
        //            FilterHiddenText(elem, ref srcStr);
        //        }
        //    }
        //}

        private void SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG flag, bool add)
        {
            if (add)
            {
                if ((m_DLCtlFlags & flag) == 0)
                    m_DLCtlFlags |= flag;
            }
            else
            {
                if ((m_DLCtlFlags & flag) != 0)
                    m_DLCtlFlags -= flag;
            }

        }

        private void VBACsEXWB_BeforeNavigate2(object sender, BeforeNavigate2EventArgs e)
        {
            if (e.url.ToLower() != "about:blank")
                log.Debug("BeforeNavigate2==>" + e.url + " =istoplevel===> " + e.istoplevel.ToString());
        }

        private void VBACsEXWB_DocumentComplete(object sender, DocumentCompleteEventArgs e)
        {
            if (e.url.ToLower() == "about:blank")
            {
                m_IsNavigationCompleted = true;

                //发出提示
                lock (navigatelocker)
                {
                    Monitor.Pulse(navigatelocker);
                }
                return;
            }

            if (e.istoplevel)
            {
                log.Debug("DocumentComplete::TopLevel is TRUE===>" + e.url);
                m_IsNavigationCompleted = true;
                //发出提示
                lock (navigatelocker)
                {
                    Monitor.Pulse(navigatelocker);
                }
            }
            else
            {
                log.Debug("DocumentComplete::TopLevel is FALSE===>" + e.url);
            }
        }

        private void VBACsEXWB_WBSecurityProblem(object sender, SecurityProblemEventArgs e)
        {
            //处理一般安全提示
            if ((e.problem == WinInetErrors.HTTP_REDIRECT_NEEDS_CONFIRMATION) ||
                (e.problem == WinInetErrors.ERROR_INTERNET_HTTP_TO_HTTPS_ON_REDIR) ||
                (e.problem == WinInetErrors.ERROR_INTERNET_HTTPS_HTTP_SUBMIT_REDIR) ||
                (e.problem == WinInetErrors.ERROR_INTERNET_HTTPS_TO_HTTP_ON_REDIR) ||
                (e.problem == WinInetErrors.ERROR_INTERNET_MIXED_SECURITY))
            {
                e.handled = true;
                e.retvalue = Hresults.S_FALSE;
                return;
            }

            //非正常安全提示
            log.Debug("WBSecurityProblem==>" + e.problem.ToString());
            m_IsNavigationError = true;
            m_IEErrorCode = e.problem;
            //lock (navigatelocker)
            //{
            //    Monitor.Pulse(navigatelocker);
            //}
        }

        private void VBACsEXWB_NavigateError(object sender, NavigateErrorEventArgs e)
        {
            m_IEErrorCode = e.statuscode;
            m_IsNavigationError = true;
            //lock (navigatelocker)
            //{
            //    Monitor.Pulse(navigatelocker);
            //}
        }

        private void VBACsEXWB_WBDocHostShowUIShowMessage(object sender, csExWB.DocHostShowUIShowMessageEventArgs e)
        {           
            //To stop messageboxs
            e.handled = true;
            //Default
            e.result = (int)DialogResult.Cancel;
        }

        private void VBACsEXWB_ProcessUrlAction(object sender, ProcessUrlActionEventArgs e)
        {
            if (e.urlAction == URLACTION.SCRIPT_RUN)
            {
                m_ScriptStartTimepoint = DateTime.Now;
            }
            else if (e.urlAction == URLACTION.ACTIVEX_RUN)
            {
                if (e.context == ms_guidFlash && (!m_IsFlashEnable))
                {
                    e.handled = true;
                    e.urlPolicy = URLPOLICY.DISALLOW;
                }
            }
        }

        private void VBACsEXWB_WBEvaluteNewWindow(object sender, EvaluateNewWindowEventArgs e)
        {
            m_RedirectUrl.Push(e.url);
        }

        private void RegiserHandler()
        {
            if (pWB != null)
            {
                //注册事件处理方法
                pWB.DocumentComplete += new DocumentCompleteEventHandler(VBACsEXWB_DocumentComplete);
                pWB.NavigateError += new NavigateErrorEventHandler(VBACsEXWB_NavigateError);
                pWB.WBSecurityProblem += new SecurityProblemEventHandler(VBACsEXWB_WBSecurityProblem);
                pWB.BeforeNavigate2 += new BeforeNavigate2EventHandler(VBACsEXWB_BeforeNavigate2);
                pWB.WBDocHostShowUIShowMessage += new DocHostShowUIShowMessageEventHandler(VBACsEXWB_WBDocHostShowUIShowMessage);
                pWB.ProcessUrlAction += new ProcessUrlActionEventHandler(VBACsEXWB_ProcessUrlAction);
                pWB.WBEvaluteNewWindow += new EvaluateNewWindowEventHandler(VBACsEXWB_WBEvaluteNewWindow);
            }
        }

        private void UnregiserHandler()
        {
            if (pWB != null)
            {
                //注销事件处理方法
                pWB.DocumentComplete -= new DocumentCompleteEventHandler(VBACsEXWB_DocumentComplete);
                pWB.NavigateError -= new NavigateErrorEventHandler(VBACsEXWB_NavigateError);
                pWB.WBSecurityProblem -= new SecurityProblemEventHandler(VBACsEXWB_WBSecurityProblem);
                pWB.BeforeNavigate2 -= new BeforeNavigate2EventHandler(VBACsEXWB_BeforeNavigate2);
                pWB.WBDocHostShowUIShowMessage -= new DocHostShowUIShowMessageEventHandler(VBACsEXWB_WBDocHostShowUIShowMessage);
                pWB.ProcessUrlAction -= new ProcessUrlActionEventHandler(VBACsEXWB_ProcessUrlAction);
                pWB.WBEvaluteNewWindow -= new EvaluateNewWindowEventHandler(VBACsEXWB_WBEvaluteNewWindow);
            }
        }

        #endregion

        #region 模块公共属性和方法(供VBA调用)***********************************************

        //************ Navigation ***************

        /// <summary>
        /// VBA属性:是否出错
        /// </summary>  
        public virtual bool IsNavigationError
        {
            get { return m_IsNavigationError; }
        }

        /// <summary>
        /// VBA属性:IE错误代码
        /// </summary>  
        public virtual int ErrorCode
        {
            get { return (int)m_IEErrorCode; }
        }

        /// <summary>
        /// VBA属性:Navigation超时设置(秒)
        /// </summary>  
        public virtual int NAVIGATIONTIMEOUT
        {
            get { return NAVIGATINGOUTOFSECOND; }
            set { NAVIGATINGOUTOFSECOND = value; }
        }

        /// <summary>
        /// VBA属性:AJAX超时设置(秒)
        /// </summary>  
        public virtual int AJAXTIMEOUT
        {
            get { return (int)AJAXOUTOFSECOND; }
            set { AJAXOUTOFSECOND = value; }
        }

        /// <summary>
        /// VBA属性:AJAX检测延时(秒)
        /// </summary>  
        public virtual int AJAXDELAY
        {
            get { return (int)AJAXDELAYSECOND; }
            set { AJAXDELAYSECOND = value; }
        }

        /// <summary>
        /// VBA属性:IE错误代码_没有网络
        /// </summary>  
        public virtual int ErrorCode_NET_NOT_FOUND
        {
            get { return (int)WinInetErrors.INET_E_RESOURCE_NOT_FOUND; }
        }

        /// <summary>
        /// VBA属性:IE错误代码_连接超时
        /// </summary>  
        public virtual int ErrorCode_TIMEOUT
        {
            get { return (int)WinInetErrors.ERROR_INTERNET_TIMEOUT; }
        }

        /// <summary>
        /// VBA方法:转到目标网址
        /// </summary>  
        public virtual void Navigate(string url)
        {
            bool isredirect;

            GetWebBrowser().NavToBlank();

            //向联网控制中心注册
            CWBPool.NavigationBegin();

            do
            {
                isredirect = false;

                Navi(url);

                //判断是否含有AJAX
                if (DownloadScripts && m_IsNavigationCompleted)
                    AjaxWatch();

                //判断是否有重定向
                isredirect = (m_IsRedirctionEnable && m_RedirectUrl.Count > 0);

                if (isredirect)
                {
                    log.Debug("Navigation-Redirection:" + url + " >>>> " + m_RedirectUrl.Peek());
                    url = m_RedirectUrl.Pop();
                }

            } while (isredirect);

            if (!m_IsNavigationCompleted && !IsWebFilled())
            {
                log.Debug("Navigation Out Of Time");
                m_IsNavigationError = true;
                m_IEErrorCode = WinInetErrors.ERROR_INTERNET_TIMEOUT;
            }

            //向联网控制中心注销
            CWBPool.NavigationEnd();
        }

        /// <summary>
        /// VBA方法:转到目标网址, 重试最多n次
        /// </summary>  
        public virtual void Navigate(string url, int retrytime)
        {
            int i = 0;
            if (url.Length > 0)
            {
                while (i < retrytime)
                {
                    Navigate(url);

                    if (m_IsNavigationCompleted)
                    {
                        log.Debug("Navigation Complete");
                        return;
                    }
                    else
                    {
                        i++;
                        log.Debug("Navigation Error, Retry "+i.ToString());
                        System.Threading.Thread.Sleep(4000);
                    }
                }
                log.Debug("Too Much Error...Navigation Abandon");
            }
        }

        /// <summary>
        /// VBA方法:转到目标网址(带延迟器,防止链接同一站点过快被禁IP)
        /// </summary>  
        public virtual void NavigateDelay(string url, int delaymilliseconds)
        {
            if (url!=null && url.Length>0)
            {
                CWBPool.WebBrowserNavigationDelayer(url, delaymilliseconds);
                Navigate(url);                
            }
        }
        
        //************ Download/Run Paramater ***************

        /// <summary>
        /// VBA属性:允许下载图片
        /// </summary>
        public virtual bool DownloadImages
        {
            get { return ((m_DLCtlFlags & DOCDOWNLOADCTLFLAG.DLIMAGES) != 0); }
            set { SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG.DLIMAGES, value); }
        }

        /// <summary>
        /// VBA属性:允许下载声音
        /// </summary>
        public virtual bool DownloadSounds
        {
            get { return ((m_DLCtlFlags & DOCDOWNLOADCTLFLAG.BGSOUNDS) != 0); }
            set { SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG.BGSOUNDS, value); }
        }

        /// <summary>
        /// VBA属性:允许下载影片(不包括Flash)
        /// </summary>
        public virtual bool DownloadVideo
        {
            get { return ((m_DLCtlFlags & DOCDOWNLOADCTLFLAG.VIDEOS) != 0); }
            set { SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG.VIDEOS, value); }
        }

        /// <summary>
        /// VBA属性:允许运行ActiveX组件
        /// </summary>
        public virtual bool RunActiveX
        {
            get { return ((m_DLCtlFlags & DOCDOWNLOADCTLFLAG.NO_RUNACTIVEXCTLS) == 0); }
            set
            {
                if (value)
                    SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG.NO_RUNACTIVEXCTLS, false);
                else
                    SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG.NO_RUNACTIVEXCTLS, true);
            }
        }

        /// <summary>
        /// VBA属性:允许下载ActiveX组件
        /// </summary>
        public virtual bool DownloadActiveX
        {
            get { return ((m_DLCtlFlags & DOCDOWNLOADCTLFLAG.NO_DLACTIVEXCTLS) == 0); }
            set
            {
                if (value)
                    SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG.NO_DLACTIVEXCTLS, false);
                else
                    SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG.NO_DLACTIVEXCTLS, true);
            }
        }

        /// <summary>
        /// VBA属性:允许下载JAVA应用程序(不是JS脚本)
        /// </summary>
        public virtual bool DownloadJava
        {
            get { return ((m_DLCtlFlags & DOCDOWNLOADCTLFLAG.NO_JAVA) == 0); }
            set
            {
                if (value)
                    SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG.NO_JAVA, false);
                else
                    SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG.NO_JAVA, true);
            }
        }

        /// <summary>
        /// VBA属性:允许下载框架
        /// </summary>
        public virtual bool DownloadFrames
        {
            get { return ((m_DLCtlFlags & DOCDOWNLOADCTLFLAG.NO_FRAMEDOWNLOAD) == 0); }
            set
            {
                if (value)
                    SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG.NO_FRAMEDOWNLOAD, false);
                else
                    SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG.NO_FRAMEDOWNLOAD, true);
            }
        }

        /// <summary>
        /// VBA属性:允许下载脚本
        /// </summary>
        public virtual bool DownloadScripts
        {
            get { return ((m_DLCtlFlags & DOCDOWNLOADCTLFLAG.NO_SCRIPTS) == 0); }
            set
            {
                if (value)
                    SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG.NO_SCRIPTS, false);
                else
                    SynchDOCDOWNLOADCTLFLAG(DOCDOWNLOADCTLFLAG.NO_SCRIPTS, true);
            }
        }

        /// <summary>
        /// VBA属性:允许网页重定向
        /// </summary>
        public virtual bool Redirection
        {
            get { return m_IsRedirctionEnable; }
            set { m_IsRedirctionEnable = value; }
        }

        /// <summary>
        /// VBA属性:网页重定向URL栈
        /// </summary>
        public virtual Stack<string> RedirectUrl
        {
            get { return m_RedirectUrl; }
        } 

        /// <summary>
        /// VBA属性:不允许弹出网页级对话框
        /// </summary>
        public virtual bool Silent
        {
            get { return GetWebBrowser().Silent; }
            set { GetWebBrowser().Silent = value; }
        }

        //**************** Base Infomation *****************

        /// <summary>
        /// VBA属性:得到当前页面的Title
        /// </summary>
        public virtual String GetCurrentTitle()
        {
            return GetWebBrowser().GetActiveDocument().title;
        }

        //**************** Cookies *****************

        /// <summary>
        /// VBA属性:得到当前站点的Cookies
        /// </summary>
        public virtual String CurrentCookies
        {
            get{return GetWebBrowser().GetActiveDocument().cookie;}
            set{GetWebBrowser().GetActiveDocument().cookie = value;}
        }

        /// <summary>
        /// VBA方法:设置缓存中的Cookie,cookiename为空表示一次传递所有cookies
        /// </summary> 
        public virtual bool SetCacheCookie(string domain, string cookiename, string cookievalue)
        {return Wininet.InternetSetCookieEX(domain,cookiename,cookievalue);}

        /// <summary>
        /// VBA方法:取得缓存中的Cookie,cookiename为空表示一次传递所有cookies
        /// </summary> 
        public virtual string GetCacheCookie(string domain, string cookiename)
        {
            string s = "";
            if (Wininet.InternetGetCookieEX(domain, cookiename,ref s))
                return s;
            else
                return null;            
        }

        /// <summary>
        /// VBA方法:清除本次Sessions缓存中的Cookies
        /// </summary> 
        public virtual bool ClearSessionCookies()
        {
            return GetWebBrowser().ClearSessionCookies();
        }

        ///// <summary>
        ///// VBA方法:自定义引用网页地址，会覆盖默认设置(设置为空则阻止覆盖)
        ///// </summary> 
        //public virtual void SetCustomReferer(string referer)
        //{
        //    customreferer = referer;
        //}

        /// <summary>
        /// VBA方法:得到当前网页的引用地址
        /// </summary> 
        public virtual string GetCurrentWebReferer()
        {
            return GetWebBrowser().GetActiveDocument().referrer;
        }

        //********** Pretreatment ***********

        public virtual void RemoveAllHiddenElements()
        {
            IHTMLElementCollection ec = (IHTMLElementCollection)GetWebBrowser().GetActiveDocument().body.all;
            IHTMLCurrentStyle cstyle = null;
            foreach (IHTMLElement e in ec)
            {
                cstyle = (IHTMLCurrentStyle)((IHTMLElement2)e).currentStyle;
                if (cstyle != null &&
                        (cstyle.display.ToLower() == "none" || cstyle.visibility.ToLower() == "hidden")
                    )

                    e.outerHTML = null;
                Application.DoEvents();                
            }
            balancer.InvokeBalancePoint();
            log.Debug("All Hidden Element Being Removed!");
        }

        public virtual string GetCurrentURL()
        {
            return GetWebBrowser().GetActiveDocument().url;
        }

        //********** Seek Conditions ***********    
        public virtual NodeSeekConditions NodeSeekTagName(string tagname)
        {
            return new NodeSeekConditions(NodeSeekConditonType.TagName, tagname);
        }

        public virtual NodeSeekConditions NodeSeekClassName(string classname)
        {
            return new NodeSeekConditions(NodeSeekConditonType.ClassName , classname);
        }

        public virtual NodeSeekConditions NodeSeekId(string id)
        {
            return new NodeSeekConditions(NodeSeekConditonType.Id , id);
        }

        public virtual NodeSeekConditions NodeSeekInnerText(string innerText, bool exactmatch)
        {
            if (exactmatch)
                return new NodeSeekConditions(NodeSeekConditonType.InnerTextExactMatch , innerText);
            else
                return new NodeSeekConditions(NodeSeekConditonType.InnerTextPartialMatch , innerText);
        }

        public virtual NodeSeekConditions NodeSeekInnerHTML(string innerHTML, bool exactmatch)
        {
            if (exactmatch)
                return new NodeSeekConditions(NodeSeekConditonType.InnerHTMLExactMatch , innerHTML);
            else
                return new NodeSeekConditions(NodeSeekConditonType.InnerHTMLPartialMatch , innerHTML);
        }

        //********** Node Range Push/Pop ***********

        //范围 = 页面所有元素
        public virtual bool NodePushRangeAll()
        {
            List<IHTMLElement> elist = SingleLayerTransefer((IHTMLElementCollection)GetWebBrowser().GetActiveDocument().all);
            if (elist.Count > 0)
            {
                //将当前Element状态压入栈
                m_NodeStack.Push(pNodeList);
                m_NodeStack.Push(pNode);
                m_NodeStack.Push(pNodesEnum);

                pNodeList = elist;
                pNodesEnum = pNodeList.GetEnumerator();
                pNode = null;
                return true;
            }
            else
                return false;
        }

        //范围 = 页面内所有Link元素
        public virtual bool NodePushRangeLinks()
        {
            List<IHTMLElement> elist = SingleLayerTransefer((IHTMLElementCollection)GetWebBrowser().GetActiveDocument().links);
            if (elist.Count > 0)
            {
                //将当前Element状态压入栈
                m_NodeStack.Push(pNodeList);
                m_NodeStack.Push(pNode);
                m_NodeStack.Push(pNodesEnum);

                pNodeList = elist;
                pNodesEnum = pNodeList.GetEnumerator();
                pNode = null;
                return true;
            }
            else
                return false;
        }

        //范围 = 页面内所有Image元素
        public virtual bool NodePushRangeImages()
        {
            List<IHTMLElement> elist = SingleLayerTransefer((IHTMLElementCollection)GetWebBrowser().GetActiveDocument().images);
            if (elist.Count > 0)
            {
                //将当前Element状态压入栈
                m_NodeStack.Push(pNodeList);
                m_NodeStack.Push(pNode);
                m_NodeStack.Push(pNodesEnum);

                pNodeList = elist;
                pNodesEnum = pNodeList.GetEnumerator();
                pNode = null;
                return true;
            }
            else
                return false;
        }

        //范围 = 页面内所有Script元素
        public virtual bool NodePushRangeScripts()
        {
            List<IHTMLElement> elist = SingleLayerTransefer((IHTMLElementCollection)GetWebBrowser().GetActiveDocument().scripts);
            if (elist.Count > 0)
            {
                //将当前Element状态压入栈
                m_NodeStack.Push(pNodeList);
                m_NodeStack.Push(pNode);
                m_NodeStack.Push(pNodesEnum);

                pNodeList = elist;
                pNodesEnum = pNodeList.GetEnumerator();
                pNode = null;
                return true;
            }
            else
                return false;
        }

        //范围 = 页面内所有特定TagName元素
        public virtual bool NodePushRangeByTagName(String tagname)
        {
            List<IHTMLElement> elist = SingleLayerTransefer(GetWebBrowser().GetElementsByTagName(true, tagname));
            if (elist.Count > 0)
            {
                //将当前Element状态压入栈
                m_NodeStack.Push(pNodeList);
                m_NodeStack.Push(pNode);
                m_NodeStack.Push(pNodesEnum);

                pNodeList = elist;
                pNodesEnum = pNodeList.GetEnumerator();
                pNode = null;
                return true;
            }
            else
                return false;
        }

        //范围 = 页面内所有特定Name元素
        public virtual bool NodePushRangeByName(String elementname)
        {
            List<IHTMLElement> elist = SingleLayerTransefer(GetWebBrowser().GetElementsByName(true, elementname));
            if (elist.Count > 0)
            {
                //将当前Element状态压入栈
                m_NodeStack.Push(pNodeList);
                m_NodeStack.Push(pNode);
                m_NodeStack.Push(pNodesEnum);

                pNodeList = elist;
                pNodesEnum = pNodeList.GetEnumerator();
                pNode = null;
                return true;
            }
            else
                return false;
        }

        //范围 = 页面内特定Id元素
        public virtual bool NodePushRangeById(String id)
        {
            IHTMLElement e  = GetWebBrowser().GetElementByID(true, id);
            if (e != null)
            {
                //将当前Element状态压入栈
                m_NodeStack.Push(pNodeList);
                m_NodeStack.Push(pNode);
                m_NodeStack.Push(pNodesEnum);

                pNodeList = new List<IHTMLElement>();
                pNodeList.Add(e);
                pNodesEnum = pNodeList.GetEnumerator();
                pNode = null;
                return true;
            }
            else
                return false;
        }

        //范围 = 当前pElement指向的Children元素
        public virtual bool NodePushRangeChildren()
        {
            List<IHTMLElement> elist = AllLayerTransefer((IHTMLElementCollection)pNode.children);
            if (elist.Count > 0)
            {
                //将当前Element状态压入栈
                m_NodeStack.Push(pNodeList);
                m_NodeStack.Push(pNode);
                m_NodeStack.Push(pNodesEnum);

                pNodeList = elist;
                pNodesEnum = pNodeList.GetEnumerator();
                pNode = null;
                return true;
            }
            else
                return false;

        }

        //弹出当前范围
        public virtual bool NodePopRange()
        {
            try
            {
                pNodesEnum = (IEnumerator)m_NodeStack.Pop();
                pNode = (IHTMLElement)m_NodeStack.Pop();
                pNodeList = (List<IHTMLElement>)m_NodeStack.Pop();
                return true;
            }
            catch (Exception e)
            {
                throw e;     
                return false;
            }
        }

        //*********** NodeMove ************

        //游标移动函数
        public virtual bool NodeMoveNext()
        {
            try
            {
                if (pNodesEnum != null && pNodesEnum.MoveNext())
                {
                    pNode = (IHTMLElement)(pNodesEnum.Current);
                    return true;
                }
                else
                {
                    pNode = null;
                    return false;
                }
            }
            catch(Exception e)
            {
                log.Error(e);
                return false;
            }
        }

        public virtual bool NodeMoveNext(int time)
        {
            int i = 1;

            if (pNodesEnum == null)
            {
                pNode = null;
                balancer.InvokeBalancePoint();
                return false;
            }

            while (pNodesEnum.MoveNext())
            {
                //if (pEnum == null)
                //{
                //    pElement = null;
                //    balancer.InvokeBalancePoint();
                //    return false;
                //}

                if (i >= time)
                {
                    pNode = (IHTMLElement)(pNodesEnum.Current);
                    balancer.InvokeBalancePoint();
                    return true;
                }
                i++;
            }
            pNode = null;
            balancer.InvokeBalancePoint();
            return false;
        }

        public virtual bool NodeMoveNextCondition(NodeSeekConditions cs)
        {
            while (NodeMoveNext())
            {
                bool IsSelected = true;
                foreach (NodeSeekCondition c in cs)
                {
                    switch (c.ConditionAttribute)
                    {
                        case NodeSeekConditonType.TagName:
                            IsSelected = IsSelected && !((c.ConditionValue == NodeTagName) ^ c.IsIncluding);
                            break;
                        case NodeSeekConditonType.ClassName:
                            IsSelected = IsSelected && !((c.ConditionValue == NodeClassName) ^ c.IsIncluding);
                            break;
                        case NodeSeekConditonType.Id:
                            IsSelected = IsSelected && !((c.ConditionValue == NodeId) ^ c.IsIncluding);
                            break;
                        case NodeSeekConditonType.InnerTextExactMatch:
                            IsSelected = IsSelected && !((NodeInnerText != null && c.ConditionValue == NodeInnerText) ^ c.IsIncluding);
                            break;
                        case NodeSeekConditonType.InnerTextPartialMatch :
                            IsSelected = IsSelected && !((NodeInnerText != null && NodeInnerText.IndexOf(c.ConditionValue) > -1) ^ c.IsIncluding);
                            break;
                        case NodeSeekConditonType.InnerHTMLExactMatch :
                            IsSelected = IsSelected && !((NodeInnerHtml != null && c.ConditionValue == NodeInnerHtml) ^ c.IsIncluding);
                            break;
                        case NodeSeekConditonType.InnerHTMLPartialMatch :
                            IsSelected = IsSelected && !((NodeInnerHtml != null && NodeInnerHtml.IndexOf(c.ConditionValue) > -1) ^ c.IsIncluding);
                            break;
                        default: break;
                    }
                }
                if (IsSelected)
                {
                    balancer.InvokeBalancePoint();
                    return true;
                }
                Application.DoEvents();
            }
            balancer.InvokeBalancePoint();
            return false;
        }

        public virtual bool NodeMoveParent()
        {
            if (pNode.parentElement != null)
            {
                IHTMLElement p = pNode.parentElement;
                pNodesEnum.Reset();
                while (NodeMoveNext())
                {
                    if ((IHTMLElement)pNodesEnum.Current == p)
                    {
                        pNode = p;
                        return true;
                    }
                    Application.DoEvents();
                }
                return false;
            }
            else
                return false;
        }

        public virtual bool NodeMoveFirstChild()
        {
            if (((IHTMLElementCollection)pNode.children).length > 0)
            {
                IHTMLElement p = (IHTMLElement)((IHTMLElementCollection)pNode.children).item(1, 1);
                pNodesEnum.Reset();
                while (NodeMoveNext())
                {
                    if ((IHTMLElement)pNodesEnum.Current == p)
                    {
                        pNode = p;
                        balancer.InvokeBalancePoint();
                        return true;
                    }
                    Application.DoEvents();
                }
                balancer.InvokeBalancePoint();
                return false;

            }
            else
            {
                balancer.InvokeBalancePoint();
                return false;
            }
        }

        //*********** NodeValue ************

        public virtual bool NodeIsNull()
        {
            return (pNode == null);
        }

        public virtual string NodeInnerText
        {
            get { if (pNode != null) return pNode.innerText; else return null; }
            set { if (pNode != null) pNode.innerText = value; }
        }

        public virtual string NodeInnerHtml
        {
            get { if (pNode != null) return pNode.innerHTML; else return null; }
            set { if (pNode != null) pNode.innerHTML = value; }
        }

        public virtual string NodeOuterText
        {
            get { if (pNode != null) return pNode.outerText; else return null; }
            set { if (pNode != null) pNode.outerText = value; }
        }

        public virtual string NodeOuterHtml
        {
            get { if (pNode != null) return pNode.outerHTML; else return null; }
            set { if (pNode != null) pNode.outerHTML = value; }
        }

        public virtual string NodeClassName
        {
            get
            {
                try
                {
                    if (pNode != null)
                    {
                        object c = pNode.getAttribute("className", 2);
                        if (c!=null)
                            return ((string)c).ToLower();

                    }
                    return null;
                }
                catch (NullReferenceException e)
                {
                    //捕捉Null引用错误 以便处理那些不带有className的Element
                    return null;
                }
                catch (Exception ee)
                {
                    throw (ee);
                }
            }
            set { if (pNode != null) pNode.className = value; }
        }

        public virtual string NodeGetAttribute(string attribute)
        {
            try
            {
                if (pNode != null && attribute != null && attribute != "")
                {
                    object c = pNode.getAttribute(attribute, 2);
                    if (c != null)
                        return ((string)c).ToLower();
                }
                return null;
            }
            catch (NullReferenceException e)
            {
                return null;
            }
            catch (Exception ee)
            {
                throw (ee);
            }
        }

        public virtual string NodeId
        {
            get
            {
                try
                {
                    object c = pNode.getAttribute("id", 2);
                    if (c != null)
                        return ((string)c).ToLower();
                    return null;
                }
                catch (NullReferenceException e)
                {
                    //捕捉Null引用错误 以便处理那些不带有className的Element
                    return null;
                }
                catch (Exception ee)
                {
                    throw (ee);
                }
            }
            set { if (pNode != null) pNode.id = value; }
        }

        public virtual string NodeTagName
        {
            get
            {
                try
                {
                    if (pNode != null)
                        return pNode.tagName.ToLower();
                    return null;
                }
                catch (NullReferenceException e)
                {
                    log.Debug("TagName NullReferenceException!");
                    return null;
                }
                catch (Exception ee)
                {
                    throw (ee);
                }
            }
            //set { if (pElement != null) pElement.tagName = value; }
        }

        public virtual string NodeHref
        {
            get
            {
                try
                {
                    if (pNode != null)
                    {
                        object h = pNode.getAttribute("href", 2);
                        if (h != null)
                            return (string)h;
                                        
                    }
                    return null;
                }
                catch (NullReferenceException e)
                {
                    log.Warn("Href NullReferenceException!");
                    return null;
                }
                catch (Exception ee)
                {
                    throw (ee);
                }
            }
            //set { if (pElement != null) pElement.tagName = value; }
        }

        public virtual bool NodeRemoveHidden()
        {
            IHTMLCurrentStyle cstyle = null;
            cstyle = (IHTMLCurrentStyle)((IHTMLElement2)pNode).currentStyle;
            if (cstyle != null &&
                    (cstyle.display.ToLower() == "none" || cstyle.visibility.ToLower() == "hidden")
                )
            {
                pNode.outerHTML = null;
                return true;
            }
            return false;
        }

        public virtual bool NodeClick()
        {
            try
            {
                if (pNode != null)
                {
                    pNode.click();                    
                    AjaxWatch();                   

                    if (m_IsRedirctionEnable && m_RedirectUrl.Count > 0)
                    {
                        log.Debug("NodeClick-Redirection:" + GetCurrentURL() + " >>>> " + m_RedirectUrl.Peek());
                        Navigate(m_RedirectUrl.Pop());
                    }
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                log.Debug("NodeClick Error:", e);
                return false;
            }
        }

        //*********** Automation Task ***********

        public virtual bool PerformClickButton(string btnnameorid)
        {
            return GetWebBrowser().AutomationTask_PerformClickButton(btnnameorid);
        }

        public virtual bool PerformSelectRadio(string radionameorid)
        {
            return GetWebBrowser().AutomationTask_PerformSelectRadio(radionameorid);
        }

        public virtual bool PerformEnterData(string inputnameorid, string strValue)
        {
            return GetWebBrowser().AutomationTask_PerformEnterData(inputnameorid, strValue);
        }

        public virtual bool PerformEnterDataTextArea(string inputnameorid, string strValue)
        {
            return GetWebBrowser().AutomationTask_PerformEnterDataTextArea(inputnameorid, strValue);
        }

        public virtual void SimulateKeyStroke(Keys keycode)
        {
            GetWebBrowser().AutomationTask_SimulateKeyStroke(keycode);
        }

        public virtual bool PerformClickLink(string linknameorid)
        {            
            return GetWebBrowser().AutomationTask_PerformClickLink(linknameorid);            
        }
        
        #endregion

		#region 构造函数 **********************************************************

		/// <summary>
		/// 构造函数
		/// </summary>
        public VBAIE()
        {

        }

        public VBAIE(System.Runtime.Serialization.SerializationInfo s, System.Runtime.Serialization.StreamingContext c)
        {

        }

        #endregion

        #region IVBAObject 成员

        public string Name
        {
            get { return "IE"; }
        }

        public bool BeSerializable
        {
            get { return false; }
        }

        public VBAObjectLife Life
        {
            get { return VBAObjectLife.Task; }
        }

        public virtual void Reset()
        {            
            if (pWB != null)
            {
                UnregiserHandler();
                CWBPool.ReturnCWB(pWB);
                pWB = null;
            }
            pNode = null;
            pNodesEnum = null;
            pNodeList = null;
            m_NodeStack.Clear();
        }

        #endregion

        #region ISerializable 成员

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            //Do Nothing!
        }

        #endregion     

        #region IDisposable 成员

        private bool IsDisposed = false;

        public virtual void Dispose()  
        {  
            Dispose(true);  
            GC.SuppressFinalize(this);  
        }

        protected void Dispose(bool Disposing)  
        {  
            if(!IsDisposed)  
            {
                if (Disposing)  
                {
                    //清理托管资源

                }  
                //清理非托管资源
                if (pWB != null)
                {
                    UnregiserHandler();
                    CWBPool.ReturnCWB(pWB);
                    pWB = null;
                }
            }  
            IsDisposed=true;  
        }

        ~VBAIE()
        {  
            Dispose(false);  
        } 

        #endregion
    }
}
