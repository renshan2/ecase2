<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MgrDashboardDueDates.ascx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Site.WebParts.MgrDashboardDueDates.MgrDashboardDueDates" %>



<div id="divDatesTasksList">
    <table border="0" cellspacing="0" cellpadding="0" width="100%">
        <tr class="ms-WPHeader">
            <td align="left" class="ms-WPHeaderTd">
                <h3 class="ms-standardheader ms-WPTitle"><a>
                    <nobr><span>Activities &amp; Tasks</span></nobr>
                </a></h3>
            </td>
        </tr>
        <tr style="background-color:white;">
            <td>&nbsp;</td>
        </tr>
        <tr style="background-color:white;">
            <td>
                &nbsp;Choose your date range:<br /><br />
            </td>
        </tr>
        <tr style="background-color:white;">
            <td>&nbsp;
                <asp:DropDownList ID="ddlNumOfDays" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlNumOfDays_SelectedIndexChanged">
                    <asp:ListItem Value="30">30 days</asp:ListItem>
                    <asp:ListItem Value="60">60 days</asp:ListItem>
                    <asp:ListItem Value="90">90 days</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr style="background-color:white;">
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td valign="top">
                <asp:GridView ID="GridViewDates" runat="server" Visible="true"
                    CssClass="ms-listviewtable" HeaderStyle-CssClass="ms-viewheadertr" RowStyle-CssClass="ms-itmhover" AlternatingRowStyle-CssClass="ms-alternating ms-itmhover"
                    AutoGenerateColumns="false" EmptyDataText="There is no data to show."
                    Width="100%">
                    <Columns>
                        <%--<asp:HyperLinkField HeaderText="Title" DataTextField="Title" DataNavigateUrlFields="ListId,ID" DataNavigateUrlFormatString="_layouts/listform.aspx?PageType=4&ListId={{{0}}}&ID={1}" ItemStyle-CssClass="ms-vb2" HeaderStyle-CssClass="ms-vh2" />--%>
                        <asp:BoundField HeaderText="Unique ID" DataField="CaseName" HeaderStyle-CssClass="ms-vh2" ItemStyle-Font-Bold="true" HeaderStyle-Font-Bold="false" />
                        <asp:BoundField HeaderText="Title" DataField="Title" ItemStyle-CssClass="ms-vb2" HeaderStyle-CssClass="ms-vh2" HeaderStyle-Font-Bold="false" />
                        <asp:BoundField HeaderText="Event Start Date" DataField="EventDate" ItemStyle-CssClass="ms-vb2" HeaderStyle-CssClass="ms-vh2" HeaderStyle-Font-Bold="false" />
                        <asp:BoundField HeaderText="Event End Date" DataField="EndDate" ItemStyle-CssClass="ms-vb2" HeaderStyle-CssClass="ms-vh2" HeaderStyle-Font-Bold="false" />
                        <asp:BoundField HeaderText="Task Start Date" DataField="StartDate" ItemStyle-CssClass="ms-vb2" HeaderStyle-CssClass="ms-vh2" HeaderStyle-Font-Bold="false" />
                        <asp:BoundField HeaderText="Task Due Date" DataField="DueDate" ItemStyle-CssClass="ms-vb2" HeaderStyle-CssClass="ms-vh2" HeaderStyle-Font-Bold="false" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</div>


