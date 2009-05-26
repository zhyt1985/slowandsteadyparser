using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;

namespace SlowAndSteadyParser
{
    public enum UserNameCombination
    {
        /// <summary>
        /// 中文-名词
        /// </summary>
        C_N = 0,
        /// <summary>
        /// 中文-动词+名词
        /// </summary>
        C_VN = 1,
        /// <summary>
        /// 中文-形容词
        /// </summary>
        C_A = 2,
        /// <summary>
        /// 中文-形容词+名词
        /// </summary>
        C_AN = 3,
        /// <summary>
        /// 中文-名词+形容词
        /// </summary>
        C_NA = 4,
        /// <summary>
        /// 中文-名词+名词
        /// </summary>
        C_NN = 5,
        /// <summary>
        /// 英文-名词
        /// </summary>
        E_N = 6,
        /// <summary>
        /// 英文-动词
        /// </summary>
        E_V = 7,
        /// <summary>
        /// 英文-形容词
        /// </summary>
        E_A = 8,
        /// <summary>
        /// 英文-名词 + the + 形容词
        /// </summary>
        E_NtA = 9,
        /// <summary>
        /// 英文-形容词+名词
        /// </summary>
        E_AN = 10,

    }

    public enum Word_Pos
    {
        /// <summary>
        /// 形容词-a
        /// </summary>
        adj = 0,
        /// <summary>
        /// 动词-v
        /// </summary>
        verb = 1,
        /// <summary>
        /// 名词-n
        /// </summary>
        n = 2
    }

    public enum Word_Language
    {
        /// <summary>
        /// 中文
        /// </summary>
        Chinese = 0,
        /// <summary>
        /// 英文
        /// </summary>
        English = 1
    }

    public class UserNameCreator
    {
        //log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static List<String> ms_chinesedict_a;
        private static List<String> ms_chinesedict_n;
        private static List<String> ms_chinesedict_v;

        private static List<String> ms_englishadjdict_a;
        private static List<String> ms_englishadjdict_n;
        private static List<String> ms_englishadjdict_v;

        private static UserNameCreator ms_instanse;
        private Random rand = new Random();

        private bool OpenDatabase(string address)
        {
            try
            {
                string word;
                string pos;
                address = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + address + ";";
                using (OleDbConnection logdbcon = new OleDbConnection(address))
                {
                    logdbcon.Open();
                    OleDbCommand cmd = new OleDbCommand(@"SELECT * FROM [chinesedict]", logdbcon);
                    OleDbDataReader dr = cmd.ExecuteReader();

                    //中文词库
                    ms_chinesedict_a = new List<string>();
                    ms_chinesedict_n = new List<string>();
                    ms_chinesedict_v = new List<string>();
                    while (dr.Read())
                    {                        
                        word = ((string)dr["word"]);
                        pos = ((string)dr["pos"]);
                        switch (pos)
                        {
                            case "a":
                                ms_chinesedict_a.Add(word);
                                break;
                            case "n":
                                ms_chinesedict_n.Add(word);
                                break;
                            case "v":
                                ms_chinesedict_v.Add(word);
                                break;
                        }
                    }
                    dr.Close();

                    cmd = new OleDbCommand(@"SELECT * FROM [englishdict]", logdbcon);
                    dr = cmd.ExecuteReader();
                    //英文词库
                    ms_englishadjdict_a = new List<string>();
                    ms_englishadjdict_n = new List<string>();
                    ms_englishadjdict_v = new List<string>();
                    while (dr.Read())
                    {
                        word = ((string)dr["word"]);
                        pos = ((string)dr["pos"]);
                        switch (pos)
                        {
                            case "a":
                                ms_englishadjdict_a.Add(word);
                                break;
                            case "n":
                                ms_englishadjdict_n.Add(word);
                                break;
                            case "v":
                                ms_englishadjdict_v.Add(word);
                                break;
                        }
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

        public static UserNameCreator getInstanse()
        {
            if (ms_instanse == null)
            {
                ms_instanse = new UserNameCreator();
                if (ms_instanse.OpenDatabase(System.IO.Path.Combine(Environment.CurrentDirectory, "SegmentDict") + System.IO.Path.DirectorySeparatorChar + "dict.mdb"))
                {
                    return ms_instanse;
                }
                else
                    throw new Exception("无法实例化新UserNameCreator对象!");
            }
            else
                return ms_instanse;
        }

        private string getRandomWord(Word_Language wl, Word_Pos wp)
        {
            List<String> ls = null;
            switch (wp)
            {
                case Word_Pos.adj:
                    if (wl == Word_Language.Chinese)
                        ls = ms_chinesedict_a;
                    else
                        ls = ms_englishadjdict_a;
                    break;
                case Word_Pos.n:
                    if (wl == Word_Language.Chinese)
                        ls = ms_chinesedict_n;
                    else
                        ls = ms_englishadjdict_n;
                    break;
                case Word_Pos.verb:
                    if (wl == Word_Language.Chinese)
                        ls = ms_chinesedict_v;
                    else
                        ls = ms_englishadjdict_v;
                    break;
            }
            if (ls == null)
                return null;
            else
                return ls[rand.Next(0, ls.Count - 1)];
        }

        public string getRandomUserName()
        {
            string username = null;
            UserNameCombination r = UserNameCombination.C_A;
            //中英文摘别
            if (rand.NextDouble() <0.75)
                r = (UserNameCombination)rand.Next(0,6);
            else
                r = (UserNameCombination)rand.Next(6,11);
            switch(r)
            {
                case UserNameCombination.C_A:
                    username = getRandomWord(Word_Language.Chinese, Word_Pos.adj);
                    break;
                case UserNameCombination.C_AN:
                    username = getRandomWord(Word_Language.Chinese, Word_Pos.adj) + getRandomWord(Word_Language.Chinese, Word_Pos.n);
                    break;
                case UserNameCombination.C_N:
                    username = getRandomWord(Word_Language.Chinese, Word_Pos.n);
                    break;
                case UserNameCombination.C_NA:
                    username = getRandomWord(Word_Language.Chinese, Word_Pos.n) + getRandomWord(Word_Language.Chinese, Word_Pos.adj);
                    break;
                case UserNameCombination.C_NN:
                    username = getRandomWord(Word_Language.Chinese, Word_Pos.n) + getRandomWord(Word_Language.Chinese, Word_Pos.n);
                    break;
                case UserNameCombination.C_VN:
                    username = getRandomWord(Word_Language.Chinese, Word_Pos.verb) + getRandomWord(Word_Language.Chinese, Word_Pos.n);
                    break;
                case UserNameCombination.E_A:
                    username = getRandomWord(Word_Language.English, Word_Pos.adj);
                    break;
                case UserNameCombination.E_AN:
                    username = getRandomWord(Word_Language.English, Word_Pos.adj) + getRandomWord(Word_Language.English, Word_Pos.n);
                    break;
                case UserNameCombination.E_N:
                    username = getRandomWord(Word_Language.English, Word_Pos.n);
                    break;
                case UserNameCombination.E_NtA:
                    username = getRandomWord(Word_Language.English, Word_Pos.n) +" the "+ getRandomWord(Word_Language.English, Word_Pos.adj);
                    break;
                case UserNameCombination.E_V:
                    username = getRandomWord(Word_Language.English, Word_Pos.verb);
                    break;
            }
            //四分之一的几率随机加上后缀
            if (username.Length <= 10 && rand.NextDouble() < 0.25)
            {
                if (rand.NextDouble() < 0.5)
                    username = username + rand.Next(1500, 2500).ToString();
                else
                    username = username + rand.Next(1, 999).ToString();
            }
            return username;
        }
    }
}
