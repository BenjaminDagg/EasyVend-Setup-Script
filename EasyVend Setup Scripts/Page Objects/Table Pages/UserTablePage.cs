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
    //stored index of header for better reusability accross different TMS products
    public enum UserTableHeaders : int
    {
        USERNAME = 0,
        FIRSTNAME = 1,
        LASTNAME = 2,
        ROLE = 3,
        LOCKED = 4,
        ACTIVE = 5,
        ACTIONS = 6
    }

    internal class UserTablePage : TablePage
    {
        public AddUserModal AddUserModal { get; set; }
        public EditUserModal EditUserModal { get; set; }
        public ModalPage LockModal { get; set; }
        public ModalPage ActivateModal { get; set; }
        public ModalPage PasswordResetModal { get; set; }

        [FindsBy(How = How.PartialLinkText, Using = "Add")]
        protected IWebElement AddUserButton;

        public UserTablePage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            AddUserModal = new AddUserModal(driver);
            EditUserModal = new EditUserModal(driver);
            LockModal = new ModalPage(driver);
            ActivateModal = new ModalPage(driver);
            PasswordResetModal = new ModalPage(driver);
        }

        public void ClickAddUser()
        {
            AddUserButton.Click();

        }


        public void AddUserSuccess(string email, string fName, string lName, string phone, int role)
        {
            this.AddUserModal.EnterForm(email, fName, lName, phone, role);
            this.AddUserModal.ClickSubmit();
            this.AddUserModal.waitForModalClose();

            waitForFilter();
        }


        public void AddUserFail(string email, string fName, string lName, string phone, int role)
        {
            this.AddUserModal.EnterForm(email, fName, lName, phone, role);
            this.AddUserModal.ClickSubmit();

        }


        public void EditUserSuccess(string email, string fName, string lName, string phone, int role)
        {
            this.EditUserModal.EnterForm(email, fName, lName, phone, role);
            this.EditUserModal.ClickSubmit();
            this.EditUserModal.waitForModalClose();

            waitForFilter();
        }


        public void EditUserFail(string email, string fName, string lName, string phone, int role)
        {
            this.EditUserModal.EnterForm(email, fName, lName, phone, role);
            this.EditUserModal.ClickSubmit();
        }


        public UserTableRecord GetUserByIndex(int index)
        {
            waitForTable();

            if (getRecordCount() == 0)
            {
                return null;
            }

            if (index < 0 || index > getRecordCount())
            {
                return null;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            UserTableRecord user = new UserTableRecord();
            user.Username = cols[(int)UserTableHeaders.USERNAME].Text;
            user.FirstName = cols[(int)UserTableHeaders.FIRSTNAME].Text;
            user.LastName = cols[(int)UserTableHeaders.LASTNAME].Text;
            user.Role = cols[(int)UserTableHeaders.ROLE].Text;

            user.display();

            return user;
        }


        public void ClickEditUserButton(int index)
        {
            waitForTable();

            if (getRecordCount() == 0)
            {
                return;
            }

            if (index < 0 || index > getRecordCount())
            {
                return;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));
            IWebElement editUser = cols[cols.Count - 1].FindElement(By.ClassName("editUser"));

            editUser.Click();

        }


        public void ClickUsername(int index)
        {
            waitForTable();

            if (getRecordCount() == 0)
            {
                return;
            }

            if (index < 0 || index > getRecordCount())
            {
                return;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));
            IWebElement editUser = cols[(int)UserTableHeaders.USERNAME].FindElement(By.ClassName("editUser"));

            editUser.Click();
        }


        public bool EditUserButtonIsVisible(int index)
        {
            waitForTable();

            if (getRecordCount() == 0)
            {
                return false;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            try
            {
                IWebElement editUser = cols[cols.Count - 1].FindElement(By.ClassName("editUser"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        //returns true if Locked column is 'Yes', or false if 'No'
        public bool UserIsLocked(int index)
        {
            waitForTable();
            waitForFilter();


            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));


            string value = cols[cols.Count - 3].Text;

            if (value == "No")
            {
                return false;
            }
            else
            {
                return true;
            }


        }


        public void ClickLockButton(int index)
        {
            waitForTable();

            if (getRecordCount() == 0)
            {
                return;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));
            IWebElement lockBtn = cols[cols.Count - 1].FindElement(By.ClassName("unlockUser"));

            lockBtn.Click();
        }


        public bool LockButtonIsVisible(int index)
        {
            waitForTable();

            if (getRecordCount() == 0)
            {
                return false;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            try
            {
                IWebElement lockBtn = cols[cols.Count - 1].FindElement(By.ClassName("unlockUser"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public void ToggleLockUser(int index)
        {
            ClickLockButton(index);
            LockModal.ClickSubmit();
            LockModal.waitForModalClose();
            waitForFilter();
        }




        //returns true if user is activated or false if user is deactivated
        public bool UserIsActive(int index)
        {
            waitForTable();
            waitForFilter();

            if (getRecordCount() == 0)
            {
                return false;
            }


            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));


            string value = cols[cols.Count - 2].Text;

            if (value == "No")
            {
                return false;
            }
            else
            {
                return true;
            }


        }


        public bool ActiveateButtonIsVisible(int index)
        {
            waitForTable();

            if (getRecordCount() == 0)
            {
                return false;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            try
            {
                IWebElement activeBtn = cols[cols.Count - 1].FindElement(By.XPath(".//a[3]"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public void ClickActiveButton(int index)
        {
            waitForTable();

            if (getRecordCount() == 0)
            {
                return;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));
            IWebElement activeBtn = cols[cols.Count - 1].FindElement(By.XPath(".//a[3]"));

            activeBtn.Click();
        }


        public void ToggleActivateUser(int index)
        {
            ClickActiveButton(index);
            ActivateModal.ClickSubmit();
            ActivateModal.waitForModalClose();
            waitForFilter();
        }


        public bool ResetPasswordButtonIsVisible(int index)
        {
            waitForTable();



            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            try
            {
                IWebElement passwordButton = cols[cols.Count - 1].FindElement(By.XPath(".//a[4]"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public void ClickResetPassword(int index)
        {
            waitForTable();

            if (getRecordCount() == 0)
            {
                return;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));
            IWebElement passwordButton = cols[cols.Count - 1].FindElement(By.XPath(".//a[4]"));

            passwordButton.Click();
        }


        public void ResetPassword(int index)
        {
            ClickResetPassword(index);
            PasswordResetModal.CloseConfirmation();
            waitForFilter();
        }


        public int indexOfUser(string username)
        {
            waitForTable();

            if (getRecordCount() == 0)
            {
                return -1;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));

            int index = 0;
            foreach (IWebElement row in rows)
            {
                IList<IWebElement> cols = row.FindElements(By.TagName("td"));
                string name = cols[(int)UserTableHeaders.USERNAME].Text;

                if (name.ToLower() == username.ToLower())
                {

                    return index;
                }

                index++;
            }

            return -1;
        }

    }
}
