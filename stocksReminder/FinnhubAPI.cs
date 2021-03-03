using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;

namespace stocksReminder
{
    public class FinnhubAPI
    {
        // this is the API I use for getting the history of the stock.
        // your'e more than welcome to explore the API documantation here:
        // https://finnhub.io/docs/api/

        public string apiUrl;
        private string stringJson;
        public Company company = new Company();

        private void SetJson()
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "yalla-maccabi");
            stringJson = webClient.DownloadString(apiUrl);
        }

        private FinnhubCompanyTarget LoadJsonToFinnhubCompanyTarget()
        {
            FinnhubCompanyTarget finnhubCompanyTarget = JsonConvert.DeserializeObject<FinnhubCompanyTarget>(stringJson);
            return finnhubCompanyTarget;
        }

        public FinnhubCompanyTarget GetCompanyTarget(string symbol)
        {
            apiUrl = @"https://finnhub.io/api/v1/stock/price-target?symbol=" + symbol + "&token=" + ConfigurationManager.AppSettings["FinnhubAPIkey"];
            SetJson();
            return LoadJsonToFinnhubCompanyTarget();
        }

        public Company GetPrices(string symbol, int numOfDaysToGoBack)
        {
            CreateURLforGettingPrices(symbol, numOfDaysToGoBack);
            SetJson();
            FinnhubCompany finnhubCompany = LoadJsonToFinnhubCompany();
            company = LoadFinnhubCompanyToCompany(symbol, finnhubCompany, numOfDaysToGoBack);
            return company;
        }

        private Company LoadFinnhubCompanyToCompany(string symbol, FinnhubCompany finnhubCompany, int numOfDaysToGoBack)
        {
            Company company = new Company();
            company.symbol = symbol;
            company.historical = CreateListOfDays(finnhubCompany, numOfDaysToGoBack);

            return company;
        }

        private List<Day> CreateListOfDays(FinnhubCompany finnhubCompany, int numOfDaysToGoBack)
        {
            List<Day> days = new List<Day>();
            List<double> longPrices = finnhubCompany.c;
            List<double> prices = new List<double>();
            longPrices.Reverse(); //so the days will be in the needed order, the latest date first
            for (int i = 0; i < numOfDaysToGoBack; i++)
            {
                prices.Add(longPrices[i]);
            }
            List<string> dates = new List<string>();
            for (int i = 1; i <= prices.Count; i++)
            {
                DateTime day = DateTime.Today.AddDays(-i);

                dates.Add(day.ToString("yyyy-MM-dd"));
            }

            for (int i = 0; i < prices.Count; i++)
            {
                days.Add(new Day(prices[i], dates[i]));
            }

            return days;
        }

        private FinnhubCompany LoadJsonToFinnhubCompany()
        {
            FinnhubCompany finnhubCompany = JsonConvert.DeserializeObject<FinnhubCompany>(stringJson);
            return finnhubCompany;
        }

        private void CreateURLforGettingPrices(string symbol, int numOfDaysToGoBack)
        {
            Int32 todaysTimestamp = ConvertToTimestamp(DateTime.UtcNow);
            Int32 theLatterTimestamp = ConvertToTimestamp(DateTime.Today.AddDays(-numOfDaysToGoBack - 90));

            apiUrl = @"https://finnhub.io/api/v1/stock/candle?symbol=" + symbol + "&resolution=D&from=" + theLatterTimestamp + "&to=" + todaysTimestamp + "&token=" + ConfigurationManager.AppSettings["FinnhubAPIkey"];
        }

        private Int32 ConvertToTimestamp(DateTime value)
        {
            Int32 unixTimestamp = (Int32)(value.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }


    }

    public class FinnhubCompany
    {
        public List<double> c { get; set; }
        public List<double> h { get; set; }
        public List<double> l { get; set; }
        public List<double> o { get; set; }
        public string s { get; set; }
        public List<double> t { get; set; }
        public List<double> v { get; set; }
    }

    public class Company
    {
        public string symbol { get; set; }
        public List<Day> historical = new List<Day>(); //the latest date (today) is the first one in the array

        public List<double> ReturnPrices(int numOfPricesToReturn)
        {
            List<double> prices = new List<double>();
            for (int i = 0; i < numOfPricesToReturn; i++)
            {
                prices.Add(historical[i].price);
            }
            return prices;
        }
    }

    public class Day
    {
        public double price;
        public string date; //if not in Bizportal (means an Israeli stock), the format is YYYY-MM-DD

        public Day(double price, string date)
        {
            this.price = price;
            this.date = date;
        }
    }

    public class FinnhubCompanyTarget
    {
        public string lastUpdated { get; set; }
        public string symbol { get; set; }
        public double targetHigh { get; set; }
        public double targetLow { get; set; }
        public double targetMean { get; set; }
        public double targetMedian { get; set; }
    }
}
