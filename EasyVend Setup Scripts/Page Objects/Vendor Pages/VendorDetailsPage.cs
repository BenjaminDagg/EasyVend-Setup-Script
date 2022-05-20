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
    internal class VendorDetailsPage : EntityDetailsPage
    {

        public VendorNavMenu EntityTabs { get; set; }
        public static string url = ConfigurationManager.AppSettings["URL"] + "Vendor/Details?entityId=1";

        public VendorDetailsPage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);

            EntityTabs = new VendorNavMenu(driver);
        }

        public List<IWebElement> getAllInputs()
        {


            IWebElement[] inputs = {
                EntityNameField,
                FirstNameField,
                LastNameField,
                EmailField,
                PhoneField,
                Phone2Field,
                Address1,
                Address2,
                CityField,
                ZipcodeField,
                CountryField
            };
            List<IWebElement> fields = new List<IWebElement>(inputs);

            return fields;
        }

    }
}
