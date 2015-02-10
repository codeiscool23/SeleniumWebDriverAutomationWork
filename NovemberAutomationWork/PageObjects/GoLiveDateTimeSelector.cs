using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using WorkareaAutomation.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace WorkareaAutomation.PageObjects
{
    public class GoLiveDateTimeSelector : PollingElementFinder
    {
         private readonly IBrowserHelper browserHelper;


        public GoLiveDateTimeSelector(IWebDriver browser, IBrowserHelper browserHelper)
            : base(browser)
        {
            this.browserHelper = browserHelper;
        }

         public GoLiveDateTimeSelector(IWebDriver browser, int pollTimeOut, IBrowserHelper browserHelper)
            : base(browser, pollTimeOut)
        {
            this.browserHelper = browserHelper;
        }

        private IWebElement DateTimeSelectorHour{get
        {
            return
                this.Find(
                    By.Name("hrSelect"));
        }}
        
        private IWebElement DateTimeSelectorMinute
        {
            get
            {
                return
                    this.Find(
                        By.Name("miSelect"));
               
            }
        }
       
        private IWebElement DateTimeSelectorAbreviation
        {
            get
            {
                return
                    this.Find(
                        By.CssSelector(
                            "body > form > table > tbody > tr:nth-child(2) > td > div:nth-child(2) > table > tbody > tr > td:nth-child(4) > select"));
            }
        }
      
        private IWebElement DateTimeSelectorDone
        {
            get
            {
                return
                    this.Find(
                        By.XPath(
                          
                            "/html/body/form/table/tbody/tr[2]/td/div[3]/input[1]"));

            }
        }

        public void SetTimeGoLiveTimeTo1255PmTonight()
        {
            ReadOnlyCollection<IWebElement> optionsHour = DateTimeSelectorHour.FindElements(By.TagName("option"));
            foreach (var option in optionsHour)
            {
                if (option.Text.Equals("11"))
                {
                    option.Click();
                    break;
                }
            }

            ReadOnlyCollection<IWebElement> optionMin = DateTimeSelectorMinute.FindElements(By.TagName("option"));
            foreach (var option in optionMin)
            {
                if (option.Text.Equals("55"))
                {
                    option.Click();
                    break;
                }
            }

            ReadOnlyCollection<IWebElement> optionAbr = DateTimeSelectorAbreviation.FindElements(By.TagName("option"));
            foreach (var option in optionAbr)
            {
                if (option.Text.Equals("PM"))
                {
                    option.Click();
                    break;
                }
            }
        
            DateTimeSelectorDone.Click();
        
        }


    }
}



