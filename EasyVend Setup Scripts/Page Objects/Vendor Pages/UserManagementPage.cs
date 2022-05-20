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
    internal class UserManagementPage : UserTablePage
    {
        public VendorNavMenu EntityTabs { get; set; }

        //override table property to use this pages table id
        [FindsBy(How = How.Id, Using = "tblAllUserList")]
        private IWebElement table;

        [FindsBy(How = How.Name, Using = "tblAllUserList_length")]
        private IWebElement tableLengthSelect;

        public Header Header { get; set; }

        public override IWebElement TableLengthSelect { get { return tableLengthSelect; } }


        public override IWebElement Table
        {
            get { return table; }
        }

        public UserManagementPage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            EntityTabs = new VendorNavMenu(driver);
            Header = new Header(driver);
        }


        //override wait for filter method to use this page's id for the filter div
        public override void waitForFilter()
        {
            wait.Until(driver => driver.FindElement(By.Id("tblAllUserList_processing")).GetAttribute("style").Contains("none"));
        }


    }
}
