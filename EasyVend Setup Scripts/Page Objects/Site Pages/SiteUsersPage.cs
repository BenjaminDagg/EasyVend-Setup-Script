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
    internal class SiteUsersPage : UserTablePage
    {


        public SiteUsersPage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }


        public void AddExistingUser(string email)
        {
            this.AddUserModal.EnterExistingUser(email);

            if (!AddUserModal.ExistingUserErrorIsDisplayed())
            {
                try
                {
                    AddUserModal.ClickSubmit();
                    AddUserModal.waitForModalClose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }


        }


        public void EnterExistingUser(string email)
        {
            this.AddUserModal.EnterExistingUser(email);
        }
    }
}
