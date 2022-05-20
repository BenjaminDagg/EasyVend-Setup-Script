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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class PaymentTerminalForm
    {
        [FindsBy(How = How.Id, Using = "_hidden.PaymentTerminals[0].PaymentTerminalId")]
        public IWebElement TerminalId
        {
            get
            {
                return driver.FindElement(By.Id(@"_hidden.PaymentTerminals[" + index + "].PaymentTerminalId"));
            }
        }


        [FindsBy(How = How.Id, Using = "PaymentTerminals_0__Name")]
        public IWebElement TerminalName
        {
            get
            {
                return driver.FindElement(By.Id(@"PaymentTerminals_" + index + "__Name"));
            }
        }


        [FindsBy(How = How.Id, Using = "PaymentTerminals_0__ExternalTerminalId")]
        public IWebElement ExternalId
        {
            get
            {
                return driver.FindElement(By.Id(@"PaymentTerminals_" + index + "__ExternalTerminalId"));
            }
        }


        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='PaymentTerminals[0].Name']")]
        public IWebElement PaymentTerminalNameError
        {
            get
            {
                return driver.FindElement(By.XPath(@"//span[@data-valmsg-for='PaymentTerminals[" + index + "].Name']"));
            }
        }


        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='PaymentTerminals[0].ExternalTerminalId']")]
        public IWebElement ExternalTerminalIdError
        {
            get
            {
                return driver.FindElement(By.XPath(@"//span[@data-valmsg-for='PaymentTerminals[" + index + "].ExternalTerminalId']"));
            }
        }

        private int index;

        IWebDriver driver;
        WebDriverWait wait;


        public PaymentTerminalForm(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            this.index = 0;
        }


        public int GetTerminalId(int index)
        {

            this.index = index;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("_hidden.PaymentTerminals[0].PaymentTerminalId")));
            string val = TerminalId.GetAttribute("value");

            if (val == "")
            {
                return 0;
            }
            return int.Parse(TerminalId.GetAttribute("value"));
        }


        public string GetTerminalName(int index)
        {

            this.index = index;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("PaymentTerminals_0__Name")));
            string val = TerminalName.GetAttribute("value");

            return TerminalName.GetAttribute("value");
        }


        public void EnterTerminalName(int index, string text)
        {
            this.index = index;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("PaymentTerminals_0__Name")));

            TerminalName.Clear();
            TerminalName.SendKeys(text);
        }


        public string GetExternalId(int index)
        {
            this.index = index;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("PaymentTerminals_0__ExternalTerminalId")));

            return ExternalId.GetAttribute("value");
        }


        public void EnterExternalId(int index, string text)
        {
            this.index = index;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("PaymentTerminals_0__ExternalTerminalId")));

            ExternalId.Clear();
            ExternalId.SendKeys(text);
        }


        public bool TerminalNameErrorIsDisplayed(int index)
        {
            this.index = index;

            try
            {
                wait.Until(d => d.FindElement(By.XPath(@"//span[@data-valmsg-for='PaymentTerminals[" + index + "].Name']")).Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public bool ExternalIdErrorIsDisplayed(int index)
        {
            this.index = index;

            try
            {
                wait.Until(d => d.FindElement(By.XPath(@"//span[@data-valmsg-for='PaymentTerminals[" + index + "].ExternalTerminalId']")).Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

    }
}
