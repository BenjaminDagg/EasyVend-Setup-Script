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
    internal class ChangePasswordPageTest
    {
        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private Header header;
        private ChangePasswordPage changePassword;

        private int MIN_PASSWORD_LENGTH;


        [SetUp]
        public void Setup()
        {
            DriverFactory.InitDriver();

            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            header = new Header(DriverFactory.Driver);
            changePassword = new ChangePasswordPage(DriverFactory.Driver);


            MIN_PASSWORD_LENGTH = int.Parse(ConfigurationManager.AppSettings["PasswordMinLength"]);

        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            header = null;
            DriverFactory.CloseDriver();
        }


        [Test, Description("Verify error is displayed when incorrect current password is enetered"), Order(10)]
        public void IncorrectCurrentPassword()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to change password page
            header.ClickUserDropdown();
            header.ClickChangePassword();

            string newPassword = "Diamond0!";
            changePassword.EnterCurrentPassword(AppUsers.SITE_REPORT.Password + "invalid");
            changePassword.EnterNewPassword(newPassword);
            changePassword.EnterConfirmPassword(newPassword);
            changePassword.ClickSubmit();

            Assert.IsTrue(changePassword.CurrentPasswordErrorIsDisplayed());
        }


        [Test, Description("Verify error is displayed when password doesn't meet minimum length"), Order(2)]
        public void PasswordTooShort()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to change password page
            header.ClickUserDropdown();
            header.ClickChangePassword();

            string newPassword = "Diam1!";
            changePassword.EnterCurrentPassword(AppUsers.SITE_REPORT.Password);
            changePassword.EnterNewPassword(newPassword);
            changePassword.EnterConfirmPassword(newPassword);
            changePassword.ClickSubmit();

            Assert.IsTrue(changePassword.NewPasswordErrorIsDisplayed());
        }


        [Test, Description("Verify error is displayed when new password and confirm password don't match"), Order(3)]
        public void PasswordMismatch()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to change password page
            header.ClickUserDropdown();
            header.ClickChangePassword();

            changePassword.EnterCurrentPassword(AppUsers.SITE_REPORT.Password);
            changePassword.EnterNewPassword("Diamond0!");
            changePassword.EnterConfirmPassword("Diamond9!");
            changePassword.ClickSubmit();

            Assert.IsTrue(changePassword.ConfirmPasswordErrorIsDisplayed());
        }


        [Test, Description("Verify error is displayed if any of the fields are empty"), Order(4)]
        public void EmptyFieldError()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to change password page
            header.ClickUserDropdown();
            header.ClickChangePassword();

            //current password field is empty
            changePassword.EnterCurrentPassword("");
            changePassword.EnterNewPassword("Diamond0!");
            changePassword.EnterConfirmPassword("Diamond0!");
            changePassword.ClickSubmit();
            Assert.IsTrue(changePassword.CurrentPasswordErrorIsDisplayed());

            //new password field is empty
            changePassword.EnterCurrentPassword(AppUsers.SITE_REPORT.Password);
            changePassword.EnterNewPassword("");
            changePassword.EnterConfirmPassword("Diamond0!");
            changePassword.ClickSubmit();
            Assert.IsTrue(changePassword.NewPasswordErrorIsDisplayed());

            //confirm password field is empty
            changePassword.EnterCurrentPassword(AppUsers.SITE_REPORT.Password);
            changePassword.EnterNewPassword("Diamond0!");
            changePassword.EnterConfirmPassword("");
            changePassword.ClickSubmit();
            Assert.IsTrue(changePassword.ConfirmPasswordErrorIsDisplayed());

        }


        [Test, Description("Verify error is displayed when new password is less than 8 characters"), Order(5)]
        public void PasswordLengthError()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to change password page
            header.ClickUserDropdown();
            header.ClickChangePassword();

            int length = MIN_PASSWORD_LENGTH - 3;
            string password = "d" + NameGenerator.GenerateEntityName(length) + "!";

            changePassword.EnterCurrentPassword(AppUsers.SITE_REPORT.Password);
            changePassword.EnterNewPassword(password);
            changePassword.EnterConfirmPassword(password);
            changePassword.ClickSubmit();

            Assert.IsTrue(changePassword.NewPasswordErrorIsDisplayed());
        }


        [Test, Description("Verify error is displayed when new password doesn't contain a number"), Order(6)]
        public void PasswordNumberError()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to change password page
            header.ClickUserDropdown();
            header.ClickChangePassword();

            changePassword.EnterCurrentPassword(AppUsers.SITE_REPORT.Password);
            changePassword.EnterNewPassword("Diamond!");
            changePassword.EnterConfirmPassword("Diamond!");
            changePassword.ClickSubmit();

            Assert.IsTrue(changePassword.ValidationErrorIsDisplayed());
        }


        [Test, Description("Verify error is displayed when new password doesn't contain an uppercase letter"), Order(7)]
        public void PasswordUppercaseError()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to change password page
            header.ClickUserDropdown();
            header.ClickChangePassword();

            changePassword.EnterCurrentPassword(AppUsers.SITE_REPORT.Password);
            changePassword.EnterNewPassword("diamond2!");
            changePassword.EnterConfirmPassword("diamond2!");
            changePassword.ClickSubmit();

            Assert.IsTrue(changePassword.ValidationErrorIsDisplayed());
        }


        [Test, Description("Verify error is displayed when new password doesn't contain a lowercase letter"), Order(8)]
        public void PasswordLowercaseError()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to change password page
            header.ClickUserDropdown();
            header.ClickChangePassword();

            changePassword.EnterCurrentPassword(AppUsers.SITE_REPORT.Password);
            changePassword.EnterNewPassword("DIAMOND2!");
            changePassword.EnterConfirmPassword("DIAMOND2!");
            changePassword.ClickSubmit();

            Assert.IsTrue(changePassword.ValidationErrorIsDisplayed());
        }


        [Test, Description("Verify error is displayed when new password doesn't contain a special character"), Order(9)]
        public void PasswordSpecialCharError()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to change password page
            header.ClickUserDropdown();
            header.ClickChangePassword();

            changePassword.EnterCurrentPassword(AppUsers.SITE_REPORT.Password);
            changePassword.EnterNewPassword("Diamond2");
            changePassword.EnterConfirmPassword("Diamond2");
            changePassword.ClickSubmit();

            Assert.IsTrue(changePassword.ValidationErrorIsDisplayed());
        }


        [Test, Order(1)]
        public void ChangePassword_Success()
        {
            //login
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, AppUsers.SITE_REPORT.Password);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //go to change password page
            header.ClickUserDropdown();
            header.ClickChangePassword();

            string newPassword = "a" + NameGenerator.GenerateEntityName(5) + "1!";

            changePassword.EnterCurrentPassword(AppUsers.SITE_REPORT.Password);
            changePassword.EnterNewPassword(newPassword);
            changePassword.EnterConfirmPassword(newPassword);
            changePassword.ClickSubmit();

            header.Logout();

            loginPage.PerformLogin(AppUsers.SITE_REPORT.Username, newPassword);
            Assert.IsTrue(LoginPage.IsLoggedIn);

            //save new password to config file
            string key = "SiteReportPassword";
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;
            try
            {
                if (settings[key] == null)
                {
                    settings.Add(key, newPassword);
                }
                else
                {
                    settings[key].Value = newPassword;
                }
                config.AppSettings.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception ex)
            {

            }

            AppUsers.RefreshSettings();
            ConfigurationManager.RefreshSection("appsettings");
        }

    }
}
