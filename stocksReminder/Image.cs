using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace stocksReminder
{
    public class Image
    {
        private double lowestPrice, highestPrice, lowestNasdaqPrice, highestNasdaqPrice, stockFloor, stockCeiling, nasdaqFloor, nasdaqCeiling;

        //regular one line stock price chart
        public static string CreateURLforImage(string name, List<Day> Days, int NUM_OF_DAYS_IN_GRAPH)
        {
            string url = "";

            url = @"https://chart.googleapis.com/chart?cht=lc&chs=300x200&chd=t:";

            double highestPrice = findMaxPriceInList(Days);
            double lowestPrice = findMinPriceInList(Days);

            double stockPercentage = findPercentage(lowestPrice, highestPrice);

            for (int i = NUM_OF_DAYS_IN_GRAPH - 1; i >= 0; i--)
            {
                url = url + Days[i].price.ToString();
                if (i != 0)
                    url = url + ",";
            }

            url = url + "&chxt=x,y&chds=" + lowestPrice * 0.95 + "," + highestPrice * 1.05 + "&chxr=1," + lowestPrice * 0.95 + "," + highestPrice * 1.05 + "100&chxl=0:|";

            for (int i = NUM_OF_DAYS_IN_GRAPH - 1; i >= 0; i--)
            {
                string currentDate = Regex.Replace(Days[i].date, @"\/\d\d\d\d", String.Empty);
                currentDate = Regex.Replace(currentDate, @"\d\d\d\d\-", String.Empty);

                if (i != 0)
                    url = url + currentDate + "|";
                else if (i == 0)
                    url = url + currentDate;
            }

            url = url + "&chm=D,0999FF,0,0,2&chg=" + (100.0 / NUM_OF_DAYS_IN_GRAPH) + ",10&chtt=" + name;

            return url;
        }

        // two line chart. for comparing of two lines of stock prices
        public string CreateURLforTwoLineImage(string name, List<Day> Days, int NUM_OF_DAYS_IN_GRAPH, List<Day> NasdaqDays, string nasdaqName)
        {
            string url = "";

            // an example of a url generated:
            // https://chart.googleapis.com/chart?cht=lc&chs=300x225&chd=t:47.74,48.34,49.6,48.25,47.94,48.06,47.93|13637.5,13580.7998,13223.7002,13194.7002,13302.2002,12828.2998,12909.4004&chxt=x,y&chds=47.1642970561072,50.1757029438929,12186.88481,14278.91499&chxr=1,47.1642970561072,50.1757029438929100&chxl=0:|02-23|02-24|02-25|02-26|02-27|02-28|03-01&chco=3072F3,ff0000&chm=D,0999FF,0,0,2&chg=14.2857142857143,10&chtt=DAL+1&chdl=DAL+1|NASDAQ-100&chdlp=b

            url = @"https://chart.googleapis.com/chart?cht=lc&chs=300x225&chd=t:";

            highestPrice = findMaxPriceInList(Days);
            lowestPrice = findMinPriceInList(Days);

            double stockPercentage = findPercentage(lowestPrice, highestPrice);

            for (int i = NUM_OF_DAYS_IN_GRAPH - 1; i >= 0; i--)
            {
                url = url + Days[i].price.ToString();
                if (i != 0)
                    url = url + ",";
            }

            url = url + "|";

            highestNasdaqPrice = findMaxPriceInList(NasdaqDays);
            lowestNasdaqPrice = findMinPriceInList(NasdaqDays);

            double nasdaqPercentage = findPercentage(lowestNasdaqPrice, highestNasdaqPrice);

            calculateFloorsOfTwoLines(stockPercentage, nasdaqPercentage);

            for (int i = NUM_OF_DAYS_IN_GRAPH - 1; i >= 0; i--)
            {
                url = url + NasdaqDays[i].price.ToString();
                if (i != 0)
                    url = url + ",";
            }

            url = url + "&chxt=x,y&chds=" + stockFloor + "," + stockCeiling + "," + nasdaqFloor + "," + nasdaqCeiling + "&chxr=1," + stockFloor + "," + stockCeiling + "100&chxl=0:|";

            for (int i = NUM_OF_DAYS_IN_GRAPH - 1; i >= 0; i--)
            {
                string currentDate = Regex.Replace(Days[i].date, @"\/\d\d\d\d", String.Empty);
                currentDate = Regex.Replace(currentDate, @"\d\d\d\d\-", String.Empty);

                if (i != 0)
                    url = url + currentDate + "|";
                else if (i == 0)
                    url = url + currentDate;
            }

            url = url + "&chco=3072F3,ff0000&chm=D,0999FF,0,0,2&chg=" + (100.0 / NUM_OF_DAYS_IN_GRAPH) + ",10&chtt=" + name;
            url = url + "&chdl=" + name + "|" + nasdaqName + "&chdlp=b";

            return url;
        }

        private double calculateFloorsOfTwoLines(double stockPercentage, double nasdaqPercentage)
        {
            if (stockPercentage < nasdaqPercentage)
            {
                nasdaqFloor = lowestNasdaqPrice * 0.95;
                nasdaqCeiling = highestNasdaqPrice + lowestNasdaqPrice * 0.05;
                double marginFromFloorToCeiling = (nasdaqPercentage / stockPercentage) * (highestPrice - lowestPrice);
                double middleOfSmallerPrices = (highestPrice + lowestPrice) / 2;
                stockCeiling = marginFromFloorToCeiling / 2 + middleOfSmallerPrices;
                stockFloor = middleOfSmallerPrices - marginFromFloorToCeiling / 2;
                return stockCeiling;
            }
            else
            {
                stockFloor = lowestPrice * 0.95;
                stockCeiling = highestPrice + lowestPrice * 0.05;
                double marginFromFloorToCeiling = (stockPercentage / nasdaqPercentage) * (highestNasdaqPrice - lowestNasdaqPrice);
                double middleOfSmallerPrices = (highestNasdaqPrice + lowestNasdaqPrice) / 2;
                nasdaqCeiling = marginFromFloorToCeiling / 2 + middleOfSmallerPrices;
                nasdaqFloor = middleOfSmallerPrices - marginFromFloorToCeiling / 2;
                return nasdaqCeiling;
            }
        }

        public double CalculateAxis(double low1, double high1, double low2, double high2)
        {
            lowestPrice = low1;
            highestPrice = high1;
            lowestNasdaqPrice = low2;
            highestNasdaqPrice = high2;
            return calculateFloorsOfTwoLines(findPercentage(low1, high1), findPercentage(low2, high2));
        }

        private static double findMaxPriceInList(List<Day> list)
        {
            double max = list[0].price;
            foreach (Day day in list)
            {
                if (day.price > max)
                {
                    max = day.price;
                }
            }
            return max;
        }
        private static double findMinPriceInList(List<Day> list)
        {
            double min = list[0].price;
            foreach (Day day in list)
            {
                if (day.price < min)
                {
                    min = day.price;
                }
            }
            return min;
        }

        private static double findPercentage(double first, double second)
        {
            double percentage = ((second / first) - 1) * 100;
            return percentage;
        }
    }
}
