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
using System.Threading;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    public class DeviceDetailsPageTest
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


        private int EXTERNAL_TERMINALID_MAX_LENGTH = 16;
        private int SERIAL_NUMBER_MAX_LENGTH = 32;


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


        [Test, Description("Verify user is taken to device details when pressing the edit device button")]
        public void Goto_DeviceDetails()
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

            deviceList.ClickEditDevice(0);

            Assert.IsTrue(deviceDetails.PageLoaded());
        }


        [Test, Description("Verify device details page displays the device ID")]
        public void DeviceDetails_DeviceId()
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

            deviceList.ClickEditDevice(0);
            int deviceId = deviceDetails.GetDeviceId();

            Assert.AreEqual(deviceId, device.DeviceId);
        }


        [Test, Description("Verify device details page displays the serial number ")]
        public void DeviceDetails_SerialNumber()
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

            deviceList.ClickEditDevice(0);
            string sn = deviceDetails.GetSerialNumber();

            Assert.AreEqual(sn, device.SerialNumber);
        }


        [Test, Description("Verify serial number can't be more than 32 characterse")]
        public void DeviceDetails_SerialNumber_Length()
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
            string sn = NameGenerator.GenerateEntityName(SERIAL_NUMBER_MAX_LENGTH + 1);

            deviceDetails.EnterSerialNumber(sn);
            deviceDetails.EnterExternalTerminalId(0, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(1, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(2, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(3, NameGenerator.GenerateEntityName(5));
            deviceDetails.ClickSubmit();

            Assert.IsTrue(deviceDetails.GetSerialNumber().Length <= SERIAL_NUMBER_MAX_LENGTH);
        }


        [Test, Description("Verify user can edit device serial number")]
        public void DeviceDetails_Edit_SN()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.AddDevice();

            DeviceTableRecord deviceBefore = deviceList.GetDeviceByIndex(0);
            string SNBefore = deviceBefore.SerialNumber;

            deviceList.ClickEditDevice(0);

            string newSN = NameGenerator.GenerateEntityName(5);

            deviceDetails.EnterSerialNumber(newSN);
            deviceDetails.EnterTerminal1Id(NameGenerator.GenerateEntityName(3));
            deviceDetails.EnterTerminal2Id(NameGenerator.GenerateEntityName(3));
            deviceDetails.EnterTerminal3Id(NameGenerator.GenerateEntityName(3));
            deviceDetails.EnterTerminal4Id(NameGenerator.GenerateEntityName(3));
            deviceDetails.ClickSubmit();

            deviceDetails.GoBackToDeviceList();
            DeviceTableRecord deviceAfter = deviceList.GetDeviceByIndex(0);
            string SNAfter = deviceAfter.SerialNumber;

            Assert.AreNotEqual(SNBefore, SNAfter);
            Assert.AreEqual(SNAfter, newSN);
        }


        [Test]
        public void DeviceDetails_ActivationDate()
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
            Assert.NotNull(device);


            deviceList.ClickEditDevice(0);

            string activationDate = deviceDetails.GetActivationDate();

            if (device.IsActive)
            {
                Assert.NotNull(activationDate);
            }
            else
            {
                Assert.IsEmpty(activationDate);
            }

        }


        [Test]
        public void DeviceDetails_Clear_ActivationDate()
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
            bool activeBefore = device.IsActive;

            //if not active  already end test
            if (!activeBefore)
            {
                Assert.Pass();
            }


            deviceList.ClickEditDevice(0);

            string activationDate = deviceDetails.GetActivationDate();
            Assert.IsNotEmpty(activationDate);

            //fill out terminal Id if not already filled
            deviceDetails.EnterExternalTerminalId(0, NameGenerator.GenerateEntityName(3));
            deviceDetails.EnterExternalTerminalId(1, NameGenerator.GenerateEntityName(3));
            deviceDetails.EnterExternalTerminalId(2, NameGenerator.GenerateEntityName(3));
            deviceDetails.EnterExternalTerminalId(3, NameGenerator.GenerateEntityName(3));

            //clear activation
            deviceDetails.ClearActivationDate();
            deviceDetails.ClickSubmit();

            Assert.IsEmpty(deviceDetails.GetActivationDate());
            deviceDetails.GoBackToDeviceList();

            DeviceTableRecord deviceAfter = deviceList.GetDeviceByIndex(0);
            bool activeAfter = deviceAfter.IsActive;

            Assert.IsFalse(activeAfter);

        }


        [Test, Description("Verify device details page displays device system version")]
        public void DeviceDetails_Version()
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
            deviceList.ClickEditDevice(0);

            string version = deviceDetails.GetVersion();
            Assert.AreEqual(device.Version, version);
        }


        [Test]
        public void DeviceDetails_BursterId()
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
            deviceList.ClickEditDevice(0);

            int bursterId = device.DeviceId * 4;
            Assert.AreEqual(deviceDetails.GetBursterId(0), bursterId - 3);
            Assert.AreEqual(deviceDetails.GetBursterId(1), bursterId - 2);
            Assert.AreEqual(deviceDetails.GetBursterId(2), bursterId - 1);
            Assert.AreEqual(deviceDetails.GetBursterId(3), bursterId);
        }


        [Test]
        public void DeviceDetails_GameName_Loaded()
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

            int terminalIndex = 0;

            if (deviceDetails.GameIsLoaded(terminalIndex) == false)
            {
                Assert.Null(deviceDetails.GetGameName(terminalIndex));
            }
            else
            {
                string gameName = deviceDetails.GetGameName(terminalIndex);
                Assert.NotNull(gameName);
                Assert.IsTrue(gameName.Length > 0);
            }

        }





        [Test]
        public void DeviceDetails_Game_UPC_Loaded()
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

            string gameName = deviceDetails.GetGameUPC(0);

            //if game not loaded pass test
            if (deviceDetails.GameIsLoaded(0) == false)
            {

                Assert.Pass();
            }

            //if game is loaded check that the UPC is 12 characters
            Assert.IsTrue(gameName.Length == 12);
        }





        [Test]
        public void DeviceDetails_GameId_Loaded()
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

            if (deviceDetails.GameIsLoaded(1) == false)
            {
                Assert.Zero(deviceDetails.GetGameId(1));
            }
            else
            {
                int id = deviceDetails.GetGameId(1);

                Assert.NotZero(id);
            }

        }





        [Test]
        public void DeviceDetails_PackSize()
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

            int terminalIndex = 0;

            if (deviceDetails.GameIsLoaded(terminalIndex) == false)
            {
                Assert.Zero(deviceDetails.GetPackSize(terminalIndex));
            }
            else
            {
                int size = deviceDetails.GetPackSize(terminalIndex);

                Assert.Greater(size, 0);
            }

        }





        [Test]
        public void DeviceDetails_Price()
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

            int terminalIndex = 1;

            if (deviceDetails.GameIsLoaded(terminalIndex) == false)
            {
                Assert.Zero(deviceDetails.GetPrice(terminalIndex));
            }
            else
            {
                int price = deviceDetails.GetPrice(0);
                Assert.Greater(price, 0);
            }


        }





        [Test]
        public void DeviceDetails_Stock_Loaded()
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

            int terminalIndex = 0;

            if (deviceDetails.GameIsLoaded(terminalIndex) == false)
            {
                Assert.Zero(deviceDetails.GetStock(terminalIndex));
            }
            else
            {
                int stock = deviceDetails.GetStock(terminalIndex);

                Assert.Greater(stock, 0);
            }


        }








        [Test]
        public void DeviceDetails_Burster_Enabled()
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

            bool active = deviceDetails.BursterIsActive(0);

            if (active)
            {
                Assert.AreEqual("Yes", deviceDetails.GetBursterStatusText(0));
            }
            else
            {

                Assert.AreEqual("No", deviceDetails.GetBursterStatusText(0));
            }
        }


        [Test]
        public void DeviceDetails_PaymentTerminalId()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            DeviceTableRecord device = deviceList.GetDeviceByIndex(0);
            deviceList.ClickEditDevice(0);

            int id = device.DeviceId * 4;
            Assert.AreEqual(deviceDetails.GetPaymentTerminalId(0), id - 3);
            Assert.AreEqual(deviceDetails.GetPaymentTerminalId(1), id - 2);
            Assert.AreEqual(deviceDetails.GetPaymentTerminalId(2), id - 1);
            Assert.AreEqual(deviceDetails.GetPaymentTerminalId(3), id);
        }


        [Test]
        public void DeviceDetails_PaymentTerminal_Name()
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

            string name = deviceDetails.GetPaymentTerminalName(0);
            Assert.IsNotEmpty(name);

            name = deviceDetails.GetPaymentTerminalName(1);
            Assert.IsNotEmpty(name);

            name = deviceDetails.GetPaymentTerminalName(2);
            Assert.IsNotEmpty(name);

            name = deviceDetails.GetPaymentTerminalName(3);
            Assert.IsNotEmpty(name);

        }


        [Test]
        public void DeviceDetails_Edit_PaymentTerminal_Name()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to device list
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.AddDevice();
            deviceList.ClickEditDevice(0);

            string nameBefore = deviceDetails.GetPaymentTerminalName(0);

            string newName = NameGenerator.GenerateEntityName(5);
            deviceDetails.EnterPaymentTerminalName(0, newName);

            //enter external Ids in case they arent filled out already
            deviceDetails.EnterExternalTerminalId(0, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(1, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(2, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(3, NameGenerator.GenerateEntityName(5));

            deviceDetails.ClickSubmit();

            deviceDetails.GoBackToDeviceList();
            deviceList.ClickEditDevice(0);

            string nameAfter = deviceDetails.GetPaymentTerminalName(0);
            Assert.AreNotEqual(nameBefore, nameAfter);
            Assert.AreEqual(newName, nameAfter);

        }


        [Test]
        public void DeviceDetails_PaymentTerminal_ExternalId()
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

            string name = deviceDetails.GetExternalTerminalId(0);
            Assert.IsNotEmpty(name);

            name = deviceDetails.GetExternalTerminalId(1);
            Assert.IsNotEmpty(name);

            name = deviceDetails.GetExternalTerminalId(2);
            Assert.IsNotEmpty(name);

            name = deviceDetails.GetExternalTerminalId(3);
            Assert.IsNotEmpty(name);

        }


        [Test]
        public void DeviceDetails_Edit_PaymentTerminal_ExternalId()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //goto device list
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.AddDevice();
            deviceList.SortByColDesc(0);
            deviceList.ClickEditDevice(0);

            string nameBefore = deviceDetails.GetExternalTerminalId(0);

            string newName = NameGenerator.GenerateEntityName(5);

            deviceDetails.EnterExternalTerminalId(0, newName);
            deviceDetails.EnterExternalTerminalId(1, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(2, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(3, NameGenerator.GenerateEntityName(5));
            deviceDetails.ClickSubmit();

            deviceDetails.GoBackToDeviceList();
            deviceList.SortByColDesc(0);
            deviceList.ClickEditDevice(0);

            string nameAfter = deviceDetails.GetExternalTerminalId(0);

            Assert.AreNotEqual(nameBefore, nameAfter);
            Assert.AreEqual(newName, nameAfter);

        }


        [Test, Description("Verify error is displayed when payment terminal name is empty")]
        public void DeviceDetails_PaymentTerminalName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.ClickEditDevice(0);

            deviceDetails.EnterPaymentTerminalName(0, "");
            deviceDetails.ClickSubmit();

            Assert.IsTrue(deviceDetails.TerminalNameErrorIsDisplayed(0));

            deviceDetails.EnterPaymentTerminalName(1, "");
            deviceDetails.ClickSubmit();

            Assert.IsTrue(deviceDetails.TerminalNameErrorIsDisplayed(1));

            deviceDetails.EnterPaymentTerminalName(2, "");
            deviceDetails.ClickSubmit();

            Assert.IsTrue(deviceDetails.TerminalNameErrorIsDisplayed(2));

            deviceDetails.EnterPaymentTerminalName(3, "");
            deviceDetails.ClickSubmit();

            Assert.IsTrue(deviceDetails.TerminalNameErrorIsDisplayed(3));

        }


        [Test, Description("Verify error is displayed when payment terminal ID is empty")]
        public void DeviceDetails_PaymentTerminalId_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.ClickEditDevice(0);

            deviceDetails.EnterExternalTerminalId(0, "");
            deviceDetails.ClickSubmit();

            Assert.IsTrue(deviceDetails.ExternalIdErrorIsDisplayed(0));

            deviceDetails.EnterExternalTerminalId(1, "");
            deviceDetails.ClickSubmit();

            Assert.IsTrue(deviceDetails.ExternalIdErrorIsDisplayed(1));

            deviceDetails.EnterExternalTerminalId(2, "");
            deviceDetails.ClickSubmit();

            Assert.IsTrue(deviceDetails.ExternalIdErrorIsDisplayed(2));

            deviceDetails.EnterExternalTerminalId(3, "");
            deviceDetails.ClickSubmit();

            Assert.IsTrue(deviceDetails.ExternalIdErrorIsDisplayed(3));

        }


        [Test, Description("Verify error is displayed when payment terminal ID is in use by another device")]
        public void DeviceDetails_PaymentTerminalId_Taken()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();


            deviceList.AddDevice();
            deviceList.AddDevice();

            deviceList.ClickEditDevice(0);

            string existingId = deviceDetails.GetExternalTerminalId(0);

            deviceDetails.GoBackToDeviceList();
            deviceList.ClickEditDevice(1);

            deviceDetails.EnterExternalTerminalId(0, existingId);
            deviceDetails.EnterExternalTerminalId(1, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(2, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(3, NameGenerator.GenerateEntityName(5));
            deviceDetails.ClickSubmit();

            Assert.IsTrue(deviceDetails.ExternalIdErrorIsDisplayed(0));

        }


        [Test, Description("Verify external terminal id cannot be more than 16 characters")]
        public void DeviceDetails_PaymentTerminalId_Length()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EntityTabs.ClickDeviceTab();

            deviceList.ClickEditDevice(0);

            deviceDetails.EnterExternalTerminalId(0, NameGenerator.GenerateEntityName(EXTERNAL_TERMINALID_MAX_LENGTH + 1));
            deviceDetails.EnterExternalTerminalId(1, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(2, NameGenerator.GenerateEntityName(5));
            deviceDetails.EnterExternalTerminalId(3, NameGenerator.GenerateEntityName(5));
            deviceDetails.ClickSubmit();

            Assert.IsTrue(deviceDetails.GetExternalTerminalId(0).Length <= EXTERNAL_TERMINALID_MAX_LENGTH);

        }

    }
}