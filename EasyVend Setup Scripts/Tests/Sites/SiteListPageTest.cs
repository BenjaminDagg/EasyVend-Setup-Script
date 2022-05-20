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
    public class SiteListPageTest
    {

        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private SiteListPage siteList;
        private SiteDetailsAddPage siteDetails;
        private DeviceListPage deviceList;
        private SiteDetailsEditPage siteDetailsEdit;


        [SetUp]
        public void Setup()
        {

            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            siteList = new SiteListPage(DriverFactory.Driver);
            siteDetails = new SiteDetailsAddPage(DriverFactory.Driver);
            deviceList = new DeviceListPage(DriverFactory.Driver);
            siteDetailsEdit = new SiteDetailsEditPage(DriverFactory.Driver);

        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            DriverFactory.CloseDriver();
        }


        [Test, Description("Verify correct url is returned from site page"), Order(1)]
        public void Goto_SiteList()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();

            string expectedUrl = baseUrl + "Site";

            Assert.AreEqual(DriverFactory.GetUrl(), expectedUrl);
        }


        [Test, Description("Verify a new site is created and added to the site list"), Order(2)]
        public void AddSite()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //verify site is not in list before adding site
            string newSiteName = NameGenerator.GenerateEntityName(5);
            int indexBefore = siteList.indexOfSite(newSiteName);

            Assert.AreEqual(indexBefore, -1);

            siteList.ClickAddSite();

            //enter form to create the new site successfully 
            siteDetails.EnterForm(newSiteName, NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.successBanerIsVisible());
            SiteTableRecord newSite = siteDetails.GetSiteRecord();
            newSite.display();

            //verify site was added to the list
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.EnterSearchTerm(newSiteName);
            int indexAfter = siteList.indexOfSite(newSiteName);

            Assert.AreNotEqual(indexAfter, -1);

            //verify site matches the info that was entered
            SiteTableRecord site = siteList.GetSiteByIndex(indexAfter);

            Assert.IsTrue(
                newSite.SiteName == site.SiteName &&
                newSite.AgentNumber == site.AgentNumber &&
                newSite.Phone == site.Phone
            );

        }


        [Test, Description("Verify record count returns correct value")]
        public void Record_Count()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            string siteName = NameGenerator.GenerateEntityName(5);
            siteDetails.EnterForm(
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
            siteDetails.ClickSubmit();
            navMenu.ClickSites();

            siteList.SetTableLength(100);
            Assert.Greater(siteList.getRecordCount(), 0);
        }


        [Test]
        public void Click_AddSite()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();

            string expectedUrl = baseUrl + "Site/AddSite";

            siteList.ClickAddSite();
            Assert.AreEqual(expectedUrl, DriverFactory.GetUrl());
        }


        [Test]
        public void Click_Site_Name()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            int id = siteList.getSiteIdByIndex(0);
            string expectedUrl = baseUrl + "Site/Details?entityId=" + id;

            siteList.ClickSiteByIndex(0);
            Assert.AreEqual(expectedUrl, DriverFactory.GetUrl());
        }


        [Test]
        public void Get_Site_Record_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);
            site.display();
            Assert.NotNull(site);
            Assert.NotNull(site.SiteName);
            Assert.NotZero(site.Id);
            Assert.NotNull(site.Lottery);
            Assert.NotNull(site.AgentNumber);
            Assert.NotNull(site.Id);
            Assert.NotNull(site.Phone);
        }

        [Test]
        public void Get_Site_Record_Fail()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            int count = siteList.getRecordCount();

            SiteTableRecord site = siteList.GetSiteByIndex(-1);
            Assert.IsNull(site);

            site = siteList.GetSiteByIndex(count + 1);
            Assert.IsNull(site);
        }


        [Test]
        public void Set_Table_Length()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickSites();

            siteList.SetTableLength(10);
            Assert.LessOrEqual(siteList.getRecordCount(), 10);

            siteList.SetTableLength(25);
            Assert.LessOrEqual(siteList.getRecordCount(), 25);

            siteList.SetTableLength(50);
            Assert.LessOrEqual(siteList.getRecordCount(), 50);

            siteList.SetTableLength(100);
            Assert.LessOrEqual(siteList.getRecordCount(), 100);
        }


        [Test]
        public void Search_Record_Exists()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);

            siteList.EnterSearchTerm(site.SiteName);
            Assert.Greater(siteList.getRecordCount(), 0);

            siteList.EnterSearchTerm(site.Lottery);
            Assert.Greater(siteList.getRecordCount(), 0);

            siteList.EnterSearchTerm(site.AgentNumber);
            Assert.Greater(siteList.getRecordCount(), 0);

            siteList.EnterSearchTerm(site.DeviceCount.ToString());
            Assert.Greater(siteList.getRecordCount(), 0);

            siteList.EnterSearchTerm(site.Phone.Replace("-", ""));
            Assert.Greater(siteList.getRecordCount(), 0);
        }


        [Test, Description("Verify error is not displayed if record is not found")]
        public void Search_Zero_Results()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.EnterSearchTerm(NameGenerator.GenerateEntityName(10));
            Assert.Zero(siteList.getRecordCount());
        }


        [Test]
        public void Sort_Column_Asc()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.SortByColAsc(0);
            //sort by site name
            IList<string> val = siteList.GetValuesForCol(0);

            IList<string> expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            siteList.SortByColAsc(1);
            //sort by site ID
            val = siteList.GetValuesForCol(1);

            expected = val.OrderBy(x => int.Parse(x)).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            siteList.SortByColAsc(2);
            //sort by lottery
            val = siteList.GetValuesForCol(2);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            siteList.SortByColAsc(3);
            //sort by role asc
            val = siteList.GetValuesForCol(3);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            siteList.SortByColAsc(4);
            //sort by locked asc
            val = siteList.GetValuesForCol(4);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            siteList.SortByColAsc(5);
            //sort by active asc
            val = siteList.GetValuesForCol(5);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

            siteList.SortByColAsc(6);
            //sort by active asc
            val = siteList.GetValuesForCol(6);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }
        }


        [Test]
        public void Sort_Column_Desc()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.SortByColDesc(0);
            //sort by site name
            IList<string> val = siteList.GetValuesForCol(0);

            IList<string> expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            siteList.SortByColDesc(1);
            //sort by site ID
            val = siteList.GetValuesForCol(1);

            expected = val.OrderByDescending(x => int.Parse(x)).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            siteList.SortByColDesc(2);
            //sort by lottery
            val = siteList.GetValuesForCol(2);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            siteList.SortByColDesc(3);
            //sort by role asc
            val = siteList.GetValuesForCol(3);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            siteList.SortByColDesc(4);
            //sort by locked asc
            val = siteList.GetValuesForCol(4);

            expected = val.OrderByDescending(x => int.Parse(x)).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            siteList.SortByColDesc(5);
            //sort by active asc
            val = siteList.GetValuesForCol(5);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

            siteList.SortByColDesc(6);
            //sort by active asc
            val = siteList.GetValuesForCol(6);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }
        }


        [Test, Description("Verify the edit site button is visible in the actions column")]
        public void EditSiteButton_Visible()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            int count = siteList.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(siteList.EditSiteButtonIsVisible(i));
            }
        }


        [Test]
        public void Click_EditSite()
        {

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            int id = siteList.getSiteIdByIndex(0);
            string expectedUrl = baseUrl + "Site/Details?entityId=" + id + "#details";
            siteList.ClickEditSiteButton(0);

            Assert.AreEqual(expectedUrl, DriverFactory.GetUrl());

        }


        [Test]
        public void Get_SiteId_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            int id = siteList.getSiteIdByIndex(0);

            Assert.Greater(id, 0);
            Assert.NotNull(id);
        }

        [Test]
        public void Get_SiteId_Fail()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            int count = siteList.getRecordCount();

            int id = siteList.getSiteIdByIndex(-1);
            Assert.AreEqual(-1, id);

            id = siteList.getSiteIdByIndex(count + 1);
            Assert.AreEqual(-1, id);
        }


        [Test, Description("Verify device count is zero for new devices")]
        public void Get_DeviceCount_Zero()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //verify site is not in list before adding site
            string newSiteName = NameGenerator.GenerateEntityName(5);
            Console.WriteLine(newSiteName);

            siteList.ClickAddSite();

            //enter form to create the new site successfully 
            siteDetails.EnterForm(newSiteName, NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.successBanerIsVisible());
            SiteTableRecord newSite = siteDetails.GetSiteRecord();
            newSite.display();

            //verify site was added to the list
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.EnterSearchTerm(newSiteName);
            int index = siteList.indexOfSite(newSiteName);

            Assert.AreNotEqual(index, -1);

            int deviceCount = siteList.getDeviceCountByIndex(index);

            Assert.Zero(deviceCount);

        }


        [Test, Description("Verify get device count returns correct amount")]
        public void Get_DeviceCount()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //add a new site
            siteList.ClickAddSite();
            string newSiteName = NameGenerator.GenerateEntityName(5);
            //enter form to create the new site successfully 
            siteDetails.EnterForm(newSiteName, NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.successBanerIsVisible());

            //verify site was added to the list
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.EnterSearchTerm(newSiteName);
            int index = siteList.indexOfSite(newSiteName);
            Assert.AreNotEqual(-1, index);

            //add a device to the new site
            siteList.ClickSiteByIndex(index);
            siteDetailsEdit.EntityTabs.ClickDeviceTab();
            for (int i = 0; i < 5; i++) { deviceList.AddDevice(); }

            //verify device count incremented
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.EnterSearchTerm(newSiteName);
            index = siteList.indexOfSite(newSiteName);
            int deviceCount = siteList.getDeviceCountByIndex(index);

            Assert.Greater(deviceCount, 0);
        }


        [Test]
        public void Filter_Column()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);

            siteList.EnterColumnFilter(site.SiteName, 0);
            Assert.Greater(siteList.getRecordCount(), 0);
            siteList.ClearColumnFilter(0);

            siteList.EnterColumnFilter(site.Id.ToString(), 1);
            Assert.Greater(siteList.getRecordCount(), 0);
            siteList.ClearColumnFilter(1);

            siteList.EnterColumnFilter(site.Lottery, 2);
            Assert.Greater(siteList.getRecordCount(), 0);
            siteList.ClearColumnFilter(2);

            siteList.EnterColumnFilter(site.AgentNumber, 3);
            Assert.Greater(siteList.getRecordCount(), 0);
            siteList.ClearColumnFilter(3);

            siteList.EnterColumnFilter(site.DeviceCount.ToString().ToString(), 4);
            Assert.Greater(siteList.getRecordCount(), 0);
            siteList.ClearColumnFilter(4);

            siteList.EnterColumnFilter(site.Phone, 5);
            Assert.Greater(siteList.getRecordCount(), 0);
            siteList.ClearColumnFilter(5);
        }


        [Test, Description("Verify indexOfSite returns index when site is found")]
        public void IndexOfSite_Record_Exists()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);


            int index = siteList.indexOfSite(site.SiteName);
            Assert.AreNotEqual(-1, index);
        }



        [Test, Description("Verify indexOfSite returns -1 when site is not found")]
        public void IndexOfSite_Record_NotFound()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            string siteName = NameGenerator.GenerateEntityName(10);
            siteList.EnterSearchTerm(siteName);
            int index = siteList.indexOfSite(siteName);

            Assert.AreEqual(-1, index);
        }


        [Test, Description("Verify site users can only see sites they are assigned to")]
        public void SiteUser_Permission()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_ADMIN.Username, AppUsers.SITE_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            Assert.AreEqual(1, siteList.getRecordCount());
        }


        [Test, Description("Verify add site button is visible for vendor admin user")]
        public void VendorAdmin_AddSite()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();

            Assert.IsTrue(siteList.AddSiteIsVisible());
        }


        [Test, Description("Verify add site button is visible for lottery admin user")]
        public void LotteryAdmin_AddSite()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.LOTTERY_ADMIN.Username, AppUsers.LOTTERY_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();

            Assert.IsTrue(siteList.AddSiteIsVisible());
        }


        [Test, Description("Verify add site button is not visible for site admin user")]
        public void SiteAdmin_AddSite()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_ADMIN.Username, AppUsers.SITE_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();

            Assert.IsFalse(siteList.AddSiteIsVisible());
        }


        [Test, Description("Verify add site button is not visible for site report user")]
        public void SiteReport_AddSite()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();

            Assert.IsFalse(siteList.AddSiteIsVisible());
        }


    }
}