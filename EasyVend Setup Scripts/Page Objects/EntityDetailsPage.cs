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
    public enum EntityDetailsInputs
    {
        ENTITY_NAME,
        ID,
        AGENT_NUM,
        EXTERNAL_STOREID,
        FNAME,
        LNAME,
        EMAIL,
        PHONE,
        PHONE2,
        ADDRESS1,
        ADDRESS2,
        CITY,
        ZIP,
        COUNTRY,
        STATE
    }

    internal abstract class EntityDetailsPage
    {
        protected IWebDriver driver;
        protected WebDriverWait elementWait;
        protected WebDriverWait errorWait;
        protected readonly double ERROR_TIMEOUT_SEC = 2;
        protected readonly double ELEMENT_TIMEOUT_SEC = 5;

        [FindsBy(How = How.Id, Using = "EntityName")]
        public virtual IWebElement EntityNameField { get; set; }

        [FindsBy(How = How.Id, Using = "Contact_FirstName")]
        public virtual IWebElement FirstNameField { get; set; }

        [FindsBy(How = How.Id, Using = "Contact_LastName")]
        public virtual IWebElement LastNameField { get; set; }

        [FindsBy(How = How.Id, Using = "Contact_EmailAddress")]
        public virtual IWebElement EmailField { get; set; }

        [FindsBy(How = How.Id, Using = "Contact_PhoneNumber")]
        public virtual IWebElement PhoneField { get; set; }

        [FindsBy(How = How.Id, Using = "txtPhone2")]
        public virtual IWebElement Phone2Field { get; set; }

        [FindsBy(How = How.Id, Using = "Address_AddressLine1")]
        public virtual IWebElement Address1 { get; set; }

        [FindsBy(How = How.Id, Using = "Address_AddressLine2")]
        public virtual IWebElement Address2 { get; set; }

        [FindsBy(How = How.Id, Using = "Address_City")]
        public virtual IWebElement CityField { get; set; }

        [FindsBy(How = How.Id, Using = "Address_ZipCode")]
        public virtual IWebElement ZipcodeField { get; set; }

        [FindsBy(How = How.Id, Using = "Address_Country")]
        public virtual IWebElement CountryField { get; set; }

        [FindsBy(How = How.Id, Using = "Address_AddressState_AddressStateId")]
        public virtual IWebElement StateSelect { get; set; }

        [FindsBy(How = How.XPath, Using = "//input[@type='submit']")]
        public virtual IWebElement Submit { get; set; }

        [FindsBy(How = How.ClassName, Using = "alert-success")]
        public virtual IWebElement SuccessBanner { get; set; }

        //========== Input field errors ===========//
        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='EntityName']")]
        public virtual IWebElement NameError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Contact.FirstName']")]
        public virtual IWebElement FNameError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Contact.LastName']")]
        public virtual IWebElement LNameError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Contact.EmailAddress']")]
        public virtual IWebElement EmailError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Contact.PhoneNumber']")]
        public virtual IWebElement PhoneError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Address.AddressLine1']")]
        public virtual IWebElement AddressError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Address.City']")]
        public virtual IWebElement CityError { get; set; }

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Address.ZipCode']")]
        public virtual IWebElement ZipCodeError { get; set; }

        public EntityDetailsPage(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);

            elementWait = new WebDriverWait(driver, TimeSpan.FromSeconds(ELEMENT_TIMEOUT_SEC));
            elementWait.PollingInterval = TimeSpan.FromMilliseconds(250);

            errorWait = new WebDriverWait(driver, TimeSpan.FromSeconds(ERROR_TIMEOUT_SEC));
            errorWait.PollingInterval = TimeSpan.FromMilliseconds(250);

        }

        protected void WaitForElement(By by)
        {
            elementWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(by));

        }

        public virtual string getEntityName()
        {

            elementWait.Until(d => d.FindElement(By.Id(EntityNameField.GetAttribute("id"))));
            return EntityNameField.GetAttribute("value");
        }

        public virtual void EnterEntityName(string text)
        {

            elementWait.Until(d => d.FindElement(By.Id(EntityNameField.GetAttribute("id"))));
            EntityNameField.Clear();
            EntityNameField.SendKeys(text);
        }

        public virtual string getFirstName()
        {

            elementWait.Until(d => d.FindElement(By.Id(FirstNameField.GetAttribute("id"))));
            return FirstNameField.GetAttribute("value");
        }

        public virtual void EnterFirstName(string text)
        {

            elementWait.Until(d => d.FindElement(By.Id(FirstNameField.GetAttribute("id"))));
            FirstNameField.Clear();
            FirstNameField.SendKeys(text);
        }

        public virtual string getLastName()
        {

            elementWait.Until(d => d.FindElement(By.Id(LastNameField.GetAttribute("id"))));
            return LastNameField.GetAttribute("value");
        }

        public virtual void EnterLastName(string text)
        {

            elementWait.Until(d => d.FindElement(By.Id(LastNameField.GetAttribute("id"))));
            LastNameField.Clear();
            LastNameField.SendKeys(text);
        }

        public virtual string getEmail()
        {

            elementWait.Until(d => d.FindElement(By.Id(EmailField.GetAttribute("id"))));
            return EmailField.GetAttribute("value");
        }

        public virtual void EnterEmail(string text)
        {

            elementWait.Until(d => d.FindElement(By.Id(EmailField.GetAttribute("id"))));
            EmailField.Clear();
            EmailField.SendKeys(text);
        }

        public virtual string getPhone()
        {

            elementWait.Until(d => d.FindElement(By.Id(PhoneField.GetAttribute("id"))));
            return PhoneField.GetAttribute("value");
        }

        public virtual void EnterPhone(string text)
        {

            elementWait.Until(d => d.FindElement(By.Id(PhoneField.GetAttribute("id"))));
            PhoneField.Clear();
            PhoneField.SendKeys(text);
        }

        public virtual string getPhone2()
        {

            elementWait.Until(d => d.FindElement(By.Id(Phone2Field.GetAttribute("id"))));
            return Phone2Field.GetAttribute("value");
        }

        public virtual void EnterPhone2(string text)
        {

            elementWait.Until(d => d.FindElement(By.Id(Phone2Field.GetAttribute("id"))));
            Phone2Field.Clear();
            Phone2Field.SendKeys(text);
        }

        public virtual string getAddress()
        {

            elementWait.Until(d => d.FindElement(By.Id(Address1.GetAttribute("id"))));
            return Address1.GetAttribute("value");
        }

        public virtual void EnterAddress(string text)
        {

            elementWait.Until(d => d.FindElement(By.Id(Address1.GetAttribute("id"))));
            Address1.Clear();
            Address1.SendKeys(text);
        }

        public virtual string getAddress2()
        {

            elementWait.Until(d => d.FindElement(By.Id(Address2.GetAttribute("id"))));
            return Address2.GetAttribute("value");
        }

        public virtual void EnterAddress2(string text)
        {

            elementWait.Until(d => d.FindElement(By.Id(Address2.GetAttribute("id"))));
            Address2.Clear();
            Address2.SendKeys(text);
        }

        public virtual string getCity()
        {

            elementWait.Until(d => d.FindElement(By.Id(CityField.GetAttribute("id"))));
            return CityField.GetAttribute("value");
        }

        public virtual void EnterCity(string text)
        {

            elementWait.Until(d => d.FindElement(By.Id(CityField.GetAttribute("id"))));
            CityField.Clear();
            CityField.SendKeys(text);
        }

        public virtual string getZipCode()
        {
            elementWait.Until(d => d.FindElement(By.Id(ZipcodeField.GetAttribute("id"))));
            return ZipcodeField.GetAttribute("value");
        }

        public virtual void EnterZipCode(string text)
        {

            elementWait.Until(d => d.FindElement(By.Id(ZipcodeField.GetAttribute("id"))));
            ZipcodeField.Clear();
            ZipcodeField.SendKeys(text);
        }

        public virtual string getCountry()
        {

            elementWait.Until(d => d.FindElement(By.Id(CountryField.GetAttribute("id"))));
            return CountryField.GetAttribute("value");
        }

        public virtual void EnterCountry(string text)
        {

            elementWait.Until(d => d.FindElement(By.Id(CountryField.GetAttribute("id"))));
            CountryField.Clear();
            CountryField.SendKeys(text);
        }


        public virtual string getState()
        {

            elementWait.Until(d => d.FindElement(By.Id(StateSelect.GetAttribute("id"))));
            SelectElement stateSelect = new SelectElement(StateSelect);
            return stateSelect.SelectedOption.Text;
        }


        public bool nameErrorIsDisplayed()
        {

            try
            {

                errorWait.Until(d => NameError.Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;


        }

        public string getNameError()
        {
            if (nameErrorIsDisplayed())
            {
                return NameError.Text;
            }
            else
            {
                return "";
            }
        }

        public bool firstNameErrorIsDisplayed()
        {
            try
            {

                errorWait.Until(d => FNameError.Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public string getFirstNameError()
        {
            if (firstNameErrorIsDisplayed())
            {
                return FNameError.Text;
            }
            else
            {
                return "";
            }
        }

        public bool lastNameErrorIsDisplayed()
        {
            try
            {

                errorWait.Until(d => LNameError.Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public string getLastNameError()
        {
            if (lastNameErrorIsDisplayed())
            {
                return LNameError.Text;
            }
            else
            {
                return "";
            }
        }

        public bool emailErrorIsDisplayed()
        {
            try
            {

                errorWait.Until(d => EmailError.Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public string getEmailError()
        {
            if (EmailError.Text.Length > 0)
            {
                return EmailError.Text;
            }
            else
            {
                return "";
            }
        }

        public bool phoneErrorIsDisplayed()
        {
            try
            {

                errorWait.Until(d => PhoneError.Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public string getPhoneError()
        {
            if (phoneErrorIsDisplayed())
            {
                return PhoneError.Text;
            }
            else
            {
                return "";
            }
        }

        public bool addressErrorIsDisplayed()
        {
            try
            {

                errorWait.Until(d => AddressError.Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public string getAddressError()
        {
            if (addressErrorIsDisplayed())
            {
                return AddressError.Text;
            }

            return "";
        }

        public bool cityErrorIsDisplayed()
        {
            try
            {

                errorWait.Until(d => CityError.Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public string getCityError()
        {
            if (addressErrorIsDisplayed())
            {
                return CityError.Text;
            }

            return "";
        }

        public bool zipErrorIsDisplayed()
        {
            try
            {

                errorWait.Until(d => ZipCodeError.Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public string getZipCodeError()
        {
            if (zipErrorIsDisplayed())
            {
                return ZipCodeError.Text;
            }

            return "";
        }


        public virtual void ClickSubmit()
        {
            WaitForElement(By.XPath("//input[@type='submit']"));
            Submit.Click();
        }


        public bool submitIsHidden()
        {
            try
            {
                WaitForElement(By.XPath("//input[@type='submit']"));
            }
            catch (Exception ex)
            {
                return true;
            }

            return false;
        }


        //public method to check if input is readonly
        public virtual bool isReadOnly(EntityDetailsInputs input)
        {

            IWebElement element = getElement(input);
            elementWait.Until(d => d.FindElement(By.Id(element.GetAttribute("id"))));


            //if element is state select check if it has 'readonly' attribute
            if (input == EntityDetailsInputs.STATE)
            {
                string disabled = StateSelect.GetAttribute("disabled");

                if (disabled == null)
                {
                    return false;
                }

                return disabled == "true";
            }

            //for other inputs. Try to enter text in the input. If throws error then its read only
            string textBefore = element.GetAttribute("value");
            try
            {
                element.Clear();
                element.SendKeys("test");
            }
            catch (Exception ex)
            {
                return true;
            }

            return false;
        }


        /*
         * Takes in a EnttiyDetailsInput enum and returns the corresponding IWebElement
         */
        protected virtual IWebElement getElement(EntityDetailsInputs element)
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
                default:
                    return StateSelect;
            }

        }

        public virtual void EnterForm(string name, string fName, string lName, string email, string Phone, string address,
            string City, string Zipcode)
        {
            EnterEntityName(name);
            EnterFirstName(fName);
            EnterLastName(lName);
            EnterEmail(email);
            EnterPhone(Phone);
            EnterAddress(address);
            EnterCity(City);
            EnterZipCode(Zipcode);


        }


        //overriden in site detail edit page
        public virtual void EnterForm(string name, string agentNum, string fName, string lName,
            string email, string Phone, string address, string City, string Zipcode)
        { }


        //overriden in Add Site page for EasyVend TMS
        public virtual void EnterForm(string name, string agentNum, string externalId, string fName, string lName,
            string email, string Phone, string address, string City, string Zipcode)
        { }


        public bool successBanerIsVisible()
        {
            try
            {
                errorWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("alert-success")));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public string getSuccessBannerText()
        {
            errorWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("alert-success")));
            return SuccessBanner.Text;
        }
    }
}
