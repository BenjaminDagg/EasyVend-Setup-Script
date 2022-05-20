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
    internal class MachineActivityReportPage : ReportPage
    {

        [FindsBy(How = How.XPath, Using = "(//input)[3]")]
        public virtual IWebElement DeviceSelect { get; set; }


        public MachineActivityReportPage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }



        //returns a MachineActivityRecord for a row in the table
        public MachineActivityRecord GetRecordByIndex(int index)
        {
            waitForTable();

            int count = getRecordCount();
            if (index > count || index < 0 || count == 0)
            {
                return null;
            }

            IWebElement targetRow = Table.FindElement(By.XPath(".//tbody/tr[" + (index + 1) + "]"));
            MachineActivityRecord record = new MachineActivityRecord();

            record.Date = DateTime.Parse(targetRow.FindElement(By.XPath(".//td[1]")).Text);
            record.SiteId = int.Parse(targetRow.FindElement(By.XPath(".//td[2]")).Text);
            record.AgentNumber = int.Parse(targetRow.FindElement(By.XPath(".//td[3]")).Text);
            record.SiteName = targetRow.FindElement(By.XPath(".//td[4]")).Text;
            record.DeviceId = int.Parse(targetRow.FindElement(By.XPath(".//td[5]")).Text);
            record.Activity = targetRow.FindElement(By.XPath(".//td[6]")).Text;
            record.LotteryGameId = int.Parse(targetRow.FindElement(By.XPath(".//td[7]")).Text);
            record.TicketPrice = double.Parse(targetRow.FindElement(By.XPath(".//td[8]")).Text, System.Globalization.NumberStyles.Currency);
            record.GameName = targetRow.FindElement(By.XPath(".//td[9]")).Text;
            record.DrawerNumber = targetRow.FindElement(By.XPath(".//td[10]")).Text;

            return record;
        }


        //returns true if device error banner is being displayed or false otherwise
        public bool DeviceErrorIsDisplayed()
        {
            return ErrorIsDisplayed("DeviceId");
        }



        //selects a device in the device dropdown by device ID value
        public void SelectDeviceByValue(int deviceId)
        {
            //wait for device select to be visible
            wait.Until(d => d.FindElement(By.XPath("(//input)[3]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[3]/following-sibling::span")));
            dropdownButton.Click();

            //get options in dropdown list
            IWebElement list = GetDropdDownList();
            if (list != null)
            {

                try
                {
                    IWebElement target = list.FindElement(By.XPath(".//li[@data-value='" + deviceId.ToString() + "']"));
                    target.Click();
                    waitForDropdownClose();
                }
                //deviceId not in list
                catch (Exception ex)
                {
                    dropdownButton.Click();
                    waitForDropdownClose();
                }
            }
            //list is empty or a site hasnt been selected yet;
            else
            {
                dropdownButton.Click();
                waitForDropdownClose();

            }

        }



        //selects a device from device dropdown by position in the list
        public void SelectDeviceByIndex(int index)
        {

            //wait for device select to be visible
            wait.Until(d => d.FindElement(By.XPath("(//input)[3]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[3]/following-sibling::span")));
            dropdownButton.Click();

            //get options in dropdown list
            IWebElement list = GetDropdDownList();
            if (list == null)
            {
                dropdownButton.Click();
                waitForDropdownClose();
            }

            //index is out of range
            IList<IWebElement> options = list.FindElements(By.TagName("li"));
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
            //catch element not interactable exception
            catch (Exception e)
            {
                dropdownButton.Click();
            }

            waitForDropdownClose();
        }



        //gets the ID of the currently selected device
        public int GetSelectedDevice()
        {

            //wait for device select to be visible
            wait.Until(d => d.FindElement(By.XPath("(//input)[3]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[3]/following-sibling::span")));
            dropdownButton.Click();

            //get options in dropdown list
            IWebElement list = GetDropdDownList();
            if (list != null)
            {
                IList<IWebElement> options = list.FindElements(By.TagName("li"));
                for (int i = 0; i < options.Count; i++)
                {
                    string className = options[i].GetAttribute("class");
                    if (className.Contains("e-active"))
                    {
                        int deviceId = int.Parse(options[i].Text);
                        options[i].Click();
                        waitForDropdownClose();

                        return deviceId;
                    }
                }

                //nothing selected in list
                dropdownButton.Click();
                waitForDropdownClose();

                return -1;
            }
            //list was empty or a site wasn't selected yet
            else
            {
                dropdownButton.Click();
                waitForDropdownClose();

                return -1;
            }
        }



        //returns true if the deviceId is found in the dropdown or false if it doesn't
        public bool DeviceOptionExists(int deviceId)
        {
            //wait for device select to be visible
            wait.Until(d => d.FindElement(By.XPath("(//input)[3]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[3]/following-sibling::span")));
            dropdownButton.Click();

            //get options in dropdown list
            IWebElement list = GetDropdDownList();

            //dropdown was empty or a site wasn't selected yet
            if (list == null)
            {
                dropdownButton.Click();
                waitForDropdownClose();
            }

            try
            {
                IWebElement target = list.FindElement(By.XPath(".//li[@data-value='" + deviceId.ToString() + "']"));
                dropdownButton.Click();
                waitForDropdownClose();
                return true;
            }
            //deviceId not in list
            catch (Exception ex)
            {
                dropdownButton.Click();
                waitForDropdownClose();
                return false;
            }

        }



        //returns a list of integers containing all values in the device dropdown
        public List<int> GetDeviceOptions()
        {
            //wait for device select to be visible
            wait.Until(d => d.FindElement(By.XPath("(//input)[3]")));

            //click dropdown button
            IWebElement dropdownButton = wait.Until(d => d.FindElement(By.XPath("(//input)[3]/following-sibling::span")));
            dropdownButton.Click();

            //get options in dropdown list
            IWebElement list = GetDropdDownList();

            List<int> devices = new List<int>();

            //dropdown was empty or a site wasn't selected yet
            if (list == null)
            {
                dropdownButton.Click();
                waitForDropdownClose();
                return devices;
            }

            IList<IWebElement> options = list.FindElements(By.TagName("li"));

            if (options.Count == 0)
            {
                return devices;
            }

            for (int i = 0; i < options.Count; i++)
            {
                int option = int.Parse(options[i].Text);
                devices.Add(option);
            }

            return devices;
        }



        //determines if excel file is in downlaods folder after pressing excel export
        protected override bool FileDownloaded(string extension)
        {
            string downloadPath = ConfigurationManager.AppSettings["DownloadPath"];

            DirectoryInfo directory = new DirectoryInfo(downloadPath);
            FileInfo[] files = directory.GetFiles(@"Machine Activity Report_*." + extension);

            return files.Length > 0;

        }




    }
}
