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
    internal class DeviceDetailsPage
    {
        [FindsBy(How = How.Id, Using = "_hidden.DeviceId")]
        private IWebElement DeviceId;

        [FindsBy(How = How.Id, Using = "SerialNumber")]
        private IWebElement SerialNumber;

        [FindsBy(How = How.Id, Using = "DeviceActivationDate")]
        private IWebElement ActivationDate;

        [FindsBy(How = How.Id, Using = "SystemVersion")]
        private IWebElement Version;

        [FindsBy(How = How.Id, Using = "LastHeartbeat")]
        private IWebElement LastHeartbeat;

        [FindsBy(How = How.ClassName, Using = "back-link")]
        private IWebElement BackButton;

        [FindsBy(How = How.XPath, Using = "//button[@type='submit']")]
        private IWebElement SubmitButton;

        [FindsBy(How = How.ClassName, Using = "alert-success")]
        private IWebElement SuccessBanner;

        [FindsBy(How = How.Id, Using = "PaymentTerminals_0__ExternalTerminalId")]
        private IWebElement PaymentTerminalId1;

        [FindsBy(How = How.Id, Using = "PaymentTerminals_1__ExternalTerminalId")]
        private IWebElement PaymentTerminalId2;

        [FindsBy(How = How.Id, Using = "PaymentTerminals_2__ExternalTerminalId")]
        private IWebElement PaymentTerminalId3;

        [FindsBy(How = How.Id, Using = "PaymentTerminals_3__ExternalTerminalId")]
        private IWebElement PaymentTerminalId4;

        [FindsBy(How = How.Id, Using = "btnClear")]
        private IWebElement ClearActivationButton;

        [FindsBy(How = How.Id, Using = "deviceActivationDateText")]
        private IWebElement ClearActivationConfirmationText;

        public ModalPage ConfirmationModal { get; set; }

        private IWebDriver driver;

        private WebDriverWait wait;

        private Header Header;

        public SiteNavMenu EntityTabs { get; set; }

        private BursterForm Bursters;

        private PaymentTerminalForm PaymentTerminals;


        public DeviceDetailsPage(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            ConfirmationModal = new ModalPage(driver);
            Header = new Header(driver);
            EntityTabs = new SiteNavMenu(driver);

            Bursters = new BursterForm(driver);
            PaymentTerminals = new PaymentTerminalForm(driver);
        }


        public int GetDeviceId()
        {
            wait.Until(d => d.FindElement(By.Id(DeviceId.GetAttribute("id"))));
            string idString = DeviceId.GetAttribute("value");
            int deviceId = int.Parse(idString);

            return deviceId;
        }


        public string GetActivationDate()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("DeviceActivationDate")));

            return ActivationDate.GetAttribute("value");
        }


        public string GetVersion()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("SystemVersion")));

            return Version.GetAttribute("value");
        }

        public string GetLastHeartBeat()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("LastHeartbeat")));

            return Version.GetAttribute("value");
        }


        public void EnterSerialNumber(string text)
        {
            wait.Until(d => d.FindElement(By.Id(SerialNumber.GetAttribute("id"))));

            SerialNumber.Clear();
            SerialNumber.SendKeys(text);
        }


        public string GetSerialNumber()
        {
            wait.Until(d => d.FindElement(By.Id(SerialNumber.GetAttribute("id"))));

            return SerialNumber.GetAttribute("value");
        }


        public void EnterTerminal1Id(string text)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("PaymentTerminals_0__ExternalTerminalId")));

            PaymentTerminalId1.Clear();
            PaymentTerminalId1.SendKeys(text);
        }


        public void EnterTerminal2Id(string text)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("PaymentTerminals_1__ExternalTerminalId")));

            PaymentTerminalId2.Clear();
            PaymentTerminalId2.SendKeys(text);
        }


        public void EnterTerminal3Id(string text)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("PaymentTerminals_2__ExternalTerminalId")));

            PaymentTerminalId3.Clear();
            PaymentTerminalId3.SendKeys(text);
        }


        public void EnterTerminal4Id(string text)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("PaymentTerminals_3__ExternalTerminalId")));

            PaymentTerminalId4.Clear();
            PaymentTerminalId4.SendKeys(text);
        }


        public void ClickBackButton()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.ClassName("back-link")));

            BackButton.Click();
            ConfirmationModal.WaitForModal();
        }


        public void ClickClearActivationDate()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("btnClear")));
            ClearActivationButton.Click();
        }


        public void ClearActivationDate()
        {
            ClickClearActivationDate();
            ClickSubmit();
        }


        public bool ClearActivationConfirmationIsDisplayed()
        {
            try
            {
                wait.Until(d => d.FindElement(By.Id("deviceActivationDateText")).Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public void GoBackToDeviceList()
        {
            //scroll up to back button or else throws click intercepted exception
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].scrollIntoView(true)", EntityTabs.DeviceTab);

            BackButton.Click();
            ConfirmationModal.ClickSubmit();

        }


        public void ClickSubmit()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//button[@type='submit']")));

            SubmitButton.Click();
        }


        public bool SuccessBannerIsDisplayed()
        {
            try
            {
                wait.Until(d => d.FindElement(By.ClassName("alert-success")));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public bool PageLoaded()
        {
            try
            {
                wait.Until(d => d.FindElement(By.Id("PaymentTerminals_3__ExternalTerminalId")));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public int GetBursterId(int index)
        {
            return Bursters.GetBursterId(index);
        }


        public string GetGameName(int index)
        {
            string val = Bursters.GetGameName(index);

            if (val == "")
            {
                return null;
            }

            int i = val.IndexOf('|');
            return val.Substring(0, i - 1);
        }


        public string GetGameUPC(int index)
        {
            string val = Bursters.GetGameName(index);

            if (val == "")
            {
                return null;
            }

            int i = val.IndexOf('|');

            return val.Substring(i + 2, 12);
        }


        public int GetDrawerNum(int index)
        {
            return Bursters.GetDrawerNum(index);
        }


        public int GetGameId(int index)
        {
            return Bursters.GetGameId(index);
        }


        public int GetPackSize(int index)
        {
            return Bursters.GetPackSize(index);
        }


        public int GetPrice(int index)
        {
            return Bursters.GetPrice(index);
        }


        public int GetStock(int index)
        {
            return Bursters.GetStock(index);
        }


        public bool BursterIsActive(int index)
        {
            return Bursters.GetActive(index);
        }


        public bool GameIsLoaded(int index)
        {
            if (Bursters.GetGameId(index) == 0)
            {
                return false;
            }

            return true;
        }


        public string GetBursterStatusText(int index)
        {
            return Bursters.GetStatusText(index);
        }


        public int GetPaymentTerminalId(int index)
        {
            return PaymentTerminals.GetTerminalId(index);
        }


        public string GetPaymentTerminalName(int index)
        {
            return PaymentTerminals.GetTerminalName(index);
        }


        public void EnterPaymentTerminalName(int index, string text)
        {
            PaymentTerminals.EnterTerminalName(index, text);
        }


        public string GetExternalTerminalId(int index)
        {
            return PaymentTerminals.GetExternalId(index);
        }


        public void EnterExternalTerminalId(int index, string text)
        {
            PaymentTerminals.EnterExternalId(index, text);
        }


        public bool TerminalNameErrorIsDisplayed(int index)
        {
            return PaymentTerminals.TerminalNameErrorIsDisplayed(index);
        }


        public bool ExternalIdErrorIsDisplayed(int index)
        {
            return PaymentTerminals.ExternalIdErrorIsDisplayed(index);
        }
    }
}
