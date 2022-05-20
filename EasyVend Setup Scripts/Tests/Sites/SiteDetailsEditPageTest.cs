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
    public class SiteDetailsEditPageTest
    {

        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private SiteListPage siteList;
        private SiteDetailsEditPage siteDetails;

        private const int ENTITY_NAME_MIN_LENGTH = 3;
        private const int ENTITY_NAME_MAX_LENGTH = 256;
        private const int EXTERNAL_STOREID_MAX_LENGTH = 64;


        [SetUp]
        public void Setup()
        {

            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            siteList = new SiteListPage(DriverFactory.Driver);
            siteDetails = new SiteDetailsEditPage(DriverFactory.Driver);

        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            siteList = null;
            siteDetails = null;
            DriverFactory.CloseDriver();
        }


        [Test, Description("Verify site details can be accessed by clicking name of site in site list")]
        public void Click_SiteName()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            SiteTableRecord site = siteList.GetSiteByIndex(0);
            string expectedUrl = SiteDetailsEditPage.url + site.Id.ToString();
            siteList.ClickSiteByIndex(0);

            Assert.AreEqual(expectedUrl, DriverFactory.GetUrl());
        }


        [Test, Description("Verify site details can be accessed by clicking edit  button for site in site list")]
        public void Click_Edit_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            SiteTableRecord site = siteList.GetSiteByIndex(0);
            string expectedUrl = SiteDetailsEditPage.url + site.Id.ToString() + "#details";
            siteList.ClickEditSiteButton(0);

            Assert.AreEqual(expectedUrl, DriverFactory.GetUrl());
        }


        [Test, Description("Verify vendor admin user can edit a site")]
        public void EditSite_VendorAdmin()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickSiteByIndex(0);

            Assert.IsFalse(siteDetails.submitIsHidden());

            siteDetails.EnterEntityName(NameGenerator.GenerateEntityName(5));
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.successBanerIsVisible());

        }


        [Test, Description("Verify lottery admin user can edit a site")]
        public void EditSite_LotteryAdmin()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.LOTTERY_ADMIN.Username, AppUsers.LOTTERY_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickSiteByIndex(0);

            Assert.IsFalse(siteDetails.submitIsHidden());

            siteDetails.EnterEntityName(NameGenerator.GenerateEntityName(5));
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.successBanerIsVisible());

        }


        [Test, Description("Verify site admin user cannot edit a site")]
        public void EditSite_SiteAdmin()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_ADMIN.Username, AppUsers.SITE_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickSiteByIndex(0);

            Assert.IsTrue(siteDetails.submitIsHidden());

        }


        [Test, Description("Verify site report user has read-only access to site details page")]
        public void EditSite_SiteReport()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickSiteByIndex(0);

            Assert.IsTrue(siteDetails.submitIsHidden());

        }


        [Test, Description("Verify correct site info is displayed when clicking edit site button")]
        public void Site_Data()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            int count = siteList.getRecordCount();
            Assert.NotZero(count);


            SiteTableRecord site = siteList.GetSiteByIndex(0);
            site.display();
            siteList.ClickEditSiteButton(0);

            Assert.IsTrue(siteDetails.SiteMatches(site));
        }


        [Test, Description("Verify success banner is displayed when valid site data is entered")]
        public void Edit_Site_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);
            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                "21",
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateUsername(5),
                "1111111111",
                "123 test st",
                "test city",
                "22222"
            );
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.successBanerIsVisible());

        }


        [Test, Description("Verify site record is edited successfully")]
        public void Edit_Site()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            SiteTableRecord siteBefore = siteList.GetSiteByIndex(0);

            siteList.ClickEditSiteButton(0);

            string newName = NameGenerator.GenerateEntityName(5);
            siteDetails.EnterForm(
                newName,
                NameGenerator.GenerateEntityName(3),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateUsername(5),
                "1111111111",
                "123 test st",
                "test city",
                "22222"
            );
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.successBanerIsVisible());

            SiteTableRecord newSite = siteDetails.GetSiteRecord();

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.EnterSearchTerm(newName);
            int newIndex = siteList.indexOfSite(newName);
            SiteTableRecord site = siteList.GetSiteByIndex(newIndex);

            Assert.IsTrue(SiteTableRecord.AreEqual(site, newSite));
            Assert.IsFalse(SiteTableRecord.AreEqual(siteBefore, site));
        }



        //Verify error is displayed when the site name field is missig
        [Test, Description("Verify error is displayed when site name is empty")]
        public void SiteName_Empty()
        {

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EnterEntityName("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.nameErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //Verify error is displayed when an existing site name is entered
        [Test, Description("Verify error is displayed when site name is in use by another site")]
        public void SiteName_Taken()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            SiteTableRecord site = siteList.GetSiteByIndex(1);
            siteList.ClickEditSiteButton(0);
            //enter existing site name
            siteDetails.EnterEntityName(site.SiteName);
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.nameErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //Verify error is displayed when site name is incorrect length
        [Test, Description("Verify error is displayed when site name is incorrect length")]
        public void SiteNameLengthError()
        {
            string siteName = NameGenerator.GenerateEntityName(ENTITY_NAME_MAX_LENGTH);
            Console.WriteLine(siteName.Length);
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            //site name less than 3
            siteName = NameGenerator.GenerateEntityName(ENTITY_NAME_MIN_LENGTH - 1);
            siteDetails.EnterEntityName(siteName);
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.nameErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());

            //site name more than 256
            siteName = NameGenerator.GenerateEntityName(ENTITY_NAME_MAX_LENGTH + 1);
            siteDetails.EnterEntityName(siteName);
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.getEntityName().Length <= ENTITY_NAME_MAX_LENGTH);
            Assert.IsTrue(siteDetails.successBanerIsVisible());
        }


        [Test, Description("Verify error is displayed when external store Id field is empty")]
        public void ExternalStoreId_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            //empty external store id
            siteDetails.EnterExternalStoreId("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.ExternalStoreIdErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        [Test, Description("Verify error is displayed when external store Id is used by another site")]
        public void ExternalStoreId_Taken()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            string externalStoreId = siteDetails.GetExernalStoreId();
            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(1);

            siteDetails.EnterExternalStoreId(externalStoreId);
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.ExternalStoreIdErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        [Test, Description("Verify error is displayed when external store Id is over 50 characters")]
        public void ExternalStoreId_Max_Length()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            //empty external store id
            siteDetails.EnterExternalStoreId(NameGenerator.GenerateEntityName(EXTERNAL_STOREID_MAX_LENGTH + 1));
            siteDetails.ClickSubmit();

            Assert.LessOrEqual(siteDetails.GetExernalStoreId().Length, EXTERNAL_STOREID_MAX_LENGTH);

        }


        //verify error is displayed when first name field is blank
        [Test, Description("Verify error is displayed when first name field is empty")]
        public void FirstName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            //first name blank
            siteDetails.EnterFirstName("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.firstNameErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when last name field is blank
        [Test, Description("Verify error is displayed when last name is empty")]
        public void LastName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            //first name blank
            siteDetails.EnterLastName("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.lastNameErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when email field is blank
        [Test, Description("verify error is displayed when email field is blank")]
        public void Email_Blank()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            //first name blank
            siteDetails.EnterEmail("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.emailErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when email is in invalid format
        [Test, Description("verify error is displayed when email is in invalid format")]
        public void Email_Invalid_Format()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            //email missing @ and .
            siteDetails.EnterEmail("email");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.emailErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //email missing  .
            siteDetails.EnterEmail("email@");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.emailErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //email missing  .
            siteDetails.EnterEmail("email@test");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.emailErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //email missing  .com
            siteDetails.EnterEmail("email@test.");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.emailErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();
        }


        //verify error is displayed when phone field is blank
        [Test, Description("verify error is displayed when phone field is blank")]
        public void Phone_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            //first name blank
            siteDetails.EnterPhone("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        [Test, Description("verify error is displayed when phone is in invalid format")]
        public void Phone_Invalid_Format()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EnterPhone("12345678901");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //less than 10 numbers
            siteDetails.EnterPhone("123456789");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //letters
            siteDetails.EnterPhone("123456789a");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //special characters
            siteDetails.EnterPhone("123456789!");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when address field is blank
        [Test, Description("verify error is displayed when address field is blank")]
        public void Address_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            //first name blank
            siteDetails.EnterAddress("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.addressErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when city field is blank
        [Test, Description("verify error is displayed when city field is blank")]
        public void City_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            //first name blank
            siteDetails.EnterCity("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.cityErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when zipcode field is blank
        [Test, Description("verify error is displayed when zipcode field is blank")]
        public void Zipcode_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            //first name blank
            siteDetails.EnterZipCode("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when zip code is in invalid format
        [Test, Description("verify error is displayed when zip code is in invalid format")]
        public void Zipcode_Invalid_Format()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            siteDetails.EnterZipCode("1234");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //6 digit zipode
            siteDetails.EnterZipCode("123456");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //XXXX-XXXXX
            siteDetails.EnterZipCode("1234-56789");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //XXX-XXX-XXX
            siteDetails.EnterZipCode("123-456-789");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //letters
            siteDetails.EnterZipCode("123ab");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //special characters
            siteDetails.EnterZipCode("123!@");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify the country field is read-only
        [Test, Description("verify the country field is read-only")]
        public void Country_Readonly()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            Assert.IsTrue(siteDetails.isReadOnly(EntityDetailsInputs.COUNTRY));
        }


        //verify state select is read only
        [Test, Description("verify state select is read only")]
        public void State_Readonly()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickEditSiteButton(0);

            Assert.IsTrue(siteDetails.isReadOnly(EntityDetailsInputs.STATE));
        }


        //verify site ID  is read only
        [Test, Description("verify site ID  is read only")]
        public void SiteId_Readonly()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            SiteTableRecord site = siteList.GetSiteByIndex(0);
            siteList.ClickEditSiteButton(0);

            Assert.IsTrue(siteDetails.isReadOnly(EntityDetailsInputs.ID));
            Assert.AreEqual(site.Id, siteDetails.GetSiteId());
        }

    }
}