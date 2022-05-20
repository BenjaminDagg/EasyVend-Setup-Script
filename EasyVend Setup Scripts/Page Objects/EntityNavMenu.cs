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
    internal class EntityNavMenu
    {
        protected IWebDriver driver;
        protected WebDriverWait wait;

        [FindsBy(How = How.Id, Using = "details-tab")]
        protected IWebElement DetailsTab;

        [FindsBy(How = How.Id, Using = "users-tab")]
        protected IWebElement UsersTab;


        public EntityNavMenu(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }


        public void WaitForElement(By by)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(by));
        }

        public void ClickDetailsTab()
        {
            WaitForElement(By.Id("details-tab"));
            DetailsTab.Click();
        }

        public void ClickUsersTab()
        {
            WaitForElement(By.Id("users-tab"));
            UsersTab.Click();
        }

    }
}
