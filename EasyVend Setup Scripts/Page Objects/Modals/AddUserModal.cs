
//test waits - add user modal - WaitForModal(), EntityDetailsPage - success banner

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
    internal enum UserModalFields
    {
        EMAIL,
        FName,
        LName,
        PHONE,
        ROLE
    }

    public enum UserRoleSelect : int
    {
        NONE = 0,
        ADMIN = 1,
        REPORT = 2
    }

    internal class AddUserModal : ModalPage
    {


        [FindsBy(How = How.Id, Using = "Email")]
        protected IWebElement ModalEmail;

        [FindsBy(How = How.Id, Using = "FirstName")]
        protected IWebElement ModalFirstName;

        [FindsBy(How = How.Id, Using = "LastName")]
        protected IWebElement ModalLastName;

        [FindsBy(How = How.Id, Using = "PhoneNumber")]
        protected IWebElement ModalPhone;

        [FindsBy(How = How.Id, Using = "UserRole")]
        public virtual IWebElement RoleSelect { get; set; }

        [FindsBy(How = How.Id, Using = "btnSearchUser")]
        public IWebElement SearchUserButton { get; set; }

        [FindsBy(How = How.Id, Using = "ExistingUser")]
        public IWebElement ExistingUserCheck { get; set; }

        //========== modal errors ==============
        [FindsBy(How = How.Id, Using = "emailValidator")]
        protected IWebElement EmailError;

        [FindsBy(How = How.Id, Using = "firstNameValidator")]
        protected IWebElement FirstNameError;

        [FindsBy(How = How.Id, Using = "lastNameValidator")]
        protected IWebElement LastNameError;

        [FindsBy(How = How.Id, Using = "phoneNumberValidator")]
        protected IWebElement PhoneError;

        [FindsBy(How = How.Id, Using = "roleValidator")]
        protected IWebElement RoleError;

        [FindsBy(How = How.ClassName, Using = "alert-danger")]
        protected IWebElement TakenEmailError;

        [FindsBy(How = How.Id, Using = "invalidEmail")]
        protected IWebElement ExistingUserError;

        public AddUserModal(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }


        /*
         * Waits until modal div changes from "display: none" to "display: block" after pressing
         * Add User button
         */
        public override void WaitForModal()
        {
            wait.Until(d => d.FindElement(By.Id(ModalPhone.GetAttribute("id"))));

        }


        //waits for error message to appear
        public void WaitForError(By by)
        {
            wait.Until(d => d.FindElement(by).Text.Length > 0);
        }


        //waits for searh user button to appear after clicking checkbox
        public void WaitForSearchuser()
        {
            wait.Until(d => d.FindElement(By.Id("btnSearchUser")).GetAttribute("style") != "display: none;");
        }


        public void EnterEmail(string email)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("Email")));
            ModalEmail.Clear();
            ModalEmail.SendKeys(email);
        }


        public void EnterFirstName(string fName)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("FirstName")));
            ModalFirstName.Clear();
            ModalFirstName.SendKeys(fName);
        }


        public void EnterLastName(string lName)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("LastName")));
            ModalLastName.Clear();
            ModalLastName.SendKeys(lName);
        }


        public void EnterPhone(string phone)
        {
            //WaitForModal();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("PhoneNumber")));
            ModalPhone.Clear();
            ModalPhone.SendKeys(phone);
        }


        //select role by index: 0 = blank, 1 = admin, 2 = report
        public virtual void SelectRole(int index)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("UserRole")));

            if (index < 0 || index > 2)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            SelectElement role = new SelectElement(RoleSelect);
            role.SelectByIndex(index);
        }


        public string GetEmail()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("Email")));

            return ModalEmail.GetAttribute("value");
        }


        public string GetFirstName()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("FirstName")));
            return ModalFirstName.GetAttribute("value");
        }

        public string GetLastName()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("LastName")));
            return ModalLastName.GetAttribute("value");
        }

        public string GetPhone()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("PhoneNumber")));
            return ModalPhone.GetAttribute("value");
        }


        public virtual string GetRole()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("UserRole")));

            SelectElement roleSelect = new SelectElement(RoleSelect);
            IWebElement option = roleSelect.SelectedOption;

            return option.Text;
        }


        public void ClickExistingUser()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("ExistingUser")));
            ExistingUserCheck.Click();
        }


        public void ClickSearchUser()
        {
            wait.Until(d => d.FindElement(By.Id(SearchUserButton.GetAttribute("id"))).GetAttribute("style") != "display: none;");
            SearchUserButton.Click();
        }


        public bool ExistingUserErrorIsDisplayed()
        {
            try
            {
                wait.Until(d => d.FindElement(By.Id("invalidEmail")).Text.Length > 0 ||
                d.FindElement(By.Id("emailValidator")).Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public virtual bool EmailErrorIsDisplayed()
        {
            try
            {
                WaitForError(By.Id("emailValidator"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public virtual bool PhoneErrorIsDisplayed()
        {

            try
            {
                WaitForError(By.Id("phoneNumberValidator"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public virtual bool fNameErrorIsDisplayed()
        {

            try
            {
                WaitForError(By.Id("firstNameValidator"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public virtual bool lNameErrorIsDisplayed()
        {

            try
            {
                WaitForError(By.Id("lastNameValidator"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public virtual bool RoleErrorIsDisplayed()
        {

            try
            {
                WaitForError(By.Id("roleValidator"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public bool TakenEmailErrorIsDisplayed()
        {//*[@id='entityUserFormPartial']/div[1]
            try
            {
                WaitForError(By.ClassName("alert-danger"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }



        public virtual void EnterFormSuccess(string email, string fName, string lName, string phone, int role)
        {
            WaitForModal();

            EnterEmail(email);
            EnterFirstName(fName);
            EnterLastName(lName);
            EnterPhone(phone);
            SelectRole(role);

            ClickSubmit();

            waitForModalClose();
        }

        public virtual void EnterFormFail(string email, string fName, string lName, string phone, int role)
        {
            WaitForModal();

            EnterEmail(email);
            EnterFirstName(fName);
            EnterLastName(lName);
            EnterPhone(phone);
            SelectRole(role);

            ClickSubmit();
        }


        public virtual void EnterForm(string email, string fName, string lName, string phone, int role)
        {
            WaitForModal();

            EnterEmail(email);
            EnterFirstName(fName);
            EnterLastName(lName);
            EnterPhone(phone);
            SelectRole(role);


        }


        public void EnterExistingUser(string email)
        {
            WaitForModal();

            EnterEmail(email);
            ClickExistingUser();
            ClickSearchUser();

            wait.Until(d => ExistingUserError.Text.Length > 0 || ModalFirstName.GetAttribute("value").Length > 0);
        }


        protected IWebElement GetField(UserModalFields input)
        {
            switch (input)
            {
                case UserModalFields.EMAIL:
                    return ModalEmail;
                case UserModalFields.FName:
                    return ModalFirstName;
                case UserModalFields.LName:
                    return ModalLastName;
                case UserModalFields.PHONE:
                    return ModalPhone;
                case UserModalFields.ROLE:
                    return RoleSelect;
                default:
                    return RoleSelect;
            }
        }


        public bool IsReadOnly(UserModalFields input)
        {
            WaitForModal();

            IWebElement element = GetField(input);
            wait.Until(d => element.GetAttribute("id"));

            //if element is state select check if it has 'readonly' attribute
            if (input == UserModalFields.ROLE)
            {
                string disabled = RoleSelect.GetAttribute("disabled");

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
    }
}
