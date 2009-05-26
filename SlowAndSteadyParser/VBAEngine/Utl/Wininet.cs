using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Net;

namespace SlowAndSteadyParser
{
      class Wininet
      {
          [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
          public static extern bool InternetGetCookie(
          string lpszUrlName,
          string lpszCookieName,
          StringBuilder lpszCookieData,
          [MarshalAs(UnmanagedType.U4)]
          ref int lpdwSize
          );


          [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
          public static extern bool InternetSetCookie(
          string lpszUrlName,
          string lpszCookieName,
          string lpszCookieData
          );


          [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
          public static extern bool InternetSetOption(
          int hInternet,
          int dwOption,
          string lpBuffer,
          int dwBufferLength
          );

          public static CookieContainer GetUriCookieContainer(Uri uri)
          {
              CookieContainer cookies = null;

              // Determine the size of the cookie
              int datasize = 256;
              StringBuilder cookieData = new StringBuilder(datasize);

              if (!InternetGetCookie(uri.ToString(), null, cookieData,
                ref datasize))
              {
                  if (datasize < 0)
                      return null;

                  // Allocate stringbuilder large enough to hold the cookie
                  cookieData = new StringBuilder(datasize);
                  if (!InternetGetCookie(uri.ToString(), null, cookieData,
                    ref datasize))
                      return null;
              }

              if (cookieData.Length > 0)
              {
                  cookies = new CookieContainer();
                  cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
              }
              return cookies;
          }

          public static bool InternetGetCookieEX(string url,string cookiename,ref string cookiedata)
          {
              int length = 1024;
              StringBuilder sb = new StringBuilder(length);
              //检查Domain是否忘记写http://
              if (url.ToLower().IndexOf("http:") <= 0) url = "http://" + url;
              //检查Cookiename
              if (cookiename == "") cookiename = null;
              if (Wininet.InternetGetCookie(url, cookiename, sb, ref length))
              {
                cookiedata = sb.ToString();
                return true;
              }
              else
                return false;
             
          }
          //public static bool InternetClearCookieEX(string url, string cookiename)
          //{
          //    //检查Domain是否忘记写http://
          //    if (url.ToLower().IndexOf("http:") <= 0) url = "http://" + url;
          //    //检查Cookiename
          //    if (cookiename == "" || cookiename == null)
          //    {
          //        //清空当前domain下的cookie


          //    }
          //    else
          //        return InternetSetCookie(url, cookiename, null);
          //}
          public static bool InternetSetCookieEX(string url, string cookiename, string cookiedata)
          {
              try
              {
                  //检查Domain是否忘记写http://
                  if (url.ToLower().IndexOf("http:") <= 0) url = "http://" + url;
                  //检查Cookiename
                  if (cookiename == "" || cookiename == null)
                  {
                      //判断cookiedata是否是多区数据
                      if (cookiedata.IndexOf(";") > 0)
                      {
                          bool flag = true;
                          string[] cookies = cookiedata.Split(';');
                          foreach (string cook in cookies)
                          {
                              flag = flag & InternetSetCookie(url, null, cook);
                          }
                          return flag;
                      }
                      else
                          return InternetSetCookie(url, null, cookiedata);
                  }
                  else
                      return InternetSetCookie(url, cookiename, cookiedata);
              }
              catch (Exception e)
              {
                  throw(e);
              }

          }
      }
}
