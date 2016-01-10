using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCrawler.Browser.Chromium;
using WebCrawler.Browser;

namespace WebCrawler.Tests
{
    [TestClass]
    public class BrowserTests
    {
        private IBrowser browser;

        public BrowserTests()
        {
            browser = new ChromiumBrowserFactory().Create();
        }

        [TestMethod]
        public void read_attribute()
        {
            browser.Navigate(new Uri("http://google.ro")).Wait();
            Assert.AreEqual(browser.Page.Root.Query("#hplogo").GetAttribute("align"), "left");
        }
    }
}
