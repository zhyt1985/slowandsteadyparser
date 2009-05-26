using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;

namespace SlowAndSteadyParser
{
    public struct ThesaurusDict
    {
        public string SrcWord;
        public string DstWord;
        public float Probility;
    }

    public class Thesaurus
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static List<ThesaurusDict> ms_td;
        public Thesaurus(string address)
        {
            if (!OpenDatabase(address))
                throw new Exception("无法初始化同义词库!");
        }
        
        private bool OpenDatabase(string address)
        {
            try
            {
                address = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + address + ";";
                using (OleDbConnection logdbcon = new OleDbConnection(address))
                {                     
                    logdbcon.Open();
                    OleDbCommand cmd = new OleDbCommand(@"SELECT * FROM [word]", logdbcon);
                    OleDbDataReader dr = cmd.ExecuteReader();
                    ThesaurusDict temptd;
                    ms_td = new List<ThesaurusDict>();
                    while (dr.Read())
                    {
                        temptd = new ThesaurusDict();
                        temptd.SrcWord = ((string)dr["src"]);
                        temptd.DstWord = ((string)dr["dst"]);
                        temptd.Probility = ((float)dr["pbl"]);
                        ms_td.Add(temptd);
                    }
                    dr.Close();
                return true;
                }
            }
            catch (Exception e)
            {
                log.Debug(e);
                return false;
            }
        }

        public string Replace(string src)
        {
            CheckInitial();
            IDictionary<int, ThesaurusDict> inttddict = new SortedDictionary<int, ThesaurusDict>();
            int index;
            foreach (ThesaurusDict td in ms_td)
            {
                index = 0;
                do
                {
                    index = src.IndexOf(td.SrcWord, index);
                    if (index > -1)
                    {
                        if (!inttddict.ContainsKey(index))
                            inttddict.Add(index, td);
                        index = index + 1;
                    }
                } while (index > -1);
            }
            //替换
            int point = 0;
            string dst = "";
            foreach (KeyValuePair<int, ThesaurusDict> kvp in inttddict)
            {
                if (kvp.Key > point)
                {
                    dst = dst + src.Substring(point, kvp.Key - point);
                    dst = dst + kvp.Value.DstWord;
                    point = kvp.Key + kvp.Value.SrcWord.Length;
                }
            }
            dst = dst + src.Substring(point);
            return dst;
        }

        private void CheckInitial()
        {
            if (ms_td == null)
                throw new Exception("同义词库未初始化!");
        }

        public string ReplaceByProbility(string src)
        {
            CheckInitial();

            IDictionary<int, ThesaurusDict> inttddict = new SortedDictionary<int, ThesaurusDict>();
            int index;
            Random rand = new Random();
            //全扫描
            foreach (ThesaurusDict td in ms_td)
            {
                //依概率扫描
                if (rand.NextDouble() < td.Probility)
                {
                    index = 0;
                    do
                    {
                        index = src.IndexOf(td.SrcWord, index);
                        if (index > -1)
                        {
                            if (!inttddict.ContainsKey(index))
                                inttddict.Add(index, td);
                            index = index + 1;
                        }
                    } while (index > -1);
                }
                                
            }
            //替换
            int point = 0;            
            string dst = "";
            foreach (KeyValuePair<int,ThesaurusDict> kvp in inttddict)
            {
                if (kvp.Key > point)
                {
                    dst = dst + src.Substring(point, kvp.Key - point);
                    dst = dst + kvp.Value.DstWord;
                    point = kvp.Key + kvp.Value.SrcWord.Length;
                }
            }
            dst = dst + src.Substring(point);
            return dst;
        }
    }
}
