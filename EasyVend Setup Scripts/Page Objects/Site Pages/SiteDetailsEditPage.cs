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
    internal class SiteDetailsEditPage : EntityDetailsPage
    {

        public SiteNavMenu EntityTabs { get; set; }
        public static string url = ConfigurationManager.AppSettings["URL"] + "Site/Details?entityId=";

        //site id (easyvend)/internal id(DMB) field
        [FindsBy(How = How.Id, Using = "EntityId")]
        private IWebElement SiteId { get; set; }

        //lottery id(DMB) / agent#(easyvend) field
        [FindsBy(How = How.Id, Using = "AgentNumber")]
        private IWebElement AgentNumber { get; set; }   //EasyVend = Agent #, DMB = Lottery Site ID

        [FindsBy(How = How.Id, Using = "ExternalStoreId")]
        private IWebElement ExternalStoreId { get; set; }   //only used for EasyVend TMS

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.AgentNumber']")]
        protected IWebElement AgentNumberError { get; set; }  //LotterySiteId (DMB) / Agent# (EasyVend) error

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='ExternalStoreId']")]
        protected IWebElement ExternalStoreIdError { get; set; }    //only used in EasyVend TMS


        public SiteDetailsEditPage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);

            EntityTabs = new SiteNavMenu(driver);
        }


        public int GetSiteId()
        {
            WaitForElement(By.Id("EntityId"));
            string rawId = SiteId.GetAttribute("value");
            int id = int.Parse(rawId);

            return id;
        }


        public void EnterSiteId(string text)
        {
            WaitForElement(By.Id("EntityId"));
            SiteId.SendKeys(text);
        }


        public string GetAgentNumber()
        {
            WaitForElement(By.Id("AgentNumber"));
            return AgentNumber.GetAttribute("value");
        }


        public void EnterAgentNumber(string text)
        {
            WaitForElement(By.Id("AgentNumber"));
            AgentNumber.Clear();
            AgentNumber.SendKeys(text);
        }


        public string GetExernalStoreId()
        {
            WaitForElement(By.Id("ExternalStoreId"));
            return ExternalStoreId.GetAttribute("value");
        }


        public void EnterExternalStoreId(string text)
        {
            WaitForElement(By.Id("ExternalStoreId"));
            ExternalStoreId.Clear();
            ExternalStoreId.SendKeys(text);
        }


        public bool ExternalStoreIdErrorIsDisplayed()
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


        public bool SiteMatches(SiteTableRecord site)
        {
            elementWait.Until(d => d.FindElement(By.Id(ZipcodeField.GetAttribute("id"))));

            SiteTableRecord newSite = new SiteTableRecord();
            newSite.SiteName = getEntityName();
            newSite.Id = GetSiteId();
            newSite.AgentNumber = GetAgentNumber();
            newSite.Phone = getPhone();

            return (
                newSite.SiteName == site.SiteName &&
                newSite.Id == site.Id &&
                newSite.AgentNumber == site.AgentNumber &&
                newSite.Phone == site.Phone
            );
        }


        public override void EnterForm(string name, string agentNum, string fName, string lName,
            string email, string Phone, string address, string City, string Zipcode)
        {
            base.EnterForm(name, fName, lName, email, Phone, address, City, Zipcode);
            EnterAgentNumber(agentNum);
        }


        public override void EnterForm(string name, string agentNum, string externalStoreId, string fName, string lName,
            string email, string Phone, string address, string City, string Zipcode)
        {
            base.EnterForm(name, fName, lName, email, Phone, address, City, Zipcode);
            EnterAgentNumber(agentNum);
            EnterExternalStoreId(externalStoreId);
        }


        public SiteTableRecord GetSiteRecord()
        {
            SiteTableRecord site = new SiteTableRecord();
            site.SiteName = getEntityName();
            site.Id = GetSiteId();
            site.AgentNumber = GetAgentNumber();
            site.Phone = getPhone();

            return site;
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
                case EntityDetailsInputs.ID:
                    return SiteId;
                case EntityDetailsInputs.AGENT_NUM:
                    return AgentNumber;
                case EntityDetailsInputs.EXTERNAL_STOREID:
                    return ExternalStoreId;
                default:
                    return StateSelect;
            }

        }
    }



}
