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
    internal class LotteryUsersPageTest
    {
        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private LotteryDetailsPage LotteryDetails;
        private LotteryUsersPage lotteryUsers;
        private VendorDetailsPage vendorDetails;
        private UserManagementPage userPage;

        [SetUp]
        public void Setup()
        {
            DriverFactory.InitDriver();

            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            LotteryDetails = new LotteryDetailsPage(DriverFactory.Driver);
            lotteryUsers = new LotteryUsersPage(DriverFactory.Driver);
            vendorDetails = new VendorDetailsPage(DriverFactory.Driver);
            userPage = new UserManagementPage(DriverFactory.Driver);
        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            LotteryDetails = null;
            lotteryUsers = null;
            DriverFactory.CloseDriver();
        }


        #region BaseLine


        [Test, Order(2), Description("Verify user can set number of records in table")]
        public void Set_Table_Length()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(10);
            Assert.LessOrEqual(lotteryUsers.getRecordCount(), 10);

            lotteryUsers.SetTableLength(25);
            Assert.LessOrEqual(lotteryUsers.getRecordCount(), 25);

            lotteryUsers.SetTableLength(50);
            Assert.LessOrEqual(lotteryUsers.getRecordCount(), 50);

            lotteryUsers.SetTableLength(100);
            Assert.LessOrEqual(lotteryUsers.getRecordCount(), 100);
        }


        [Test, Description("Verify table can be sorted by columns in ascending order")]
        public void Sort_Table_Asc()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            lotteryUsers.SortByColAsc((int)UserTableHeaders.USERNAME);
            //sort by email asc
            IList<string> val = lotteryUsers.GetValuesForCol((int)UserTableHeaders.USERNAME);

            IList<string> expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            lotteryUsers.SortByColAsc((int)UserTableHeaders.FIRSTNAME);
            //sort by first name asc
            val = lotteryUsers.GetValuesForCol((int)UserTableHeaders.FIRSTNAME);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            lotteryUsers.SortByColAsc((int)UserTableHeaders.LASTNAME);
            //sort by last name asc
            val = lotteryUsers.GetValuesForCol((int)UserTableHeaders.LASTNAME);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            lotteryUsers.SortByColAsc((int)UserTableHeaders.ROLE);
            //sort by role asc
            val = lotteryUsers.GetValuesForCol((int)UserTableHeaders.ROLE);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            lotteryUsers.SortByColAsc((int)UserTableHeaders.LOCKED);
            //sort by locked asc
            val = lotteryUsers.GetValuesForCol((int)UserTableHeaders.LOCKED);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            lotteryUsers.SortByColAsc((int)UserTableHeaders.ACTIVE);
            //sort by active asc
            val = lotteryUsers.GetValuesForCol((int)UserTableHeaders.ACTIVE);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

        }


        [Test, Description("Verify table can be sorted by columns in descending order")]
        public void Sort_Table_Desc()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            lotteryUsers.SortByColDesc((int)UserTableHeaders.USERNAME);
            //sort by email asc
            IList<string> val = lotteryUsers.GetValuesForCol((int)UserTableHeaders.USERNAME);

            IList<string> expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            lotteryUsers.SortByColDesc((int)UserTableHeaders.FIRSTNAME);
            //sort by first name asc
            val = lotteryUsers.GetValuesForCol((int)UserTableHeaders.FIRSTNAME);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            lotteryUsers.SortByColDesc((int)UserTableHeaders.LASTNAME);
            //sort by last name asc
            val = lotteryUsers.GetValuesForCol((int)UserTableHeaders.LASTNAME);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            lotteryUsers.SortByColDesc((int)UserTableHeaders.ROLE);
            //sort by role asc
            val = lotteryUsers.GetValuesForCol((int)UserTableHeaders.ROLE);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            lotteryUsers.SortByColDesc((int)UserTableHeaders.LOCKED);
            //sort by locked asc
            val = lotteryUsers.GetValuesForCol((int)UserTableHeaders.LOCKED);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            lotteryUsers.SortByColDesc((int)UserTableHeaders.ACTIVE);
            //sort by active asc
            val = lotteryUsers.GetValuesForCol((int)UserTableHeaders.ACTIVE);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

        }


        [Test, Description("Verify record exists returns true when a matching record is found")]
        public void Record_Exists_True()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            UserTableRecord user = lotteryUsers.GetUserByIndex(0);

            //searchby username
            Assert.IsTrue(lotteryUsers.recordExists(AppUsers.LOTTERY_ADMIN.Username));
            //search by first name
            Assert.IsTrue(lotteryUsers.recordExists(user.FirstName));
            //search by last name
            Assert.IsTrue(lotteryUsers.recordExists(user.LastName));
            //search by role
            Assert.IsTrue(lotteryUsers.recordExists(user.Role));
        }


        [Test, Description("Verify record exists returns false when a matching record is not found")]
        public void Record_Exists_False()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            string username = NameGenerator.GenerateUsername(10);

            Assert.IsFalse(lotteryUsers.recordExists(username));
        }


        [Test, Description("Verify matchingRecordCount returns correct number of records")]
        public void Matching_Record_Count()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            int expected = lotteryUsers.matchingRecordCount("@test.com");
            lotteryUsers.EnterSearchTerm("@test.com");
            int count = lotteryUsers.getRecordCount();

            Assert.AreEqual(expected, count);
        }


        [Test, Order(3), Description("verify table can be filtered by search term")]
        public void Test_Search()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            int index = lotteryUsers.indexOfUser(AppUsers.LOTTERY_ADMIN.Username);
            UserTableRecord user = lotteryUsers.GetUserByIndex(index);
            Assert.NotNull(user);

            int count = lotteryUsers.getRecordCount();

            //search by email
            lotteryUsers.EnterSearchTerm(user.Username);
            Assert.AreEqual(lotteryUsers.getRecordCount(), 1);

            // search by first name
            lotteryUsers.EnterSearchTerm(user.FirstName);
            Assert.GreaterOrEqual(lotteryUsers.getRecordCount(), 1);

            // search by last name
            lotteryUsers.EnterSearchTerm(user.LastName);
            Assert.GreaterOrEqual(lotteryUsers.getRecordCount(), 1);

            // search role
            lotteryUsers.EnterSearchTerm("Report");
            Assert.GreaterOrEqual(lotteryUsers.getRecordCount(), 1);

            // search role
            lotteryUsers.EnterSearchTerm("Lottery");
            Assert.GreaterOrEqual(lotteryUsers.getRecordCount(), count);
        }


        [Test, Order(4), Description("Verify error is not shown when no search results are found+")]
        public void Zero_Search_Results()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            string search = NameGenerator.GenerateUsername(10);
            lotteryUsers.EnterSearchTerm(search);

            Assert.Zero(lotteryUsers.getRecordCount());
        }


        #endregion

        #region AddUser


        [Test, Order(1), Description("Verify a new lottery user can be created")]
        public void AddUser_Success()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);
            lotteryUsers.ClickAddUser();

            string username = NameGenerator.GenerateUsername(5);
            Assert.AreEqual(-1, lotteryUsers.indexOfUser(username));

            lotteryUsers.AddUserSuccess(
               username,
               NameGenerator.GenerateEntityName(5),
               NameGenerator.GenerateEntityName(5),
               "6612200748",
               (int)UserRoleSelect.ADMIN
            );

            Assert.AreNotEqual(-1, lotteryUsers.indexOfUser(username));
        }


        [Test, Description("Verify the user is not added if invalid data is entered")]
        public void AddUser_Fail()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            string username = NameGenerator.GenerateUsername(5);
            lotteryUsers.ClickAddUser();

            lotteryUsers.AddUserFail(
               username,
               "",
               NameGenerator.GenerateEntityName(5),
               "6612200748",
               (int)UserRoleSelect.ADMIN
            );

            //verify modal is still open with erros
            Assert.IsTrue(lotteryUsers.AddUserModal.fNameErrorIsDisplayed());

            lotteryUsers.AddUserModal.ClickCancel();
            //verify the user was not added to the list
            Assert.AreEqual(-1, lotteryUsers.indexOfUser(username));

        }


        [Test, Description("Verify an error is displayed if the email field is blank")]
        public void AddUser_Email_Empty()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.ClickAddUser();

            lotteryUsers.AddUserFail(
                "",
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(lotteryUsers.AddUserModal.EmailErrorIsDisplayed());
        }


        [Test, Description("Verify error is displayed when email is in use by another user")]
        public void AddUser_Email_Taken()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.ClickAddUser();

            lotteryUsers.AddUserFail(
                AppUsers.LOTTERY_ADMIN.Username,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(lotteryUsers.AddUserModal.TakenEmailErrorIsDisplayed());
        }


        //Verify error is displayed when email is in incorrect format
        [Test, Description("Verify error is displayed when email is in incorrect format")]
        public void AddUser_Email_Invalid()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.ClickAddUser();

            //missing @
            lotteryUsers.AddUserFail(
                "testemail.com",
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );
            Assert.IsTrue(lotteryUsers.AddUserModal.EmailErrorIsDisplayed());
            lotteryUsers.AddUserModal.ClickCancel();

            //missing .
            lotteryUsers.ClickAddUser();
            lotteryUsers.AddUserFail(
                "test@emailcom",
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );
            Assert.IsTrue(lotteryUsers.AddUserModal.EmailErrorIsDisplayed());
            lotteryUsers.AddUserModal.ClickCancel();

            //missing com
            lotteryUsers.ClickAddUser();
            lotteryUsers.AddUserFail(
                "test@email.",
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );
            Assert.IsTrue(lotteryUsers.AddUserModal.EmailErrorIsDisplayed());
            lotteryUsers.AddUserModal.ClickCancel();

            //numbers in com
            lotteryUsers.ClickAddUser();
            lotteryUsers.AddUserFail(
                "test@email.123",
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );
            Assert.IsTrue(lotteryUsers.AddUserModal.EmailErrorIsDisplayed());

        }


        [Test, Description("Verify an error is displayed if the first name field is blank")]
        public void AddUser_FirstName_Empty()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.ClickAddUser();

            string username = NameGenerator.GenerateUsername(5);
            lotteryUsers.AddUserFail(
                username,
                "",
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(lotteryUsers.AddUserModal.fNameErrorIsDisplayed());
            lotteryUsers.AddUserModal.ClickCancel();

            Assert.AreEqual(-1, lotteryUsers.indexOfUser(username));
        }


        [Test, Description("Verify an error is displayed if the first name field is blank")]
        public void AddUser_LastName_Empty()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.ClickAddUser();

            string username = NameGenerator.GenerateUsername(5);
            lotteryUsers.AddUserFail(
                username,
                NameGenerator.GenerateEntityName(5),
                "",
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(lotteryUsers.AddUserModal.lNameErrorIsDisplayed());
            lotteryUsers.AddUserModal.ClickCancel();

            Assert.AreEqual(-1, lotteryUsers.indexOfUser(username));
        }


        [Test, Description("Verify an error is displayed if the first name field is blank")]
        public void AddUser_Phone_Empty()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.ClickAddUser();

            string username = NameGenerator.GenerateUsername(5);
            lotteryUsers.AddUserFail(
                username,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(lotteryUsers.AddUserModal.PhoneErrorIsDisplayed());
            lotteryUsers.AddUserModal.ClickCancel();

            Assert.AreEqual(-1, lotteryUsers.indexOfUser(username));
        }


        //verify error is displayed when phone number is in invalid format
        [Test, Description("verify error is displayed when phone number is in invalid format")]
        public void AddUser_Phone_Invalid()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.ClickAddUser();

            //more than 10 digits
            lotteryUsers.AddUserFail(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "11111111111",
                (int)UserRoleSelect.ADMIN
            );
            Assert.IsTrue(lotteryUsers.AddUserModal.PhoneErrorIsDisplayed());
            lotteryUsers.AddUserModal.ClickCancel();

            //less than 10 numbers 123456789
            lotteryUsers.ClickAddUser();
            lotteryUsers.AddUserFail(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "123456789",
                (int)UserRoleSelect.ADMIN
            );
            Assert.IsTrue(lotteryUsers.AddUserModal.PhoneErrorIsDisplayed());
            lotteryUsers.AddUserModal.ClickCancel();

            //letters 123456789a
            lotteryUsers.ClickAddUser();
            lotteryUsers.AddUserFail(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "123456789a",
                (int)UserRoleSelect.ADMIN
            );
            Assert.IsTrue(lotteryUsers.AddUserModal.PhoneErrorIsDisplayed());
            lotteryUsers.AddUserModal.ClickCancel();

            //special characters 123456789!
            lotteryUsers.ClickAddUser();
            lotteryUsers.AddUserFail(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "123456789!",
                (int)UserRoleSelect.ADMIN
            );
            Assert.IsTrue(lotteryUsers.AddUserModal.PhoneErrorIsDisplayed());
        }


        [Test, Description("Verify an error is displayed if the first name field is blank")]
        public void AddUser_Role_Empty()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.ClickAddUser();

            string username = NameGenerator.GenerateUsername(5);
            lotteryUsers.AddUserFail(
                username,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.NONE
            );

            Assert.IsTrue(lotteryUsers.AddUserModal.RoleErrorIsDisplayed());
            lotteryUsers.AddUserModal.ClickCancel();

            Assert.AreEqual(-1, lotteryUsers.indexOfUser(username));
        }

        #endregion

        #region EditUser


        [Test, Description("Verify fields are pre-populated with users' current information")]
        public void EditUser_Prepopulated_Data()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            UserTableRecord user = lotteryUsers.GetUserByIndex(0);
            lotteryUsers.ClickEditUserButton(0);

            Assert.IsTrue(lotteryUsers.EditUserModal.GetFirstName().Length > 0);
            Assert.IsTrue(lotteryUsers.EditUserModal.GetLastName().Length > 0);
            Assert.IsTrue(lotteryUsers.EditUserModal.GetEmail().Length > 0);
            Assert.IsTrue(lotteryUsers.EditUserModal.GetPhone().Length > 0);
            Assert.IsTrue(lotteryUsers.EditUserModal.GetRole().Length > 0);

            Assert.IsTrue(lotteryUsers.EditUserModal.UserMatches(user));
        }


        [Test, Description("Verify edit user modal opens when clicking a username from the list")]
        public void EditUser_Click_Username()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            lotteryUsers.ClickUsername(0);

            Assert.IsTrue(lotteryUsers.EditUserModal.IsOpen);
        }


        //Verify the edit user button is visible next to users in the list
        [Test, Description("Verify the edit user button is visible next to users in the list")]
        public void EditUser_Button()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);


            int count = lotteryUsers.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(lotteryUsers.EditUserButtonIsVisible(i));
            }
        }


        [Test, Description("Verify edit user modal opens when clicking the edit user button")]
        public void EditUser_Click_Button()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            Assert.IsTrue(lotteryUsers.EditUserButtonIsVisible(0));

            lotteryUsers.ClickEditUserButton(0);

            Assert.IsTrue(lotteryUsers.EditUserModal.IsOpen);
        }


        [Test, Description("Verify edit user modal closes when the cancel button is pressed and user is unchanged")]
        public void EditUser_Cancel()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            UserTableRecord userBefore = lotteryUsers.GetUserByIndex(0);
            lotteryUsers.ClickEditUserButton(0);
            lotteryUsers.EditUserModal.EnterForm(
                userBefore.Username,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.REPORT
            );

            lotteryUsers.EditUserModal.ClickCancel();
            Assert.IsFalse(lotteryUsers.EditUserModal.IsOpen);

            UserTableRecord userAfter = lotteryUsers.GetUserByIndex(0);
            Assert.IsTrue(UserTableRecord.AreEqual(userBefore, userAfter));
        }


        [Test, Description("Verify a user can be edited successfully")]
        public void EditUser_Success()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            UserTableRecord userBefore = lotteryUsers.GetUserByIndex(0);

            lotteryUsers.ClickEditUserButton(0);
            lotteryUsers.EditUserSuccess(
                userBefore.Username,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.REPORT
            );

            int index = lotteryUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = lotteryUsers.GetUserByIndex(index);

            Assert.IsFalse(UserTableRecord.AreEqual(userBefore, userAfter));
        }


        [Test, Description("Verify the user is not modified if invalid data is entered")]
        public void EditUser_Fail()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            UserTableRecord userBefore = lotteryUsers.GetUserByIndex(0);

            //attempt to edit user - failure
            lotteryUsers.ClickEditUserButton(0);
            lotteryUsers.EditUserFail(
                userBefore.Username,
                "",
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(lotteryUsers.EditUserModal.fNameErrorIsDisplayed());

            lotteryUsers.EditUserModal.ClickCancel();

            int index = lotteryUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = lotteryUsers.GetUserByIndex(index);

            Assert.IsTrue(UserTableRecord.AreEqual(userAfter, userBefore));

        }



        [Test, Description("Verify usere can edit another user's first name")]
        public void EditUser_Edit_FirstName()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            UserTableRecord userBefore = lotteryUsers.GetUserByIndex(0);
            string newName = NameGenerator.GenerateEntityName(5);
            //eidt user
            lotteryUsers.ClickEditUserButton(0);
            lotteryUsers.EditUserSuccess(
                userBefore.Username,
                newName,
                userBefore.LastName,
                "6612200748",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            int index = lotteryUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = lotteryUsers.GetUserByIndex(index);
            //verify users info matches the new user info
            Assert.IsFalse(UserTableRecord.AreEqual(userAfter, userBefore));
            Assert.AreEqual(newName, userAfter.FirstName);
        }


        [Test, Description("Verify error is displayed when first name field is empty")]
        public void EditUser_FirstName_Empty()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            UserTableRecord userBefore = lotteryUsers.GetUserByIndex(0);

            //eidt user
            lotteryUsers.ClickEditUserButton(0);
            lotteryUsers.EditUserFail(
                userBefore.Username,
                "",
                userBefore.LastName,
                "6612200748",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(lotteryUsers.EditUserModal.fNameErrorIsDisplayed());

            lotteryUsers.EditUserModal.ClickCancel();
            int index = lotteryUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = lotteryUsers.GetUserByIndex(index);

            Assert.IsTrue(UserTableRecord.AreEqual(userBefore, userAfter));
        }


        [Test, Description("Verify usere can edit another user's last name")]
        public void EditUser_Edit_LastName()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            UserTableRecord userBefore = lotteryUsers.GetUserByIndex(0);
            string newName = NameGenerator.GenerateEntityName(5);
            //eidt user
            lotteryUsers.ClickEditUserButton(0);
            lotteryUsers.EditUserSuccess(
                userBefore.Username,
                userBefore.FirstName,
                newName,
                "6612200748",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            int index = lotteryUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = lotteryUsers.GetUserByIndex(index);

            //verify users info matches the new user info
            Assert.IsFalse(UserTableRecord.AreEqual(userAfter, userBefore));
            Assert.AreEqual(newName, userAfter.LastName);
        }


        [Test, Description("Verify error is displayed when first name field is empty")]
        public void EditUser_LastName_Empty()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            UserTableRecord userBefore = lotteryUsers.GetUserByIndex(0);

            //eidt user
            lotteryUsers.ClickEditUserButton(0);
            lotteryUsers.EditUserFail(
                userBefore.Username,
                userBefore.FirstName,
                "",
                "6612200748",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(lotteryUsers.EditUserModal.lNameErrorIsDisplayed());

            lotteryUsers.EditUserModal.ClickCancel();
            int index = lotteryUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = lotteryUsers.GetUserByIndex(index);

            Assert.IsTrue(UserTableRecord.AreEqual(userBefore, userAfter));
        }


        //verify the email field is read only on the edit user modal
        [Test, Description("Verify the email field is read only on the edit user modal")]
        public void EditUser_Email_Readonly()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            //eidt user
            lotteryUsers.ClickEditUserButton(0);

            Assert.IsTrue(lotteryUsers.EditUserModal.EmailIsReadyOnly());
        }


        [Test, Description("Verify usere can edit another user's phone")]
        public void EditUser_Edit_Phone()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            //gets users current phone
            lotteryUsers.ClickEditUserButton(0);
            UserTableRecord userBefore = lotteryUsers.EditUserModal.GetUser();
            lotteryUsers.EditUserModal.ClickCancel();
            Console.WriteLine(userBefore.Phone);
            //reverse phone number
            char[] c = userBefore.Phone.ToCharArray();
            Array.Reverse(c);
            string newPhone = new string(c);


            //eidt user
            lotteryUsers.ClickEditUserButton(0);
            lotteryUsers.EditUserSuccess(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                newPhone,
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            int index = lotteryUsers.indexOfUser(userBefore.Username);
            lotteryUsers.ClickEditUserButton(index);
            UserTableRecord userAfter = lotteryUsers.EditUserModal.GetUser();
            Console.WriteLine(userAfter.Phone);
            //verify users info matches the new user info
            Assert.AreNotEqual(userBefore.Phone, userAfter.Phone);
            Assert.AreEqual(newPhone, userAfter.Phone);

        }


        //Verify error is displayed when phone field is empty
        [Test, Description("Verify error is displayed when first role is empty")]
        public void EditUser_Phone_Empty()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            //get users current phone
            lotteryUsers.ClickEditUserButton(0);
            UserTableRecord userBefore = lotteryUsers.EditUserModal.GetUser();

            lotteryUsers.EditUserFail(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                "",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(lotteryUsers.EditUserModal.PhoneErrorIsDisplayed());
            lotteryUsers.EditUserModal.ClickCancel();

            //get users phone after
            int index = lotteryUsers.indexOfUser(userBefore.Username);
            lotteryUsers.ClickEditUserButton(0);
            UserTableRecord userAfter = lotteryUsers.EditUserModal.GetUser();

            //verify user was unchanged
            Assert.AreEqual(userBefore.Role, userAfter.Role);
            Assert.AreEqual(userBefore.Phone, userAfter.Phone);
        }


        //verify error is displayed when phone number is in invalid format
        [Test, Description("verify error is displayed when phone number is in invalid format")]
        public void EditUser_Phone_Invalid()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            UserTableRecord userBefore = lotteryUsers.GetUserByIndex(0);
            //get users current phone
            lotteryUsers.ClickEditUserButton(0);

            //more than 10 digits

            lotteryUsers.EditUserModal.EnterPhone("12345678912");
            lotteryUsers.EditUserModal.ClickSubmit();
            Assert.IsTrue(lotteryUsers.EditUserModal.PhoneErrorIsDisplayed());
            lotteryUsers.EditUserModal.ClickCancel();

            lotteryUsers.ClickEditUserButton(0);

            //less than 10 numbers 123456789
            lotteryUsers.EditUserModal.EnterPhone("123456789");
            lotteryUsers.EditUserModal.ClickSubmit();
            Assert.IsTrue(lotteryUsers.EditUserModal.PhoneErrorIsDisplayed());
            lotteryUsers.EditUserModal.ClickCancel();

            lotteryUsers.ClickEditUserButton(0);

            //letters 123456789a
            lotteryUsers.EditUserModal.EnterPhone("123456789a");
            lotteryUsers.EditUserModal.ClickSubmit();
            Assert.IsTrue(lotteryUsers.EditUserModal.PhoneErrorIsDisplayed());
            lotteryUsers.EditUserModal.ClickCancel();

            lotteryUsers.ClickEditUserButton(0);

            //special characters 123456789!
            lotteryUsers.EditUserModal.EnterPhone("123456789!");
            lotteryUsers.EditUserModal.ClickSubmit();
            Assert.IsTrue(lotteryUsers.EditUserModal.PhoneErrorIsDisplayed());

        }


        //verify user can edit a users role on edit user modal
        [Test, Description("verify user can edit a users role on edit user modal")]
        public void EditUser_Edit_Role()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            UserTableRecord userBefore = lotteryUsers.GetUserByIndex(0);
            int roleBefore = userBefore.Role == "Lottery Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT;

            //eidt user
            lotteryUsers.ClickEditUserButton(0);
            lotteryUsers.EditUserSuccess(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                "6612200748",
                userBefore.Role == "Lottery Admin" ? (int)UserRoleSelect.REPORT : (int)UserRoleSelect.ADMIN
            );

            int index = lotteryUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = lotteryUsers.GetUserByIndex(index);
            int roleAfter = userAfter.Role == "Lottery Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT;

            //verify users info matches the new user info
            Assert.AreNotEqual(roleBefore, roleAfter);

        }



        [Test, Description("Verify error is displayed when first role is empty")]
        public void EditUser_Role_Empty()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            UserTableRecord userBefore = lotteryUsers.GetUserByIndex(0);

            //eidt user
            lotteryUsers.ClickEditUserButton(0);
            lotteryUsers.EditUserFail(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                "6612200748",
                (int)UserRoleSelect.NONE
            );

            Assert.IsTrue(lotteryUsers.EditUserModal.RoleErrorIsDisplayed());

            lotteryUsers.EditUserModal.ClickCancel();
            int index = lotteryUsers.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = lotteryUsers.GetUserByIndex(index);

            Assert.AreEqual(userBefore.Role, userAfter.Role);
        }

        #endregion

        #region LockUser


        [Test, Description("Verify locked column displays if user is locked or not")]
        public void LockUser_Lock_Column()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            bool lockedBefore = lotteryUsers.UserIsLocked(0);
            lotteryUsers.ToggleLockUser(0);
            bool lockedAfter = lotteryUsers.UserIsLocked(0);

            Assert.AreNotEqual(lockedBefore, lockedAfter);
        }


        [Test, Description("Verify each user in table has lock button")]
        public void LockUser_Lock_Button()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            int count = lotteryUsers.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(lotteryUsers.LockButtonIsVisible(i));
            }
        }


        [Test, Description("Test click lock button for a user")]
        public void LockUser_Open_Modal()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            Assert.IsFalse(lotteryUsers.LockModal.IsOpen);
            lotteryUsers.ClickLockButton(0);
            Assert.IsTrue(lotteryUsers.LockModal.IsOpen);
        }


        [Test, Description("Verify a user can be locked/unlocked by pressing lock button")]
        public void Toggle_Lock_User()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            bool lockedBefore = lotteryUsers.UserIsLocked(0);
            lotteryUsers.ToggleLockUser(0);
            bool lockedAfter = lotteryUsers.UserIsLocked(0);

            Assert.AreNotEqual(lockedBefore, lockedAfter);
        }


        [Test, Description("Verify modal closes and user is not changed if cancel button is pressed")]
        public void LockUser_Cancel()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            bool statusBefore = lotteryUsers.UserIsLocked(0);
            lotteryUsers.ClickLockButton(0);
            lotteryUsers.LockModal.ClickCancel();
            bool statusAfter = lotteryUsers.UserIsLocked(0);

            Assert.AreEqual(statusBefore, statusAfter);
        }


        [Test, Description("Verify a locked user cannot login")]
        public void LockUser_Login_Fail()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            //lock user
            lotteryUsers.EnterSearchTerm(AppUsers.LOTTERY_ADMIN.Username);
            lotteryUsers.ToggleLockUser(0);
            //if user was already locked before lock again
            if (lotteryUsers.UserIsLocked(0) == false)
            {
                lotteryUsers.ToggleLockUser(0);
            }

            //logout
            lotteryUsers.Header.Logout();

            //attempt to login - verify usre is locked
            loginPage.PerformLogin(AppUsers.LOTTERY_ADMIN.Username, AppUsers.LOTTERY_ADMIN.Password);

            Assert.IsTrue(loginPage.validationErrorIsDisplayed());
            Assert.IsFalse(LoginPage.IsLoggedIn);

            //unlock the user
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickVendor();
            vendorDetails.EntityTabs.ClickUserManagementTab();
            userPage.EnterSearchTerm(AppUsers.LOTTERY_ADMIN.Username);
            if (userPage.UserIsLocked(0) == true)
            {
                userPage.ToggleLockUser(0);
            }
            if (userPage.UserIsActive(0) == false)
            {
                userPage.ToggleActivateUser(0);
            }

        }


        #endregion

        #region ActivateUser


        [Test, Description("Verify active column displays if user is locked or not")]
        public void ActivateUser_Active_Column()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            bool activeBefore = lotteryUsers.UserIsActive(0);
            lotteryUsers.ToggleActivateUser(0);
            bool activeAfter = lotteryUsers.UserIsActive(0);

            Assert.AreNotEqual(activeBefore, activeAfter);
        }


        [Test, Description("Verify a user can be activated/deactivate by pressing activate button")]
        public void Toggle_Activate_User()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            bool activeBefore = lotteryUsers.UserIsActive(0);
            lotteryUsers.ToggleActivateUser(0);
            bool activeAfter = lotteryUsers.UserIsActive(0);

            Assert.AreNotEqual(activeBefore, activeAfter);
        }


        [Test, Description("Verify each user in table has activate button")]
        public void ActivateUser_Activate_Button()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            int count = lotteryUsers.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(lotteryUsers.ActiveateButtonIsVisible(i));
            }
        }


        [Test, Description("Verify modal closes and user is not changed if cancel button is pressed")]
        public void ActivateUser_Cancel()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            bool statusBefore = lotteryUsers.UserIsActive(0);
            lotteryUsers.ClickActiveButton(0);
            lotteryUsers.ActivateModal.ClickCancel();
            bool statusAfter = lotteryUsers.UserIsActive(0);

            Assert.AreEqual(statusBefore, statusAfter);
        }


        [Test, Description("Verify activate modal opens when clicking activate button")]
        public void ActivateUser_Open_Modal()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            Assert.IsFalse(lotteryUsers.ActivateModal.IsOpen);
            lotteryUsers.ClickActiveButton(0);
            Assert.IsTrue(lotteryUsers.ActivateModal.IsOpen);
            lotteryUsers.ActivateModal.ClickCancel();
            Assert.IsFalse(lotteryUsers.ActivateModal.IsOpen);
        }


        [Test, Description("Verify a deactivated user cannot login")]
        public void ActivateUser_Fail_Login()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            //lock user
            lotteryUsers.EnterSearchTerm(AppUsers.LOTTERY_ADMIN.Username);
            lotteryUsers.ToggleActivateUser(0);
            //if user was already locked before lock again
            if (lotteryUsers.UserIsActive(0) == true)
            {
                lotteryUsers.ToggleActivateUser(0);
            }

            //logout
            lotteryUsers.Header.Logout();
            //attempt to login - verify usre is locked
            loginPage.PerformLogin(AppUsers.LOTTERY_ADMIN.Username, AppUsers.LOTTERY_ADMIN.Password);

            Assert.IsTrue(loginPage.validationErrorIsDisplayed());
            Assert.IsFalse(LoginPage.IsLoggedIn);

            //unlock the user
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickVendor();
            vendorDetails.EntityTabs.ClickUserManagementTab();
            userPage.EnterSearchTerm(AppUsers.LOTTERY_ADMIN.Username);

            if (userPage.UserIsActive(0) == false)
            {
                userPage.ToggleActivateUser(0);
            }
            if (userPage.UserIsLocked(0) == true)
            {
                userPage.ToggleLockUser(0);
            }
        }

        #endregion

        #region ResetPassword

        [Test, Description("Verify each user in table has reset password button")]
        public void ResetPassword_Button()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            int count = lotteryUsers.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(lotteryUsers.ResetPasswordButtonIsVisible(i));
            }
        }


        [Test, Description("Verify user becomes activated after pressing lock button")]
        public void ResetPassword_Success()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            lotteryUsers.EnterSearchTerm(AppUsers.LOTTERY_ADMIN.Username);
            lotteryUsers.ResetPassword(0);

            Assert.IsFalse(lotteryUsers.PasswordResetModal.IsOpen);

        }


        [Test, Description("Test click lock button for a user")]
        public void ResetPassword_Modal()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            Assert.IsFalse(lotteryUsers.PasswordResetModal.IsOpen);
            lotteryUsers.ClickResetPassword(0);
            Assert.IsTrue(lotteryUsers.PasswordResetModal.IsOpen);
            lotteryUsers.PasswordResetModal.ClickCancel();
            Assert.IsFalse(lotteryUsers.PasswordResetModal.IsOpen);
        }


        [Test, Description("Verify modal closes and user is not changed if cancel button is pressed")]
        public void ResetPassword_Cancel()
        {
            //navigate to lottery users page
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();
            LotteryDetails.EntityTabs.ClickUsersTab();

            lotteryUsers.SetTableLength(100);

            lotteryUsers.EnterSearchTerm(AppUsers.LOTTERY_ADMIN.Username);

            Assert.IsFalse(lotteryUsers.PasswordResetModal.IsOpen);
            lotteryUsers.ClickResetPassword(0);
            Assert.IsTrue(lotteryUsers.PasswordResetModal.IsOpen);
            lotteryUsers.PasswordResetModal.ClickCancel();
            Assert.IsFalse(lotteryUsers.PasswordResetModal.IsOpen);
        }

        #endregion

    }
}
