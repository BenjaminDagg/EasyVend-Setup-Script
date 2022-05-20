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
    internal class BarcodeModal
    {

        [FindsBy(How = How.XPath, Using = "//button[text()='CANCEL']")]
        private IWebElement CancelButton;

        [FindsBy(How = How.XPath, Using = "//button[text()='DELETE']")]
        private IWebElement DeleteButton;

        [FindsBy(How = How.ClassName, Using = "modal-body")]
        private IWebElement ModalContent;

        IWebDriver driver;

        WebDriverWait wait;


        public bool IsOpen
        {
            get
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
        }

        public BarcodeModal(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }


        //waits for modal to open by checking if modal body is displayed
        public void WaitForModal()
        {

            wait.Until(driver => driver.FindElement(By.ClassName("modal-body")).Text.Length > 0);
        }


        //waits for modal to close by checking that button doesn't exist
        public void waitForModalClose()
        {
            wait.Until(d => d.FindElements(By.XPath("//button[text()='CANCEL']")).Count == 0);
        }


        public void ClickCancel()
        {
            WaitForModal();
            CancelButton.Click();
            waitForModalClose();
        }


        public void ClickDelete()
        {
            WaitForModal();
            DeleteButton.Click();
            waitForModalClose();
        }


        public string GetModalContent()
        {
            WaitForModal();
            return ModalContent.Text;
        }


        //parses the barcode from the modal body
        public string ParseBarcode()
        {
            string body = GetModalContent();
            int index = body.IndexOf("PBL");

            string barcode = body.Substring(index, 13);

            return barcode;
        }
    }
}
