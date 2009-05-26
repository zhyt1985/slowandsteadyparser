using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public struct ChineseAddress
    {
        public string source;
        public string nation;
        public string province;
        public string city;
        public string county;
        public string district;
        public string street;
        public IList<string> roads;
        public string number;
        public string plaza;
        public string ip;
        public string town;
        public string village;
        public string zone;
        public string underground;
        public IList<string> notes;
        public IList<string> noises;

        public override string ToString()
        {
            string s = "src: " + source + Environment.NewLine;
            if (nation != null) s = s + "nat: " + nation + Environment.NewLine;
            if (province != null) s = s + "pro: " + province + Environment.NewLine;
            if (city != null) s = s + "cit: " + city + Environment.NewLine;
            if (county != null) s = s + "cou: " + county + Environment.NewLine;
            if (district != null) s = s + "dis: " + district + Environment.NewLine;            
            if (street != null) s = s + "str: " + street + Environment.NewLine;
            if (number != null) s = s + "num: " + number + Environment.NewLine;
            if (plaza != null) s = s + "pla: " + plaza + Environment.NewLine;
            if (ip != null) s = s + "idp: " + ip + Environment.NewLine;
            if (town != null) s = s + "twn: " + town + Environment.NewLine;
            if (village != null) s = s + "vil: " + village + Environment.NewLine;
            if (zone != null) s = s + "zon: " + zone + Environment.NewLine;
            if (underground  != null) s = s + "udg: " + underground + Environment.NewLine;
            if (roads!= null)
            {
                s = s + "rod: ";
                foreach (string r in roads)
                    if (r == roads[0])
                        s = s + r;
                    else
                        s = s + @" \ " + r;
                s = s + Environment.NewLine;
            }
            if (notes != null)
            {
                s = s + "not: ";
                foreach (string n in notes)
                    if (n == roads[0])
                        s = s + n;
                    else
                        s = s + @" \ " + n;
                s = s + Environment.NewLine;
            }
            if (noises != null)
            {
                s = s + "noi: ";
                foreach (string n in noises)
                    s = s + n + @" \ ";
                s = s + Environment.NewLine;
            }
            return s;
        }
    }
}
