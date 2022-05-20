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
    public class UserTableTest
    {

        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private VendorDetailsPage vendorDetials;
        private VendorUsersPage vendorUsersPage;
        private SiteListPage siteListPage;
        private SiteDetailsAddPage siteDetailsAddPage;
        private SiteDetailsEditPage editSite;
        private SiteUsersPage siteUsersPage;

        [SetUp]
        public void Setup()
        {

            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            vendorDetials = new VendorDetailsPage(DriverFactory.Driver);
            vendorUsersPage = new VendorUsersPage(DriverFactory.Driver);
            siteListPage = new SiteListPage(DriverFactory.Driver);
            siteDetailsAddPage = new SiteDetailsAddPage(DriverFactory.Driver);
            siteUsersPage = new SiteUsersPage(DriverFactory.Driver);
            editSite = new SiteDetailsEditPage(DriverFactory.Driver);
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


        //test record count
        [Test]
        public void Record_Count()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.SetTableLength(100);
            int count = vendorUsersPage.getRecordCount();

            Assert.NotZero(count);
        }


        //test zero record count
        [Test]
        public void Record_Count_Zero()
        {

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickSites();

            siteListPage.ClickAddSite();
            string siteName = NameGenerator.GenerateEntityName(5);
            siteDetailsAddPage.EnterForm(
                siteName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "city",
                "12345"
            );
            siteDetailsAddPage.ClickSubmit();
            navMenu.ClickSites();


            siteListPage.EnterSearchTerm(siteName);
            int index = siteListPage.indexOfSite(siteName);
            siteListPage.ClickSiteByIndex(index);
            editSite.EntityTabs.ClickUsersTab();

            int count = siteUsersPage.getRecordCount();
            Assert.AreEqual(0, count);
        }


        [Test]
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


        [Test]
        public void Record_Exists_False()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            Assert.IsFalse(vendorUsersPage.recordExists(NameGenerator.GenerateEntityName(5)));
        }


        [Test]
        public void Search_Table_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord user = vendorUsersPage.GetUserByIndex(0);

            //search for single matching record
            vendorUsersPage.EnterSearchTerm(AppUsers.VENDOR_ADMIN.Username);
            Assert.AreEqual(1, vendorUsersPage.getRecordCount());

            //search for multiple records
            vendorUsersPage.EnterSearchTerm("@test.com");
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


        [Test]
        public void Search_Table_Zero_Results()
        {

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            //search for single matching record
            vendorUsersPage.EnterSearchTerm(NameGenerator.GenerateUsername(10));
            Assert.Zero(vendorUsersPage.getRecordCount());
        }





        [Test]
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


        [Test]
        public void Sort_Table_Ascending()
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


        [Test]
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


        [Test]
        public void Get_Matches_ForCol()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.SetTableLength(100);

            //get one match
            Assert.AreEqual(1, vendorUsersPage.GetMatchesForCol(AppUsers.VENDOR_ADMIN.Username, (int)UserTableHeaders.USERNAME));

            //multiple matches
            Assert.Greater(vendorUsersPage.GetMatchesForCol("@test.com", (int)UserTableHeaders.USERNAME), 0);

            //search first name
            Assert.Greater(vendorUsersPage.GetMatchesForCol("ben", (int)UserTableHeaders.FIRSTNAME), 0);

            //search last name
            Assert.Greater(vendorUsersPage.GetMatchesForCol("dagg", (int)UserTableHeaders.USERNAME), 0);
        }


        [Test]
        public void Get_User_By_Index()
        {
            Random random = new Random();

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.SetTableLength(100);

            int count = vendorUsersPage.getRecordCount();

            UserTableRecord user = vendorUsersPage.GetUserByIndex(random.Next(count));
            user.display();
            Assert.NotNull(user);

            user = vendorUsersPage.GetUserByIndex(-1);
            Assert.IsNull(user);

            user = vendorUsersPage.GetUserByIndex(count + 1);
            Assert.IsNull(user);
        }


        [Test]
        public void Index_Of_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.SetTableLength(100);

            //record found
            vendorUsersPage.EnterSearchTerm(AppUsers.VENDOR_ADMIN.Username);
            Assert.AreNotEqual(-1, vendorUsersPage.indexOfUser(AppUsers.VENDOR_ADMIN.Username));

            //record not found
            Assert.AreEqual(-1, vendorUsersPage.indexOfUser(NameGenerator.GenerateUsername(10)));
        }


        [Test]
        public void Open_Edit_User_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.SetTableLength(100);

            Assert.IsTrue(vendorUsersPage.EditUserButtonIsVisible(0));

            vendorUsersPage.ClickEditUserButton(0);

            Assert.IsTrue(vendorUsersPage.EditUserModal.IsOpen);
        }


        [Test]
        public void Click_Username()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.SetTableLength(100);

            vendorUsersPage.ClickUsername(0);

            Assert.IsTrue(vendorUsersPage.EditUserModal.IsOpen);
        }


        [Test]
        public void Add_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();

            vendorUsersPage.ClickAddUser();
            string username = NameGenerator.GenerateUsername(5);
            vendorUsersPage.AddUserSuccess(
               username,
               NameGenerator.GenerateEntityName(5),
               NameGenerator.GenerateEntityName(5),
               "6612200748",
               (int)UserRoleSelect.ADMIN
            );


            vendorUsersPage.SetTableLength(100);
            vendorUsersPage.EnterSearchTerm(username);
            Assert.AreNotEqual(-1, vendorUsersPage.indexOfUser(username));
        }


        [Test]
        public void Edit_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord userBefore = vendorUsersPage.GetUserByIndex(0);
            vendorUsersPage.ClickEditUserButton(0);

            string newName = NameGenerator.GenerateEntityName(5);
            vendorUsersPage.EditUserSuccess(
               userBefore.Username,
               newName,
               userBefore.LastName,
               "6612200748",
               (int)UserRoleSelect.ADMIN
            );

            Assert.IsFalse(vendorUsersPage.EditUserModal.IsOpen);

            UserTableRecord userAfter = vendorUsersPage.GetUserByIndex(0);
            Assert.AreEqual(userAfter.FirstName, newName);
        }


        [Test]
        public void Lock_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            bool lockedBefore = vendorUsersPage.UserIsLocked(0);
            vendorUsersPage.ToggleLockUser(0);
            bool lockedAfter = vendorUsersPage.UserIsLocked(0);

            Assert.AreNotEqual(lockedAfter, lockedBefore);
        }


        [Test]
        public void Deactivate_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            bool activeBefore = vendorUsersPage.UserIsActive(0);
            vendorUsersPage.ToggleActivateUser(0);
            bool activeAfter = vendorUsersPage.UserIsActive(0);

            Assert.AreNotEqual(activeAfter, activeBefore);
        }


        [Test]
        public void Reset_Password()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            vendorUsersPage.EnterSearchTerm(AppUsers.VENDOR_ADMIN.Username);
            int index = vendorUsersPage.indexOfUser(AppUsers.VENDOR_ADMIN.Username);
            vendorUsersPage.ResetPassword(index);

        }

    }
}