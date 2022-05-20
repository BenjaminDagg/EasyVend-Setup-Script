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
    internal class TablePage
    {
        protected IWebDriver driver;
        protected WebDriverWait wait;

        [FindsBy(How = How.Id, Using = "tblUserList")]
        protected IWebElement table;

        [FindsBy(How = How.XPath, Using = "//input[@type='search']")]
        protected IWebElement SearchInput;

        [FindsBy(How = How.Name, Using = "tblUserList_length")]
        protected IWebElement tableLengthSelect;



        //overriden in derived classes to fit the name of the table on the page
        public virtual IWebElement Table
        {
            get { return table; }

        }

        public virtual IWebElement TableLengthSelect { get { return tableLengthSelect; } }

        public TablePage(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }

        //waits for at least 1 record to be in table (no records counts as one)
        public virtual void waitForTable()
        {

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("odd")));
        }

        //waits for table to stop loading after entering a search key by checking the style of processing div to
        //change from display: none to display:block;
        public virtual void waitForFilter()
        {
            wait.Until(driver => driver.FindElement(By.Id("tblUserList_processing")).GetAttribute("style").Contains("none"));
        }

        protected void WaitForElement(By by)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(by));
        }


        //returns headers of the table
        public string getHeader()
        {
            waitForTable();

            string id = Table.GetAttribute("id");
            IWebElement header = Table.FindElement(By.XPath("//*[@id='" + id + "']/thead/tr/th[1]"));
            string text = header.GetAttribute("innerHTML");

            return text;
        }


        //returns number of rows in the table
        public virtual int getRecordCount()
        {
            //wait for table to load
            waitForTable();


            //get all rows in the table
            string id = Table.GetAttribute("id");
            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));

            //get data of the first td in first row. Check if table is empty
            IWebElement firstRow = rows[0].FindElement(By.TagName("td"));
            if (firstRow.GetAttribute("class") == "dataTables_empty")
            {
                return 0;
            }

            return rows.Count();
        }


        //check if any records in the table contain the search term
        public bool recordExists(string search)
        {
            waitForTable();
            search = search.ToLower();

            if (getRecordCount() == 0)
            {
                return false;
            }



            //get all rows in the table
            string id = Table.GetAttribute("id");
            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));

            foreach (IWebElement row in rows)
            {
                IList<IWebElement> cols = row.FindElements(By.TagName("td"));

                foreach (IWebElement col in cols)
                {
                    string content = col.Text;

                    if (content.ToLower().Contains(search))
                    {

                        return true;

                    }
                }


            }

            return false;
        }


        //Types in the search input and filters the table
        public void EnterSearchTerm(string search)
        {
            WaitForElement(By.XPath("//input[@type='search']"));
            
            SearchInput.Clear();
            SearchInput.SendKeys(search);

            waitForFilter();
        }


        public void ClearSearchField()
        {
            WaitForElement(By.XPath("//input[@type='search']"));

            SearchInput.Clear();
            waitForFilter();
        }



        //finds the number of records on the table that contains the search term
        public virtual int matchingRecordCount(string search)
        {
            waitForTable();
            waitForFilter();



            int matchCount = 0;
            search = search.ToLower();

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));

            foreach (IWebElement row in rows)
            {
                IList<IWebElement> cols = row.FindElements(By.TagName("td"));

                string content = "";
                foreach (IWebElement col in cols)
                {
                    try
                    {
                        content += " " + col.Text;
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (content.ToLower().Contains(search))
                {

                    matchCount++;
                }

            }

            return matchCount;
        }


        public virtual void SetTableLength(int length)
        {

            //wait.Until(d => d.FindElement(By.Name(TableLengthSelect.GetAttribute("name"))));
            //wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Name(TableLengthSelect.GetAttribute("name"))));
            SelectElement select = new SelectElement(TableLengthSelect);

            switch (length)
            {
                case 10:
                    select.SelectByIndex(0);
                    break;
                case 25:
                    select.SelectByIndex(1);
                    break;
                case 50:
                    select.SelectByIndex(2);
                    break;
                case 100:
                    select.SelectByIndex(3);
                    break;
                default:
                    select.SelectByIndex(3);
                    break;
            }

            waitForFilter();
        }


        public void ClickHeader(int index)
        {
            waitForTable();

            IWebElement thead = Table.FindElement(By.TagName("thead"));
            IWebElement headerRow = thead.FindElement(By.TagName("tr"));
            IList<IWebElement> headers = headerRow.FindElements(By.TagName("th"));

            IWebElement target = headers[index];


            target.Click();
        }


        public virtual void SortByColAsc(int index)
        {
            waitForTable();

            IWebElement thead = Table.FindElement(By.TagName("thead"));
            IWebElement headerRow = thead.FindElement(By.TagName("tr"));
            IList<IWebElement> headers = headerRow.FindElements(By.TagName("th"));

            IWebElement target = headers[index];


            string sortedBy = target.GetAttribute("class");

            if (sortedBy == "sorting_asc")
            {
                return;
            }

            target.Click();

            waitForFilter();
        }


        public virtual void SortByColDesc(int index)
        {
            waitForTable();

            IWebElement thead = Table.FindElement(By.TagName("thead"));
            IWebElement headerRow = thead.FindElement(By.TagName("tr"));
            IList<IWebElement> headers = headerRow.FindElements(By.TagName("th"));

            IWebElement target = headers[index];

            string sortedBy = target.GetAttribute("class");

            if (sortedBy == "sorting")
            {
                target.Click();
                target.Click();
            }
            else if (sortedBy == "sorting_asc")
            {
                target.Click();
            }


            waitForFilter();
        }


        public List<string> GetValuesForCol(int index)
        {
            waitForTable();


            List<string> values = new List<string>();

            if (getRecordCount() == 0)
            {
                return values;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));

            foreach (IWebElement row in rows)
            {
                IList<IWebElement> cols = row.FindElements(By.TagName("td"));
                values.Add(cols[index].Text);
            }

            return values;
        }


        public int GetMatchesForCol(string search, int index)
        {
            waitForTable();

            int count = getRecordCount();
            if (count == 0)
            {
                return 0;
            }

            int matches = 0;

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));

            foreach (IWebElement row in rows)
            {
                IList<IWebElement> cols = row.FindElements(By.TagName("td"));
                string data;

                try
                {
                    data = cols[index].Text;
                }
                catch (Exception ex)
                {
                    data = "";
                }

                if (data.ToLower().Contains(search.ToLower()))
                {
                    matches++;
                }
            }

            return matches;
        }

    }
}
