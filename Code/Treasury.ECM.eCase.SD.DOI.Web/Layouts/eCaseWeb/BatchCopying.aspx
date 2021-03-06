﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BatchCopying.aspx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Web.Layouts.Treasury.ECM.eCase.SusDeb.DOI.Web.BatchCopying" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <b>
        <asp:Label runat="server" ID="lblInstructions" /></b><br />
    <asp:Label runat="server" ID="lblItems" /><br />
    <asp:Label runat="server" ID="lblErrors" CssClass="ms-error" /><br />
    <asp:TreeView runat="server" ID="treeView" />
    <table id="maintable" class="ms-propertysheet" border="0" cellspacing="0" cellpadding="0" width="100%">
        <tbody>
            <tr id="ctl00_PlaceHolderMain_ctl00_tablerow1">
                <td class="ms-sectionline" height="1" colspan="2">
                    <img alt="" src="/_layouts/images/blank.gif" width="1" height="1"></td>
            </tr>
            <tr id="ctl00_PlaceHolderMain_ctl00">
                <td class="ms-descriptiontext" valign="top">
                    <table border="0" cellspacing="0" cellpadding="1" width="100%">
                        <tbody>
                            <tr>
                                <td style="PADDING-TOP: 4px" class="ms-sectionheader" height="22" valign="top">
                                    <h3 class="ms-standardheader ms-inputformheader">Destination </h3>
                                </td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Specify a destination - the destination must be a URL to a SharePoint document library or document set.  Click the 'Validate' button to check for conflicts at the destination.<br>
                                </td>
                                <td>
                                    <img alt="" src="/_layouts/images/blank.gif" width="8" height="1"></td>
                            </tr>
                            <tr>
                                <td>
                                    <img alt="" src="/_layouts/images/blank.gif" width="150" height="19"></td>
                            </tr>
                        </tbody>
                    </table>
                </td>
                <td class="ms-authoringcontrols ms-inputformcontrols" valign="top" align="left">
                    <table border="0" cellspacing="0" cellpadding="0" width="100%">
                        <tbody>
                            <tr>
                                <td width="9">
                                    <img alt="" src="/_layouts/images/blank.gif" width="9" height="7"></td>
                                <td>
                                    <img alt="" src="/_layouts/images/blank.gif" width="150" height="7"></td>
                                <td width="10">
                                    <img alt="" src="/_layouts/images/blank.gif" width="10" height="1"></td>
                            </tr>
                            <tr>
                                <td>
                                    <td class="ms-authoringcontrols">
                                        <table class="ms-authoringcontrols" border="0" cellspacing="0" cellpadding="0" width="100%">
                                            <tbody>
                                                <tr id="ctl00_PlaceHolderMain_ctl00_ctl04_tablerow1">
                                                    <td class="ms-authoringcontrols" colspan="2"><span id="ctl00_PlaceHolderMain_ctl00_ctl04_LiteralLabelText"></span></td>
                                                </tr>
                                                <tr id="ctl00_PlaceHolderMain_ctl00_ctl04_tablerow2">
                                                    <td>
                                                        <img style="DISPLAY: block" alt="" src="/_layouts/images/blank.gif" width="1" height="3"></td>
                                                </tr>
                                                <!-- End Right_Text -->
                                                <tr id="ctl00_PlaceHolderMain_ctl00_ctl04_tablerow3">
                                                    <td width="11">
                                                        <img style="DISPLAY: block" alt="" src="/_layouts/images/blank.gif" width="11" height="1"></td>
                                                    <td class="ms-authoringcontrols" width="99%">
                                                        <table border="0" cellspacing="1">
                                                            <tbody>
                                                                <tr>
                                                                    <td class="ms-authoringcontrols" colspan="2" nowrap>Destination document library or folder&nbsp;(<a href="javascript:" onclick="javascript:TestDir();return false;">Click here to test)</a></td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="ms-authoringcontrols" width="100%" colspan="2">
                                                                        <asp:TextBox runat="server" CssClass="ms-input" ID="txtDestination" /><asp:RequiredFieldValidator ID="DestinationValidator" runat="server" ControlToValidate="txtDestination" ErrorMessage="* Required" /><asp:CustomValidator ID="UrlValidator" runat="server" ControlToValidate="txtDestination" OnServerValidate="UrlValidator_ServerValidate" ErrorMessage="This is an invalid URL." />
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr id="ctl00_PlaceHolderMain_ctl00_ctl04_tablerow5">
                                                    <td>
                                                        <img style="DISPLAY: block" alt="" src="/_layouts/images/blank.gif" width="1" height="6"></td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                    <td width="10">
                                        <img alt="" src="/_layouts/images/blank.gif" width="10" height="1"></td>
                            </tr>
                            <tr>
                                <td>
                                    <td>
                                        <img alt="" src="/_layouts/images/blank.gif" width="150" height="13"></td>
                                    <td></td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr id="ctl00_PlaceHolderMain_ctl01_tablerow1">
                <td class="ms-sectionline" height="1" colspan="2">
                    <img alt="" src="/_layouts/images/blank.gif" width="1" height="1"></td>
            </tr>

            <tr>
                <td class="ms-sectionline" height="2" colspan="2">
                    <img alt="" src="/_layouts/images/blank.gif" width="1" height="1"></td>
            </tr>
            <tr>
                <td class="ms-descriptiontext" height="10" colspan="2">
                    <img alt="" src="/_layouts/images/blank.gif" width="1" height="10"></td>
            </tr>
            <tr>
                <td colspan="2">
                    <table cellspacing="0" cellpadding="0" width="100%">
                        <colgroup>
                            <col width="99%">
                            <col width="1%">
                        </colgroup>
                        <tbody>
                            <tr>
                                <td>&nbsp;</td>
                                <td nowrap>
                                    <asp:Button runat="server" ID="btnValidate" class="ms-ButtonHeightWidth" Text="Validate Files" OnClick="btnValidate_Click" />&nbsp;<asp:Button runat="server" ID="btnSave" CausesValidation="true" class="ms-ButtonHeightWidth" Text="Copy" OnClick="btnSave_Click" />&nbsp;<asp:Button runat="server" CausesValidation="false" class="ms-ButtonHeightWidth" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="ms-descriptiontext s4-notdlg" height="40" colspan="2">
                    <img alt="" src="/_layouts/images/blank.gif" width="1" height="40"></td>
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
    Batch Copy
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Batch Copy
</asp:Content>
