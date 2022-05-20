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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class TicketSalesReportPage : ReportPage
    {

        [FindsBy(How = How.XPath, Using = "//input[@type='checkbox']")]
        private IWebElement SelectAllCheckbox;

        [FindsBy(How = How.XPath, Using = "(//input)[3]")]
        public override IWebElement SiteSelect
        {
            get
            {
                return driver.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]"));
            }

        }

        protected override int SiteInputIndex { get; set; }
        protected override int PeriodInputIndex { get; set; }

        public bool SelectAllIsEnabled
        {
            get
            {
                wait.Until(d => d.FindElement(By.XPath("//input[@type='checkbox']")));

                return SelectAllCheckbox.Selected;
            }
        }


        public TicketSalesReportPage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            SiteInputIndex = 3;
            PeriodInputIndex = 4;
        }



        protected override IWebElement GetDropdDownList()
        {
            //get parent div of ul elements
            IWebElement dropdownParent = wait.Until(d => d.FindElement(By.XPath("//ul[contains(@class,'e-list-parent')]/parent::div")));

            //try to find selected list marked by class 'e-reorder'. If found then get the 2nd <ul> element.
            //if not found then get the first <ul> element
            try
            {
                IWebElement selectedList = driver.FindElement(By.ClassName("e-reorder"));
                IWebElement list = dropdownParent.FindElement(By.XPath(".//ul[2]"));

                return list;
            }
            //selected list not found so select the first <ul>
            catch (Exception e)
            {
                IWebElement list = dropdownParent.FindElement(By.XPath(".//ul[1]"));
                return list;
            }
        }



        private IWebElement GetSelectedList()
        {
            try
            {
                IWebElement list = wait.Until(d => d.FindElement(By.ClassName("e-reorder")));
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        //clicks the 'Select All' checkbox
        public void ClickSelectAllSites()
        {
            wait.Until(d => d.FindElement(By.XPath("//input[@type='checkbox']")));
            SelectAllCheckbox.Click();
        }


        //selects 'Select All' sites checkbox
        public void EnableSelectAllSites()
        {
            if (SelectAllIsEnabled)
            {
                return;
            }

            ClickSelectAllSites();
        }



        //deselects 'Select All' sites checkbox
        public void DisableSelectAllSites()
        {
            if (!SelectAllIsEnabled)
            {
                return;
            }

            ClickSelectAllSites();
        }




        //select a site from the site dropdown by index in the list
        public override void SelectSiteByIndex(int index)
        {
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]/../..")).FindElement(By.ClassName("e-input-group-icon")));
            dropdownButton.Click();
            waitForDropdownOpen();

            //gets all values in dropdown
            IWebElement list = GetDropdDownList();
            if (list == null)
            {
                Console.WriteLine("null");
                dropdownButton.Click();
                waitForDropdownClose();
                return;
            }

            IList<IWebElement> options = list.FindElements(By.TagName("li"));
            Console.WriteLine(options.Count);
            //index out of range
            if (index > options.Count || index < 0 || options.Count == 0)
            {
                Console.WriteLine("out of index");
                dropdownButton.Click();
                waitForDropdownClose();
                return;

            }

            try
            {
                options[index].Click();
                dropdownButton.Click();

            }
            //element not interactable exception
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                dropdownButton.Click();
            }

            waitForDropdownClose();

        }



        //selects multiple options in the Site dropdown
        public void SelectMultipleSitesByIndex(params int[] index)
        {

            wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]/../..")).FindElement(By.ClassName("e-input-group-icon")));
            dropdownButton.Click();
            waitForDropdownOpen();

            //gets all values in dropdown
            IWebElement list = GetDropdDownList();
            if (list == null)
            {
                Console.WriteLine("null");
                dropdownButton.Click();
                waitForDropdownClose();
                return;
            }

            IList<IWebElement> options = list.FindElements(By.TagName("li"));
            Console.WriteLine("count: {0}" + options.Count);
            foreach (int i in index)
            {
                //index out of range
                if (i > options.Count || i < 0 || options.Count == 0)
                {
                    continue;

                }

                try
                {
                    options[i].Click();

                }
                //element not interactable exception
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    continue;
                }
            }

            dropdownButton.Click();
            waitForDropdownClose();
        }



        //select a site in the site dropdown by site name
        public virtual void SelectSiteByValue(string siteName)
        {
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]/../..")).FindElement(By.ClassName("e-input-group-icon")));
            dropdownButton.Click();
            waitForDropdownOpen();

            //gets all values in dropdown
            IWebElement list = GetDropdDownList();
            if (list == null)
            {
                dropdownButton.Click();
                waitForDropdownClose();
                return;
            }

            IList<IWebElement> options = list.FindElements(By.TagName("li"));

            for (int i = 0; i < options.Count; i++)
            {
                string site = options[i].Text;
                if (site.ToLower().Contains(siteName.ToLower()))
                {
                    options[i].Click();
                    dropdownButton.Click();
                    waitForDropdownClose();
                    return;
                }
            }

            //item not found in list close dropdown manually
            if (DropDownIsOpen())
            {
                dropdownButton.Click();
                waitForDropdownClose();
            }
        }



        public void SelectMultipleSitesByValue(params string[] siteNames)
        {
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]/../../span[3]")));
            dropdownButton.Click();
            waitForDropdownOpen();

            //gets all values in dropdown
            IWebElement list = GetDropdDownList();
            if (list == null)
            {
                dropdownButton.Click();
                waitForDropdownClose();
                return;
            }

            IList<IWebElement> options = list.FindElements(By.TagName("li"));

            for (int i = 0; i < options.Count; i++)
            {
                string site = options[i].Text;

                for (int j = 0; j < siteNames.Length; j++)
                {
                    string siteName = siteNames[j];

                    if (site.ToLower().Contains(siteName.ToLower()))
                    {
                        options[i].Click();
                    }
                }

            }

            dropdownButton.Click();
            waitForDropdownClose();
        }



        //returns the currently selected option in the site dropdown  or null if nothing is selcted
        public override string GetSelectedSite()
        {
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]/../..")).FindElement(By.ClassName("e-input-group-icon")));
            dropdownButton.Click();
            waitForDropdownOpen();

            //try to get 'e-reorder' <ul>. The list contains only options that are checked

            IWebElement list = GetSelectedList();
            if (list == null)
            {
                dropdownButton.Click();
                waitForDropdownClose();
                return null;
            }

            IList<IWebElement> options = list.FindElements(By.TagName("li"));
            string siteName;
            try
            {
                siteName = options[0].Text;

            }
            catch (Exception ex)
            {
                siteName = null;
            }

            dropdownButton.Click();
            waitForDropdownClose();
            return siteName;

        }



        public List<string> GetAllSelectedSites()
        {
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]/../..")).FindElement(By.ClassName("e-input-group-icon")));
            dropdownButton.Click();
            waitForDropdownOpen();

            //try to get 'e-reorder' <ul>. The list contains only options that are checked

            IWebElement list = GetSelectedList();
            List<string> result = new List<string>();

            if (list == null)
            {
                dropdownButton.Click();
                waitForDropdownClose();
                return result;
            }

            IList<IWebElement> options = list.FindElements(By.TagName("li"));

            foreach (IWebElement element in options)
            {
                result.Add(element.Text);
            }

            dropdownButton.Click();
            waitForDropdownClose();

            return result;
        }


        //returns true if the searchSite is found as an option in the site dropdown
        public override bool SiteOptionExists(string siteName)
        {
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]/../..")).FindElement(By.ClassName("e-input-group-icon")));
            dropdownButton.Click();
            waitForDropdownOpen();

            //gets all values in dropdown
            IWebElement list = GetDropdDownList();
            if (list == null)
            {
                dropdownButton.Click();
                waitForDropdownClose();
                return false;
            }

            IList<IWebElement> options = list.FindElements(By.TagName("li"));

            for (int i = 0; i < options.Count; i++)
            {
                string site = options[i].Text;
                if (site.ToLower().Contains(siteName.ToLower()))
                {
                    dropdownButton.Click();
                    waitForDropdownClose();
                    return true;
                }
            }

            //site wasn't found in the dropdown
            dropdownButton.Click();
            waitForDropdownClose();

            return false;
        }



        //determines if excel file is in downlaods folder after pressing excel export
        protected override bool FileDownloaded(string extension)
        {
            string downloadPath = ConfigurationManager.AppSettings["DownloadPath"];

            DirectoryInfo directory = new DirectoryInfo(downloadPath);
            FileInfo[] files = directory.GetFiles(@"Ticket Sales Report_*." + extension);

            return files.Length > 0;

        }



        //select a period from the dropdown
        public override void SelectPeriodByValue(PeriodSelectOptions val)
        {
            int index = 0;

            //get the index of the option in the list
            switch (val)
            {
                case PeriodSelectOptions.CURRENT_DAY:
                    index = 0;
                    break;
                case PeriodSelectOptions.PREV_DAY:
                    index = 1;
                    break;
                case PeriodSelectOptions.PREV_WEEK:
                    index = 2;
                    break;
                case PeriodSelectOptions.PREV_MONTH:
                    index = 3;
                    break;
                case PeriodSelectOptions.DATE_RANGE:
                    index = 4;
                    break;
                default:
                    index = 0;
                    break;
            }

            //wait for device select to be visible
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + PeriodInputIndex + "]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + PeriodInputIndex + "]/following-sibling::span")));
            dropdownButton.Click();
            waitForDropdownOpen();

            //get options in dropdown list
            IWebElement list = GetDropdDownList();
            //list is empty or not found
            if (list == null)
            {
                dropdownButton.Click();
                waitForDropdownClose();
                return;
            }
            else
            {
                IList<IWebElement> options = list.FindElements(By.TagName("li"));

                //index out of range
                if (index > options.Count || index < 0)
                {
                    dropdownButton.Click();
                    waitForDropdownClose();
                    return;
                }

                try
                {
                    options[index].Click();

                }
                //element not interactable exception
                catch (Exception ex)
                {
                    dropdownButton.Click();
                }
                waitForDropdownClose();
            }
        }



        public TicketSalesReportRecord GetRecordByIndex(int index)
        {
            waitForTable();

            int count = getRecordCount();
            if (index > count || index < 0 || count == 0)
            {
                return null;
            }

            IWebElement targetRow = Table.FindElement(By.XPath(".//tbody/tr[" + (index + 1) + "]"));
            TicketSalesReportRecord record = new TicketSalesReportRecord();

            try
            {
                record.SiteId = int.Parse(targetRow.FindElement(By.XPath(".//td[1]")).Text);
                record.AgentNum = targetRow.FindElement(By.XPath(".//td[2]")).Text;
                record.SiteName = targetRow.FindElement(By.XPath(".//td[3]")).Text;
                record.DeviceId = int.Parse(targetRow.FindElement(By.XPath(".//td[4]")).Text);
                record.LotteryGameId = int.Parse(targetRow.FindElement(By.XPath(".//td[5]")).Text);
                record.TicketPrice = double.Parse(targetRow.FindElement(By.XPath(".//td[6]")).Text, System.Globalization.NumberStyles.Currency);
                record.GameName = targetRow.FindElement(By.XPath(".//td[7]")).Text;
                record.NumTicketsSold = int.Parse(targetRow.FindElement(By.XPath(".//td[8]")).Text);
                record.TotalAmount = double.Parse(targetRow.FindElement(By.XPath(".//td[9]")).Text, System.Globalization.NumberStyles.Currency);
            }
            catch (Exception ex)
            {
                return null;
            }


            return record;
        }



        public double GetTotalAmountSold()
        {
            waitForTable();

            try
            {
                IWebElement summaryRow = wait.Until(d => d.FindElement(By.ClassName("e-summaryrow")));
                int colCount = GetColCount();

                string price = summaryRow.FindElement(By.XPath(".//td[" + (colCount) + "]")).Text;

                double total = double.Parse(price, System.Globalization.NumberStyles.Currency);
                return total;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }



        public int GetTotalNumTicketsSold()
        {
            waitForTable();

            try
            {
                IWebElement summaryRow = wait.Until(d => d.FindElement(By.ClassName("e-summaryrow")));
                int colCount = GetColCount();

                string val = summaryRow.FindElement(By.XPath(".//td[" + (colCount - 1) + "]")).Text;

                int total = int.Parse(val);
                return total;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
