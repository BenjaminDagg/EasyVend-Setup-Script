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
    internal class HeaderTest
    {
        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private Header header;

        //users
        private string VENDOR_ADMIN_USERNAME;
        private string Vendor_ADMIN_PASSWORD;


        [SetUp]
        public void Setup()
        {
            DriverFactory.InitDriver();

            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            header = new Header(DriverFactory.Driver);


            VENDOR_ADMIN_USERNAME = ConfigurationManager.AppSettings["VendorAdminUsername"];
            Vendor_ADMIN_PASSWORD = ConfigurationManager.AppSettings["VendorAdminPassword"];

            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(VENDOR_ADMIN_USERNAME, Vendor_ADMIN_PASSWORD);


        }


        [Test, Description("Verify user is returned to home page when clicking the logo")]
        public void ClickLogo()
        {
            string expectedUrl = baseUrl + "Site";
            navMenu.ClickSites();
            Assert.AreEqual(DriverFactory.GetUrl(), expectedUrl);
            header.ClickLogo();
            Assert.AreEqual(DriverFactory.GetUrl(), baseUrl);
        }


        [Test, Description("Verify user dropdown opens when click username")]
        public void OpenUserDropdown()
        {
            Assert.IsFalse(header.UserDropDownIsOpen);
            header.ClickUserDropdown();
            Assert.IsTrue(header.UserDropDownIsOpen);
            header.ClickUserDropdown();
            Assert.IsFalse(header.UserDropDownIsOpen);
        }


        [Test, Description("Verify user is redirected to change password page when clicking button")]
        public void ClickChangePassword()
        {
            header.ClickUserDropdown();
            header.ClickChangePassword();

        }


        [Test, Description("Verify logout modal opens when click logout button")]
        public void ClickLogout()
        {
            header.ClickUserDropdown();
            header.ClickLogout();

        }


        [Test, Description("Verify user can logout by clicking logout button on header")]
        public void Logout()
        {
            header.ClickUserDropdown();
            header.ClickLogout();
            header.ClickLogoutConfirm();

            Assert.AreEqual(DriverFactory.GetUrl(), LoginPage.url);
        }


        [Test, Description("Verify user remains logged in if they cancel logout")]
        public void LogoutCancel()
        {
            header.ClickLogo();
            header.ClickUserDropdown();
            header.ClickLogout();
            header.ClickLogoutCancel();

            Assert.IsFalse(header.LogoutModal.IsOpen);
            Assert.AreEqual(DriverFactory.GetUrl(), baseUrl);
        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            DriverFactory.CloseDriver();
        }
    }
}
