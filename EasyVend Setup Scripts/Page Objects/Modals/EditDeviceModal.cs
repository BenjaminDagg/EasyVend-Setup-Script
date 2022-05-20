using NUnit.Framework;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
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
    internal class EditDeviceModal : AddDeviceModal
    {

        [FindsBy(How = How.Id, Using = "SiteName")]
        protected IWebElement SiteNameField;

        [FindsBy(How = How.Id, Using = "DeviceId")]
        protected IWebElement DeviceIdField;

        public EditDeviceModal(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }


        public override void WaitForModal()
        {

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("SiteName")));
        }


        public string GetDeviceIdField()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("DeviceId")));
            return DeviceIdField.GetAttribute("value");
        }


        public bool DeviceMatches(DeviceTableRecord device, SiteTableRecord site)
        {
            WaitForModal();

            int deviceId = int.Parse(GetDeviceIdField());
            string siteName = GetSiteNameField();

            return (deviceId == device.DeviceId && siteName == site.SiteName);
        }
    }
}
