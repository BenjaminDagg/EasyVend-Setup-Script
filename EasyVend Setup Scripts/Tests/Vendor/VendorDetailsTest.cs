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
    public class VendorDetailsTest
    {

        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private VendorDetailsPage vendorDetailsPage;

        [SetUp]
        public void Setup()
        {

            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            vendorDetailsPage = new VendorDetailsPage(DriverFactory.Driver);
        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            vendorDetailsPage = null;
            DriverFactory.CloseDriver();
        }


        [Test, Description("Go to vendor details page")]
        public void GoTo_Vendor_Details()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickVendor();
            Assert.AreEqual(DriverFactory.GetUrl(), VendorDetailsPage.url);
        }


        //Verifies all inputs are read only
        [Test, Description("Verifies all inputs are read only")]
        public void VendorDetails_Readonly()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickVendor();

            Assert.IsTrue(vendorDetailsPage.isReadOnly(EntityDetailsInputs.ENTITY_NAME));
            Assert.IsTrue(vendorDetailsPage.isReadOnly(EntityDetailsInputs.FNAME));
            Assert.IsTrue(vendorDetailsPage.isReadOnly(EntityDetailsInputs.LNAME));
            Assert.IsTrue(vendorDetailsPage.isReadOnly(EntityDetailsInputs.EMAIL));
            Assert.IsTrue(vendorDetailsPage.isReadOnly(EntityDetailsInputs.PHONE));
            Assert.IsTrue(vendorDetailsPage.isReadOnly(EntityDetailsInputs.ADDRESS1));
            Assert.IsTrue(vendorDetailsPage.isReadOnly(EntityDetailsInputs.CITY));
            Assert.IsTrue(vendorDetailsPage.isReadOnly(EntityDetailsInputs.ZIP));
            Assert.IsTrue(vendorDetailsPage.isReadOnly(EntityDetailsInputs.COUNTRY));
            Assert.IsTrue(vendorDetailsPage.isReadOnly(EntityDetailsInputs.STATE));

        }


        [Test, Description("Verifies the correct state is saved for the current market")]
        public void Market_Correct_State()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickVendor();

            string state = ConfigurationManager.AppSettings["Vendor_State"];

            Assert.AreEqual(state, vendorDetailsPage.getState());
        }


        [Test, Description("Verifies the correct vendor record name is used")]
        public void Vendot_Entity_Name()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickVendor();

            string entityName = ConfigurationManager.AppSettings["Vendor_Record_Name"];

            Assert.AreEqual(entityName, vendorDetailsPage.getEntityName());
        }


        [Test, Description("Verify the country field is set to the country the market is deployed in")]
        public void Market_Correct_Country()
        {
            string marketCountry = ConfigurationManager.AppSettings["Market_Country"];

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);

            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickVendor();

            Assert.AreEqual(marketCountry, vendorDetailsPage.getCountry());
        }


        [Test, Description("Verify submit button is hidden")]
        public void Submit_Hidden()
        {
            string marketCountry = ConfigurationManager.AppSettings["Market_Country"];

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);

            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickVendor();

            Assert.IsTrue(vendorDetailsPage.submitIsHidden());
        }


    }
}