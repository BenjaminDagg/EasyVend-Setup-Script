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
    internal class LotteryDetailsTest
    {

        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private LotteryDetailsPage LotteryDetails;

        [SetUp]
        public void Setup()
        {

            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            LotteryDetails = new LotteryDetailsPage(DriverFactory.Driver);
        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            LotteryDetails = null;
            DriverFactory.CloseDriver();
        }


        [Test, Description("Verifies the Lottery Details page has correct url")]
        public void LotteryDetails_GoToPage()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            string expectedUrl = baseUrl + LotteryDetailsPage.url;
            Assert.AreEqual(DriverFactory.GetUrl(), expectedUrl);
        }

        [Test, Description("Verifies the country input is read only")]
        public void Country_Readonly()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);

            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //verify country is read only
            Assert.IsTrue(LotteryDetails.isReadOnly(EntityDetailsInputs.COUNTRY));

        }


        [Test, Description("Verify the country field is set to the country the market is deployed in")]
        public void Market_Correct_Country()
        {
            string marketCountry = ConfigurationManager.AppSettings["Market_Country"];

            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);

            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            Assert.AreEqual(marketCountry, LotteryDetails.getCountry());
        }


        [Test, Description("Verifies the state select is read only")]
        public void State_Readonly()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);

            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            Assert.IsTrue(LotteryDetails.isReadOnly(EntityDetailsInputs.STATE));

        }

        [Test, Description("Verifies the submit button is not hidden for vendor admin user and page is not read only")]
        public void LotteryDetails_VendorAdmin_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            Assert.IsFalse(LotteryDetails.submitIsHidden());
            Assert.IsFalse(LotteryDetails.isReadOnly(EntityDetailsInputs.ENTITY_NAME));
        }



        [Test, Description("Verify lottery menu option is hidden for vendor report users")]
        public void LotteryDetails_VendorReport_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_REPORT.Username, AppUsers.VENDOR_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            Assert.IsFalse(navMenu.lotteryIsVisible());
        }


        [Test, Description("Verifies the submit button is  hidden for lottery admin user and page is  read only")]
        public void LotteryDetails_LotteryAdmin_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.LOTTERY_ADMIN.Username, AppUsers.LOTTERY_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //verify submit button is hidden
            Assert.IsTrue(LotteryDetails.submitIsHidden());
            Assert.IsTrue(LotteryDetails.isReadOnly(EntityDetailsInputs.ENTITY_NAME));
        }


        [Test, Description("Verifies the submit button is  hidden for lottery admin user and page is  read only")]
        public void LotteryDetails_LotteryReport_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.LOTTERY_REPORT.Username, AppUsers.LOTTERY_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //verify submit button is hidden
            Assert.IsTrue(LotteryDetails.submitIsHidden());
            Assert.IsTrue(LotteryDetails.isReadOnly(EntityDetailsInputs.ENTITY_NAME));
        }


        [Test, Description("Verify lottery menu option is hidden for vendor report users")]
        public void LotteryDetails_Site_User()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_ADMIN.Username, AppUsers.SITE_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            Assert.IsFalse(navMenu.lotteryIsVisible());
        }


        [Test, Description("Verify success banner is displayed when valid inputs are entered")]
        public void Edit_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();

            LotteryDetails.EnterForm(
                "Texas Lottery Commission",
                "Missouri Lottery",
                "DGE",
                "test@email.com",
                "9876543210",
                "1823 Southridge Drive",
                "Jefferson City",
                "54321"
            );
            LotteryDetails.ClickSubmit();

            Assert.IsTrue(LotteryDetails.successBanerIsVisible());

        }


        [Test, Description("Verifies the correct state is saved for the current market")]
        public void Market_Correct_State()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);
            navMenu.ClickLottery();

            string marketState = ConfigurationManager.AppSettings["Market_State"];

            Assert.AreEqual(marketState, LotteryDetails.getState());
        }


        [Test, Description("Verify error is displayed when Entity name field is empty")]
        public void Entity_Name_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //fill out form, leave entity name blank
            LotteryDetails.EnterForm(
                "",
                "Missouri Lottery",
                "DGE",
                "test@email.com",
                "9876543210",
                "1823 Southridge Drive",
                "Jefferson City",
                "54321"
            );
            LotteryDetails.ClickSubmit();

            //cehck if entity name error is displayed and the success banner is not displayed
            Assert.IsTrue(LotteryDetails.nameErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
        }


        [Test, Description("Verify error is displayed when Entity name field is in use by another entity")]
        public void Entity_Name_Taken()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //get vendor entity name
            navMenu.ClickVendor();
            VendorDetailsPage vendorDetails = new VendorDetailsPage(DriverFactory.Driver);
            string name = vendorDetails.getEntityName();

            navMenu.ClickLottery();

            //fill out form, leave entity name blank
            LotteryDetails.EnterForm(
                name,
                "Missouri Lottery",
                "DGE",
                "test@email.com",
                "9876543210",
                "1823 Southridge Drive",
                "Jefferson City",
                "54321"
            );
            LotteryDetails.ClickSubmit();

            //cehck if entity name error is displayed and the success banner is not displayed
            Assert.IsTrue(LotteryDetails.nameErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
        }


        [Test, Description("Verify error is displayed when first name is blank")]
        public void First_Name_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //fill out form, leave first name blank
            LotteryDetails.EnterForm(
                "Texas Lottery Commission",
                "",
                "DGE",
                "test@email.com",
                "9876543210",
                "1823 Southridge Drive",
                "Jefferson City",
                "54321"
            );
            LotteryDetails.ClickSubmit();

            //cehck if entity name error is displayed and the success banner is not displayed
            Assert.IsTrue(LotteryDetails.firstNameErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
        }


        [Test, Description("Verify error is displayed when last name is blank")]
        public void Last_Name_Blank()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //fill out form, leave entity name blank
            LotteryDetails.EnterForm(
                "Texas Lottery Commission",
                "Missouri Lottery",
                "",
                "test@email.com",
                "9876543210",
                "1823 Southridge Drive",
                "Jefferson City",
                "54321"
            );
            LotteryDetails.ClickSubmit();

            //cehck if entity name error is displayed and the success banner is not displayed
            Assert.IsTrue(LotteryDetails.lastNameErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
        }


        [Test, Description("Verify an error is displayed when the email field is blank")]
        public void Email_Blank()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //fill out form, leave entity name blank
            LotteryDetails.EnterForm(
                "Texas Lottery Commission",
                "Missouri Lottery",
                "DGE",
                "",
                "9876543210",
                "1823 Southridge Drive",
                "Jefferson City",
                "54321"
            );
            LotteryDetails.ClickSubmit();

            //cehck if entity name error is displayed and the success banner is not displayed
            Assert.IsTrue(LotteryDetails.emailErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
        }

        [Test, Description("Verify an error is displayed when an invalid email format is given")]
        public void Invalid_Email_Format()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //fill out form, enter incorrect email (missing @)
            LotteryDetails.EnterForm("Texas Lottery Commission", "Missouri Lottery", "DGE",
                "testemail.com", "9876543210", "1823 Southridge Drive", "Jefferson City", "54321");
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.emailErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //incorrect email (missing ".")
            LotteryDetails.EnterEmail("test@emailcom");
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.emailErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //incorrect email (missing .com)
            LotteryDetails.EnterEmail("test@email.");
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.emailErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());

        }


        [Test, Description("Verify an error is displayed when the phone field is left blank")]
        public void Phone_Blank()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //fill out form, enter empty phone
            LotteryDetails.EnterForm(
                "Texas Lottery Commission",
                "Missouri Lottery",
                "DGE",
                "test@email.com",
                "",
                "1823 Southridge Drive",
                "Jefferson City",
                "54321"
            );
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
        }


        [Test, Description("Verify an error is displayed when the phone is in an incorrect format")]
        public void VerifyPhoneFormatError()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //more than 10 numbers
            LotteryDetails.EnterForm("Texas Lottery Commission", "Missouri Lottery", "DGE",
                "test@email.com", "12345678901", "1823 Southridge Drive", "Jefferson City", "54321");
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());

            //less than 10 numbers
            LotteryDetails.EnterPhone("123456789");
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());

            //letters
            LotteryDetails.EnterPhone("123456789a");
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());

            //special characters
            LotteryDetails.EnterPhone("123456789!");
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.phoneErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
        }


        [Test, Description("Verify error is displayed when the address field is blank")]
        public void Address_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //fill out form, enter incorrect email (missing @)
            LotteryDetails.EnterForm(
                "Texas Lottery Commission",
                "Missouri Lottery",
                "DGE",
                "test@email.com",
                "1112223333",
                "",
                "Jefferson City",
                "54321"
            );
            LotteryDetails.ClickSubmit();

            Assert.IsTrue(LotteryDetails.addressErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
        }


        [Test, Description("Verify error is displayed when the city field is blank")]
        public void City_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //fill out form, enter incorrect email (missing @)
            LotteryDetails.EnterForm(
                "Texas Lottery Commission",
                "Missouri Lottery",
                "DGE",
                "test@email.com",
                "1112223333",
                "1823 Southridge Drive",
                "",
                "54321"
            );
            LotteryDetails.ClickSubmit();

            Assert.IsTrue(LotteryDetails.cityErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
        }


        [Test, Description("Verify error is displayed when the zipcode field is blank")]
        public void Zipcode_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //fill out form, enter incorrect email (missing @)
            LotteryDetails.EnterForm(
                "Texas Lottery Commission",
                "Missouri Lottery",
                "DGE",
                "test@email.com",
                "1112223333",
                "1823 Southridge Drive",
                "Jefferson City",
                ""
            );
            LotteryDetails.ClickSubmit();

            Assert.IsTrue(LotteryDetails.zipErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
        }


        [Test, Description("Verify error is displayed when the zipcode has incorrect format")]
        public void Invalid_Zipcode_Format()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            navMenu.ClickLottery();

            //zip code wrong format: 4 numbers
            LotteryDetails.EnterForm("Texas Lottery Commission", "Missouri Lottery", "DGE",
                "test@email.com", "1112223333", "1823 Southridge Drive", "Jefferson City", "1234");
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.zipErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //6 digit zipode
            LotteryDetails.EnterZipCode("123456");
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.zipErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //XXXX-XXXXX
            LotteryDetails.EnterZipCode("1234-56789");
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.zipErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //XXX-XXX-XXX
            LotteryDetails.EnterZipCode("123-456-789");
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.zipErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //letters
            LotteryDetails.EnterZipCode("123ab");
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.zipErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
            DriverFactory.Refresh();

            //special characters
            LotteryDetails.EnterZipCode("123!@");
            LotteryDetails.ClickSubmit();
            Assert.IsTrue(LotteryDetails.zipErrorIsDisplayed());
            Assert.IsFalse(LotteryDetails.successBanerIsVisible());
        }

    }
}