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
    public class DeviceListPageTest
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
        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            DriverFactory.CloseDriver();
        }


        [Test, Description("Verify device management tab redirects to device list page")]
        public void Goto_Device_List()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            int id = siteList.getSiteIdByIndex(0);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            string expectedUrl = baseUrl + "Site/Details?entityId=" + id + "#devices";
            Assert.AreEqual(DriverFactory.GetUrl(), expectedUrl);
        }


        [Test, Description("Verify user can set the number of records to display in the table")]
        public void Set_Table_Length()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.AddDevice();

            deviceList.SetTableLength(10);
            Assert.LessOrEqual(deviceList.getRecordCount(), 10);

            deviceList.SetTableLength(25);
            Assert.LessOrEqual(deviceList.getRecordCount(), 25);

            deviceList.SetTableLength(50);
            Assert.LessOrEqual(deviceList.getRecordCount(), 50);

            deviceList.SetTableLength(100);
            Assert.LessOrEqual(deviceList.getRecordCount(), 100);
        }

        [Test, Description("Verify user can add a device to a site"), Order(1)]
        public void AddDevice()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.SetTableLength(100);

            int countBefore = deviceList.getRecordCount();
            deviceList.AddDevice();
            int countAfter = deviceList.getRecordCount();

            Assert.Greater(countAfter, countBefore);
        }


        [Test, Description("Verify user is directed to device details page when clicking the edit button")]
        public void Goto_Device_Details()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.AddDevice();


            deviceList.ClickEditDevice(0);

            Assert.IsTrue(deviceDetails.PageLoaded());
        }


        [Test, Description("Verify user is directed to device details page when clicking the device ID link")]
        public void Click_DeviceId()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.AddDevice();

            deviceList.ClickDevice(0);

            Assert.IsTrue(deviceDetails.PageLoaded());
        }


        [Test]
        public void Index_Of_Device_Found()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.AddDevice();

            int count = deviceList.getRecordCount();
            int index = new Random().Next(count);

            DeviceTableRecord device = deviceList.GetDeviceByIndex(index);
            int foundAt = deviceList.IndexOfDevice(device.DeviceId);

            Assert.AreNotEqual(-1, foundAt);
        }


        [Test]
        public void Index_Of_Device_NotFound()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.AddDevice();

            int id = new Random().Next(1000, 2000);
            int foundAt = deviceList.IndexOfDevice(id);

            Assert.AreEqual(-1, foundAt);
        }


        [Test, Description("Verify add device modal displays the site name the devie is being added to ")]
        public void AddDevice_Modal_SiteName()
        {

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.ClickAddDevice();
            string siteName = deviceList.AddDeviceModal.GetSiteNameField();

            Assert.AreEqual(site.SiteName, siteName);
        }


        [Test, Description("Verify device serial # can be edited"), Order(2)]
        public void Edit_Serial_Number()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.SetTableLength(100);
            string serialBefore = deviceList.GetSerialNumber(0);

            deviceList.ClickEditDevice(0);

            string newSerial = NameGenerator.GenerateEntityName(5);

            deviceDetails.EnterSerialNumber(newSerial);
            deviceDetails.EnterTerminal1Id(NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterTerminal2Id(NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterTerminal3Id(NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterTerminal4Id(NameGenerator.GenerateEntityName(5));
            deviceDetails.ClickSubmit();

            Assert.IsTrue(deviceDetails.SuccessBannerIsDisplayed());

            deviceDetails.GoBackToDeviceList();

            deviceList.SetTableLength(100);
            string serialAfter = deviceList.GetSerialNumber(0);

            Assert.AreEqual(newSerial, serialAfter);
            Assert.AreNotEqual(serialBefore, serialAfter);
        }


        [Test, Description("Verify device list table can be searched")]
        public void Search_List()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.SetTableLength(100);

            deviceList.AddDevice();
            int count = deviceList.getRecordCount();
            deviceList.ClickEditDevice(0);

            //add a serial number to device 
            string SN = NameGenerator.GenerateEntityName(5);
            deviceDetails.EnterSerialNumber(SN);
            deviceDetails.EnterExternalTerminalId(0, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(1, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(2, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(3, NameGenerator.GenerateEntityName(5));
            deviceDetails.ClickSubmit();
            deviceDetails.GoBackToDeviceList();

            deviceList.SetTableLength(100);
            DeviceTableRecord device = deviceList.GetDeviceByIndex(0);

            //search by deviceId
            deviceList.EnterSearchTerm(device.DeviceId.ToString());

            Assert.Greater(deviceList.getRecordCount(), 0);

            //search by site Id
            deviceList.EnterSearchTerm(site.Id.ToString());
            Assert.AreEqual(count, deviceList.getRecordCount());

            //search by serial number
            deviceList.EnterSearchTerm(device.SerialNumber);
            Assert.Greater(deviceList.getRecordCount(), 0);
        }


        [Test, Description("Verify device list table can be searched")]
        public void Search_List_Zero_Results()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.SetTableLength(100);
            deviceList.AddDevice();

            //search by deviceId
            string searchTerm = NameGenerator.GenerateEntityName(10);
            deviceList.EnterSearchTerm(searchTerm);

            Assert.Zero(deviceList.getRecordCount());

        }


        [Test]
        public void Sort_Table_Ascending()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.SetTableLength(100);

            deviceList.SortByColAsc(0);
            //sort by email asc
            IList<string> val = deviceList.GetValuesForCol(0);

            IList<string> expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            deviceList.SortByColAsc(1);
            //sort by first name asc
            val = deviceList.GetValuesForCol(1);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            deviceList.SortByColAsc(2);
            //sort by last name asc
            val = deviceList.GetValuesForCol(2);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {
                if (expected[i].ToLower() == "n/a" || val[i].ToLower() == "n/a")
                {
                    continue;
                }

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            deviceList.SortByColAsc(3);
            //sort by role asc
            val = deviceList.GetValuesForCol(3);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            deviceList.SortByColAsc(4);
            //sort by locked asc
            val = deviceList.GetValuesForCol(4);

            expected = val.OrderBy(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

        }


        [Test]
        public void Sort_Table_Descending()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.SetTableLength(100);

            deviceList.SortByColDesc(0);
            //sort by email asc
            IList<string> val = deviceList.GetValuesForCol(0);

            IList<string> expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            deviceList.SortByColDesc(1);
            //sort by first name asc
            val = deviceList.GetValuesForCol(1);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            deviceList.SortByColDesc(2);
            //sort by last name asc
            val = deviceList.GetValuesForCol(2);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {
                if (expected[i].ToLower() == "n/a" || val[i].ToLower() == "n/a")
                {
                    continue;
                }
                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            deviceList.SortByColDesc(3);
            //sort by role asc
            val = deviceList.GetValuesForCol(3);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }


            deviceList.SortByColDesc(4);
            //sort by locked asc
            val = deviceList.GetValuesForCol(4);

            expected = val.OrderByDescending(x => x).ToList();
            for (int i = 0; i < expected.Count; i++)
            {

                Assert.AreEqual(expected[i].ToLower(), val[i].ToLower());
            }

        }




        [Test, Ignore("Skipping test case 'Open_EditDevice_Modal'. Edit device modal is specific to DMB TMS")]
        public void Open_EditDevice_Modal()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.SetTableLength(100);
            deviceList.AddDevice();

            Assert.IsFalse(deviceList.EditDeviceModal.IsOpen);
            deviceList.ClickEditDevice(0);
            Assert.IsTrue(deviceList.EditDeviceModal.IsOpen);

        }


        [Test, Ignore("Skipping test case. Edit device modal is specific to DMB TMS")]
        public void EditDevicePreopulatedData()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.SetTableLength(100);

            deviceList.AddDevice();
            DeviceTableRecord device = deviceList.GetDeviceByIndex(0);

            deviceList.ClickEditDevice(0);
            Assert.IsTrue(deviceList.EditDeviceModal.DeviceMatches(device, site));
        }


        [Test]
        public void Site_Device_Count()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);


            //add devices to site
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();
            int numDevices = new Random().Next(20);
            for (int i = 0; i < numDevices; i++) { deviceList.AddDevice(); }
            navMenu.ClickSites();

            siteList.SetTableLength(100);
            SiteTableRecord site = siteList.GetSiteByIndex(0);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.SetTableLength(100);

            int count = deviceList.getRecordCount();

            Assert.AreEqual(count, site.DeviceCount);
        }


        [Test, Description("Verify devices in the list ahve the correct site ID assigned")]
        public void DeviceList_SiteId()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            List<string> idCol = deviceList.GetValuesForCol(1);
            foreach (string val in idCol)
            {
                Assert.AreEqual(val, site.Id.ToString());
            }
        }


        //test record count
        [Test]
        public void Record_Count()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.AddDevice();

            int count = deviceList.getRecordCount();

            Assert.NotZero(count);
        }


        //test zero record count
        [Test]
        public void Record_Count_Zero()
        {

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickSites();

            //add a new site
            siteList.ClickAddSite();
            string siteName = NameGenerator.GenerateEntityName(5);
            addSite.EnterForm(
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
            addSite.ClickSubmit();
            navMenu.ClickSites();

            siteList.SetTableLength(100);
            siteList.EnterSearchTerm(siteName);

            int index = siteList.indexOfSite(siteName);
            Assert.AreNotEqual(index, -1);

            //go to device list page for new site
            siteList.ClickSiteByIndex(index);
            siteDetails.EntityTabs.ClickDeviceTab();

            //verify list is empty
            int count = deviceList.getRecordCount();
            Assert.AreEqual(0, count);
        }


        //test record exists
        [Test]
        public void Record_Exists_True()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.AddDevice();

            DeviceTableRecord device = deviceList.GetDeviceByIndex(0);

            //search by device ID
            Assert.IsTrue(deviceList.recordExists(device.DeviceId.ToString()));

            //search by site ID
            Assert.IsTrue(deviceList.recordExists(device.SiteId.ToString()));

            //search by serial #
            Assert.IsTrue(deviceList.recordExists(device.SerialNumber));
        }


        [Test]
        public void Matching_Record_Count()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.AddDevice();

            DeviceTableRecord device = deviceList.GetDeviceByIndex(0);

            int deviceIDCount = deviceList.matchingRecordCount(device.DeviceId.ToString());
            Assert.AreEqual(1, deviceIDCount);

            int siteIdCount = deviceList.matchingRecordCount(site.Id.ToString());
            Assert.AreEqual(siteIdCount, deviceList.getRecordCount());

            int serialNumCount = deviceList.matchingRecordCount(device.SerialNumber.ToString());
            Assert.AreEqual(1, serialNumCount);
        }


        [Test]
        public void Matches_For_Column()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);

            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.AddDevice();


            deviceList.ClickEditDevice(0);

            string SN = NameGenerator.GenerateEntityName(5);
            deviceDetails.EnterSerialNumber(SN);
            deviceDetails.EnterExternalTerminalId(0, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(1, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(2, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(3, NameGenerator.GenerateEntityName(5));
            deviceDetails.ClickSubmit();

            deviceDetails.GoBackToDeviceList();
            DeviceTableRecord device = deviceList.GetDeviceByIndex(0);

            Assert.AreEqual(1, deviceList.GetMatchesForCol(device.DeviceId.ToString(), 0));
            Assert.AreEqual(deviceList.getRecordCount(), deviceList.GetMatchesForCol(site.Id.ToString(), 1));
            Assert.AreEqual(1, deviceList.GetMatchesForCol(device.SerialNumber, 2));
        }

    }
}