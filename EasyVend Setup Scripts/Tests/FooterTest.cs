using NUnit.Framework;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    public class FooterTest
    {

        string baseUrl;
        private LoginPage loginPage;
        private Footer footer;

        [SetUp]
        public void Setup()
        {
            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            footer = new Footer(DriverFactory.Driver);

        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            footer = null;
            DriverFactory.CloseDriver();
        }

        //test if footer is present
        [Test, Description("Verify the footer is visible at the bottom of the page")]
        public void Test_Footer()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            string footerText = footer.getFooterText();
            Assert.IsNotEmpty(footerText);

        }

        //test copyright date in footer
        [Test, Description("Verify footer copyright has correct date")]
        public void Footer_Copyright_Date()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            string expectedCopyright = ConfigurationManager.AppSettings["Copyright"];
            string copyright = footer.parseCopyright();

            Assert.AreEqual(expectedCopyright, copyright);
        }

        //test version number is expected value
        [Test, Description("Verify footer has correct TMS version")]
        public void Footer_TMS_Version()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            string expectedVersion = ConfigurationManager.AppSettings["Version"];
            string version = footer.parseVersion();

            Assert.AreEqual(expectedVersion, version);
        }

    }
}