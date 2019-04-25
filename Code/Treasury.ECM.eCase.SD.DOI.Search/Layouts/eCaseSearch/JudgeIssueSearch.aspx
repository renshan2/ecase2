<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JudgeIssueSearch.aspx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Search.Layouts.eCaseSearch.JudgeIssueSearch" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register TagPrefix="Tax" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<table id="maintable" class="ms-propertysheet" border="0" cellspacing="0" cellpadding="0" width="100%">
        <tbody>
            <tr id="ctl00_PlaceHolderMain_ctl00_tablerow1">
                <td class="ms-sectionline" height="1" colspan="2">
                    <img alt="" src="/_layouts/images/blank.gif" width="1" height="1"></td>
            </tr>
            <tr id="ctl00_PlaceHolderMain_ctl00">
                <td class="ms-descriptiontext" valign="top" style="width: 300px;">
                    <table border="0" cellspacing="0" cellpadding="1" width="100%">
                        <tbody>
                            <tr>
                                <td><h3>Law Issue Search</h3></td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div style="display:none">
                                    <Tax:TaxonomyWebTaggingControl id="taxTest" runat="server" Width="200px"></Tax:TaxonomyWebTaggingControl>
                                    </div>
                                </td>

                            </tr>                            
                            <%--<tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Tax Court Judge:  <span style="color:red"><b>*</b></span>
                                </td>
                                <td>
                                    <img alt="" src="/_layouts/images/blank.gif" width="8" height="1"></td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription" colspan="2">
                                    <Tax:TaxonomyWebTaggingControl id="taxJudge" runat="server" Width="200px"></Tax:TaxonomyWebTaggingControl>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <img alt="" src="/_layouts/images/blank.gif" width="150" height="19"></td>
                            </tr>--%>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Law Issue List:  <span style="color:red"><b>*</b></span>
                                </td>
                                <td>
                                    <img alt="" src="/_layouts/images/blank.gif" width="8" height="1"></td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription" colspan="2">
                                    <Tax:TaxonomyWebTaggingControl id="lawIssue" runat="server" Width="200px"></Tax:TaxonomyWebTaggingControl>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <img alt="" src="/_layouts/images/blank.gif" width="150" height="19"></td>
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
                                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
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
            
            <tr>
               <td colspan="2">
                   <asp:Panel runat="server" ID="pnlResults" Visible="false">
                    Case owner results for cases containing Issue <b><asp:Literal runat="server" ID="litIssue"></asp:Literal></b>
                    <br/>
                    <ul>
                        <asp:Literal runat="server" ID="litOwners"></asp:Literal>
                    </ul>
                    </asp:Panel>
               </td> 

            </tr>
        </tbody>
    </table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Search Law Issue
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    Search Law Issue
</asp:Content>
