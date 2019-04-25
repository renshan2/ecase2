<%@ Assembly Name="Treasury.ECM.eCase.SusDeb.DOI.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1dad96f5b8a688f6" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="DocWebControls" Namespace="Microsoft.Office.Server.WebControls" Assembly="Microsoft.Office.DocumentManagement, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuditReport.aspx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Web.Layouts.AuditReport" MasterPageFile="~/_layouts/application.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <style type="text/css">
        body #s4-leftpanel {
            display: none;
        }

        .s4-ca {
            margin-left: 0px;
        }

        .ecase-main-content-center-right table {
            text-align: left;
        }
    </style>
   
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
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<table cellpadding="0" cellspacing="0" class="style1">
    <tr>
        <td colspan="2">
            <asp:ValidationSummary ID="validationSummaryControl" runat="server" ValidationGroup="AuditReportValidationGroup" />
            <asp:CustomValidator ID="miscValidationLogicValidator" runat="server" Display="None" OnServerValidate="miscValidationLogicValidator_ServerValidate" ValidationGroup="AuditReportValidationGroup" />
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
            Report Start Date:</td>
        <td class="style7">
            <asp:TextBox ID="selectedStartDateTextBox" runat="server"
                BorderStyle="None" ReadOnly="True" CssClass="style8" 
                style="color: #808080; font-size: small" Width="103px" />
        </td>
    </tr>
    <tr>
        <td class="style3" valign="top">
            &nbsp;</td>
        <td class="style7">
            <asp:Calendar ID="startDateCalendar" runat="server"
                style="font-family: Calibri; margin-left: 0px;" 
                onselectionchanged="startDateCalendar_SelectionChanged" />
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
            <asp:TextBox ID="selectedEndDateTextBox" runat="server"
                BorderStyle="None" 
                ReadOnly="True" 
                style="color: #808080; font-size: small; font-weight: 700;" Width="103px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style3" valign="top">
            &nbsp;</td>
        <td class="style7">
            <asp:Calendar ID="endDateCalendar" runat="server" style="font-family: Calibri" 
                onselectionchanged="endDateCalendar_SelectionChanged" />
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
            Select a library to save the report:</td>
        <td class="style5" valign="top">
            <asp:DropDownList ID="documentLibrariesDropDown" runat="server" />
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
            <asp:Button ID="generateButton" runat="server" Text="Generate Audit Report" 
                CssClass="style4" onclick="generateButton_Click" CausesValidation="true" ValidationGroup="AuditReportValidationGroup"/>
        </td>
    </tr>
</table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    eCase Audit Report
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    eCase Audit Report
</asp:Content>
