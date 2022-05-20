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
    internal class BursterForm
    {
        [FindsBy(How = How.Id, Using = "_hidden.Bursters[0].BursterId")]
        public IWebElement BursterId
        {
            get
            {
                return driver.FindElement(By.Id(@"_hidden.Bursters[" + index + "].BursterId"));
            }
        }


        [FindsBy(How = How.Id, Using = "_hidden.Bursters[0].DisplayGame")]
        public IWebElement GameName
        {
            get
            {
                return driver.FindElement(By.Id(@"_hidden.Bursters[" + index + "].DisplayGame"));
            }
        }


        [FindsBy(How = How.Id, Using = "_hidden.Bursters[0].DrawerNumber")]
        public IWebElement DrawerNum
        {
            get
            {
                return driver.FindElement(By.Id(@"_hidden.Bursters[" + index + "].DrawerNumber"));
            }
        }


        [FindsBy(How = How.Id, Using = "_hidden.Bursters[0].LotteryGameId")]
        public IWebElement GameId
        {
            get
            {
                return driver.FindElement(By.Id(@"_hidden.Bursters[" + index + "].LotteryGameId"));
            }
        }


        [FindsBy(How = How.Id, Using = "_hidden.Bursters[0].PackSize")]
        public IWebElement PackSize
        {
            get
            {
                return driver.FindElement(By.Id(@"_hidden.Bursters[" + index + "].PackSize"));
            }
        }


        [FindsBy(How = How.Id, Using = "_hidden.Bursters[0].Price")]
        public IWebElement Price
        {
            get
            {
                return driver.FindElement(By.Id(@"_hidden.Bursters[" + index + "].Price"));
            }
        }


        [FindsBy(How = How.Id, Using = "_hidden.Bursters[0].Stock")]
        public IWebElement Stock
        {
            get
            {
                return driver.FindElement(By.Id(@"_hidden.Bursters[" + index + "].Stock"));
            }
        }


        [FindsBy(How = How.Id, Using = "_hidden.Bursters[0].IsActive")]
        public IWebElement Active
        {
            get
            {
                return driver.FindElement(By.Id(@"_hidden.Bursters[" + index + "].IsActive"));
            }
        }

        private int index;

        IWebDriver driver;
        WebDriverWait wait;


        public BursterForm(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            this.index = 0;
        }



        public int GetGameId(int index)
        {

            this.index = index;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("_hidden.Bursters[0].LotteryGameId")));
            string val = GameId.GetAttribute("value");

            if (val == "")
            {
                return 0;
            }
            return int.Parse(GameId.GetAttribute("value"));
        }


        public int GetBursterId(int index)
        {
            this.index = index;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("_hidden.Bursters[" + index + "].BursterId")));

            return int.Parse(BursterId.GetAttribute("value"));
        }


        public string GetGameName(int index)
        {
            this.index = index;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("_hidden.Bursters[" + index + "].DisplayGame")));

            return GameName.GetAttribute("value");
        }


        public int GetDrawerNum(int index)
        {
            this.index = index;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("_hidden.Bursters[" + index + "].DrawerNumber")));

            return int.Parse(DrawerNum.GetAttribute("value"));
        }


        public int GetPackSize(int index)
        {
            this.index = index;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("_hidden.Bursters[" + index + "].PackSize")));

            string val = PackSize.GetAttribute("value");

            if (val == "")
            {
                return 0;
            }
            return int.Parse(PackSize.GetAttribute("value"));
        }


        public int GetPrice(int index)
        {
            this.index = index;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("_hidden.Bursters[" + index + "].Price")));

            string val = Price.GetAttribute("value");

            if (val == "")
            {
                return 0;
            }
            return int.Parse(Price.GetAttribute("value"));
        }


        public int GetStock(int index)
        {
            this.index = index;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("_hidden.Bursters[" + index + "].Stock")));

            string val = Stock.GetAttribute("value");

            if (val == "")
            {
                return 0;
            }
            return int.Parse(Stock.GetAttribute("value"));
        }


        public bool GetActive(int index)
        {
            this.index = index;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("_hidden.Bursters[" + index + "].IsActive")));

            string val = Active.GetAttribute("value");

            if (val == "True")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public string GetStatusText(int index)
        {
            this.index = index;

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("_hidden.Bursters[" + index + "].IsActive")));

            string val = Active.GetAttribute("value");

            if (val == "True")
            {
                return "Yes";
            }
            else
            {
                return "No";
            }
        }

    }
}
