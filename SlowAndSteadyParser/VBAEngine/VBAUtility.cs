using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.IO;
using System.Data.OleDb;

namespace SlowAndSteadyParser
{
    [Serializable]
    public class VBAUtility : IVBAObject, IVBAUtility
    {
        //log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region 公共方法 用于VBA调用 - Convert 转化

        public virtual string UnicodeToUTF8(string sourcetext)
        {
            return System.Web.HttpUtility.UrlEncode(sourcetext, Encoding.GetEncoding("UTF-8")).ToUpper();
        }

        public virtual string UTF8ToUnicode(string sourcetext)
        {
            return System.Web.HttpUtility.UrlDecode(sourcetext, Encoding.GetEncoding("UTF-8"));
        }

        public virtual string UnicodeToGB2312(string sourcetext)
        {
            return System.Web.HttpUtility.UrlEncode(sourcetext, Encoding.GetEncoding("GB2312")).ToUpper();
        }

        public virtual string GB2312ToUnicode(string sourcetext)
        {
            return System.Web.HttpUtility.UrlDecode(sourcetext, Encoding.GetEncoding("GB2312"));
        }

        public virtual string ConvertHrefToAbsoluteURL(string currentpageurl, string href)
        {
            try
            {
                Uri addr = new Uri(new Uri(currentpageurl), href);
                return addr.AbsoluteUri;
            }
            catch (Exception e)
            {
                log.Debug("ConvertHrefToAbsoluteURL - Wrong URL Input:" + e.ToString());
                return null;
            }
        }

        public virtual NameValueCollection ConvertURLToQueryString(string URL)
        {
            return System.Web.HttpUtility.ParseQueryString(URL);
        }

        #endregion

        #region 公共方法 用于VBA调用 - 生活常识工具

        public virtual NameValueCollection ChineseAddressParse(string srcaddress)
        {
            NameValueCollection nv = new NameValueCollection();            
            ChineseAddress ca = ChineseAddressParser.Parse(srcaddress);
            nv.Add("all", ca.ToString());
            nv.Add("nation", ca.nation);
            nv.Add("province", ca.province);
            nv.Add("city", ca.city);
            nv.Add("county", ca.county);
            nv.Add("district", ca.district);
            nv.Add("street", ca.street);
            nv.Add("number", ca.number);
            nv.Add("plaza", ca.plaza);
            nv.Add("industrialpark", ca.ip);
            nv.Add("town", ca.town);
            nv.Add("village", ca.village);
            if (ca.roads.Count > 0) nv.Add("road", ca.roads[0]);
            nv.Add("underground", ca.underground);            
            return nv;
        }

        #endregion

        #region 公共方法 用于VBA调用 - 字符串处理

        public virtual float ComputeLevenshteinIndex(string str1, string str2)
        {
            return (float)StringDifference.ComputeLevenshteinIndex(str1,str2);
        }

        public virtual float ComputeLCSIndex(string str1, string str2)
        {
            return (float)StringDifference.ComputeLCSIndex(str1, str2);
        }

        public virtual List<string> ChineseCommentReduction(string src)
        {
            string s = src.Replace("'", "\"").Replace(Environment.NewLine, "");
            return CommentDimensionReduction.ReduceWithReplacment(s);
        }

        public virtual string GetRandomUserName()
        {
            return UserNameCreator.getInstanse().getRandomUserName();
        }

        public virtual string PathCombine(string p1, string p2)
        {
            return Path.Combine(p1, p2);
        }

        public virtual string AccessSqlString(string source)
        {
            if (source == null)
                return "''";
            else
                return @"'" + source.Replace(@"'", @"''").Replace(Environment.NewLine, @"'& CHR(13) &'") + @"'";
        }

        #endregion

        #region 公共方法 用于VBA调用 - ADSL控制

        //private string m_lastip;

        public virtual void ADSLChangeIP()
        {
            CWBPool.ApplyChangeIP();
        }

        #endregion

        #region 公共方法 用于VBA调用 - SMS

        #endregion

        #region 公共方法 用于VBA调用 - Oledb

        public virtual OleDbConnection OpenSafeOleDbConnection(string oledbconnstring)
        {
            return SafeOledb.OpenSafeOleDbConnection(oledbconnstring);
        }

        public virtual void CloseSafeOleDbConnectionByString(string oledbconnstring)
        {
            SafeOledb.CloseSafeOleDbConnection(oledbconnstring);
        }

        public virtual void CloseSafeOleDbConnection(OleDbConnection oledbconn)
        {
            SafeOledb.CloseSafeOleDbConnection(oledbconn);
        }

        #endregion

        #region 构造函数 **************************************************************************

        /// <summary>
		/// 初始化对象
		/// </summary>
		public VBAUtility()
        {
        }

        /// <summary>
        /// 用来反序列化的构造函数
        /// </summary>
        public VBAUtility(System.Runtime.Serialization.SerializationInfo s, System.Runtime.Serialization.StreamingContext c)
        {
        }
		#endregion

		#region IVBAObject 成员

        public string Name
        {
            get { return "Utility"; }
        }

        public bool BeSerializable
        {
            get { return true; }
        }

        public VBAObjectLife Life
        {
            get { return VBAObjectLife.Task; }
        }

        public virtual void Reset()
        {
        }
        #endregion

        #region ISerializable 成员

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
                if(Disposing)  
                {
                    //清理托管资源                        
                }  
                //清理非托管资源
            }  
            IsDisposed=true;  
        }

        ~VBAUtility()  
        {  
            Dispose(false);  
        } 

        #endregion

    }
}
