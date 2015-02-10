using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Interactions;
using WorkareaAutomation.PageObjects;

namespace CUVerification
{
    [TestClass]
    public class CU03_91SP1TestCases : TestCaseBase
    {
        [TestMethod]
        [TestCategory("9.1SP1"), TestCategory("CU3"), TestCategory("Localization"), TestCategory("Go-Live"),
         TestCategory("Pagination")]
        [Description(
            "Task 11979 verifies Escalation# 11881: French Content Will Go-Live Immediately"
            )]
        public void AddGoLiveFor()
        {
            //  sqlHelper.BackupSiteDatabase(this.siteDataBase, this.siteConnectionString);
            try
            {
               Userlogin(this.adminUserName, this.adminUserPassword);
                browser.Navigate().GoToUrl(string.Format("{0}/{1}", siteUrl, "workarea/workarea.aspx"));
                workareaNavigation.NavigateToContent();
                workareaContentNavTree.NavigateToRoot();
                browser.Manage().Window.Maximize();
                workareaContentNavTree.NavigateToRoot();
                workareaFrames.SwitchToMainWindow();
                
                mainContentPage.ChangeLanguageToFrench();
                mainContentPage.AddContent()
                    .EnterTitle("AutomationTest9992 French Content")
                    .GoToScheduleTab()
                    .ClickSelectStartDataAndTime();
                //get the current window
                var winName = windowManager.CurrentWindow;
                //switch to the dateselector popup window
                windowManager.SwitchToWindowByTitle("Date Time Selector");
                goLiveDateTimeSelector.SetTimeGoLiveTimeTo1255PmTonight();
                //switchback to the cms window
                windowManager.SwitchToWindow(winName);
                windowManager.RefocusCurrentWindow();
                 workareaFrames.SwitchToMainWindow();
                //save the content
                contentPage.Save();
                     windowManager.SwitchToWindow(winName);
                windowManager.RefocusCurrentWindow();
                 workareaFrames.SwitchToMainWindow();
                contentPage.RecentlyUpdateContentProperties();
             //verify that the content is pending. 
               Assert.IsTrue(contentPage.CheckContentPropertiesStatus()); 
              
            }
            finally
            {
              //  sqlHelper.RestoreSiteDatabase(this.siteDataBase, this.siteConnectionString);
            }

        }

        [TestMethod]
        [TestCategory("9.1SP1"), TestCategory("CU3"), TestCategory("Aloha"), TestCategory("Inspector"),
         TestCategory("CustomStyle")]
        [Description(
            "NOT CORRECT_Task 11979 : Add a Custom Style to Aloha"
            )]
        public void CustomStyleForAlohaSeenInInspector()
        {
            //  sqlHelper.BackupSiteDatabase(this.siteDataBase, this.siteConnectionString);
            try
            {
                Userlogin(this.adminUserName, this.adminUserPassword);
                browser.Navigate().GoToUrl(string.Format("{0}/{1}", siteUrl, "workarea/workarea.aspx"));
                workareaNavigation.NavigateToSettings();
                workareaFrames.SwitchToSettingsFoldersTree();
                settingsNavTree.GoToConfiguration();
            }
            finally{}



            //make the style.js be added to the path. 
                //
                //    string sConfig =
                //        "\Workarea\FrameworkUI\js\Ektron\Controls\EktronUI\Editor\Aloha\plugins\ektron\advancedinspector\lib";
                //    siteHelper.CopyTemplateToSite("StyleConfig.js",
                //        Path.Combine(this.assemblyPath, "Test Resources", "Test Pages"), this.siteRoot);
                //      Userlogin(this.adminUserName, this.adminUserPassword);
                //    browser.Navigate().GoToUrl(string.Format("{0}/{1}", siteUrl, "workarea/workarea.aspx"));
                //    workareaNavigation.NavigateToContent();
                //    workareaContentNavTree.NavigateToRoot();
                //    browser.Manage().Window.Maximize();
                //    workareaContentNavTree.NavigateToRoot();
                //    workareaFrames.SwitchToMainWindow();

                //    mainContentPage.AddContent()
                //        .EnterTitle("AutomationTestWHATEVER StyleConfig change");
                //}
                //finally
                //{

                //}
            }
    }

}