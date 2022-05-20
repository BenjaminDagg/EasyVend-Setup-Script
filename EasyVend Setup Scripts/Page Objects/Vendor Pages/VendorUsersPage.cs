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

//https://lkrnac.net/blog/2016/10/component-object-pattern-example/
//https://stackoverflow.com/questions/43362865/how-to-add-repeatable-sections-to-page-object-model-with-selenium-and-c

namespace EasyVend_Setup_Scripts
{
    internal class VendorUsersPage : UserTablePage
    {

        private WebDriverWait wait;
        public VendorNavMenu EntityTabs { get; set; }

        public Header Header { get; set; }


        [FindsBy(How = How.Id, Using = "tblUserList")]
        private IWebElement table;


        public override IWebElement Table
        {
            get { return table; }
        }

        public VendorUsersPage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            AddUserModal = new AddUserModal(driver);
            EntityTabs = new VendorNavMenu(driver);
            Header = new Header(driver);
        }


    }
}
