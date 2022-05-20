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
    public class NavMenuTest
    {

        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private Header header;

        string VENDOR_URL;
        string LOTTERY_URL;
        string SITE_URL;


        [SetUp]
        public void Setup()
        {

            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

            VENDOR_URL = baseUrl + "Vendor/Details?entityId=1";
            LOTTERY_URL = baseUrl + "Lottery/Details?entityId=2";
            SITE_URL = baseUrl + "Site";

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            header = new Header(DriverFactory.Driver);

        }

        //Verifies clicking the vendor link takes user to expected page
        [Test, Description("Verifies clicking the vendor link takes user to expected page")]
        public void NavMenu_Click_Vendor()
        {

            string expectedUrl = VENDOR_URL;
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickVendor();

            Assert.AreEqual(expectedUrl, DriverFactory.GetUrl());

        }

        //Verifies clicking the lottery link takes user to expected page
        [Test, Description("Verifies clicking the lottery link takes user to expected page")]
        public void NavMenu_Click_Lottery()
        {
            string expectedUrl = LOTTERY_URL;
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();

            Assert.AreEqual(expectedUrl, DriverFactory.GetUrl());

        }

        //Verifies clicking the sites link takes user to expected page
        [Test, Description("Verifies clicking the vendor link takes user to expected page")]
        public void NavMenu_Click_Site()
        {
            string expectedUrl = SITE_URL;
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickSites();

            Assert.AreEqual(expectedUrl, DriverFactory.GetUrl());

        }

        //Verifies a modal appears after clicking the logout link
        [Test, Description("Verifies a modal appears after clicking the logout link")]
        public void NavMenu_Logout_Modal()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLogout();

            Assert.IsTrue(navMenu.LogoutModal.IsOpen);

        }


        //Verify correct tabs are available when Vendor Admin user logs in
        [Test, Description("Verify correct tabs are available when Vendor Admin user logs in")]
        public void NavMenu_Vendor_Admin()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //verify Vendor details is the homepage when user logs in
            Assert.AreEqual(DriverFactory.GetUrl(), VENDOR_URL);

            //verify all tabs are visible on the nav menu
            Assert.IsTrue(navMenu.vendorIsVisible());
            Assert.IsTrue(navMenu.lotteryIsVisible());
            Assert.IsTrue(navMenu.siteIsVisible());
        }


        //Verify correct tabs are available when Vendor Report user logs in
        [Test, Description("Verify correct tabs are available when Vendor Report user logs in")]
        public void NavMenu_Vendor_Report()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_REPORT.Username, AppUsers.VENDOR_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            Assert.IsTrue(LoginPage.IsLoggedIn);

            //verify all tabs are visible on the nav menu
            Assert.IsFalse(navMenu.vendorIsVisible());
            Assert.IsFalse(navMenu.lotteryIsVisible());
            Assert.IsFalse(navMenu.siteIsVisible());
        }


        //Verify correct tabs are available when lottery admin user logs in
        [Test, Description("Verify correct tabs are available when lottery admin user logs in")]
        public void NavMenu_Lottery_Admin()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.LOTTERY_ADMIN.Username, AppUsers.LOTTERY_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //verify Vendor details is the homepage when user logs in
            Assert.AreEqual(DriverFactory.GetUrl(), LOTTERY_URL);

            //verify all tabs are visible on the nav menu
            Assert.IsFalse(navMenu.vendorIsVisible());
            Assert.IsTrue(navMenu.lotteryIsVisible());
            Assert.IsTrue(navMenu.siteIsVisible());
        }


        //Verify correct tabs are available when lottery report user logs in
        [Test, Description("Verify correct tabs are available when lottery report user logs in")]
        public void NavMenu_Lottery_Report()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.LOTTERY_REPORT.Username, AppUsers.LOTTERY_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //verify Vendor details is the homepage when user logs in
            Assert.AreEqual(DriverFactory.GetUrl(), LOTTERY_URL);

            //verify all tabs are visible on the nav menu
            Assert.IsFalse(navMenu.vendorIsVisible());
            Assert.IsTrue(navMenu.lotteryIsVisible());
            Assert.IsFalse(navMenu.siteIsVisible());
        }


        //Verify correct tabs are available when site admin user logs in
        [Test, Description("Verify correct tabs are available when lottery report user logs in")]
        public void NavMenu_Site_Admin()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_ADMIN.Username, AppUsers.SITE_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //verify Vendor details is the homepage when user logs in
            Assert.AreEqual(DriverFactory.GetUrl(), SITE_URL);

            //verify all tabs are visible on the nav menu
            Assert.IsFalse(navMenu.vendorIsVisible());
            Assert.IsFalse(navMenu.lotteryIsVisible());
            Assert.IsTrue(navMenu.siteIsVisible());
        }


        //Verify correct tabs are available when site report user logs in
        [Test, Description("Verify correct tabs are available when site report user logs in")]
        public void NavMenu_Site_Report()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //verify all tabs are visible on the nav menu
            Assert.IsFalse(navMenu.vendorIsVisible());
            Assert.IsFalse(navMenu.lotteryIsVisible());
            Assert.IsTrue(navMenu.siteIsVisible());
        }




        //Verify user can logout by pressing logout button on navmenu
        [Test, Description("Performs a logout")]
        public void NavMenu_Logout()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);


            navMenu.PerformLogout();
            Assert.AreEqual(LoginPage.url, DriverFactory.GetUrl());
            Assert.IsFalse(LoginPage.IsLoggedIn);
        }


        [Test, Description("Verify user remains logged in if they cancel logout prompt")]
        public void NavMenu_Cancel_Logout()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            header.ClickLogo();
            navMenu.ClickLogout();
            navMenu.ClickModalCancel();

            Assert.AreEqual(baseUrl, DriverFactory.GetUrl());
            Assert.IsTrue(LoginPage.IsLoggedIn);

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