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
    internal class BarcodePageTest
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
        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            DriverFactory.CloseDriver();
        }


        [Test, Description("Verify user is directed to barcodes page when click Admin Key tag tab")]
        public void GoTo_Barcode_Page()
        {

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);
            Assert.NotNull(site);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickBarcodeTab();

            string expectedUrl = baseUrl + "Site/Details?entityId=" + site.Id + "#";
            Assert.AreEqual(expectedUrl, DriverFactory.GetUrl());
        }


        [Test, Description("Verify barcodes are in the correct format PBL-SSSSUUUUU")]
        public void Barcode_Format()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);
            Assert.NotNull(site);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickBarcodeTab();

            barcodePage.AddBarcode();
            string barcode = barcodePage.GetBarcodeByIndex(0);

            //add leading zeroes to siteId if its less than 4 digits
            string input = site.Id.ToString();
            string siteId = Regex.Replace(input, @"\d+", m => m.Value.PadLeft(4, '0'));

            //verify barcode is in the format PBL-SSSSUUUUU
            string pattern = @"PBL-" + siteId + "[0-9a-zA-Z]{5}";
            Regex r = new Regex(pattern);

            Assert.IsTrue(r.IsMatch(barcode));
        }


        [Test, Description("Verify user can delete a barcode from the list")]
        public void Delete_Barcode()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickBarcodeTab();

            //add a barcode first
            barcodePage.AddBarcode();

            string barcode = barcodePage.GetBarcodeByIndex(0);
            barcodePage.DeleteBarcode(0);

            int index = barcodePage.IndexOfBarcode(barcode);

            Assert.AreEqual(-1, index);
            Assert.IsFalse(barcodePage.BarcodeExists(barcode));
        }


        [Test, Description("Verify delete barcode confirmation displays the barcode that will be deleted")]
        public void Delete_Barcode_Dialog()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickBarcodeTab();

            //add a barcode first
            barcodePage.AddBarcode();

            string barcode = barcodePage.GetBarcodeByIndex(0);

            barcodePage.ClickDelete(0);
            string barcodePrompt = barcodePage.BarcodeModal.ParseBarcode();

            Assert.AreEqual(barcode, barcodePrompt);

        }


        [Test, Description("Verify barcode is not deleted if user presses the cancel button")]
        public void Delete_Barcode_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickBarcodeTab();

            //add a barcode first
            barcodePage.AddBarcode();

            string barcode = barcodePage.GetBarcodeByIndex(0);

            //open delete barcode modal
            barcodePage.ClickDelete(0);
            Assert.IsTrue(barcodePage.BarcodeModal.IsOpen);
            //cancel and close the modal
            barcodePage.BarcodeModal.ClickCancel();
            Assert.IsFalse(barcodePage.BarcodeModal.IsOpen);
            //verify barcode still exists
            Assert.IsTrue(barcodePage.BarcodeExists(barcode));
        }


        [Test, Description("Verify user can add a new barcode to the site")]
        public void Add_Barcode()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickBarcodeTab();

            //get # of barcodes in list before adding a barcode
            int countBefore = barcodePage.getRecordCount();

            //add a barcode
            for (int j = 0; j < 10; j++) { barcodePage.AddBarcode(); }
            barcodePage.AddBarcode();

            //get # of barcodes in list after adding a barcode
            int countAfter = barcodePage.getRecordCount();

            Assert.Greater(countAfter, countBefore);

        }


        [Test, Description("Verify table can be sorted by columns in ascending order")]
        public void Sort_Table_Asc()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickBarcodeTab();

            barcodePage.AddBarcode();

            //sort by ID
            barcodePage.SortByColAsc(0);
            IList<string> val = barcodePage.GetValuesForCol(0);
            //convert strings to int before comparing
            IList<int> expectedInt = val.Select(int.Parse).ToList().OrderBy(x => x).ToList();

            for (int i = 0; i < expectedInt.Count; i++)
            {
                Assert.AreEqual(expectedInt[i], int.Parse(val[i]));
            }

            //sort by barcode
            barcodePage.SortByColAsc(1);
            val = barcodePage.GetValuesForCol(1);

            IList<String> expected = val.OrderBy(x => x).ToList();
            expected = val.OrderBy(x => x).ToList();

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by CreatedDate col
            barcodePage.SortByColAsc(2);
            val = barcodePage.GetValuesForCol(2);
            //parse DateTime from strings before comparing
            List<DateTime> actualDate = val.Select(date => DateTime.Parse(date)).ToList();
            IList<DateTime> expectedDate = actualDate.OrderBy(x => x).ToList();

            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expectedDate[i], actualDate[i]);
            }

            //sort by CreatedBy col
            barcodePage.SortByColAsc(3);
            val = barcodePage.GetValuesForCol(3);
            expected = val.OrderBy(x => x).ToList();

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }
        }


        [Test, Description("Verify table can be sorted in descending order")]
        public void Sort_Table_Desc()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickBarcodeTab();

            barcodePage.AddBarcode();

            //sort by ID
            barcodePage.SortByColDesc(0);
            IList<string> val = barcodePage.GetValuesForCol(0);
            //convert strings to int before comparing
            IList<int> expectedInt = val.Select(int.Parse).ToList().OrderByDescending(x => x).ToList();

            for (int i = 0; i < expectedInt.Count; i++)
            {
                Assert.AreEqual(expectedInt[i], int.Parse(val[i]));
            }

            //sort by barcode
            barcodePage.SortByColDesc(1);
            val = barcodePage.GetValuesForCol(1);

            IList<String> expected = val.OrderByDescending(x => x).ToList();
            expected = val.OrderByDescending(x => x).ToList();

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            //sort by CreatedDate col
            barcodePage.SortByColDesc(2);
            val = barcodePage.GetValuesForCol(2);
            //parse DateTime from strings before comparing
            List<DateTime> actualDate = val.Select(date => DateTime.Parse(date)).ToList();
            IList<DateTime> expectedDate = actualDate.OrderByDescending(x => x).ToList();

            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expectedDate[i], actualDate[i]);
            }

            //sort by CreatedBy col
            barcodePage.SortByColDesc(3);
            val = barcodePage.GetValuesForCol(3);
            expected = val.OrderByDescending(x => x).ToList();

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }
        }


        [Test, Description("Verify user can export the barcodes in excel format")]
        public void Excel_Export()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickBarcodeTab();

            //add a barcode
            barcodePage.AddBarcode();

            barcodePage.DownloadExcelFile();

            Assert.IsTrue(barcodePage.FileDownloaded());
        }


        [Test, Description("Verify table records the email of the user who created the barcode")]
        public void Barcode_CreatedBy()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.LOTTERY_ADMIN.Username, AppUsers.LOTTERY_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickBarcodeTab();

            barcodePage.AddBarcode();

            //sort by ID to get newest barcode first
            barcodePage.SortByColDesc(0);
            string createdBy = barcodePage.GetCreatedBy(0);

            Assert.AreEqual(AppUsers.LOTTERY_ADMIN.Username, createdBy);
        }
    }
}
