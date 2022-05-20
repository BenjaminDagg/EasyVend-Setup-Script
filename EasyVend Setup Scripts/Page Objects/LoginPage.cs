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

//https://www.guru99.com/handling-dynamic-selenium-webdriver.html

namespace EasyVend_Setup_Scripts
{
    internal class LoginPage
    {
        IWebDriver driver;

        WebDriverWait wait;

        public static string url = ConfigurationManager.AppSettings["URL"] + "Identity/Account/Login";

        //Page web elements
        [FindsBy(How = How.Id, Using = "Input_Email")]
        private IWebElement Username { get; set; }

        [FindsBy(How = How.Id, Using = "Input_Password")]
        private IWebElement Password { get; set; }

        [FindsBy(How = How.Id, Using = "btnLogin")]
        private IWebElement LoginButton { get; set; }

        [FindsBy(How = How.Id, Using = "resetPwd")]
        private IWebElement ResetPassword { get; set; }

        //have to use xpath because a span is dynamically added when an invalid email is entered
        //in dmb select email error by id instead
        [FindsBy(How = How.XPath, Using = "//span[@data-valmsg-for='Input.Email']")]
        private IWebElement EmailError { get; set; }

        [FindsBy(How = How.Id, Using = "Input_Password-error")]
        private IWebElement PasswordError { get; set; }

        [FindsBy(How = How.XPath, Using = "//*[@class='main-error-msg']/span")]
        private IWebElement ValidationError { get; set; }


        public static bool IsLoggedIn
        {
            get
            {
                try
                {
                    DriverFactory.Driver.FindElement(By.Id("userNameh1"));
                }
                catch (Exception ex)
                {
                    return false;
                }

                return true;
            }
        }



        //Constructor
        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

        }


        private bool ErrorIsVisible(IWebElement element)
        {
            try
            {
                wait.Until(d => d.FindElement(By.Id(element.GetAttribute("id"))));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        //enter text in username input
        public void EnterUsername(string username)
        {
            Username.Clear();
            Username.SendKeys(username);
        }

        //enter text in password input
        public void EnterPassword(string password)
        {
            Password.Clear();
            Password.SendKeys(password);
        }

        //clear text from username input
        public void ClearUsername()
        {
            Username.Clear();
        }

        //clear text from password input
        public void ClearPassword()
        {
            Password.Clear();
        }

        public void ClickLogin()
        {
            LoginButton.Click();
        }

        //Enters username and password and click login
        public void PerformLogin(string username, string password)
        {
            EnterUsername(username);
            EnterPassword(password);
            ClickLogin();
        }

        //Checks if the email error message is currently displayed
        public bool emailErrorIsDisplayed()
        {
            try
            {
                wait.Until(d => d.FindElement(By.XPath("//span[@data-valmsg-for='Input.Email']")).Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;

        }

        public string getEmailErrorText()
        {
            wait.Until(d => d.FindElement(By.XPath("//span[@data-valmsg-for='Input.Email']")).Text.Length > 0);
            return EmailError.Text;
        }

        public bool passwordErrorIsDisplayed()
        {
            return ErrorIsVisible(PasswordError);
        }

        public string getPasswordErrorText()
        {
            wait.Until(d => d.FindElement(By.Id(PasswordError.GetAttribute("id"))).Text.Length > 0);
            return PasswordError.Text;
        }

        public bool validationErrorIsDisplayed()
        {
            try
            {
                wait.Until(d => d.FindElement(By.XPath("//*[@class='main-error-msg']/span")).Text.Length > 0);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public string getValidationErrorText()
        {
            return ValidationError.Text;
        }


        public void ClickResetPassword()
        {
            ResetPassword.Click();
        }



    }
}
