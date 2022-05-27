using NUnit.Framework;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

using Excel = Microsoft.Office.Interop.Excel;

namespace EasyVend_Setup_Scripts
{
    public enum ExcelUserColumn : int
    {
        Email = 1,
        FirstName = 2,
        LastName = 3,
        Phone = 4,
        Role = 5,
        SiteName = 6
    }

    public enum ExcelSiteColumn : int
    {
        Name = 1,
        AgentNumber = 2,
        ExternalStoreId = 3,
        FirstName = 4,
        LastName = 5,
        Email = 6,
        Phone = 7,
        Address = 8,
        City = 9,
        Zipcode = 10,
        DeviceCount = 11
    }

    public enum ExcelDeviceColumn : int
    {
        SiteName = 1,
        SerialNumber = 2,
        ExternalTerminalId1 = 3,
        ExternalTerminalId2 = 4,
        ExternalTerminalId3 = 5,
        ExternalTerminalId4 = 6,
    }

    public enum ExcelSheets : int
    {
        Users = 1,
        Sites = 2,
        Devices = 3
    }

    class ExcelReader
    {
        Excel.Application xlApp;
        Excel.Workbook wb;
        Excel.Worksheet ws;
        Excel.Range range;

        public ExcelReader(string filePath, int sheet)
        {
            xlApp = new Excel.Application();
            wb = xlApp.Workbooks.Open(filePath);
            ws = wb.Sheets[sheet];
            range = ws.UsedRange;
        }


        public void SetSheet(int sheetNum)
        {

            ws = (Excel.Worksheet)wb.Sheets[sheetNum];
            ws.Select();
            range = ws.UsedRange;
        }


        public UserTableRecord GetUserByRole(string role)
        {

            int count = RowCount();
            
            for (int i = 2; i <= count; i++)
            {
                string userRole = range.Cells[i, (int)ExcelUserColumn.Role].Value2.ToString();

                if(userRole == role)
                {
                    string email = range.Cells[i, (int)ExcelUserColumn.Email].Value2.ToString();
                    string fName = range.Cells[i, (int)ExcelUserColumn.FirstName].Value2.ToString();
                    string lName = range.Cells[i, (int)ExcelUserColumn.LastName].Value2.ToString();
                    string phone = range.Cells[i, (int)ExcelUserColumn.Phone].Value2.ToString();

                    UserTableRecord user = new UserTableRecord();
                    user.Username = email;
                    user.FirstName = fName;
                    user.LastName = lName;
                    user.Phone = phone;
                    user.Role = role;

                    return user;
                }
            }

            return new UserTableRecord();
        }


        public List<UserTableRecord> GetUsers()
        {
            List<UserTableRecord> users = new List<UserTableRecord>();
            int count = RowCount();
            

            for (int i = 2; i <= range.Rows.Count; i++)
            {
                try
                {
                    string email = range.Cells[i, (int)ExcelUserColumn.Email].Value2.ToString();
                    string fName = range.Cells[i, (int)ExcelUserColumn.FirstName].Value2.ToString();
                    string lName = range.Cells[i, (int)ExcelUserColumn.LastName].Value2.ToString();
                    string phone = range.Cells[i, (int)ExcelUserColumn.Phone].Value2.ToString();
                    string userRole = range.Cells[i, (int)ExcelUserColumn.Role].Value2.ToString();

                    UserTableRecord user = new UserTableRecord();
                    user.Username = email;
                    user.FirstName = fName;
                    user.LastName = lName;
                    user.Phone = phone;
                    user.Role = userRole;

                    users.Add(user);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return users;
        }


        public List<UserTableRecord> GetUsersByRole(string role)
        {
            SetSheet((int)ExcelSheets.Users);
            List<UserTableRecord> users = new List<UserTableRecord>();
            int count = RowCount();
            

            for (int i = 2; i <= range.Rows.Count; i++)
            {
                try
                {
                    string email = range.Cells[i, (int)ExcelUserColumn.Email].Value2.ToString();
                    string fName = range.Cells[i, (int)ExcelUserColumn.FirstName].Value2.ToString();
                    string lName = range.Cells[i, (int)ExcelUserColumn.LastName].Value2.ToString();
                    string phone = range.Cells[i, (int)ExcelUserColumn.Phone].Value2.ToString();
                    string userRole = range.Cells[i, (int)ExcelUserColumn.Role].Value2.ToString();

                    UserTableRecord user = new UserTableRecord();
                    user.Username = email;
                    user.FirstName = fName;
                    user.LastName = lName;
                    user.Phone = phone;
                    user.Role = userRole;

                    users.Add(user);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return users.Where(user => user.Role == role).ToList();
        }


        public List<SiteTableRecord> GetSites()
        {
            SetSheet((int)ExcelSheets.Sites);

            List<SiteTableRecord> sites = new List<SiteTableRecord>();
            int recordCount = ws.Cells.Find(
                What: "*",
                SearchOrder: Excel.XlSearchOrder.xlByRows,
                SearchDirection: Excel.XlSearchDirection.xlPrevious,
                MatchCase: false
            ).Row;
            
            if(recordCount == 1)
            {
                return sites;
            }
            
            for(int i = 2; i <= recordCount; i++)
            {
                
                    if (ws.Cells[i, 1].Value2 == null)
                    {
                        continue;
                    }

                    SiteTableRecord site = new SiteTableRecord();

                    string name = range.Cells[i, (int)ExcelSiteColumn.Name].Value2.ToString();
                    var AgentNum = range.Cells[i, (int)ExcelSiteColumn.AgentNumber].Value2.ToString().ToString();
                    string ExternalStoreId = range.Cells[i, (int)ExcelSiteColumn.ExternalStoreId].Value2;
                    string fName = range.Cells[i, (int)ExcelSiteColumn.FirstName].Value2.ToString();
                    string lName = range.Cells[i, (int)ExcelSiteColumn.LastName].Value2.ToString();
                    string email = range.Cells[i, (int)ExcelSiteColumn.Email].Value2.ToString();
                    var Phone = range.Cells[i, (int)ExcelSiteColumn.Phone].Value2.ToString();
                    string Address = range.Cells[i, (int)ExcelSiteColumn.Address].Value2.ToString();
                    string City = range.Cells[i, (int)ExcelSiteColumn.City].Value2.ToString();
                    var zip = range.Cells[i, (int)ExcelSiteColumn.Zipcode].Value2.ToString();
                    int deviceCount = 0;

                    
                    //add devices
                    if (range.Cells[i, (int)ExcelSiteColumn.DeviceCount].Value != null)
                    {
                        deviceCount = int.Parse(range.Cells[i, (int)ExcelSiteColumn.DeviceCount].Value.ToString());
                    }
                    List<DeviceTableRecord> devices = GetDevices(name);
                    site.Devices = devices;
                    SetSheet((int)ExcelSheets.Sites);
                    
                    site.SiteName = name;
                    site.AgentNumber = AgentNum;
                    site.ExternalStoreId = ExternalStoreId;
                    site.FirstName = fName;
                    site.LastName = lName;
                    site.Email = email;
                    site.Phone = Phone;
                    site.Address = Address;
                    site.City = City;
                    site.Zipcode = zip;
                    site.DeviceCount = deviceCount;

                    foreach(DeviceTableRecord device in site.Devices)
                    {
                        Console.WriteLine(site.SiteName + "Devices: " + device.SerialNumber);
                    }

                    sites.Add(site);
                }
                


            

            return sites;
        }


        public List<UserTableRecord> GetSiteUsers()
        {
            SetSheet((int)ExcelSheets.Users);

            List<UserTableRecord> users = new List<UserTableRecord>();

            int count = RowCount();
       
            for (int i = 2; i <= count; i++)
            {
                if(range.Cells[i,ExcelUserColumn.SiteName].Value == null || range.Cells[i, ExcelUserColumn.SiteName].Value == "")
                {
                    continue;
                }

                string name = range.Cells[i,ExcelUserColumn.Email].Value.ToString();
                string fName = range.Cells[i, ExcelUserColumn.FirstName].Value.ToString();
                string lName = range.Cells[i, ExcelUserColumn.LastName].Value.ToString();
                string phone = range.Cells[i, ExcelUserColumn.Phone].Value.ToString();
                string siteName = range.Cells[i, ExcelUserColumn.SiteName].Value.ToString();
                string role = range.Cells[i, ExcelUserColumn.Role].Value.ToString();

                UserTableRecord user = new UserTableRecord();
                user.Username = name;
                user.FirstName = fName;
                user.LastName = lName;
                user.Phone = phone;
                user.SiteName = siteName;
                user.Role = role;

                users.Add(user);
            }

            return users;
        }



        public List<DeviceTableRecord> GetDevices(string siteName)
        {
            SetSheet((int)ExcelSheets.Devices);

            int count = RowCount();
            

            List<DeviceTableRecord> devices = new List<DeviceTableRecord>();

            for(int i = 2; i <= count; i++)
            {
                string site = range.Cells[i, ExcelDeviceColumn.SiteName].Value2.ToString();

                if(site == siteName)
                {
                    string SN;
                    if(range.Cells[i, ExcelDeviceColumn.SerialNumber].Value2 == null || range.Cells[i, ExcelDeviceColumn.SerialNumber].Value2 == "")
                    {
                        SN = null;
                    }
                    else
                    {
                        SN = range.Cells[i, ExcelDeviceColumn.SerialNumber].Value2.ToString();
                    }
                    string terminal1 = range.Cells[i, ExcelDeviceColumn.ExternalTerminalId1].Value2.ToString();
                    string terminal2 = range.Cells[i, ExcelDeviceColumn.ExternalTerminalId2].Value2.ToString();
                    string terminal3 = range.Cells[i, ExcelDeviceColumn.ExternalTerminalId3].Value2.ToString();
                    string terminal4 = range.Cells[i, ExcelDeviceColumn.ExternalTerminalId4].Value2.ToString();

                    string[] terminals = { terminal1, terminal2, terminal3, terminal4 };
                    DeviceTableRecord device = new DeviceTableRecord(site, SN, terminals);
                    device.Display();

                    devices.Add(device);
                }
            }

            return devices;
        }


        public string ReadCell(int rowNum, int colNum, int sheetNum)
        {
            SetSheet(sheetNum);

            if(range.Cells[rowNum, colNum].Value == null)
            {
                return null;
            }

            return range.Cells[rowNum, colNum].Value.ToString();
        }


        public int RowCount()
        {
            int recordCount = ws.Cells.Find(
                What: "*",
                SearchOrder: Excel.XlSearchOrder.xlByRows,
                SearchDirection: Excel.XlSearchDirection.xlPrevious,
                MatchCase: false
            ).Row;

            return recordCount;
        }


        public void Close()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            wb.Close(0);
            xlApp.Quit();

            Marshal.ReleaseComObject(ws);
            Marshal.ReleaseComObject(wb);
            Marshal.ReleaseComObject(xlApp);
        }
    }
}
