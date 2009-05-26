using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SlowAndSteadyParser
{
    public class ChineseAddressParser
    {
        #region 分割字符
        
        static Regex ms_regex_guo = new Regex(@"中国", RegexOptions.Compiled);
        static Regex ms_regex_jinjiao = new Regex(@"近郊", RegexOptions.Compiled);
        static Regex ms_regex_sheng = new Regex(@"[\u4e00-\u9fa5]+?省", RegexOptions.Compiled);
        static Regex ms_regex_shi = new Regex(@"[\u4e00-\u9fa5]+?市(?!场)", RegexOptions.Compiled);
        static Regex ms_regex_qu = new Regex(@"[\u4e00-\u9fa5]+?区", RegexOptions.Compiled);
        static Regex ms_regex_xiang = new Regex(@"[\u4e00-\u9fa5]+?乡", RegexOptions.Compiled);
        static Regex ms_regex_xian = new Regex(@"[\u4e00-\u9fa5]+?县", RegexOptions.Compiled);
        static Regex ms_regex_dao = new Regex(@"[\u4e00-\u9fa5]+?道", RegexOptions.Compiled);
        static Regex ms_regex_hutong = new Regex(@"[\u4e00-\u9fa5]+?胡同", RegexOptions.Compiled);
        static Regex ms_regex_nongtang = new Regex(@"[\u4e00-\u9fa5]+?弄堂", RegexOptions.Compiled);
        static Regex ms_regex_jie = new Regex(@"[\u4e00-\u9fa5]+?街", RegexOptions.Compiled);
        static Regex ms_regex_xiangg = new Regex(@"[\u4e00-\u9fa5]+?巷", RegexOptions.Compiled);
        static Regex ms_regex_lu = new Regex(@"[\u4e00-\u9fa5]+?路", RegexOptions.Compiled);
        static Regex ms_regex_cun = new Regex(@"[\u4e00-\u9fa5]+?村", RegexOptions.Compiled);
        static Regex ms_regex_zhen = new Regex(@"[\u4e00-\u9fa5]+?镇", RegexOptions.Compiled);
        static Regex ms_regex_hao = new Regex(@"[甲_乙_丙_0-9_-]+?号", RegexOptions.Compiled);
        static Regex ms_regex_point = new Regex(@"[\u4e00-\u9fa5]+?(?:广场|酒店|饭店|宾馆|中心|大厦|百货|大楼|商城)", RegexOptions.Compiled);
        static Regex ms_regex_ditie = new Regex(@"地铁[\u4e00-\u9fa5]+?线(?:[\u4e00-\u9fa5]+?站)?", RegexOptions.Compiled);

        #endregion

        #region 行政区划

        static Regex ms_regex_nation = new Regex(@"中国", RegexOptions.Compiled);
        static Regex ms_regex_province = new Regex(@"[\u4e00-\u9fa5]{2,10}?(?:省|特区|自治区|特别行政区)", RegexOptions.Compiled);
        static Regex ms_regex_city = new Regex(@"[\u4e00-\u9fa5]+?(?:市|地区|自治州)", RegexOptions.Compiled);
        static Regex ms_regex_nearby = new Regex(@"近郊", RegexOptions.Compiled);
        static Regex ms_regex_district = new Regex(@"[\u4e00-\u9fa5]+?区", RegexOptions.Compiled);
        static Regex ms_regex_county = new Regex(@"[\u4e00-\u9fa5]+?(?:乡|县)", RegexOptions.Compiled);
        static Regex ms_regex_street = new Regex(@"[\u4e00-\u9fa5]+?街道", RegexOptions.Compiled);
        static Regex ms_regex_road = new Regex(@"[\u4e00-\u9fa5]+?(?:胡同|弄堂|街|巷|路|道)", RegexOptions.Compiled);
        static Regex ms_regex_roadnear = new Regex(@"(?<=近)[\u4e00-\u9fa5]+?(?:胡同|弄堂|街|巷|路|道)", RegexOptions.Compiled);
        static Regex ms_regex_ip = new Regex(@"[\u4e00-\u9fa5]+?(?:开发区|科技区|园区)", RegexOptions.Compiled);
        static Regex ms_regex_zone = new Regex(@"[\u4e00-\u9fa5]+?(?:小区|社区|新村)", RegexOptions.Compiled);
        static Regex ms_regex_village = new Regex(@"[\u4e00-\u9fa5]+?村", RegexOptions.Compiled);
        static Regex ms_regex_town = new Regex(@"[\u4e00-\u9fa5]+?镇", RegexOptions.Compiled);
        static Regex ms_regex_number = new Regex(@"[甲_乙_丙_0-9_-]+号", RegexOptions.Compiled);
        static Regex ms_regex_plaza = new Regex(@"[\u4e00-\u9fa5]+?(?:广场|酒店|饭店|宾馆|中心|大厦|百货|大楼|商城)", RegexOptions.Compiled);
        static Regex ms_regex_underground = new Regex(@"地铁[\u4e00-\u9fa5]+?线(?:[\u4e00-\u9fa5]+?站)?", RegexOptions.Compiled);

        #endregion

        #region 组合分割器

        static Splitter ms_splitter_guo = new Splitter(ms_regex_guo, new Regex[] { ms_regex_nation });
        static Splitter ms_splitter_sheng = new Splitter(ms_regex_sheng, new Regex[] { ms_regex_province});
        static Splitter ms_splitter_shi = new Splitter(ms_regex_shi, new Regex[] { ms_regex_city },false);
        static Splitter ms_splitter_jinjiao = new Splitter(ms_regex_jinjiao, new Regex[] { ms_regex_nearby });
        static Splitter ms_splitter_qu = new Splitter(ms_regex_qu, new Regex[] { ms_regex_province, ms_regex_city, ms_regex_zone, ms_regex_ip, ms_regex_district }, false);
        static Splitter ms_splitter_xiang = new Splitter(ms_regex_xiang, new Regex[] { ms_regex_county });
        static Splitter ms_splitter_xian = new Splitter(ms_regex_xian, new Regex[] { ms_regex_county });
        static Splitter ms_splitter_dao = new Splitter(ms_regex_dao, new Regex[] { ms_regex_street, ms_regex_roadnear, ms_regex_road }, false);
        static Splitter ms_splitter_hutong = new Splitter(ms_regex_hutong, new Regex[] { ms_regex_roadnear, ms_regex_road }, false);
        static Splitter ms_splitter_nongtang = new Splitter(ms_regex_nongtang, new Regex[] { ms_regex_roadnear, ms_regex_road }, false);
        static Splitter ms_splitter_jie = new Splitter(ms_regex_jie, new Regex[] { ms_regex_roadnear, ms_regex_road }, false);
        static Splitter ms_splitter_lu = new Splitter(ms_regex_lu, new Regex[] { ms_regex_roadnear, ms_regex_road }, false);
        static Splitter ms_splitter_xiangg = new Splitter(ms_regex_xiangg, new Regex[] { ms_regex_roadnear, ms_regex_road }, false);
        static Splitter ms_splitter_cun = new Splitter(ms_regex_cun, new Regex[] { ms_regex_zone, ms_regex_village });
        static Splitter ms_splitter_zhen = new Splitter(ms_regex_zhen, new Regex[] { ms_regex_town });
        static Splitter ms_splitter_hao = new Splitter(ms_regex_hao, new Regex[] { ms_regex_number });
        static Splitter ms_splitter_point = new Splitter(ms_regex_point, new Regex[] { ms_regex_plaza });
        static Splitter ms_splitter_ditie = new Splitter(ms_regex_ditie, new Regex[] { ms_regex_underground });

        static Splitter[] ms_defaultsplitters = new Splitter[]
        {
            ms_splitter_guo,
            ms_splitter_sheng,
            ms_splitter_shi,
            ms_splitter_qu,
            ms_splitter_xiang,
            ms_splitter_xian,
            ms_splitter_dao,
            ms_splitter_hutong,
            ms_splitter_nongtang,
            ms_splitter_jie,
            ms_splitter_xiangg,
            ms_splitter_lu,
            ms_splitter_cun,
            ms_splitter_zhen,
            ms_splitter_hao,
            ms_splitter_point,
            ms_splitter_ditie,
            ms_splitter_jinjiao
        };

        #endregion

        private static IDictionary<int, Splitter> Split(string src, Splitter[] splitters)
        {
            IDictionary<int, Splitter> splitterdic = new SortedDictionary<int, Splitter>();
            //Match m;
            //MatchCollection mc;

            foreach (Splitter s in splitters)
            {
                foreach (Match m in s.SplitRegex.Matches(src))
                    if (m.Success)
                    {
                        splitterdic.Add(m.Index + m.Length, s);
                        if (s.IsSingleSplit)
                            break;
                    }
            }

            return splitterdic;
    
        }

        private static IList<Segment> Recognize(string src, IDictionary<int, Splitter> splitterdic)
        {
            Segment s;
            int index = 0;
            IList<Segment> segments = new List<Segment>();

            if (src.Length > 0)
            {
                foreach (KeyValuePair<int, Splitter> kvp in splitterdic)
                {
                    if (kvp.Key > index && kvp.Key < src.Length)
                    {
                        foreach (Regex r in kvp.Value.MatchRegexs)
                        {
                            s = SegmentRecognize(src.Substring(index, kvp.Key - index), r);
                            if (s == null)
                                continue;
                            else
                            {
                                segments.Add(s);                                
                                break;
                            }                            
                        }
                        index = kvp.Key;
                    }
                }

            }

            return segments;
        }

        private static Segment SegmentRecognize(string src, Regex r)
        {
            Match m = r.Match(src);
            if (m.Success)
            {
                return new Segment(m.Value, r);
            }
            else
                return null;
        }

        private static IList<string> SegmentsGetStringListForRegex(IList<Segment> segments, Regex r)
        {
            IList<string> ss = new List<string>();            
            foreach (Segment s in segments)
            {
                if (s.Matchregex == r)
                    ss.Add(s.StringSegment);
            }
            return ss;
        }

        private static string SegmentsGetStringForRegex(IList<Segment> segments, Regex r)
        {
            foreach (Segment s in segments)
            {
                if (s.Matchregex == r)
                    return s.StringSegment;
            }
            return null;
        }

        public static ChineseAddress Parse(string source)
        {
            source = source.Replace(".", "").Replace("，", "").Replace(",", "");
            IList<Segment> segments = Recognize(source, Split(source, ms_defaultsplitters));
            ChineseAddress ca = new ChineseAddress();
            ca.source = source;

            ca.nation = SegmentsGetStringForRegex(segments, ms_regex_nation);
            ca.province = SegmentsGetStringForRegex(segments, ms_regex_province);
            ca.city = SegmentsGetStringForRegex(segments, ms_regex_city);
            ca.district = SegmentsGetStringForRegex(segments, ms_regex_district);
            ca.county = SegmentsGetStringForRegex(segments, ms_regex_county);
            ca.street = SegmentsGetStringForRegex(segments, ms_regex_street);

            IList<string> roads = SegmentsGetStringListForRegex(segments, ms_regex_road);            
            foreach (string s in SegmentsGetStringListForRegex(segments, ms_regex_roadnear)) roads.Add(s);
            ca.roads = roads;

            ca.underground = SegmentsGetStringForRegex(segments, ms_regex_underground);
            ca.number = SegmentsGetStringForRegex(segments, ms_regex_number);
            ca.plaza = SegmentsGetStringForRegex(segments, ms_regex_plaza);
            ca.ip = SegmentsGetStringForRegex(segments, ms_regex_ip);
            ca.town = SegmentsGetStringForRegex(segments, ms_regex_town);
            ca.village = SegmentsGetStringForRegex(segments, ms_regex_village);
            return ca;
        }
    }
}
