<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SaveSearchResultsWebPart.ascx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Search.SaveSearchResultsWebPart.SaveSearchResultsWebPart" %>

<input type="hidden" runat="server" id="hdnSearchUrl" />
<input type="hidden" runat="server" id="hdnSearchKeywords" />

<asp:LinkButton ID="saveSearchResultsButton" CssClass="ecase-search-link" runat="server" OnClick="saveSearchResultsButton_Click"><i class="icon-save"></i> Save Search Results</asp:LinkButton>&nbsp;&nbsp<asp:LinkButton ID="manageSavedSearchResultsButton" runat="server" CssClass="ecase-search-link" OnClick="manageSavedSearchResultsButton_Click"><i class="icon-list-alt"></i> Manage Saved Results</asp:LinkButton>