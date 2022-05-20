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

namespace EasyVend_Setup_Scripts
{
    public class DriverFactoryTest
    {
        string baseUrl;


        [SetUp]
        public void Setup()
        {
            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

        }

        [Test]
        public void TestInitDriver()
        {
            DriverFactory.GoToUrl(baseUrl);
        }

        [TearDown]
        public void EndTest()
        {
            DriverFactory.CloseDriver();
        }
    }
}