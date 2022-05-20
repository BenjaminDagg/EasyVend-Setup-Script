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
    internal class NavMenu
    {
        IWebDriver driver;
        WebDriverWait wait;

        [FindsBy(How = How.XPath, Using = "//a[@href='/Vendor/Details?entityId=1']")]
        private IWebElement VendorLink { get; set; }

        [FindsBy(How = How.XPath, Using = "//a[@href='/Lottery/Details?entityId=2']")]
        private IWebElement LotteryLink { get; set; }

        [FindsBy(How = How.XPath, Using = "//a[@href='/Site']")]
        private IWebElement SiteLink { get; set; }

        [FindsBy(How = How.XPath, Using = "//a[@href='/Reports?Id=1']")]
        private IWebElement MachineActivityLink;

        [FindsBy(How = How.XPath, Using = "//a[@href='/Reports?Id=3']")]
        private IWebElement TicketSalesLink;

        [FindsBy(How = How.XPath, Using = "//a[@data-original-title='Logout']")]
        private IWebElement LogoutButton { get; set; }

        public ModalPage LogoutModal { get; set; }

        public NavMenu(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            LogoutModal = new ModalPage(driver);
        }

        public void ClickVendor()
        {
            VendorLink.Click();
        }

        public void ClickLottery()
        {
            LotteryLink.Click();
        }

        public void ClickSites()
        {
            SiteLink.Click();
        }

        public void ClickLogout()
        {
            LogoutButton.Click();
        }

        public void ClickMachineActivityReport()
        {
            MachineActivityLink.Click();
        }


        public void ClickTicketSalesReport()
        {
            wait.Until(d => d.FindElement(By.XPath("//a[@href='/Reports?Id=3']")));
            TicketSalesLink.Click();
        }

        public bool vendorIsVisible()
        {
            try
            {
                driver.FindElement(By.XPath("//a[@href='/Vendor/Details?entityId=1']"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool lotteryIsVisible()
        {
            try
            {
                driver.FindElement(By.XPath("//a[@href='/Lottery/Details?entityId=2']"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool siteIsVisible()
        {
            try
            {
                driver.FindElement(By.XPath("//a[@href='/Site']"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public void ClickModalAccept()
        {
            LogoutModal.ClickSubmit();
        }

        public void ClickModalCancel()
        {
            LogoutModal.ClickCancel();
        }

        public void PerformLogout()
        {
            LogoutButton.Click();
            LogoutModal.ClickSubmit();
        }

    }
}
