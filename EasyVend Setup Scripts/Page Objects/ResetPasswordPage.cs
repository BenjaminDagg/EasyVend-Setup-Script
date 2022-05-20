using NUnit.Framework;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras;
using SeleniumExtras.PageObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class ResetPasswordPage
    {
        IWebDriver driver;
        WebDriverWait wait;
        public static string url = ConfigurationManager.AppSettings["URL"] + "Identity/Account/ForgotPassword";

        //page web elements
        [FindsBy(How = How.Id, Using = "Input_Email")]
        private IWebElement Email { get; set; }

        [FindsBy(How = How.Id, Using = "btnCancel")]
        private IWebElement CancelButton { get; set; }

        [FindsBy(How = How.Id, Using = "btnSend")]
        private IWebElement SendButton { get; set; }

        [FindsBy(How = How.ClassName, Using = "text-danger")]
        private IWebElement ValidationError { get; set; }

        [FindsBy(How = How.XPath, Using = "//*[@class='row login-body']/div")]
        private IWebElement PasswordConfirmation { get; set; }

        //constructor
        public ResetPasswordPage(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));


        }

        //enter text in email input
        public void EnterEmail(string email)
        {
            Email.Clear();
            Email.SendKeys(email);
        }

        //clear test in email input
        public void clearEmail()
        {
            Email.Clear();
        }

        public void ClickCancel()
        {
            CancelButton.Click();
        }

        public void ClickSend()
        {
            SendButton.Click();
        }

        public void PerformPasswordReset(string email)
        {
            EnterEmail(email);
            ClickSend();
        }


        private void WaitForValidationError()
        {
            wait.Until(d => ValidationError.Text.Length > 0);
        }


        public bool ValidationErrorIsDisplayed()
        {
            try
            {
                WaitForValidationError();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }


        public string GetValidationError()
        {
            WaitForValidationError();
            return ValidationError.Text;
        }


        public string getPasswordConfirmationText()
        {

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//*[@class='row login-body']/div")));
            return PasswordConfirmation.Text;
        }
    }
}
