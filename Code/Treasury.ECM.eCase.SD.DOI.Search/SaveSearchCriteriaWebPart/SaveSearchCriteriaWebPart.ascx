<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SaveSearchCriteriaWebPart.ascx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Search.SaveSearchCriteriaWebPart.SaveSearchCriteriaWebPart" %>

<input type="hidden" runat="server" id="hdnSearchUrl" />
<input type="hidden" runat="server" id="hdnSearchKeywords" />

<a id="btnSaveQuery" class="ecase-search-link" onclick="OpenModal()"><i class="icon-save"></i> Save Search Query</a>&nbsp;<asp:DropDownList ID="ddlSavedSearches" CssClass="saved-searches-select" runat="server"></asp:DropDownList>&nbsp<asp:LinkButton ID="btnRunSearch" runat="server" CssClass="ecase-search-link" OnClick="btnRunSearch_Click"><i class="icon-play"></i> Run Search</asp:LinkButton>

<script type="text/javascript">

    function OpenModal() {
        var searchUrl = document.getElementById("<%= hdnSearchUrl.ClientID %>").value;
        var options = SP.UI.$create_DialogOptions();
        options.url = "../../_layouts/eCaseSearch/SaveSearchQuery.aspx?searchUrl=" + searchUrl;
        options.width = 800;
        options.height = 600;
        options.showMaximize = false;
        options.allowMaximize = false;
        options.dialogReturnValueCallback = Function.createDelegate(null, sitePropertiesDialogCallback);
        SP.UI.ModalDialog.showModalDialog(options);
    }

    function sitePropertiesDialogCallback(dialogResult, returnValue) {
        document.location.reload(true);
    }

</script>