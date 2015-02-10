using System;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Framework;
using Ektron.Cms.Framework.Content;
using Ektron.Cms.Framework.Organization;
using Ektron.Cms.Framework.User;
using Ektron.Cms.API.User;

public partial class templatesa_AddTemplate : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CreateAndLogin();
        var templateId = AddTemplate();
        AssignWidgetToWireFrame(templateId);
        var folderId = CreateFolderAssociatePageLayoutAsTemplate(templateId);
        contentCreation(folderId);
    }

    public void CreateAndLogin()
    {
        var uM = new UserManager(ApiAccessMode.Admin);
        var uData = new UserData();
	 try
        {
        uData.Username = "adminA";
        uData.FirstName = "adminA";
        uData.LastName = "adminA";
        uData.Password = "Ektron";
        uData.Email = "adminA@test.com";
        uData.DisplayName = "adminA";
        uData.Address = "1243 Test Addresss Nashua, NH 03063";
        uData.IsMemberShip = false;

        //add custom properties
        uData.CustomProperties = uM.GetCustomPropertyList();
        uData.CustomProperties["Time Zone"].Value = "Central Standard Time";
        uData.CustomProperties["Phone"].Value = "603-333-3333";
        uData = uM.Add(uData);
	}
	   catch (UserAlreadyExistsException ex)
        {
              var u = new User();
            uData=u.GetUserByUsername("adminA");
        }
        UserGroupManager userGroupmanager = new UserGroupManager(ApiAccessMode.Admin);
        userGroupmanager.AddUser(1, uData.Id);


        var userName = "adminA";
        var passWord = "Ektron";
        var ud = uM.Login(userName, passWord);

    }

    public long AddTemplate()
    {
        const string fileNameString = "PageLayout.aspx";
        const string descriptionString = "PageBuilder wireframe";
        const string templateNameString = "Add user api";
        var templateManager = new TemplateManager(ApiAccessMode.Admin);
        //populate template data object defining that is type pb
        var templateData = new TemplateData()
        {
            SubType = EkEnumeration.TemplateSubType.Wireframes,
            TemplateName = templateNameString,
            Description = descriptionString,
            IsToolbarEnabled = true,
            Type = Ektron.Cms.Common.EkEnumeration.TemplateType.Default,
            FileName = fileNameString
        };

        templateManager.Add(templateData);

        return templateData.Id;
    }

    public void AssignWidgetToWireFrame(long templateDataId)
    {
        //new up a wire frame model
        var test2 = new Ektron.Cms.PageBuilder.WireframeModel();
        //return wireframe data object
        var wireFrameData = test2.FindByTemplateID(templateDataId);
        //assign the content block widget to the wireframe data.id
        test2.AddWidgetTypeAssociation(wireFrameData.ID, 23);
    }
    //add a  new folder to the cms and associate the pagelayout as a template
    public long CreateFolderAssociatePageLayoutAsTemplate(long templateDataId)
    {
        var fm = new FolderManager(ApiAccessMode.Admin);
        //populate folder data object assigning the PageLayout.aspx id to the folder
        var fd = new FolderData()
        {
            Name = "AutomationTest9992",
            ParentId = 0,
            IsTemplateInherited = false,
            TemplateId = templateDataId,
        };
        fd = fm.Add(fd);
        return fd.Id;
    }
    //using the cmslogin.aspx template that is in all workareas create 15 content items
    public void contentCreation(long folderID)
    {
        var cm = new ContentManager(ApiAccessMode.Admin);
        for (var i = 0; i < 15; i++)
        {
            var cd = new ContentData { Title = "Test content" + i, Html = "testing content" + i, FolderId = folderID };
            cm.Add(cd);
        }
    }

}


