<%@ Assembly Name="Treasury.ECM.eCase.SusDeb.DOI.Site, Version=1.0.0.0, Culture=neutral, PublicKeyToken=44198732fb780fac" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>

<%@ Page Language="C#" EnableSessionState="True" CodeBehind="ManagerDashboard.aspx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Site.eCaseRootWeb.Modules.eCasesPages.ManagerDashboard, Treasury.ECM.eCase.SusDeb.DOI.Site, Version=1.0.0.0, Culture=neutral, PublicKeyToken=44198732fb780fac" MasterPageFile="Style Library/MasterPages/eCase_main.master" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PlaceHolderAdditionalPageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
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
	</style>
	<style type="text/css">
		@media print {
			svg g.highcharts-data-labels text {
			  fill: #fff !important;
			  color: #fff !important;
			}

			.highcharts-data-labels div span {
			  fill: #fff !important;
			  color: #fff !important;
			}  
		}
	</style>
	<script type="text/javascript">
		if (typeof jQuery == "undefined") {
			var jQPath = "Style%20Library/Scripts/";
			document.write("<script src='", jQPath, "jquery-1.8.2.js' type='text/javascript'><\/script>");
		}
	</script>
	<script src="Style%20Library/Scripts/highcharts.js" type="text/javascript"></script>
	<script src="Style%20Library/Scripts/dashboardcontent.js" type="text/javascript"></script>
	<script type="text/javascript">
	var allCaseData = new Array();
	<asp:Literal ID="litCaseDataArray" runat="server" />

	var pieData = new Array();
	<asp:Literal ID="litPieDataArray" runat="server" />

	var numCaseItems = allCaseData.length;
	var caseContainerHeight = (100 + (numCaseItems * 40));

	var numPieItems = pieData.length;
	var pieContainerHeight = (250 + (numPieItems * 20));

	$(document).ready(function () {
	
		if (numCaseItems > 0) {
			$('#ecase-status-chart').height(caseContainerHeight);
			buildCaseArrays();	            
		}  
		else {
			$('#ecase-status-chart').html('<p style="text-align: center;"><br/>No data found.</p>');
		}

		if (numPieItems > 0) {
			$('#ecase-pie-chart').height(pieContainerHeight);
			drawPieChart();				
		}  
		else {
			$('#ecase-pie-chart').html('<p style="text-align: center;"><br/>No data found.</p>');
		}

	});	
	</script>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
	<div id="ecase-main-content">
		<div id="content-top-left" class="ecase-main-content-top-left"></div>
		<div id="content-top-right" class="ecase-main-content-top-right"></div>
		<br class="clearfloat" />
		<div id="content-dashboard-top" class="ecase-main-content-dashboard-top">
			<div id="content-center-left" class="ecase-main-content-center-left">
				<h2 class="ecase-site-title"><i class="icon-list-alt"></i>&nbsp;&nbsp;&nbsp;S&amp;D Case Dashboard</h2>
				<div class="chart-title chart-title-left"><span>Cases At-A-Glance</span></div>
				<div id="ecase-status-chart"></div>
				<WebPartPages:WebPartZone runat="server" ID="wpzoneLeftColumn">
					<ZoneTemplate></ZoneTemplate>
				</WebPartPages:WebPartZone>
			</div>
			<div id="content-center-right" class="ecase-main-content-center-right">
				<div class="ecases-home-link"><asp:HyperLink ID="lnkHome" CssClass="ecase-long-link" Text="<i class='icon-home'></i> Return Home" runat="server" NavigateUrl="default.aspx" /></div>
				<div class="chart-title"><span>Number of Cases By Bureau</span></div>
				<div id="ecase-pie-chart"></div>
			</div>
			<br class="clearfloat" />
		</div>
		<div id="content-footer" class="ecase-main-content-footer">
		</div>
		<br class="clearfloat" />        
	</div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
	S&amp;D Case Dashboard
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
	S&amp;D Case Dashboard
</asp:Content>
