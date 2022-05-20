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
    internal class ChangePasswordPage
    {
        IWebDriver driver;
        WebDriverWait wait;

        [FindsBy(How = How.Id, Using = "Input_Password")]
        private IWebElement CurrentPasswordField;

        [FindsBy(How = How.Id, Using = "Input_NewPassword")]
        private IWebElement NewPasswordField;

        [FindsBy(How = How.Id, Using = "Input_ConfirmPassword")]
        private IWebElement ConfirmPasswordField;

        [FindsBy(How = How.XPath, Using = "//button[@type='submit']")]
        private IWebElement SubmitButton;

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.Password']")]
        private IWebElement CurrentPasswordError;

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.NewPassword']")]
        private IWebElement NewPasswordError;

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.ConfirmPassword']")]
        private IWebElement ConfirmPasswordError;

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='ValidationError']")]
        private IWebElement ValidationError;

        public ChangePasswordPage(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }


        private void WaitForElement(IWebElement element)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id(element.GetAttribute("id"))));
        }


        private void WaitForError(IWebElement element)
        {
            wait.Until(d => element.Text.Length > 0);
        }


        private bool ErrorIsDisplayed(IWebElement element)
        {
            try
            {
                WaitForError(element);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public void EnterCurrentPassword(string text)
        {
            WaitForElement(CurrentPasswordField);
            CurrentPasswordField.Clear();
            CurrentPasswordField.SendKeys(text);
        }


        public void EnterNewPassword(string text)
        {
            WaitForElement(NewPasswordField);
            NewPasswordField.Clear();
            NewPasswordField.SendKeys(text);
        }


        public void EnterConfirmPassword(string text)
        {
            WaitForElement(ConfirmPasswordField);
            ConfirmPasswordField.Clear();
            ConfirmPasswordField.SendKeys(text);
        }


        public void ClickSubmit()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//button[@type='submit']")));
            SubmitButton.Click();
        }


        public bool CurrentPasswordErrorIsDisplayed()
        {
            return ErrorIsDisplayed(CurrentPasswordError);
        }


        public bool NewPasswordErrorIsDisplayed()
        {
            return ErrorIsDisplayed(NewPasswordError);
        }


        public bool ConfirmPasswordErrorIsDisplayed()
        {
            return ErrorIsDisplayed(ConfirmPasswordError);
        }


        public bool ValidationErrorIsDisplayed()
        {
            return ErrorIsDisplayed(ValidationError);
        }



    }
}
