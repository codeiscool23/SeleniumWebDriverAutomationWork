using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WorkareaAutomation.Helpers;

namespace WorkareaAutomation.PageObjects
{
    public class CommunityGroupsPage : PollingElementFinder 
    {
        private readonly IBrowserHelper browserHelper;
        private string _alertTextForSaveNewCommunityGroup;

        /// <summary>
        /// Constructor that allows for overriding the time to wait for an element to be found on a web page.
        /// </summary>
        /// <param name="browser">An instance of a Selenium Web Driver, e.g. FireFoxDriver, which allows interaction with the browser.</param>
        /// <param name="pollTimeOut">How long you want to wait for elements on the web page to be found.</param>
        /// <param name="browserHelper">An instance of <see cref="IBrowserHelper"/> as this class knows of certain times the community groups page needs to wait for javascript exectution.</param>
        public CommunityGroupsPage(IWebDriver browser, int pollTimeOut, IBrowserHelper browserHelper) 
            : base(browser, pollTimeOut) 
        {
            this.browserHelper = browserHelper;
        }

        private IWebElement addNewCommunityGroupButton { get { return this.Find(By.Id("image_link_100")); } }
        private IWebElement saveNewCommunityGroupButton { get { return this.Find(By.Id("image_link_101")); } }

        /// <summary>
        /// Click "Add" on the community group page.
        /// </summary>
        /// <returns>The current instance of <see cref="CommunityGroupsPage"/> , this means state is persisted between calls.</returns>
        public CommunityGroupsPage AddNewCommunityGroup()
        {
            this.addNewCommunityGroupButton.Click();

            return this;
        }

        /// <summary>
        /// Click "Save" on the community group page.
        /// </summary>
        /// <returns>The current instance of <see cref="CommunityGroupsPage"/> , this means state is persisted between calls.</returns>
        public CommunityGroupsPage SaveNewCommunityGroup()
        {
            this.saveNewCommunityGroupButton.Click();
            if(isAlertPresent())
            {
                this._alertTextForSaveNewCommunityGroup = this.closeAlertAndGetItsText();
            }
            return this;
        }

        public string AlertTextForSaveNewCommunityGroup { get { return this._alertTextForSaveNewCommunityGroup; } }
    }
}
