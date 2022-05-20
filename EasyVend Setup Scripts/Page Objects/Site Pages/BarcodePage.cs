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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class BarcodePage : TablePage
    {

        [FindsBy(How = How.ClassName, Using = "add-bt")]
        private IWebElement AddButton;

        [FindsBy(How = How.XPath, Using = "//button[@aria-label='Excel Export']")]
        private IWebElement ExcelExport;

        [FindsBy(How = How.XPath, Using = "//*[contains(@id,'content_table')]")]
        private IWebElement table;

        [FindsBy(How = How.XPath, Using = "//*[contains(@id,'header_table')]")]
        private IWebElement HeaderTable;

        public override IWebElement Table { get { return table; } }

        public BarcodeModal BarcodeModal { get; set; }



        public BarcodePage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            BarcodeModal = new BarcodeModal(driver);
        }


        //wait for a row to be in the table
        public override void waitForTable()
        {
            WebDriverWait errorWait = new WebDriverWait(driver, TimeSpan.FromSeconds(2.5));

            try
            {

                errorWait.Until(d => Table.FindElements(By.ClassName("e-rowcell")).Count > 0);
            }
            catch (Exception ex)
            {

            }

        }


        /*
         * Check for the spinning wheel at the top of the page under the 'e-spinner-inner' div to dissapear
         * so I know the new row has been added to the table. The wheel is only visible for very small
         * amount of time so check every 50ms. If not found wait for wheel to dissapear
         */
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


        //wait for downloaded file to appear in downloads folder
        private void WaitForFileDownload()
        {
            wait.Until(d => FileDownloaded());
        }


        public override int getRecordCount()
        {
            //wait for table to load
            waitForTable();


            //get all rows in the table
            string id = Table.GetAttribute("id");
            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));

            //get data of the first td in first row. Check if table is empty
            IWebElement firstRow = rows[0];
            if (firstRow.GetAttribute("class") == "e-emptyrow")
            {
                return 0;
            }

            return rows.Count();
        }


        public void AddBarcode()
        {

            wait.Until(d => d.FindElement(By.ClassName("add-bt")));

            waitForTable();

            AddButton.Click();

            waitForFilter();
        }


        private void ClickExcelExport()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//button[@aria-label='Excel Export']")));
            ExcelExport.Click();

        }



        //click excel button and download file
        public void DownloadExcelFile()
        {
            ClickExcelExport();

            try
            {
                WaitForFileDownload();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public string GetBarcodeByIndex(int index)
        {
            waitForTable();

            int count = getRecordCount();


            if (index < 0 || index > count)
            {
                return null;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            string barcode;
            try
            {
                barcode = cols[1].Text;
            }
            catch (Exception ex)
            {
                barcode = null;
            }

            return barcode;
        }


        public override void SortByColAsc(int index)
        {
            waitForTable();

            IWebElement thead = HeaderTable.FindElement(By.TagName("thead"));
            IWebElement headerRow = thead.FindElement(By.TagName("tr"));
            IList<IWebElement> headers = headerRow.FindElements(By.TagName("th"));

            IWebElement target = headers[index];
            IWebElement sortBtn = target.FindElement(By.ClassName("e-sortfilterdiv"));

            string className = sortBtn.GetAttribute("class");

            if (className.Contains("ascending"))
            {
                return;
            }

            target.Click();

            waitForFilter();
        }


        public override void SortByColDesc(int index)
        {
            waitForTable();

            IWebElement thead = HeaderTable.FindElement(By.TagName("thead"));
            IWebElement headerRow = thead.FindElement(By.TagName("tr"));
            IList<IWebElement> headers = headerRow.FindElements(By.TagName("th"));

            IWebElement target = headers[index];
            IWebElement sortBtn = target.FindElement(By.ClassName("e-sortfilterdiv"));

            string sortedBy = sortBtn.GetAttribute("class");

            //col is already sorted in descending
            if (sortedBy.Contains("descending"))
            {
                return;
            }

            //col has no class so click it twice to go from blank > asc > desc
            if (!sortedBy.Contains("ascending") && !sortedBy.Contains("descending"))
            {
                sortBtn.Click();
                sortBtn.Click();
            }
            //col is already on asc click it once for desc
            else if (sortedBy == "sorting_asc")
            {
                sortBtn.Click();
            }


            waitForFilter();

        }


        public void ClickDelete(int index)
        {
            waitForTable();

            int count = getRecordCount();
            if (count == 0)
            {
                return;
            }

            if (index < 0 || index > count)
            {
                return;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            try
            {
                IWebElement deleteBtn = cols[4].FindElement(By.TagName("button"));
                deleteBtn.Click();

            }
            catch (Exception ex)
            {

            }

        }


        public void DeleteBarcode(int index)
        {
            ClickDelete(index);
            BarcodeModal.ClickDelete();

            waitForFilter();
        }


        //returns index of barcode in the list
        public int IndexOfBarcode(string barcode)
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
                string text = cols[1].Text;

                if (text.ToLower() == barcode.ToLower())
                {

                    return index;
                }

                index++;
            }

            return -1;
        }


        //returns true if barcode is in the list or false if it doesn't exist
        public bool BarcodeExists(string barcode)
        {
            int index = IndexOfBarcode(barcode);

            if (index == -1)
            {
                return false;
            }

            return true;
        }



        //returns the barcode for a given barcode ID
        public string BarcodeForId(int id)
        {
            waitForTable();

            if (getRecordCount() == 0)
            {
                return null;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));

            foreach (IWebElement row in rows)
            {
                IList<IWebElement> cols = row.FindElements(By.TagName("td"));
                int barcodeId = int.Parse(cols[0].Text);

                if (barcodeId == id)
                {
                    string barcode = cols[1].Text;
                    return barcode;
                }
            }

            return null;
        }


        //determines if excel file is in downlaods folder after pressing excel export
        public bool FileDownloaded()
        {
            string downloadPath = ConfigurationManager.AppSettings["DownloadPath"];

            DirectoryInfo directory = new DirectoryInfo(downloadPath);
            FileInfo[] files = directory.GetFiles(@"Export*.xlsx");

            return files.Length > 0;

        }


        //returns the CreatedBy field for a barcode by index
        public string GetCreatedBy(int index)
        {
            waitForTable();

            int count = getRecordCount();


            if (index < 0 || index > count)
            {
                return null;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            string createdBy;

            try
            {
                createdBy = cols[3].Text;
            }
            catch (Exception)
            {
                createdBy = null;
            }

            return createdBy;
        }
    }
}
