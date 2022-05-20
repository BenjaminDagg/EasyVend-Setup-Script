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
    internal class SiteNavMenu : EntityNavMenu
    {
        [FindsBy(How = How.Id, Using = "device-tab")]
        public IWebElement DeviceTab;

        [FindsBy(How = How.Id, Using = "barcodes-tab")]
        public IWebElement BarcodeTab;

        [FindsBy(How = How.Id, Using = "virtualaccountnumbers-tab")]
        public IWebElement VANTab;

        public SiteNavMenu(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }


        public void ClickDeviceTab()
        {
            WaitForElement(By.Id("device-tab"));
            DeviceTab.Click();
        }


        public void ClickBarcodeTab()
        {
            WaitForElement(By.Id("barcodes-tab"));
            BarcodeTab.Click();
        }


        public void ClickVANTab()
        {
            WaitForElement(By.Id("virtualaccountnumbers-tab"));
            VANTab.Click();
        }
    }
}
