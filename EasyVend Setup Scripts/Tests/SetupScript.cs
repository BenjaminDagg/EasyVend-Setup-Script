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
using System.IO;
using System.Runtime.InteropServices;

using Excel = Microsoft.Office.Interop.Excel;

namespace EasyVend_Setup_Scripts
{
    class SetupScript
    {
        string baseUrl;
        string excelPath;

        ExcelReader excelData;

        private LoginPage loginPage;
        private NavMenu navMenu;
        private SiteListPage siteList;
        private SiteDetailsEditPage siteDetails;
        private DeviceListPage deviceList;
        private SiteDetailsAddPage addSite;
        private VendorDetailsPage vendorDetails;
        private VendorUsersPage vendorUsers;
        private LotteryDetailsPage lotteryDetails;
        private LotteryUsersPage lotteryUsers;
        private SiteUsersPage siteUsers;
        private UserManagementPage userManagement;

        [SetUp]
        public void Setup()
        {
            DriverFactory.InitDriver();

            baseUrl = ConfigurationManager.AppSettings["URL"];
            excelPath = ConfigurationManager.AppSettings["ExcelPath"];

            excelData = new ExcelReader(excelPath,1);

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            siteList = new SiteListPage(DriverFactory.Driver);
            siteDetails = new SiteDetailsEditPage(DriverFactory.Driver);
            deviceList = new DeviceListPage(DriverFactory.Driver);
            addSite = new SiteDetailsAddPage(DriverFactory.Driver);
            vendorDetails = new VendorDetailsPage(DriverFactory.Driver);
            vendorUsers = new VendorUsersPage(DriverFactory.Driver);
            lotteryDetails = new LotteryDetailsPage(DriverFactory.Driver);
            lotteryUsers = new LotteryUsersPage(DriverFactory.Driver);
            siteUsers = new SiteUsersPage(DriverFactory.Driver);
            userManagement = new UserManagementPage(DriverFactory.Driver);
        }

        [TearDown]
        public void EndTest()
        {
            excelData.Close();
            DriverFactory.CloseDriver();
        }

        


        [Test, Description("Add all vendor users from the Excel file")]
        public void Add_Vendor_Users()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.Default.Username, AppUsers.Default.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            excelData.SetSheet((int)ExcelSheets.Users);
            List<UserTableRecord> vendorAdminUsers = excelData.GetUsers()
                .Where(user => user.Role.Contains("Vendor"))
                .ToList();
            
            navMenu.ClickVendor();
            vendorDetails.EntityTabs.ClickUsersTab();
            
            //add each user
            foreach(UserTableRecord user in vendorAdminUsers)
            {
                Console.WriteLine("Adding {0} user {1}", user.Role, user.Username);

                //check if user already exists
                vendorUsers.EnterSearchTerm(user.Username);
                if (vendorUsers.indexOfUser(user.Username) != -1)
                {
                    Console.WriteLine("\t Error adding user {0}. User already exists", user.Username);
                    continue;
                }

                vendorUsers.ClickAddUser();
                vendorUsers.AddUserSuccess(
                    user.Username,
                    user.FirstName,
                    user.LastName,
                    user.Phone,
                    user.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
                 );
                vendorUsers.EnterSearchTerm(user.Username);
                Assert.AreNotEqual(-1, vendorUsers.indexOfUser(user.Username));
            }
            
        }


        [Test, Description("Add all Lottery Admin users from the Exccel file")]
        public void Add_Lottery_Users()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.Default.Username, AppUsers.Default.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            excelData.SetSheet((int)ExcelSheets.Users);
            //List<UserTableRecord> lotteryAdminUsers = excelData.GetUsersByRole("Lottery Admin");
            List<UserTableRecord> lotteryAdminUsers = excelData.GetUsers()
                .Where(user => user.Role.ToLower().Contains("lottery"))
                .ToList();

            navMenu.ClickLottery();
            lotteryDetails.EntityTabs.ClickUsersTab();

            //add each user
            foreach (UserTableRecord user in lotteryAdminUsers)
            {
                Console.WriteLine("Adding {0} user {1}", user.Role, user.Username);

                lotteryUsers.ClearSearchField();
                //check if user already exists
                lotteryUsers.EnterSearchTerm(user.Username);
                if (lotteryUsers.indexOfUser(user.Username) != -1)
                {
                    Console.WriteLine("\t Error adding user {0}. User already exists", user.Username);
                    continue;
                }

                lotteryUsers.ClickAddUser();
                lotteryUsers.AddUserSuccess(
                    user.Username,
                    user.FirstName,
                    user.LastName,
                    user.Phone,
                    user.Role.ToLower().Contains("admin") ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
                 );
                lotteryUsers.EnterSearchTerm(user.Username);
                Assert.AreNotEqual(-1, lotteryUsers.indexOfUser(user.Username));
            }

        }


        [Test, Description("Add all sites from the Excel file"), Order(1)]
        public void Add_Sites()
        {
            List<SiteTableRecord> sites = excelData.GetSites();
            
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.Default.Username, AppUsers.Default.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();

            foreach(SiteTableRecord site in sites)
            {
                Console.WriteLine("Adding site {0}", site.SiteName);

                navMenu.ClickSites();

                //site already exists
                siteList.EnterSearchTerm(site.SiteName);
                if(siteList.indexOfSite(site.SiteName) != -1)
                {
                    Console.WriteLine("\t Error adding site {0}. Site already exists", site.SiteName);
                    continue;
                }

                //add site
                siteList.ClickAddSite();
                addSite.EnterForm(
                    site.SiteName,
                    site.AgentNumber,
                    site.ExternalStoreId,
                    site.FirstName,
                    site.LastName,
                    site.Email,
                    site.Phone,
                    site.Address,
                    site.City,
                    site.Zipcode
                );
                addSite.ClickSubmit();
                Assert.IsTrue(addSite.successBanerIsVisible());

                //verify site was addd
                navMenu.ClickSites();
                siteList.EnterSearchTerm(site.SiteName);
                Assert.AreNotEqual(-1, siteList.indexOfSite(site.SiteName));
            }
        }


        [Test, Description("Add site users to their respective sites")]
        public void Add_Site_Users()
        {
            List<UserTableRecord> users = excelData.GetSiteUsers();
            if(users.Count == 0)
            {
                Assert.Pass();
            }
            

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.Default.Username, AppUsers.Default.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();

            bool userExists = false;
            foreach(UserTableRecord user in users)
            {
                Console.WriteLine("Adding user {0} to site {1}", user.Username, user.SiteName);

                //check if user already exists. If they do add them as existing user
                navMenu.ClickVendor();
                vendorDetails.EntityTabs.ClickUserManagementTab();
                userManagement.EnterSearchTerm(user.Username);
                if(userManagement.indexOfUser(user.Username) != -1)
                {
                    userExists = true;
                }

                string siteName = user.SiteName;

                navMenu.ClickSites();
                //site hasn't been added yet
                siteList.EnterSearchTerm(siteName);
                if(siteList.indexOfSite(siteName) == -1)
                {
                    Console.WriteLine("\t Error adding user {0} to site {1}. Site {2} was not found",user.Username,user.SiteName,user.SiteName);
                    continue;
                }
                siteList.ClickSiteByIndex(0);
                siteDetails.EntityTabs.ClickUsersTab();

                //user already added
                siteUsers.EnterSearchTerm(user.Username);
                if(siteUsers.indexOfUser(user.Username) != -1)
                {
                    Console.WriteLine("\t Error adding user {0} to site {1}. User already exists at this site", user.Username,user.SiteName);
                    continue;
                }

                //add user
                if(userExists == true)
                {
                    siteUsers.ClickAddUser();
                    siteUsers.AddExistingUser(user.Username);
                }
                else
                {
                    siteUsers.ClickAddUser();
                    siteUsers.AddUserSuccess(
                        user.Username,
                        user.FirstName,
                        user.LastName,
                        user.Phone,
                        user.Role.ToLower().Contains("admin") ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
                    );
                }
                

                //verify user was added
                siteUsers.EnterSearchTerm(user.Username);
                Assert.AreNotEqual(-1, siteUsers.indexOfUser(user.Username));

                userExists = false;
            }

        }


        [Test, Description("Add devices for sites")]
        public void Add_Devices()
        {
            List<SiteTableRecord> sites = excelData.GetSites()
                .DistinctBy(site => site.SiteName)
                .ToList();

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.Default.Username, AppUsers.Default.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();

            foreach(SiteTableRecord site in sites)
            {
                navMenu.ClickSites();
                siteList.ClearSearchField();

                //site doesn't have any devices to add
                if(site.DeviceCount == 0)
                {
                    
                    continue;
                }

                siteList.EnterSearchTerm(site.SiteName);

                //site hasn't been added yet
                if(siteList.indexOfSite(site.SiteName) == -1)
                {
                    continue;
                }

                siteList.ClickSiteByIndex(0);
                siteDetails.EntityTabs.ClickDeviceTab();

                //site already has the specified number of devices
                if(deviceList.getRecordCount() >= site.DeviceCount)
                {
                    continue;
                }

                for(int i = 0; i < site.DeviceCount; i++)
                {
                    deviceList.AddDevice();
                }

                navMenu.ClickSites();
            }
        }
    }
}
