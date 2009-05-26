using System;
using System.Collections.Specialized;
using System.Collections.Generic;
namespace SlowAndSteadyParser
{
    interface IVBAUtility
    {
        string ConvertHrefToAbsoluteURL(string currentpageurl, string href);
        System.Collections.Specialized.NameValueCollection ConvertURLToQueryString(string URL);
        string GB2312ToUnicode(string sourcetext);
        string UnicodeToGB2312(string sourcetext);
        string UnicodeToUTF8(string sourcetext);
        string UTF8ToUnicode(string sourcetext);
        NameValueCollection ChineseAddressParse(string srcaddress);
        float ComputeLevenshteinIndex(string str1, string str2);
        float ComputeLCSIndex(string str1, string str2);
        List<string> ChineseCommentReduction(string src);        
        void ADSLChangeIP();
        string GetRandomUserName();
        string PathCombine(string p1, string p2);
        System.Data.OleDb.OleDbConnection OpenSafeOleDbConnection(string oledbconnstring);
        void CloseSafeOleDbConnectionByString(string oledbconnstring);
        void CloseSafeOleDbConnection(System.Data.OleDb.OleDbConnection oledbconn);
        string AccessSqlString(string source);
        
    }
}
