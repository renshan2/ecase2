<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ECaseAuditUC.ascx.cs" Inherits="ECaseAuditWebPart.VisualWebPart1.ECaseAuditUC" %>
<style type="text/css">
    .style1
    {
        width: 100%;
        border-left: 1px solid #808080;
        border-right-style: solid;
        border-right-width: 1px;
        border-top: 1px solid #808080;
        border-bottom-style: solid;
        border-bottom-width: 1px;
    }
    .style3
    {
        width: 195px;
        font-family: Calibri;
        text-align: left;
    }
    .style4
    {
        font-family: Calibri;
    }
    .style5
    {
        font-family: Calibri;
        width: 250px;
    }
    .style7
    {
        width: 250px;
    }
    .style8
    {
        font-weight: bold;
    }
    .style9
    {
        width: 195px;
        font-family: Calibri;
        text-align: left;
        font-weight: normal;
    }
</style>

<table cellpadding="0" cellspacing="0" class="style1">
    <tr>
        <td class="style3" valign="top">
            &nbsp;</td>
        <td class="style7">
            &nbsp;</td>
    </tr>
    <tr>
        <td class="style3" valign="top">
            Report Start Date:</td>
        <td class="style7">
            <asp:TextBox ID="txtSelectedStartDate" runat="server" 
                BorderStyle="None" ReadOnly="True" CssClass="style8" 
                style="color: #808080; font-size: small" Width="103px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style3" valign="top">
            &nbsp;</td>
        <td class="style7">
            <asp:Calendar ID="calStartDate" runat="server" 
                style="font-family: Calibri; margin-left: 0px;" 
                onselectionchanged="calStartDate_Change"></asp:Calendar>
        </td>
    </tr>
    <tr>
        <td class="style3" valign="top">
            &nbsp;</td>
        <td class="style7">
            &nbsp;</td>
    </tr>
    <tr>
        <td class="style3" valign="top">
            Report
            End Date:</td>
        <td class="style7">
            <asp:TextBox ID="txtSelectedEndDate" runat="server" 
                BorderStyle="None" 
                ReadOnly="True" 
                style="color: #808080; font-size: small; font-weight: 700;" Width="103px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style3" valign="top">
            &nbsp;</td>
        <td class="style7">
            <asp:Calendar ID="calEndDate" runat="server" style="font-family: Calibri" 
                onselectionchanged="calEndDate_Change"></asp:Calendar>
        </td>
    </tr>
    <tr>
        <td class="style3">
            &nbsp;</td>
        <td class="style5">
            &nbsp;</td>
    </tr>
    <tr>
        <td class="style3">
            Select a document library:</td>
        <td class="style5" valign="top">
            <asp:DropDownList ID="ddlDocumentLibraries" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="style3">
            &nbsp;</td>
        <td class="style5">
            &nbsp;</td>
    </tr>
    <tr>
        <td class="style3">
            &nbsp;</td>
        <td class="style7">
            <asp:Button ID="btnGenerate" runat="server" Text="Generate Audit Report" 
                CssClass="style4" onclick="btnGenerate_Click" />
        </td>
    </tr>
</table>