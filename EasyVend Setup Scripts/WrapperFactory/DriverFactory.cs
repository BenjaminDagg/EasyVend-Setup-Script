using NUnit.Framework;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class DriverFactory
    {

        private static IWebDriver driver;

        public static IWebDriver Driver
        {
            get
            {
                if (driver == null)
                {
                    throw new NullReferenceException("IWebDriver instance not initialized.You must first call the InitDriver method");
                }

                return driver;
            }

            private set
            {
                driver = value;
            }
        }

        public static void InitDriver()
        {
            string ChromeDriverPath = ConfigurationManager.AppSettings["ChromeDriverPath"];

            if (driver == null)
            {
                driver = new ChromeDriver(ChromeDriverPath);
            }
        }

        public static void GoToUrl(string url)
        {
            Driver.Url = url;
        }

        public static void CloseDriver()
        {
            driver.Close();
            driver.Quit();
            driver = null;


        }

        public static string GetUrl()
        {
            return Driver.Url;
        }


        public static void Refresh()
        {
            Driver.Navigate().Refresh();
        }

        public static void ExplicitWait(By identifier)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(identifier));
        }
    }
}
