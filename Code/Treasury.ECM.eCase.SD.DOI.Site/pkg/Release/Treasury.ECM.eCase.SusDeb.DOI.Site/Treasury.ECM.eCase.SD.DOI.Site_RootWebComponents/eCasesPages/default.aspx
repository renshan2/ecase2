<%@ Assembly Name="Treasury.ECM.eCase.SusDeb.DOI.Site, Version=1.0.0.0, Culture=neutral, PublicKeyToken=44198732fb780fac" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=14.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" MasterPageFile="Style Library/MasterPages/eCase_main.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <style type="text/css">
        body #s4-leftpanel {
            display: none;
        }

        .s4-ca {
            margin-left: 0px;
        }

        .searchlabel {
            font: 0/0 a;
            text-shadow: none;
            color: transparent;
        }

        .ecase-main-content-center-left .ms-vh-icon, .ecase-main-content-center-left .ms-vb-itmcbx, .ecase-main-content-center-left .ms-addnew, .ecase-main-content-center-left .ms-partline {
            display: none;
        }
    </style>
    <script type="text/javascript">
        if (typeof jQuery == "undefined") {
            var jQPath = "Style%20Library/Scripts/";
            document.write("<script src='", jQPath, "jquery-1.8.2.js' type='text/javascript'><\/script>");
        }
    </script>
    <script type="text/javascript" src="Style%20Library/Scripts/eCaseRootWebHomePageScripts.js"></script>
    <noscript>
        <style type="text/css">
            .ecase-main-content-center-left td.ms-addnew {
                display: table-cell;
            }

            .ecase-main-content-center-left a.ms-addnew {
                display: inline;
            }
        </style>
    </noscript>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div id="ecase-main-content">
        <div id="content-top-left" class="ecase-main-content-top-left">
            <asp:HyperLink ID="lnkAddCase" CssClass="ecase-long-add-link" Text="<i class='icon-plus-sign'></i> Add a New Case" runat="server" NavigateUrl="Lists/Cases/AllItems.aspx" />
        </div>
        <div id="content-top-right" class="ecase-main-content-top-right">
            <asp:HyperLink ID="lnkManagerDashboard" CssClass="ecase-long-search-link" Text="S&amp;D Case Dashboard" runat="server" NavigateUrl="managerdashboard.aspx" />
            <asp:HyperLink ID="lnkSearchCase" CssClass="ecase-long-search-link" Text="<i class='icon-search'></i> Search Cases" runat="server" NavigateUrl="search/" />
            <a href="javascript:openJudgeIssueSearch();" class="ecase-long-search-link"><i class='icon-search'></i> Search Law Issue</a>
        </div>
        <br class="clearfloat" />
        <div id="content-dashboard-top" class="ecase-main-content-dashboard-top">
            <div id="content-center-left" class="ecase-main-content-center-left">
                <h2 class="ecase-site-title"><i class="icon-list-alt"></i>&nbsp;&nbsp;&nbsp;My Dashboard</h2>
                <WebPartPages:WebPartZone runat="server" ID="wpzoneLeftColumn">
                    <ZoneTemplate></ZoneTemplate>
                </WebPartPages:WebPartZone>
            </div>
            <div id="content-center-right" class="ecase-main-content-center-right">
                <asp:HyperLink ID="lnkViewAdminPanel" CssClass="ecase-long-link" Text="View Admin Panel" runat="server" NavigateUrl="_layouts/settings.aspx" />
                <WebPartPages:WebPartZone runat="server" ID="wpzoneRightColumn">
                    <ZoneTemplate>
                    </ZoneTemplate>
                </WebPartPages:WebPartZone>
            </div>
            <br class="clearfloat" />
        </div>
        <div id="content-footer" class="ecase-main-content-footer">
        </div>
        <br class="clearfloat" />
    </div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    eCase Management Home
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    eCase Management Home
</asp:Content>
