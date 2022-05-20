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
    public class VendorUsersPageLocked
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

            Assert.AreNotEqual(-1, vendorUsersPage.indexOfUser(username));
        }


        [Test, Description("Verify locked column displays if user is locked or not")]
        public void LockUser_Lock_Column()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            bool lockedBefore = vendorUsersPage.UserIsLocked(0);
            vendorUsersPage.ToggleLockUser(0);
            bool lockedAfter = vendorUsersPage.UserIsLocked(0);

            Assert.AreNotEqual(lockedBefore, lockedAfter);
        }


        [Test, Description("Verify each user in table has lock button")]
        public void LockUser_Lock_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);
            int count = vendorUsersPage.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(vendorUsersPage.LockButtonIsVisible(i));
            }
        }


        [Test, Description("Test click lock button for a user")]
        public void LockUser_Open_Modal()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            Assert.IsFalse(vendorUsersPage.LockModal.IsOpen);
            vendorUsersPage.ClickLockButton(0);
            Assert.IsTrue(vendorUsersPage.LockModal.IsOpen);
        }


        [Test, Description("Verify a user can be locked/unlocked by pressing lock button")]
        public void Toggle_Lock_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            bool lockedBefore = vendorUsersPage.UserIsLocked(0);
            vendorUsersPage.ToggleLockUser(0);
            bool lockedAfter = vendorUsersPage.UserIsLocked(0);

            Assert.AreNotEqual(lockedBefore, lockedAfter);
        }


        [Test, Description("Verify modal closes and user is not changed if cancel button is pressed")]
        public void LockUser_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            bool statusBefore = vendorUsersPage.UserIsLocked(0);
            vendorUsersPage.ClickLockButton(0);
            vendorUsersPage.LockModal.ClickCancel();
            bool statusAfter = vendorUsersPage.UserIsLocked(0);

            Assert.AreEqual(statusBefore, statusAfter);
        }


        [Test, Description("Verify a locked user cannot login")]
        public void LockUser_Login_Fail()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            //lock user
            vendorUsersPage.EnterSearchTerm(AppUsers.VENDOR_REPORT.Username);
            vendorUsersPage.ToggleLockUser(0);
            //if user was already locked before lock again
            if (vendorUsersPage.UserIsLocked(0) == false)
            {
                vendorUsersPage.ToggleLockUser(0);
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