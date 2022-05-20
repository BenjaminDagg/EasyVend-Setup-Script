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
    public class UserManagementPageTest
    {

        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private VendorDetailsPage vendorDetials;
        private VendorUsersPage vendorUsersPage;
        private UserManagementPage userPage;
        private SiteListPage siteList;
        private SiteDetailsEditPage editSite;
        private SiteUsersPage siteUsers;

        [SetUp]
        public void Setup()
        {
            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            vendorDetials = new VendorDetailsPage(DriverFactory.Driver);
            vendorUsersPage = new VendorUsersPage(DriverFactory.Driver);
            userPage = new UserManagementPage(DriverFactory.Driver);
            siteList = new SiteListPage(DriverFactory.Driver);
            editSite = new SiteDetailsEditPage(DriverFactory.Driver);
            siteUsers = new SiteUsersPage(DriverFactory.Driver);
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


        #region Baseline

        [Test, Description("Verify user can set the number of records to display in the table")]
        public void Set_Table_Length()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();

            userPage.SetTableLength(10);
            Assert.LessOrEqual(userPage.getRecordCount(), 10);

            userPage.SetTableLength(25);
            Assert.LessOrEqual(userPage.getRecordCount(), 25);

            userPage.SetTableLength(50);
            Assert.LessOrEqual(userPage.getRecordCount(), 50);

            userPage.SetTableLength(100);
            Assert.LessOrEqual(userPage.getRecordCount(), 100);
        }


        [Test, Description("Verify record exists returns true when a matching record is found")]
        public void Record_Exists_True()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            UserTableRecord user = userPage.GetUserByIndex(0);

            //searchby username
            Assert.IsTrue(userPage.recordExists(AppUsers.VENDOR_ADMIN.Username));
            //search by first name
            Assert.IsTrue(userPage.recordExists(user.FirstName));
            //search by last name
            Assert.IsTrue(userPage.recordExists(user.LastName));
            //search by role
            Assert.IsTrue(userPage.recordExists(user.Role));
        }


        [Test, Description("Verify recordExists returns false when no matching records sre found")]
        public void Record_Exists_False()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            Assert.IsFalse(userPage.recordExists(NameGenerator.GenerateEntityName(5)));
        }



        [Test, Description("Verify table can be filtered by a search term")]
        public void Search_Table_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //get a site name
            navMenu.ClickSites();
            SiteTableRecord site = siteList.GetSiteByIndex(0);
            siteList.ClickEditSiteButton(0);

            //add user to site
            editSite.EntityTabs.ClickUsersTab();
            string username = NameGenerator.GenerateUsername(5);
            siteUsers.ClickAddUser();
            siteUsers.AddUserSuccess(username, "ben", "dagg", "6612200748", 1);

            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            UserTableRecord user = userPage.GetUserByIndex(0);

            //search for a single user
            userPage.EnterSearchTerm(AppUsers.VENDOR_ADMIN.Username);
            Assert.AreEqual(1, userPage.getRecordCount());
            userPage.ClearSearchField();

            //search for multiple records
            userPage.EnterSearchTerm("@test.com");
            Assert.Greater(userPage.getRecordCount(), 0);
            userPage.ClearSearchField();

            //search by first name
            userPage.EnterSearchTerm(user.FirstName);
            Assert.Greater(userPage.getRecordCount(), 0);
            userPage.ClearSearchField();

            //search by last name
            userPage.EnterSearchTerm(user.LastName);
            Assert.Greater(userPage.getRecordCount(), 0);
            userPage.ClearSearchField();

            //search by role
            userPage.EnterSearchTerm(user.Role);
            Assert.Greater(userPage.getRecordCount(), 0);
            userPage.ClearSearchField();

            //search by site name
            userPage.EnterSearchTerm(site.SiteName);
            Assert.Greater(userPage.getRecordCount(), 0);
        }


        //verifies an error is not created if no search results are found
        [Test, Description("verifies an error is not created if no search results are found")]
        public void Search_Zero_Results()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            string search = NameGenerator.GenerateUsername(10);
            userPage.EnterSearchTerm(search);

            Assert.Zero(userPage.getRecordCount());
        }


        [Test, Description("Verify table can be sorted by columns in ascending order")]
        public void Sort_Table_Asc()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            //sort by email
            userPage.SortByColAsc((int)UserTableHeaders.USERNAME);
            IList<string> val = userPage.GetValuesForCol((int)UserTableHeaders.USERNAME);

            IList<string> expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by first name
            userPage.SortByColAsc((int)UserTableHeaders.FIRSTNAME);
            //sort by first name asc
            val = userPage.GetValuesForCol((int)UserTableHeaders.FIRSTNAME);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by last name asc
            userPage.SortByColAsc((int)UserTableHeaders.LASTNAME);
            val = userPage.GetValuesForCol((int)UserTableHeaders.LASTNAME);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by role asc
            userPage.SortByColAsc((int)UserTableHeaders.ROLE);
            val = userPage.GetValuesForCol((int)UserTableHeaders.ROLE);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by entity
            userPage.SortByColAsc(4);
            val = userPage.GetValuesForCol(4);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by lock
            userPage.SortByColAsc(5);
            val = userPage.GetValuesForCol(5);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by active
            userPage.SortByColAsc(6);
            val = userPage.GetValuesForCol(6);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }
        }


        [Test, Description("Verify table can be sorted by columns in descending order")]
        public void Sort_Table_Desc()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            //sort by email
            userPage.SortByColDesc((int)UserTableHeaders.USERNAME);
            IList<string> val = userPage.GetValuesForCol((int)UserTableHeaders.USERNAME);

            IList<string> expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by first name
            userPage.SortByColDesc((int)UserTableHeaders.FIRSTNAME);
            //sort by first name asc
            val = userPage.GetValuesForCol((int)UserTableHeaders.FIRSTNAME);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by last name asc
            userPage.SortByColDesc((int)UserTableHeaders.LASTNAME);
            val = userPage.GetValuesForCol((int)UserTableHeaders.LASTNAME);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by role asc
            userPage.SortByColDesc((int)UserTableHeaders.ROLE);
            val = userPage.GetValuesForCol((int)UserTableHeaders.ROLE);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by entity
            userPage.SortByColDesc(4);
            val = userPage.GetValuesForCol(4);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by lock
            userPage.SortByColDesc(5);
            val = userPage.GetValuesForCol(5);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by active
            userPage.SortByColDesc(6);
            val = userPage.GetValuesForCol(6);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }
        }


        [Test, Description("Search user in list and return a UserTableRecord object")]
        public void Get_User_By_Index()
        {
            Random random = new Random();

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            int count = userPage.getRecordCount();

            UserTableRecord user = userPage.GetUserByIndex(random.Next(count));
            user.display();
            Assert.NotNull(user);

            user = userPage.GetUserByIndex(-1);
            Assert.IsNull(user);

            user = userPage.GetUserByIndex(count + 1);
            Assert.IsNull(user);
        }


        [Test, Description("search a user and get the index of the user in the list")]
        public void Index_Of_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            //record found
            userPage.EnterSearchTerm(AppUsers.VENDOR_ADMIN.Username);
            Assert.AreNotEqual(-1, userPage.indexOfUser(AppUsers.VENDOR_ADMIN.Username));

            //record not found
            Assert.AreEqual(-1, userPage.indexOfUser(NameGenerator.GenerateUsername(10)));
        }


        #endregion

        #region EditUser

        [Test, Description("Verify each user in the list has edit user button")]
        public void EditUser_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            int count = userPage.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(userPage.EditUserButtonIsVisible(i));
            }
        }


        [Test, Description("Verfiy edit user modal opens when clicking a username from the list")]
        public void EditUser_Click_Username()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            userPage.ClickUsername(0);

            Assert.IsTrue(userPage.EditUserModal.IsOpen);

        }


        [Test, Description("Verify edit user modal opens when clicking edit user button")]
        public void EditUser_Click_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            Assert.IsTrue(userPage.EditUserButtonIsVisible(0));
            Assert.IsFalse(userPage.EditUserModal.IsOpen);
            userPage.ClickEditUserButton(0);

            Assert.IsTrue(userPage.EditUserModal.IsOpen);
        }


        [Test, Description("Verify edit user modal is pre populated with the users data")]
        public void EditUser_Prepopulated_Data()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            UserTableRecord user = userPage.GetUserByIndex(0);
            userPage.ClickEditUserButton(0);

            Assert.IsTrue(userPage.EditUserModal.GetFirstName().Length > 0);
            Assert.IsTrue(userPage.EditUserModal.GetLastName().Length > 0);
            Assert.IsTrue(userPage.EditUserModal.GetEmail().Length > 0);
            Assert.IsTrue(userPage.EditUserModal.GetPhone().Length > 0);
            Assert.IsTrue(userPage.EditUserModal.GetRole().Length > 0);

            Assert.IsTrue(userPage.EditUserModal.UserMatches(user));
        }


        [Test, Description("Verify edit user modal closes when the cancel button is pressed and user is unchanged")]
        public void EditUser_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            UserTableRecord userBefore = userPage.GetUserByIndex(0);

            userPage.ClickEditUserButton(0);
            userPage.EditUserModal.EnterForm(
                NameGenerator.GenerateUsername(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.REPORT
            );

            userPage.EditUserModal.ClickCancel();
            Assert.IsFalse(userPage.EditUserModal.IsOpen);

            UserTableRecord userAfter = userPage.GetUserByIndex(0);
            Assert.IsTrue(UserTableRecord.AreEqual(userBefore, userAfter));
        }


        [Test, Description("Verify a user can be edited successfully")]
        public void EditUser_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            UserTableRecord userBefore = userPage.GetUserByIndex(0);

            userPage.ClickEditUserButton(0);
            userPage.EditUserSuccess(
                userBefore.Username,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                userBefore.Role.Contains("Admin") ? (int)UserRoleSelect.REPORT : (int)UserRoleSelect.ADMIN
            );
            userPage.EnterSearchTerm(userBefore.Username);
            int index = userPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = userPage.GetUserByIndex(index);

            Assert.IsFalse(UserTableRecord.AreEqual(userBefore, userAfter));
            Assert.AreNotEqual(userBefore.Role, userAfter.Role);
        }


        [Test, Description("Verify the user is not modified if invalid data is entered")]
        public void EditUser_Fail()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            UserTableRecord userBefore = userPage.GetUserByIndex(0);

            //attempt to edit user - failure
            userPage.ClickEditUserButton(0);
            userPage.EditUserFail(
                userBefore.Username,
                "",
                NameGenerator.GenerateEntityName(5),
                "5674356128",
                userBefore.Role.Contains("Admin") ? (int)UserRoleSelect.REPORT : (int)UserRoleSelect.ADMIN
            );

            Assert.IsTrue(userPage.EditUserModal.fNameErrorIsDisplayed());

            userPage.EditUserModal.ClickCancel();

            int index = userPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = userPage.GetUserByIndex(index);

            Assert.IsTrue(UserTableRecord.AreEqual(userAfter, userBefore));
            Assert.AreEqual(userAfter.Role, userBefore.Role);
        }


        [Test, Description("Verify usere can edit another user's first name")]
        public void EditUser_Edit_FirstName()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            UserTableRecord userBefore = userPage.GetUserByIndex(0);
            string newName = NameGenerator.GenerateEntityName(5);
            //eidt user
            userPage.ClickEditUserButton(0);
            userPage.EditUserSuccess(
                userBefore.Username,
                newName,
                userBefore.LastName,
                "6612200748",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            int index = userPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = userPage.GetUserByIndex(index);
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
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            UserTableRecord userBefore = userPage.GetUserByIndex(0);

            //eidt user
            userPage.ClickEditUserButton(0);
            userPage.EditUserFail(
                userBefore.Username,
                "",
                userBefore.LastName,
                "6612200748",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(userPage.EditUserModal.fNameErrorIsDisplayed());

            userPage.EditUserModal.ClickCancel();
            int index = userPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = userPage.GetUserByIndex(index);

            Assert.IsTrue(UserTableRecord.AreEqual(userBefore, userAfter));
        }


        [Test, Description("Verify usere can edit another user's last name")]
        public void EditUser_Edit_LastName()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            UserTableRecord userBefore = userPage.GetUserByIndex(0);
            string newName = NameGenerator.GenerateEntityName(5);

            //eidt user
            userPage.ClickEditUserButton(0);
            userPage.EditUserSuccess(
                userBefore.Username,
                userBefore.FirstName,
                newName,
                "6612200748",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            int index = userPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = userPage.GetUserByIndex(index);

            //verify users info matches the new user info
            Assert.IsFalse(UserTableRecord.AreEqual(userAfter, userBefore));
            Assert.AreEqual(newName, userAfter.LastName);
        }


        [Test, Description("Verify error is displayed when last name field is empty")]
        public void EditUser_LastName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            UserTableRecord userBefore = userPage.GetUserByIndex(0);

            //eidt user
            userPage.ClickEditUserButton(0);
            userPage.EditUserFail(
                userBefore.Username,
                userBefore.FirstName,
                "",
                "6612200748",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(userPage.EditUserModal.lNameErrorIsDisplayed());

            userPage.EditUserModal.ClickCancel();
            int index = userPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = userPage.GetUserByIndex(index);

            Assert.IsTrue(UserTableRecord.AreEqual(userBefore, userAfter));
        }


        //verify the email field is read only on the edit user modal
        [Test, Description("Verify the email field is read only on the edit user modal")]
        public void EditUser_Email_Readonly()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            //eidt user
            userPage.ClickEditUserButton(0);

            Assert.IsTrue(userPage.EditUserModal.EmailIsReadyOnly());
        }


        //verify user can edit a users role on edit user modal
        [Test, Description("verify user can edit a users role on edit user modal")]
        public void EditUser_Edit_Role()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            UserTableRecord userBefore = userPage.GetUserByIndex(0);
            int roleBefore = userBefore.Role.Contains("Admin") ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT;

            //eidt user
            userPage.ClickEditUserButton(0);
            userPage.EditUserSuccess(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                "6612200748",
                userBefore.Role.Contains("Admin") ? (int)UserRoleSelect.REPORT : (int)UserRoleSelect.ADMIN
            );

            int index = userPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = userPage.GetUserByIndex(index);
            int roleAfter = userAfter.Role.Contains("Admin") ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT;

            //verify users info matches the new user info
            Assert.AreNotEqual(roleBefore, roleAfter);

        }


        [Test, Description("Verify error is displayed when first role is empty")]
        public void EditUser_Role_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            UserTableRecord userBefore = userPage.GetUserByIndex(0);

            //eidt user
            userPage.ClickEditUserButton(0);
            userPage.EditUserFail(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                "6612200748",
                (int)UserRoleSelect.NONE
            );

            Assert.IsTrue(userPage.EditUserModal.RoleErrorIsDisplayed());

            userPage.EditUserModal.ClickCancel();
            int index = userPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = userPage.GetUserByIndex(index);

            Assert.IsTrue(UserTableRecord.AreEqual(userAfter, userBefore));
            Assert.AreEqual(userBefore.Role, userAfter.Role);
        }


        [Test, Description("Verify usere can edit another user's phone")]
        public void EditUser_Edit_Phone()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            //gets users current phone
            userPage.ClickEditUserButton(0);
            UserTableRecord userBefore = userPage.EditUserModal.GetUser();
            userPage.EditUserModal.ClickCancel();

            //reverse phone number
            char[] c = userBefore.Phone.ToCharArray();
            Array.Reverse(c);
            string newPhone = new string(c);


            //eidt user
            userPage.ClickEditUserButton(0);
            userPage.EditUserSuccess(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                newPhone,
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            int index = userPage.indexOfUser(userBefore.Username);
            userPage.ClickEditUserButton(index);
            UserTableRecord userAfter = userPage.EditUserModal.GetUser();

            //verify users info matches the new user info
            Assert.AreNotEqual(userBefore.Phone, userAfter.Phone);
            Assert.AreEqual(newPhone, userAfter.Phone);

        }


        //Verify error is displayed when phone field is empty
        [Test, Description("Verify error is displayed when  phone is empty")]
        public void EditUser_Phone_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            //get users current phone
            userPage.ClickEditUserButton(0);
            UserTableRecord userBefore = userPage.EditUserModal.GetUser();
            userPage.EditUserModal.ClickCancel();

            //edit user - phone blank
            userPage.ClickEditUserButton(0);
            userPage.EditUserFail(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                "",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(userPage.EditUserModal.PhoneErrorIsDisplayed());
            userPage.EditUserModal.ClickCancel();

            //get users phone after
            int index = userPage.indexOfUser(userBefore.Username);
            userPage.ClickEditUserButton(0);
            UserTableRecord userAfter = userPage.EditUserModal.GetUser();
            userPage.EditUserModal.ClickCancel();

            //verify user was unchanged
            Assert.AreEqual(userBefore.Role, userAfter.Role);
            Assert.AreEqual(userBefore.Phone, userAfter.Phone);
            Assert.IsTrue(UserTableRecord.AreEqual(userAfter, userBefore));
        }


        //verify error is displayed when phone number is in invalid format
        [Test, Description("verify error is displayed when phone number is in invalid format")]
        public void EditUser_Phone_Invalid()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            UserTableRecord userBefore = userPage.GetUserByIndex(0);
            //get users current phone
            userPage.ClickEditUserButton(0);

            //more than 10 digits
            userPage.EditUserModal.EnterPhone("12345678912");
            userPage.EditUserModal.ClickSubmit();
            Assert.IsTrue(userPage.EditUserModal.PhoneErrorIsDisplayed());
            userPage.EditUserModal.ClickCancel();

            userPage.ClickEditUserButton(0);

            //less than 10 numbers 123456789
            userPage.EditUserModal.EnterPhone("123456789");
            userPage.EditUserModal.ClickSubmit();
            Assert.IsTrue(userPage.EditUserModal.PhoneErrorIsDisplayed());
            userPage.EditUserModal.ClickCancel();

            userPage.ClickEditUserButton(0);

            //letters 123456789a
            userPage.EditUserModal.EnterPhone("123456789a");
            userPage.EditUserModal.ClickSubmit();
            Assert.IsTrue(userPage.EditUserModal.PhoneErrorIsDisplayed());
            userPage.EditUserModal.ClickCancel();

            userPage.ClickEditUserButton(0);

            //special characters 123456789!
            userPage.EditUserModal.EnterPhone("123456789!");
            userPage.EditUserModal.ClickSubmit();
            Assert.IsTrue(userPage.EditUserModal.PhoneErrorIsDisplayed());

        }


        #endregion

        #region LockUser


        [Test, Description("Verify locked column displays if user is locked or not")]
        public void LockUser_Lock_Column()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            bool lockedBefore = userPage.UserIsLocked(0);
            userPage.ToggleLockUser(0);
            bool lockedAfter = userPage.UserIsLocked(0);

            Assert.AreNotEqual(lockedBefore, lockedAfter);
        }


        [Test, Description("Verify each user in table has lock button")]
        public void LockUser_Lock_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            int count = userPage.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(userPage.LockButtonIsVisible(i));
            }
        }


        [Test, Description("Test click lock button for a user")]
        public void LockUser_Open_Modal()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            Assert.IsFalse(userPage.LockModal.IsOpen);
            userPage.ClickLockButton(0);
            Assert.IsTrue(userPage.LockModal.IsOpen);
        }


        [Test, Description("Verify a user can be locked/unlocked by pressing lock button")]
        public void LockUser_Toggle()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            bool lockedBefore = userPage.UserIsLocked(0);
            userPage.ToggleLockUser(0);
            bool lockedAfter = userPage.UserIsLocked(0);

            Assert.AreNotEqual(lockedBefore, lockedAfter);
        }


        [Test, Description("Verify modal closes and user is not changed if cancel button is pressed")]
        public void LockUser_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            bool statusBefore = userPage.UserIsLocked(0);
            userPage.ClickLockButton(0);
            userPage.LockModal.ClickCancel();
            bool statusAfter = userPage.UserIsLocked(0);

            Assert.AreEqual(statusBefore, statusAfter);
        }


        [Test, Description("Verify a locked user cannot login")]
        public void LockUser_Login_Fail()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            //lock user
            userPage.EnterSearchTerm(AppUsers.LOTTERY_ADMIN.Username);
            userPage.ToggleLockUser(0);
            //if user was already locked before lock again
            if (userPage.UserIsLocked(0) == false)
            {
                userPage.ToggleLockUser(0);
            }

            //logout
            userPage.Header.Logout();

            //attempt to login - verify usre is locked
            loginPage.PerformLogin(AppUsers.LOTTERY_ADMIN.Username, AppUsers.LOTTERY_ADMIN.Password);

            Assert.IsTrue(loginPage.validationErrorIsDisplayed());
            Assert.IsFalse(LoginPage.IsLoggedIn);

            //unlock the user
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
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
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            bool activeBefore = userPage.UserIsActive(0);
            userPage.ToggleActivateUser(0);
            bool activeAfter = userPage.UserIsActive(0);

            Assert.AreNotEqual(activeBefore, activeAfter);
        }


        [Test, Description("Verify each user in table has activate button")]
        public void ActivateUser_Activate_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            int count = userPage.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(userPage.ActiveateButtonIsVisible(i));
            }
        }


        [Test, Description("Verify activate modal opens when clicking activate button")]
        public void ActivateUser_Open_Modal()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            userPage.ClickActiveButton(0);
            Assert.IsTrue(userPage.ActivateModal.IsOpen);
            userPage.ActivateModal.ClickCancel();
            Assert.IsFalse(userPage.ActivateModal.IsOpen);
        }


        [Test, Description("Verify a user can be activated/deactivate by pressing activate button")]
        public void ActivateUser_Toggle()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            bool activeBefore = userPage.UserIsActive(0);
            userPage.ToggleActivateUser(0);
            bool activeAfter = userPage.UserIsActive(0);

            Assert.AreNotEqual(activeBefore, activeAfter);
        }


        [Test, Description("Verify modal closes and user is not changed if cancel button is pressed")]
        public void ActivateUser_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            bool statusBefore = userPage.UserIsActive(0);
            userPage.ClickActiveButton(0);
            userPage.ActivateModal.ClickCancel();
            bool statusAfter = userPage.UserIsActive(0);

            Assert.AreEqual(statusBefore, statusAfter);
        }


        [Test, Description("Verify a locked user cannot login")]
        public void ActivateUser_Login_Fail()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            //lock user
            userPage.EnterSearchTerm(AppUsers.LOTTERY_ADMIN.Username);
            userPage.ToggleActivateUser(0);
            //if user was already locked before lock again
            if (userPage.UserIsActive(0) == true)
            {
                userPage.ToggleActivateUser(0);

            }

            //logout
            userPage.Header.Logout();

            //attempt to login - verify usre is locked
            loginPage.PerformLogin(AppUsers.LOTTERY_ADMIN.Username, AppUsers.LOTTERY_ADMIN.Password);

            Assert.IsTrue(loginPage.validationErrorIsDisplayed());
            Assert.IsFalse(LoginPage.IsLoggedIn);

            //unlock the user
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
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

        #region ResetPassword


        [Test, Description("Verify each user in table has reset password button")]
        public void ResetPassword_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            int count = userPage.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(userPage.ResetPasswordButtonIsVisible(i));
            }
        }


        [Test, Description("Verify reset password modal opens when clicing reset password button")]
        public void ResetPassword_Modal()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            userPage.ClickResetPassword(0);
            Assert.IsTrue(userPage.PasswordResetModal.IsOpen);
            userPage.PasswordResetModal.ClickCancel();
            Assert.IsFalse(userPage.PasswordResetModal.IsOpen);
        }


        [Test, Description("Verify reset password is sent to user")]
        public void ResetPassword_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            userPage.EnterSearchTerm(AppUsers.VENDOR_ADMIN.Username);
            userPage.ResetPassword(0);

            Assert.IsFalse(userPage.PasswordResetModal.IsOpen);

        }


        [Test, Description("Verify password reset email isn't sent to user if cancel button is pressed")]
        public void ResetPassword_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUserManagementTab();
            userPage.SetTableLength(100);

            userPage.EnterSearchTerm(AppUsers.VENDOR_ADMIN.Username);

            userPage.ClickResetPassword(0);
            Assert.IsTrue(userPage.PasswordResetModal.IsOpen);
            userPage.PasswordResetModal.ClickCancel();
            Assert.IsFalse(userPage.PasswordResetModal.IsOpen);
        }


        #endregion

    }
}