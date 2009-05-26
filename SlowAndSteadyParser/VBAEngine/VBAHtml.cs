using System;
using System.Collections.Generic;
using System.Text;
using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Tags;
using Winista.Text.HtmlParser.Filters;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Nodes;
using System.Collections;
using System.Windows.Forms;
using System.Net; 
using System.IO; 
using System.Text.RegularExpressions;
using Winista.Text.HtmlParser.Lex;

namespace SlowAndSteadyParser
{
    public class VBAHtml : IVBAObject, IVBAHtml
    {
        //log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string m_url = null;
        private Parser m_parser = new Parser();
        private NodeList m_nodelist = null;
        private INode m_node = null;
        private ISimpleNodeIterator m_nodeenum = null;
        private Stack m_nodestack = new Stack();

        #region IVBAObject 成员

        public string Name
        {
            get { return "Html"; }
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
            
        }

        #endregion

        #region 构造函数

		/// <summary>
		/// 构造函数
		/// </summary>
        public VBAHtml()
        {

        }

        public VBAHtml(System.Runtime.Serialization.SerializationInfo s, System.Runtime.Serialization.StreamingContext c)
        {

        }

        #endregion

        #region ISerializable 成员

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            
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
                    m_parser = null;
                    //if (m_nodelist != null) m_nodelist.Clear();
                    m_nodelist = null;
                    m_node = null;
                    m_nodeenum = null;
                    //m_nodestack.Clear();
                    m_nodestack = null;
                }  
                //清理非托管资源

            }  
            IsDisposed=true;  
        }

        ~VBAHtml()
        {  
            Dispose(false);  
        } 

        #endregion

        #region IVBAHtml 成员

        public virtual string NodeClassName
        {
            get
            {
                try
                {
                    if (m_node is TagNode)
                    {
                        return (m_node as TagNode).GetAttribute("class").ToLower();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (m_node is TagNode)
                {
                    TagNode tag = m_node as TagNode;
                    tag.SetAttribute("class", value);
                }
            }
        }

        public virtual string NodeHref
        {
            get
            {
                try
                {
                    if (m_node is TagNode)
                    {
                        return (m_node as TagNode).GetAttribute("href");
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (m_node is TagNode)
                {
                    TagNode tag = m_node as TagNode;
                    tag.SetAttribute("href", value);
                }
            }
        }

        public virtual string NodeId
        {
            get
            {
                try
                {
                    if (m_node is TagNode)
                    {
                        return (m_node as TagNode).GetAttribute("ID").ToLower();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (m_node is TagNode)
                {
                    TagNode tag = m_node as TagNode;
                    tag.SetAttribute("ID",value);
                }
            }
        }

        public virtual string NodeInnerHtml
        {
            get
            {
                return m_node.ToHtml();
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public virtual string NodeInnerText
        {
            get
            {
                return m_node.GetText();
            }
            set
            {
                m_node.SetText(value);
            }
        }

        public virtual bool NodeIsNull()
        {
            return (m_node == null);
        }

        public virtual bool NodeMoveNext()
        {
            if (m_nodeenum != null && m_nodeenum.HasMoreNodes())
            {
                m_node = m_nodeenum.NextNode();
                return true;
            }
            else
            {
                m_node = null;
                return false;
            }
        }

        public virtual bool NodeMoveNext(int time)
        {
            int i = 1;

            if (m_nodeenum == null)
            {
                m_node = null;
                return false;
            }

            while (m_nodeenum.HasMoreNodes())
            {
                if (i >= time)
                {
                    m_node = m_nodeenum.NextNode();
                    return true;
                }
                i++;
            }
            m_node = null;
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
                        case NodeSeekConditonType.InnerTextPartialMatch:
                            IsSelected = IsSelected && !((NodeInnerText != null && NodeInnerText.IndexOf(c.ConditionValue) > -1) ^ c.IsIncluding);
                            break;
                        case NodeSeekConditonType.InnerHTMLExactMatch:
                            IsSelected = IsSelected && !((NodeInnerHtml != null && c.ConditionValue == NodeInnerHtml) ^ c.IsIncluding);
                            break;
                        case NodeSeekConditonType.InnerHTMLPartialMatch:
                            IsSelected = IsSelected && !((NodeInnerHtml != null && NodeInnerHtml.IndexOf(c.ConditionValue) > -1) ^ c.IsIncluding);
                            break;
                        default: break;
                    }
                }
                if (IsSelected)
                {
                    return true;
                }
                Application.DoEvents();
            }
            return false;
        }

        public virtual string NodeOuterHTML
        {
            get
            {
                return m_node.ToHtml();
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public virtual string NodeOuterText
        {
            get
            {
                return m_node.ToPlainTextString();
            }
            set
            {
                m_node.SetText(value);
            }
        }

        public virtual bool NodePopRange()
        {   
            m_nodelist = (NodeList)m_nodestack.Pop();
            m_nodeenum = (ISimpleNodeIterator)m_nodestack.Pop();
            m_node = (INode)m_nodestack.Pop();
            return true;
        }

        private IList<INode> ParseFilterIntoNodeList(INodeFilter nodefilter)
        {
            return m_parser.Parse(nodefilter).ToNodeArray();
        }

        public virtual bool NodePushRangeAll()
        {
            NodeList nl = m_parser.Parse(AndFilter.TrueFilter);
            if (nl.Count > 0)
            {
                m_nodestack.Push(m_node);
                m_nodestack.Push(m_nodeenum);
                m_nodestack.Push(m_nodelist);

                m_nodelist = nl;
                m_nodeenum = m_nodelist.Elements();
                m_node = null;
                return true;
            }
            else
                return false;
        }

        public virtual bool NodePushRangeById(string id)
        {
            NodeList nl = m_parser.Parse(new HasAttributeFilter("ID", id));
            if (nl.Count > 0)
            {
                m_nodestack.Push(m_node);
                m_nodestack.Push(m_nodeenum);
                m_nodestack.Push(m_nodelist);

                m_nodelist = nl;
                m_nodeenum = m_nodelist.Elements();
                m_node = null;
                return true;
            }
            else
                return false;
        }

        public virtual bool NodePushRangeByName(string elementname)
        {
            NodeList nl = m_parser.Parse(new HasAttributeFilter("name", elementname));
            if (nl.Count > 0)
            {
                m_nodestack.Push(m_node);
                m_nodestack.Push(m_nodeenum);
                m_nodestack.Push(m_nodelist);

                m_nodelist = nl;
                m_nodeenum = m_nodelist.Elements();
                m_node = null;
                return true;
            }
            else
                return false;
        }

        public virtual bool NodePushRangeByTagName(string tagname)
        {
            NodeList nl = m_parser.Parse(new TagNameFilter(tagname));
            if (nl.Count > 0)
            {
                m_nodestack.Push(m_node);
                m_nodestack.Push(m_nodeenum);
                m_nodestack.Push(m_nodelist);

                m_nodelist = nl;
                m_nodeenum = m_nodelist.Elements();
                m_node = null;
                return true;
            }
            else
                return false;
        }        

        //加入所有元素(包括子对象)
        private IList<INode> AllLayerTransefer(IList<INode> nl)
        {
            IList<INode> nlist = new List<INode>();
            AllLayerTransferRecursion(ref nlist, nl);
            return nlist;
        }

        private void AllLayerTransferRecursion(ref IList<INode> nlist, IList<INode> nl)
        {
            foreach (INode n in nl)
            {
                nlist.Add(n);
                if (nl != null && nl.Count > 0)
                    AllLayerTransferRecursion(ref nlist, n.Children.ToNodeArray());
            }
        }

        public virtual bool NodePushRangeChildren()
        {
            NodeList nl = m_node.Children;
            nl = nl.ExtractAllNodesThatMatch(AndFilter.TrueFilter,true);
            if (nl.Count > 0)
            {
                m_nodestack.Push(m_node);
                m_nodestack.Push(m_nodeenum);
                m_nodestack.Push(m_nodelist);

                m_nodelist = nl;
                m_nodeenum = m_nodelist.Elements();
                m_node = null;
                return true;
            }
            else
                return false;
        }

        public virtual bool NodePushRangeImages()
        {
            NodeList nl = m_parser.Parse(new TagNameFilter("img"));
            if (nl.Count > 0)
            {
                m_nodestack.Push(m_node);
                m_nodestack.Push(m_nodeenum);
                m_nodestack.Push(m_nodelist);

                m_nodelist = nl;
                m_nodeenum = m_nodelist.Elements();
                m_node = null;
                return true;
            }
            else
                return false;
        }

        public virtual bool NodePushRangeLinks()
        {
            NodeList nl = m_parser.Parse(new TagNameFilter("a"));
            if (nl.Count > 0)
            {
                m_nodestack.Push(m_node);
                m_nodestack.Push(m_nodeenum);
                m_nodestack.Push(m_nodelist);

                m_nodelist = nl;
                m_nodeenum = m_nodelist.Elements();
                m_node = null;
                return true;
            }
            else
                return false;
        }

        public virtual bool NodePushRangeScripts()
        {
            NodeList nl = m_parser.Parse(new TagNameFilter("SCRIPT"));
            if (nl.Count > 0)
            {
                m_nodestack.Push(m_node);
                m_nodestack.Push(m_nodeenum);
                m_nodestack.Push(m_nodelist);

                m_nodelist = nl;
                m_nodeenum = m_nodelist.Elements();
                m_node = null;
                return true;
            }
            else
                return false;
        }

        public virtual string NodeTagName
        {
            get
            {
                if (m_node is TagNode)
                {
                    TagNode tag = m_node as TagNode;
                    return tag.TagName.ToLower();
                }
                else
                {
                    return null;
                }
            }
        }

        public virtual NodeSeekConditions NodeSeekTagName(string tagname)
        {
            return new NodeSeekConditions(NodeSeekConditonType.TagName, tagname);
        }

        public virtual NodeSeekConditions NodeSeekClassName(string classname)
        {
            return new NodeSeekConditions(NodeSeekConditonType.ClassName, classname);
        }

        public virtual NodeSeekConditions NodeSeekId(string id)
        {
            return new NodeSeekConditions(NodeSeekConditonType.Id, id);
        }

        public virtual NodeSeekConditions NodeSeekInnerText(string innerText, bool exactmatch)
        {
            if (exactmatch)
                return new NodeSeekConditions(NodeSeekConditonType.InnerTextExactMatch, innerText);
            else
                return new NodeSeekConditions(NodeSeekConditonType.InnerTextPartialMatch, innerText);
        }

        public virtual NodeSeekConditions NodeSeekInnerHTML(string innerHTML, bool exactmatch)
        {
            if (exactmatch)
                return new NodeSeekConditions(NodeSeekConditonType.InnerHTMLExactMatch, innerHTML);
            else
                return new NodeSeekConditions(NodeSeekConditonType.InnerHTMLPartialMatch, innerHTML);
        }

        public virtual string GetCurrentURL()
        {
            return m_url;
        }

        public virtual string GetCurrentTitle()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private string getHtml(string url, string charSet)
        {
            //创建WebClient实例myWebClient 
            WebClient myWebClient = new WebClient();
            //            需要注意的： 
            //有的网页可能下不下来，有种种原因比如需要cookie,编码问题等等 
            //这是就要具体问题具体分析比如在头部加入cookie 
            // webclient.Headers.Add("Cookie", cookie); 
            //这样可能需要一些重载方法。根据需要写就可以了

            //获取或设置用于对向 Internet 资源的请求进行身份验证的网络凭据。             
            myWebClient.Credentials = CredentialCache.DefaultCredentials;

            //如果服务器要验证用户名,密码 
            //NetworkCredential mycred = new NetworkCredential(struser, strpassword); 
            //myWebClient.Credentials = mycred; 
            //从资源下载数据并返回字节数组。（加@是因为网址中间有"/"符号） 
            byte[] myDataBuffer = myWebClient.DownloadData(url);
            string strWebData = Encoding.Default.GetString(myDataBuffer);

            //获取网页字符编码描述信息 
            Match charSetMatch = Regex.Match(strWebData, "<meta([^<]*)charset=([^<]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string webCharSet = charSetMatch.Groups[2].Value;
            if (charSet == null || charSet == "")
                charSet = webCharSet;
            if (charSet != null && charSet != "" && Encoding.GetEncoding(charSet) != Encoding.Default)
                strWebData = Encoding.GetEncoding(charSet).GetString(myDataBuffer);
            return strWebData;
        }

        public virtual void Navigate(string url)
        {
            try
            {
                m_parser = new Parser(new Lexer(getHtml(url,null)),null);
                m_nodestack.Clear();
                m_node = null;
                m_nodeenum = null;
                m_nodelist = null;
                m_url = url;
                //m_parser.InputHTML = getHtml(url, null);
                //m_parser.URL = url;
                //m_parser.AnalyzePage();
            }
            catch (Exception e)
            {
                log.Error("Navigate: "+url, e);
            }
        }

        public virtual void Navigate(string url, int retrytime)
        {
            int i = 0;
            if (url.Length > 0)
            {
                while (i < retrytime)
                {
                    try
                    {
                        Navigate(url);
                        log.Debug("Navigation Complete");
                        return;
                    }
                    catch
                    {
                        i++;
                        log.Debug("Navigation Error, Retry " + i.ToString());
                        System.Threading.Thread.Sleep(4000);
                    }
                }
                log.Debug("Too Much Error...Navigation Abandon");
            }
        }

        public virtual void NavigateDelay(string url, int delaymilliseconds)
        {
            if (url != null && url.Length > 0)
            {
                CWBPool.WebBrowserNavigationDelayer(url, delaymilliseconds);
                Navigate(url);
            }
        }

        public virtual string NodeGetAttribute(string attribute)
        {
            if (m_node is TagNode)
            {
                TagNode tag = m_node as TagNode;
                return tag.GetAttribute(attribute);
            }
            else
            {
                return null;
            }
        }

        #endregion

    }
}
