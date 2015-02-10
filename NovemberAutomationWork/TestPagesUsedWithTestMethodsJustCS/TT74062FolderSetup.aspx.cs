using Ektron.Cms;
using Ektron.Cms.Framework.Content;
using Ektron.Cms.Framework.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class TT74062FolderSetup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        FolderManager fm = new FolderManager(Ektron.Cms.Framework.ApiAccessMode.Admin);
        
        FolderData fd1 = new FolderData(){
            Name="Test Alias"
        };
        fd1 = fm.Add(fd1);

        FolderData fd2 = new FolderData(){
            Name = "Test Sub",
            ParentId = fd1.Id,
            IsPermissionsInherited = false,
            PrivateContent = true
        };
        fm.Add(fd2);

        fd1 = new FolderData(){
            Name = "Test Asset"
        };
        fd1 = fm.Add(fd1);

        ContentManager cm = new ContentManager(Ektron.Cms.Framework.ApiAccessMode.Admin);
        ContentData cd = new ContentData(){
            FolderId = fd1.Id,
            Title = "My Test Content",
            Html = "<H1>NOTHING TO SEE HERE</H1>"
        };
        cm.Add(cd);


    }
}