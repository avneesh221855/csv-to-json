using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public class Jsonfirst
    {
        public string IndicatorName { get; set; }
        public string year { get; set; }
        public string value { get; set; }
    }
    public class Jsonthird
    {
        public string year { get; set; }
        public string IndicatorName { get; set; }
        public string Countrycode { get; set; }
        public string countryname { get; set; }
        public double value { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            FileStream sample = new FileStream(@"D:\Indicators.csv", FileMode.Open, FileAccess.Read);
            FileStream part1 = new FileStream(@"D:\output1.json", FileMode.OpenOrCreate, FileAccess.Write);
            FileStream part2 = new FileStream(@"D:\output2.json", FileMode.OpenOrCreate, FileAccess.Write);
            FileStream part3 = new FileStream(@"D:\output3.json", FileMode.OpenOrCreate, FileAccess.Write);
            StreamReader sample1 = new StreamReader(sample);
            StreamWriter firstJson = new StreamWriter(part1);
            StreamWriter secondJson = new StreamWriter(part2);
            StreamWriter thirdJson = new StreamWriter(part3);
            string[] countrycode = { "AFG", "ARM", "AZE", "BHR", "BGD", "BTN", "BRN", "KHM", "CHN", "CXR", "CCK", "IOT", "GEO", "HKG", "IND", "IDN", "IRN", "IRQ", "ISR", "JPN", "JOR", "KAZ", "KWT", "KGZ", "LAO", "LBN", "MAC", "MYS", "MDV", "MNG", "MMR", "NPL", "PRK", "OMN", "PAK", "PHL", "QAT", "SAU", "SGP", "KOR", "LKA", "SYR", "TWN", "TJK", "THA", "TUR", "TKM", "ARE", "UZB", "VNM", "YEM" };
            List<Jsonthird> Jsonchart = new List<Jsonthird>();
            List<Jsonfirst> first = new List<Jsonfirst>();
            string[] data = sample1.ReadLine().Split(',');// for  storing key in a string array . 
            sample1.ReadLine(); 
            secondJson.Write("[" + "\n");
            while (!sample1.EndOfStream)
            {
                string[] data1 = sample1.ReadLine().Split(',');// for storing values in a string array.
                if (data1[0] == "India" && (data1[2] == "Rural population (% of total population)" || data1[2] == "Urban population (% of total)"))
                {
                   first.Add(new Jsonfirst() { year = data1[4], IndicatorName = data1[2], value = data1[5] });
                }
                if (data1[0] == "India" && (data1[2] == "Urban population growth (annual %)"))// writing data into 2nd json
                {
                    secondJson.Write("{" + "\n" + "\"" + data[4] + "\"" + ":" + data1[4] + "," + "\n" + "\"" + "urban" + "\"" + ":" + data1[5] + "\n" + "}");
                    string y = (data1[4] == "2014" && data1[2] == "Urban population growth (annual %)") ? "]" : "," + "\n";
                    secondJson.Write(y);
                    secondJson.Flush();
                }
                for (int i = 0; i < countrycode.Length; i++)//for asian countries
                {
                    if ((countrycode[i] == data1[1]) && (data1[2] == "Urban population" || data1[2] == "Rural population"))
                    {
                        double temp;
                        double.TryParse(data1[5], out temp);
                        Jsonchart.Add(new Jsonthird() { year = data1[4], IndicatorName = data1[2], value = temp, Countrycode = data1[1], countryname = data1[0] });
                    }
                }
            }
                    var query = 
                    from Jsonfirst in first
                    group new { Jsonfirst.IndicatorName, Jsonfirst.value } by Jsonfirst.year into yeargroup
                    select yeargroup;
            firstJson.Write("{" + "\n" + "\"" + "India" + "\"" + ":" + "[");// writing data into first json
            foreach (var a in query)
            {
                firstJson.WriteLine("\n" + "{" + "\"" + "year" + "\"" + ":" + a.Key + ",");
                string temp = "";
                string j = "";
                foreach (var item in a)
                {
                    if ("Rural population (% of total population)" == item.IndicatorName)
                    { j = "rural"; }
                    else j = "urban";
                    temp = temp + "\"" + j + "\":" + item.value + ",";
                }
                temp = temp.Remove(temp.Length - 1);
               
                firstJson.WriteLine(temp);
                
                if (a.Key != "2014")
                    firstJson.WriteLine("},");
                else
                    firstJson.WriteLine("}");
                firstJson.Flush();
            }
            firstJson.Write("]" + "\n" + "}");
            firstJson.Flush();
            var val = from m in Jsonchart group new { m.value, m.IndicatorName } by m.countryname into cntry from n in (from m in cntry group new { m.value } by m.IndicatorName into xyz select new { xyz.Key, sum = xyz.Sum(o => o.value) }) group n by cntry.Key;
            thirdJson.WriteLine("["+"\n");
            foreach (var i in val)
            {
                thirdJson.Write("{"+"\"" + "CountryCode" + "\"" + ":" + "\"" + i.Key+ "\""+""+","+ "\n");
                foreach (var j in i)
                {
                    var r = j.Key == "Urban population" ?  "\"" + "Urban" + "\"" + ":" + j.sum : "\"" + "Rural" + "\"" + ":" + j.sum + ",";
                    thirdJson.WriteLine(r);
                }
                var res = i.Key == "Vietnam" ? "}" : "},";
                 thirdJson.WriteLine(res);
            }
            thirdJson.WriteLine("]");
            thirdJson.Flush();
        }
    }
}
