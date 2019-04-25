<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SaveSearchResults.aspx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Search.Layouts.eCaseSearch.SaveSearchResults" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <SharePoint:CssRegistration ID="CssRegistration1" Name="<% $SPUrl:~sitecollection/Style Library/eCase-styles.css?rev=1 %>" After="corev4.css" runat="server" />
    <SharePoint:CssRegistration ID="CssRegistration2" Name="<% $SPUrl:~sitecollection/Style Library/font-awesome.css %>" After="corev4.css" runat="server" />
    <style type="text/css">        
        body #s4-leftpanel {
            display: none;
        }
        body {
            background-image: none !important;
        }
        .s4-ca {
            margin-left: 20px;
        }
        .ecase-long-link {
            width: 200px !important;
            margin: 5px !important;
            display: inline-block !important;
        }
    </style>
    <script type="text/javascript">
        /// <reference path="jquery-1.8.2.js" />

        function SaveSearchResultData(id, field, value) {
            $(document).ready(function () {
                $.ajax({
                    type: "POST",
                    url: "SaveSearchResults.aspx/SaveSearchResultData",
                    contentType: "application/json; charset=utf-8",
                    data: "{\"id\":\"" + id + "\", \"field\":\"" + field + "\", \"value\":\"" + value + "\"}",
                    dataType: "json",
                    success: SaveSearchResultDataSuccessful,
                    error: SaveSearchResultDataFailed
                });
            });
        }
        function SaveSearchResultDataSuccessful(result) {
            SP.UI.Notify.addNotification(result.d);
            hideErrorText();
        }
        function SaveSearchResultDataFailed(result) {
            SP.UI.Notify.addNotification(result.status + ' ' + result.statusText);
            errorText.innerText = result.status + ' ' + result.statusText;
            errorDetails.innerText = result.responseText;
            showErrorText();
        }

        function CopySelectedSearchResults(id) {
            if (id) {
                $(document).ready(function () {
                    var numChecked = $('input:checkbox[id*=includeInSetCheckBox]:checked').length;
                    if (numChecked == 0) {
                        alert("You must check at least one item to 'Include in Set'.");
                    }
                    else {
                        $.ajax({
                            type: "POST",
                            url: "SaveSearchResults.aspx/SendSelectedItemsToSession",
                            contentType: "application/json; charset=utf-8",
                            data: "{\"resultSetId\":\"" + id + "\"}",
                            dataType: "json",
                            success: CopySelectedSearchResultsSucceeded,
                            error: CopySelectedSearchResultsFailed
                        });
                    }

                });
            }
        }
        function CopySelectedSearchResultsSucceeded(result) {
            SP.UI.Notify.addNotification("Successfully copied " + result.d + " items.");
            // window.location = "SearchBatchCopying.aspx?serverItemsList=true";
            // open modal dialog instead?
            hideErrorText();
            SP.UI.ModalDialog.showModalDialog(
            {
                url: "SearchBatchCopying.aspx?serverItemsList=true",
                width: 900,
                height: 700,
                title: "Batch Copy Search Results"
            }
            );
        }
        function CopySelectedSearchResultsFailed(result) {
            SP.UI.Notify.addNotification(result.status + ' ' + result.statusText);
            errorText.innerText = result.status + ' ' + result.statusText;
            errorDetails.innerText = result.responseText;
            showErrorText();
        }

        function hideErrorText() {
            $("#errorTextContainer").hide();
            $("#errorDetails").hide();
        }

        function showErrorText() {
            $("#errorTextContainer").show();
            $("#errorDetails").hide();
            $('html, body').scrollTop(0);
        }

  </script>  
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:HiddenField ID="savedSearchResultsIdHidden" runat="server" />
    <div style="margin-top: 15px;">
        <h2 class="ecase-site-title"><i class="icon-save"></i>&nbsp;&nbsp;&nbsp;Save Search Results</h2>
    </div>
    <table id="maintable" class="ms-propertysheet" border="0" cellspacing="0" cellpadding="0" width="100%">
        <tbody>            
            <tr>
                <td>
                    <asp:ValidationSummary ID="valSummary" runat="server" ShowSummary="true" DisplayMode="BulletList" />
                    <div id="errorTextContainer" style="display:none;"><span id="errorText"></span><br />
                        <a href="#" onclick="$('#errorDetails').toggle();">Click to Show Details</a>
                    </div>
                    <div id="errorDetails" style="display:none;"></div>
                </td>
            </tr>
            <tr id="ctl00_PlaceHolderMain_ctl00">
                <td class="ms-descriptiontext" valign="top">
                    <table border="0" cellspacing="0" cellpadding="1" width="100%" id="existingSavedSearchResultsTable" style="display:none;">
                        <tbody>
                            <tr>
                                <td><h3>Saved Search Results</h3></td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription" colspan="2">
                                    <asp:DropDownList ID="savedSearchesDropDown" runat="server" ValidationGroup="ExistingSearch" />
                                    <asp:RequiredFieldValidator ID="savedSearchesDropDownValidator" ValidationGroup="ExistingSearch" ControlToValidate="savedSearchesDropdown"
                                         EnableClientScript="true" runat="server" ErrorMessage="Select a saved search to retrieve." Display="Dynamic" />
                                    <asp:Button ID="retrieveSavedSearchButton" runat="server" OnClick="retrieveSavedSearchButton_Click" Text="Retrieve Saved Search" ValidationGroup="ExistingSearch" />
                                </td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription" colspan="2">
                                    <asp:Label ID="messsageLabel" runat="server" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>            
            <tr>
                <td class="ms-descriptiontext"><img alt="" src="/_layouts/images/blank.gif" width="1" height="10" /></td>
            </tr>
            <tr>
                <td>
                    <table cellspacing="0" cellpadding="0" width="100%" id="newSavedSearchResultsTable">
                        <tbody>
                            <tr>
                                <td>
                                    <asp:LinkButton ID="returnToSearch" OnClick="returnToSearch_Click" Text="Return to Search" CssClass="ecase-long-link" runat="server" />
                                    <asp:LinkButton ID="returnToResultSetsLink" OnClick="returnToResultSetsLink_Click" Text="Manage Search Result Sets" CssClass="ecase-long-link" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td><img alt="" src="/_layouts/images/blank.gif" width="150" height="20" /></td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Original Query: <asp:Label ID="queryLabel" runat="server" /></td>
                            </tr>                            
                            <tr>
                                <td><img alt="" src="/_layouts/images/blank.gif" width="150" height="20" /></td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Name: </td>                                
                            </tr>
                             <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">
                                    <asp:TextBox ID="savedSearchNameTextBox" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="savedSearchNameTextBoxValidator" ValidationGroup="NewSearch" ControlToValidate="savedSearchNameTextBox"
                                         EnableClientScript="true" Text="Name is required to save search results." Display="Dynamic" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <img alt="" src="/_layouts/images/blank.gif" width="150" height="20" /></td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Description: </td>
                                <td><img alt="" src="/_layouts/images/blank.gif" width="8" height="1" /></td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">
                                    <asp:TextBox ID="savedSearchDescriptionTextBox" runat="server" Width="400" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="savedSearchDescriptionTextBoxValidator" ValidationGroup="NewSearch" ControlToValidate="savedSearchDescriptionTextBox"
                                         EnableClientScript="true" Text="Description is required to save search results." Display="Dynamic" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td><img alt="" src="/_layouts/images/blank.gif" width="150" height="20" /></td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Owner: </td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">
                                    <SharePoint:PeopleEditor ID="ownerPeopleEditor" EnableViewState="true" MultiSelect="false" SelectionSet="User" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td><img alt="" src="/_layouts/images/blank.gif" width="150" height="20" /></td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Share With:</td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">
                                    <SharePoint:PeopleEditor ID="shareWithPeopleEditor" Width="400" Height="5" MaximumHeight="10" EnableViewState="true" MultiSelect="true" SelectionSet="User" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td><img alt="" src="/_layouts/images/blank.gif" width="150" height="20" /></td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Rows identified by search (approximate): <span id="rowCountSpan" runat="server"></span></td>
                            </tr>
                            <tr>
                                <td><img alt="" src="/_layouts/images/blank.gif" width="150" height="20" /></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:LinkButton ID="saveSearchResultsButton" Text="Save Search Results" ValidationGroup="NewSearch" OnClick="saveSearchResultsButton_Click" CssClass="ecase-long-link" runat="server" />
                                    <asp:LinkButton ID="editSearchResultsButton" Text="Edit Search Results" OnClick="editSearchResultsButton_Click" CssClass="ecase-long-link" runat="server" />
                                    <asp:LinkButton ID="cancelEditButton" Text="Cancel Edit" OnClick="cancelEditButton_Click" CssClass="ecase-long-link" runat="server" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="ms-descriptiontext s4-notdlg"><img alt="" src="/_layouts/images/blank.gif" width="1" height="10" /></td>
            </tr>
        </tbody>
    </table>

    <div>
        <asp:HyperLink ID="copySelectedResults" runat="server" Text="Copy Selected Results" CssClass="ecase-long-link" />
        <asp:Literal ID="copySelectedResultsNote" runat="server"><p style="margin-top: 0px; font-style: italic;">Note that only the documents marked 'Include in Set' will be copied to the destination.</p><br /></asp:Literal>
    </div>

    <asp:Repeater ID="savedSearchResultsRepeater" runat="server" OnItemDataBound="savedSearchResultsRepeater_ItemDataBound">
        <HeaderTemplate>
            <table class="ms-listviewtable search-results-table" border="0" cellpadding="1" cellspacing="0">
                <tbody>
                    <tr>
                        <th class="search-results-headercell" scope="col">
                            Reviewed
                        </th>
                        <th class="search-results-headercell" scope="col">
                            Include in Set
                        </th>
                        <th class="search-results-headercell">&nbsp;</th>
                    </tr>            
        </HeaderTemplate>
        <ItemTemplate>
                    <tr class="ms-itmhover">
                        <td class="ms-vb-itmcbx ms-vb2 search-results-cell" style="text-align: center;">
                            <asp:CheckBox ID="reviewedCheckBox" runat="server" />
                        </td>
                        <td class="ms-vb-itmcbx ms-vb2 search-results-cell" style="text-align: center;">
                            <asp:CheckBox ID="includeInSetCheckBox" runat="server" />
                        </td>
                        <td class="search-results-cell">
                            <div class="srch-results">
                            <div style="clear: both;">
                                <div class="srch-Icon"><asp:Image ID="docTypeImage" runat="server" /></div>
                                <div class="srch-Title2">
                                    <div class="srch-Title3">
                                        &nbsp;<asp:HyperLink ID="titleLink" runat="server" />
                                    </div>
                                </div>
                                <div class="srch-Description2"><%# Eval("HitHighlightedSummary") %></div>
                                <div class="srch-Metadata2">Authors: <%# Eval("Author") %></div>
                                <div class="srch-Metadata2"><span class="srch-URL2">Site Url: <asp:HyperLink ID="siteLink" runat="server" NavigateUrl='<%# Eval("SpSiteUrl") %>' Text='<%# Eval("SpSiteUrl") %>' /></span></div>
                            </div>
                            </div>
                        </td>
                    </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>

 <div style="display: none;"><asp:DataGrid ID="searchResultsDataGrid" runat="server"></asp:DataGrid></div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Save Search Results
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
   Save Search Results
</asp:Content>
