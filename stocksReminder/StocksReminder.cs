using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using static stocksReminder.Program;

namespace stocksReminder
{
    class StocksReminder
    {
        //stock reminder
        private const int NUM_OF_DAYS_IN_GRAPH = 7;
        public List<Day> Days = new List<Day>();
        private List<double> revenues = new List<double>();
        private List<string> revenuesToPrint = new List<string>();
        private List<string> imageUrls = new List<string>();
        private string emailSubject = "";
        private string emailBody = "";

        private Users users;

        //alert check
        private List<string> AlertedStocksNames = new List<string>();
        private List<double> AlertedStocksPrecentage = new List<double>();
        private double CHANGE_TO_ALERT_ON = double.Parse(ConfigurationManager.AppSettings["ChangeToAlertOn"]);

        /*
         * here we check for each stock of each user the following:
         * if the stock price got up or down in the last two days by the percentage set in CHANGE_TO_ALERT_ON.
        */
        public void CheckForAlert()
        {
            SetUsers();
            foreach (User user in users.users)
            {
                Days.Clear();
                AlertedStocksNames.Clear();
                AlertedStocksPrecentage.Clear();
                emailBody = String.Empty;

                // we go over the user's stocks
                for (int i = 0; i < user.Stocks.Count; i++)
                {
                    string stock = user.Stocks[i].Id;

                    // we get the history of the stock price
                    SetDays(user.Stocks[i].Id, 2);

                    // we check if the the stock got up or down in the last two days by the percentage set in CHANGE_TO_ALERT_ON.
                    if ((Days[1].price * (1 + (CHANGE_TO_ALERT_ON / 100)) <= Days[0].price) || (Days[1].price * (1 - (CHANGE_TO_ALERT_ON / 100)) >= Days[0].price))
                    {
                        AlertedStocksNames.Add(user.Stocks[i].Name);
                        double percentage = ((Days[0].price / Days[1].price) - 1) * 100;
                        percentage = Math.Round(percentage, 2);
                        AlertedStocksPrecentage.Add(percentage);
                    }
                }
                // if we found some stocks to alert on, we send an email
                if (AlertedStocksNames.Count > 0)
                {
                    SetEmailSubject("Stocks alert☢️☢️☢️");
                    SetEmailBodyAlert();
                    SendEmail(user.EmailAdresses); //Sends the alert
                }
            }
        }

        private void SetEmailBodyAlert()
        {
            string fellOrRisen = "";
            for (int i = 0; i < AlertedStocksNames.Count; i++)
            {
                if (AlertedStocksPrecentage[i] > 0)
                {
                    fellOrRisen = "rose";
                }
                else if (AlertedStocksPrecentage[i] < 0)
                {
                    fellOrRisen = "fell";
                }
                emailBody = emailBody + AlertedStocksNames[i] + " " + fellOrRisen + " by " + " " + AlertedStocksPrecentage[i] + "% from yesterday to today. <br/>";
            }
        }

        private void SetUsers()
        {
            // we get the users.json file
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"users.json");
            string json = File.ReadAllText(path);
            users = JsonConvert.DeserializeObject<Users>(json);
        }

        private void SetDays(string symbol, int numOfDays)
        {
            // if the symbol is in digits, it is an Israeli stock.
            if (symbol.All(char.IsDigit))
            {
                Bizportal bizportal = new Bizportal(symbol);
                bizportal.GetDays(numOfDays);
                Company company = bizportal.company;
                Days = company.historical;
            }
            //else, NASDAQ stock
            else
            {
                FinnhubAPI finnhubAPI = new FinnhubAPI();
                Company company = finnhubAPI.GetPrices(symbol, numOfDays);
                Days = company.historical;
            }
        }


        public void RemindAll()
        {
            SetUsers();
            // IXIC is the Nasdaq Composite, it used to worked,
            // but Finnhub made it a premium feature to get the prices of this index.
            // So instead I check and compare the user's stocks to NDX index, the Nasdaq-100 Market index

            //List<Day> nasdaqDays = new FinnhubAPI().GetPrices("^IXIC", NUM_OF_DAYS_IN_GRAPH).historical;

            List<Day> nasdaqDays = new FinnhubAPI().GetPrices("NDX", NUM_OF_DAYS_IN_GRAPH).historical;

            string nasdaqName = "NASDAQ-100";
            foreach (User user in users.users)
            {
                Days.Clear();
                revenues.Clear();
                revenuesToPrint.Clear();
                imageUrls.Clear();
                emailBody = String.Empty;

                for (int i = 0; i < user.Stocks.Count; i++)
                {
                    // here we get the prices of the stock in the last NUM_OF_DAYS_IN_GRAPH days.
                    // we use the finnhub API for NASDAQ stocks, and Bizportal site for Israeli stocks.
                    SetDays(user.Stocks[i].Id, NUM_OF_DAYS_IN_GRAPH);

                    // here we generate the image representing the stock's prices compared to the NASDAQ-100 prices.
                    // an example of a url generated:
                    //https://chart.googleapis.com/chart?cht=lc&chs=300x225&chd=t:47.74,48.34,49.6,48.25,47.94,48.06,47.93|13637.5,13580.7998,13223.7002,13194.7002,13302.2002,12828.2998,12909.4004&chxt=x,y&chds=47.1642970561072,50.1757029438929,12186.88481,14278.91499&chxr=1,47.1642970561072,50.1757029438929100&chxl=0:|02-23|02-24|02-25|02-26|02-27|02-28|03-01&chco=3072F3,ff0000&chm=D,0999FF,0,0,2&chg=14.2857142857143,10&chtt=DAL+1&chdl=DAL+1|NASDAQ-100&chdlp=b
                    imageUrls.Add(new Image().CreateURLforTwoLineImage(user.Stocks[i].Name.Replace(" ", "+"), Days, NUM_OF_DAYS_IN_GRAPH, nasdaqDays, nasdaqName));

                    // we calcuate the revenue the stock has made. we know the buying price from the users.json, and the latest price of the stock.
                    CalculateRevenue(user.Stocks[i].Price);
                }
                SetEmailSubject("Stocks state");
                SetEmailBody(user.Stocks.Count);
                SendEmail(user.EmailAdresses);
            }
        }

        private void SetEmailSubject(string subject)
        {
            emailSubject = subject + " " + DateTime.Today.ToString("dd/MM/yyyy");
        }

        private void SetEmailBody(int numOfStocks)
        {
            for (int i = 0; i < numOfStocks; i++)
            {
                emailBody = emailBody + "<h3>The revenue is: " + revenuesToPrint[i] + "% </h3> <img src=\" " + imageUrls[i] + " \">" + "<br/> <br/>";
            }
        }

        private void CalculateRevenue(double PriceBought)
        {
            double currentRevenue = ((Days[0].price / PriceBought) - 1) * 100;
            revenues.Add(currentRevenue);
            revenuesToPrint.Add(Math.Round(currentRevenue, 2).ToString());
        }

        private void SendEmail(List<string> emailAdresses)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    string fromMail = ConfigurationManager.AppSettings["MailFromAddress"];
                    string fromMailPass = ConfigurationManager.AppSettings["MailFromPass"];

                    mail.From = new MailAddress(fromMail);
                    foreach (string mailAdress in emailAdresses)
                    {
                        mail.To.Add(mailAdress);
                    }
                    mail.Subject = emailSubject;
                    mail.Body = emailBody;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(fromMail, fromMailPass);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                        Console.WriteLine("Email sent successfully");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
