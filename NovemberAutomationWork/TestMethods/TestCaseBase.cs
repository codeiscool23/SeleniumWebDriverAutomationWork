using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WorkareaAutomation.Helpers;
using WorkareaAutomation.PageObjects;

namespace CUVerification
{
    public abstract class TestCaseBase
    {
        #region protected properties, lazy loaded
        private string _siteUrl;
        protected string siteUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_siteUrl))
                {
                    this._siteUrl = ConfigurationManager.AppSettings.Get("siteurl");
                }
                return this._siteUrl;
            }
        }
        private string _adminUserName;
        protected string adminUserName
        {
            get
            {
                if (string.IsNullOrEmpty(_adminUserName))
                {
                    _adminUserName = ConfigurationManager.AppSettings.Get("adminusername");
                }
                return _adminUserName;
            }
        }
        private string _adminUserPassword;
        protected string adminUserPassword
        {
            get
            {
                if (string.IsNullOrEmpty(_adminUserPassword))
                {
                    _adminUserPassword = ConfigurationManager.AppSettings.Get("adminpassword");
                }
                return _adminUserPassword;
            }
        }
        private string _builtinUserName;
        protected string builtinUserName
        {
            get
            {
                if (string.IsNullOrEmpty(_builtinUserName))
                {
                    _builtinUserName = ConfigurationManager.AppSettings.Get("builtinusername");
                }
                return _builtinUserName;
            }
        }
        private string _builtinUserPassword;
        protected string builtinUserPassword
        {
            get
            {
                if (string.IsNullOrEmpty(_builtinUserPassword))
                {
                    _builtinUserPassword = ConfigurationManager.AppSettings.Get("builtinpassword");
                }
                return _builtinUserPassword;
            }
        }
        private string _siteRoot;
        protected string siteRoot
        {
            get
            {
                if (string.IsNullOrEmpty(_siteRoot))
                {
                    _siteRoot = ConfigurationManager.AppSettings.Get("siteroot");
                }
                return _siteRoot;
            }
        }
        private string _assmblyPath;
        protected string assemblyPath
        {
            get
            {
                if (string.IsNullOrEmpty(_assmblyPath))
                {
                    _assmblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }
                return _assmblyPath;
            }
        }
        private string _templatePath;
        protected string templatePath
        {
            get
            {
                if (string.IsNullOrEmpty(_templatePath))
                {
                    _templatePath = Path.Combine(this.assemblyPath, @"Templates");
                }
                return _templatePath;
            }
        }
        private string _siteDataBase;
        protected string siteDataBase
        {
            get
            {
                if (string.IsNullOrEmpty(_siteDataBase))
                {
                    _siteDataBase = ConfigurationManager.AppSettings.Get("siteDb");
                }
                return _siteDataBase;
            }
        }
        private string _siteConnectionString;
        protected string siteConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_siteConnectionString))
                {
                    _siteConnectionString = ConfigurationManager.AppSettings.Get("connString");
                }
                return _siteConnectionString;
            }
        }
        #endregion

        #region private members

        protected CalendarPage CalendarPage;
        protected EventPage EventPage;
        protected IWebDriver browser;
        protected ISiteHelper siteHelper;
        protected IBrowserHelper browserHelper;
        protected ISQLHelper sqlHelper;
        protected LoginPage loginPage;
        protected WindowManager windowManager;
        protected WorkareaFrames workareaFrames;
        protected WorkareaTopNav workareaNavigation;
        protected WorkareaContentFolderNavTree workareaContentNavTree;
        protected MainContentPage mainContentPage;
        protected TaxonomyFolderNavTree taxonomyNavTree;
        protected TaxonomyPage taxonomyPage;
        protected WorkareaSettingsNav settingsNavTree;
        protected UserSettingsListPage userSettingsListPage;
        protected TemplatePage templatePage;
        protected AlohaEditor AlohaEditor;
        //added to work with page layouts
        protected PageBuilderPageLayoutNavigation pageBuilderPageLayoutNavigation;
        //added to work with GoLiveTimeForContent
        protected GoLiveDateTimeSelector goLiveDateTimeSelector;
        protected ContentPage contentPage;
        protected UrlAliasingPage urlAliasingPage;
        protected WorkareaLibraryContentFolderNavTree workareaLibraryFolderNavigation;
        protected IEktronConfig cmsWebConfig;
        protected SmartFormConfigurations smartFormConfigurations;
        protected SetupPage setupPage;
        protected FolderPage folderPage;
        //protected FormPage htmlFormPage;
        protected MenuPage menuPage;
        protected WorkareaMenuNavTree workareaMenuNavTree;
        protected StringBuilder verificationErrors;
        protected PollingElementFinder pollingElementFinder;
        protected Paging paging;
        protected DiscussionForumPage DiscussionForumPage;
        protected DiscussionBoardPage DiscussionBoardPage;
        protected TopicPage TopicPage;
        
        #endregion

        #region test setup and tear down methods.
        [TestInitialize]
        public void Setup()
        {
            //Setup
            this.browser = new FirefoxDriver();
            this.changeBrowserTimeOut(20);
            this.verificationErrors = new StringBuilder();

            this.siteHelper = TestHelperFactory.CreateSiteHelper();
            this.browserHelper = TestHelperFactory.CreateBrowserHelper();
            this.sqlHelper = TestHelperFactory.CreateSQLHelper();
            this.loginPage = new LoginPage(this.browser);
            this.windowManager = new WindowManager(this.browser);
            this.workareaFrames = new WorkareaFrames(this.browser);
            this.workareaNavigation = new WorkareaTopNav(this.browser, this.workareaFrames, this.browserHelper);
            
            this.mainContentPage = new MainContentPage(this.browser,workareaFrames, this.browserHelper);
            this.settingsNavTree = new WorkareaSettingsNav(this.browser, this.workareaFrames);
            this.userSettingsListPage = new UserSettingsListPage(this.browser);
            this.templatePage = new TemplatePage(this.browser, this.workareaFrames);
            this.urlAliasingPage = new UrlAliasingPage(this.browser);
            this.taxonomyNavTree = new TaxonomyFolderNavTree(this.browser);
            this.taxonomyPage = new TaxonomyPage(this.browser,this.workareaFrames, this.browserHelper);
            this.workareaLibraryFolderNavigation = new WorkareaLibraryContentFolderNavTree(this.browser, this.browserHelper);
            this.menuPage = new MenuPage(this.browser, workareaFrames, browserHelper);
            this.workareaMenuNavTree = new WorkareaMenuNavTree(this.browser);
            this.folderPage = new FolderPage(this.browser, this.browserHelper);
            this.cmsWebConfig = new EktronWebConfig(Path.Combine(this.siteRoot, "web.config"));
            this.smartFormConfigurations = new SmartFormConfigurations(this.browser, this.workareaFrames);
            this.CalendarPage = new CalendarPage(this.browser, this.workareaFrames);
            this.AlohaEditor = new AlohaEditor(this.browser);
            this.EventPage = new EventPage(this.browser,this.AlohaEditor);
            this.pageBuilderPageLayoutNavigation = new PageBuilderPageLayoutNavigation(this.browser, this.browserHelper);
            this.goLiveDateTimeSelector = new GoLiveDateTimeSelector(this.browser, this.browserHelper);
            this.contentPage = new ContentPage(this.browser);
            this.setupPage = new SetupPage(this.browser);
            this.workareaContentNavTree = new WorkareaContentFolderNavTree(this.browser, this.browserHelper, this.workareaFrames);
            this.pollingElementFinder = new PollingElementFinder(this.browser);
            this.browser.Navigate().GoToUrl(this.siteUrl);
            this.paging = new Paging(this.browser,this.workareaFrames,this.browserHelper);
            this.DiscussionForumPage = new DiscussionForumPage(this.browser);
            this.DiscussionBoardPage = new DiscussionBoardPage(this.browser);
            this.TopicPage = new TopicPage(this.browser);

        }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                this.browser.Close();
                this.browser.Quit();
            }
            catch (Exception ex)
            {
                verificationErrors.AppendLine(ex.Message);
            }
            finally
            {

            }

            Assert.AreEqual("", verificationErrors.ToString());
        }

        #endregion

        #region protected helpers
        protected void changeBrowserTimeOut(int seconds)
        {
            this.browser.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(seconds));
        }
       

        

        protected string loginAndOpenWorkarea(string userName, string userPassword)
        {
            return windowManager.InvokePopUpWindowActionAndReturnOriginalWindow(() =>
            {
                loginPage.EnterUserName(userName)
                    .EnterUserPassWord(userPassword)
                    .Login();
                     this.browserHelper.WaitForJavascriptExecution();
                    loginPage.OpenWorkarea();

                this.browserHelper.WaitForJavascriptExecution();
            });
        }
        protected string Userlogin(string userName, string userPassword)
        {
            return windowManager.InvokePopUpWindowActionAndReturnOriginalWindow(() =>
            {
                loginPage.EnterUserName(userName)
                    .EnterUserPassWord(userPassword)
                    .Login();
            });
        }
        #endregion
    }
}
