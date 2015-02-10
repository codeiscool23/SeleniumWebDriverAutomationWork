using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Framework.Content;

public partial class TT74050 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        long id = 0;
        ResultMessage.Text = "";

        if (long.TryParse(this.Request.QueryString["id"], out id))
        {
            try
            {
                AssetManager assetManager = new AssetManager();
                var contentAssetData = assetManager.GetItem(id);
                assetManager.Update(contentAssetData);
                ResultMessage.Text = string.Format("Asset ID {0} Updated", id.ToString());
            }
            catch (Exception ex)
            {
                ResultMessage.Text = ex.Message;
            }
        }
    }
}