using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using System.Threading;

// This class is headless, meaning the steps aren't shown on screen. Still testing, but so far it works well

namespace BingRewardsBot
{
    class HeadlessBot
    {
        IWebDriver browser;

        public HeadlessBot(int mode)
        {
            PhantomJSOptions options = new PhantomJSOptions();
            if ( mode == 1 )
            {
                options.AddAdditionalCapability("phantomjs.page.settings.userAgent", "Mozilla/5.0 (Android; Mobile; rv:30.0) Gecko/30.0 Firefox/30.0");
            }
            PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();
            service.AddArgument("--ignore-ssl-errors=true");
            service.HideCommandPromptWindow = true;

            browser = new PhantomJSDriver(service, options);
        }
        public void login(string username, string password)
        {
            IWebElement element;

            browser.Navigate().GoToUrl("https://login.live.com/login.srf?wa=wsignin1.0&rpsnv=12&ct=1419757623&rver=6.0.5286.0&wp=MBI&wreply=https:%2F%2Fwww.bing.com%2Fsecure%2FPassport.aspx%3Frequrl%3Dhttp%253a%252f%252fwww.bing.com%252f&lc=1033&id=264960");
            Thread.Sleep(3 * 1000);
            element = browser.FindElement(By.Name("loginfmt"));
            element.SendKeys(username);
            element = browser.FindElement(By.Name("passwd"));
            element.SendKeys(password + Keys.Return);
        }
        public void search(string query)
        {
            IWebElement element;

            if (!browser.Title.Contains("Bing"))
            {
                browser.Navigate().GoToUrl("http://www.bing.com");
            }
            element = browser.FindElement(By.Name("q"));
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

        public bool check_rewards_loaded()
        {
            IWebElement element;
            try
            {
                element = browser.FindElement(By.Id("id_rc"));
                if (Convert.ToInt32(element.Text) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public void quit()
        {
            browser.Quit();
        }
    }
}
