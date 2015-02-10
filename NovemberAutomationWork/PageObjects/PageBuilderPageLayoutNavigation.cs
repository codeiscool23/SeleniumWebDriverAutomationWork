using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkareaAutomation.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace WorkareaAutomation.PageObjects
{
    public class PageBuilderPageLayoutNavigation : PollingElementFinder
    {

        private readonly IBrowserHelper browserHelper;

        public PageBuilderPageLayoutNavigation(IWebDriver browser, IBrowserHelper browserHelper)
            : base(browser)
        {
            this.browserHelper = browserHelper;
        }

        public PageBuilderPageLayoutNavigation(IWebDriver browser, int pollTimeOut, IBrowserHelper browserHelper)
            : base(browser, pollTimeOut)
        {
            this.browserHelper = browserHelper;
        }

        private IWebElement toolbarContentButton { get { return this.Find(By.CssSelector("body > div.ektron-ux-reset.ektron-ux.ektron-ux-UITheme > div.toolbarWrapper.ux-clearfix > div.applicationToolbar > ul > li:nth-child(2) > div > label:nth-child(2)")); } }
        private IWebElement toolbarDesignButton { get { return this.Find(By.CssSelector("body > div.ektron-ux-reset.ektron-ux.ektron-ux-UITheme > div.toolbarWrapper.ux-clearfix > div.applicationToolbar > ul > li:nth-child(2) > div > label:nth-child(4)")); } }
        private IWebElement widgetAccordion { get { return this.Find(By.Id("ui-accordion-2-header-2")); } }
        private IWebElement firstWidgetSelect { get { return this.Find(By.CssSelector("div.ux-siteApp-pageBuilderWidgetList ul li div.widgetIcon")); } }
        private IWebElement firstDropZone { get { return this.Find(By.CssSelector("div#Top_uxDropZone li.widgetMoveLocation")); } }
        private IWebElement firstUpdatePanelAfterWidgetAddSelect { get { return this.Find(By.CssSelector("#Top_uxColumnDisplay_ctl00_uxControlColumn_ctl00_uxWidgetHost_uxUpdatePanel")); } }
        private IWebElement closeTrayIcon { get { return this.Find(By.CssSelector("#form1 > div.ektron-ux-reset.ektron-ux-UITheme.ux-app-siteApp-dialogs > div:nth-child(2) > div.ui-dialog-titlebar.ui-widget-header.ui-corner-all.ui-helper-clearfix > button > span.ui-button-text > span")); } }
        private IWebElement contentBlockWidgetEdit { get { return this.Find(By.CssSelector("#Top_uxColumnDisplay_ctl00_uxControlColumn_ctl00_uxWidgetHost_uxAction > span > a.edit > span")); } }
        private IWebElement contentBlockWidgetFolderViewFirstFolder { get { return this.Find(By.CssSelector("#Top_uxColumnDisplay_ctl00_uxControlColumn_ctl00_uxWidgetHost_uxWidgetHost_widget > div > div.CBTabInterface > div.ByFolder.CBTabPanel > div > ul > li > span")); } }

        public PageBuilderPageLayoutNavigation toolbarContentButtonClick()
        {
            this.toolbarContentButton.Click();
            return this;
        }
        public PageBuilderPageLayoutNavigation toolbarDesignButtonClick()
        {
            this.toolbarDesignButton.Click();
            return this;
        }
        public PageBuilderPageLayoutNavigation widgetAccordionClick()
        {
            this.widgetAccordion.Click();
            return this;
        }
        public PageBuilderPageLayoutNavigation firstWidgetSelectClick()
        {
            this.firstWidgetSelect.Click();
            return this;
        }

        public PageBuilderPageLayoutNavigation firstDropZoneClick()
        {
            this.firstDropZone.Click();
            return this;
        }

        public PageBuilderPageLayoutNavigation closeTrayIconClick()
        {
            this.closeTrayIcon.Click();
            return this;
        }
        public PageBuilderPageLayoutNavigation contentBlockWidgetFolderViewFirstFolderClick()
        {
            this.contentBlockWidgetFolderViewFirstFolder.Click();
            return this;
        }
        public PageBuilderPageLayoutNavigation editContentBlockWidgetClick()
        {
            var actions = new Actions(this.browser);
            actions.MoveToElement(this.firstUpdatePanelAfterWidgetAddSelect).MoveToElement(contentBlockWidgetEdit).Click().Build().Perform();
            return this;
        }
    }
}
