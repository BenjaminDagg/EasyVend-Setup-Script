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
    public enum PeriodSelectOptions : int
    {
        CURRENT_DAY = 0,
        PREV_DAY = 1,
        PREV_WEEK = 2,
        DATE_RANGE = 3,
        PREV_MONTH = 4,
        NONE = 5
    }

    internal class ReportPage : TablePage
    {

        [FindsBy(How = How.XPath, Using = "//*[contains(@id,'content_table')]")]
        private IWebElement table;

        public override IWebElement Table { get { return table; } }

        [FindsBy(How = How.XPath, Using = "//*[contains(@id,'header_table')]")]
        protected IWebElement HeaderTable;

        [FindsBy(How = How.XPath, Using = "(//input)[2]")]
        public virtual IWebElement SiteSelect
        {
            get
            {
                return driver.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]"));
            }

        }

        [FindsBy(How = How.XPath, Using = "(//input)[4]")]
        public virtual IWebElement PeriodSelect
        {
            get
            {
                return driver.FindElement(By.XPath("(//input)[" + PeriodInputIndex + "]"));
            }

        }

        [FindsBy(How = How.Id, Using = "RunReport")]
        public virtual IWebElement RunReportButton { get; set; }

        [FindsBy(How = How.XPath, Using = "//input[@aria-label='Pagerdropdown']")]
        protected IWebElement tableLengthSelect;

        [FindsBy(How = How.XPath, Using = "(//input[contains(@id, 'datepicker')])[1]")]
        protected IWebElement StartDate;

        [FindsBy(How = How.XPath, Using = "(//input[contains(@id, 'datepicker')])[2]")]
        protected IWebElement EndDate;

        [FindsBy(How = How.ClassName, Using = "alert-danger")]
        protected IWebElement ErrorBanner;

        [FindsBy(How = How.Id, Using = "CsvExport")]
        protected IWebElement CSVExport;

        [FindsBy(How = How.Id, Using = "PdfExport")]
        protected IWebElement PDFExport;

        [FindsBy(How = How.Id, Using = "ExcelExport")]
        protected IWebElement ExcelExport;

        public override IWebElement TableLengthSelect { get { return tableLengthSelect; } }

        protected virtual int SiteInputIndex { get; set; }
        protected virtual int PeriodInputIndex { get; set; }




        public ReportPage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            SiteInputIndex = 2;
            PeriodInputIndex = 4;
        }



        //waits for table to load
        public override void waitForTable()
        {
            try
            {
                wait.Until(d => d.FindElements(By.ClassName("e-row")).Count > 0);
            }
            catch (Exception ex)
            {

            }
        }




        //waits for table to finish filtering by checking for spinner loading bar
        public override void waitForFilter()
        {


            //create new wait that polls for element every 100ms
            WebDriverWait fluentWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(50);

            //wait for spinner to appear 
            try
            {
                fluentWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("e-spin-fabric")));
            }
            catch (Exception ex)
            {

            }
            //if wheel was missed (already gone) just skip the wait because the table is most likely loaded already
            finally
            {
                fluentWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("e-spin-fabric")));
            }


        }



        //returns number of records in the table
        public override int getRecordCount()
        {
            waitForTable();

            //get all rows in the table
            IList<IWebElement> rows = Table.FindElements(By.TagName("tr"));
            //if first row has class 'e-emptyrow' then table is empty
            if (rows[0].GetAttribute("class") == "e-emptyrow")
            {
                return 0;
            }

            return rows.Count();
        }




        //determines if a dropdown menu is open by searching for element e-ddl
        public bool DropDownIsOpen()
        {
            try
            {
                wait.Until(d => d.FindElement(By.ClassName("e-ddl")));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        protected virtual void waitForDropdownOpen()
        {
            try
            {
                wait.Until(d => d.FindElement(By.ClassName("e-list-parent")));
            }
            catch (Exception ex)
            {

            }
        }


        protected void waitForDropdownClose()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("e-list-parent")));
        }


        protected virtual IWebElement GetDropdDownList()
        {
            try
            {
                IWebElement list = wait.Until(d => d.FindElement(By.ClassName("e-list-parent")));
                return list;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        //determines if error banner with the given text is being displayed
        protected bool ErrorIsDisplayed(string errorType)
        {
            try
            {
                //wait for error banner to be visible
                wait.Until(d => d.FindElement(By.ClassName("alert-danger")));

                //go throguh all banners and check if a banner has the word errorType
                IList<IWebElement> errors = driver.FindElements(By.ClassName("alert-danger"));
                foreach (IWebElement error in errors)
                {
                    string errorText = error.Text;
                    if (errorText.ToLower().Contains(errorType.ToLower()))
                    {
                        return true;
                    }
                }

                //banner not found with search term
                return false;
            }
            //no banners were found
            catch (Exception e)
            {
                return false;
            }
        }


        //returns true if site error banner is displayed or false otherwise
        public bool SiteErrorIsDisplayed()
        {
            return ErrorIsDisplayed("Site");
        }


        //returns true if date error banner is displayed or false otherwise
        public bool DateErrorIsDisplayed()
        {
            return ErrorIsDisplayed("date");
        }



        //enters text into the start date select
        public void EnterStartDate(string date)
        {
            wait.Until(d => d.FindElement(By.XPath("(//input[contains(@id, 'datepicker')])[1]")));
            StartDate.SendKeys(date);
        }



        public void ClickRunReport()
        {
            wait.Until(d => d.FindElement(By.Id("RunReport")));
            RunReportButton.Click();
        }



        //enters text into the end date select
        public void EnterEndDate(string date)
        {
            wait.Until(d => d.FindElement(By.XPath("(//input[contains(@id, 'datepicker')])[2]")));
            EndDate.SendKeys(date);
        }


        //select Date Range time period and enters start date and end date values
        public void EnterDateRange(string start, string end)
        {
            SelectPeriodByValue(PeriodSelectOptions.DATE_RANGE);
            EnterStartDate(start);
            EnterEndDate(end);
        }


        public void GenerateReport()
        {
            wait.Until(d => d.FindElement(By.Id("RunReport")));
            RunReportButton.Click();

            waitForTable();
        }


        //enter text in site dropdown search input
        public virtual void EnterSite(string text)
        {
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]")));

            SiteSelect.SendKeys(text);
        }



        public void ClickCsvExport()
        {
            wait.Until(d => d.FindElement(By.Id("CsvExport")));
            CSVExport.Click();


        }



        public bool CsvFileDownloaded()
        {
            try
            {
                WaitForFileDownload("csv");
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }



        public void ClickPdfExport()
        {
            wait.Until(d => d.FindElement(By.Id("PdfExport")));
            PDFExport.Click();


        }



        public bool PdfFileDownloaded()
        {
            try
            {
                WaitForFileDownload("PDF");
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }



        public void ClickExcelExport()
        {
            wait.Until(d => d.FindElement(By.Id("ExcelExport")));
            ExcelExport.Click();

        }



        public bool ExcelFileDownloaded()
        {
            try
            {
                WaitForFileDownload("xlsx");
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }



        //select a site from the site dropdown by index in the list
        public virtual void SelectSiteByIndex(int index)
        {
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]/following-sibling::span")));
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

            //index out of range
            if (index > options.Count || index < 0 || options.Count == 0)
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



        //select a site in the site dropdown by site name
        public virtual void SelectSiteByValue(string siteName)
        {
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]/following-sibling::span")));
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



        //returns the currently selected option in the site dropdown  or null if nothing is selcted
        public virtual string GetSelectedSite()
        {
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]")));

            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]/following-sibling::span")));
            dropdownButton.Click();
            waitForDropdownOpen();

            IWebElement list = GetDropdDownList();
            //list is empty or not found
            if (list == null)
            {
                dropdownButton.Click();
                waitForDropdownClose();
                return null;
            }

            IList<IWebElement> options = list.FindElements(By.TagName("li"));
            string selectedSite = "";
            for (int i = 0; i < options.Count; i++)
            {
                string itemClass = options[i].GetAttribute("class");
                if (itemClass.Contains("e-active"))
                {
                    selectedSite = options[i].Text;
                }
            }

            dropdownButton.Click();
            waitForDropdownClose();

            return selectedSite;
        }



        //displays all options in the site selection dropdown
        public List<string> GetSiteOptions()
        {
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]")));

            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]/following-sibling::span")));
            dropdownButton.Click();
            waitForDropdownOpen();

            IWebElement list = wait.Until(d => d.FindElement(By.ClassName("e-list-parent")));
            List<string> siteOptions = new List<string>();
            if (list == null)
            {
                dropdownButton.Click();
                waitForDropdownClose();

                return siteOptions;
            }

            IList<IWebElement> options = list.FindElements(By.TagName("li"));
            foreach (IWebElement option in options)
            {
                siteOptions.Add(option.Text);
            }

            dropdownButton.Click();
            waitForDropdownClose();

            return siteOptions;

        }



        //returns true if the searchSite is found as an option in the site dropdown
        public virtual bool SiteOptionExists(string siteName)
        {
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + SiteInputIndex + "]/following-sibling::span")));
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




        //select a period from the dropdown
        public virtual void SelectPeriodByValue(PeriodSelectOptions val)
        {
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
                int index = (int)val;


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



        //takes a string and returns the corresponding PeriodSelectOption value
        protected virtual PeriodSelectOptions parsePeriodSelectOption(string val)
        {
            switch (val)
            {
                case "Current Day":
                    return PeriodSelectOptions.CURRENT_DAY;
                case "Previous Day":
                    return PeriodSelectOptions.PREV_DAY;
                case "Previous Week":
                    return PeriodSelectOptions.PREV_WEEK;
                case "Date Range":
                    return PeriodSelectOptions.DATE_RANGE;
                case "Previous Month":
                    return PeriodSelectOptions.PREV_MONTH;
                default:
                    return PeriodSelectOptions.NONE;
            }
        }



        //gets the value of the currently selected time period option
        public PeriodSelectOptions GetSelectedPeriod()
        {
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
                return PeriodSelectOptions.NONE;
            }
            else
            {

                IList<IWebElement> options = list.FindElements(By.TagName("li"));
                for (int i = 0; i < options.Count; i++)
                {
                    string className = options[i].GetAttribute("class");
                    if (className.Contains("e-active"))
                    {
                        string optionVal = options[i].Text;
                        PeriodSelectOptions result = parsePeriodSelectOption(optionVal);

                        options[i].Click();
                        waitForDropdownClose();

                        return result;
                    }
                }

                //selected options wasn't found
                dropdownButton.Click();
                waitForDropdownClose();
                return PeriodSelectOptions.NONE;
            }
        }



        //returns list of PeriodSelectOptions of all options in the period dropdown
        public List<PeriodSelectOptions> GetPeriodOptions()
        {
            //wait for device select to be visible
            wait.Until(d => d.FindElement(By.XPath("(//input)[" + PeriodInputIndex + "]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[" + PeriodInputIndex + "]/following-sibling::span")));
            dropdownButton.Click();
            waitForDropdownOpen();

            List<PeriodSelectOptions> periodOptions = new List<PeriodSelectOptions>();

            //get options in dropdown list
            IWebElement list = GetDropdDownList();
            //list is empty or not found
            if (list == null)
            {
                dropdownButton.Click();
                waitForDropdownClose();
                return periodOptions;
            }
            else
            {

                IList<IWebElement> options = list.FindElements(By.TagName("li"));
                for (int i = 0; i < options.Count; i++)
                {
                    string optionVal = options[i].Text;
                    PeriodSelectOptions result = parsePeriodSelectOption(optionVal);

                    periodOptions.Add(result);
                }


                dropdownButton.Click();
                waitForDropdownClose();
                return periodOptions;
            }
        }



        //selects 'item per page' dropdown'
        public override void SetTableLength(int length)
        {
            waitForTable();

            //get index of length option in list
            int index;
            switch (length)
            {
                case 10:
                    index = 0;
                    break;
                case 25:
                    index = 1;
                    break;
                case 50:
                    index = 2;
                    break;
                case 100:
                    index = 3;
                    break;
                default:
                    index = 0;
                    break;
            }

            wait.Until(d => d.FindElement(By.XPath("//input[@aria-label='Pagerdropdown']")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("//input[@aria-label='Pagerdropdown']/following-sibling::span")));
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

            //index is out of range
            IList<IWebElement> options = wait.Until(d => list.FindElements(By.TagName("li")));
            if (index > options.Count || index < 0)
            {
                dropdownButton.Click();
                waitForDropdownClose();
                return;
            }

            try
            {
                //click device based off index in list
                options[index].Click();
            }
            catch (Exception ex)
            {
                dropdownButton.Click();
            }
            waitForDropdownClose();
            waitForFilter();

        }



        //determines if excel file is in downlaods folder after pressing excel export
        protected virtual bool FileDownloaded(string extension)
        {
            string downloadPath = ConfigurationManager.AppSettings["DownloadPath"];

            DirectoryInfo directory = new DirectoryInfo(downloadPath);
            FileInfo[] files = directory.GetFiles(@"Export *." + extension);

            return files.Length > 0;

        }



        //wait for downloaded file to appear in downloads folder
        protected virtual void WaitForFileDownload(string extension)
        {
            wait.Until(d => FileDownloaded(extension));
        }



        //returns number of headers columns in the table
        public virtual int GetColCount()
        {
            try
            {
                //wait for header table to by visible
                wait.Until(d => d.FindElement(By.XPath("//*[contains(@id,'header_table')]")));

                IList<IWebElement> headers = driver.FindElements(By.ClassName("e-headercell"));

                return headers.Count;
            }
            //table hasnt loaded yet or a site/device/preiod hasnt been selected uet
            catch (Exception ex)
            {
                return 0;
            }
        }



        //returns the text inside a header by index of the header in the list starting from 0
        public virtual string GetHeaderByColNum(int colNum)
        {
            try
            {
                //wait for header table to by visible
                wait.Until(d => d.FindElement(By.XPath("//*[contains(@id,'header_table')]")));

                IList<IWebElement> headers = driver.FindElements(By.ClassName("e-headercell"));

                if (colNum < 0 || colNum > headers.Count || headers.Count == 0)
                {
                    return null;
                }

                return headers[colNum].Text;
            }
            //table hasnt loaded yet or a site/device/preiod hasnt been selected uet
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
