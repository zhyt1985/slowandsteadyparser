using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace SlowAndSteadyParser
{
    public class VBAUtilityHost : IVBAObjectHost, IVBAUtility
    {
        private VBAUtility m_vbautility = null;

        #region IVBAUtility 成员

        public string ConvertHrefToAbsoluteURL(string currentpageurl, string href)
        {
            return m_vbautility.ConvertHrefToAbsoluteURL(currentpageurl, href);
        }

        public NameValueCollection ConvertURLToQueryString(string URL)
        {
            return m_vbautility.ConvertURLToQueryString(URL);
        }

        public string GB2312ToUnicode(string sourcetext)
        {
            return m_vbautility.GB2312ToUnicode(sourcetext);
        }

        public string UnicodeToGB2312(string sourcetext)
        {
            return m_vbautility.UnicodeToGB2312(sourcetext);
        }

        public string UnicodeToUTF8(string sourcetext)
        {
            return m_vbautility.UnicodeToUTF8(sourcetext);
        }

        public string UTF8ToUnicode(string sourcetext)
        {
            return m_vbautility.UTF8ToUnicode(sourcetext);
        }

        public NameValueCollection ChineseAddressParse(string srcaddress)
        {
            return m_vbautility.ChineseAddressParse(srcaddress);
        }

        #endregion        

        #region IVBAObjectHost 成员

        public string Name
        {
            get { return "Utility"; }
        }

        public IVBAObject Tenant
        {
            get
            {
                return m_vbautility;
            }
            set
            {
                m_vbautility = (VBAUtility)value;
            }
        }

        #endregion

        #region IVBAUtility 成员


        public void ADSLChangeIP()
        {
            m_vbautility.ADSLChangeIP();
        }

        #endregion

        #region IVBAUtility 成员

        public float ComputeLevenshteinIndex(string str1, string str2)
        {
            return m_vbautility.ComputeLevenshteinIndex(str1, str2);
        }

        public float ComputeLCSIndex(string str1, string str2)
        {
            return m_vbautility.ComputeLCSIndex(str1, str2);
        }

        #endregion

        #region IVBAUtility 成员


        public List<string> ChineseCommentReduction(string src)
        {
            return m_vbautility.ChineseCommentReduction(src);
        }

        #endregion

        #region IVBAUtility 成员


        public string GetRandomUserName()
        {
            return m_vbautility.GetRandomUserName();
        }

        #endregion

        #region IVBAUtility 成员


        public string PathCombine(string p1, string p2)
        {
            return m_vbautility.PathCombine(p1, p2);
        }

        #endregion

        #region IVBAUtility 成员

        public System.Data.OleDb.OleDbConnection OpenSafeOleDbConnection(string oledbconnstring)
        {
            return m_vbautility.OpenSafeOleDbConnection(oledbconnstring);
        }

        public void CloseSafeOleDbConnectionByString(string oledbconnstring)
        {
            m_vbautility.CloseSafeOleDbConnectionByString(oledbconnstring);
        }

        public void CloseSafeOleDbConnection(System.Data.OleDb.OleDbConnection oledbconn)
        {
            m_vbautility.CloseSafeOleDbConnection(oledbconn);
        }

        #endregion

        #region IVBAUtility 成员

        public string AccessSqlString(string source)
        {
            return m_vbautility.AccessSqlString(source);
        }

        #endregion
    }
}
