using OpenQA.Selenium;

namespace WorkareaAutomation.PageObjects
{
    public class DiscussionForumPage : PollingElementFinder
    {
        private IWebElement ForumNameInput { get { return this.Find(By.Id("adddfolder_txt_adf_forumname")); } }
        private IWebElement ForumDescriptionInput { get { return this.Find(By.Id("adddfolder_txt_adf_forumtitle")); } }
        private IWebElement saveButton { get { return this.Find(By.Id("image_link_101")); } }


        public DiscussionForumPage(IWebDriver browser, int pollTimeOut)
            : base(browser, pollTimeOut) { }

        public DiscussionForumPage(IWebDriver browser)
            : base(browser) { }

        public DiscussionForumPage EnterName(string forumName)
        {
            ForumName = forumName;
            return this;
        }

        public string ForumName
        {
            set
            {
                this.ForumNameInput.Clear();
                this.ForumNameInput.SendKeys(value);
            }
        }

        public DiscussionForumPage EnterDescription(string forumDescription)
        {
            ForumDescription = forumDescription;
            return this;
        }

        public string ForumDescription
        {
            set
            {
                this.ForumDescriptionInput.Clear();
                this.ForumDescriptionInput.SendKeys(value);
            }
        }
        
        public void Save()
        {
            this.saveButton.Click();
        }
    }
}
