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
    public class VendorUsersActive
    {

        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private VendorDetailsPage vendorDetials;
        private VendorUsersPage vendorUsersPage;

        [SetUp]
        public void Setup()
        {

            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            vendorDetials = new VendorDetailsPage(DriverFactory.Driver);
            vendorUsersPage = new VendorUsersPage(DriverFactory.Driver);
        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            vendorDetials = null;
            vendorUsersPage = null;
            DriverFactory.CloseDriver();
        }


        [Test, Description("Verify a new vendor user can be created and added to the list"), Order(1)]
        public void AddUser()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            vendorUsersPage.ClickAddUser();

            string username = NameGenerator.GenerateUsername(5);
            Assert.AreEqual(-1, vendorUsersPage.indexOfUser(username));

            vendorUsersPage.AddUserSuccess(
               username,
               NameGenerator.GenerateEntityName(5),
               NameGenerator.GenerateEntityName(5),
               "6612200748",
               (int)UserRoleSelect.ADMIN
            );

            vendorUsersPage.EnterSearchTerm(username);
            Assert.AreNotEqual(-1, vendorUsersPage.indexOfUser(username));
        }


        [Test, Description("Verify active column displays if user is locked or not")]
        public void ActivateUser_Active_Column()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            bool activeBefore = vendorUsersPage.UserIsActive(0);
            vendorUsersPage.ToggleActivateUser(0);
            bool activeAfter = vendorUsersPage.UserIsActive(0);

            Assert.AreNotEqual(activeBefore, activeAfter);
        }


        [Test, Description("Verify each user in table has activate button")]
        public void ActivateUser_Activate_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);
            int count = vendorUsersPage.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(vendorUsersPage.ActiveateButtonIsVisible(i));
            }
        }


        [Test, Description("Verify activate modal opens when clicking activate button")]
        public void ActivateUser_Open_Modal()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            Assert.IsFalse(vendorUsersPage.ActivateModal.IsOpen);
            vendorUsersPage.ClickActiveButton(0);
            Assert.IsTrue(vendorUsersPage.ActivateModal.IsOpen);
            vendorUsersPage.ActivateModal.ClickCancel();
            Assert.IsFalse(vendorUsersPage.ActivateModal.IsOpen);
        }


        [Test, Description("Verify a user can be activated/deactivate by pressing activate button")]
        public void Toggle_Activate_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            bool activeBefore = vendorUsersPage.UserIsActive(0);
            vendorUsersPage.ToggleActivateUser(0);
            bool activeAfter = vendorUsersPage.UserIsActive(0);

            Assert.AreNotEqual(activeBefore, activeAfter);
        }


        [Test, Description("Verify modal closes and user is not changed if cancel button is pressed")]
        public void ActivateUser_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            bool statusBefore = vendorUsersPage.UserIsActive(0);
            vendorUsersPage.ClickActiveButton(0);
            vendorUsersPage.ActivateModal.ClickCancel();
            bool statusAfter = vendorUsersPage.UserIsActive(0);

            Assert.AreEqual(statusBefore, statusAfter);
        }


        [Test, Description("Verify a locked user cannot login")]
        public void ActivateUser_Login_Fail()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            //lock user
            vendorUsersPage.EnterSearchTerm(AppUsers.VENDOR_REPORT.Username);
            vendorUsersPage.ToggleActivateUser(0);
            //if user was already locked before lock again
            if (vendorUsersPage.UserIsActive(0) == true)
            {
                vendorUsersPage.ToggleActivateUser(0);
            }

            //logout
            vendorUsersPage.Header.Logout();

            //attempt to login - verify usre is locked
            loginPage.PerformLogin(AppUsers.VENDOR_REPORT.Username, AppUsers.VENDOR_REPORT.Password);

            Assert.IsTrue(loginPage.validationErrorIsDisplayed());
            Assert.IsFalse(LoginPage.IsLoggedIn);

        }

    }
}