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
    public class SiteDetailsAddPageTest
    {

        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private SiteListPage siteList;
        private SiteDetailsAddPage siteDetails;

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
            siteDetails = new SiteDetailsAddPage(DriverFactory.Driver);

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


        [Test, Description("Verify vendor admin user can add a site")]
        public void AddSite_Vendor_Admin()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            Assert.IsTrue(siteList.AddSiteIsVisible());

            siteList.ClickAddSite();
            Assert.AreEqual(SiteDetailsAddPage.url, DriverFactory.GetUrl());
        }


        [Test, Description("Verify lottery admin user can add a site")]
        public void AddSite_Lottery_Admin()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.LOTTERY_ADMIN.Username, AppUsers.LOTTERY_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            Assert.IsTrue(siteList.AddSiteIsVisible());

            siteList.ClickAddSite();
            Assert.AreEqual(SiteDetailsAddPage.url, DriverFactory.GetUrl());
        }


        [Test, Description("Verify site admin user cannot add a site")]
        public void AddSite_Site_Admin()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_ADMIN.Username, AppUsers.SITE_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            Assert.IsFalse(siteList.AddSiteIsVisible());
        }


        [Test, Description("Verify site report user cannot add a site")]
        public void AddSite_Site_Report()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            Assert.IsFalse(siteList.AddSiteIsVisible());
        }


        [Test, Description("Verify a new site is created and added to the site list")]
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
            siteDetails.EnterForm(
                newSiteName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.successBanerIsVisible());
            SiteTableRecord newSite = siteDetails.GetSiteRecord();

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


        [Test, Description("Verify success banner is displayed when valid data is entered for all fields")]
        public void SuccessfulForm()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.successBanerIsVisible());

        }


        [Test, Description("Verify new site is added to list")]
        public void SiteList_Add_NewSite()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();

            string newName = NameGenerator.GenerateEntityName(5);
            siteDetails.EnterForm(
                newName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.successBanerIsVisible());


            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.EnterSearchTerm(newName);

            int index = siteList.indexOfSite(newName);

            Assert.AreNotEqual(index, -1);
        }



        //Verify error is displayed when the site name field is missig
        [Test, Description("Verify error is displayed when the site name field is missig")]
        public void AddSite_Name_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();

            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");
            siteDetails.EnterEntityName("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.nameErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //Verify error is displayed when an existing site name is entered
        [Test, Description("Verify an error is displayed when a site name is in use by an existing site")]
        public void AddSite_SiteName_Taken()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            SiteTableRecord site = siteList.GetSiteByIndex(0);

            siteList.ClickAddSite();
            //enter existing site name
            siteDetails.EnterForm(
                site.SiteName,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.nameErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //Verify error is displayed when site name is incorrect length
        [Test, Description("Verify an error is displayed when site name is less than 3 characers or more than 256")]
        public void AddSite_SiteName_Length()
        {
            string siteName;

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            siteDetails.EnterForm("",
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            //site name less than 3
            siteName = NameGenerator.GenerateEntityName(ENTITY_NAME_MIN_LENGTH - 1);
            siteDetails.EnterEntityName(siteName);
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.nameErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //site name more than 256
            siteName = NameGenerator.GenerateEntityName(ENTITY_NAME_MAX_LENGTH + 1);
            siteDetails.EnterEntityName(siteName);
            siteDetails.EnterFirstName(""); //leave a field blank so it doesnt submit the form
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.getEntityName().Length <= ENTITY_NAME_MAX_LENGTH);
        }


        [Test, Description("Verify error is displayed when ExternalStoreId is being used by another site")]
        public void AddSite_ExternalStoreId_Taken()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();

            siteList.ClickEditSiteButton(0);

            SiteDetailsEditPage editSite = new SiteDetailsEditPage(DriverFactory.Driver);
            string externalStoreId = editSite.GetExernalStoreId();
            Console.WriteLine(externalStoreId);
            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "",
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.EnterExternalStoreId(externalStoreId);
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.ExternalStoreIdErrorIsShown());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        [Test, Description("Verify error is displayed when external store Id field is empty")]
        public void AddSite_ExternalStoreId_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            //leave external id blank
            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "",
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.EnterExternalStoreId("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.ExternalStoreIdErrorIsShown());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        [Test, Description("Verify error is displayed when external store id is over character limit")]
        public void AddSite_ExternalStoreId_Max_Length()
        {

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "",
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.EnterExternalStoreId(NameGenerator.GenerateEntityName(EXTERNAL_STOREID_MAX_LENGTH + 1));
            siteDetails.EnterEntityName("");    //Leave a field blank so the form doesn't submit
            siteDetails.ClickSubmit();

            Assert.LessOrEqual(siteDetails.getExternalStoreId().Length, EXTERNAL_STOREID_MAX_LENGTH);
        }


        //verify error is displayed when first name field is blank
        [Test, Description("verify error is displayed when first name field is blank")]
        public void AddSite_FirstName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            //first name blank
            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.EnterFirstName("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.firstNameErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when last name field is blank
        [Test, Description("verify error is displayed when last name field is blank")]
        public void AddSite_LastName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            //last name blank
            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.EnterLastName("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.lastNameErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when email field is blank
        [Test, Description("verify error is displayed when email field is blank")]
        public void AddSite_Email_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            //last name blank
            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.EnterEmail("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.emailErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when email is in invalid format
        [Test, Description("verify error is displayed when email is in invalid format")]
        public void AddSite_Invalid_Email()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            //email missing @ and .
            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.EnterEmail("email");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.emailErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //email missing  .
            siteDetails.EnterForm(NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.EnterEmail("email@");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.emailErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //email missing  .
            siteDetails.EnterForm(NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.EnterEmail("email@test");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.emailErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //email missing  .com
            siteDetails.EnterForm(NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.EnterEmail("email@test.");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.emailErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());

        }


        //verify error is displayed when phone field is blank
        [Test, Description("verify error is displayed when phone field is blank")]
        public void AddSite_Phone_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            //last name blank
            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.EnterPhone("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        [Test, Description("Verify error is displayed when invalid phone format is entered")]
        public void AddSite_Invalid_Phone()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            siteDetails.EnterForm(NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");

            siteDetails.EnterPhone("12345678901");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //less than 10 numbers
            siteDetails.EnterForm(NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.EnterPhone("123456789");

            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //letters
            siteDetails.EnterForm(NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.EnterPhone("123456789a");

            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //special characters
            siteDetails.EnterForm(NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.EnterPhone("123456789!");

            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when address field is blank
        [Test, Description("verify error is displayed when address field is blank")]
        public void AddSite_Address_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            //last name blank
            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.EnterAddress("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.addressErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when city field is blank
        [Test, Description("verify error is displayed when city field is blank")]
        public void AddSite_City_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            //last name blank
            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.EnterCity("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.cityErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when zipcode field is blank
        [Test, Description("verify error is displayed when zipcode field is blank")]
        public void AddSite_Zipcode_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            //last name blank
            siteDetails.EnterForm(
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.EnterZipCode("");
            siteDetails.ClickSubmit();

            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify error is displayed when zip code is in invalid format
        [Test, Description("verify error is displayed when zip code is in invalid format")]
        public void AddSite_Invalid_Zipcode()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();
            siteDetails.EnterForm(NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");

            siteDetails.EnterZipCode("1234");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //6 digit zipode
            siteDetails.EnterForm(NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.EnterZipCode("123456");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //XXXX-XXXXX
            siteDetails.EnterForm(NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.EnterZipCode("1234-56789");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //XXX-XXX-XXX
            siteDetails.EnterForm(NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.EnterZipCode("123-456-789");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //letters
            siteDetails.EnterForm(NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.EnterZipCode("123ab");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //special characters
            siteDetails.EnterForm(NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), NameGenerator.GenerateEntityName(5), "FName", "Lname",
            NameGenerator.GenerateUsername(5), "6612200748", "123 test st", "City", "12345");
            siteDetails.EnterZipCode("123!@");
            siteDetails.ClickSubmit();
            Assert.IsTrue(siteDetails.zipErrorIsDisplayed());
            Assert.IsFalse(siteDetails.successBanerIsVisible());
        }


        //verify the country field is read-only
        [Test, Description("verify the country field is read-only")]
        public void AddSite_Country_Readonly()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);

            siteList.ClickAddSite();

            Assert.IsTrue(siteDetails.isReadOnly(EntityDetailsInputs.COUNTRY));
        }


        //verify state select is read only
        [Test, Description("verify the state field is read-only")]
        public void AddSite_State_Readonly()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickAddSite();

            Assert.IsTrue(siteDetails.isReadOnly(EntityDetailsInputs.STATE));
        }


        [Test, Description("Verify default selected state is set to the market")]
        public void AddSite_State_Default()
        {
            string expectedState = ConfigurationManager.AppSettings["Site_Default_State"];

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickAddSite();

            string actualState = siteDetails.getState();

            Assert.AreEqual(expectedState, actualState);
        }


        [Test, Description("Verify default country is set to the market")]
        public void AddSite_Country_Default()
        {
            string expectedCountry = ConfigurationManager.AppSettings["Site_Default_Country"];

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickAddSite();

            string actualCountry = siteDetails.getCountry();

            Assert.AreEqual(expectedCountry, actualCountry);
        }


        [Test, Description("Verify new site is not added if the cancel button is selected")]
        public void Cancel_AddSite()
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
                "FName",
                "Lname",
                NameGenerator.GenerateUsername(5),
                "6612200748",
                "123 test st",
                "City",
                "12345");

            siteDetails.ClickCancel();
            siteDetails.ConfirmationModal.ClickSubmit();

            Assert.AreEqual(baseUrl + "Site", DriverFactory.GetUrl());

            siteList.SetTableLength(100);
            siteList.EnterSearchTerm(siteName);
            Assert.AreEqual(-1, siteList.indexOfSite(siteName));
        }


        [Test, Description("Verify user remains on the add site page when clicking no on cancel modal")]
        public void CancelModal_Select_No()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickSites();
            siteList.SetTableLength(100);
            siteList.ClickAddSite();

            Assert.AreEqual(SiteDetailsAddPage.url, DriverFactory.GetUrl());

            siteDetails.ClickCancel();
            siteDetails.ConfirmationModal.ClickCancel();

            Assert.AreEqual(SiteDetailsAddPage.url, DriverFactory.GetUrl());
        }

    }
}