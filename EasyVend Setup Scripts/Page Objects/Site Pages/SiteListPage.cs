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
    internal class SiteListPage : TablePage
    {

        //override table property to use this pages table id
        [FindsBy(How = How.Id, Using = "tblEntityList")]
        private IWebElement table;

        [FindsBy(How = How.Name, Using = "tblEntityList_length")]
        private IWebElement tableLengthSelect;

        [FindsBy(How = How.ClassName, Using = "add-bt")]
        private IWebElement AddSiteButton;
        public override IWebElement Table { get { return table; } }
        public override IWebElement TableLengthSelect { get { return tableLengthSelect; } }

        public SiteListPage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }


        //override wait for filter method to use this page's id for the filter div
        public override void waitForFilter()
        {
            wait.Until(driver => driver.FindElement(By.Id("tblEntityList_processing")).GetAttribute("style").Contains("none"));
        }




        public void ClickAddSite()
        {
            waitForTable();
            AddSiteButton.Click();
        }


        public bool AddSiteIsVisible()
        {

            try
            {
                wait.Until(d => d.FindElement(By.ClassName("add-bt")));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public void ClickSiteByIndex(int index)
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
            IWebElement siteBtn = cols[0].FindElement(By.ClassName("businessName"));

            siteBtn.Click();
        }


        public SiteTableRecord GetSiteByIndex(int index)
        {
            Console.WriteLine("index: {0}", index);
            waitForTable();

            int count = getRecordCount();

            if (count == 0)
            {
                return null;
            }

            if (index < 0 || index >= count)
            {
                return null;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            SiteTableRecord site = new SiteTableRecord();
            site.SiteName = cols[0].Text;
            string link = cols[cols.Count - 1].FindElement(By.TagName("a")).GetAttribute("href");
            site.Id = parseIdFromLink(link);
            site.Lottery = cols[2].Text;
            site.AgentNumber = cols[3].Text;
            site.DeviceCount = int.Parse(cols[cols.Count - 4].Text);
            site.Phone = cols[5].Text;

            return site;
        }


        public override void SortByColAsc(int index)
        {
            waitForTable();

            IWebElement thead = Table.FindElement(By.TagName("thead"));
            IWebElement headerRow = thead.FindElement(By.TagName("tr"));
            IList<IWebElement> headers = headerRow.FindElements(By.TagName("th"));

            IWebElement target = headers[index];


            string sortedBy = target.GetAttribute("class");

            if (sortedBy.Contains("sorting_asc"))
            {
                return;
            }

            target.Click();

            waitForFilter();
        }


        public override void SortByColDesc(int index)
        {
            waitForTable();

            IWebElement thead = Table.FindElement(By.TagName("thead"));
            IWebElement headerRow = thead.FindElement(By.TagName("tr"));
            IList<IWebElement> headers = headerRow.FindElements(By.TagName("th"));

            IWebElement target = headers[index];

            string sortedBy = target.GetAttribute("class");
            Console.WriteLine(sortedBy);

            if (sortedBy == "column-filter sorting")
            {
                target.Click();
                target.Click();
            }
            else if (sortedBy == "column-filter sorting_asc")
            {
                target.Click();
            }


            waitForFilter();
        }


        public int getSiteIdByIndex(int index)
        {

            waitForTable();

            int count = getRecordCount();

            if (count == 0)
            {
                return -1;
            }

            if (index < 0 || index >= count)
            {
                return -1;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            //extract the site id from the link
            string link = cols[cols.Count - 1].FindElement(By.ClassName("editSite")).GetAttribute("href");
            int id = parseIdFromLink(link);

            return id;

        }


        public int getDeviceCountByIndex(int index)
        {
            waitForTable();

            int count = getRecordCount();

            if (count == 0)
            {
                return -1;
            }

            if (index < 0 || index >= count)
            {
                return -1;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            int deviceCount = int.Parse(cols[4].Text);

            return deviceCount;

        }


        public bool EditSiteButtonIsVisible(int index)
        {
            waitForTable();

            int count = getRecordCount();

            if (count == 0)
            {
                return false;
            }

            if (index < 0 || index > count)
            {
                return false;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            try
            {
                IWebElement siteButton = cols[cols.Count - 1].FindElement(By.ClassName("editSite"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public void ClickEditSiteButton(int index)
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
            IWebElement editSite = cols[cols.Count - 1].FindElement(By.ClassName("editSite"));

            editSite.Click();

        }


        public void EnterColumnFilter(string text, int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            waitForTable();

            if (getRecordCount() == 0)
            {
                return;
            }

            try
            {
                IWebElement filter = Table.FindElement(By.XPath(".//thead/tr[2]/th[" + (index + 1) + "]/input"));
                filter.SendKeys(text);

                waitForFilter();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        public void ClearColumnFilter(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            waitForTable();

            if (getRecordCount() == 0)
            {
                return;
            }

            try
            {
                IWebElement filter = Table.FindElement(By.XPath(".//thead/tr[2]/th[" + (index + 1) + "]/input"));
                filter.Clear();

                waitForFilter();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        //takes in a href from the edit site button and returns the site id
        private int parseIdFromLink(string link)
        {

            string rawId = Regex.Match(link, @"\d+").Value;
            int Id = int.Parse(rawId);

            return Id;
        }


        public int indexOfSite(string siteName)
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
                string name = cols[0].Text;

                if (name.ToLower() == siteName.ToLower())
                {

                    return index;
                }

                index++;
            }

            return -1;
        }

    }
}
