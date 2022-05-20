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
    public class ResetPasswordTest
    {
        string baseUrl;
        private string DEFAULT_USERNAME;
        private string DEFAULT_PASSWORD;

        [SetUp]
        public void Setup()
        {
            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];
            DEFAULT_USERNAME = ConfigurationManager.AppSettings["DefaultUsername"];
            DEFAULT_PASSWORD = ConfigurationManager.AppSettings["DefaultUserPassword"];

        }

        [Test, Description("Successfully reset password")]
        public void SuccessfullPasswordReset()
        {
            DriverFactory.GoToUrl(ResetPasswordPage.url);
            string expectedUrl = baseUrl + "Identity/Account/ForgotPasswordConfirmation";
            ResetPasswordPage resetPage = new ResetPasswordPage(DriverFactory.Driver);

            resetPage.PerformPasswordReset(DEFAULT_USERNAME);

            Assert.AreEqual(DriverFactory.GetUrl(), expectedUrl);
        }

        [Test, Description("Enter empty string for email input")]
        public void BlankEmail()
        {
            DriverFactory.GoToUrl(ResetPasswordPage.url);
            string expectedError = "User Name is required.";

            ResetPasswordPage resetPage = new ResetPasswordPage(DriverFactory.Driver);

            resetPage.PerformPasswordReset("");

            //verify error is not empty and has correct message
            Assert.IsTrue(resetPage.ValidationErrorIsDisplayed());
            Assert.AreEqual(expectedError, resetPage.GetValidationError());

            //verify user is still on the reset password page
            Assert.AreEqual(DriverFactory.GetUrl(), ResetPasswordPage.url);
        }

        [Test, Description("Enter an invalid email format")]
        public void InvalidEmail()
        {
            DriverFactory.GoToUrl(ResetPasswordPage.url);
            string errorText = "";
            string expectedError = "User Name must be a valid email address.";

            ResetPasswordPage resetPage = new ResetPasswordPage(DriverFactory.Driver);

            //enter email missing @ and .
            resetPage.PerformPasswordReset("email");
            Assert.IsTrue(resetPage.ValidationErrorIsDisplayed());
            Assert.AreEqual(resetPage.GetValidationError(), expectedError);

            //enter email missing .
            DriverFactory.Driver.Navigate().Refresh();
            resetPage.clearEmail();
            resetPage.PerformPasswordReset("email@");
            Assert.IsTrue(resetPage.ValidationErrorIsDisplayed());
            Assert.AreEqual(resetPage.GetValidationError(), expectedError);

            //enter email missing text at end of period
            DriverFactory.Driver.Navigate().Refresh();
            resetPage.clearEmail();
            resetPage.PerformPasswordReset("email@test.");
            Assert.IsTrue(resetPage.ValidationErrorIsDisplayed());
            Assert.AreEqual(resetPage.GetValidationError(), expectedError);

            //enter email with numbers in the domain
            DriverFactory.Driver.Navigate().Refresh();
            resetPage.clearEmail();
            resetPage.PerformPasswordReset("email@test.123");
            Assert.IsTrue(resetPage.ValidationErrorIsDisplayed());
            Assert.AreEqual(resetPage.GetValidationError(), expectedError);
        }

        [Test, Description("User is returned to login page when pressing the Cancel button")]
        public void TestCancel()
        {
            DriverFactory.GoToUrl(ResetPasswordPage.url);
            ResetPasswordPage resetPage = new ResetPasswordPage(DriverFactory.Driver);
            string expectedUrl = LoginPage.url;

            resetPage.ClickCancel();

            Assert.AreEqual(DriverFactory.GetUrl(), expectedUrl);
        }

        [Test, Description("Test the confirm password reset page")]
        public void TestPasswordConfirmation()
        {
            DriverFactory.GoToUrl(ResetPasswordPage.url);
            ResetPasswordPage resetPage = new ResetPasswordPage(DriverFactory.Driver);
            string expectedText = "If you have an account, instructions for resetting your password will be sent to your email on record. Please allow up to 10 minutes for the email to be delivered.";
            resetPage.PerformPasswordReset(DEFAULT_USERNAME);
            string text = resetPage.getPasswordConfirmationText();

            Assert.AreEqual(expectedText, text);
        }


        [TearDown]
        public void EndTest()
        {
            DriverFactory.CloseDriver();
        }
    }
}