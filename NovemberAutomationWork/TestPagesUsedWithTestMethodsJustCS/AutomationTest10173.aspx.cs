using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Framework;
using Ektron.Cms.Framework.User;

public partial class templatesa_AutomationTest10173 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var id = CreateUserAndLogin();
        AddUserToAdminGroup(id);
        Login();
        Localization();
    }
    public void Login()
    {
        var uM = new UserManager(ApiAccessMode.Admin);
        var uD = new UserData();
        var userName = "frCanUser";
        var passWord = "frCanUser";
        var ud = uM.Login(userName, passWord);

    }
    public void Localization()
    {
        var LocalizationCRUD = new Ektron.Cms.Framework.Settings.LocaleManager();

        if (LocalizationCRUD.RequestInformation.UserId > 0)
        {
            int localeId = 3084;
            var locale = LocalizationCRUD.GetItem(localeId);
            if (locale != null)
            {
                locale.LanguageState = Ektron.Cms.Localization.LanguageState.SiteEnabled;
                LocalizationCRUD.Update(locale);
                Response.Write("Locale Enabled: " + locale.CombinedName);
            }
            else
            {
                Response.Write("Error: Locale selected is invalid.");
            }
        }
        else
        {
            Response.Write("Error: You must be logged in to use this service.");
        }

    }
    public long CreateUserAndLogin()
    {
        var uMgr = new UserManager(ApiAccessMode.Admin);
        var uData = new UserData();
        //add user info
        uData.Username = "frCanUser";
        uData.FirstName = "frCanUser";
        uData.LastName = "frCanUser";
        uData.Password = "frCanUser";
        uData.Email = "frCanUser@test.com";
        uData.DisplayName = "frCanUser";
        uData.Address = "1243 Test Addresss Nashua, NH 03063";
        uData.IsMemberShip = false;
        uData.LanguageId = 3084;
        
        //add custom properties
        uData.CustomProperties = uMgr.GetCustomPropertyList();
        uData.CustomProperties["Time Zone"].Value = "Central Standard Time";
        uData.CustomProperties["Phone"].Value = "603-333-3333";
        uMgr.Add(uData);
        return uData.Id;
    }

    public void AddUserToAdminGroup(long id)
    {
        var uGroupMgr = new UserGroupManager(ApiAccessMode.Admin);
        uGroupMgr.AddUser(1, id);
    }




}