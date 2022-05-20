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
    internal class VirtualAccountNumbersPage : TablePage
    {

        [FindsBy(How = How.XPath, Using = "(//table)[6]")]
        private IWebElement table;

        [FindsBy(How = How.XPath, Using = "(//table)[5]")]
        private IWebElement HeaderTable;

        [FindsBy(How = How.ClassName, Using = "alert-warning")]
        private IWebElement MissingVANAlert;

        [FindsBy(How = How.XPath, Using = "//input[@type='checkbox']")]
        private IWebElement SharedVANCheckbox;

        [FindsBy(How = How.XPath, Using = "//button[text()='Generate All Missing VANs']")]
        private IWebElement GenerateAllVANButton;

        [FindsBy(How = How.XPath, Using = "//button[text()='Generate VAN']")]
        private IWebElement GenerateVANButton;

        [FindsBy(How = How.XPath, Using = "//*[contains(@id,'ToolbarSearchBox')]")]
        private IWebElement SearchInput;

        [FindsBy(How = How.XPath, Using = "//button[@aria-label='Collapse All']")]
        private IWebElement CollapseAllButton;

        [FindsBy(How = How.XPath, Using = "//button[@aria-label='Expand All']")]
        private IWebElement ExpandAllButton;

        [FindsBy(How = How.XPath, Using = "//div[@class='e-toolbar-left']/div[1]/button")]
        private IWebElement ExcelButton;

        [FindsBy(How = How.ClassName, Using = "e-clear-icon")]
        private IWebElement SearchClearButton;

        public VanModal ConfirmationModal;

        public static int VANS_PER_GAME = int.Parse(ConfigurationManager.AppSettings["NumberVANsPerGame"]);

        public static string VAN_INN = ConfigurationManager.AppSettings["INN"];


        public override IWebElement Table
        {
            get
            {

                return table;
            }
        }


        public VirtualAccountNumbersPage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            ConfirmationModal = new VanModal(driver);
        }


        //waits for table to either have records or display "no records found"
        public override void waitForTable()
        {
            //when table is in shared VAN mode, the row with the VAN has no class. Just wait for a reoq to be visible
            if (SharedVanIsEnabled())
            {
                wait.Until(d => Table.FindElements(By.TagName("tr")).Count > 0);
            }
            else
            {
                /*
                 * 3 Scenarios to wait for when shared VAN is disabled:
                 * 1st - if VANs are generated wait until e-row is greater than 0
                 * 2nd - if search results are 0 or if shared VAN hasnt been generated then 'No records to display' row is in the table
                 *       and the row has the class 'e-emptyrow'
                 * 3rd - if all sections are collapsed then only the game headers are in the table. In this scenario wait until row
                 *       that don't have the aria-rowindex attribute (header rows) to be visible
                 */

                try
                {

                    wait.Until(d => Table.FindElements(By.TagName("tr")).Count >= 4);

                }
                catch (Exception ex)
                {

                }
            }
        }


        //waits for the collapse button to have the class 'collapse'
        private void waitForSectionCollapse(int index)
        {
            waitForTable();

            IList<IWebElement> headers = Table.FindElements(By.XPath(".//tr[not(@aria-rowindex)]"));
            //find specific header by index
            IWebElement targetHeader = headers[index];

            wait.Until(d => targetHeader.FindElement(By.XPath(".//td[1]")).GetAttribute("class").Contains("collapse"));
        }


        //waits for the collapse button to have the class 'expand'
        private void waitForSectionExpand(int index)
        {
            IList<IWebElement> headers = Table.FindElements(By.XPath(".//tr[not(@aria-rowindex)]"));
            //find specific header by index
            IWebElement targetHeader = headers[index];

            wait.Until(d => targetHeader.FindElement(By.XPath(".//td[1]")).GetAttribute("class").Contains("expand"));
        }


        //wait for spinning loading bar to dissapear after performing an action on the table
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



        //returns number of VAN entries in the table
        public override int getRecordCount()
        {
            waitForTable();
            /*
            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));

            if (SharedVanIsEnabled())
            {
                if(rows[0].GetAttribute("class") == "e-emptyrow")
                {
                    return 0;
                }

                return rows.Count();
            }

            int count = 0;

            foreach(IWebElement row in rows)
            {
                if (row.GetAttribute("aria-rowindex") != null && row.GetAttribute("class").Contains("e-row"))
                {
                    IWebElement content = row.FindElement(By.XPath(".//td[2]"));
                    if (content.Text != "No VAN generated for this game.")
                    {
                        count++;
                    }
                }
            }

            return count;
            */


            //get all rows in the table regardless of if they are a header or VAN row
            IList<IWebElement> rows = Table.FindElements(By.TagName("tr"));

            //van is stored in 2nd column of table for multiple VAN mode and in 1st column for shared VAN mode
            //use this to index into the row to get the text
            int dataIndex = SharedVanIsEnabled() ? 1 : 2;

            //regex to match a VAN (19 digit number)
            string pattern = @"[0-9]{19}";
            Regex regex = new Regex(pattern);

            //loop over all rows. If the text in the data column matches the regex for a VAN
            //this is considered a valid record
            int count = 0;
            foreach (IWebElement row in rows)
            {
                try
                {
                    //check if data in row matches a valid VAN regex. If it does increment record count
                    string data = row.FindElement(By.XPath(".//td[" + dataIndex + "]")).Text;
                    if (Regex.Match(data, pattern).Success)
                    {
                        count++;
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return count;
        }


        public override int matchingRecordCount(string search)
        {
            waitForTable();

            int colIndex = SharedVanIsEnabled() ? 1 : 2;

            IList<IWebElement> headers = Table.FindElements(By.XPath(".//tr[not(@aria-rowindex)]"));

            //if search string is a game name or game UPC return game VAN count
            int index = 0;
            foreach (IWebElement row in headers)
            {
                string gameName = row.FindElement(By.XPath(".//td[2]")).Text;
                Console.WriteLine(gameName);
                if (gameName.ToLower().Contains(search.ToLower()))
                {
                    Console.WriteLine("index in matching record: {0}", index);
                    return recordCountForHeader(index);
                }
                index++;
            }

            int matches = 0;

            IList<IWebElement> rows = Table.FindElements(By.TagName("tr"));
            foreach (IWebElement row in rows)
            {
                if (row.GetAttribute("aria-rowindex") != null)
                {

                    IWebElement content = row.FindElement(By.XPath(".//td[" + colIndex + "]"));
                    //find non empty rows
                    if (content.Text != "No VAN generated for this game.")
                    {

                        if (content.Text.IndexOf(search) != -1)
                        {
                            matches++;
                        }
                    }
                }
            }

            return matches;

        }


        public int GetGameCount()
        {
            waitForTable();

            if (SharedVanIsEnabled())
            {
                return 0;
            }

            //get game header rows
            IList<IWebElement> headers = Table.FindElements(By.XPath(".//tr[not(@aria-rowindex)]"));

            return headers.Count();
        }


        //presses the collapse button for a section by index
        public void CollapseSection(int sectionIndex)
        {
            waitForTable();

            if (sectionIndex < 0 || sectionIndex > 4)
            {
                return;
            }

            //if section already collapsed just return
            if (SectionIsCollapsed(sectionIndex))
            {
                return;
            }

            IWebElement collapseButton = GetCollapseButtonByIndex(sectionIndex);

            collapseButton.Click();

            waitForSectionCollapse(sectionIndex);

        }


        public void ExpandSection(int sectionIndex)
        {
            waitForTable();

            if (sectionIndex < 0 || sectionIndex > 4)
            {
                return;
            }

            //if section already collapsed just return
            if (!SectionIsCollapsed(sectionIndex))
            {
                return;
            }

            IWebElement collapseButton = GetCollapseButtonByIndex(sectionIndex);

            collapseButton.Click();

            waitForSectionExpand(sectionIndex);
        }



        //returns IWebElement of the collapse button for a game section
        private IWebElement GetCollapseButtonByIndex(int index)
        {
            waitForTable();


            //get game header rows
            IList<IWebElement> headers = Table.FindElements(By.XPath(".//tr[not(@aria-rowindex)]"));

            //find specific header by index
            IWebElement targetHeader = headers[index];
            //find collapse button
            IWebElement collapseButton = targetHeader.FindElement(By.XPath(".//td[1]"));

            return collapseButton;
        }


        //determines if game section is collapsed by looking at the class of the row
        public bool SectionIsCollapsed(int index)
        {
            waitForTable();

            if (index < 0 || index > 4)
            {
                return false;
            }

            IWebElement collapseButton = GetCollapseButtonByIndex(index);
            string buttonClass = collapseButton.GetAttribute("class");

            if (buttonClass.Contains("collapse"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public string GetUPCForGame(int index)
        {
            waitForTable();

            //get game header rows
            IList<IWebElement> headers = Table.FindElements(By.XPath(".//tr[not(@aria-rowindex)]"));
            Console.WriteLine("header count: {0}", headers.Count);
            //find specific header by index
            IWebElement targetHeader = headers[index];
            IWebElement targetCol = targetHeader.FindElement(By.XPath(".//td[2]"));
            string text = targetCol.Text;

            string pattern = @"[0-9]{12}";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(text);

            return match.Value;
        }


        public int indexOfHeader(int index)
        {
            waitForTable();

            if (index < 0)
            {
                return -1;
            }


            IList<IWebElement> rows = Table.FindElements(By.TagName("tr"));

            int headerIndex = -1;
            for (int i = 0; i < rows.Count; i++)
            {
                IWebElement row = rows[i];

                //if row doesn't have class 'e-row' then its a game header row
                if (row.GetAttribute("aria-rowindex") == null)
                {
                    //if index of header in list mtches index retrun the position in the list
                    headerIndex++;
                    if (headerIndex == index)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }


        public IList<string> GetVansForSectionByIndex(int index)
        {
            waitForTable();

            ExpandSection(index);

            //get game header rows
            IList<IWebElement> headers = Table.FindElements(By.XPath(".//tr[not(@aria-rowindex)]"));
            //find specific header by index
            IWebElement targetHeader = headers[index];
            IList<IWebElement> rows = Table.FindElements(By.TagName("tr"));
            Console.WriteLine(rows.Count);
            List<string> vans = new List<string>();
            Console.WriteLine("index in gat vans for sections: {0}", index);
            //get index of header in the list
            int startIndex = indexOfHeader(index);
            Console.WriteLine("header index: " + startIndex);
            IWebElement nextRow = rows[startIndex + 1];

            //if next row is empty then the header doesn't have any vans generated yet
            string content = nextRow.FindElement(By.XPath(".//td[2]")).Text;
            if (content == "No VAN generated for this game.")
            {
                return vans;
            }

            //if next row isnt empty then return next 10 rows in the table
            List<IWebElement> headerRows = rows.ToList().GetRange(startIndex + 1, VANS_PER_GAME);
            foreach (IWebElement r in headerRows)
            {
                vans.Add(r.FindElement(By.XPath(".//td[2]")).Text);
            }

            return vans;

        }


        //gets number of vans for a game by index of the game section in the list
        public int recordCountForHeader(int headerIndex)
        {
            Console.WriteLine("index in record count: {0}", headerIndex);
            IList<string> vans = GetVansForSectionByIndex(headerIndex);

            return vans.Count;
        }


        //clicks the generate VANs button for a game that is missing vans. Selects game by index in the list
        public void GenerateVansForGame(int headerIndex)
        {
            waitForTable();

            int index = indexOfHeader(headerIndex);
            IList<IWebElement> rows = Table.FindElements(By.TagName("tr"));
            IWebElement nextRow = rows[index + 1];

            //verify next row is empty
            string nextRowText = nextRow.FindElement(By.XPath(".//td[2]")).Text;
            if (nextRowText == "No VAN generated for this game.")
            {
                try
                {
                    IWebElement generateVanButton = nextRow.FindElement(By.XPath(".//td[4]/div/button"));
                    generateVanButton.Click();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                waitForFilter();
            }

        }


        //Clicks "Generate All Missing VANs" button or "Generate VAN" button if shared van is enabled
        public void GenerateAllVans()
        {
            if (SharedVanIsEnabled())
            {
                //try to click button. It may be gone if VAN already generated
                try
                {
                    wait.Until(d => d.FindElement(By.XPath("//button[text()='Generate VAN']")));
                    GenerateVANButton.Click();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Generate VAN button not found");
                }
            }
            else
            {
                GenerateAllVANButton.Click();
            }

            waitForFilter();
        }





        //Enter text into the search input
        public void EnterSearchTerm(string text)
        {
            wait.Until(d => d.FindElement(By.XPath("//*[contains(@id,'ToolbarSearchBox')]")));

            SearchInput.Clear();
            SearchInput.SendKeys(text);

            //press ENTER
            SearchInput.SendKeys(Keys.Enter);

            waitForFilter();
        }


        //clears the search box of text
        public void ClearSearch()
        {
            wait.Until(d => d.FindElement(By.ClassName("e-clear-icon")));

            SearchClearButton.Click();
            waitForFilter();
        }


        public string GetGameByIndex(int headerIndex)
        {
            waitForTable();

            //get game header rows
            IList<IWebElement> headers = Table.FindElements(By.XPath(".//tr[not(@aria-rowindex)]"));
            //find specific header by index
            IWebElement targetHeader = headers[headerIndex];
            IWebElement targetCol = targetHeader.FindElement(By.XPath(".//td[2]"));
            string text = targetCol.Text;

            int hyphenIndex = text.IndexOf("-");
            text = text.Substring(0, hyphenIndex - 1);

            return text;
        }


        public List<string> GetGames()
        {
            waitForTable();

            List<string> games = new List<string>();

            IList<IWebElement> headers = Table.FindElements(By.XPath(".//tr[not(@aria-rowindex)]"));
            for (int i = 0; i < headers.Count; i++)
            {
                games.Add(GetGameByIndex(i));
            }

            return games;
        }


        public string GetVanByIndex(int index)
        {
            waitForTable();

            IList<IWebElement> rows = Table.FindElements(By.TagName("tr"));

            //shared van
            if (SharedVanIsEnabled())
            {
                if (getRecordCount() == 0)
                {
                    return null;
                }

                return rows[0].FindElement(By.XPath(".//td[1]")).Text;
            }

            foreach (IWebElement row in rows)
            {
                if (row.GetAttribute("aria-rowindex") != null && row.GetAttribute("class").Contains("e-row"))
                {
                    IWebElement content = row.FindElement(By.XPath(".//td[2]"));

                    try
                    {
                        int rowIndex = int.Parse(row.GetAttribute("aria-rowindex"));

                        if (rowIndex == index)
                        {
                            return content.Text;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            return null;

        }


        public void CollapseAll()
        {
            waitForTable();

            CollapseAllButton.Click();

            waitForSectionCollapse(0);
        }


        public void ExpandAll()
        {
            waitForTable();

            ExpandAllButton.Click();

            waitForSectionExpand(0);

        }


        public void EnableSharedVan()
        {

            wait.Until(d => d.FindElement(By.XPath("//input[@type='checkbox']")));

            if (SharedVanIsEnabled())
            {
                return;
            }

            SharedVANCheckbox.Click();
            ConfirmationModal.ClickConfirm();

            waitForFilter();
        }


        public void DisableSharedVan()
        {
            waitForTable();
            wait.Until(d => d.FindElement(By.XPath("//input[@type='checkbox']")));

            if (!SharedVanIsEnabled())
            {
                return;
            }

            SharedVANCheckbox.Click();
            waitForFilter();
        }


        public bool SharedVanIsEnabled()
        {

            wait.Until(d => d.FindElement(By.XPath("//input[@type='checkbox']")));

            return SharedVANCheckbox.Selected;
        }


        public bool MissingVanAlertIsVisible()
        {
            WebDriverWait fluentWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(50);

            try
            {
                fluentWait.Until(d => d.FindElement(By.ClassName("alert-warning")));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        //determines if excel file is in downlaods folder after pressing excel export
        public bool FileDownloaded()
        {
            string downloadPath = ConfigurationManager.AppSettings["DownloadPath"];

            DirectoryInfo directory = new DirectoryInfo(downloadPath);
            FileInfo[] files = directory.GetFiles(@"Export*.xlsx");

            return files.Length > 0;

        }


        private void ClickExcelExport()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//div[@class='e-toolbar-left']/div[1]/button")));
            ExcelButton.Click();

        }


        //click excel button and download file
        public void DownloadExcelFile()
        {
            ClickExcelExport();

            WaitForFileDownload();
        }


        public override void SortByColAsc(int index)
        {
            waitForTable();

            IWebElement vanCol = HeaderTable.FindElement(By.XPath(".//thead/tr[1]/th[2]"));
            IWebElement sortedBy = vanCol.FindElement(By.XPath(".//div[2]"));

            //aready sorted by ascending
            if (sortedBy.GetAttribute("class").Contains("ascending"))
            {
                return;
            }
            //if not sorted by anything click once to sort by ascending
            else if (!sortedBy.GetAttribute("class").Contains("ascending") && !sortedBy.GetAttribute("class").Contains("descending"))
            {
                vanCol.Click();
            }
            //if set to descending click twice to get to ascending
            else
            {
                vanCol.Click();
                vanCol.Click();
            }
        }


        public override void SortByColDesc(int index)
        {
            waitForTable();

            IWebElement vanCol = HeaderTable.FindElement(By.XPath(".//thead/tr[1]/th[2]"));
            IWebElement sortedBy = vanCol.FindElement(By.XPath(".//div[2]"));

            //aready sorted by descinding
            if (sortedBy.GetAttribute("class").Contains("descending"))
            {
                return;
            }
            //if not sorted by anything click twice to sort by descedning
            else if (!sortedBy.GetAttribute("class").Contains("ascending") && !sortedBy.GetAttribute("class").Contains("descending"))
            {
                vanCol.Click();
                vanCol.Click();
            }
            //if set to ascending click twice to get to ascending
            else
            {
                vanCol.Click();
            }
        }


        //determines if the "Generate VAN" button is visible for a game
        public bool GameGenerateVanIsVisible(int gameIndex)
        {
            IList<IWebElement> rows = Table.FindElements(By.TagName("tr"));
            int startIndex = indexOfHeader(gameIndex);

            IWebElement nextRow = rows[startIndex + 1];

            try
            {
                IWebElement vanBtn = nextRow.FindElement(By.XPath(".//td[4]/div/button"));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Generate van button not found for row: " + (startIndex + 1));
                return false;
            }
        }


        //returns a List containing all VANs in the table
        public List<string> GetAllVans()
        {
            waitForTable();

            List<string> vans = new List<string>();
            IList<IWebElement> rows = Table.FindElements(By.TagName("tr"));

            int dataIndex = SharedVanIsEnabled() ? 1 : 2;

            string pattern = @"[0-9]{19}";
            Regex regex = new Regex(pattern);

            foreach (IWebElement row in rows)
            {
                try
                {
                    string data = row.FindElement(By.XPath(".//td[" + dataIndex + "]")).Text;

                    if (Regex.Match(data, pattern).Success)
                    {
                        vans.Add(data);
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return vans;
        }


        //returns true if Generate shared van button is visible of false if not
        public bool GenerateSharedVanButtonIsVisible()
        {
            try
            {
                wait.Until(d => d.FindElement(By.XPath("//button[text()='Generate VAN']")));
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
