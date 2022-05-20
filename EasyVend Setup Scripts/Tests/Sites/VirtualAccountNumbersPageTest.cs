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
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class VirtualAccountNumbersPageTest
    {
        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private SiteListPage siteList;
        private SiteDetailsEditPage siteDetails;
        private DeviceListPage deviceList;
        private SiteTableRecord targetSite;
        private DeviceDetailsPage deviceDetails;
        private SiteDetailsAddPage addSite;
        private BarcodePage barcodePage;
        private VirtualAccountNumbersPage VANPage;



        [SetUp]
        public void Setup()
        {

            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            siteList = new SiteListPage(DriverFactory.Driver);
            siteDetails = new SiteDetailsEditPage(DriverFactory.Driver);
            deviceList = new DeviceListPage(DriverFactory.Driver);
            deviceDetails = new DeviceDetailsPage(DriverFactory.Driver);
            addSite = new SiteDetailsAddPage(DriverFactory.Driver);
            barcodePage = new BarcodePage(DriverFactory.Driver);
            VANPage = new VirtualAccountNumbersPage(DriverFactory.Driver);
        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            DriverFactory.CloseDriver();
        }


        [Test, Description("Verify user is directed to VAN page when click VAN tab")]
        public void GoTo_VAN_Page()
        {

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickVANTab();
            VANPage.waitForTable();
            string expectedUrl = baseUrl + "Site/Details?entityId=" + site.Id + "#";
            Assert.AreEqual(expectedUrl, DriverFactory.GetUrl());
        }


        [Test]
        public void Get_Record_Count()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //add a new site
            siteList.ClickAddSite();
            string newName = NameGenerator.GenerateEntityName(5);
            Console.WriteLine(newName);
            addSite.EnterForm(
                newName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "Ben",
                "Dagg",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "91354"
            );
            addSite.ClickSubmit();

            //go to VAN page for site
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.EnterSearchTerm(newName);
            int siteIndex = siteList.indexOfSite(newName);
            Assert.AreNotEqual(-1, siteIndex);

            siteList.ClickSiteByIndex(siteIndex);

            int vansPerGame = VirtualAccountNumbersPage.VANS_PER_GAME;

            siteDetails.EntityTabs.ClickVANTab();

            //verify record count is 0 when no vans have been generated
            Assert.Zero(VANPage.getRecordCount());

            //verify count is 10 after genearting van for 1 game
            VANPage.GenerateVansForGame(0);
            Assert.AreEqual(vansPerGame, VANPage.getRecordCount());

            //verify count is 20 after genearting van for 1 game
            VANPage.GenerateVansForGame(1);
            Assert.AreEqual(vansPerGame * 2, VANPage.getRecordCount());

            //verify count is 30 after genearting van for 1 game
            VANPage.GenerateVansForGame(2);
            Assert.AreEqual(vansPerGame * 3, VANPage.getRecordCount());

            //verify count is 40 after genearting van for 1 game
            VANPage.GenerateVansForGame(3);
            Assert.AreEqual(vansPerGame * 4, VANPage.getRecordCount());

            VANPage.EnableSharedVan();

            Assert.Zero(VANPage.getRecordCount());
            VANPage.GenerateAllVans();
            Assert.AreEqual(1, VANPage.getRecordCount());
        }


        [Test]
        public void RecordCount_Shared_VAN()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //add a new site
            siteList.ClickAddSite();
            string newName = NameGenerator.GenerateEntityName(5);
            Console.WriteLine(newName);
            addSite.EnterForm(
                newName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "Ben",
                "Dagg",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "91354"
            );
            addSite.ClickSubmit();

            //go to VAN page for site
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.EnterSearchTerm(newName);
            int siteIndex = siteList.indexOfSite(newName);
            Assert.AreNotEqual(-1, siteIndex);

            siteList.ClickSiteByIndex(siteIndex);


            siteDetails.EntityTabs.ClickVANTab();
            VANPage.EnableSharedVan();



            Assert.Zero(VANPage.getRecordCount());
            VANPage.GenerateAllVans();
            Assert.AreEqual(1, VANPage.getRecordCount());

        }


        [Test]
        public void Collapse_Section()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.DisableSharedVan();
            VANPage.GenerateAllVans();
            VANPage.ExpandAll();

            int gameCount = VANPage.GetGameCount();

            int recordsBefore = VirtualAccountNumbersPage.VANS_PER_GAME * gameCount;
            Assert.AreEqual(recordsBefore, VANPage.getRecordCount());

            bool collapsedBefore = VANPage.SectionIsCollapsed(0);
            Assert.IsFalse(collapsedBefore);

            VANPage.CollapseSection(0);

            bool collapsedAfter = VANPage.SectionIsCollapsed(0);
            int recordsAfter = VirtualAccountNumbersPage.VANS_PER_GAME * (gameCount - 1);

            Assert.IsTrue(collapsedAfter);
            Assert.AreEqual(recordsAfter, VANPage.getRecordCount());

        }


        [Test]
        public void ExpandSection()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.DisableSharedVan();
            VANPage.GenerateAllVans();
            VANPage.CollapseAll();

            bool collapsedBefore = VANPage.SectionIsCollapsed(0);
            Assert.True(collapsedBefore);
            Assert.Zero(VANPage.getRecordCount());

            VANPage.ExpandSection(0);

            bool collapsedAfter = VANPage.SectionIsCollapsed(0);
            Assert.False(collapsedAfter);
            Assert.AreEqual(VirtualAccountNumbersPage.VANS_PER_GAME, VANPage.getRecordCount());
        }


        [Test]
        public void Get_Game_Count()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(2);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.DisableSharedVan();

            int count = VANPage.GetGameCount();
            Assert.AreEqual(4, count);

            VANPage.EnableSharedVan();
            Assert.Zero(VANPage.GetGameCount());
        }


        [Test]
        public void Get_Game_UPC()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickVANTab();

            Console.WriteLine(VANPage.GetUPCForGame(0));
        }


        [Test]
        public void Num_VANS_Per_Game()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.DisableSharedVan();
            VANPage.GenerateAllVans();

            IList<string> vans = VANPage.GetVansForSectionByIndex(0);
            Assert.AreEqual(VirtualAccountNumbersPage.VANS_PER_GAME, vans.Count());

            vans = VANPage.GetVansForSectionByIndex(1);
            Assert.AreEqual(VirtualAccountNumbersPage.VANS_PER_GAME, vans.Count());

            vans = VANPage.GetVansForSectionByIndex(2);
            Assert.AreEqual(VirtualAccountNumbersPage.VANS_PER_GAME, vans.Count());

            vans = VANPage.GetVansForSectionByIndex(3);
            Assert.AreEqual(VirtualAccountNumbersPage.VANS_PER_GAME, vans.Count());
        }


        [Test, Description("Verify user can choose to generate VANs for an individual game instead of all VANs")]
        public void Generate_VANS_ForGame()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //add a new site
            siteList.ClickAddSite();
            string newName = NameGenerator.GenerateEntityName(5);
            Console.WriteLine(newName);
            addSite.EnterForm(
                newName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "Ben",
                "Dagg",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "91354"
            );
            addSite.ClickSubmit();

            //go to VAN page for site
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.EnterSearchTerm(newName);
            int siteIndex = siteList.indexOfSite(newName);
            Assert.AreNotEqual(-1, siteIndex);

            siteList.ClickSiteByIndex(siteIndex);


            siteDetails.EntityTabs.ClickVANTab();
            VANPage.DisableSharedVan();

            Assert.Zero(VANPage.getRecordCount());

            //loop throguh games and generated vans for each game
            int gameCount = VANPage.GetGameCount();
            for (int i = 0; i < gameCount; i++)
            {
                VANPage.GenerateVansForGame(i);

                Assert.AreEqual(VirtualAccountNumbersPage.VANS_PER_GAME, VANPage.recordCountForHeader(i));

                int expectedCount = VirtualAccountNumbersPage.VANS_PER_GAME * (i + 1);
                Assert.AreEqual(expectedCount, VANPage.getRecordCount());
            }
            //verify total record count is as e xpected
            Assert.AreEqual(VirtualAccountNumbersPage.VANS_PER_GAME * gameCount, VANPage.getRecordCount());

        }


        [Test]
        public void Generate_All_Vans()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //add a new site
            siteList.ClickAddSite();
            string newName = NameGenerator.GenerateEntityName(5);
            addSite.EnterForm(
                newName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "Ben",
                "Dagg",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "91354"
            );
            addSite.ClickSubmit();

            //go to VAN page for site
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.EnterSearchTerm(newName);
            int siteIndex = siteList.indexOfSite(newName);
            Assert.AreNotEqual(-1, siteIndex);

            siteList.ClickSiteByIndex(siteIndex);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.DisableSharedVan();

            Assert.Zero(VANPage.getRecordCount());

            VANPage.GenerateAllVans();

            int gameCount = VANPage.GetGameCount();
            int expectedCount = VirtualAccountNumbersPage.VANS_PER_GAME * gameCount;

            Assert.AreEqual(expectedCount, VANPage.getRecordCount());
        }


        [Test]
        public void Search_Table()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.DisableSharedVan();
            VANPage.GenerateAllVans();

            IList<string> gameNames = VANPage.GetGames();

            //search by game name
            VANPage.EnterSearchTerm(gameNames[0]);

            Assert.AreEqual(VirtualAccountNumbersPage.VANS_PER_GAME, VANPage.getRecordCount());
            VANPage.ClearSearch();

            //search by game UPC
            string UPC = VANPage.GetUPCForGame(1);
            VANPage.EnterSearchTerm(UPC);

            Assert.AreEqual(VirtualAccountNumbersPage.VANS_PER_GAME, VANPage.getRecordCount());
            VANPage.ClearSearch();

            //search by full VAN
            string van = VANPage.GetVanByIndex(new Random().Next(VANPage.getRecordCount()));
            VANPage.EnterSearchTerm(van);

            Assert.AreEqual(1, VANPage.getRecordCount());
            VANPage.ClearSearch();

            //search by partial van multiple matches
            int expected = VANPage.matchingRecordCount(VirtualAccountNumbersPage.VAN_INN);
            VANPage.EnterSearchTerm(VirtualAccountNumbersPage.VAN_INN);

            Assert.AreEqual(expected, VANPage.getRecordCount());
            VANPage.ClearSearch();

            //zero search results
            VANPage.EnterSearchTerm(NameGenerator.GenerateEntityName(5));
            Assert.Zero(VANPage.getRecordCount());

        }


        [Test]
        public void MatchingRecordCount()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.GenerateAllVans();
            int numVans = VirtualAccountNumbersPage.VANS_PER_GAME;

            //search by game name
            Assert.AreEqual(numVans, VANPage.matchingRecordCount("Diamond"));

            //search by game UPC
            Assert.AreEqual(numVans, VANPage.matchingRecordCount("799366961444"));

            //search for specific VAN
            string van = VANPage.GetVanByIndex(0);
            Assert.AreEqual(1, VANPage.matchingRecordCount(van));
        }


        [Test]
        public void MatchingRecordCount_Shared_VAN()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.EnableSharedVan();

            VANPage.GenerateAllVans();

            string van = VANPage.GetVanByIndex(0);
            Console.WriteLine("Van: {0}", van);
            Assert.AreEqual(1, VANPage.matchingRecordCount(van));
        }


        [Test]
        public void Collapse_All()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.DisableSharedVan();
            VANPage.GenerateAllVans();

            VANPage.CollapseAll();

            int count = VANPage.getRecordCount();
            Assert.Zero(count);

            int gameCount = VANPage.GetGameCount();
            for (int i = 0; i < gameCount; i++)
            {
                Assert.IsTrue(VANPage.SectionIsCollapsed(i));
            }
        }


        [Test]
        public void Expand_All()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.DisableSharedVan();
            VANPage.GenerateAllVans();

            VANPage.ExpandAll();
            VANPage.CollapseAll();


            Assert.Zero(VANPage.getRecordCount());

            int gameCount = VANPage.GetGameCount();
            for (int i = 0; i < gameCount; i++)
            {
                Assert.IsTrue(VANPage.SectionIsCollapsed(i));
            }

            VANPage.ExpandAll();

            int expectedRecordCount = VirtualAccountNumbersPage.VANS_PER_GAME * VANPage.GetGameCount();
            Assert.AreEqual(expectedRecordCount, VANPage.getRecordCount());
            for (int i = 0; i < gameCount; i++)
            {
                Assert.IsFalse(VANPage.SectionIsCollapsed(i));
            }
        }


        [Test]
        public void Enable_Shared_VAN()
        {

            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //add a new site
            siteList.ClickAddSite();
            string newName = NameGenerator.GenerateEntityName(5);
            Console.WriteLine(newName);
            addSite.EnterForm(
                newName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "Ben",
                "Dagg",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "91354"
            );
            addSite.ClickSubmit();

            //go to VAN page for site
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.EnterSearchTerm(newName);
            int siteIndex = siteList.indexOfSite(newName);
            Assert.AreNotEqual(-1, siteIndex);

            siteList.ClickSiteByIndex(siteIndex);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.DisableSharedVan();
            VANPage.GenerateAllVans();

            //verify table displays 10 VANs per game when shared VAN is disabled
            int gameCount = VANPage.GetGameCount();
            int expectedCount = VirtualAccountNumbersPage.VANS_PER_GAME * gameCount;
            Assert.AreEqual(expectedCount, VANPage.getRecordCount());

            VANPage.EnableSharedVan();
            VANPage.GenerateAllVans();

            //verify table only displays 1 VAN when shared VAN is enabled
            Assert.IsTrue(VANPage.SharedVanIsEnabled());
            Assert.LessOrEqual(VANPage.getRecordCount(), 1);

        }


        [Test, Description("Verify table displays 10 VANs per game when shared VAN is disabled")]
        public void Disable_Shared_VAN()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //add a new site
            siteList.ClickAddSite();
            string newName = NameGenerator.GenerateEntityName(5);
            Console.WriteLine(newName);
            addSite.EnterForm(
                newName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "Ben",
                "Dagg",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "91354"
            );
            addSite.ClickSubmit();

            //go to VAN page for site
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.EnterSearchTerm(newName);
            int siteIndex = siteList.indexOfSite(newName);
            Assert.AreNotEqual(-1, siteIndex);

            siteList.ClickSiteByIndex(siteIndex);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.DisableSharedVan();
            VANPage.GenerateAllVans();

            //verify table displays 10 VANs per game when shared VAN is disabled
            int gameCount = VANPage.GetGameCount();
            int expectedCount = VirtualAccountNumbersPage.VANS_PER_GAME * gameCount;
            Assert.AreEqual(expectedCount, VANPage.getRecordCount());

            VANPage.EnableSharedVan();
            VANPage.GenerateAllVans();

            //verify table only displays 1 VAN when shared VAN is enabled
            Assert.IsTrue(VANPage.SharedVanIsEnabled());
            Assert.AreEqual(VANPage.getRecordCount(), 1);

            //switch back to multiple VAN and verify 10 VANs are displayed per game
            VANPage.DisableSharedVan();
            Assert.AreEqual(expectedCount, VANPage.getRecordCount());
        }


        [Test]
        public void GenerateSharedVan()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(8);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.EnableSharedVan();

            Assert.IsTrue(VANPage.SharedVanIsEnabled());

            VANPage.GenerateAllVans();

            Assert.AreEqual(1, VANPage.getRecordCount());

            string van = VANPage.GetVanByIndex(0);

            Assert.NotNull(van);
            Assert.IsTrue(van.Length == 19);
        }


        [Test]
        public void Missing_VAN_Banner()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //add a new site
            siteList.ClickAddSite();
            string newName = NameGenerator.GenerateEntityName(5);
            Console.WriteLine(newName);
            addSite.EnterForm(
                newName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "Ben",
                "Dagg",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "91354"
            );
            addSite.ClickSubmit();

            //go to VAN page for site
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.EnterSearchTerm(newName);
            int siteIndex = siteList.indexOfSite(newName);
            Assert.AreNotEqual(-1, siteIndex);

            siteList.ClickSiteByIndex(siteIndex);
            siteDetails.EntityTabs.ClickVANTab();

            //switch to multiple van  mode
            VANPage.DisableSharedVan();

            Assert.IsTrue(VANPage.MissingVanAlertIsVisible());

            //verify banner is visible while not every game has VANs generated
            VANPage.GenerateVansForGame(0);
            Assert.IsTrue(VANPage.MissingVanAlertIsVisible());

            VANPage.GenerateVansForGame(1);
            Assert.IsTrue(VANPage.MissingVanAlertIsVisible());

            VANPage.GenerateVansForGame(2);
            Assert.IsTrue(VANPage.MissingVanAlertIsVisible());
            //verify banner dissapears after van is generated
            VANPage.GenerateVansForGame(3);
            Assert.IsFalse(VANPage.MissingVanAlertIsVisible());
        }


        [Test, Description("Verify 'Missing VAN' banner is only visible when a shared VAN has not been created")]
        public void Missing_VAN_Banner_Shared()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //add a new site
            siteList.ClickAddSite();
            string newName = NameGenerator.GenerateEntityName(5);
            Console.WriteLine(newName);
            addSite.EnterForm(
                newName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "Ben",
                "Dagg",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "91354"
            );
            addSite.ClickSubmit();

            //go to VAN page for site
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.EnterSearchTerm(newName);
            int siteIndex = siteList.indexOfSite(newName);
            Assert.AreNotEqual(-1, siteIndex);

            siteList.ClickSiteByIndex(siteIndex);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.EnableSharedVan();

            //verify banner is visible if van hasn't been created yet
            Assert.Zero(VANPage.getRecordCount());
            Assert.IsTrue(VANPage.MissingVanAlertIsVisible());

            //generate a shared VAN
            VANPage.GenerateAllVans();

            //verify banner dissapears when a shared VAN has been created
            Assert.AreEqual(1, VANPage.getRecordCount());
            Assert.IsFalse(VANPage.MissingVanAlertIsVisible());
        }


        [Test]
        public void Record_Count_for_Game()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //add a new site
            siteList.ClickAddSite();
            string newName = NameGenerator.GenerateEntityName(5);
            Console.WriteLine(newName);
            addSite.EnterForm(
                newName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "Ben",
                "Dagg",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "91354"
            );
            addSite.ClickSubmit();

            //go to VAN page for site
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.EnterSearchTerm(newName);
            int siteIndex = siteList.indexOfSite(newName);
            Assert.AreNotEqual(-1, siteIndex);

            siteList.ClickSiteByIndex(siteIndex);
            siteDetails.EntityTabs.ClickVANTab();

            //verfy there are no vans generated for game yet
            IList<string> vans = VANPage.GetVansForSectionByIndex(0);
            Assert.Zero(vans.Count);

            VANPage.GenerateVansForGame(0);
            vans = VANPage.GetVansForSectionByIndex(0);
            Assert.AreEqual(VirtualAccountNumbersPage.VANS_PER_GAME, vans.Count);

        }


        [Test, Description("Verify user can export VAN data to Excel file")]
        public void Excel_Export()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(8);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.DisableSharedVan();
            VANPage.GenerateAllVans();

            VANPage.DownloadExcelFile();

            Assert.IsTrue(VANPage.FileDownloaded());
        }


        [Test, Description("Verify table can sort VANs in ascending order")]
        public void Sort_Table_Asc()
        {


            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.GenerateAllVans();
            VANPage.SortByColAsc(0);

            int gameCount = VANPage.GetGameCount();
            for (int i = 0; i < gameCount; i++)
            {
                IList<string> vans = VANPage.GetVansForSectionByIndex(i);
                IList<long> expected = vans.Select(long.Parse).ToList();
                IList<long> sorted = expected.OrderBy(x => x).ToList();
                for (int j = 0; j < expected.Count; j++)
                {
                    Assert.AreEqual(sorted[j], long.Parse(vans[j]));
                }
            }
        }


        [Test, Description("Verify table can be sorted by VANs in descending order")]
        public void Sort_Table_Desc()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.GenerateAllVans();
            VANPage.SortByColDesc(0);

            int gameCount = VANPage.GetGameCount();
            for (int i = 0; i < gameCount; i++)
            {
                IList<string> vans = VANPage.GetVansForSectionByIndex(i);
                IList<long> expected = vans.Select(long.Parse).ToList();
                IList<long> sorted = expected.OrderByDescending(x => x).ToList();
                for (int j = 0; j < expected.Count; j++)
                {
                    Assert.AreEqual(sorted[j], long.Parse(vans[j]));
                }
            }
        }


        [Test]
        public void Game_Generate_VAN()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //add a new site
            siteList.ClickAddSite();
            string newName = NameGenerator.GenerateEntityName(5);
            Console.WriteLine(newName);
            addSite.EnterForm(
                newName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "Ben",
                "Dagg",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "91354"
            );
            addSite.ClickSubmit();

            //go to VAN page for site
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.EnterSearchTerm(newName);
            int siteIndex = siteList.indexOfSite(newName);
            Assert.AreNotEqual(-1, siteIndex);

            siteList.ClickSiteByIndex(siteIndex);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.DisableSharedVan();

            int gameNum = 2;

            Assert.IsTrue(VANPage.GameGenerateVanIsVisible(gameNum));
            VANPage.GenerateVansForGame(gameNum);
            Assert.IsFalse(VANPage.GameGenerateVanIsVisible(gameNum));

        }


        [Test, Description("Verify VANs are in the format NNNNNNNNSSSSUUUUUUZ")]
        public void VAN_Format()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickVANTab();

            VANPage.DisableSharedVan();
            VANPage.GenerateAllVans();

            int expectedCount = VirtualAccountNumbersPage.VANS_PER_GAME * VANPage.GetGameCount();

            //add leading zeroes to siteId if its less than 4 digits
            string input = site.Id.ToString();
            string siteId = Regex.Replace(input, @"\d+", m => m.Value.PadLeft(4, '0'));

            string INN = VirtualAccountNumbersPage.VAN_INN;

            string pattern = INN + siteId + @"\d{7}$";
            Regex r = new Regex(pattern);

            IList<string> vans = VANPage.GetAllVans();
            foreach (string van in vans)
            {
                Assert.IsTrue(r.IsMatch(van));
            }

            Assert.AreEqual(expectedCount, vans.Count);
        }


        [Test, Description("Verify generate shared VAN button is only visible when a shared VAN hasn't been created")]
        public void Shared_VAN_Button_Visibility()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            //add a new site
            siteList.ClickAddSite();
            string newName = NameGenerator.GenerateEntityName(5);
            Console.WriteLine(newName);
            addSite.EnterForm(
                newName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "Ben",
                "Dagg",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "91354"
            );
            addSite.ClickSubmit();

            //go to VAN page for site
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.EnterSearchTerm(newName);
            int siteIndex = siteList.indexOfSite(newName);
            Assert.AreNotEqual(-1, siteIndex);

            siteList.ClickSiteByIndex(siteIndex);
            siteDetails.EntityTabs.ClickVANTab();

            //switch to shared VAN
            VANPage.EnableSharedVan();

            //verify shared VAN button is visible when a VAN hasn't been created yet
            Assert.IsTrue(VANPage.GenerateSharedVanButtonIsVisible());
            //generate a shared VAN
            VANPage.GenerateAllVans();

            //verify shared VAN button is not visible when a VAN has been created yet
            Assert.IsFalse(VANPage.GenerateSharedVanButtonIsVisible());
        }

    }
}
