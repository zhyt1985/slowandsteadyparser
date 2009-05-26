using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public class VBAUtilityStop : VBAUtility
    {
        #region IVBAUtility 成员

        public override string ConvertHrefToAbsoluteURL(string currentpageurl, string href)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override System.Collections.Specialized.NameValueCollection ConvertURLToQueryString(string URL)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string GB2312ToUnicode(string sourcetext)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string UnicodeToGB2312(string sourcetext)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string UnicodeToUTF8(string sourcetext)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string UTF8ToUnicode(string sourcetext)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override System.Collections.Specialized.NameValueCollection ChineseAddressParse(string srcaddress)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void ADSLChangeIP()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override float ComputeLevenshteinIndex(string str1, string str2)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override float ComputeLCSIndex(string str1, string str2)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override List<string> ChineseCommentReduction(string src)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string GetRandomUserName()
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string PathCombine(string p1, string p2)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override System.Data.OleDb.OleDbConnection OpenSafeOleDbConnection(string oledbconnstring)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void CloseSafeOleDbConnectionByString(string oledbconnstring)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override void CloseSafeOleDbConnection(System.Data.OleDb.OleDbConnection oledbconn)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        public override string AccessSqlString(string source)
        {
            throw new Exception("VBAEngine Stopping Exception!");
        }

        #endregion

        #region IVBAObject 成员


        public override void Reset()
        {

        }

        #endregion

    }
}
