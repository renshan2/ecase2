<%@ Assembly Name="Treasury.ECM.eCase.SusDeb.DOI.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1dad96f5b8a688f6" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="DocWebControls" Namespace="Microsoft.Office.Server.WebControls" Assembly="Microsoft.Office.DocumentManagement, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%-- CodeBehind="Documents.aspx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Web.CaseSiteDefinition.Modules.eCasePages.DocumentsPage, --%>
<%@ Page Language="C#" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=14.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" MasterPageFile="../Style Library/MasterPages/eCase_main.master" %>

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
    <script type="text/javascript">
        if (typeof jQuery == "undefined") {
            var jQPath = "../Style%20Library/Scripts/";
            document.write("<script src='", jQPath, "jquery-1.8.2.js' type='text/javascript'><\/script>");
        }
    </script>
    <script type="text/javascript" src="../Style%20Library/Scripts/jquery.SPServices-0.7.2.min.js"></script>
    <script type="text/javascript" src="../Style%20Library/Scripts/jquery-ui.js"></script>
    <script type="text/javascript" src="../Style%20Library/Scripts/jquery.cookie.js"></script>
    <script type="text/javascript" src="../Style%20Library/Scripts/jquery.dotdotdot.min.js"></script>
    <link href="../Style%20Library/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="../Style%20Library/eCase-tab-styles.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        // Load Scripts for document.ready  
        _spBodyOnLoadFunctionNames.push('eCaseReadyFunction');

        function eCaseReadyFunction() {
            // Fix display issue with empty list tables
            if ($('table.ms-summarystandardbody > tbody > tr > td.ms-vb:contains("There are no items")').length) {
                $('td.ms-vb:contains("There are no items")').closest('table.ms-summarystandardbody').css('table-layout', 'auto');
            }
            // Load Tab Scripts
            $("#subtabs").each(function () {
                var tabid = $(this).attr("tabid");
                if (tabid == undefined || tabid == null || tabid == "") {
                    $(this).tabs({
                        cookie: {
                            expires: null
                        }
                    });
                }
                else {
                    $(this).tabs({
                        cookie: {
                            expires: null,
                            name: tabid
                        }
                    });
                }
            });
            // Truncate note text 
            var divht = 28;
            var thisht = 0;
            $('.note-truncated a').each(function () {
                var noteA = $(this);
                thisht = $(noteA).outerHeight();
                if (thisht > 26) {
                    // Note text is greater than two lines, truncate with ellipsis
                    $(noteA).parent().dotdotdot({
                        wrap: 'letter'
                    });
                } else if (thisht < 14) {
                    // Note text is only one line, reduce height of container
                    $(noteA).parent().css("height", "15px");
                }
            });
            // Get CaseID query string parameter
            // JSRequest.EnsureSetup();
            // var strCaseID = JSRequest.QueryString["CaseID"];
            var strCaseID = '';
            strCaseID = GetUrlKeyValue('CaseID');
            // Append CaseID querystring to all tab hrefs (except anchors)
            var querystringTabs = '?CaseID=' + strCaseID;
            $('#tabs ul.ui-tabs-nav li a.ui-tabs-anchor').attr('href', function (index, value) {
                var n = value.charAt(0);
                if (n != "#") {
                    return value + querystringTabs;
                } else {
                    return value;
                }
            });
            ;
            var siteCollectionUrl = '<asp:Literal runat="server" Text="<%$SPUrl:~SiteCollection/%>"></asp:Literal>';
            var strUniqueID = '';
            // Get UniqueCaseID for this case from Cases list 
            $().SPServices({
                operation: "GetListItems",
                async: true,
                listName: "Cases",
                webURL: siteCollectionUrl,
                CAMLViewFields: "<ViewFields><FieldRef Name='UniqueCaseID' /></ViewFields>",
                CAMLQuery: "<Query><Where><Eq><FieldRef Name='ID' /><Value Type='Integer'>" + strCaseID + "</Value></Eq></Where></Query>",
                completefunc: function (xData, Status) {
                    $(xData.responseXML).SPFilterNode("z:row").each(function () {
                        strUniqueID = $(this).attr("ows_UniqueCaseID");
                        // Set Case ID Label text
                        if ($('#ctl00_PlaceHolderMain_lblCaseID').length) {
                            $('#ctl00_PlaceHolderMain_lblCaseID').text(strUniqueID);
                        }
                    });
                }
            });
        };
        // Function to remove note from displaying in right column
        function RemoveNote(itemID, listName) {
            try {
                var cnf = confirm("Are you sure you want to remove this note?");
                if (cnf) {
                    // Use SPServices to update the list item
                    $().SPServices({
                        operation: "UpdateListItems",
                        async: false,
                        batchCmd: "Update",
                        listName: listName,
                        ID: itemID,
                        valuepairs: [["Visible", 0]],
                        completefunc: function (xData, Status) {
                            // Check the error codes for the web service call.
                            $(xData.responseXML).SPFilterNode('ErrorCode').each(function () {
                                responseError = $(this).text();
                                if (responseError === '0x00000000') {
                                    window.location = window.location;
                                }
                                else {
                                    alert("There was an error trying to update the note.");
                                }
                            });
                        }
                    });
                }
            } catch (ex) { alert(ex); }
        };
    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div id="ecase-main-content">
        <div id="content-top-left" class="ecase-main-content-top-left"></div>
        <div id="content-top-right" class="ecase-main-content-top-right"></div>
        <br class="clearfloat" />
        <div id="content-dashboard-top" class="ecase-main-content-dashboard-top">
            <div id="content-center-left" class="ecase-main-content-center-left">
                <h2 class="ecase-site-title"><i class="icon-briefcase"></i>&nbsp;&nbsp;&nbsp;<asp:Label ID="lblCaseID" runat="server"><img src="/_layouts/images/gears_anv4.gif" border="0" align="absmiddle" style="vertical-align: middle;" /></asp:Label></h2>
                <br class="clearfloat" />
                <div id="tabs-wrapper">
                    <div id="tabs" class="ui-tabs ui-widget ui-widget-content ui-corner-all">
                        <ul class="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all">
                            <li class="ui-state-default ui-corner-top"><a href="default.aspx" class="ui-tabs-anchor">Details</a></li>
                            <li class="ui-state-default ui-corner-top"><a href="CaseInfo.aspx" class="ui-tabs-anchor">Case Info</a></li>
                            <%--<li class="ui-state-default ui-corner-top"><a href="CaseData.aspx" class="ui-tabs-anchor">Case Data</a></li>--%>
                            <li class="ui-state-default ui-corner-top ui-tabs-active ui-state-active"><a href="#tabs-4" class="ui-tabs-anchor">Documents</a></li>
                            <li class="ui-state-default ui-corner-top"><a href="Discussion.aspx" class="ui-tabs-anchor">Discussion</a></li>
                        </ul>
                        <div id="tabs-4" class="ui-tabs-panel ui-widget-content ui-corner-bottom">
                            <WebPartPages:WebPartZone ID="ID4" runat="server" Title="Zone Tab 4">
                                <ZoneTemplate></ZoneTemplate>
                            </WebPartPages:WebPartZone>
                            <div id="subtabs">
                                <ul>
                                    <li><a href="#subtabs-1">Referral Documents</a></li>
                                    <li><a href="#subtabs-2">Investigation Documents</a></li>
                                    <li><a href="#subtabs-3">SDO Documents</a></li>
                                    <li><a href="#subtabs-4">Sharing with External Party</a></li>
                                </ul>
                                <div id="subtabs-1">
                                    <WebPartPages:WebPartZone ID="SUBID1" runat="server" Title="Zone Sub Tab 1">
                                        <ZoneTemplate></ZoneTemplate>
                                    </WebPartPages:WebPartZone>
                                </div>
                                <div id="subtabs-2">
                                    <WebPartPages:WebPartZone ID="SUBID2" runat="server" Title="Zone Sub Tab 2">
                                        <ZoneTemplate></ZoneTemplate>
                                    </WebPartPages:WebPartZone>
                                </div>
                                <div id="subtabs-3">
                                    <WebPartPages:WebPartZone ID="SUBID3" runat="server" Title="Zone Sub Tab 3">
                                        <ZoneTemplate></ZoneTemplate>
                                    </WebPartPages:WebPartZone>
                                </div>
                                <div id="subtabs-4">
                                    <WebPartPages:WebPartZone ID="SUBID4" runat="server" Title="Zone Sub Tab 4">
                                        <ZoneTemplate></ZoneTemplate>
                                    </WebPartPages:WebPartZone>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="content-center-right" class="ecase-main-content-center-right ecase-note-column">
                <div class="ecases-home-link"><asp:HyperLink ID="lnkHome" CssClass="ecase-long-link" Text="<i class='icon-home'></i> Return Home" runat="server" NavigateUrl="../default.aspx" /></div>
                <WebPartPages:WebPartZone ID="ZoneRight" runat="server" Title="Zone Right">
                    <ZoneTemplate></ZoneTemplate>
                </WebPartPages:WebPartZone>
            </div>
            <br class="clearfloat" />
        </div>
        <div id="content-footer" class="ecase-main-content-footer"></div>
        <br class="clearfloat" />
    </div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    eCase Sub Site
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    eCase Sub Site
</asp:Content>
