<%@ Assembly Name="Treasury.ECM.eCase.SusDeb.DOI.Search, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4e7a656e7f3196c4" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SaveSearchResultSets.aspx.cs" Inherits="Treasury.ECM.eCase.SusDeb.DOI.Search.Layouts.eCaseSearch.SaveSearchResultSets" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <SharePoint:CssRegistration ID="CssRegistration1" Name="<% $SPUrl:~sitecollection/Style Library/eCase-styles.css?rev=1 %>" After="corev4.css" runat="server" />
    <SharePoint:CssRegistration ID="CssRegistration2" Name="<% $SPUrl:~sitecollection/Style Library/font-awesome.css %>" After="corev4.css" runat="server" />
    <style type="text/css">
        body #s4-leftpanel {
            display: none;
        }
        body {
            background-image: none !important;
        }
        .s4-ca {
            margin-left: 20px;
        }
        .search-results-headercell {
            text-align: left;
        }
        .ms-listviewtable {
            width: 95% !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:PlaceHolder ID="executionResultsPlaceholder" runat="server" />

    <div style="margin-top: 15px;">
        <h2 class="ecase-site-title"><i class="icon-list-alt"></i>&nbsp;&nbsp;&nbsp;Manage Saved Search Results</h2>
    </div>

    <%-- <div>
        <asp:LinkButton ID="returnToSearch" OnClick="returnToSearch_Click"  Text="Return to Search Result Sets" runat="server" />
    </div> --%> 
    

    <div style="left:20px;">
        <SharePoint:SPGridView ID="savedSearchResultSetGrid" runat="server" OnRowCancelingEdit="savedSearchResultSetGrid_RowCancelingEdit" OnRowCommand="savedSearchResultSetGrid_RowCommand" OnRowUpdating="savedSearchResultSetGrid_RowUpdating" OnRowDeleting="savedSearchResultSetGrid_RowDeleting" OnRowEditing="savedSearchResultSetGrid_RowEditing" GridLines="Both" AllowPaging="True" AutoGenerateEditButton="False" AutoGenerateDeleteButton="False" AutoGenerateSelectButton="False" AutoGenerateColumns="false" RowStyle-CssClass="ms-itmhover" AlternatingRowStyle-CssClass="search-results-alt-row ms-itmhover">
            <EmptyDataTemplate>
                There are no Saved Search Result Sets available.
            </EmptyDataTemplate>
            <Columns>                
                <asp:CommandField ItemStyle-CssClass="ms-vb2 search-results-cell search-results-first-cell" HeaderStyle-CssClass="search-results-headercell" ButtonType="Link" ShowEditButton="true" EditText="<i class='icon-pencil'></i> Edit " UpdateText="<i class='icon-ok'></i> Update " CancelText="<i class='icon-ban-circle'></i> Cancel " />                
                <asp:TemplateField HeaderText=" " ItemStyle-CssClass="ms-vb2 search-results-cell" HeaderStyle-CssClass="search-results-headercell">
                    <ItemTemplate>
                        <asp:LinkButton ID="deleteButton" runat="server" CommandName="Delete" Text="<i class='icon-remove'></i> Delete" OnClientClick="return confirm('Are you sure you want to delete this?');"  />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Show Search Results" ItemStyle-CssClass="ms-vb2 search-results-cell" HeaderStyle-CssClass="search-results-headercell">
                    <ItemTemplate>
                        <asp:LinkButton ID="showResultsButton" runat="server" CommandName="ShowResults" CommandArgument='<%# Eval("Id") %>' Text="Show Search Results" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Id" Visible="false">
                    <ItemTemplate>
                        <asp:Label ID="idLabel" Text='<%# Bind("Id") %>' runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Name" ItemStyle-CssClass="ms-vb2 search-results-cell" HeaderStyle-CssClass="search-results-headercell">
                    <ItemTemplate>
                        <asp:Label ID="nameLabel" Text='<%# Bind("Name") %>' runat="server" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="nameTextBox" Text='<%# Bind("Name") %>' runat="server" />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Description" ItemStyle-CssClass="ms-vb2 search-results-cell" HeaderStyle-CssClass="search-results-headercell">
                    <ItemTemplate>
                        <asp:Label ID="descriptionLabel" Text='<%# Bind("Description") %>' runat="server" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="descriptionTextBox" Text='<%# Bind("Description") %>' runat="server" />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Query" ItemStyle-CssClass="ms-vb2 search-results-cell" HeaderStyle-CssClass="search-results-headercell">
                    <ItemTemplate>
                        <asp:Label ID="queryLabel" Text='<%# Bind("OriginalQuery") %>' runat="server" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:Label ID="queryLabel" Text='<%# Bind("OriginalQuery") %>' runat="server" />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Owner" ItemStyle-CssClass="ms-vb2 search-results-cell" HeaderStyle-CssClass="search-results-headercell">
                    <ItemTemplate>
                        <SharePoint:PeopleEditor ID="ownerPeopleEditor" AllowTypeIn="false" ShowButtons="false" MultiSelect="false" SelectionSet="User" runat="server" CommaSeparatedAccounts='<%# Bind("Owner") %>'  />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <SharePoint:PeopleEditor ID="ownerPeopleEditor" MultiSelect="false" SelectionSet="User" runat="server" CommaSeparatedAccounts='<%# Bind("Owner") %>'  />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Share With" ItemStyle-CssClass="ms-vb2 search-results-cell" HeaderStyle-CssClass="search-results-headercell">
                    <ItemTemplate>
                        <SharePoint:PeopleEditor ID="shareWithPeopleEditor" AllowTypeIn="false" ShowButtons="false"  MultiSelect="true" SelectionSet="User" runat="server" CommaSeparatedAccounts='<%# Bind("ShareWith") %>'  />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <SharePoint:PeopleEditor ID="shareWithPeopleEditor" MultiSelect="true" SelectionSet="User" runat="server" CommaSeparatedAccounts='<%# Bind("ShareWith") %>'  />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Results" ItemStyle-CssClass="ms-vb2 search-results-cell" HeaderStyle-CssClass="search-results-headercell">
                    <ItemTemplate>
                        <asp:Label ID="resultsCountLabel" Text='<%# Bind("ResultsCount") %>' runat="server" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:Label ID="resultsCountLabel" Text='<%# Bind("ResultsCount") %>' runat="server" />
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>
        </SharePoint:SPGridView>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Saved Search Results
</asp:Content>
 
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Saved Search Results
</asp:Content>
