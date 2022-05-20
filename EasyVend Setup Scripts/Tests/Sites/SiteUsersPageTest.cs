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
    public class SiteUsersPageTest
    {

        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private SiteListPage siteList;
        private SiteDetailsEditPage siteDetails;
        private SiteUsersPage siteUsers;

        private const int ENTITY_NAME_MIN_LENGTH = 3;
        private const int ENTITY_NAME_MAX_LENGTH = 256;


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


        [Test, Order(1), Description("Add a user to the first site as a prereq for other tests")]
        public void AddUser()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(
                NameGenerator.GenerateUsername(5),
                "ben",
                "dagg",
                "6612200748",
                1
            );
        }


        [Test]
        public void Set_Table_length()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.SetTableLength(10);
            Assert.LessOrEqual(siteUsers.getRecordCount(), 10);

            siteUsers.SetTableLength(25);
            Assert.LessOrEqual(siteUsers.getRecordCount(), 25);

            siteUsers.SetTableLength(50);
            Assert.LessOrEqual(siteUsers.getRecordCount(), 50);

            siteUsers.SetTableLength(100);
            Assert.LessOrEqual(siteUsers.getRecordCount(), 100);

        }


        [Test]
        public void Add_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            string email = NameGenerator.GenerateUsername(5);
            siteUsers.AddUserSuccess(email, "FName", "LName", "6612200748", 1);

            siteUsers.EnterSearchTerm(email);
            Assert.IsTrue(siteUsers.recordExists(email));
        }


        [Test]
        public void AddUser_Fail()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            string email = NameGenerator.GenerateUsername(5);
            siteUsers.AddUserFail(email, "", "LName", "6612200748", 1);

            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
            siteUsers.AddUserModal.ClickCancel();

            Assert.IsFalse(siteUsers.recordExists(email));
        }


        [Test]
        public void Search_List()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            //add a new user
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateUsername(6),
                NameGenerator.GenerateUsername(7),
                "6612200748",
                1
            );

            UserTableRecord user = siteUsers.GetUserByIndex(0);


            //search by email
            siteUsers.EnterSearchTerm(user.Username);
            Assert.AreEqual(siteUsers.GetMatchesForCol(user.Username, 0), siteUsers.getRecordCount());

            // search by first name
            siteUsers.EnterSearchTerm(user.FirstName);
            Assert.AreEqual(siteUsers.GetMatchesForCol(user.FirstName, 1), siteUsers.getRecordCount());

            // search by last name
            siteUsers.EnterSearchTerm(user.LastName);
            Assert.AreEqual(siteUsers.GetMatchesForCol(user.LastName, 2), siteUsers.getRecordCount());

            // search role
            siteUsers.EnterSearchTerm(user.Role);
            Assert.AreEqual(siteUsers.GetMatchesForCol(user.Role, 3), siteUsers.getRecordCount());
        }


        [Test]
        public void Sort_Table_Asc()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            int index = 0;
            siteUsers.SortByColAsc(index);
            //sort by email asc
            IList<string> val = siteUsers.GetValuesForCol(index);

            IList<string> expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

            index = 1;
            siteUsers.SortByColAsc(index);
            //sort by first name asc
            val = siteUsers.GetValuesForCol(index);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

            index = 2;
            siteUsers.SortByColAsc(index);
            //sort by last name asc
            val = siteUsers.GetValuesForCol(index);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

            index = 3;
            siteUsers.SortByColAsc(index);
            //sort by role asc
            val = siteUsers.GetValuesForCol(index);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

            index = 4;
            siteUsers.SortByColAsc(index);
            //sort by locked asc
            val = siteUsers.GetValuesForCol(index);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

            index = 5;
            siteUsers.SortByColAsc(index);
            //sort by active asc
            val = siteUsers.GetValuesForCol(index);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }
        }


        [Test]
        public void Sort_Table_Desc()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            int index = 0;
            siteUsers.SortByColDesc(index);
            //sort by email asc
            IList<string> val = siteUsers.GetValuesForCol(index);

            IList<string> expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

            index = 1;
            siteUsers.SortByColDesc(index);
            //sort by first name asc
            val = siteUsers.GetValuesForCol(index);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

            index = 2;
            siteUsers.SortByColDesc(index);
            //sort by last name asc
            val = siteUsers.GetValuesForCol(index);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

            index = 3;
            siteUsers.SortByColDesc(index);
            //sort by role asc
            val = siteUsers.GetValuesForCol(index);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

            index = 4;
            siteUsers.SortByColDesc(index);
            //sort by locked asc
            val = siteUsers.GetValuesForCol(index);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

            index = 5;
            siteUsers.SortByColDesc(index);
            //sort by active asc
            val = siteUsers.GetValuesForCol(index);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }
        }


        [Test]
        public void Add_Existing_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //find email of first user in the site

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            //add user
            string newUsername = NameGenerator.GenerateUsername(5);
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(newUsername, "ben", "dagg", "6612200748", 1);

            siteUsers.EnterSearchTerm(newUsername);
            UserTableRecord existingUser = siteUsers.GetUserByIndex(0);
            Assert.NotNull(existingUser);

            //go back to site user page
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(1);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            siteUsers.AddExistingUser(existingUser.Username);

            Assert.IsFalse(siteUsers.AddUserModal.IsOpen);
            Assert.IsTrue(siteUsers.recordExists(existingUser.Username));

        }


        [Test]
        public void AddExistingUser_Fail_UserNotFound()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            string username = NameGenerator.GenerateUsername(5);
            siteUsers.ClickAddUser();
            siteUsers.AddExistingUser(username);

            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
            Assert.IsTrue(siteUsers.AddUserModal.ExistingUserErrorIsDisplayed());

            siteUsers.AddUserModal.ClickCancel();
            Assert.IsFalse(siteUsers.recordExists(username));
        }


        [Test]
        public void AddExistingUser_Fail_UserAlreadyAdded()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            //ADD A NEW USER
            string newUsername = NameGenerator.GenerateUsername(5);
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(newUsername, "Ben", "Dagg", "6612200748", 1);

            siteUsers.EnterSearchTerm(newUsername);
            UserTableRecord existingUser = siteUsers.GetUserByIndex(0);

            siteUsers.ClickAddUser();
            siteUsers.AddExistingUser(existingUser.Username);

            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
            Assert.IsTrue(siteUsers.AddUserModal.ExistingUserErrorIsDisplayed());


        }


        [Test]
        public void AddExistingUser_Fail_IncorrectPermission()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            siteUsers.AddExistingUser(AppUsers.VENDOR_ADMIN.Username);

            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
            Assert.IsTrue(siteUsers.AddUserModal.ExistingUserErrorIsDisplayed());
            siteUsers.AddUserModal.ClickCancel();

            siteUsers.ClickAddUser();
            siteUsers.AddExistingUser(AppUsers.LOTTERY_ADMIN.Username);

            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
            Assert.IsTrue(siteUsers.AddUserModal.ExistingUserErrorIsDisplayed());
            siteUsers.AddUserModal.ClickCancel();


        }


        [Test]
        public void AddExistingUser_Fail_EmailBlank()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            //enter empty string in search box
            siteUsers.ClickAddUser();
            siteUsers.AddExistingUser("");

            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
            Assert.IsTrue(siteUsers.AddUserModal.ExistingUserErrorIsDisplayed());
            Thread.Sleep(3000);

        }


        [Test]
        public void AddExistingUser_ReadOnly()
        {

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //find email of first user in the site

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            //add user
            string newUsername = NameGenerator.GenerateUsername(5);
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(newUsername, "Ben", "Dagg", "6612200748", 1);

            siteUsers.EnterSearchTerm(newUsername);
            UserTableRecord existingUser = siteUsers.GetUserByIndex(0);

            //go back to site user page
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(1);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            siteUsers.EnterExistingUser(existingUser.Username);

            Assert.IsTrue(siteUsers.AddUserModal.IsReadOnly(UserModalFields.FName));
            Assert.IsTrue(siteUsers.AddUserModal.IsReadOnly(UserModalFields.LName));
            Assert.IsTrue(siteUsers.AddUserModal.IsReadOnly(UserModalFields.PHONE));
            Assert.IsTrue(siteUsers.AddUserModal.IsReadOnly(UserModalFields.ROLE));

        }

        /*
         * ======== Add User validation tests ==============
         */

        [Test]
        public void AddUserFail_EmailTaken()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            //add user
            string newUsername = NameGenerator.GenerateUsername(5);
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(newUsername, NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "6612200748", 1);

            siteUsers.EnterSearchTerm(newUsername);
            UserTableRecord existingUser = siteUsers.GetUserByIndex(0);

            siteUsers.ClickAddUser();
            siteUsers.AddUserFail(existingUser.Username, "FName", "LName", "6612200748", 1);

            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
            Assert.IsTrue(siteUsers.AddUserModal.TakenEmailErrorIsDisplayed());

        }


        [Test]
        public void AddUser_Emai_lEmpty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            siteUsers.AddUserFail("", "FName", "LName", "6612200748", 1);

            Assert.IsTrue(siteUsers.AddUserModal.EmailErrorIsDisplayed());
            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
        }


        [Test]
        public void AddUser_Invalid_Email()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();

            siteUsers.AddUserFail("testemail.com", "Ben", "dagg", "6612200748", 1);
            Assert.IsTrue(siteUsers.AddUserModal.EmailErrorIsDisplayed());
            siteUsers.AddUserModal.ClickCancel();

            //missing .
            siteUsers.ClickAddUser();
            siteUsers.AddUserFail("test@emailcom", "Ben", "dagg", "6612200748", 1);
            Assert.IsTrue(siteUsers.AddUserModal.EmailErrorIsDisplayed());
            siteUsers.AddUserModal.ClickCancel();

            //missing com
            siteUsers.ClickAddUser();
            siteUsers.AddUserFail("test@email.", "Ben", "dagg", "6612200748", 1);
            Assert.IsTrue(siteUsers.AddUserModal.EmailErrorIsDisplayed());
            siteUsers.AddUserModal.ClickCancel();

            //numbers in com
            siteUsers.ClickAddUser();
            siteUsers.AddUserFail("test@email.123", "Ben", "dagg", "6612200748", 1);
            Assert.IsTrue(siteUsers.AddUserModal.EmailErrorIsDisplayed());
            siteUsers.AddUserModal.ClickCancel();
        }


        [Test]
        public void AddUser_FirstName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            siteUsers.AddUserFail(NameGenerator.GenerateUsername(5), "", "dagg", "6612200748", 1);

            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
            Assert.IsTrue(siteUsers.AddUserModal.fNameErrorIsDisplayed());
        }


        [Test]
        public void AddUser_LastName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            siteUsers.AddUserFail(NameGenerator.GenerateUsername(5), "Ben", "", "6612200748", 1);

            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
            Assert.IsTrue(siteUsers.AddUserModal.lNameErrorIsDisplayed());
        }


        [Test]
        public void AddUser_Phone_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            siteUsers.AddUserFail(NameGenerator.GenerateUsername(5), "Ben", "Dagg", "", 1);

            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
            Assert.IsTrue(siteUsers.AddUserModal.PhoneErrorIsDisplayed());
        }


        [Test]
        public void AddUser_Invalid_Phone()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            siteUsers.AddUserFail(NameGenerator.GenerateUsername(5), "ben", "dagg", "11111111111", 1);
            Assert.IsTrue(siteUsers.AddUserModal.PhoneErrorIsDisplayed());


            //less than 10 numbers 123456789
            siteUsers.AddUserModal.EnterPhone("123456789");
            siteUsers.AddUserModal.ClickSubmit();
            Assert.IsTrue(siteUsers.AddUserModal.PhoneErrorIsDisplayed());

            //letters 123456789a
            siteUsers.AddUserModal.EnterPhone("123456789a");
            siteUsers.AddUserModal.ClickSubmit();
            Assert.IsTrue(siteUsers.AddUserModal.PhoneErrorIsDisplayed());

            //special characters 123456789!
            siteUsers.AddUserModal.EnterPhone("123456789!");
            siteUsers.AddUserModal.ClickSubmit();
            Assert.IsTrue(siteUsers.AddUserModal.PhoneErrorIsDisplayed());
        }


        [Test]
        public void AddUser_Cancel()
        {

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            string username = NameGenerator.GenerateUsername(5);
            siteUsers.AddUserFail(username, "ben", "dagg", "11111111111", 0);

            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
            siteUsers.AddUserModal.ClickCancel();

            Assert.IsFalse(siteUsers.AddUserModal.IsOpen);
            Assert.IsFalse(siteUsers.recordExists(username));
        }


        [Test]
        public void AddUser_Role_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            siteUsers.ClickAddUser();
            siteUsers.AddUserFail("test@email.com", "ben", "dagg", "6612200748", 0);

            Assert.IsTrue(siteUsers.AddUserModal.IsOpen);
            Assert.IsTrue(siteUsers.AddUserModal.RoleErrorIsDisplayed());
        }


        /*
         * ========= Edit user test cases ===========
         */

        [Test, Description("Verify each user in the list has edit user button")]
        public void EditUser_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            int count = siteUsers.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(siteUsers.EditUserButtonIsVisible(i));
            }
        }


        [Test, Description("Verfiy edit user modal opens when clicking a username from the list")]
        public void EdittUser_Click_Username()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            //add user
            string username = NameGenerator.GenerateUsername(5);
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(username, "Ben", "Dagg", "6612200748", 1);

            siteUsers.EnterSearchTerm(username);
            UserTableRecord user = siteUsers.GetUserByIndex(0);
            siteUsers.ClickUsername(0);

            Assert.IsTrue(siteUsers.EditUserModal.IsOpen);
            Assert.IsTrue(siteUsers.EditUserModal.UserMatches(user));
        }


        [Test, Description("Verify edit user modal opens when clicking edit user button")]
        public void EditUser_Click_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            //add user
            string username = NameGenerator.GenerateUsername(5);
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(username, "Ben", "Dagg", "6612200748", 1);

            siteUsers.ClickEditUserButton(0);
            Assert.IsTrue(siteUsers.EditUserModal.IsOpen);
        }


        [Test]
        public void EditUser_Prepopulated()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            //add user
            string username = NameGenerator.GenerateUsername(5);
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(username, "Ben", "Dagg", "6612200748", 1);

            siteUsers.EnterSearchTerm(username);
            UserTableRecord user = siteUsers.GetUserByIndex(0);
            siteUsers.ClickUsername(0);

            Assert.IsTrue(siteUsers.EditUserModal.UserMatches(user));
        }


        [Test]
        public void EditUser_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();

            //add user
            string username = NameGenerator.GenerateUsername(5);
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(username, "Ben", "Dagg", "6612200748", 1);

            siteUsers.EnterSearchTerm(username);
            UserTableRecord userBefore = siteUsers.GetUserByIndex(0);

            UserTableRecord newUser = new UserTableRecord();
            newUser.Username = userBefore.Username;
            newUser.FirstName = NameGenerator.GenerateEntityName(5);
            newUser.LastName = NameGenerator.GenerateEntityName(5);
            newUser.Phone = "1111111111";

            siteUsers.ClickEditUserButton(0);
            siteUsers.EditUserSuccess("", newUser.FirstName, newUser.LastName, newUser.Phone, 2);
            UserTableRecord userAfter = siteUsers.GetUserByIndex(0);

            Assert.IsFalse(UserTableRecord.AreEqual(userBefore, userAfter));
            Assert.IsTrue(UserTableRecord.AreEqual(newUser, userAfter));
        }


        [Test]
        public void EditUser_Fail()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();


            //add user
            string username = NameGenerator.GenerateUsername(5);
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(username, "Ben", "Dagg", "6612200748", 1);

            siteUsers.EnterSearchTerm(username);
            UserTableRecord userBefore = siteUsers.GetUserByIndex(0);

            siteUsers.ClickEditUserButton(0);
            siteUsers.EditUserFail(userBefore.Username, "cba", "abc", "6612200748", 0);

            siteUsers.EditUserModal.ClickCancel();

            UserTableRecord userAfter = siteUsers.GetUserByIndex(0);
            Assert.AreEqual(userBefore.FirstName, userAfter.FirstName);
        }


        [Test, Description("Verify usere can edit another user's first name")]
        public void EditUser_Edit_FirstName()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            UserTableRecord userBefore = siteUsers.GetUserByIndex(0);
            string newName = NameGenerator.GenerateEntityName(5);
            //eidt user
            siteUsers.ClickEditUserButton(0);
            siteUsers.EditUserSuccess(
                userBefore.Username,
                newName,
                userBefore.LastName,
                "6612200748",
                userBefore.Role == "Site Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            int index = siteUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = siteUsers.GetUserByIndex(index);
            //verify users info matches the new user info
            Assert.IsFalse(UserTableRecord.AreEqual(userAfter, userBefore));
            Assert.AreEqual(newName, userAfter.FirstName);
        }


        //Verify error is displayed when first name field is empty
        [Test, Description("Verify error is displayed when first name field is empty")]
        public void EditUser_FirstName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            UserTableRecord userBefore = siteUsers.GetUserByIndex(0);

            //eidt user
            siteUsers.ClickEditUserButton(0);
            siteUsers.EditUserFail(
                userBefore.Username,
                "",
                userBefore.LastName,
                "6612200748",
                userBefore.Role == "Site Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(siteUsers.EditUserModal.fNameErrorIsDisplayed());

            siteUsers.EditUserModal.ClickCancel();
            int index = siteUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = siteUsers.GetUserByIndex(index);

            Assert.IsTrue(UserTableRecord.AreEqual(userBefore, userAfter));
        }


        [Test, Description("Verify usere can edit another user's last name")]
        public void EditUser_Edit_LastName()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            UserTableRecord userBefore = siteUsers.GetUserByIndex(0);
            string newName = NameGenerator.GenerateEntityName(5);
            //eidt user
            siteUsers.ClickEditUserButton(0);
            siteUsers.EditUserSuccess(
                userBefore.Username,
                userBefore.FirstName,
                newName,
                "6612200748",
                userBefore.Role == "Site Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            int index = siteUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = siteUsers.GetUserByIndex(index);

            //verify users info matches the new user info
            Assert.IsFalse(UserTableRecord.AreEqual(userAfter, userBefore));
            Assert.AreEqual(newName, userAfter.LastName);
        }


        [Test, Description("Verify error is displayed when first name field is empty")]
        public void EditUser_LastName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            UserTableRecord userBefore = siteUsers.GetUserByIndex(0);

            //eidt user
            siteUsers.ClickEditUserButton(0);
            siteUsers.EditUserFail(
                userBefore.Username,
                userBefore.FirstName,
                "",
                "6612200748",
                userBefore.Role == "Site Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(siteUsers.EditUserModal.lNameErrorIsDisplayed());

            siteUsers.EditUserModal.ClickCancel();
            int index = siteUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = siteUsers.GetUserByIndex(index);

            Assert.IsTrue(UserTableRecord.AreEqual(userBefore, userAfter));
        }


        //verify the email field is read only on the edit user modal
        [Test, Description("Verify the email field is read only on the edit user modal")]
        public void EditUser_Email_Readonly()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            //eidt user
            siteUsers.ClickEditUserButton(0);

            Assert.IsTrue(siteUsers.EditUserModal.EmailIsReadyOnly());
        }


        //verify user can edit a users role on edit user modal
        [Test, Description("verify user can edit a users role on edit user modal")]
        public void EditUser_Edit_Role()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            UserTableRecord userBefore = siteUsers.GetUserByIndex(0);
            int roleBefore = userBefore.Role == "Site Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT;

            //eidt user
            siteUsers.ClickEditUserButton(0);
            siteUsers.EditUserSuccess(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                "6612200748",
                userBefore.Role == "Site Admin" ? (int)UserRoleSelect.REPORT : (int)UserRoleSelect.ADMIN
            );

            int index = siteUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = siteUsers.GetUserByIndex(index);
            int roleAfter = userAfter.Role == "Site Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT;

            //verify users info matches the new user info
            Assert.AreNotEqual(roleBefore, roleAfter);

        }


        [Test, Description("Verify error is displayed when first role is empty")]
        public void EditUser_Role_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            UserTableRecord userBefore = siteUsers.GetUserByIndex(0);

            //eidt user
            siteUsers.ClickEditUserButton(0);
            siteUsers.EditUserFail(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                "6612200748",
                (int)UserRoleSelect.NONE
            );

            Assert.IsTrue(siteUsers.EditUserModal.RoleErrorIsDisplayed());

            siteUsers.EditUserModal.ClickCancel();
            int index = siteUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = siteUsers.GetUserByIndex(index);

            Assert.AreEqual(userBefore.Role, userAfter.Role);
        }


        [Test, Description("Verify usere can edit another user's phone")]
        public void EditUser_Edit_Phone()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            //gets users current phone
            siteUsers.ClickEditUserButton(0);
            UserTableRecord userBefore = siteUsers.EditUserModal.GetUser();
            siteUsers.EditUserModal.ClickCancel();

            //reverse phone number
            char[] c = userBefore.Phone.ToCharArray();
            Array.Reverse(c);
            string newPhone = new string(c);


            //eidt user
            siteUsers.ClickEditUserButton(0);
            siteUsers.EditUserSuccess(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                newPhone,
                userBefore.Role == "Site Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            int index = siteUsers.indexOfUser(userBefore.Username);
            siteUsers.ClickEditUserButton(index);
            UserTableRecord userAfter = siteUsers.EditUserModal.GetUser();

            //verify users info matches the new user info
            Assert.AreNotEqual(userBefore.Phone, userAfter.Phone);
            Assert.AreEqual(newPhone, userAfter.Phone);

        }



        //Verify error is displayed when phone field is empty
        [Test, Description("Verify error is displayed when first role is empty")]
        public void EditUser_Phone_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            //get users current phone
            siteUsers.ClickEditUserButton(0);
            UserTableRecord userBefore = siteUsers.EditUserModal.GetUser();
            siteUsers.EditUserModal.ClickCancel();

            //edit user - phone blank
            siteUsers.ClickEditUserButton(0);
            siteUsers.EditUserFail(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                "",
                userBefore.Role == "Site Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(siteUsers.EditUserModal.PhoneErrorIsDisplayed());
            siteUsers.EditUserModal.ClickCancel();

            //get users phone after
            int index = siteUsers.indexOfUser(userBefore.Username);
            siteUsers.ClickEditUserButton(0);
            UserTableRecord userAfter = siteUsers.EditUserModal.GetUser();
            siteUsers.EditUserModal.ClickCancel();

            //verify user was unchanged
            Assert.AreEqual(userBefore.Role, userAfter.Role);
            Assert.AreEqual(userBefore.Phone, userAfter.Phone);
        }


        //verify error is displayed when phone number is in invalid format
        [Test, Description("verify error is displayed when phone number is in invalid format")]
        public void EditUser_Phone_Invalid()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            UserTableRecord userBefore = siteUsers.GetUserByIndex(0);
            //get users current phone
            siteUsers.ClickEditUserButton(0);

            //more than 10 digits
            siteUsers.EditUserModal.EnterPhone("12345678912");
            siteUsers.EditUserModal.ClickSubmit();
            Assert.IsTrue(siteUsers.EditUserModal.PhoneErrorIsDisplayed());
            siteUsers.EditUserModal.ClickCancel();

            siteUsers.ClickEditUserButton(0);

            //less than 10 numbers 123456789
            siteUsers.EditUserModal.EnterPhone("123456789");
            siteUsers.EditUserModal.ClickSubmit();
            Assert.IsTrue(siteUsers.EditUserModal.PhoneErrorIsDisplayed());
            siteUsers.EditUserModal.ClickCancel();

            siteUsers.ClickEditUserButton(0);

            //letters 123456789a
            siteUsers.EditUserModal.EnterPhone("123456789a");
            siteUsers.EditUserModal.ClickSubmit();
            Assert.IsTrue(siteUsers.EditUserModal.PhoneErrorIsDisplayed());
            siteUsers.EditUserModal.ClickCancel();

            siteUsers.ClickEditUserButton(0);

            //special characters 123456789!
            siteUsers.EditUserModal.EnterPhone("123456789!");
            siteUsers.EditUserModal.ClickSubmit();
            Assert.IsTrue(siteUsers.EditUserModal.PhoneErrorIsDisplayed());

        }


        /*
         * ========== Lock User test cases ==========
         */

        [Test, Description("Verify locked column displays if user is locked or not")]
        public void LockUser_Lock_Column()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            bool lockedBefore = siteUsers.UserIsLocked(0);
            siteUsers.ToggleLockUser(0);
            bool lockedAfter = siteUsers.UserIsLocked(0);

            Assert.AreNotEqual(lockedBefore, lockedAfter);
        }


        [Test, Description("Verify each user in table has lock button")]
        public void LockUser_Lock_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            int count = siteUsers.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(siteUsers.LockButtonIsVisible(i));
            }
        }


        [Test, Description("Test click lock button for a user")]
        public void LockUser_Open_Modal()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            Assert.IsFalse(siteUsers.LockModal.IsOpen);
            siteUsers.ClickLockButton(0);
            Assert.IsTrue(siteUsers.LockModal.IsOpen);
        }


        [Test, Description("Verify a user can be locked/unlocked by pressing lock button")]
        public void Toggle_Lock_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            bool lockedBefore = siteUsers.UserIsLocked(0);
            siteUsers.ToggleLockUser(0);
            bool lockedAfter = siteUsers.UserIsLocked(0);

            Assert.AreNotEqual(lockedBefore, lockedAfter);
        }


        [Test, Description("Verify modal closes and user is not changed if cancel button is pressed")]
        public void LockUser_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            bool statusBefore = siteUsers.UserIsLocked(0);
            siteUsers.ClickLockButton(0);
            siteUsers.LockModal.ClickCancel();
            bool statusAfter = siteUsers.UserIsLocked(0);

            Assert.AreEqual(statusBefore, statusAfter);
        }


        [Test, Description("Verify active column displays if user is locked or not")]
        public void ActivateUser_Active_Column()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            bool activeBefore = siteUsers.UserIsActive(0);
            siteUsers.ToggleActivateUser(0);
            bool activeAfter = siteUsers.UserIsActive(0);

            Assert.AreNotEqual(activeBefore, activeAfter);
        }


        [Test, Description("Verify each user in table has activate button")]
        public void ActivateUser_Activate_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);
            int count = siteUsers.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(siteUsers.ActiveateButtonIsVisible(i));
            }
        }


        [Test, Description("Verify activate modal opens when clicking activate button")]
        public void ActivateUser_Open_Modal()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            Assert.IsFalse(siteUsers.ActivateModal.IsOpen);
            siteUsers.ClickActiveButton(0);
            Assert.IsTrue(siteUsers.ActivateModal.IsOpen);
            siteUsers.ActivateModal.ClickCancel();
            Assert.IsFalse(siteUsers.ActivateModal.IsOpen);
        }


        [Test, Description("Verify a user can be activated/deactivate by pressing activate button")]
        public void Toggle_Activate_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            bool activeBefore = siteUsers.UserIsActive(0);
            siteUsers.ToggleActivateUser(0);
            bool activeAfter = siteUsers.UserIsActive(0);

            Assert.AreNotEqual(activeBefore, activeAfter);
        }


        [Test, Description("Verify modal closes and user is not changed if cancel button is pressed")]
        public void ActivateUser_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            bool statusBefore = siteUsers.UserIsActive(0);
            siteUsers.ClickActiveButton(0);
            siteUsers.ActivateModal.ClickCancel();
            bool statusAfter = siteUsers.UserIsActive(0);

            Assert.AreEqual(statusBefore, statusAfter);
        }


        /*
         * ========= Reset Password tests ===========
         */
        [Test, Description("Verify each user in table has reset password button")]
        public void ResetPassword_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            int count = siteUsers.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(siteUsers.ResetPasswordButtonIsVisible(i));
            }
        }


        [Test, Description("Test click lock button for a user")]
        public void ResetPassword_Modal()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            Assert.IsFalse(siteUsers.PasswordResetModal.IsOpen);
            siteUsers.ClickResetPassword(0);
            Assert.IsTrue(siteUsers.PasswordResetModal.IsOpen);
            siteUsers.PasswordResetModal.ClickCancel();
            Assert.IsFalse(siteUsers.PasswordResetModal.IsOpen);
        }


        [Test, Description("Verify user becomes activated after pressing lock button")]
        public void ResetPassword_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            //add user
            string newUsername = NameGenerator.GenerateUsername(5);
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(newUsername, "ben", "dagg", "6612200748", 1);

            siteUsers.EnterSearchTerm(newUsername);
            siteUsers.ResetPassword(0);

            Assert.IsFalse(siteUsers.PasswordResetModal.IsOpen);

        }


        [Test, Description("Verify modal closes and user is not changed if cancel button is pressed")]
        public void ResetPassword_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();
            siteList.SetTableLength(100);


            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickUsersTab();
            siteUsers.SetTableLength(100);

            //add user
            string newUsername = NameGenerator.GenerateUsername(5);
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(newUsername, "ben", "dagg", "6612200748", 1);

            siteUsers.EnterSearchTerm(newUsername);

            Assert.IsFalse(siteUsers.PasswordResetModal.IsOpen);
            siteUsers.ClickResetPassword(0);
            Assert.IsTrue(siteUsers.PasswordResetModal.IsOpen);
            siteUsers.PasswordResetModal.ClickCancel();
            Assert.IsFalse(siteUsers.PasswordResetModal.IsOpen);
        }
    }
}