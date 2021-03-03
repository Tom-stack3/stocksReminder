# stocksReminder
A c# project, for managing your stock buyings.\
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




