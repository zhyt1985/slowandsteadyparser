using System;
namespace SlowAndSteadyParser
{
    interface IVBAHtml
    {        
        string NodeClassName { get; set; }
        string NodeHref { get; }
        string NodeId { get; set; }
        string NodeInnerHtml { get; set; }
        string NodeInnerText { get; set; }
        bool NodeIsNull();
        bool NodeMoveNext();
        bool NodeMoveNext(int time);
        bool NodeMoveNextCondition(NodeSeekConditions cs);
        string NodeOuterHTML { get; set; }
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
        string NodeTagName { get; }
        NodeSeekConditions NodeSeekClassName(string classname);
        NodeSeekConditions NodeSeekId(string id);
        NodeSeekConditions NodeSeekInnerHTML(string innerHTML, bool exactmatch);
        NodeSeekConditions NodeSeekInnerText(string innerText, bool exactmatch);
        NodeSeekConditions NodeSeekTagName(string tagname);        
        string GetCurrentURL();
        void Navigate(string url);
        void Navigate(string url, int retrytime);
        void NavigateDelay(string url, int delaymilliseconds);        
        string GetCurrentTitle();
        string NodeGetAttribute(string attribute);
    }
}
