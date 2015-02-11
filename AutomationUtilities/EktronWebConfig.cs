using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace WorkareaAutomation.Helpers
{
    //TODO: refactor into separate class file.
    public static class ConfigHelper
    {

        public static Configuration OpenMappedWebConfig(string configPath)
        {
            var configFile = new FileInfo(configPath);
            var webConfigVirtualDirectoryMapping = new VirtualDirectoryMapping(configFile.DirectoryName, true, configFile.Name);
            var configFileMap = new WebConfigurationFileMap();
            configFileMap.VirtualDirectories.Add("/", webConfigVirtualDirectoryMapping);
            return WebConfigurationManager.OpenMappedWebConfiguration(configFileMap, "/");
        }
    }
    //extenstion of the AutomatedTestBase to get vital version infromation
    public class ConfigVersionInfo : AutomationTestBase
    {
        //list of cms version iformation. 
        //index 0 = buildnumber
        //index 1 = service pack
        //index 2 = cumulative update
        private List<string> versionInfo;
        //
        public void allVersionConfigInfo()
        {
           var cmsWebConfig = new EktronWebConfig(Path.Combine(siteRoot, "web.config"));
           cmsWebConfig.Open();
           versionInfo = cmsWebConfig.getConfigVersion();
        }
        //get a double of what the cms version is
        public double versionDouble()
        {
            
            var version = Convert.ToDouble(this.versionInfo.First().ToString().Substring(0, 3));
            return version;

        }
        //get the full string of the CMS version
        public string versionString()
        {
            var version = this.versionInfo.First().ToString();
            return version;
        }
        //checks if there is a service pack applied to the cms
        public bool isVersionAServicePack()
        {
            try
            {
               
                if (String.IsNullOrEmpty(this.versionInfo[1]))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        //checks to see if there is a CU applied to the cms 
        public bool isVersionACumulativeUpdate()
        {
            try
            {

                if (String.IsNullOrEmpty(this.versionInfo[2]))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }

    /// <summary>
    /// Different Editing Interfaces that the CMS supports.
    /// </summary>
    public enum EditingInterface
    {
        /// <summary>
        /// A legacy wysiwyg editor
        /// </summary>
        /// <remarks>Requires a web.config change at the least.</remarks>
        ContentDesigner,
        /// <summary>
        /// The current wysiwyg editor
        /// </summary>
        /// <remarks>Enabled by default.</remarks>
        Aloha
    }

    /// <summary>
    /// An instance of <see cref="IEktronConfig"/> that specifically works with a CMS website's main web.config.
    /// </summary>
    public class EktronWebConfig : IEktronConfig
    {
        private readonly string configPath;
        private Configuration config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configPath">A <see cref="System.String"/> that points to the installed CMS website's web.config.</param>
        public EktronWebConfig(string configPath)
        {
            this.configPath = configPath;
        }

        private void CheckConfigNotNull()
        {
            if (this.config == null)
            {
                throw new NullReferenceException("IEktronConfig.Open() needs to be executed before interacting with config.");
            }
        }

        /// <summary>
        /// Opens a <see cref="System.Web.Configuration.WebConfigurationFileMap"/> that allows file handling of the CMS web.config.
        /// </summary>
        /// <returns>The current instance of <see cref="EktronWebConfig"/>, meaning state is persisted, such as the configPath.</returns>
        public IEktronConfig Open()
        {
            this.config = ConfigHelper.OpenMappedWebConfig(configPath);
            
            return this;
        }
        /// <summary>
        /// Change the default wysiwyg editor the Ektron CMS will use for content authoring.
        /// </summary>
        /// <param name="editor">The <see cref="EditingInterface"/> that you want the CMS to use for content authoring.</param>
        /// <returns>The current instance of <see cref="EktronWebConfig"/>, meaning state is persisted, such as the configPath.</returns>
        public IEktronConfig SetAuthoringEditor(EditingInterface editor)
        {
            this.AuthoringEditor = editor;
            return this;
        }

        /// <summary>
        /// Set the CMS web.config app setting for default wysiwyg editor.
        /// </summary>
        /// <remarks>
        /// This will save the web.config after the value has been set.
        /// </remarks>
        /// <exception cref="NullReferenceException">If the Configuration has been loaded.</exception>
        public EditingInterface AuthoringEditor 
        { 
            set
            {
                CheckConfigNotNull();
                this.config.AppSettings.Settings["ek_EditControlWin"].Value = value.ToString();
                this.config.Save();
            }
        }
        /// <summary>
        /// Set/Get the CMS web.config value for Global Default Content Language.
        /// </summary>
        /// /// <remarks>
        /// The set will save the web.config after the value has been set.
        /// </remarks>
        /// <exception cref="NullReferenceException">If the Configuration has been loaded.</exception>
        public string DefaultContentLanguage
        {
            get
                {
                    CheckConfigNotNull();
                    return this.config.AppSettings.Settings["ek_DefaultContentLanguage"].Value;
                    
                }

            set
            {
                CheckConfigNotNull();
                this.config.AppSettings.Settings["ek_DefaultContentLanguage"].Value = value.ToString();
                this.config.Save();
            }
        }
        protected List<string> versionInformation
        {
            get
                {
                    CheckConfigNotNull();
                    List<string> listRange = new List<string>();
                    listRange.Add(this.config.AppSettings.Settings["ek_buildNumber"].Value);
                    listRange.Add(this.config.AppSettings.Settings["ek_ServicePack"].Value);
                    listRange.Add(this.config.AppSettings.Settings["ek_PatchUpdate"].Value);
                    return listRange;
                    
                }          
        }

        /// <summary>
        /// Set the CMS default language id (lcid)
        /// </summary>
        /// <param name="languageId">The language id (lcid) that the CMS will use.</param>
        /// <returns>An instance of <see cref="IEktronConfig"/> for chaining.</returns>
        public IEktronConfig SetDefaultContentLanguageId(int languageId)
        {
            this.DefaultContentLanguage = languageId.ToString();
            return this;
        }


        public List<string> getConfigVersion()
        {
            return this.versionInformation;
        }
       



    }

    //TODO: refactor into separate class file
    public class EktronWorkareaWebConfig : IEktronWorkareaConfig
    {
        private readonly string configPath;
        private Configuration config;
        public EktronWorkareaWebConfig(string configPath)
        {
            this.configPath = configPath;
        }

        public IEktronWorkareaConfig Open()
        {
            this.config = ConfigHelper.OpenMappedWebConfig(configPath);
            return this;
        }

        public IEktronWorkareaConfig EnableTrace()
        {
            this.Trace = true;
            return this;
        }

        public IEktronWorkareaConfig DisableTrace()
        {
            //TODO: finish implementing DisableTrace
            throw new NotImplementedException();
        }

        public bool Trace
        {
            set
            {
                //TODO: finish setting the workarea web.config system.web trace value
                throw new NotImplementedException("need to finish");
            }
        }
    }
    /// <summary>
    /// An interface for interacting with the CMS web.config.
    /// </summary>
    public interface IEktronConfig
    {//TODO: refactor into separate class file
        /// <summary>
        /// A contract that opens a given cms root web.config
        /// </summary>
        /// <returns>An instance of <see cref="IEktronConfig"/> for chaining.</returns>
        IEktronConfig Open();
        /// <summary>
        /// A contract that will set the CMS editing Interface.
        /// </summary>
        /// <param name="editor">The <see cref="EditingInterface"/> that the CMS will use.</param>
        /// <returns>An instance of <see cref="IEktronConfig"/> for chaining.</returns>
        IEktronConfig SetAuthoringEditor(EditingInterface editor);

        /// <summary>
        /// A contract that will set the CMS editing Interface.
        /// </summary>
        /// <param name="languageId">The language id (lcid) that the CMS will use.</param>
        /// <returns>An instance of <see cref="IEktronConfig"/> for chaining.</returns>
        IEktronConfig SetDefaultContentLanguageId(int languageId);
        /// <summary>
        /// must be able to retrieve webconfig configuration object from only workarea automation framework
        /// </summary>
        /// <returns>
        /// .net configuration object
        /// </returns>

        List<string> getConfigVersion();


      
    }

    /// <summary>
    /// An interface for interacting with the CMS workarea web.config.
    /// </summary>
    public interface IEktronWorkareaConfig
    {//TODO: refactor into separate class file
        /// <summary>
        /// A contract that opens a given cms workarea web.config
        /// </summary>
        /// <returns>An instance of <see cref="IEktronWorkareaConfig"/> for chaining.</returns>
        IEktronWorkareaConfig Open();
        /// <summary>
        /// A contract that will set the workarea web.config configuration->System.Web->Trace to True
        /// </summary>
        /// <returns>An instance of <see cref="IEktronWorkareaConfig"/> for chaining.</returns>
        IEktronWorkareaConfig EnableTrace();
        /// <summary>
        /// A contract that will set the workarea web.config configuration->System.Web->Trace to False
        /// </summary>
        /// <returns>An instance of <see cref="IEktronWorkareaConfig"/> for chaining.</returns>
        IEktronWorkareaConfig DisableTrace();
       
    }


    
}
