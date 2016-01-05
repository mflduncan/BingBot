using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace BingRewardsBot
{
    class BingBot
    {
        public const int MOBILE = 1, DESKTOP = 0;   
        IWebDriver browser;

        public BingBot(int mode)
        {
            PhantomJSOptions options = new PhantomJSOptions();
            if (mode == 1)
            {
                options.AddAdditionalCapability("phantomjs.page.settings.userAgent", "Mozilla/5.0 (Android; Mobile; rv:30.0) Gecko/30.0 Firefox/30.0");
            }
            else
            {
                options.AddAdditionalCapability("phantomjs.page.settings.userAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Safari/537.36 Edge/13.10586 ");
            }
            PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();
            service.AddArgument("--ignore-ssl-errors=true");
            //arguments for proxy
            service.AddArgument("--proxy=127.0.0.1:9050");
            service.AddArgument("--proxy-type=socks5");
            service.HideCommandPromptWindow = true;

            browser = new PhantomJSDriver(service, options);
        }

        public string getIP()
        {
            string currUrl = browser.Url;
            browser.Navigate().GoToUrl("http://api.ipify.org");

            string ip = browser.PageSource;
            //MatchCollection mc = Regex.Matches(ip, "\d.*\d");

            browser.Navigate().GoToUrl(currUrl);
            return ip;
            
        }
        public void login(string username, string password)
        {
            IWebElement element;
            TimeSpan ts = new TimeSpan(0, 0, 10);

            browser.Navigate().GoToUrl(Properties.Settings.Default.LoginSite);
            browser.Manage().Timeouts().ImplicitlyWait(ts);
            element = browser.FindElement(By.Name(Properties.Settings.Default.UsernameBox));
            element.SendKeys(username);
            element = browser.FindElement(By.Name(Properties.Settings.Default.PasswordBox));
            element.SendKeys(password + Keys.Return);
        }
        public void search(string query)
        {
            IWebElement element;

            browser.Navigate().GoToUrl("http://www.bing.com");
            element = browser.FindElement(By.Name(Properties.Settings.Default.SearchBox));
            element.Clear();
            element.SendKeys(query + Keys.Return);
        }

        public bool check_bing_login()
        {
            IWebElement element;
            try
            {
                element = browser.FindElement(By.Id("id_n"));
                if (element.Text != "Sign in") { return true; }
                else { return false; }
            }
            catch
            {
                return false;
            }
        }

        public bool check_login_redirect()
        {
            //return (browser.Title == Properties.Settings.Default.LoginRedirect);
            try
            {
                TimeSpan ts = new TimeSpan(0, 0, Properties.Settings.Default.SleepTime);
                browser.Manage().Timeouts().ImplicitlyWait(ts);
                browser.FindElement(By.Name(Properties.Settings.Default.SearchBox));
                return true;
            }
            catch
            {
                try //if it is bitching about new account info
                {
                    IWebElement element;
                    element = browser.FindElement(By.Id("iLandingViewAction"));
                    element.Click();
                    TimeSpan ts = new TimeSpan(0, 0, 10);
                    browser.Manage().Timeouts().ImplicitlyWait(ts);
                    browser.FindElement(By.Name(Properties.Settings.Default.SearchBox));
                    return true;
                }
                catch
                {
                    Screenshot ss = ((ITakesScreenshot)browser).GetScreenshot();
                    ss.SaveAsFile("screenshot.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    return false;
                }
            }
        }

        public void getEarnExploreID()
        {
           browser.Navigate().GoToUrl("https://www.bing.com/rewards/dashboard"); //go to bing rewards dashboard

            //waitForRewards();
            string source = browser.PageSource;
            MatchCollection mc = Regex.Matches(source, "rewardsapp.*?id=\"(.*?)\"");
            //Console.WriteLine(mc[0].Groups[1].ToString());

            for (int i = 0; i < mc.Count; i++)
            {
                browser.Navigate().GoToUrl("https://www.bing.com/rewards/dashboard");
                string id = mc[i].Groups[1].ToString();
                IWebElement e = browser.FindElement(By.Id(id));
                e.Click();
                //Thread.Sleep(Properties.Settings.Default.SleepTime);
            }

        }

        public bool waitForRewards()
        {
            browser.Navigate().GoToUrl("https://www.bing.com/rewards/dashboard");
            bool loaded = false;
            for (int i = 0; i < 5 && !loaded; i++)
            {
                TimeSpan ts = new TimeSpan(0, 0, 3*(i+1)); //wait for up to 60 seconds
                browser.Manage().Timeouts().ImplicitlyWait(ts);
                loaded = true;
                try
                {
                    browser.Navigate().Refresh();
                    browser.FindElement(By.Id("user-status"));
                    return true;
                }
                catch
                {
                    loaded = false;
                    //Console.WriteLine("One down. Attempting again...");
                }              
            }
            return false;
        }
        public void quit()
        {
            browser.Quit();
        }

    }
}
