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
    internal class TicketSalesReportTest
    {
        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private SiteListPage siteList;
        private SiteDetailsEditPage siteDetails;
        private DeviceListPage deviceList;
        private SiteDetailsAddPage addSite;
        private TicketSalesReportPage ticketSalesReport;


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
            ticketSalesReport = new TicketSalesReportPage(DriverFactory.Driver);
        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            DriverFactory.CloseDriver();
        }


        [Test, Description("Verify correct url is received on machine activity report page")]
        public void GoTo_TSR_Page()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to machine activity report page
            navMenu.ClickTicketSalesReport();

            string expectedUrl = baseUrl + "Reports?Id=3";
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

            //go to ticket sales report page
            navMenu.ClickTicketSalesReport();
            ticketSalesReport.DisableSelectAllSites();

            string expectedSite = agentNum + " - " + siteName;

            ticketSalesReport.SelectSiteByValue(expectedSite);
            string selected = ticketSalesReport.GetSelectedSite();
            Assert.AreEqual(expectedSite, selected);
        }


        [Test]
        public void Select_Single_Site()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to ticket sales report page
            navMenu.ClickTicketSalesReport();

            ticketSalesReport.DisableSelectAllSites();
            ticketSalesReport.SelectSiteByIndex(0);

            string selected = ticketSalesReport.GetSelectedSite();

            Assert.NotNull(selected);

        }


        [Test, Ignore("Internal test. Skipping")]
        public void Select_Site_ByIndex_NotFound()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to machine activity report page
            navMenu.ClickTicketSalesReport();

            ticketSalesReport.DisableSelectAllSites();
            ticketSalesReport.SelectSiteByIndex(-1);

            string selected = ticketSalesReport.GetSelectedSite();
            Assert.Null(selected);


            ticketSalesReport.DisableSelectAllSites();
            ticketSalesReport.SelectSiteByIndex(1000);

            selected = ticketSalesReport.GetSelectedSite();
            Assert.Null(selected);
        }


        [Test]
        public void Select_Multiple_Sites_ByIndex()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to machine activity report page
            navMenu.ClickTicketSalesReport();

            ticketSalesReport.DisableSelectAllSites();

            ticketSalesReport.SelectMultipleSitesByIndex(0, 1, 2);
            List<string> selected = ticketSalesReport.GetAllSelectedSites();

            Assert.AreEqual(3, selected.Count);
        }


        [Test]
        public void Select_Site_ByValue()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            SiteTableRecord site = siteList.GetSiteByIndex(0);

            //go to machine activity report page
            navMenu.ClickTicketSalesReport();

            ticketSalesReport.DisableSelectAllSites();
            ticketSalesReport.SelectSiteByValue(site.SiteName);

            string selected = ticketSalesReport.GetSelectedSite();
            Assert.NotNull(selected);
        }


        [Test, Ignore("Internal test. Skipping")]
        public void Select_Site_ByValue_NotFound()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to machine activity report page
            navMenu.ClickTicketSalesReport();

            ticketSalesReport.DisableSelectAllSites();
            ticketSalesReport.SelectSiteByValue(NameGenerator.GenerateEntityName(5));

            string selected = ticketSalesReport.GetSelectedSite();
            Assert.Null(selected);
        }


        [Test]
        public void Select_Multiple_Site_ByValue()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            List<string> siteNames = siteList.GetValuesForCol(0);


            //go to machine activity report page
            navMenu.ClickTicketSalesReport();

            ticketSalesReport.DisableSelectAllSites();
            ticketSalesReport.SelectMultipleSitesByValue(siteNames.GetRange(0, 3).ToArray());

            List<string> selected = ticketSalesReport.GetAllSelectedSites();
            Assert.AreEqual(selected.Count, 3);
        }


        [Test, Ignore("Internal test. Skipping")]
        public void GetSelected()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to machine activity report page
            navMenu.ClickTicketSalesReport();
            ticketSalesReport.DisableSelectAllSites();


            Assert.Null(ticketSalesReport.GetSelectedSite());

            ticketSalesReport.SelectSiteByIndex(0);

            Assert.AreEqual(1, ticketSalesReport.GetAllSelectedSites().Count);

            ticketSalesReport.SelectMultipleSitesByIndex(1, 2, 3);

            Assert.AreEqual(4, ticketSalesReport.GetAllSelectedSites().Count);
        }



        [Test, Description("Verify the default selection for period is set to 'Prior Day'")]
        public void Default_TimePeriod()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickTicketSalesReport();

            //verify default selected time period is 'Previous week'
            PeriodSelectOptions currentSelectedPeriod = ticketSalesReport.GetSelectedPeriod();
            Assert.AreEqual(PeriodSelectOptions.PREV_WEEK, currentSelectedPeriod);
        }


        [Test, Description("Verify Period dropdown has expected options")]
        public void Period_Options()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickTicketSalesReport();

            List<PeriodSelectOptions> periodOptions = ticketSalesReport.GetPeriodOptions();

            Assert.IsTrue(periodOptions.Contains(PeriodSelectOptions.CURRENT_DAY));
            Assert.IsTrue(periodOptions.Contains(PeriodSelectOptions.PREV_DAY));
            Assert.IsTrue(periodOptions.Contains(PeriodSelectOptions.PREV_WEEK));
            Assert.IsTrue(periodOptions.Contains(PeriodSelectOptions.PREV_MONTH));
            Assert.IsTrue(periodOptions.Contains(PeriodSelectOptions.DATE_RANGE));

            ticketSalesReport.SelectPeriodByValue(PeriodSelectOptions.PREV_DAY);
            Assert.AreEqual(PeriodSelectOptions.PREV_DAY, ticketSalesReport.GetSelectedPeriod());

            ticketSalesReport.SelectPeriodByValue(PeriodSelectOptions.CURRENT_DAY);
            Assert.AreEqual(PeriodSelectOptions.CURRENT_DAY, ticketSalesReport.GetSelectedPeriod());

            ticketSalesReport.SelectPeriodByValue(PeriodSelectOptions.PREV_WEEK);
            Assert.AreEqual(PeriodSelectOptions.PREV_WEEK, ticketSalesReport.GetSelectedPeriod());

            ticketSalesReport.SelectPeriodByValue(PeriodSelectOptions.PREV_MONTH);
            Assert.AreEqual(PeriodSelectOptions.PREV_MONTH, ticketSalesReport.GetSelectedPeriod());

            ticketSalesReport.SelectPeriodByValue(PeriodSelectOptions.DATE_RANGE);
            Assert.AreEqual(PeriodSelectOptions.DATE_RANGE, ticketSalesReport.GetSelectedPeriod());

        }



        [Test, Description("Verify report can be exported to CSV format")]
        public void CSV_Export()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickTicketSalesReport();

            ticketSalesReport.EnableSelectAllSites();
            ticketSalesReport.SelectPeriodByValue(PeriodSelectOptions.PREV_MONTH);

            ticketSalesReport.ClickRunReport();
            Assert.Greater(ticketSalesReport.getRecordCount(), 0);

            ticketSalesReport.ClickCsvExport();
            Assert.IsTrue(ticketSalesReport.CsvFileDownloaded());
        }


        [Test, Description("Verify report can be exported to Excel format")]
        public void Excel_Export()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickTicketSalesReport();

            ticketSalesReport.EnableSelectAllSites();
            ticketSalesReport.SelectPeriodByValue(PeriodSelectOptions.PREV_MONTH);

            ticketSalesReport.ClickRunReport();
            Assert.Greater(ticketSalesReport.getRecordCount(), 0);

            ticketSalesReport.ClickExcelExport();
            Assert.IsTrue(ticketSalesReport.ExcelFileDownloaded());
        }


        [Test, Description("Verify report can be exported to PDF format")]
        public void PDF_Export()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickTicketSalesReport();


            ticketSalesReport.EnableSelectAllSites();
            ticketSalesReport.SelectPeriodByValue(PeriodSelectOptions.PREV_MONTH);

            ticketSalesReport.ClickRunReport();
            Assert.Greater(ticketSalesReport.getRecordCount(), 0);

            ticketSalesReport.ClickPdfExport();
            Assert.IsTrue(ticketSalesReport.PdfFileDownloaded());
        }


        [Test]
        public void Site_Error()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickTicketSalesReport();

            ticketSalesReport.DisableSelectAllSites();

            Assert.IsFalse(ticketSalesReport.SiteErrorIsDisplayed());
            ticketSalesReport.ClickRunReport();
            Assert.IsTrue(ticketSalesReport.SiteErrorIsDisplayed());

        }




        [Test]
        public void Date_Error()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickTicketSalesReport();

            ticketSalesReport.EnableSelectAllSites();

            //date range is empty
            Assert.IsFalse(ticketSalesReport.DateErrorIsDisplayed());
            ticketSalesReport.EnterDateRange("", "");
            ticketSalesReport.ClickRunReport();
            Assert.IsTrue(ticketSalesReport.DateErrorIsDisplayed());

            //invalid date range - start date is after end date
            ticketSalesReport.EnterDateRange("6/14/2022", "4/24/2022");
            ticketSalesReport.ClickRunReport();

            Assert.IsTrue(ticketSalesReport.DateErrorIsDisplayed());

        }


        [Test]
        public void Set_Table_Length()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickTicketSalesReport();

            ticketSalesReport.EnableSelectAllSites();
            ticketSalesReport.SelectPeriodByValue(PeriodSelectOptions.PREV_MONTH);
            ticketSalesReport.GenerateReport();

            Assert.Greater(ticketSalesReport.getRecordCount(), 0);

            ticketSalesReport.SetTableLength(10);
            Assert.LessOrEqual(ticketSalesReport.getRecordCount(), 10);

            ticketSalesReport.SetTableLength(25);
            Assert.LessOrEqual(ticketSalesReport.getRecordCount(), 25);

            ticketSalesReport.SetTableLength(50);
            Assert.LessOrEqual(ticketSalesReport.getRecordCount(), 50);

            ticketSalesReport.SetTableLength(100);
            Assert.LessOrEqual(ticketSalesReport.getRecordCount(), 100);


        }


        [Test]
        public void Verify_Columns()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickTicketSalesReport();

            ticketSalesReport.EnableSelectAllSites();
            ticketSalesReport.SelectPeriodByValue(PeriodSelectOptions.PREV_MONTH);
            ticketSalesReport.GenerateReport();

            Assert.Greater(ticketSalesReport.getRecordCount(), 0);
            Assert.AreEqual(9, ticketSalesReport.GetColCount());

            Assert.AreEqual("Site Id", ticketSalesReport.GetHeaderByColNum(0));
            Assert.AreEqual("Agent Number", ticketSalesReport.GetHeaderByColNum(1));
            Assert.AreEqual("Site Name", ticketSalesReport.GetHeaderByColNum(2));
            Assert.AreEqual("Device Id", ticketSalesReport.GetHeaderByColNum(3));
            Assert.AreEqual("Lottery Game Id", ticketSalesReport.GetHeaderByColNum(4));
            Assert.AreEqual("Ticket Price", ticketSalesReport.GetHeaderByColNum(5));
            Assert.AreEqual("Game Name", ticketSalesReport.GetHeaderByColNum(6));
            Assert.AreEqual("# Tickets Sold", ticketSalesReport.GetHeaderByColNum(7));
            Assert.AreEqual("Total Amount Sold", ticketSalesReport.GetHeaderByColNum(8));
        }


        [Test]
        public void Select_All_Sites()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to machine activity report page
            navMenu.ClickTicketSalesReport();

            ticketSalesReport.EnableSelectAllSites();
            Assert.IsTrue(ticketSalesReport.SelectAllIsEnabled);

            ticketSalesReport.DisableSelectAllSites();
            Assert.IsFalse(ticketSalesReport.SelectAllIsEnabled);

        }

        [Test]
        public void Verify_Tickets_Sold()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to machine activity report page
            navMenu.ClickTicketSalesReport();

            ticketSalesReport.EnableSelectAllSites();
            ticketSalesReport.SelectPeriodByValue(PeriodSelectOptions.PREV_MONTH);
            ticketSalesReport.GenerateReport();

            Assert.Greater(ticketSalesReport.getRecordCount(), 0);

            ticketSalesReport.SetTableLength(100);

            TicketSalesReportRecord record = ticketSalesReport.GetRecordByIndex(0);
            Assert.NotNull(record);

            double expected = record.TicketPrice * record.NumTicketsSold;
            Assert.AreEqual(expected, record.TotalAmount);

        }


        [Test]
        public void Total_Tickets_Sold()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to machine activity report page
            navMenu.ClickTicketSalesReport();

            ticketSalesReport.EnableSelectAllSites();
            ticketSalesReport.SelectPeriodByValue(PeriodSelectOptions.PREV_MONTH);
            ticketSalesReport.GenerateReport();

            Assert.Greater(ticketSalesReport.getRecordCount(), 0);
            ticketSalesReport.SetTableLength(100);

            int count = 0;
            int recordCount = ticketSalesReport.getRecordCount();
            int totalCount = ticketSalesReport.GetTotalNumTicketsSold();

            for (int i = 0; i < recordCount; i++)
            {
                TicketSalesReportRecord record = ticketSalesReport.GetRecordByIndex(i);
                Assert.NotNull(record);
                count += record.NumTicketsSold;
            }

            Assert.AreEqual(totalCount, count);
        }


        [Test]
        public void Total_Amount_Sold()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to machine activity report page
            navMenu.ClickTicketSalesReport();

            ticketSalesReport.EnableSelectAllSites();
            ticketSalesReport.SelectPeriodByValue(PeriodSelectOptions.PREV_MONTH);
            ticketSalesReport.GenerateReport();

            Assert.Greater(ticketSalesReport.getRecordCount(), 0);
            ticketSalesReport.SetTableLength(100);

            double count = 0;
            int recordCount = ticketSalesReport.getRecordCount();
            double totalCount = ticketSalesReport.GetTotalAmountSold();

            for (int i = 0; i < recordCount; i++)
            {
                TicketSalesReportRecord record = ticketSalesReport.GetRecordByIndex(i);
                Assert.NotNull(record);
                count += record.TotalAmount;
            }

            Assert.AreEqual(totalCount, count);
        }
    }
}
