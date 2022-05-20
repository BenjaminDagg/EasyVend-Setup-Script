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
    public class ModalTest
    {

        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private SiteListPage siteList;
        private SiteDetailsEditPage siteDetails;
        private SiteUsersPage siteUsers;

        [SetUp]
        public void Setup()
        {

            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            siteList = new SiteListPage(DriverFactory.Driver);
            siteDetails = new SiteDetailsEditPage(DriverFactory.Driver);
            siteUsers = new SiteUsersPage(DriverFactory.Driver);

        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            DriverFactory.CloseDriver();
        }


        [Test, Order(1)]
        public void Add_Test_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );
        }


        [Test]
        public void Open_AddUser_Modal()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            Assert.IsFalse(siteUsers.AddUserModal.IsOpen);
            siteUsers.ClickAddUser();
            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);

        }


        [Test]
        public void Open_Base_Modal()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            Assert.IsFalse(siteUsers.ActivateModal.IsOpen);
            siteUsers.ClickActiveButton(0);
            Assert.IsTrue(siteUsers.ActivateModal.IsOpen);
        }


        [Test]
        public void Close_Modal_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            //add user test
            Assert.IsFalse(siteUsers.AddUserModal.IsOpen);
            siteUsers.ClickAddUser();
            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
            siteUsers.AddUserModal.ClickCancel();
            Assert.IsFalse(siteUsers.AddUserModal.IsOpen);

            //base modal test
            Assert.IsFalse(siteUsers.ActivateModal.IsOpen);
            siteUsers.ClickActiveButton(0);
            Assert.IsTrue(siteUsers.ActivateModal.IsOpen);
            siteUsers.ActivateModal.ClickCancel();
            Assert.IsFalse(siteUsers.ActivateModal.IsOpen);
        }


        [Test]
        public void Modal_Submit()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            Assert.IsFalse(siteUsers.ActivateModal.IsOpen);
            siteUsers.ClickActiveButton(0);
            Assert.IsTrue(siteUsers.ActivateModal.IsOpen);
            siteUsers.ActivateModal.ClickSubmit();
            siteUsers.ActivateModal.waitForModalClose();
            Assert.IsFalse(siteUsers.ActivateModal.IsOpen);
        }


        [Test]
        public void Email_Error()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            Assert.IsFalse(siteUsers.AddUserModal.EmailErrorIsDisplayed());
            siteUsers.AddUserFail(
                "",
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(siteUsers.AddUserModal.EmailErrorIsDisplayed());

        }


        [Test]
        public void Existing_User_Error()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            UserTableRecord existingUser = siteUsers.GetUserByIndex(0);

            siteUsers.ClickAddUser();
            Assert.IsFalse(siteUsers.AddUserModal.TakenEmailErrorIsDisplayed());
            siteUsers.AddUserFail(
                existingUser.Username,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(siteUsers.AddUserModal.TakenEmailErrorIsDisplayed());

        }


        [Test]
        public void FirstName_Error()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            Assert.IsFalse(siteUsers.AddUserModal.fNameErrorIsDisplayed());
            siteUsers.AddUserFail(
                NameGenerator.GenerateUsername(5),
                "",
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(siteUsers.AddUserModal.fNameErrorIsDisplayed());

        }


        [Test]
        public void LastName_Error()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            Assert.IsFalse(siteUsers.AddUserModal.lNameErrorIsDisplayed());
            siteUsers.AddUserFail(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateEntityName(5),
                "",
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(siteUsers.AddUserModal.lNameErrorIsDisplayed());

        }


        [Test]
        public void Phone_Error()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            Assert.IsFalse(siteUsers.AddUserModal.PhoneErrorIsDisplayed());
            siteUsers.AddUserFail(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(siteUsers.AddUserModal.PhoneErrorIsDisplayed());

        }


        [Test]
        public void Role_Error()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            Assert.IsFalse(siteUsers.AddUserModal.RoleErrorIsDisplayed());
            siteUsers.AddUserFail(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.NONE
            );

            Assert.IsTrue(siteUsers.AddUserModal.RoleErrorIsDisplayed());

        }


        [Test]
        public void Existing_User_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            siteUsers.AddUserModal.EnterExistingUser(AppUsers.SITE_ADMIN.Username);

            Assert.IsTrue(
                siteUsers.AddUserModal.GetFirstName().Length > 0 &&
                siteUsers.AddUserModal.GetLastName().Length > 0 &&
                siteUsers.AddUserModal.GetPhone().Length > 0
            );
        }


        [Test]
        public void Existing_User_Fail()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            siteUsers.AddUserModal.EnterExistingUser(NameGenerator.GenerateUsername(10));

            Assert.IsTrue(siteUsers.AddUserModal.ExistingUserErrorIsDisplayed());
            Assert.IsTrue(
                siteUsers.AddUserModal.GetFirstName().Length == 0 &&
                siteUsers.AddUserModal.GetLastName().Length == 0 &&
                siteUsers.AddUserModal.GetPhone().Length == 0
            );
        }

    }
}