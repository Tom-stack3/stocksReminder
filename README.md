# stocksReminder
A C# project I made for managing stock purchases, which sends a weekly recap for all stocks interested in.
For each stock we generate a graph, that shows the progress of the stock compared to the Nasdaq-100, and calculates the revenue for each stock.

### Weekly reminder example:
<img src="https://raw.githubusercontent.com/Tom-stack3/stocksReminder/main/images/weekly_email_example.png" alt="" width="300" height="516" />

## How it works:
Every execution, the program reads from the [users.json](/stocksReminder/bin/Debug/users.json).\
For every user we go over his stocks, getting the history of the stock prices using a free stocks API I found on the web: [finnhub api](https://finnhub.io/).\
For Israeli stocks, which don't show up on the finnhub API, I use the [bizportal](https://www.bizportal.co.il/) site, to get info about the Israeli stocks.

You are welcome to check those out! 

**If we run the Weekly reminder:**\
We proccess the stock prices, and create a graph using Google Chart API. Go check it out [here](https://developers.google.com/chart/image/)!\
An example for a graph generated for Amazon stock:
[here](https://chart.googleapis.com/chart?cht=lc&chs=300x225&chd=t:3180.74,3194.5,3159.53,3057.16,3092.93,3146.14,3094.53|13637.5,13580.7998,13223.7002,13194.7002,13302.2002,12828.2998,12909.4004&chxt=x,y&chds=3029.40824021886,3222.25175978114,12186.88481,14278.91499&chxr=1,3029.40824021886,3222.25175978114100&chxl=0:|02-24|02-25|02-26|02-27|02-28|03-01|03-02&chco=3072F3,ff0000&chm=D,0999FF,0,0,2&chg=14.2857142857143,10&chtt=Amazon&chdl=Amazon|NASDAQ-100&chdlp=b)

Finally, the program sends an email to the client with his revenue and a graph for every stock.

**If we run the Alert Check:**\
We proccess the stock prices, and check if the stock prices went up or down by a certain percentage, (set by the user in the `App.config` - explanation bellow).
If it changed by that percentage or more, we send an email to alert the client.

## Setup:
### App.config
Before running the program, you will need to edit the [App.config](/stocksReminder/App.config).
It will look like this: 
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6.1" />
    </startup>
    <appSettings>
	  <!--MailFromAddress: the email address the program sends its email from -->
      <add key="MailFromAddress" value="from_mail_example@gmail.com" />
	  <!--MailFromPass: the password to that email address -->
      <add key="MailFromPass" value="from_mail_pass" />
	  <!--bizportalUrlAPI: to work with Israeli stocks, Don't change!! -->
      <add key="bizportalUrlAPI" value="https://www.bizportal.co.il/forex/quote/ajaxrequests/paperdatagraphjson?period=weekly&amp;paperID=" />
	  <!--ChangeToAlertOn: the change in percentage the program will alert on, in checkForAlert() -->
      <add key="ChangeToAlertOn" value="20" />
	  <!--FinnhubAPIkey: the finnhub api key the program works with. get yours on https://finnhub.io/ -->
      <add key="FinnhubAPIkey" value="finnhub key. get yours on https://finnhub.io/" />
    </appSettings>
</configuration>
```
In the `App.config`, you have a couple of things to change in the `<appSettings>`:
 * Change `MailFromAddress` to a working email, you want the program to send the emails from.
 * Change `MailFromPass` to the matching password for the email address you chose above.
 * Change `FinnhubAPIkey` to your API key. You can get yours on https://finnhub.io/.

Optional:
 * Change `ChangeToAlertOn` to the percentage you want the program to alert on.

### users.json
Before running the program, you will also need to edit the [users.json](/stocksReminder/bin/Debug/users.json).\
This is the file which contains the info about each client:
 1. `"EmailAdresses"`:\
The email addresses to send the Weekly reports and the alerts.\
There can be multipule email addresses for each client. (for each bundle of stocks).
 2. `"Stocks"`: (the client's stocks)\
 	for each stock:
 	* `"Id"`: the stock's symbol.
 	* `"Name"`: the stock's name, the client wants to be shown on the reports.
 	* `"Price"`: the price the client bought the stock originally. (for calculating the revenue)
 
See an example in [users.json](/stocksReminder/bin/Debug/users.json).



