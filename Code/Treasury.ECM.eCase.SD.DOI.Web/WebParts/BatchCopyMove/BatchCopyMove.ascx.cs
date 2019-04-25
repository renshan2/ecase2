using Microsoft.SharePoint;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;

namespace Treasury.ECM.eCase.Web.WebParts.BatchCopyMove
{
    [ToolboxItemAttribute(false)]
    public partial class BatchCopyMove : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BatchCopyMove()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (SPSite theSite = SPContext.Current.Site)
                {
                    using (SPWeb rootWeb = theSite.OpenWeb())
                    {
                        DocLibRecursive(rootWeb);
                    }
                }
            }
        }

        private void DocLibRecursive(SPWeb web)
        {
            List<SPDocumentLibrary> docLibList = new List<SPDocumentLibrary>();
            foreach (SPList list in web.Lists)
            {
                if (list.BaseType == SPBaseType.DocumentLibrary)
                {
                    SPDocumentLibrary docLib = (SPDocumentLibrary)list;
                    if (docLib.IsCatalog == false)
                    {
                        docLibList.Add(docLib);
                    }
                }
            }

            foreach (SPWeb subWeb in web.Webs)
            {
                DocLibRecursive(subWeb);
            }

            TreeNode tn = null;
            TreeNode child = new TreeNode();

            foreach (var item in docLibList)
            {
                tn = new TreeNode();
                tn.ImageUrl = item.ImageUrl;
                tn.Text = item.Title;
                if (item.Folders.Count > 0)
                {
                    var childItems = item.Folders;
                    foreach (SPListItem childItem in childItems)
                    {
                        child = new TreeNode();
                        child.ImageUrl = tn.ImageUrl; ;
                        child.Text = childItem.Name;
                        tn.ChildNodes.Add(child);
                        child = null;
                    }
                }
                tn.ChildNodes.Add(tn);
                this.treeView.Nodes.Add(tn);
                tn = null;
            }
        }
    }
}
