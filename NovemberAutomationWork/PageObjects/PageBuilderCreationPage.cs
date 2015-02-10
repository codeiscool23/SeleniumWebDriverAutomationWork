using WorkareaAutomation.Helpers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkareaAutomation.PageObjects
{
    public class PageBuilderCreationPage : PollingElementFinder
    {
        private readonly WorkareaFrames workareaFrames;
        private readonly IBrowserHelper browserHelper;
        
        public PageBuilderCreationPage(IWebDriver browser, int pollTimeOut, WorkareaFrames workareaFrames, IBrowserHelper browserHelper) 
            : base(browser, pollTimeOut) 
        {
            this.workareaFrames = workareaFrames;
            this.browserHelper = browserHelper;
        }
        
        private IWebElement nextButton { get { return this.Find(By.Id("ektronPageBuilderNext")); } }
        private IWebElement titleInput { get { return this.Find(By.Name("pageBuilderWizardPageTitle")); } }
        private IWebElement finishButton { get { return this.Find(By.Id("ektronPageBuilderFinish")); } }
        private IWebElement ok { get { return this.Find(By.Id("ektronPageBuilderFinish")); } }
        private IWebElement okAndEdit { get { return this.Find(By.LinkText("OK")); } }
        private IWebElement cancelPageEdit { get { return this.Find(By.Id("ektronPageBuilderCancel")); } }

        public string Title { 
            set
            {
                this.titleInput.Clear();
                this.titleInput.SendKeys(value);
            }
        }

        public PageBuilderCreationPage Next()
        {
            this.browserHelper.WaitForJavascriptExecution();
            this.nextButton.Click();
            return this;
        }

        public PageBuilderCreationPage EnterTitle(string pageTitle)
        {
            this.browserHelper.WaitForJavascriptExecution();
            this.workareaFrames.SwitchToPageBuilderCreation();
            this.Title = pageTitle;
            this.workareaFrames.SwitchToMainWindow();
            return this;
        }


        public PageBuilderCreationPage Finish()
        {
            this.browserHelper.WaitForJavascriptExecution();
            this.finishButton.Click();
            
            return this;
        }

        

        public PageBuilderCreationPage CancelGoToPageEdit()
        {
            this.cancelPageEdit.Click();
            
            return this;
        }

        public string GoToPageEdit(bool returnWindowTitleString = false)
        {
            string result = string.Empty;
            if (returnWindowTitleString)
            {
                result = this.browser.CurrentWindowHandle;
            }

            this.okAndEdit.Click();
            return result;
        }
    }
}
