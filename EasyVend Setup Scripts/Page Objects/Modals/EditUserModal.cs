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

/*
 * TODO
 * - change name of waitForAddUserModal
 * - update EderTableRecord with correct fields /role
 */

namespace EasyVend_Setup_Scripts
{
    internal class EditUserModal : AddUserModal
    {

        [FindsBy(How = How.Id, Using = "UserRole_RoleId")]
        private IWebElement roleSelect;

        //========== modal errors ==============
        [FindsBy(How = How.Id, Using = "emailValidator")]
        protected IWebElement EmailError;

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for = 'FirstName']")]
        protected IWebElement FirstNameError;

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for = 'LastName']")]
        protected IWebElement LastNameError;

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for = 'PhoneNumber']")]
        protected IWebElement PhoneError;

        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for = 'UserRole.RoleId']")]
        protected IWebElement RoleError;

        [FindsBy(How = How.XPath, Using = "//*[@id='entityUserFormPartial']/div[1]")]
        protected IWebElement TakenEmailError;



        public override IWebElement RoleSelect { get { return roleSelect; } }

        public EditUserModal(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }


        public override void SelectRole(int index)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("UserRole_RoleId")));

            if (index < 0 || index > 2)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            SelectElement role = new SelectElement(RoleSelect);
            role.SelectByIndex(index);
        }


        public override string GetRole()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("UserRole_RoleId")));

            SelectElement roleSelect = new SelectElement(RoleSelect);
            IWebElement option = roleSelect.SelectedOption;

            return option.Text;
        }


        public bool UserMatches(UserTableRecord user)
        {
            WaitForModal();

            string email = GetEmail();
            string fName = GetFirstName();
            string lName = GetLastName();
            string phone = GetPhone();
            string role = GetRole();

            return (user.Username == email &&
                user.FirstName == fName &&
                user.LastName == lName &&
                user.Role == role);
        }

        public override void EnterFormSuccess(string email, string fName, string lName, string phone, int role)
        {
            WaitForModal();


            EnterFirstName(fName);
            EnterLastName(lName);
            EnterPhone(phone);
            SelectRole(role);

            ClickSubmit();

            waitForModalClose();
        }


        public override void EnterFormFail(string email, string fName, string lName, string phone, int role)
        {
            WaitForModal();


            EnterFirstName(fName);
            EnterLastName(lName);
            EnterPhone(phone);
            SelectRole(role);

            ClickSubmit();
        }


        public override void EnterForm(string email, string fName, string lName, string phone, int role)
        {
            WaitForModal();

            EnterFirstName(fName);
            EnterLastName(lName);
            EnterPhone(phone);
            SelectRole(role);

        }


        public bool EmailIsReadyOnly()
        {
            WaitForModal();

            //try to clear field and enter text. If throws bad element state exception then its read only
            try
            {
                EnterEmail("test");
            }
            catch (Exception ex)
            {
                return true;
            }

            return false;
        }


        public override bool fNameErrorIsDisplayed()
        {

            try
            {
                WaitForError(By.XPath("//span[@data-valmsg-for='FirstName']"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public override bool lNameErrorIsDisplayed()
        {

            try
            {
                WaitForError(By.XPath("//span[@data-valmsg-for = 'LastName']"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public override bool PhoneErrorIsDisplayed()
        {

            try
            {
                WaitForError(By.XPath("//span[@data-valmsg-for = 'PhoneNumber']"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public override bool RoleErrorIsDisplayed()
        {

            try
            {
                WaitForError(By.XPath("//span[@data-valmsg-for = 'UserRole.RoleId']"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public UserTableRecord GetUser()
        {
            WaitForModal();

            UserTableRecord user = new UserTableRecord();
            user.FirstName = GetFirstName();
            user.LastName = GetLastName();
            user.Username = GetEmail();
            user.Phone = GetPhone().Replace("-", "");
            user.Role = GetRole();

            return user;
        }
    }
}
