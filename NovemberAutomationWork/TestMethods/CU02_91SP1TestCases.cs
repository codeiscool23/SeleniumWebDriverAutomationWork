using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using WorkareaAutomation.PageObjects;

namespace CUVerification
{
    [TestClass]
    public class Cu02_91sp1TestCases : TestCaseBase
    {

        [TestMethod]
        [TestCategory("9.1SP1"), TestCategory("CU2"), TestCategory("Content")]
        [Description(
            "Escalation 11327 Add a / to a content block title"
            )]
        public void ContentPublish_AllowSlashesInContentTitle()
        {
            try
            {
                this.loginAndOpenWorkarea(this.adminUserName, this.adminUserPassword);

                workareaNavigation.NavigateToContent();
                workareaContentNavTree.NavigateToRoot();

                this.mainContentPage.AddContent(false)
                            .EnterTitle("adding a \\ slash and a / slash!")
                            .Save();
                
                //verify content was added
                workareaNavigation.NavigateToContent();
                workareaContentNavTree.NavigateToRoot();
              
                 this.workareaFrames.SwitchToMainWindow();

                 this.mainContentPage
                     .SelectContentItem("adding a \\ slash and a / slash!");
            }
            catch
            {
                Assert.Fail("Failed to add a content with a / slash");
            }

            finally
            {
                this.TearDown();

                this.Setup();
                this.loginAndOpenWorkarea(this.adminUserName, this.adminUserPassword);
                workareaNavigation.NavigateToContent();
                workareaContentNavTree.NavigateToRoot();
                this.workareaFrames.SwitchToMainWindow();

                this.mainContentPage
                       .SelectContentItem("adding a \\ slash and a / slash!")
                       .Delete();
            }
        }

    }
}
