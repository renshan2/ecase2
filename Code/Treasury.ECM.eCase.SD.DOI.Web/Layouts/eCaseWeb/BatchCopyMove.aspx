<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BatchCopyMove.aspx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Web.Layouts.Treasury.ECM.eCase.SusDeb.DOI.Web.BatchCopyMove" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript">
            if (typeof jQuery == "undefined") {
                var jQPath = "../../../Style%20Library/Scripts/";
                document.write("<script src='", jQPath, "jquery-1.8.2.js' type='text/javascript'><\/script>");
            }
    </script>
    <script type="text/javascript">
        // Load Scripts for document.ready  
        _spBodyOnLoadFunctionNames.push('eCaseReadyFunction');

        function eCaseReadyFunction() {
            // Hide First TreeView Node (Site Collection Node)
            $('#ctl00_PlaceHolderMain_treeView').find('table').eq(0).css('display', 'none');

            var strVisible = $('#ctl00_PlaceHolderMain_hidOtherLocationVisible').val();

            if (strVisible == 'true') {
                // Show Other Location TextBox
                $('#table-enter-location').show();
            }
            else {
                // Hide Other Location TextBox
                $('#table-enter-location').hide();
            }
            // Other Location Checkbox Toggle
            $('#ctl00_PlaceHolderMain_chkboxEnterLocation').bind("change", function () {
                $('#table-enter-location').toggle();
                if ($('#ctl00_PlaceHolderMain_hidOtherLocationVisible').val() == 'true') {
                    $('#ctl00_PlaceHolderMain_hidOtherLocationVisible').val('false');
                }
                else {
                    $('#ctl00_PlaceHolderMain_hidOtherLocationVisible').val('true');
                }
            });
        }

    </script>
    <SharePoint:CssRegistration ID="CssRegistration1" Name="<% $SPUrl:~sitecollection/Style Library/eCase-styles.css?rev=1 %>" After="corev4.css" runat="server" />
    <SharePoint:CssRegistration ID="CssRegistration2" Name="<% $SPUrl:~sitecollection/Style Library/font-awesome.css %>" After="corev4.css" runat="server" />
    <style type="text/css">
        .s4-ca { background: transparent !important; }
        #ctl00_PlaceHolderMain_hidOtherLocationVisible { display: none; }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <strong><asp:Label runat="server" ID="lblInstructions" /></strong><br />
    <asp:Label runat="server" ID="lblErrors" CssClass="ms-error" /><br />
    <asp:HiddenField ID="hidOtherLocationVisible" runat="server" />
    <table id="maintable" class="ms-propertysheet" border="0" cellspacing="0" cellpadding="0" width="100%">
        <tbody>
            <tr>
                <td class="ecase-copy-heading" colspan="2"><h2>Copy / Move Documents</h2></td>
            </tr>
            <tr>
                <td class="ms-descriptiontext ecase-selected-docs-cell" valign="top">
                    <table border="0" cellspacing="0" cellpadding="1" width="100%">
                        <tbody>
                            <tr>
                                <td style="padding-top: 4px" class="ms-sectionheader" height="22" valign="top">
                                    <h3 class="ms-standardheader ms-inputformheader">Selected Documents</h3>
                                </td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Below is a list of documents that you have selected. To prevent a file from being copied/moved, simply uncheck the box next to the file name.<br />
                                </td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">
                                    <asp:TreeView runat="server" ID="treeViewSelectedDocs" />
                                </td>
                            </tr>
                            <tr>
                                <td><img alt="" src="/_layouts/images/blank.gif" width="150" height="19" /></td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">
                                    <asp:Label runat="server" ID="lblItems" /><br />
                                </td>
                            </tr>
                            <tr>
                                <td><img alt="" src="/_layouts/images/blank.gif" width="150" height="19" /></td>
                            </tr>
                        </tbody>
                    </table>
                </td>
                <td class="ms-authoringcontrols ms-inputformcontrols" valign="top" align="left">
                    <table class="ms-authoringcontrols" border="0" cellspacing="0" cellpadding="0" width="100%">
                        <tbody>                            
                            <tr>
                                <td width="11"><img style="display: block" alt="" src="/_layouts/images/blank.gif" width="11" height="1" /></td>
                                <td class="ms-authoringcontrols">                                    
                                    <table border="0" border="0" cellspacing="0" cellpadding="1">
                                        <tbody>
                                            <tr>
                                                <td style="padding-top: 4px" class="ms-sectionheader" height="22" valign="top">
                                                    <h3 class="ms-standardheader ms-inputformheader">Destination </h3>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="ms-descriptiontext ms-inputformdescription">Select a destination below, or check the box to manually enter a destination.  Click the 'Validate' button to ensure the selected files do not exist at the chosen destination.<br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="ms-authoringcontrols" width="100%">
                                                    <asp:TreeView runat="server" ID="treeView" RootNodeStyle-Font-Bold="true" OnSelectedNodeChanged="treeView_SelectedNodeChanged">
                                                        <NodeStyle CssClass="ecase-tree-node" />
                                                        <HoverNodeStyle CssClass="ecase-tree-node-hover"/>
                                                        <SelectedNodeStyle CssClass="ecase-tree-node-selected"/>
                                                    </asp:TreeView>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                    <!--<input type="checkbox" id="chkbox-enter-location" />-->
                                    <asp:CheckBox ID="chkboxEnterLocation" runat="server" /><label for="ctl00_PlaceHolderMain_chkboxEnterLocation" class="ms-standardheader ms-inputformheader chkbox-enter-location-label">Enter Other Location</label>
                                    <table id="table-enter-location" border="0" border="0" cellspacing="0" cellpadding="1">
                                        <tbody>
                                            <tr>
                                                <td class="ms-descriptiontext ms-inputformdescription">The destination must be a URL to a SharePoint document library or document set.<br />( Example: http://www.example.gov/sites/MyBureau/MyCase/Lists/ReferralDocuments )<br />
                                                </td>                                                
                                            </tr>
                                            <tr>
                                                <td class="ms-authoringcontrols">
                                                    <asp:TextBox runat="server" CssClass="ms-input" ID="txtDestination" Width="450px" /><br />(<a href="javascript:" onclick="javascript:TestDir();return false;">Click here to test destination in a new window</a>)<asp:RequiredFieldValidator ID="DestinationValidator" runat="server" ControlToValidate="txtDestination" ErrorMessage="* Required" /><asp:CustomValidator ID="UrlValidator" runat="server" ControlToValidate="txtDestination" OnServerValidate="UrlValidator_ServerValidate" ErrorMessage="This is an invalid URL." />
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2"><img style="display: block" alt="" src="/_layouts/images/blank.gif" width="1" height="6" /></td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="ms-sectionline" height="1" colspan="2"><img alt="" src="/_layouts/images/blank.gif" width="1" height="1" /></td>
            </tr>
            <tr>
                <td height="10" colspan="2" class="ecase-white-cell"><img alt="" src="/_layouts/images/blank.gif" width="1" height="10" /></td>
            </tr>
            <tr>
                <td colspan="2" class="ecase-white-cell">
                    <table cellspacing="0" cellpadding="0" width="100%">
                        <tbody>
                            <tr>
                                <td align="center">
                                    <asp:LinkButton runat="server" ID="btnValidate" OnClick="btnValidate_Click" CssClass="ecase-long-link ecase-copy-button"><i class="icon-check"></i> Validate</asp:LinkButton>
                                    <asp:LinkButton runat="server" ID="btnCopy" CausesValidation="true" OnClick="btnSave_Click" CssClass="ecase-long-link ecase-copy-button"><i class="icon-copy"></i> Copy</asp:LinkButton>
                                    <asp:LinkButton runat="server" ID="btnMove" CausesValidation="true" OnClick="btnSave_Click" CssClass="ecase-long-link ecase-copy-button"><i class="icon-share-alt"></i> Move</asp:LinkButton>
                                    <asp:LinkButton runat="server" CausesValidation="false" ID="btnCancel" OnClientClick="javascript:window.frameElement.commonModalDialogClose(0, 'Cancelled');" CssClass="ecase-long-link ecase-copy-button"><i class="icon-ban-circle"></i> Cancel</asp:LinkButton>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>            
            <tr>
                <td height="10" colspan="2" class="ecase-white-cell"><img alt="" src="/_layouts/images/blank.gif" width="1" height="10" /></td>
            </tr>
            <tr>
                <td class="ms-sectionline" height="1" colspan="2"><img alt="" src="/_layouts/images/blank.gif" width="1" height="1" /></td>
            </tr>
            <tr>
                <td height="40" colspan="2"><img alt="" src="/_layouts/images/blank.gif" width="1" height="40" /></td>
            </tr>
        </tbody>
    </table>
    <script type="text/javascript">
        function TestDir() {
            var form = document.forms.aspnetForm;
            var folderUrl = form.ctl00$PlaceHolderMain$txtDestination.value;
            if (folderUrl == null ||
                typeof (folderUrl) == "undefined") {
                alert("Please enter a valid url. The url must be a folder with a prefix of http:// or https://.");
                return;
            }
            try {
                var form = document.forms.aspnetForm;
                var url = form.ctl00$PlaceHolderMain$txtDestination.value;
                window.open(url, '_blank');
            }
            catch (e) {
                alert(L_EnterValidCopyDest_Text);
            }
        }

    </script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Batch Copy/Move
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Batch Copy/Move
</asp:Content>
