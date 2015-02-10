using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WorkareaAutomation.PageObjects
{

    /// <summary>
    /// Entity to interact with the Aloha editor wysiwig.
    /// </summary>
    /// <remarks>This class has intimate knowledge of the Aloha editor, and interacts with it through javascript.</remarks>
    public class AlohaEditor : PollingElementFinder
    {

        #region ctors
        /// <summary>
        /// Constructor that lets the web.config "elmentPollTimeOutInSeconds" app settings key value set how long to wait for an element on a page to be found.
        /// </summary>
        /// <param name="browser">An instance of a <see cref="OpenQA.Selenium.IWebDriver"/> ,e.g. FireFoxDriver, which allows interaction with the browser</param>
        public AlohaEditor(IWebDriver browser) : base(browser) { }
        /// <summary>
        /// Constructor that allows for overriding the time to wait for an element to be found on a web page.
        /// </summary>
        /// <param name="browser">An instance of a Selenium Web Driver, e.g. FireFoxDriver, which allows interaction with the browser.</param>
        /// <param name="pollTimeOut">How long you want to wait for elements on the web page to be found.</param>
        public AlohaEditor(IWebDriver browser, int pollTimeOut) : base(browser, pollTimeOut) { }
        #endregion

        #region private members
        private IWebElement reviewTab { get { return this.Find(By.LinkText("Review")); } }
        private IWebElement viewSourceButton { get { return this.Find(By.CssSelector("button[title='View Source']")); } }
        private IWebElement contentEditableDiv { get { return this.Find(By.CssSelector("div[contenteditable='true']")); } }

        private IWebElement orderListButton
        {
            get { return this.Find(By.CssSelector("span.aloha-icon-orderedlist")); }
        }
        #endregion


        #region private methods
        private string requireJSCodeToGetSourceText()
        {
            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.Append(@"Aloha.require(['aceSourceEditorFacade'], function(editorFacade){ window['aceSource'] = editorFacade.getSource();});");
            scriptBuilder.Append(@"return window['aceSource']");
           
            return scriptBuilder.ToString();
        }

        private void loadJavascriptAceEditorFacadeIntoBrowser()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var aceEditorFacade = File.ReadAllText(Path.Combine(assemblyPath,"AlohaEditorScriptInteraction","aceEditorFacade.js"));
            IJavaScriptExecutor js = this.browser as IJavaScriptExecutor;
            js.ExecuteScript(aceEditorFacade);

        }

        private string requireJSCodeToEnterSourceText(string value)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.Append(@"Aloha.require(['aceSourceEditorFacade'], function(editor){");
            scriptBuilder.Append(@"editor.enterSource(""");
            scriptBuilder.Append(value);
            scriptBuilder.Append(@""");});");
            return scriptBuilder.ToString();
        }

        private void setContentAsBrowserFocus()
        {
            new Actions(this.browser).MoveToElement(this.contentEditableDiv).Click().Perform();
        }

        #endregion
        /// <summary>
        /// Property to Get/Set the source html for the Aloha Editor content.
        /// </summary>
        /// <example>
        /// <code>
        /// alohaInstance.SourceText = "&lt;p&gt;foobar&lt;/p&gt; ;"
        /// </code>
        /// </example>
        /// <remarks>This will inject javascript at runtime to interact with the browser. If it fails, please check the browsers error console.</remarks>
        public string SourceText
        {
            get
            {
                IJavaScriptExecutor js = this.browser as IJavaScriptExecutor;
                this.loadJavascriptAceEditorFacadeIntoBrowser();
                var editorSource = js.ExecuteScript(this.requireJSCodeToGetSourceText()) as string;
                return editorSource;
            }
            set
            {
                IJavaScriptExecutor js = this.browser as IJavaScriptExecutor;
                this.loadJavascriptAceEditorFacadeIntoBrowser();
                var setEditorSourceValue = this.requireJSCodeToEnterSourceText(value);
                js.ExecuteScript(setEditorSourceValue);
            }
        }

        /// <summary>
        /// Tell the Aloha editor to go to the "Review" tab
        /// </summary>
        /// <returns>The current instance of <see cref="AlohaEditor"/> , this means state is persisted between calls.</returns>
        public AlohaEditor Review()
        {
            this.reviewTab.Click();
            return this;
        }

        /// <summary>
        /// Tell the Aloha editor to view the content's HTML source. Requires <see cref="AlohaEditor.Review()"/> to be called first.
        /// </summary>
        /// <returns>The current instance of <see cref="AlohaEditor"/> , this means state is persisted between calls.</returns>
        public AlohaEditor ViewSource()
        {
            setContentAsBrowserFocus();
            this.viewSourceButton.Click();
            return this;
        }

       
        /// <summary>
        /// Set the Aloha editor source text. Requires <see cref="AlohaEditor.ViewSource()"/> to be called first.
        /// </summary>
        /// <param name="sourceText">The source <see cref="System.String"/> that should be entered into the Aloha Editor's source Text Area.</param>
        /// <returns>The current instance of <see cref="AlohaEditor"/> , this means state is persisted between calls.</returns>
        public AlohaEditor EnterSource(string sourceText)
        {
            this.SourceText = sourceText;
            return this;
        }

       


        /// <summary>
        /// Tell Aloha to close the the view of the content's HTML source. Requires <see cref="AlohaEditor.ViewSource()"/> to have been called first.
        /// </summary>
        /// <returns>The current instance of <see cref="AlohaEditor"/> , this means state is persisted between calls.</returns>
        public AlohaEditor CloseSource()
        {

            //UNDONE
            throw new NotImplementedException("Can determine best way to find the dialog close button, as there are many dialogs on the page...");
        }


        /// <summary>
        /// Stop working with the Aloha editor on the page.
        /// </summary>
        /// <returns>A new instance of <see cref="ContentPage"/> that has a reference to this AlohaEditor browser instance and pollTimeout.</returns>
        public ContentPage GoToContent()
        {
            return new ContentPage(this.browser, this.pollTimeOut);
        }

        /// <summary>
        /// Retrieve the current source <see cref="System.String"/> that the Aloha Editor has for the current content HTML.
        /// </summary>
        /// <param name="sourceText">A ref parameter which will be modified to have the current source that the Aloha Editor has for the current content HTML.</param>
        /// <returns>The current instance of <see cref="AlohaEditor"/> , this means state is persisted between calls.</returns>
        public AlohaEditor GetSource(ref string sourceText)
        {

            //HACK: I don't like how I wrote this. Having an "out" (ref) parameter is just plain confusing.  Should follow pattern of persisting the source state for evaluation when needed. See contentDesigner.AlertTextForViewDesign
            sourceText = this.SourceText;
            return this;
        }

        public AlohaEditor EnterText(string text)
        {
            this.setContentAsBrowserFocus();

            Text = text;

            return this;
        }

        public string Text
        {
            set
            {
                this.contentEditableDiv.Clear();
                this.contentEditableDiv.SendKeys(value);
            }
        }

        public AlohaEditor ClickOrderedList()
        {
            this.orderListButton.Click();
            return this;
        }
    }
}
