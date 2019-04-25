using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using System.Text;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Globalization;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search.Layouts.eCaseSearch
{
    public partial class SearchBatchMetadata : LayoutsPageBase
    {
        protected List<string> selectedSearchItems;
        protected List<SPList> selectedLists;
        protected List<SPListItem> selectedListItems;
        protected List<SPField> fieldsToUpdate;

        StringBuilder errorMessages = null;

        protected override void OnLoad(EventArgs e)
        {
            if (IsPostBack)
            {
                var a = 1;
            }

            try
            {
                if (Request.QueryString["items"] != null && Request.QueryString["sourceurl"] != null)
                {
                    string[] items = Request.QueryString["items"].ToString().Split('|');
                    string currentWeb = Request.QueryString["sourceurl"].Replace("'", string.Empty);

                    lblInstructions.Text = "The following items have been selected for metadata changes. <br><br>";

                    selectedSearchItems = new List<string>();
                    selectedLists = new List<SPList>();
                    selectedListItems = new List<SPListItem>();
                    fieldsToUpdate = new List<SPField>();

                    //start at 1 due to items split containing a leading empty value
                    for (int i = 1; i < items.Length; i++)
                    {
                        if (items[i] != null && items[i] != string.Empty)
                        {
                            //let's the Url strings for what our user's selected
                            selectedSearchItems.Add(items[i]);
                            lblItems.Text += items[i] + "<br />";
                            using (SPSite site = new SPSite(items[i]))
                            {
                                using (SPWeb web = site.OpenWeb())
                                {
                                    //now let's get our selected list
                                    selectedLists.Add(web.GetList(items[i]));
                                    //and our selected list items
                                    selectedListItems.Add(web.GetListItem(items[i]));
                                }
                            }
                        }
                    }

                    // check to see what fields are common across all of the selected lists

                    int j = 0;  // counter for the number of lists the field is contained in
                    foreach (SPListItem item in selectedListItems)
                    {
                        foreach (SPField field in item.Fields)
                        {
                            try
                            {
                                // loop through all of our selected lists and if we find
                                // the field in all our lists, increment our counter
                                foreach (SPList list in selectedLists)
                                {
                                    if (list.Fields.ContainsField(field.InternalName) && !field.Hidden && !field.ReadOnlyField && (field.CanBeDisplayedInEditForm == null || field.CanBeDisplayedInEditForm == true))
                                        j++;
                                }
                                // if our counter is equal to the number of lists selected
                                // then add the field for processing
                                if (j == selectedLists.Count && !isInUpdateList(field))
                                {
                                    fieldsToUpdate.Add(field);
                                    AddFormControl(field, item, field.ParentList, field.ParentList.ParentWeb);
                                    // reset our counter
                                    j = 0;
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                    }
                }
            }
            catch (ArgumentException argEx)
            { }
            catch (Exception ex)
            {
                Logger.Instance.Error("Error while loading items for batch metadata.", ex, DiagnosticsCategories.eCaseSearch);
            }
        }

        private bool isInUpdateList(SPField fieldToAdd)
        {
            bool retVal = false;
            foreach (SPField field in fieldsToUpdate)
            {
                if (field.ToString() == fieldToAdd.ToString())
                {
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }

        private void AddFormControl(SPField field, SPListItem listItem, SPList list, SPWeb spWeb)
        {
            // our table row
            HtmlTableRow newRow = new HtmlTableRow();
            HtmlTableCell lblCell = new HtmlTableCell();
            HtmlTableCell controlCell = new HtmlTableCell();

            // our form control types
            TaxonomyWebTaggingControl taxonomyControl = null;
            BaseFieldControl webControl = null;
            DateTimeControl dateTimeControl = null;

            using (SPSite site = list.ParentWeb.Site)
            {
                using (SPWeb web = site.OpenWeb(spWeb.ID))
                {
                    if (field.FieldRenderingControl != null && !skipField(field))
                    {
                        //add our label to the table
                        System.Web.UI.WebControls.Label newLabel = new System.Web.UI.WebControls.Label();
                        newLabel.Width = Unit.Pixel(150);
                        newLabel.Text = field.Title;
                        lblCell.Controls.Add(newLabel);
                        newRow.Cells.Add(lblCell);

                        try
                        {
                            switch (field.FieldRenderingControl.GetType().Name)
                            {
                                case ("DateTimeField"):
                                    dateTimeControl = new DateTimeControl();
                                    dateTimeControl.DateOnly = true;
                                    dateTimeControl.ID = string.Format("ctrl_{0}", field.InternalName);
                                    break;
                                case ("TaxonomyFieldControl"):
                                    TaxonomySession session = new TaxonomySession(field.ParentList.ParentWeb.Site);
                                    var store = session.TermStores[0];

                                    taxonomyControl = new TaxonomyWebTaggingControl();
                                    taxonomyControl.IsMulti = true;
                                    taxonomyControl.IsAddTerms = true;
                                    taxonomyControl.TermSetId.Add(session.TermStores[0].Id);
                                    taxonomyControl.ID = string.Format("ctrl_{0}", field.InternalName);
                                    taxonomyControl.FieldName = field.Title;
                                    taxonomyControl.FieldId = field.Id.ToString();
                                    taxonomyControl.SSPList = ((TaxonomyField)field).SspId.ToString();
                                    taxonomyControl.AnchorId = ((TaxonomyField)field).AnchorId;
                                    taxonomyControl.TermSetList = ((TaxonomyField)field).TermSetId.ToString();
                                    break;
                                default:
                                    webControl = field.FieldRenderingControl;
                                    webControl.ID = string.Format("ctrl_{0}", field.InternalName);
                                    webControl.ControlMode = SPControlMode.New;
                                    webControl.ListId = list.ID;
                                    webControl.FieldName = field.InternalName;
                                    SPContext Context = SPContext.GetContext(HttpContext.Current, list.Items.GetItemById(listItem.ID).ID, list.ID, web);
                                    webControl.RenderContext = Context;
                                    webControl.ItemContext = Context;
                                    break;
                            }

                            //add our new row with controls to our placeholder
                            phDynamicFormControls.Controls.Add(newRow);
                            // add the cell into our row
                            newRow.Cells.Add(controlCell);
                            // add the row to the table
                            phDynamicFormControls.Controls.Add(newRow);
                            if (webControl != null)
                                controlCell.Controls.Add(webControl);
                            else if (taxonomyControl != null)
                                controlCell.Controls.Add(taxonomyControl);
                            else if (dateTimeControl != null)
                                controlCell.Controls.Add(dateTimeControl);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
        }

        private bool skipField(SPField field)
        {
            bool retVal = false;

            switch (field.Title)
            {
                case ("Document ID"):
                    retVal = true;
                    break;
                case("Document ID Value"):
                    retVal = true;
                    break;
                default:
                    break;
            }

            return retVal;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    Control control = null;
                    string controlValue = null;

                    foreach (SPListItem listItem in selectedListItems)
                    {
                        listItem.ParentList.ParentWeb.AllowUnsafeUpdates = true;
                        foreach (SPField field in fieldsToUpdate)
                        {
                            control = phDynamicFormControls.FindControl(string.Format("ctrl_{0}", field.InternalName));
                            controlValue = GetControlValue(control);
                            
                            if (control != null && GetControlValue(control) != string.Empty)
                            {
                                if (control.GetType().Equals(typeof(Microsoft.SharePoint.Taxonomy.TaxonomyWebTaggingControl)))
                                {
                                    TaxonomyField fld = (TaxonomyField)field;
                                    if (fld.AllowMultipleValues)
                                    {
                                        var values = new TaxonomyFieldValueCollection(field);
                                        values.PopulateFromLabelGuidPairs(controlValue);
                                        fld.SetFieldValue(listItem, values);
                                    }
                                }
                                else
                                {
                                    listItem[field.Id] = controlValue;
                                }
                            }
                        }
                        listItem.Update();
                        listItem.ParentList.ParentWeb.AllowUnsafeUpdates = false;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Error while saving fields during batch moving.", ex, DiagnosticsCategories.eCaseSearch);
            }

            //show any error messages that we have
            lblErrors.Text = errorMessages == null ? null : errorMessages.ToString();

            if (errorMessages == null || errorMessages.Length == 0)
            {
                //close the modal
                Response.Write(string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">window.frameElement.commonModalDialogClose(1, '{0}');</script>", selectedLists.Count));
                Response.Flush();
                Response.End();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //close the window on cancel
            Response.Write(string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">window.frameElement.commonModalDialogClose(0, '{0}');</script>", selectedLists.Count));
            Response.Flush();
            Response.End();
        }

        private string GetControlValue(Control SPCtrl)
        {
            if (SPCtrl is TextField)
                return ((TextField)SPCtrl).Text;

            if (SPCtrl is NoteField)
                return ((NoteField)SPCtrl).Text;

            if (SPCtrl is RichTextField)
                return ((RichTextField)SPCtrl).Text;

            if (SPCtrl is CheckBoxChoiceField)
                return ((CheckBoxChoiceField)SPCtrl).Value.ToString();

            if (SPCtrl is DropDownChoiceField)
                return ((DropDownChoiceField)SPCtrl).Value.ToString();

            if (SPCtrl is RadioButtonChoiceField)
                return ((RadioButtonChoiceField)SPCtrl).Value.ToString();

            if (SPCtrl is NumberField)
                return ((NumberField)SPCtrl).Text;

            if (SPCtrl is CurrencyField)
                return ((CurrencyField)SPCtrl).Text;

            if (SPCtrl is DateTimeControl)
                {
                    if (((DateTimeControl)SPCtrl).SelectedDate == null)
                        return string.Empty;
                    else
                        return ((DateTimeControl)SPCtrl).SelectedDate.ToShortDateString();
                }


            if (SPCtrl is LookupField)
                return ((LookupField)SPCtrl).Value.ToString();

            if (SPCtrl is BooleanField)
                return ((BooleanField)SPCtrl).Value.ToString();

            if (SPCtrl is CalculatedField)
                return ((CalculatedField)SPCtrl).Value.ToString();

            if (SPCtrl is TaxonomyWebTaggingControl)
                return ((TaxonomyWebTaggingControl)SPCtrl).Text;

            // if we have no match
            return string.Empty;
        }

    }
}
