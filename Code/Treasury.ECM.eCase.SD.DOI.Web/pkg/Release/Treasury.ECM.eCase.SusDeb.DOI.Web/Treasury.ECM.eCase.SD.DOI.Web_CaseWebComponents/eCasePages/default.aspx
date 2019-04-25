<%@ Assembly Name="Treasury.ECM.eCase.SusDeb.DOI.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1dad96f5b8a688f6" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="DocWebControls" Namespace="Microsoft.Office.Server.WebControls" Assembly="Microsoft.Office.DocumentManagement, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%-- CodeBehind="default.aspx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Web.CaseSiteDefinition.Modules.eCasePages.DefaultPage, --%>
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
	<noscript>
		<style type="text/css">
			/*Hide toggle button for users with javascript disabled*/
			#btnViewList {
				display: none;
			}
		</style>
	</noscript>
	<script type="text/javascript">
		if (typeof jQuery == "undefined") {
			var jQPath = "../Style%20Library/Scripts/";
			document.write("<script src='", jQPath, "jquery-1.8.2.js' type='text/javascript'><\/script>");
		}
	</script>
	<script type="text/javascript" src="../Style%20Library/Scripts/jquery.SPServices-0.7.2.min.js"></script>
	<script type="text/javascript" src="../Style%20Library/Scripts/jquery-ui.js"></script>
	<script type="text/javascript" src="../Style%20Library/Scripts/jquery.cookie.js"></script>
	<script type="text/javascript" src="../Style%20Library/Scripts/fullcalendar.min.js"></script>
	<script type="text/javascript" src="../Style%20Library/Scripts/jquery.dotdotdot.min.js"></script>
	<script type="text/javascript" src="../Style%20Library/Scripts/eCaseWebHomePageScripts.js"></script>
	<link href="../Style%20Library/jquery-ui.css" rel="stylesheet" type="text/css" />
	<link href="../Style%20Library/eCase-tab-styles.css" rel="stylesheet" type="text/css" />
	<link href="../Style%20Library/fullcalendar.css" rel="stylesheet" type="text/css" />
	<link href="../Style%20Library/fullcalendar.print.css" rel="stylesheet" type="text/css" media="print" />    
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
							<li class="ui-state-default ui-corner-top ui-tabs-active ui-state-active"><a href="#tabs-1" class="ui-tabs-anchor">Details</a></li>
							<li class="ui-state-default ui-corner-top"><a href="CaseInfo.aspx?CaseID={ListItemId}" class="ui-tabs-anchor">Case Info</a></li>
							<%--<li class="ui-state-default ui-corner-top"><a href="CaseData.aspx?CaseID={ListItemId}" class="ui-tabs-anchor">Case Data</a></li>--%>
							<li class="ui-state-default ui-corner-top"><a href="Documents.aspx?CaseID={ListItemId}" class="ui-tabs-anchor">Documents</a></li>
							<li class="ui-state-default ui-corner-top"><a href="Discussion.aspx?CaseID={ListItemId}" class="ui-tabs-anchor">Discussion</a></li>
						</ul>
						<div id="tabs-1" class="ui-tabs-panel ui-widget-content ui-corner-bottom">
							<WebPartPages:WebPartZone ID="ID1" runat="server" Title="Zone Tab 1">
								<ZoneTemplate>
									<WebPartPages:DataFormWebPart runat="server" Description="Case Details" Title="Case Details" DisplayName="Case Details" ViewFlag="8" NoDefaultStyle="TRUE" FrameType="None" ChromeType="None" MissingAssembly="Cannot import this Web Part." __MarkupType="vsattributemarkup" ID="g_661ea7d1_3101_4fde_add0_e81bee9d041d" __WebPartId="{661EA7D1-3101-4FDE-ADD0-E81BEE9D041D}" WebPart="true">
										<DataSources>
											<SharePoint:SPDataSource runat="server" DataSourceMode="ListItem" UseInternalName="True" UseServerDataFormat="True" ID="SPDataSourceCaseDetails">
												<SelectParameters>
													<WebPartPages:DataFormParameter Name="ListItemId" ParameterKey="ListItemId" PropertyName="ParameterValues" DefaultValue="{ListItemId}" />
													<WebPartPages:DataFormParameter Name="ListName" ParameterKey="ListName" PropertyName="ParameterValues" DefaultValue="Cases" />
													<WebPartPages:DataFormParameter Name="WebURL" ParameterKey="WebURL" PropertyName="ParameterValues" DefaultValue="{sitecollectionroot}" />
													<asp:Parameter Name="MaximumRows" DefaultValue="1" />
												</SelectParameters>
											</SharePoint:SPDataSource>
										</DataSources>
										<DataFields>@Title,Title;@UniqueCaseID,Unique ID;@TaskDueDate,Due Date;@NextDueDateUrl,Due Date Url;@AssignedTo,Assigned To;@BureauIG,Bureau IG;@Description,Description;@CaseStatusLookup,Case Status;@CaseStep,Case Step;@CaseUrl,Case Url;@TaxKeyword,Tags;@Judge,Judge;@LawIssueList,LIL;@Affiliates,Affiliates;@AllPriorMaterialsSubmitted,AllPriorMaterialsSubmitted;@DateAllPriorMaterialsExpected,DateAllPriorMaterialsExpected;@OpeningActionDate,OpeningActionDate;@OpeningActionCompleted,OpeningActionCompleted;@PrimaryInitiatingDocumentAttached,PrimaryInitiatingDocumentAttached;@CaseOpeningDate,CaseOpeningDate;@SourceofPrimaryInitiatingDocument,SourceofPrimaryInitiatingDocument;@OtherInformation,OtherInformation;@AllPriorMaterialsSubmitted,AllPriorMaterialsSubmitted;@ID,ID;@FileDirRef,FileDirRef;@PermMask,PermMask;</DataFields>
										<ParameterBindings>
										<ParameterBinding Name="ListItemId" Location="None" DefaultValue="{ListItemId}"/>
										<ParameterBinding Name="ListName" Location="None" DefaultValue="Cases"/>
										<ParameterBinding Name="WebURL" Location="None" DefaultValue="{sitecollectionroot}"/>
										<ParameterBinding Name="dvt_apos" Location="Postback;Connection"/>
										<ParameterBinding Name="ManualRefresh" Location="WPProperty[ManualRefresh]"/>
										<ParameterBinding Name="UserID" Location="CAMLVariable" DefaultValue="CurrentUserName"/>
										<ParameterBinding Name="Today" Location="CAMLVariable" DefaultValue="CurrentDate"/>
										</ParameterBindings>
										<XslLink>../Style%20Library/case-details-styles.xsl</XslLink>
									</WebPartPages:DataFormWebPart>                            
								</ZoneTemplate>
							</WebPartPages:WebPartZone>

								<table border="0" cellspacing="0" cellpadding="0" width="100%">
									<tr class="ms-WPHeader">
										<td align="left" class="ms-WPHeaderTd">
											<h3 class="ms-standardheader ms-WPTitle"><a>
												<nobr><span>Activities &amp; Tasks</span></nobr>
											</a></h3>
										</td>
									</tr>
								</table>
														
							<div id="calendar"><div id="loading"><img src="/_layouts/images/gears_anv4.gif" border="0" /></div></div>

							<a id="lnkViewList" href="#" class="ecase-long-link">View List</a>
							<div id="divDatesTasksList">
								<table border="0" cellspacing="0" cellpadding="0" width="100%">
									<tr class="ms-WPHeader">
										<td align="left" class="ms-WPHeaderTd">
											<h3 class="ms-standardheader ms-WPTitle"><a>
												<nobr><span>Case Related Dates and Activities &amp; Tasks</span></nobr>
											</a></h3>
										</td>
									</tr>
									<tr>
										<td valign="top">
											<SharePoint:SPDataSource ID="SPDataSourceDates" runat="server" DataSourceMode="CrossList" UseInternalName="true"
												SelectCommand="<Webs></Webs><Lists><List ID='{ActivitiesAndTasksGuid}'></List><List ID='{CaseRelatedDatesGuid}'></List></Lists><View><ViewFields><FieldRef Name='Title'/><FieldRef Name='StartDate' Nullable='TRUE'/><FieldRef Name='EventDate' Nullable='TRUE'/><FieldRef Name='fAllDayEvent' Nullable='TRUE'/><FieldRef Name='EndDate' Nullable='TRUE'/><FieldRef Name='TimeZone' Nullable='TRUE'/><FieldRef Name='XMLTZone' Nullable='TRUE'/><FieldRef Name='DueDate' Nullable='TRUE'/><FieldRef Name='ContentType' Nullable='TRUE'/><FieldRef Name='FileDirRef' Nullable='TRUE'/></ViewFields><Query></Query></View>">
											</SharePoint:SPDataSource>
											<asp:GridView ID="GridViewDates" runat="server"
												DataSourceID="SPDataSourceDates"
												CssClass="ms-listviewtable" HeaderStyle-CssClass="ms-viewheadertr" RowStyle-CssClass="ms-itmhover" AlternatingRowStyle-CssClass="ms-alternating ms-itmhover"
												AutoGenerateColumns="false" EmptyDataText="There is no data to show."
												Width="100%">
												<Columns>
													<asp:HyperLinkField HeaderText="Title" DataTextField="Title" DataNavigateUrlFields="ListId,ID" DataNavigateUrlFormatString="_layouts/listform.aspx?PageType=4&ListId={{{0}}}&ID={1}" ItemStyle-CssClass="ms-vb2" HeaderStyle-CssClass="ms-vh2" />
													<asp:BoundField HeaderText="Event Start Date" DataField="EventDate" ItemStyle-CssClass="ms-vb2" HeaderStyle-CssClass="ms-vh2" />
													<asp:BoundField HeaderText="Event End Date" DataField="EndDate" ItemStyle-CssClass="ms-vb2" HeaderStyle-CssClass="ms-vh2" />
													<asp:BoundField HeaderText="Task Start Date" DataField="StartDate" ItemStyle-CssClass="ms-vb2" HeaderStyle-CssClass="ms-vh2" />
													<asp:BoundField HeaderText="Task Due Date" DataField="DueDate" ItemStyle-CssClass="ms-vb2" HeaderStyle-CssClass="ms-vh2" />
												</Columns>
											</asp:GridView>
										</td>
									</tr>
								</table>
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
