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
    public class VendorUsersResetPassword
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



        [Test, Description("Verify each user in table has reset password button")]
        public void ResetPassword_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            int count = vendorUsersPage.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(vendorUsersPage.ResetPasswordButtonIsVisible(i));
            }
        }


        [Test, Description("Test click lock button for a user")]
        public void ResetPassword_Modal()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            Assert.IsFalse(vendorUsersPage.PasswordResetModal.IsOpen);
            vendorUsersPage.ClickResetPassword(0);
            Assert.IsTrue(vendorUsersPage.PasswordResetModal.IsOpen);
            vendorUsersPage.PasswordResetModal.ClickCancel();
            Assert.IsFalse(vendorUsersPage.PasswordResetModal.IsOpen);
        }


        [Test, Description("Verify user becomes activated after pressing lock button")]
        public void ResetPassword_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            vendorUsersPage.EnterSearchTerm(AppUsers.VENDOR_ADMIN.Username);
            vendorUsersPage.ResetPassword(0);

            Assert.IsFalse(vendorUsersPage.PasswordResetModal.IsOpen);

        }


        [Test, Description("Verify modal closes and user is not changed if cancel button is pressed")]
        public void ResetPassword_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            vendorUsersPage.EnterSearchTerm(AppUsers.VENDOR_ADMIN.Username);

            Assert.IsFalse(vendorUsersPage.PasswordResetModal.IsOpen);
            vendorUsersPage.ClickResetPassword(0);
            Assert.IsTrue(vendorUsersPage.PasswordResetModal.IsOpen);
            vendorUsersPage.PasswordResetModal.ClickCancel();
            Assert.IsFalse(vendorUsersPage.PasswordResetModal.IsOpen);
        }

    }
}