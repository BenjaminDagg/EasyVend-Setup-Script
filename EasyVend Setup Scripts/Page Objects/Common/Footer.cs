using NUnit.Framework;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras;
using SeleniumExtras.PageObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class Footer
    {
        IWebDriver driver;

        [FindsBy(How = How.TagName, Using = "footer")]
        private IWebElement FooterText { get; set; }

        //constructor
        public Footer(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
        }

        public string getFooterText()
        {
            return FooterText.GetAttribute("innerHTML");
        }

        public string parseCopyright()
        {
            string footerText = getFooterText();

            int indexCopyright = footerText.IndexOf('©') + 2;
            string year = footerText.Substring(indexCopyright, 4);

            return year;
        }

        public string parseVersion()
        {
            string footerText = getFooterText();

            string searchString = @"</span>";
            int indexVersionStart = footerText.IndexOf(searchString);

            string version = footerText.Substring(indexVersionStart + searchString.Length);
            version = version.Replace("\r", "").Replace("\n", "");

            return version;
        }

    }
}
