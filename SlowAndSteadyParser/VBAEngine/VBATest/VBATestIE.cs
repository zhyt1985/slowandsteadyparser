using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using csExWB;
using IfacesEnumsStructsClasses;
using System.Collections;

//测试用
namespace SlowAndSteadyParser
{
    [Serializable]
    public class VBATestIE: VBAIE
    {
        public override void RemoveAllHiddenElements()
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

        }

        public override bool NodeMoveNext(int time)
        {
            int i = 1;

            if (pNodesEnum == null)
            {
                pNode = null;
                return false;
            }

            while (pNodesEnum.MoveNext())
            {

                if (i >= time)
                {
                    pNode = (IHTMLElement)(pNodesEnum.Current);
                    return true;
                }
                i++;
            }
            pNode = null;
            return false;
        }

        public override bool NodeMoveNextCondition(NodeSeekConditions cs)
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
                            IsSelected = IsSelected && !((c.ConditionValue == NodeInnerText) ^ c.IsIncluding);
                            break;
                        case NodeSeekConditonType.InnerTextPartialMatch:
                            IsSelected = IsSelected && !((NodeInnerText.IndexOf(c.ConditionValue) > 0) ^ c.IsIncluding);
                            break;
                        case NodeSeekConditonType.InnerHTMLExactMatch:
                            IsSelected = IsSelected && !((c.ConditionValue == NodeInnerHtml) ^ c.IsIncluding);
                            break;
                        case NodeSeekConditonType.InnerHTMLPartialMatch:
                            IsSelected = IsSelected && !((NodeInnerHtml.IndexOf(c.ConditionValue) > 0) ^ c.IsIncluding);
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

        public override bool NodeMoveFirstChild()
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
                        return true;
                    }
                    Application.DoEvents();
                }
                return false;

            }
            else
            {
                return false;
            }
        }

		#region 构造函数 **********************************************************

		/// <summary>
		/// 构造函数
		/// </summary>
        public VBATestIE()
        {

        }

        #endregion

        
    }
}
