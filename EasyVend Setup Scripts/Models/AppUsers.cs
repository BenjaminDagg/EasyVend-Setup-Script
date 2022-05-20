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
using System.Threading;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class User
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    internal class AppUsers
    {
        public static User VENDOR_ADMIN = new User(ConfigurationManager.AppSettings["VendorAdminUsername"],
            ConfigurationManager.AppSettings["VendorAdminPassword"]);

        public static User VENDOR_REPORT = new User(ConfigurationManager.AppSettings["VendorReportUsername"],
            ConfigurationManager.AppSettings["VendorReportPassword"]);

        public static User LOTTERY_ADMIN = new User(ConfigurationManager.AppSettings["LotteryAdminUsername"],
            ConfigurationManager.AppSettings["LotteryAdminPassword"]);

        public static User LOTTERY_REPORT = new User(ConfigurationManager.AppSettings["LotteryReportUsername"],
            ConfigurationManager.AppSettings["LotteryReportPassword"]);

        public static User SITE_ADMIN = new User(ConfigurationManager.AppSettings["SiteAdminUsername"],
            ConfigurationManager.AppSettings["SiteAdminPassword"]);

        public static User SITE_REPORT
        {
            get
            {
                ConfigurationManager.RefreshSection("appSettings");

                User siteReport = new User(ConfigurationManager.AppSettings["SiteReportUsername"],
                             ConfigurationManager.AppSettings["SiteReportPassword"]);
                return siteReport;
            }
        }




        public static void RefreshSettings()
        {


            VENDOR_ADMIN.Username = ConfigurationManager.AppSettings["VendorAdminUsername"];
            VENDOR_ADMIN.Password = ConfigurationManager.AppSettings["VendorAdminPassword"];

            VENDOR_REPORT.Username = ConfigurationManager.AppSettings["VendorReportUsername"];
            VENDOR_REPORT.Password = ConfigurationManager.AppSettings["VendorReportPassword"];

            LOTTERY_ADMIN.Username = ConfigurationManager.AppSettings["LotteryAdminUsername"];
            LOTTERY_ADMIN.Password = ConfigurationManager.AppSettings["LotteryAdminPassword"];

            LOTTERY_REPORT.Username = ConfigurationManager.AppSettings["LotteryReportUsername"];
            LOTTERY_REPORT.Password = ConfigurationManager.AppSettings["LotteryReportPassword"];

            SITE_ADMIN.Username = ConfigurationManager.AppSettings["SiteAdminUsername"];
            SITE_ADMIN.Password = ConfigurationManager.AppSettings["SiteAdminPassword"];

            SITE_REPORT.Username = ConfigurationManager.AppSettings["SiteReportUsername"];
            SITE_REPORT.Password = ConfigurationManager.AppSettings["SiteReportUsername"];
        }
    }
}
