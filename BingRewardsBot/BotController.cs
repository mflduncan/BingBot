using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BingRewardsBot
{
    class BotController
    {
        string username, password;
        List<string> queries;
        int mode;
        public static double progress = 0;
        BingBot bot;
        static string status = "Starting...";
      
        public BotController(string username, string password, List<string> queries, int mode)
        {
            this.username = username;
            this.password = password;
            this.queries = queries;
            this.mode = mode;
            status = "Initializing webdriver...";
            bot = new BingBot(mode);
        }

        public void start()
        {
            status = "Logging in to " + username + " in mode " + mode.ToString() + "... ";
            login();
            if (!check())
            {
                progress += 30;
                bot.quit();
                return;
            }
            progress += 3;
            for (int i = 0; i < 3; i++)
            {
                status = "Waiting (" + Convert.ToString(i) + "/3)...";
                Thread.Sleep(3000);
                progress += 2;
            }
            perform_searches();
            status = "Done";
        }
        public void login()
        {
            bot.login(username, password);
        }
        public bool check()
        {
            if (bot.check_bing_login())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void perform_searches()
        {
            //Perform each query in the list
            double increment = (double)20 / queries.Count();
            for (int i = 0; i < queries.Count(); i++)
            {
                status = "Performing searches... (" + (i + 1).ToString() + "/" + queries.Count().ToString() + ")";
                string query = queries[i];
                bot.search(query.ToLower());
                progress += increment;
            }
            status = "Closing driver... ";
            bot.quit();
            progress += 1;
        }

        public void quit()
        {
            bot.quit();
        }
    }
}
