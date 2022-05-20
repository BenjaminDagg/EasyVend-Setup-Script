using NUnit.Framework;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
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
    internal class ModalPage
    {
        protected IWebDriver driver;
        protected WebDriverWait wait;

        [FindsBy(How = How.Id, Using = "myModal")]
        protected IWebElement Modal;

        [FindsBy(How = How.ClassName, Using = "modal-title")]
        protected IWebElement ModalTitle;

        // ========== buttons ==============
        [FindsBy(How = How.Id, Using = "btnSubmit")]
        protected IWebElement SubmitButton;

        [FindsBy(How = How.Id, Using = "btnCancel")]
        protected IWebElement CancelButton;

        public bool IsOpen
        {
            get
            {
                return modalIsOpen();
            }
        }

        public ModalPage(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }


        /*
         * Waits until modal div changes from "display: none" to "display: block" after pressing
         * Add User button
         */
        public virtual void WaitForModal()
        {

            wait.Until(driver => driver.FindElement(By.Id("myModal")).GetAttribute("style").Contains("block"));
        }


        /*
         * Waits for modal to close by checking display status is "none"
         */
        public virtual void waitForModalClose()
        {
            wait.Until(driver => driver.FindElement(By.Id("myModal")).GetAttribute("style").Contains("none"));
        }


        public void waitForModalTitle(string expectedText)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElement(ModalTitle, expectedText));
        }


        private bool modalIsOpen()
        {
            try
            {
                WaitForModal();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public void ClickSubmit()
        {
            WaitForModal();
            SubmitButton.Click();
            //waitForModalClose();
        }


        public void ClickCancel()
        {
            WaitForModal();
            CancelButton.Click();
            waitForModalClose();
        }


        //used for reset password modal since there is a second conformation you have to click
        public void CloseConfirmation()
        {
            WaitForModal();
            SubmitButton.Click();
            waitForModalTitle("Reset Email Sent");
            CancelButton.Click();
            waitForModalClose();
        }

    }
}
