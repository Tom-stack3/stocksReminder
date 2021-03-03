using System.Collections.Generic;

namespace stocksReminder
{
    class Program
    {
        static void Main(string[] args)
        {
            StocksReminder stockreminder = new StocksReminder();
            if (args.Length == 0)
            {
                stockreminder.RemindAll();
            }
            else
            {
                if (args[0] == "0")
                {
                    stockreminder.RemindAll();
                }
                else if (args[0] == "1")
                {
                    stockreminder.CheckForAlert();
                }
            }
        }

        public class Users
        {
            public List<User> users { get; set; }
        }

        public class Stock
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public double Price { get; set; }
        }

        public class User
        {
            public List<string> EmailAdresses { get; set; }
            public List<Stock> Stocks { get; set; }
        }

        public class Point
        {
            public string D_p { get; set; }
            public double C_p { get; set; }
            public double V_p { get; set; }
            public int T_p { get; set; }
        }

        public class RootObject
        {
            public string paperId { get; set; }
            public string paperName { get; set; }
            public List<Point> points { get; set; }
            public string baseRate { get; set; }
        }
    }
}
