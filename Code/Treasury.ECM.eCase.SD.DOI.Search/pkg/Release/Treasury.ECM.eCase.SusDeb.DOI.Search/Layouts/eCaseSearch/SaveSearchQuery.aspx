<%@ Assembly Name="Treasury.ECM.eCase.SusDeb.DOI.Search, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4e7a656e7f3196c4" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SaveSearchQuery.aspx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Search.Layouts.eCaseSearch.SaveSearchQuery" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <SharePoint:CssRegistration ID="CssRegistration1" Name="<% $SPUrl:~sitecollection/Style Library/eCase-styles.css?rev=1 %>" After="corev4.css" runat="server" />
    <SharePoint:CssRegistration ID="CssRegistration2" Name="<% $SPUrl:~sitecollection/Style Library/font-awesome.css %>" After="corev4.css" runat="server" />
    <style type="text/css">
        .s4-ca { background: transparent !important; }
        #ctl00_PlaceHolderMain_hidOtherLocationVisible { display: none; }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div style="margin-top: 15px;">
        <h2 class="ecase-site-title"><i class="icon-save"></i>&nbsp;&nbsp;&nbsp;Save Search Query</h2>
    </div>
    <table id="maintable" class="ms-propertysheet" border="0" cellspacing="0" cellpadding="0" width="100%">
        <tbody>            
            <tr id="ctl00_PlaceHolderMain_ctl00">
                <td class="ms-descriptiontext" valign="top" style="width: 300px;">
                    <table border="0" cellspacing="0" cellpadding="1" width="100%">
                        <tbody>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Title:</td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">
                                    <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox><asp:RequiredFieldValidator ID="rfdQueryTitle" runat="server" ControlToValidate="txtTitle" ErrorMessage="* Required" />
                                </td>
                            </tr>
                            <tr>
                                <td><img alt="" src="/_layouts/images/blank.gif" width="150" height="19"></td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Description:</td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">
                                    <asp:TextBox ID="txtDescription" runat="server" Width="300"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td><img alt="" src="/_layouts/images/blank.gif" width="150" height="19"></td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Share With:</td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">
                                    <SharePoint:PeopleEditor ID="spPeoplePicker" runat="server" MultiSelect="true"  />
                                </td>
                            </tr>
                            <tr>
                                <td><img alt="" src="/_layouts/images/blank.gif" width="150" height="19"></td>
                            </tr>
                        </tbody>
                    </table>
                </td>
                <td>&nbsp;</td>
            </tr>            
            <tr>
                <td colspan="2">
                    <table cellspacing="0" cellpadding="0" width="100%">
                        <tbody>
                            <tr>
                                <td>&nbsp;</td>
                                <td nowrap align="right">
                                    <asp:LinkButton ID="btnSave" runat="server" Text="Save Query" CssClass="ecase-long-link"  OnClick="btnSave_Click" />
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


</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Save Search Query
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Save Search Query
</asp:Content>
