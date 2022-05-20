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
    internal class SiteDetailsAddPage : EntityDetailsPage
    {

        [FindsBy(How = How.Id, Using = "Input_EntityName")]
        public override IWebElement EntityNameField { get; set; }

        //lottery id(DMB) / agent#(easyvend) field
        [FindsBy(How = How.Id, Using = "Input_AgentNumber")]
        private IWebElement AgentNumber { get; set; }

        //only used in EasyVend TMS
        [FindsBy(How = How.Id, Using = "Input_ExternalStoreId")]
        private IWebElement ExternalStoreId { get; set; }

        [FindsBy(How = How.Id, Using = "Input_Contact_FirstName")]
        public override IWebElement FirstNameField { get; set; }

        [FindsBy(How = How.Id, Using = "Input_Contact_LastName")]
        public override IWebElement LastNameField { get; set; }

        [FindsBy(How = How.Id, Using = "Input_Contact_EmailAddress")]
        public override IWebElement EmailField { get; set; }

        [FindsBy(How = How.Id, Using = "Input_Contact_PhoneNumber")]
        public override IWebElement PhoneField { get; set; }

        [FindsBy(How = How.Id, Using = "txtPhone2")]
        public override IWebElement Phone2Field { get; set; }

        [FindsBy(How = How.Id, Using = "Input_Address_AddressLine1")]
        public override IWebElement Address1 { get; set; }

        [FindsBy(How = How.Id, Using = "Input_Address_AddressLine2")]
        public override IWebElement Address2 { get; set; }

        [FindsBy(How = How.Id, Using = "Input_Address_City")]
        public override IWebElement CityField { get; set; }

        [FindsBy(How = How.Id, Using = "Input_Address_ZipCode")]
        public override IWebElement ZipcodeField { get; set; }

        [FindsBy(How = How.Id, Using = "Input_Address_Country")]
        public override IWebElement CountryField { get; set; }

        [FindsBy(How = How.Id, Using = "Input_Address_AddressState_AddressStateId")]
        public override IWebElement StateSelect { get; set; }

        [FindsBy(How = How.XPath, Using = "//button[@type='submit']")]
        public override IWebElement Submit { get; set; }

        [FindsBy(How = How.XPath, Using = "//button[@type='button']")]
        private IWebElement Cancel { get; set; }

        [FindsBy(How = How.ClassName, Using = "alert-success")]
        public override IWebElement SuccessBanner { get; set; }

        //========== Input field errors ===========
        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.EntityName']")]
        public override IWebElement NameError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.AgentNumber']")]
        protected IWebElement AgentNumberError { get; set; }  //LotterySiteId (DMB) / Agent# (EasyVend) error

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.ExternalStoreId']")]
        protected IWebElement ExternalStoreIdError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.Contact.FirstName']")]
        public override IWebElement FNameError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.Contact.LastName']")]
        public override IWebElement LNameError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.Contact.EmailAddress']")]
        public override IWebElement EmailError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.Contact.PhoneNumber']")]
        public override IWebElement PhoneError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.Address.AddressLine1']")]
        public override IWebElement AddressError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.Address.City']")]
        public override IWebElement CityError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.Address.ZipCode']")]
        public override IWebElement ZipCodeError { get; set; }


        public ModalPage ConfirmationModal;
        public SiteNavMenu EntityTabs { get; set; }

        public static string url = ConfigurationManager.AppSettings["URL"] + "Site/AddSite";


        public SiteDetailsAddPage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);

            ConfirmationModal = new ModalPage(driver);
            EntityTabs = new SiteNavMenu(driver);
        }


        public override void ClickSubmit()
        {
            WaitForElement(By.XPath("//button[@type='submit']"));
            Submit.Click();
        }


        public void ClickCancel()
        {
            WaitForElement(By.XPath("//button[@type='button']"));
            Cancel.Click();
        }


        public string getExternalStoreId()
        {
            WaitForElement(By.Id("Input_ExternalStoreId"));
            return ExternalStoreId.GetAttribute("value");
        }

        public void EnterExternalStoreId(string text)
        {
            WaitForElement(By.Id("Input_ExternalStoreId"));
            ExternalStoreId.Clear();
            ExternalStoreId.SendKeys(text);
        }


        public string GetAgentNumber()
        {
            WaitForElement(By.Id("Input_AgentNumber"));
            return AgentNumber.GetAttribute("value");
        }


        public void EnterAgentNumber(string text)
        {
            WaitForElement(By.Id("Input_AgentNumber"));
            AgentNumber.SendKeys(text);
        }


        public bool ExternalStoreIdErrorIsShown()
        {
            try
            {

                errorWait.Until(d => ExternalStoreIdError.Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        /*
         * Takes in a EnttiyDetailsInput enum and returns the corresponding IWebElement
         * Overriden in site details page to account for siteId and agent# fields
         */
        protected override IWebElement getElement(EntityDetailsInputs element)
        {
            switch (element)
            {
                case EntityDetailsInputs.ENTITY_NAME:
                    return EntityNameField;
                case EntityDetailsInputs.FNAME:
                    return FirstNameField;
                case EntityDetailsInputs.LNAME:
                    return LastNameField;
                case EntityDetailsInputs.EMAIL:
                    return EmailField;
                case EntityDetailsInputs.PHONE:
                    return PhoneField;
                case EntityDetailsInputs.PHONE2:
                    return Phone2Field;
                case EntityDetailsInputs.ADDRESS1:
                    return Address1;
                case EntityDetailsInputs.ADDRESS2:
                    return Address2;
                case EntityDetailsInputs.CITY:
                    return CityField;
                case EntityDetailsInputs.ZIP:
                    return ZipcodeField;
                case EntityDetailsInputs.COUNTRY:
                    return CountryField;
                case EntityDetailsInputs.STATE:
                    return StateSelect;
                case EntityDetailsInputs.AGENT_NUM:
                    return AgentNumber;
                case EntityDetailsInputs.EXTERNAL_STOREID:
                    return ExternalStoreId;
                default:
                    return StateSelect;
            }

        }


        public SiteTableRecord GetSiteRecord()
        {
            SiteTableRecord site = new SiteTableRecord();
            site.SiteName = getEntityName();
            site.AgentNumber = GetAgentNumber();
            site.Phone = getPhone();

            return site;
        }


        public void CancelModal()
        {
            ConfirmationModal.ClickCancel();
        }


        public void ConfirmModal()
        {
            this.ConfirmationModal.WaitForModal();
            this.ConfirmationModal.ClickSubmit();
            this.ConfirmationModal.waitForModalClose();
        }


        public override void EnterForm(string name, string agentNum, string externalId, string fName, string lName,
            string email, string Phone, string address, string City, string Zipcode)
        {
            base.EnterForm(name, fName, lName, email, Phone, address, City, Zipcode);
            EnterAgentNumber(agentNum);
            EnterExternalStoreId(externalId);

        }
    }
}
