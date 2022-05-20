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
    internal class DeviceListPage : TablePage
    {

        //override table property to use this pages table id
        [FindsBy(How = How.Id, Using = "tblDeviceList")]
        private IWebElement table;
        public override IWebElement Table { get { return table; } }
        [FindsBy(How = How.Name, Using = "tblDeviceList_length")]

        //override table length select
        private IWebElement tableLengthSelect;
        public override IWebElement TableLengthSelect { get { return tableLengthSelect; } }

        [FindsBy(How = How.ClassName, Using = "add-bt")]
        private IWebElement AddDeviceButton;

        public AddDeviceModal AddDeviceModal { get; set; }
        public EditDeviceModal EditDeviceModal { get; set; }

        public SiteNavMenu EntityTabs { get; set; }

        public DeviceListPage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            AddDeviceModal = new AddDeviceModal(driver);
            EditDeviceModal = new EditDeviceModal(driver);
            EntityTabs = new SiteNavMenu(driver);
        }



        public override void waitForFilter()
        {
            wait.Until(driver => driver.FindElement(By.Id("tblDeviceList_processing")).GetAttribute("style").Contains("none"));
        }


        public void ClickAddDevice()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("add-bt")));
            AddDeviceButton.Click();
        }


        public void AddDevice()
        {
            ClickAddDevice();
            this.AddDeviceModal.WaitForModal();
            this.AddDeviceModal.ClickSubmit();
            this.AddDeviceModal.waitForModalClose();

        }


        public void ClickEditDevice(int index)
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

            try
            {
                IWebElement editDevice = cols[cols.Count - 1].FindElement(By.ClassName("editDevice"));

                editDevice.Click();
            }
            catch (Exception ex)
            {

            }
        }


        public DeviceTableRecord GetDeviceByIndex(int index)
        {
            waitForTable();

            int count = getRecordCount();

            if (count == 0)
            {
                return null;
            }

            if (index < 0 || index > count)
            {
                return null;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            DeviceTableRecord device = new DeviceTableRecord();
            device.DeviceId = int.Parse(cols[0].Text);
            device.SiteId = int.Parse(cols[1].Text);
            device.SerialNumber = cols[2].Text;
            device.Version = cols[3].Text;
            device.IsActive = cols[cols.Count - 2].Text == "No" ? false : true;

            return device;
        }


        public bool DeviceIsActive(int index)
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
                bool isActive = cols[cols.Count - 2].Text == "No" ? false : true;
                return isActive;
            }
            catch (Exception ex)
            {
                return false;
            }


        }


        public string GetSerialNumber(int index)
        {
            waitForTable();

            int count = getRecordCount();

            if (count == 0)
            {
                return null;
            }

            if (index < 0 || index > count)
            {
                return null;
            }

            IWebElement body = Table.FindElement(By.TagName("tbody"));
            IList<IWebElement> rows = body.FindElements(By.TagName("tr"));
            IWebElement targetRow = rows[index];

            IList<IWebElement> cols = targetRow.FindElements(By.TagName("td"));

            try
            {
                string serial = cols[2].Text;
                return serial;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public void ClickDevice(int index)
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

            try
            {
                IWebElement editDevice = cols[0].FindElement(By.ClassName("editDevice"));

                editDevice.Click();
            }
            catch (Exception ex)
            {

            }
        }


        //returns index of device in list by the device iD
        public int IndexOfDevice(int deviceId)
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
                string id = cols[0].Text;

                if (id == deviceId.ToString())
                {

                    return index;
                }

                index++;
            }

            return -1;

        }
    }
}
