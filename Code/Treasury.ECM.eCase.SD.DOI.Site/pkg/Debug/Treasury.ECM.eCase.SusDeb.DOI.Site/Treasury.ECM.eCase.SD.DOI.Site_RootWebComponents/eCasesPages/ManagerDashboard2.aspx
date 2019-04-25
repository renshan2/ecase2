<%@ Assembly Name="Treasury.ECM.eCase.SusDeb.DOI.Site, Version=1.0.0.0, Culture=neutral, PublicKeyToken=44198732fb780fac" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>

<%@ Page Language="C#" EnableSessionState="True" CodeBehind="ManagerDashboard2.aspx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Site.eCaseRootWeb.Modules.eCasesPages.ManagerDashboard2, Treasury.ECM.eCase.SusDeb.DOI.Site, Version=1.0.0.0, Culture=neutral, PublicKeyToken=44198732fb780fac" MasterPageFile="Style Library/MasterPages/eCase_main.master" %>

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
	<script type="text/javascript">
		if (typeof jQuery == "undefined") {
			var jQPath = "Style%20Library/Scripts/";
			document.write("<script src='", jQPath, "jquery-1.8.2.js' type='text/javascript'><\/script>");
		}
	</script>
	<script src="Style%20Library/Scripts/highcharts.js" type="text/javascript"></script>
	<script src="Style%20Library/Scripts/dashboardcontent2.js" type="text/javascript"></script>
	<script type="text/javascript">
		var pie1Data = new Array();
		<asp:Literal ID="litPie1DataArray" runat="server" />
		var numPie1Items = pie1Data.length;
		var pie1ContainerHeight = (250 + (numPie1Items * 20));

		var pie2Data = new Array();
		<asp:Literal ID="litPie2DataArray" runat="server" />
		var numPie2Items = pie2Data.length;
		var pie2ContainerHeight = (250 + (numPie2Items * 20));

		var pie3Data = new Array();
		<asp:Literal ID="litPie3DataArray" runat="server" />
		var numPie3Items = pie3Data.length;
		var pie3ContainerHeight = (250 + (numPie3Items * 20));

		var pie4Data = new Array();
		<asp:Literal ID="litPie4DataArray" runat="server" />
		var numPie4Items = pie4Data.length;
		var pie4ContainerHeight = (250 + (numPie4Items * 20));

		$(document).ready(function () {
			renderPieChart(numPie1Items, 'ecase-pie-chart1', pie1ContainerHeight, 'Number of Cases by Investigator', pie1Data);
			renderPieChart(numPie2Items, 'ecase-pie-chart2', pie2ContainerHeight, 'Number of Cases by Step', pie2Data);
			renderPieChart(numPie3Items, 'ecase-pie-chart3', pie3ContainerHeight, 'Number of Cases by Law Issue', pie3Data);
			renderPieChart(numPie4Items, 'ecase-pie-chart4', pie4ContainerHeight, 'Number of Cases by Bureau', pie4Data);
		});
	</script>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
	<div id="ecase-main-content">
		<div id="content-top-left" class="ecase-main-content-top-left"></div>
		<div id="content-top-right" class="ecase-main-content-top-right"></div>
		<br class="clearfloat" />
		<div id="content-dashboard-top" class="ecase-main-content-dashboard-top">
			<div id="content-center-full" class="ecase-main-content-center-full">				
				<h2 class="ecase-site-title"><i class="icon-list-alt"></i>&nbsp;&nbsp;&nbsp;S&amp;D Case Dashboard</h2>
				<div class="ecases-home-link"><asp:HyperLink ID="lnkHome" CssClass="ecase-long-link" Text="<i class='icon-home'></i> Return Home" runat="server" NavigateUrl="default.aspx" /></div>
				<div class="chart-title chart-title-full"><span>Other Charts</span></div>
				<div class="pies-container">
					<div id="ecase-pie-chart1"></div>
					<div id="ecase-pie-chart2"></div>
					<div id="ecase-pie-chart3"></div>
					<div id="ecase-pie-chart4"></div>
					<br class="clearfloat" />
				</div>				
				<WebPartPages:WebPartZone runat="server" ID="wpzoneLeftColumn">
					<ZoneTemplate></ZoneTemplate>
				</WebPartPages:WebPartZone>
			</div>
			<br class="clearfloat" />
		</div>
		<div id="content-footer" class="ecase-main-content-footer">
			<asp:Literal ID="litTest" runat="server" />
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
