using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Treasury.ECM.eCase.SusDeb.DOI.Web.Layouts.eCaseWeb
{
    public partial class ExportToCSV : LayoutsPageBase
    {
        protected List<SPListItem> itemsToExport;

        protected void Page_Load(object sender, EventArgs e)
        {
            SPLongOperation.Begin(delegate(SPLongOperation longOperation)
            {
                longOperation.TrailingHTML = "Please wait while your data is being exported...";
                try
                {
                    if (Request.QueryString["items"] != null && Request.QueryString["source"] != null && Request.QueryString["sourceurl"] != null)
                    {
                        string source = Request.QueryString["source"];
                        string[] items = Request.QueryString["items"].ToString().Split('|');
                        string currentWeb = Request.QueryString["sourceurl"].Replace("'", string.Empty);

                        source = source.Substring(1, source.Length - 2).ToLower();
                        Guid sourceId = new Guid(source);

                        SPWeb web = SPContext.Current.Web;
                        SPDocumentLibrary sourceDocLibrary = (SPDocumentLibrary)web.Lists[sourceId];
                        SPFolder folder = null;
                        itemsToExport = new List<SPListItem>();

                        //start at 1 due to items split containing a leading empty value
                        for (int i = 1; i < items.Length; i++)
                        {
                            SPListItem currentListItem = sourceDocLibrary.GetItemById(int.Parse(items[i]));
                            itemsToExport.Add(currentListItem);
                            folder = currentListItem.File.ParentFolder;
                        }

                        // build a memory stream of our file contents
                        MemoryStream exportStream = BuildStreamToExport(itemsToExport);
                        // save our export file to our doc libary
                        web.AllowUnsafeUpdates = true;
                        SaveFile(exportStream, folder);
                        web.AllowUnsafeUpdates = false;

                        //close the modal
                        //Response.Write(string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">window.frameElement.commonModalDialogClose(1, '{0}');</script>", itemsToExport.Count));
                        //Response.Flush();
                        //Response.End();
                        if (SPContext.Current.IsPopUI)
                            longOperation.EndScript("window.frameElement.commitPopup();");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error("Error while loading documents for metadata export.", ex, DiagnosticsCategories.eCaseWeb);
                }
            }
            );
        }

        private void SaveFile(MemoryStream exportStream, SPFolder docLibFolder)
        {
            string fileName = "TAB_Export_" + DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + ".txt";

            exportStream.Position = 0;
            byte[] contents = new byte[exportStream.Length];
            exportStream.Read(contents, 0, (int)exportStream.Length);
            SPFile csvFile = docLibFolder.Files.Add(fileName, contents, false);
            exportStream.Close();
        }

        private MemoryStream BuildStreamToExport(List<SPListItem> listItems)
        {
            StringBuilder fileContents = new StringBuilder();
            StringBuilder headerContents = new StringBuilder();
            StringBuilder fieldContents = new StringBuilder();

            //string delimiter = ",";
            string delimiter = "\t";
            //string delimiter = "\x0009"; 

            // build our header line
            foreach (SPField fld in listItems[0].ContentType.Fields)
            {
                headerContents.Append(fld.Title + delimiter);
            }
            // add our header line to our file
            fileContents.AppendLine(headerContents.ToString());

            // build our file contents
            foreach (SPListItem item in listItems)
            {
                foreach (SPField fld in item.ContentType.Fields)
                {
                    //fieldContents.Append(item[fld.InternalName] == null ? string.Empty + delimiter : item[fld.InternalName].ToString() + delimiter);
                    if (item[fld.InternalName] == null)
                    {
                        fieldContents.Append(string.Empty + delimiter);
                    }
                    else
                    {
                        string strFldValue = item[fld.InternalName].ToString();
                        // replace delimiter characters first
                        //strFldValue = strFldValue.Replace(",", " ");
                        strFldValue = strFldValue.Replace("\t", " ");
                        fieldContents.Append(strFldValue + delimiter);
                    }
                }
                fileContents.AppendLine(fieldContents.ToString());
                fieldContents.Length = 0;
                fieldContents.Capacity = 0;
            }

            // create a file and return it to the caller
            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            writer.Write(fileContents);
            writer.Flush();

            return output;
        }
    }
}
