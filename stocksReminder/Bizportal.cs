using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using static stocksReminder.Program;

namespace stocksReminder
{
    public class Bizportal
    {
        private string stockUrl;
        private List<Day> Days = new List<Day>();
        public Company company = new Company();
        public string stockName;

        public Bizportal(string stockUrl)
        {
            this.stockUrl = new AppSettingsReader().GetValue("bizportalUrlAPI", typeof(string)) + stockUrl;
        }

        public void GetDays(int NUM_OF_DAYS_IN_GRAPH)
        {
            SaveDaysFromJSON(LoadJson(GetJson()), NUM_OF_DAYS_IN_GRAPH);
            company.historical = Days;
        }

        private string GetJson()
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "yalla-maccabi");
            string json = webClient.DownloadString(stockUrl);
            return json;
        }

        private RootObject LoadJson(string stringJson)
        {
            RootObject theJsonDeserialized = JsonConvert.DeserializeObject<RootObject>(stringJson);
            string name = theJsonDeserialized.paperName;
            //EncodeAndSaveName(name);
            return theJsonDeserialized;
        }

        private void EncodeAndSaveName(string name)
        {
            Encoding latinEncoding = Encoding.GetEncoding("Windows-1252");
            Encoding hebrewEncoding = Encoding.GetEncoding("Windows-1255");

            byte[] latinBytes = latinEncoding.GetBytes(name);

            string hebrewString = hebrewEncoding.GetString(latinBytes);
            stockName = hebrewString;
            company.symbol = stockName;
        }

        private void SaveDaysFromJSON(RootObject theJsonDeserialized, int NUM_OF_DAYS_IN_GRAPH)
        {
            int i = 1;
            foreach (Point point in theJsonDeserialized.points)
            {
                if (i <= NUM_OF_DAYS_IN_GRAPH)
                {
                    Day day = new Day(point.C_p, point.D_p);
                    Days.Add(day);
                    i++;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
