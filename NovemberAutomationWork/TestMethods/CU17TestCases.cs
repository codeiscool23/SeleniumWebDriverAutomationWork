using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Threading;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using WorkareaAutomation.Helpers;
using WorkareaAutomation.PageObjects;
using WorkareaAutomation;

namespace CUVerification
{
    [TestClass]
    public class CU17TestCases : TestCaseBase
    {
        [TestMethod]
        [TestCategory("CU17"), TestCategory("Settings"), TestCategory("User Management"), TestCategory("Min Site")]
        [Description("Automates Task-8201/Verifies TTPro #73873: System changes builtin acct password when edit-update setup")]
        public void EditUpdateSetupAsBuiltin_ShouldNotResetBuiltinPassword()
        {
            //sqlHelper.BackupSiteDatabase(this.siteDataBase,this.siteConnectionString);
          loginAndOpenWorkarea(this.builtinUserName, this.builtinUserPassword);
 
            workareaFrames.SwitchToSettingsFoldersTree();
            settingsNavTree.GoToConfigurationAsBuiltin();
            settingsNavTree.GoToSetup();
            workareaFrames.SwitchToMainWindow();
            setupPage.ClickEdit();
            setupPage.ClickUpdate();
            browserHelper.WaitForJavascriptExecution(5);
            windowManager.CloseCurrentWindow();
            windowManager.SwitchToWindowByTitle("Login");
   
           var winName = windowManager.CurrentWindow;
           loginPage.LogOut();
           
            List<string> handles = browser.WindowHandles.ToList<string>();
            browser.SwitchTo().Window(handles.Last());
          
            loginPage.ConfirmLogOut();
            windowManager.SwitchToWindow(winName);
            windowManager.RefocusCurrentWindow();


            loginAndOpenWorkarea(this.builtinUserName, this.builtinUserPassword);
           
         //   // sqlHelper.RestoreSiteDatabase(this.siteDataBase, this.siteConnectionString);

        }
    }
}
