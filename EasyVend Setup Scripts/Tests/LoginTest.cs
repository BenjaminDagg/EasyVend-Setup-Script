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
    public class LoginTest
    {
        string baseUrl;

        //users
        private string DEFAULT_USERNAME;
        private string DEFAULT_PASSWORD;


        private int INCORRECT_LOGIN_ATTEMPTS;


        [SetUp]
        public void Setup()
        {
            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

            //read user login data from config file
            DEFAULT_USERNAME = ConfigurationManager.AppSettings["DefaultUsername"];
            DEFAULT_PASSWORD = ConfigurationManager.AppSettings["DefaultUserPassword"];
            INCORRECT_LOGIN_ATTEMPTS = Int32.Parse(ConfigurationManager.AppSettings["IncorrectLoginAttempts"]);

        }

        [Test, Description("Verify user is redirected to home page when valid username passowrd credentials are given")]
        public void SuccessfullLogin()
        {

            DriverFactory.GoToUrl(baseUrl);
            LoginPage loginPage = new LoginPage(DriverFactory.Driver);
            loginPage.PerformLogin(DEFAULT_USERNAME, DEFAULT_PASSWORD);

            //verify after login url is the home page
            Assert.AreNotEqual(DriverFactory.GetUrl(), LoginPage.url);
            Assert.IsTrue(LoginPage.IsLoggedIn);
        }


        [Test, Description("Verify an error is displayed when the incorrect password is entered")]
        public void IncorrectPassword()
        {
            DriverFactory.GoToUrl(baseUrl);
            LoginPage loginPage = new LoginPage(DriverFactory.Driver);
            string expectedUrl = LoginPage.url + "?ReturnUrl=%2F";

            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, "invalid");

            Assert.IsTrue(loginPage.validationErrorIsDisplayed());
            Assert.AreEqual(DriverFactory.GetUrl(), expectedUrl);
            Assert.IsFalse(LoginPage.IsLoggedIn);
        }


        [Test, Description("Verify error is displayed when email field is blank")]
        public void EmailIsBlankError()
        {
            const string expectedError = "Please enter a valid email.";
            DriverFactory.GoToUrl(baseUrl);

            LoginPage loginPage = new LoginPage(DriverFactory.Driver);


            loginPage.EnterPassword("password");
            loginPage.ClickLogin();

            //check text of the error
            Assert.IsTrue(loginPage.emailErrorIsDisplayed());
            Assert.AreEqual(expectedError, loginPage.getEmailErrorText());
            Assert.IsFalse(LoginPage.IsLoggedIn);

        }


        //Tests invalid email format. Not in format XXX@XXX.XXX
        //Test case fails because child span dissapears after entering XXX@XXX
        [Test, Description("Verify error is displayed when the email is in invalid format")]
        public void InvalidEmailError()
        {
            bool errorIsDisplayed = false;

            DriverFactory.GoToUrl(baseUrl);

            LoginPage loginPage = new LoginPage(DriverFactory.Driver);

            //invalid email missing @ and .
            loginPage.PerformLogin("username", "Diamond1!");
            Assert.IsTrue(loginPage.emailErrorIsDisplayed());

            //missing .
            errorIsDisplayed = false;
            loginPage.PerformLogin("username@", "Diamond1!");
            Assert.IsTrue(loginPage.emailErrorIsDisplayed());

            //missing .com
            errorIsDisplayed = false;
            loginPage.PerformLogin("username@abcd", "Diamond1!");
            Assert.IsTrue(loginPage.emailErrorIsDisplayed());

            Assert.IsFalse(LoginPage.IsLoggedIn);

        }

        [Test, Description("Verify error is displayed when password field is blank")]
        public void BlankPassword()
        {
            const string expectedError = "Please enter a password.";

            DriverFactory.GoToUrl(baseUrl);

            LoginPage loginPage = new LoginPage(DriverFactory.Driver);

            loginPage.EnterUsername("test@test.com");
            loginPage.ClickLogin();

            //verify error is displayed
            Assert.IsTrue(loginPage.passwordErrorIsDisplayed());
            //verify error has correct text
            Assert.AreEqual(expectedError, loginPage.getPasswordErrorText());

            Assert.IsFalse(LoginPage.IsLoggedIn);
        }

        [Test, Description("Verify error is displayed when invalid login credentials are entered")]
        public void FailedLogin()
        {
            const string expectedError = "Invalid login attempt.";


            DriverFactory.GoToUrl(baseUrl);

            LoginPage loginPage = new LoginPage(DriverFactory.Driver);
            Assert.IsFalse(loginPage.validationErrorIsDisplayed());
            string expectedUrl = LoginPage.url + "?ReturnUrl=%2F";

            loginPage.PerformLogin("user@invalid.com", "password");

            //verify error is displayed and the error has correct text
            Assert.IsTrue(loginPage.validationErrorIsDisplayed());
            Assert.AreEqual(expectedError, loginPage.getValidationErrorText());

            //verify user is still on the login page and not on the homepage
            Assert.AreEqual(DriverFactory.GetUrl(), expectedUrl);
            Assert.IsFalse(LoginPage.IsLoggedIn);

        }

        [Test, Description("Verify user is locked out after 3 incorrect login attempts")]
        public void UserLockout()
        {
            const string expectedLockoutError = "Account is locked. Too many failed login attempts. Please try again later or contact your Administrator.";
            DriverFactory.GoToUrl(baseUrl);

            LoginPage loginPage = new LoginPage(DriverFactory.Driver);

            //provide incorrect credentials twice - invalid login should appear
            for (int i = 0; i < INCORRECT_LOGIN_ATTEMPTS - 1; i++)
            {
                loginPage.PerformLogin("gsimmons@diamondgame.com", "password");
                Assert.IsTrue(loginPage.validationErrorIsDisplayed());

            }

            //login with incorrect credentials a 3rd time. Now user is locked out
            loginPage.PerformLogin("gsimmons@diamondgame.com", "password");

            Assert.IsTrue(loginPage.validationErrorIsDisplayed());

            Assert.AreEqual(expectedLockoutError, loginPage.getValidationErrorText());
            Assert.IsFalse(LoginPage.IsLoggedIn);
        }

        [Test, Description("Verify user is can reset password from login page")]
        public void ResetPassword()
        {
            string expectedUrl = ResetPasswordPage.url;

            DriverFactory.GoToUrl(baseUrl);

            LoginPage loginPage = new LoginPage(DriverFactory.Driver);
            loginPage.ClickResetPassword();

            //verify user was directed to the reset password page
            Assert.AreEqual(DriverFactory.GetUrl(), expectedUrl);
        }


        [Test, Description("Verify error is displayed when email doesn't exist in the system")]
        public void EmailDoesntExist()
        {
            DriverFactory.GoToUrl(baseUrl);

            LoginPage loginPage = new LoginPage(DriverFactory.Driver);
            loginPage.PerformLogin(NameGenerator.GenerateUsername(5), "Diamond1!");

            Assert.IsTrue(loginPage.validationErrorIsDisplayed());
            Assert.IsFalse(LoginPage.IsLoggedIn);
        }


        [TearDown]
        public void EndTest()
        {
            DriverFactory.CloseDriver();
        }
    }
}