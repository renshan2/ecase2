using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class SPFolderExtensions
    {
        public static bool IsDocumentSetFolder(this SPFolder folder)
        {
            bool documentSetFolder = false;
            try
            {
                DocumentSet documentSet = null;
                documentSet = DocumentSet.GetDocumentSet(folder);
                if (documentSet != null && documentSet.ContentType != null)
                {
                    if (documentSet.ContentType.Id.IsChildOf(SPBuiltInContentTypeId.DocumentSet))
                        documentSetFolder = true;
                }
            }
            catch (NullReferenceException nullEx)
            {
                //TODO: Find a better way to do this
                //if our content type is null, then we are likely dealing with a document library
                //so we'll swallow this here
            }
            return documentSetFolder;
        }
    }
}
