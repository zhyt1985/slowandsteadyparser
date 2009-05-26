using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace SlowAndSteadyParser
{
    public class ChineseAddressWordParser
    {
        static AbstractWordmatch ms_headwordmatch;
        static AbstractWordmatch ms_nationwordmatch;
        static AbstractWordmatch ms_provincewordmatch;
        static AbstractWordmatch ms_citywordmatch;
        static AbstractWordmatch ms_countywordmatch;
        static AbstractWordmatch ms_districtwordmatch;
        static AbstractWordmatch ms_streetwordmatch;
        static AbstractWordmatch ms_roadwordmatch;
        static AbstractWordmatch ms_numberwordmatch;
        static AbstractWordmatch ms_plazawordmatch;
        static AbstractWordmatch ms_notewordmatch;
        static AbstractWordmatch ms_noisecollector;
        static AbstractWordmatch ms_townwordmatch;
        static AbstractWordmatch ms_villagewordmatch;
        static AbstractWordmatch ms_industrialparkwordmatch;
        static AbstractWordmatch ms_zonewordmatch;

        private static AggregateWordmatch NewSingleElementWordmatch(string category, string element)
        {            
            List<String> scnations = new List<String>();
            scnations.Add(element);
            return new AggregateWordmatch(category, scnations);
        }

        private static AggregateWordmatch NewElementsWordmatch(string category, string[] elements)
        {
            List<String> scnations = new List<String>();
            scnations.AddRange(elements);
            return new AggregateWordmatch(category, scnations);
        }

        private static void InitBasicWordmatch()
        {
            //国名
            ms_nationwordmatch = NewSingleElementWordmatch("nation", "中国");

            //省和直辖市
            ms_provincewordmatch = new AggregateWordmatch("province", "province.txt", true);

            //地级市
            ms_citywordmatch = new AggregateWordmatch("city", "city.txt", true);

            //县级市
            ms_countywordmatch = new RegexWordmatch("county", new Regex(@"[\u4e00-\u9fa5]+?(县|市)", RegexOptions.Compiled));

            //工业区、高新区
            ms_industrialparkwordmatch = new RegexWordmatch("plaza", new Regex(@"[\u4e00-\u9fa5]+(园区|工业区|工业城)", RegexOptions.Compiled));

            //区
            ms_districtwordmatch = new RegexWordmatch("district", new Regex(@"近郊|[\u4e00-\u9fa5]+?区", RegexOptions.Compiled));

            //街道&道路
            ms_streetwordmatch = new RegexWordmatch("street", new Regex(@"[\u4e00-\u9fa5]+街道", RegexOptions.Compiled));
            ms_roadwordmatch = new RegexWordmatch("road", new Regex(@"[\u4e00-\u9fa5]+?(胡同|弄堂|街|巷|路|道)", RegexOptions.Compiled));

            //门牌
            ms_numberwordmatch = new RegexWordmatch("number", new Regex(@"(\d|-|甲|乙|丙)+?号(?!楼)", RegexOptions.Compiled));

            //住宅区
            ms_zonewordmatch = new RegexWordmatch("number", new Regex(@"[\u4e00-\u9fa5]+?(社区|小区)", RegexOptions.Compiled));

            //广场/购物中心/酒店
            ms_plazawordmatch = new RegexWordmatch("plaza", new Regex(@"[\u4e00-\u9fa5]+(层|楼|广场|商城|商场|酒店|购物中心|市场|大厦|校区|百货)([A-Z]座)?", RegexOptions.Compiled));

            //镇
            ms_townwordmatch = new RegexWordmatch("town", new Regex(@"[\u4e00-\u9fa5]+?(镇|乡)", RegexOptions.Compiled));

            //村
            ms_villagewordmatch = new RegexWordmatch("village", new Regex(@"[\u4e00-\u9fa5]+?村", RegexOptions.Compiled));

            //链头
            ms_headwordmatch = new HeadWordmatch();

            //备注
            ms_notewordmatch = new RegexWordmatch("note", new Regex(@"\(.+\)", RegexOptions.Compiled),false);

            //噪音收集器
            ms_noisecollector = new NoiseCollector();
        }

        private static void InitBasicWordmatchChain()
        {
            //默认职责链
            ms_headwordmatch.SetNext(ms_notewordmatch).SetNext(ms_nationwordmatch).SetNext(ms_provincewordmatch)
                .SetNext(ms_citywordmatch).SetNext(ms_countywordmatch).SetNext(ms_districtwordmatch)
                .SetNext(ms_townwordmatch).SetNext(ms_streetwordmatch).SetNext(ms_industrialparkwordmatch)
                .SetNext(ms_villagewordmatch).SetNext(ms_roadwordmatch).SetNext(ms_numberwordmatch)
                .SetNext(ms_zonewordmatch).SetNext(ms_plazawordmatch).SetNext(ms_noisecollector);
        }

        public ChineseAddress Parse(String str)
        {
            str = str.Replace(".", "").Replace("，", "").Replace(",","");
            MatchingString ms = new MatchingString(str);
            ms_headwordmatch.Process(ms);

            ChineseAddress ca = new ChineseAddress();
            ca.source = str;
            ca.nation = ms.GetStringByWordmatch(ms_nationwordmatch);
            ca.province = ms.GetStringByWordmatch(ms_provincewordmatch);
            ca.city = ms.GetStringByWordmatch(ms_citywordmatch);
            ca.district = ms.GetStringByWordmatch(ms_districtwordmatch);
            ca.county = ms.GetStringByWordmatch(ms_countywordmatch);
            if (ms.GetStringByWordmatch(ms_streetwordmatch) != null)
                ca.street = ms.GetStringByWordmatch(ms_streetwordmatch);
            else
                ca.street = ms.GetStringByWordmatch(ms_roadwordmatch);
            ca.number = ms.GetStringByWordmatch(ms_numberwordmatch);
            ca.plaza = ms.GetStringByWordmatch(ms_plazawordmatch);
            ca.ip = ms.GetStringByWordmatch(ms_industrialparkwordmatch);
            ca.town = ms.GetStringByWordmatch(ms_townwordmatch);
            ca.village = ms.GetStringByWordmatch(ms_villagewordmatch);
            //ca.notes = ms.GetStringsByWordmatch(ms_notewordmatch);
            //ca.noises = ms.GetStringsByWordmatch(ms_noisecollector);
            return ca;
        }   

        public ChineseAddressWordParser()
        {
            if (ms_nationwordmatch == null)
            {
                InitBasicWordmatch();
                InitBasicWordmatchChain();
            }
        }

        public ChineseAddressWordParser(string[] citys)
        {
            if (ms_nationwordmatch == null)
                InitBasicWordmatch();

            ms_citywordmatch = NewElementsWordmatch("city", citys);
            InitBasicWordmatchChain();
        }
    }
}
