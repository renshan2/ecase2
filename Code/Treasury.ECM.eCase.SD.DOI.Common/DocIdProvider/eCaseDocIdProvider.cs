using System;
using System.Text;
using Microsoft.Office.DocumentManagement;
using Microsoft.SharePoint;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Database;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Database.StoredProcedures;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.DocIdProvider
{
    public class eCaseDocIdProvider : DocumentIdProvider
    {

        private string GetConnectionString(SPListItem listItem)
        {
            string connectionString = "Initial Catalog=eCaseManagement;Data Source=sp2010auto;User ID=eCaseUser;Password=Devise!!!";
            if (listItem.ParentList.ParentWeb.Site.RootWeb.Properties.ContainsKey(eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING))
                connectionString = listItem.ParentList.ParentWeb.Site.RootWeb.Properties[eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING];
            else
            {
                listItem.ParentList.ParentWeb.Site.RootWeb.Properties.Add(eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING, connectionString);
                listItem.ParentList.ParentWeb.Site.RootWeb.Properties.Update();
            }
            return connectionString;
        }

        private string GetDocumentIdForPrefix(SPListItem listItem, string prefix, bool forceUpdate)
        {
            string connectionString = GetConnectionString(listItem);

            GetCreateDocIdSProc sProc = new GetCreateDocIdSProc(listItem.ParentList.ParentWeb.Site.ID, listItem.ParentList.ParentWeb.ID, listItem.UniqueId, forceUpdate, prefix, 0, listItem.ID);
            using (DbAdapter dbAdapter = new DbAdapter())
            {
                dbAdapter.Connect(connectionString);
                dbAdapter.ExecuteNonQueryStoredProcedure(sProc);
            }
            return string.Format("{0}-{1}", sProc.Parameters[4].Value, sProc.Parameters[5].Value.ToString().PadLeft(9, '0'));
        }

        // Method to generate the actual document ID. Awesomeness lives here! 
        public override string GenerateDocumentId(SPListItem listItem)
        {

            string prefix = "CCCM";
            if (listItem.ParentList.ParentWeb.Properties.ContainsKey(eCaseConstants.PropertyBagKeys.ECASE_DOC_ID_PREFIX))
                prefix = listItem.ParentList.ParentWeb.Properties[eCaseConstants.PropertyBagKeys.ECASE_DOC_ID_PREFIX];
            else 
            {
                if (listItem.ParentList.ParentWeb.Site.RootWeb.Properties.ContainsKey(eCaseConstants.PropertyBagKeys.ECASE_DOC_ID_PREFIX))
                    prefix = listItem.ParentList.ParentWeb.Site.RootWeb.Properties[eCaseConstants.PropertyBagKeys.ECASE_DOC_ID_PREFIX];

                listItem.ParentList.ParentWeb.Properties.Add(eCaseConstants.PropertyBagKeys.ECASE_DOC_ID_PREFIX, prefix);
                listItem.ParentList.ParentWeb.Properties.Update();
            }

            return GetDocumentIdForPrefix(listItem, prefix, false);
        }

        // Method to re-assign the actual document ID with a specific prefix. More awesomeness lives here! This is not as complete as OOTB approach AND not meant as original Doc ID Assignment
        public string AssignDocumentId(SPListItem listItem, string prefix)
        {
            string docId = GetDocumentIdForPrefix(listItem, prefix, true);
            /*obtained via reflector from Microsoft.Office.DocumentManagement.DocumentId - 
             * not meant as shared code, but provides best known and quick
             * method of changing the doc id for an individual document
             */
            Guid s_guidDocIdField = new Guid("{AE3E2A36-125D-45d3-9051-744B513536A6}");
            Guid s_guidDocIdUrlField = new Guid("{3B63724F-3418-461f-868B-7706F69B029C}");

            listItem[s_guidDocIdField] = docId;
            StringBuilder docUrl = new StringBuilder(1024);
            docUrl.Append(listItem.Web.Url);
            docUrl.Append("/_layouts/DocIdRedir.aspx?ID=");
            docUrl.Append(System.Web.HttpUtility.UrlEncode(docId));

            SPFieldUrlValue sPFieldUrlValue = new SPFieldUrlValue();
            sPFieldUrlValue.Description = docId;
            listItem[s_guidDocIdUrlField] = sPFieldUrlValue;

            listItem.SystemUpdate();

            return docId;
        }

        public override bool DoCustomSearchBeforeDefaultSearch
        {
            // If set to true: It will call the GetDocumentUrlsById method before search 
            // If set to false: It will use SharePoint Search before custom methods 
            get { return false; }
        }

        public override string[] GetDocumentUrlsById(SPSite site, string documentId)
        {
            // Returns an array of URLs pointing to  
            // documents with a specified DocumentId 
            // An empty string array  

            // This is where you will implement your logic to find 
            // documents based on a documentId if you don’t want to use 
            // the search-approach. 
            return new string[] { };
        }

        public override string GetSampleDocumentIdText(SPSite site)
        {
            // Returns the default Document ID value that will be initially 
            // displayed in the Document ID search web part as a help when searching 
            // for documents based on ID’s. 
            // This should correspond with the way you’ve designed your ID pattern 
            #region Configure Prefix
            string prefix = "CCCM";
            if (site.RootWeb.Properties.ContainsKey(eCaseConstants.PropertyBagKeys.ECASE_DOC_ID_PREFIX))
                prefix = site.RootWeb.Properties[eCaseConstants.PropertyBagKeys.ECASE_DOC_ID_PREFIX];
            #endregion

            return string.Format("{0}-123456789", prefix);
        } 

    }
}
