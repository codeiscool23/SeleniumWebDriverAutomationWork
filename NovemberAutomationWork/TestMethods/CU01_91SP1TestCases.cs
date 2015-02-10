using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using WorkareaAutomation.PageObjects;


namespace CUVerification
{
    [TestClass]
    public class Cu01_91sp1TestCases : TestCaseBase
    {
        [TestMethod]
        [TestCategory("9.1SP1"), TestCategory("CU1"), TestCategory("Pagebuilder"), TestCategory("Widgets"),
         TestCategory("Pagination")]
        [Description(
            "Task 9992 verifies TT# 74588: Content Widget Pagination in Edit View"
            )]
        public void PaginationShouldAppearOnContentBlockWidget()
        {
            const string folderName = "AutomationTest9992";
           
            sqlHelper.BackupSiteDatabase(this.siteDataBase, this.siteConnectionString);
            try
            {
                siteHelper.CopyTemplateToSite("PageLayout.aspx",
                    Path.Combine(this.assemblyPath, "Test Resources", "Test Pages"), this.siteRoot)
                    .CopyTemplateToSite("PageLayout.aspx.cs",
                        Path.Combine(this.assemblyPath, "Test Resources", "Test Pages"), this.siteRoot);
                siteHelper.CopyTemplateToSite("AutomationTest9992.aspx",
                    Path.Combine(this.assemblyPath, "Test Resources", "Test Pages"), this.siteRoot)
                    .CopyTemplateToSite("AutomationTest9992.aspx.cs",
                        Path.Combine(this.assemblyPath, "Test Resources", "Test Pages"), this.siteRoot);


                Userlogin(this.adminUserName, this.adminUserPassword);
                browser.Navigate().GoToUrl(string.Format("{0}/{1}", siteUrl, "workarea/workarea.aspx"));
                workareaNavigation.NavigateToSettings();
                workareaFrames.SwitchToSettingsFoldersTree();
                settingsNavTree.GoToConfiguration()
                .Personalizations()
                .Widgets();
                workareaFrames.SwitchToMainWindow();
                settingsNavTree.SynchonizeWidgetsFolder();

                if (settingsNavTree.isWidgetSynchronizationAlertPresent(browser).Equals(true))
                {
                    IAlert alert = this.browser.SwitchTo().Alert();
                    alert.Accept();

                }
                
                

                windowManager.RefocusCurrentWindow();

                browser.Navigate().GoToUrl(string.Format("{0}/{1}", siteUrl, "workarea/login.aspx"));
              
              

                browser.Navigate().GoToUrl(string.Format("{0}/{1}", siteUrl, "AutomationTest9992.aspx"));


                browser.Navigate().GoToUrl(string.Format("{0}/{1}", siteUrl, "workarea/workarea.aspx"));

                windowManager.RefocusCurrentWindow();
                this.browserHelper.WaitForJavascriptExecution(5);
         
                
                workareaNavigation.NavigateToContent();
                workareaContentNavTree.NavigateToRoot();
                this.browser.Manage().Window.Maximize();
                workareaContentNavTree.NavigateToRoot();
                workareaContentNavTree.NavigateToChildFolder(folderName);
                mainContentPage.AddPageBuilderPage();

                mainContentPage.AddPageBuilderPage()
                    .Next()
                    .EnterTitle("MyTestPageLayout")
                    .Next()
                    .Finish()
                    .GoToPageEdit(true);

                windowManager.SwitchToWindowByTitle("Page Builder Sample");

                pageBuilderPageLayoutNavigation.toolbarDesignButtonClick()
                    .widgetAccordionClick()
                    .firstWidgetSelectClick()
                    .firstDropZoneClick()
                    .toolbarContentButtonClick()
                    .toolbarDesignButtonClick()
                    .closeTrayIconClick()
                    .editContentBlockWidgetClick()
                    .contentBlockWidgetFolderViewFirstFolderClick();

                
                bool isPaginationPresentOnContentBlock =
                    pollingElementFinder.Find(By.CssSelector("#CBPaging > a:nth-child(1)")).Displayed;

                Assert.IsTrue(isPaginationPresentOnContentBlock,
                    "When a content block widget has a selected folder with more than 10 item pagination is available.");
            }
            finally
            {
                siteHelper.RemoveTemplateFromSite("PageLayout.aspx", this.siteRoot)
                    .RemoveTemplateFromSite("PageLayout.aspx.cs", this.siteRoot);
                siteHelper.RemoveTemplateFromSite("AutomationTest9992.aspx", this.siteRoot)
                    .RemoveTemplateFromSite("AutomationTest9992.aspx.cs", this.siteRoot);

            }
            sqlHelper.RestoreSiteDatabase(this.siteDataBase, this.siteConnectionString);

        }

        [TestMethod]
        [TestCategory("9.1SP1"), TestCategory("CU1"), TestCategory("IE"), TestCategory("Console Error"),
         TestCategory("DMS")]
        [Description(
            "Task 10173 verifies TTPro# 74584 - JS Error using Community upload file in the French CA language"
        // "In prior versions of the F12 tools, output directed to the Console was not recorded during a browsing 
        //session until the console had been opened. For developers who need the console to record messages at all times,
        //Windows 8.1 Update adds the Always record developer console messages option. To find the option: 
        //Open the Internet Options window from the Tools menu.On the Advanced tab, in the Browsing section, you'll see Always record developer console messages."

            )]
        public void NoJSErrorInConsoleForFrenchCanadianUser()
        {
            windowManager.CloseCurrentWindow();

           // sqlHelper.BackupSiteDatabase(this.siteDataBase, this.siteConnectionString);
            var IEdriver = Path.Combine(this.assemblyPath, "packages", "WebDriver.IEDriver.2.29.1.1", "tools");
            browser = new InternetExplorerDriver(IEdriver);
            windowManager = new WindowManager(browser);
       
            try
            {
                siteHelper.CopyTemplateToSite("AutomationTest10173.aspx",
                    Path.Combine(this.assemblyPath, "Test Resources", "Test Pages"), this.siteRoot)
                    .CopyTemplateToSite("AutomationTest10173.aspx.cs",
                        Path.Combine(this.assemblyPath, "Test Resources", "Test Pages"), this.siteRoot);

                browser.Navigate().GoToUrl(string.Format("{0}/{1}", siteUrl, "AutomationTest10173.aspx"));


                browser.Navigate()
                    .GoToUrl(string.Format("{0}/{1}", siteUrl, "workarea/Community/ManageCommunityDocuments.aspx#tabMultipleDMS"));


                var winName = windowManager.CurrentWindow;
                windowManager.SwitchToWindow(winName);

                var js1 = browser as IJavaScriptExecutor;
                js1.ExecuteScript("document.getElementById('rbl_OfficeVersion_0').checked = true;");
                js1.ExecuteScript("document.getElementById('liMultipleDMS').display = 'block';");

                js1.ExecuteScript("(function (global) {var preExistingHandler = global.onerror;var errors = [];" +
              "var errorHandler = function (errorMsg, url, lineNumber) {if (preExistingHandler) {preExistingHandler.call(null, errorMsg, url, lineNumber);}errors.push(errorMsg);return false;};" +
                  "global.testingLogger = {};global.onerror = errorHandler;global.testingLogger.getErrors = function () {return errors;};}(window));");

                js1.ExecuteScript("document.querySelectorAll('input[name=btn_VersionSelect]')[0].click();");
               
                var pageErrors = js1.ExecuteScript("window.testingLogger.getErrors();");
                this.browserHelper.WaitForJavascriptExecution(10);
          
             Assert.IsNull(pageErrors);


            }
            finally
            {
                siteHelper.RemoveTemplateFromSite("AutomationTest10173.aspx", this.siteRoot)
                   .RemoveTemplateFromSite("AutomationTest10173.aspx.cs", this.siteRoot);
            }
            // sqlHelper.RestoreSiteDatabase(this.siteDataBase, this.siteConnectionString);

        }
    
    }
}
