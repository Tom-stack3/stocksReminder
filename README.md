# stocksReminder
A C# project for managing your stock purchases.\
It sends a weekly recap for all your stocks.\
For each stock we generate a graph, that shows the progress of the stock compared to the Nasdaq-100.\
and calculates the revenue for each stock.

### Weekly reminder example:
<img src="https://raw.githubusercontent.com/Tom-stack3/stocksReminder/main/images/weekly_email_example.png" alt="" width="300" height="516" />

## How it works:
Every execution, the program reads from the [users.json](/stocksReminder/bin/Debug/users.json).\
For every user we go over his stocks, getting the history of the stock prices using a free stocks API I found on the web: [finnhub api](https://finnhub.io/).\
For Israeli stocks, which don't show up on the API, I use the [bizportal](https://www.bizportal.co.il/) site, to get info about the Israeli stocks.

You are welcome to check those out! 

Than we proccess the stock prices, and create a graph using Google Chart API. Go check it out [here](https://developers.google.com/chart/image/)!\
An example for a graph generated for Amazon stock:
[here](https://chart.googleapis.com/chart?cht=lc&chs=300x225&chd=t:3180.74,3194.5,3159.53,3057.16,3092.93,3146.14,3094.53|13637.5,13580.7998,13223.7002,13194.7002,13302.2002,12828.2998,12909.4004&chxt=x,y&chds=3029.40824021886,3222.25175978114,12186.88481,14278.91499&chxr=1,3029.40824021886,3222.25175978114100&chxl=0:|02-24|02-25|02-26|02-27|02-28|03-01|03-02&chco=3072F3,ff0000&chm=D,0999FF,0,0,2&chg=14.2857142857143,10&chtt=Amazon&chdl=Amazon|NASDAQ-100&chdlp=b)

Finally, the program sends an email to the client with his revenue and a graph for every stock.

## Setup:
### App.config
Before running the program, you need to edit the [App.config](/stocksReminder/App.config).
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
In the App.config, you have a couple of things to change in the `<appSettings>`:
 * Change `MailFromAddress` to a working email, you want the program to send the emails from.
 * Change `MailFromPass` to the matching password for the email address you chose above.
 * Change `FinnhubAPIkey` to your API key. You can get yours on https://finnhub.io/.

Optional:
 * Change `ChangeToAlertOn` to the percentage you want the program to alert on.
