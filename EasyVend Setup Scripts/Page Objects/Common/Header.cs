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
    internal class Header
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [FindsBy(How = How.ClassName, Using = "logo")]
        private IWebElement SiteLogo;

        [FindsBy(How = How.Id, Using = "TMSForm")]
        private IWebElement UserDropdown;

        [FindsBy(How = How.Id, Using = "btnChangepassword")]
        private IWebElement ChangePasswordButton;

        [FindsBy(How = How.LinkText, Using = "Logout")]
        private IWebElement LogoutButton;

        public ModalPage LogoutModal { get; set; }

        public bool UserDropDownIsOpen
        {
            get
            {
                return UserDropdown.GetAttribute("class") == "form-inline show";
            }
        }

        public Header(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            LogoutModal = new ModalPage(driver);
        }


        //waits for dropdown menu to open by checking class
        private void waitForDropdownOpen()
        {
            wait.Until(d => UserDropdown.GetAttribute("class") == "form-inline show");
        }


        //waits for dropdown menu to close by checking class
        private void waitForDropdownClose()
        {
            wait.Until(d => UserDropdown.GetAttribute("class") == "form-inline");
        }


        //click top left logo to return to homepage
        public void ClickLogo()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("logo")));
            SiteLogo.Click();
        }


        //open dropdown menu
        public void ClickUserDropdown()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("TMSForm")));
            UserDropdown.Click();
        }


        //click change password option in dropdown
        public void ClickChangePassword()
        {
            waitForDropdownOpen();
            ChangePasswordButton.Click();
        }


        //click logout option in dropdown
        public void ClickLogout()
        {
            waitForDropdownOpen();
            LogoutButton.Click();
        }


        //cancel logout modal
        public void ClickLogoutCancel()
        {
            LogoutModal.ClickCancel();
        }


        //confirm logout modal
        public void ClickLogoutConfirm()
        {
            LogoutModal.ClickSubmit();
        }


        //performs all steps to logout 
        public void Logout()
        {
            ClickUserDropdown();
            ClickLogout();
            ClickLogoutConfirm();
        }
    }
}
