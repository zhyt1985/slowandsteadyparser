using System;
using System.Collections.Generic;
using System.Text;
using SharpICTCLAS;
using System.IO;
using System.Text.RegularExpressions;

namespace SlowAndSteadyParser
{
    public class CommentDimensionReduction
    {
        //log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static int ms_nKind = 2;  //在NShortPath方法中用来决定初步切分时分成几种结果
        private static WordSegment ms_wordSegment;
        private static Thesaurus ms_thesaurus;
        private static Regex ms_sentencesplitor = new Regex(@"[.!?;~！？。；…]", RegexOptions.Compiled);

        private static WordSegment getWordSegmentInstance()
        {
            if (ms_wordSegment == null)
            {
                try
                {
                    string dictPath = Path.Combine(Environment.CurrentDirectory, "SegmentDict") + Path.DirectorySeparatorChar;
                    ms_wordSegment = new WordSegment();
                    ms_wordSegment.InitWordSegment(dictPath);
                    log.Debug("正在初始化字典库，请稍候...");
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("字典库初始化失败!", "错误", System.Windows.Forms.MessageBoxButtons.OK);
                    log.Debug("字典库初始化失败");
                }                
            }
            return ms_wordSegment;
        }

        private static Thesaurus getThesaurusInstance()
        {
            if (ms_thesaurus == null)
            {
                try
                {
                    ms_thesaurus = new Thesaurus(System.IO.Path.Combine(Environment.CurrentDirectory, "SegmentDict") + System.IO.Path.DirectorySeparatorChar + "tyc.mdb");
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("同义词库初始化失败!","错误", System.Windows.Forms.MessageBoxButtons.OK);
                    log.Debug("同义词库初始化失败!");
                }   
            }
            return ms_thesaurus;
        }

        private static List<WordResult[]> resultSegment(string src)
        {
            String s;
            WordSegment ws = getWordSegmentInstance();
            lock(ws)
            {
                s = Utility.Traditional2Simplified(src);
                s = Utility.ToDBC(s);
                return ws.Segment(s, ms_nKind);
            }
        }

        private static List<WordResult[]> Split(string src)
        {
            List<WordResult[]> lwr = new List<WordResult[]>();
            string[] ss = ms_sentencesplitor.Split(src.Replace(Environment.NewLine,"."));
            foreach (string s in ss)
                if (s != null && s.Length > 0)
                    lwr.AddRange(resultSegment(s));
            return lwr;
        }

        public static string Segment(string src)
        {
            return PrintResult(resultSegment(src));
        }

        public static List<string> Reduce(string src)
        {
            List<WordResult[]> result = Split(src);
            List<string> ls = new List<string>();
            WordResult[] twr;
            string s;
            int k;
            bool IsNFound;            
            for (int i = 0; i < result.Count; i++)
            {
                IsNFound = false;
                for (int j = 1; j < result[i].Length - 1; j++)
                {
                    s = Utility.GetPOSString(result[i][j].nPOS, nPosLevel.LevelOne);
                    if (s == "n")                    
                    {
                        IsNFound = true;
                        continue;
                    }
                    if (IsNFound)
                    {
                        if (s == "a")
                        {
                            twr = result[i];
                            //去掉句首的连词
                            if (Utility.GetPOSString(twr[1].nPOS, nPosLevel.LevelOne) == "c")
                                twr[1].sWord = "";
                            ls.Add(PrintResultStringOnly(twr));
                            break;
                        }
                        else if (s == "d" || s == "n")
                        {
                            continue;
                        }
                        else
                        {
                            IsNFound = false;
                            continue;
                        }                            
                    }
                }
            }
            return ls;
        }

        public static List<string> ReduceWithReplacment(string src)
        {
            Thesaurus t = getThesaurusInstance();
            List<WordResult[]> result = Split(src);
            List<string> ls = new List<string>();
            WordResult[] twr;
            string s;
            int k;
            bool IsNFound;
            for (int i = 0; i < result.Count; i++)
            {
                IsNFound = false;
                for (int j = 1; j < result[i].Length - 1; j++)
                {
                    s = Utility.GetPOSString(result[i][j].nPOS, nPosLevel.LevelOne);
                    if (s == "n")
                    {
                        IsNFound = true;
                        continue;
                    }
                    if (IsNFound)
                    {
                        if (s == "a")
                        {
                            twr = result[i];
                            //去掉句首的连词
                            if (Utility.GetPOSString(twr[1].nPOS, nPosLevel.LevelOne) == "c")
                                twr[1].sWord = "";
                            ls.Add(t.ReplaceByProbility(PrintResultStringOnly(twr)));
                            break;
                        }
                        else if (s == "d" || s == "n")
                        {
                            continue;
                        }
                        else
                        {
                            IsNFound = false;
                            continue;
                        }
                    }
                }
            }
            return ls;
        }

        private static string PrintResult(List<WordResult[]> result)
        {
            string s = "";
            for (int i = 0; i < result.Count; i++)
            {
                for (int j = 1; j < result[i].Length - 1; j++)                    
                    s = s + string.Format(@"{0}/{1}", result[i][j].sWord, Utility.GetPOSString(result[i][j].nPOS, nPosLevel.LevelOne));
                s = s + Environment.NewLine;
            }
            return s;
        }

        private static string PrintResult(WordResult[] wr)
        {
            string s = "";
            for (int j = 1; j < wr.Length - 1; j++)
                s = s + string.Format(@"{0}/{1}", wr[j].sWord, Utility.GetPOSString(wr[j].nPOS, nPosLevel.LevelOne));
            return s;
        }

        private static string PrintResultStringOnly(WordResult[] wr)
        {
            string s = "";
            for (int j = 1; j < wr.Length - 1; j++)
                s = s + string.Format(@"{0}", wr[j].sWord);
            return s;
        }
    }
}
