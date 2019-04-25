using System;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Utilities;

namespace Treasury.ECM.eCase.SusDeb.DOI.Site.Layouts.Treasury.ECM.eCase.SusDeb.DOI.Site
{
    public partial class CaseNewForm : WebPartPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // SPContext.Current.FormContext.OnSaveHandler += new EventHandler(onSave);                
        }

        private void onSave(object sender, EventArgs e)
        {            
            Page.Validate();
            if (Page.IsValid)
            {                
                try
                {
                    SaveButton btnSave = sender as SaveButton;
                    StringBuilder sbErrors = new StringBuilder();
                    SPLongOperation longOp = new SPLongOperation(this.Page);
                    longOp.LeadingHTML = "Please wait while the new case data is saved.";
                    longOp.Begin();
                    try
                    {
                        //SaveButton.SaveItem(btnSave.ItemContext, false, "");
                        bool isSaveSuccessful = false;
                        isSaveSuccessful = SaveButton.SaveItem(btnSave.ItemContext, false, "");
                        if (isSaveSuccessful == false)
                        {
                            sbErrors.Append("An error occurred.");
                        }                        
                    }
                    catch (SPException spex) 
                    {
                        sbErrors.Append("An error occurred: " + spex.Message);
                    }
                    catch (Exception ex)
                    {
                        sbErrors.Append("An error occurred: " + ex.Message);
                    }

                    if (SPContext.Current.IsPopUI)
                    {
                        if (sbErrors.Length > 0)
                        {
                            sbErrors.Append("<br/><br/>Please <a href='#' onclick='history.go(-1);return false;'>go back</a> and try again.");
                            longOp.EndScript("document.getElementById('s4-simple-card-content').innerHTML = \"<br/>Errors have occurred during the submission. Details: <br/>" + sbErrors.ToString() + " \";");
                        }
                        else
                        {
                            longOp.EndScript("try { window.frameElement.commitPopup(); } catch (e) {}");
                        }
                    }
                    else
                    {
                        if (sbErrors.Length > 0)
                        {
                            SPUtility.TransferToErrorPage(sbErrors.ToString());
                        }
                        else
                        {
                            longOp.End(SPContext.Current.Web.Url, SPRedirectFlags.DoNotEndResponse | SPRedirectFlags.Trusted, HttpContext.Current, "");
                        }                        
                    }
                }
                catch (ThreadAbortException) { /* Thrown when redirected */ }
                catch (Exception ex)
                {
                    SPUtility.TransferToErrorPage(ex.ToString());                    
                }              

            }            
        }        
    }
}
