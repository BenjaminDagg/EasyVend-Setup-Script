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
    internal class MachineActivityReportTest
    {
        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private SiteListPage siteList;
        private SiteDetailsEditPage siteDetails;
        private DeviceListPage deviceList;
        private SiteTableRecord targetSite;
        private SiteDetailsAddPage addSite;
        private MachineActivityReportPage marPage;


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
            addSite = new SiteDetailsAddPage(DriverFactory.Driver);
            marPage = new MachineActivityReportPage(DriverFactory.Driver);
        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            DriverFactory.CloseDriver();
        }


        [Test, Description("Verify correct url is received on machine activity report page")]
        public void GoTo_MAR_Page()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to machine activity report page
            navMenu.ClickMachineActivityReport();

            string expectedUrl = baseUrl + "Reports?Id=1";
            Assert.AreEqual(expectedUrl, DriverFactory.GetUrl());


        }


        [Test, Description("Verify Sites listed in the dropdown are in the format: {agent#} - {Name}")]
        public void SiteDropdown_Format()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.ClickAddSite();

            //add a new site
            string siteName = NameGenerator.GenerateEntityName(5);
            string agentNum = NameGenerator.GenerateEntityName(5);
            addSite.EnterForm(
                siteName,
                agentNum,
                NameGenerator.GenerateEntityName(5),
                "Ben",
                "Dagg",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 Test St",
                "City",
                "91354"
            );
            addSite.ClickSubmit();

            //go to machine activity report page
            navMenu.ClickMachineActivityReport();

            string expectedSite = agentNum + " - " + siteName;

            marPage.SelectSiteByValue(expectedSite);
            Assert.IsTrue(marPage.SiteOptionExists(expectedSite));
        }


        [Test, Description("Verify all of a sites devices are listed in the dropdown")]
        public void Device_List()
        {

            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //get site with the most devices
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.SortByColDesc(4);

            //add 10 devices to the site
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            for (int i = 0; i < 10; i++)
            {
                deviceList.AddDevice();
            }

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.SortByColDesc(4);
            //get site record
            SiteTableRecord site = siteList.GetSiteByIndex(0);

            //go to machine activity report
            navMenu.ClickMachineActivityReport();
            marPage.SelectSiteByValue(site.SiteName);
            List<int> devices = marPage.GetDeviceOptions();

            //verify # of devices in the dropdown equals # of devices registered to the site
            Assert.AreEqual(devices.Count, site.DeviceCount);
        }


        [Test, Description("Verify the default selection for period is set to 'Prior Day'")]
        public void Default_TimePeriod()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickMachineActivityReport();

            //verify default selected time period is 'Previous Day'
            PeriodSelectOptions currentSelectedPeriod = marPage.GetSelectedPeriod();
            Assert.AreEqual(PeriodSelectOptions.PREV_DAY, currentSelectedPeriod);
        }


        [Test, Description("Verify Period dropdown has expected options")]
        public void Period_Options()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickMachineActivityReport();

            List<PeriodSelectOptions> periodOptions = marPage.GetPeriodOptions();

            Assert.IsTrue(periodOptions.Contains(PeriodSelectOptions.CURRENT_DAY));
            Assert.IsTrue(periodOptions.Contains(PeriodSelectOptions.PREV_DAY));
            Assert.IsTrue(periodOptions.Contains(PeriodSelectOptions.PREV_WEEK));
            Assert.IsTrue(periodOptions.Contains(PeriodSelectOptions.DATE_RANGE));

        }



        [Test, Description("Verify report can be exported to CSV format")]
        public void CSV_Export()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickMachineActivityReport();

            marPage.SelectSiteByIndex(0);
            marPage.SelectDeviceByValue(171);
            marPage.SelectPeriodByValue(PeriodSelectOptions.PREV_WEEK);

            marPage.ClickRunReport();

            marPage.ClickCsvExport();
            Assert.IsTrue(marPage.CsvFileDownloaded());
        }


        [Test, Description("Verify report can be exported to Excel format")]
        public void Excel_Export()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickMachineActivityReport();

            marPage.SelectSiteByIndex(0);
            marPage.SelectDeviceByValue(171);
            marPage.SelectPeriodByValue(PeriodSelectOptions.PREV_WEEK);

            marPage.ClickRunReport();

            marPage.ClickExcelExport();
            Assert.IsTrue(marPage.ExcelFileDownloaded());
        }


        [Test, Description("Verify report can be exported to PDF format")]
        public void PDF_Export()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickMachineActivityReport();

            marPage.SelectSiteByIndex(0);
            marPage.SelectDeviceByValue(171);
            marPage.SelectPeriodByValue(PeriodSelectOptions.PREV_WEEK);

            marPage.ClickRunReport();

            marPage.ClickPdfExport();
            Assert.IsTrue(marPage.PdfFileDownloaded());
        }


        [Test]
        public void Site_Error()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickMachineActivityReport();

            marPage.EnterDateRange("3/14/2022", "4/24/2022");
            Assert.IsFalse(marPage.SiteErrorIsDisplayed());
            marPage.ClickRunReport();
            Assert.IsTrue(marPage.SiteErrorIsDisplayed());

        }


        [Test]
        public void Device_Error()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickMachineActivityReport();

            marPage.SelectSiteByIndex(0);
            marPage.EnterDateRange("3/14/2022", "4/24/2022");
            Assert.IsFalse(marPage.DeviceErrorIsDisplayed());
            marPage.ClickRunReport();
            Assert.IsTrue(marPage.DeviceErrorIsDisplayed());

        }


        [Test]
        public void Date_Error()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickMachineActivityReport();

            marPage.SelectSiteByIndex(0);
            marPage.EnterDateRange("6/14/2022", "4/24/2022");
            marPage.SelectDeviceByIndex(0);
            Assert.IsFalse(marPage.DateErrorIsDisplayed());
            marPage.ClickRunReport();
            Assert.IsTrue(marPage.DateErrorIsDisplayed());

        }


        [Test]
        public void Set_Table_Length()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickMachineActivityReport();

            marPage.SelectSiteByIndex(0);
            marPage.SelectDeviceByValue(171);
            marPage.EnterDateRange("1/1/2022", "4/26/2022");
            marPage.GenerateReport();

            marPage.SetTableLength(10);
            Assert.LessOrEqual(marPage.getRecordCount(), 10);

            marPage.SetTableLength(25);
            Assert.LessOrEqual(marPage.getRecordCount(), 25);

            marPage.SetTableLength(50);
            Assert.LessOrEqual(marPage.getRecordCount(), 50);

            marPage.SetTableLength(100);
            Assert.LessOrEqual(marPage.getRecordCount(), 100);


        }


        [Test]
        public void Verify_Columns()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickMachineActivityReport();

            marPage.SelectSiteByIndex(0);
            marPage.SelectDeviceByValue(171);
            marPage.EnterDateRange("1/1/2022", "4/26/2022");
            marPage.GenerateReport();

            Assert.AreEqual(10, marPage.GetColCount());

            Assert.AreEqual("Date / Time", marPage.GetHeaderByColNum(0));
            Assert.AreEqual("Site Id", marPage.GetHeaderByColNum(1));
            Assert.AreEqual("Agent Number", marPage.GetHeaderByColNum(2));
            Assert.AreEqual("Site Name", marPage.GetHeaderByColNum(3));
            Assert.AreEqual("Device Id", marPage.GetHeaderByColNum(4));
            Assert.AreEqual("Activity", marPage.GetHeaderByColNum(5));
            Assert.AreEqual("Lottery Game Id", marPage.GetHeaderByColNum(6));
            Assert.AreEqual("Ticket Price", marPage.GetHeaderByColNum(7));
            Assert.AreEqual("Game Name", marPage.GetHeaderByColNum(8));
            Assert.AreEqual("Drawer Number", marPage.GetHeaderByColNum(9));

        }
    }
}
