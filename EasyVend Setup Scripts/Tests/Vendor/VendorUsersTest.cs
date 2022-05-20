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
    public class VendorUsersTest
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



        [Test, Description("Verify user can choose how many records to display in the table")]
        public void Set_Table_Length()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.SetTableLength(10);
            Assert.LessOrEqual(vendorUsersPage.getRecordCount(), 10);

            vendorUsersPage.SetTableLength(25);
            Assert.LessOrEqual(vendorUsersPage.getRecordCount(), 25);

            vendorUsersPage.SetTableLength(50);
            Assert.LessOrEqual(vendorUsersPage.getRecordCount(), 50);

            vendorUsersPage.SetTableLength(100);
            Assert.LessOrEqual(vendorUsersPage.getRecordCount(), 100);
        }


        [Test, Description("Verify record exists returns true when a matching record is found")]
        public void Record_Exists_True()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord user = vendorUsersPage.GetUserByIndex(0);

            //searchby username
            Assert.IsTrue(vendorUsersPage.recordExists(AppUsers.VENDOR_ADMIN.Username));
            //search by first name
            Assert.IsTrue(vendorUsersPage.recordExists(user.FirstName));
            //search by last name
            Assert.IsTrue(vendorUsersPage.recordExists(user.LastName));
            //search by role
            Assert.IsTrue(vendorUsersPage.recordExists(user.Role));
        }



        [Test, Description("verify table can be searched")]
        public void Search_Table_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord user = vendorUsersPage.GetUserByIndex(0);

            //get single result
            vendorUsersPage.EnterSearchTerm(AppUsers.VENDOR_ADMIN.Username);
            Assert.AreEqual(1, vendorUsersPage.getRecordCount());


            //search for multiple records
            vendorUsersPage.EnterSearchTerm("@test");
            Assert.Greater(vendorUsersPage.getRecordCount(), 0);

            //search by first name
            vendorUsersPage.EnterSearchTerm(user.FirstName);
            Assert.Greater(vendorUsersPage.getRecordCount(), 0);

            //search by last name
            vendorUsersPage.EnterSearchTerm(user.LastName);
            Assert.Greater(vendorUsersPage.getRecordCount(), 0);

            //search by role
            vendorUsersPage.EnterSearchTerm(user.Role);
            Assert.Greater(vendorUsersPage.getRecordCount(), 0);

        }


        //verifies an error is not created if no search results are found
        [Test, Description("verifies an error is not created if no search results are found")]
        public void Search_Zero_Results()
        {

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            string search = NameGenerator.GenerateUsername(10);
            vendorUsersPage.EnterSearchTerm(search);

            Assert.Zero(vendorUsersPage.getRecordCount());
        }


        [Test, Description("Verify a new vendor user can be created and added to the list"), Order(1)]
        public void AddUser_Success()
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


        [Test, Description("Verify the user is not added if invalid data is entered")]
        public void AddUser_Fail()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            string username = NameGenerator.GenerateUsername(5);
            vendorUsersPage.ClickAddUser();

            vendorUsersPage.AddUserFail(
               username,
               "",
               NameGenerator.GenerateEntityName(5),
               "6612200748",
               (int)UserRoleSelect.ADMIN
            );

            //verify modal is still open with erros
            Assert.IsTrue(vendorUsersPage.AddUserModal.IsOpen);

            vendorUsersPage.AddUserModal.ClickCancel();
            //verify the user was not added to the list
            vendorUsersPage.EnterSearchTerm(username);
            Assert.AreEqual(-1, vendorUsersPage.indexOfUser(username));

        }


        [Test, Description("Verify an error is displayed if the email field is blank")]
        public void AddUser_Email_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.ClickAddUser();

            vendorUsersPage.AddUserFail(
                "",
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(vendorUsersPage.AddUserModal.EmailErrorIsDisplayed());
        }

        //Verify error is displayed when email is in incorrect format
        [Test, Description("Verify error is displayed when email is in incorrect format")]
        public void AddUser_Email_Invalid()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.ClickAddUser();

            //missing @
            vendorUsersPage.AddUserFail(
                "testemail.com",
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );
            Assert.IsTrue(vendorUsersPage.AddUserModal.EmailErrorIsDisplayed());

            //missing .
            vendorUsersPage.AddUserModal.EnterEmail("test@emailcom");
            vendorUsersPage.AddUserModal.ClickSubmit();
            Assert.IsTrue(vendorUsersPage.AddUserModal.EmailErrorIsDisplayed());

            //missing com
            vendorUsersPage.AddUserModal.EnterEmail("test@email.");
            vendorUsersPage.AddUserModal.ClickSubmit();
            Assert.IsTrue(vendorUsersPage.AddUserModal.EmailErrorIsDisplayed());

            //numbers in com
            vendorUsersPage.AddUserModal.EnterEmail("test@email.123");
            vendorUsersPage.AddUserModal.ClickSubmit();
            Assert.IsTrue(vendorUsersPage.AddUserModal.EmailErrorIsDisplayed());



        }


        [Test, Description("Verify error is displayed when email is in use by another user")]
        public void AddUser_Email_Taken()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.ClickAddUser();

            vendorUsersPage.AddUserFail(
                AppUsers.VENDOR_ADMIN.Username,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(vendorUsersPage.AddUserModal.TakenEmailErrorIsDisplayed());
        }


        [Test, Description("Verify an error is displayed if the first name field is blank")]
        public void AddUser_FirstName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.ClickAddUser();

            vendorUsersPage.AddUserFail(
                NameGenerator.GenerateUsername(5),
                "",
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(vendorUsersPage.AddUserModal.fNameErrorIsDisplayed());
        }


        [Test, Description("Verify an error is displayed if the last name field is blank")]
        public void AddUser_LastName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.ClickAddUser();

            vendorUsersPage.AddUserFail(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateEntityName(5),
                "",
                "6612200748",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(vendorUsersPage.AddUserModal.lNameErrorIsDisplayed());
        }


        [Test, Description("Verify an error is displayed if the phone field is blank")]
        public void AddUser_Phone_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.ClickAddUser();

            vendorUsersPage.AddUserFail(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "",
                (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(vendorUsersPage.AddUserModal.PhoneErrorIsDisplayed());
        }


        //verify error is displayed when phone number is in invalid format
        [Test, Description("verify error is displayed when phone number is in invalid format")]
        public void AddUser_Phone_Invalid()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.ClickAddUser();

            //more than 10 digits
            vendorUsersPage.AddUserFail(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "11111111111",
                (int)UserRoleSelect.ADMIN
            );
            Assert.IsTrue(vendorUsersPage.AddUserModal.PhoneErrorIsDisplayed());


            //less than 10 numbers 123456789
            vendorUsersPage.AddUserModal.EnterPhone("123456789");
            vendorUsersPage.AddUserModal.ClickSubmit();
            Assert.IsTrue(vendorUsersPage.AddUserModal.PhoneErrorIsDisplayed());

            //letters 123456789a
            vendorUsersPage.AddUserModal.EnterPhone("123456789a");
            vendorUsersPage.AddUserModal.ClickSubmit();
            Assert.IsTrue(vendorUsersPage.AddUserModal.PhoneErrorIsDisplayed());

            //special characters 123456789!
            vendorUsersPage.AddUserModal.EnterPhone("123456789!");
            vendorUsersPage.AddUserModal.ClickSubmit();
            Assert.IsTrue(vendorUsersPage.AddUserModal.PhoneErrorIsDisplayed());
        }


        [Test, Description("Verify an error is displayed if the role field is blank")]
        public void AddUser_Role_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.ClickAddUser();

            vendorUsersPage.AddUserFail(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.NONE
            );

            Assert.IsTrue(vendorUsersPage.AddUserModal.RoleErrorIsDisplayed());
        }


        [Test, Description("Verify user is not added when cancel button is pressed")]
        public void AddUser_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            vendorUsersPage.ClickAddUser();

            string username = NameGenerator.GenerateUsername(5);
            vendorUsersPage.AddUserFail(
                username,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.NONE
            );

            vendorUsersPage.AddUserModal.ClickCancel();

            vendorUsersPage.EnterSearchTerm(username);
            Assert.AreEqual(-1, vendorUsersPage.indexOfUser(username));
        }


        [Test, Description("Verify table can be sorted by columns in descending order")]
        public void Sort_Table_Desc()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.SetTableLength(100);

            vendorUsersPage.SortByColDesc((int)UserTableHeaders.USERNAME);
            //sort by email asc
            IList<string> val = vendorUsersPage.GetValuesForCol((int)UserTableHeaders.USERNAME);

            IList<string> expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            vendorUsersPage.SortByColDesc((int)UserTableHeaders.FIRSTNAME);
            //sort by first name asc
            val = vendorUsersPage.GetValuesForCol((int)UserTableHeaders.FIRSTNAME);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            vendorUsersPage.SortByColDesc((int)UserTableHeaders.LASTNAME);
            //sort by last name asc
            val = vendorUsersPage.GetValuesForCol((int)UserTableHeaders.LASTNAME);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            vendorUsersPage.SortByColDesc((int)UserTableHeaders.ROLE);
            //sort by role asc
            val = vendorUsersPage.GetValuesForCol((int)UserTableHeaders.ROLE);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            vendorUsersPage.SortByColDesc((int)UserTableHeaders.LOCKED);
            //sort by locked asc
            val = vendorUsersPage.GetValuesForCol((int)UserTableHeaders.LOCKED);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            vendorUsersPage.SortByColDesc((int)UserTableHeaders.ACTIVE);
            //sort by active asc
            val = vendorUsersPage.GetValuesForCol((int)UserTableHeaders.ACTIVE);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

        }


        [Test, Description("Verify table can be sorted by columns in ascending order")]
        public void Sort_Table_Asc()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.SetTableLength(100);

            vendorUsersPage.SortByColAsc((int)UserTableHeaders.USERNAME);
            //sort by email asc
            IList<string> val = vendorUsersPage.GetValuesForCol((int)UserTableHeaders.USERNAME);

            IList<string> expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            vendorUsersPage.SortByColAsc((int)UserTableHeaders.FIRSTNAME);
            //sort by first name asc
            val = vendorUsersPage.GetValuesForCol((int)UserTableHeaders.FIRSTNAME);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            vendorUsersPage.SortByColAsc((int)UserTableHeaders.LASTNAME);
            //sort by last name asc
            val = vendorUsersPage.GetValuesForCol((int)UserTableHeaders.LASTNAME);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            vendorUsersPage.SortByColAsc((int)UserTableHeaders.ROLE);
            //sort by role asc
            val = vendorUsersPage.GetValuesForCol((int)UserTableHeaders.ROLE);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            vendorUsersPage.SortByColAsc((int)UserTableHeaders.LOCKED);
            //sort by locked asc
            val = vendorUsersPage.GetValuesForCol((int)UserTableHeaders.LOCKED);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            vendorUsersPage.SortByColAsc((int)UserTableHeaders.ACTIVE);
            //sort by active asc
            val = vendorUsersPage.GetValuesForCol((int)UserTableHeaders.ACTIVE);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }



        }

    }
}