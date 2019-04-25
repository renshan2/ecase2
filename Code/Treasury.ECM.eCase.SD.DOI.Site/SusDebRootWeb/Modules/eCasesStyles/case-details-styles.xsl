<xsl:stylesheet xmlns:x="http://www.w3.org/2001/XMLSchema" xmlns:d="http://schemas.microsoft.com/sharepoint/dsp" version="1.0" exclude-result-prefixes="xsl msxsl ddwrt" xmlns:ddwrt="http://schemas.microsoft.com/WebParts/v2/DataView/runtime" xmlns:asp="http://schemas.microsoft.com/ASPNET/20" xmlns:__designer="http://schemas.microsoft.com/WebParts/v2/DataView/designer" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:SharePoint="Microsoft.SharePoint.WebControls" xmlns:ddwrt2="urn:frontpage:internal">
  <xsl:output method="html" indent="no"/>
  <xsl:decimal-format NaN=""/>
  <xsl:param name="dvt_apos">'</xsl:param>
  <xsl:param name="ManualRefresh"></xsl:param>
  <xsl:variable name="dvt_1_automode">0</xsl:variable>
  <xsl:template match="/">
    <xsl:choose>
      <xsl:when test="($ManualRefresh = 'True')">
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
          <tr>
            <td valign="top">
              <xsl:call-template name="dvt_1"/>
            </td>
            <td class="ms-vb" valign="top">
              <img src="/_layouts/images/staticrefresh.gif" id="ManualRefresh" border="0" onclick="javascript: {ddwrt:GenFireServerEvent('__cancel')}" alt="Click here to refresh the dataview."/>
            </td>
          </tr>
        </table>
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="dvt_1"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="dvt_1">
    <xsl:variable name="dvt_StyleName">RepForm3</xsl:variable>
    <xsl:variable name="Rows" select="/dsQueryResponse/Rows/Row"/>
    <xsl:variable name="RowLimit" select="1" />
    <xsl:variable name="dvt_RowCount" select="count($Rows)"/>
    <xsl:variable name="IsEmpty" select="$dvt_RowCount = 0" />
    <xsl:variable name="dvt_IsEmpty" select="$dvt_RowCount = 0"/>
    <xsl:choose>
      <xsl:when test="$dvt_IsEmpty">
        <xsl:call-template name="dvt_1.empty"/>
      </xsl:when>
      <xsl:otherwise>
        <table border="0" cellspacing="0" cellpadding="0" width="100%">
          <tr class="ms-WPHeader">
            <td align="left" class="ms-WPHeaderTd">
              <h3 class="ms-standardheader ms-WPTitle">
                <a>
                  <nobr>
                    <span>Case Details</span>
                  </nobr>
                </a>
              </h3>
            </td>
          </tr>
          <xsl:call-template name="dvt_1.body">
            <xsl:with-param name="Rows" select="$Rows"/>
            <xsl:with-param name="FirstRow" select="1" />
            <xsl:with-param name="LastRow" select="$RowLimit" />
          </xsl:call-template>
        </table>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="dvt_1.body">
    <xsl:param name="Rows"/>
    <xsl:param name="FirstRow" />
    <xsl:param name="LastRow" />
    <xsl:for-each select="$Rows">
      <xsl:variable name="dvt_KeepItemsTogether" select="false()" />
      <xsl:variable name="dvt_HideGroupDetail" select="false()" />
      <xsl:if test="(position() &gt;= $FirstRow and position() &lt;= $LastRow) or $dvt_KeepItemsTogether">
        <xsl:if test="not($dvt_HideGroupDetail)" ddwrt:cf_ignore="1">
          <xsl:call-template name="dvt_1.rowview" />
        </xsl:if>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>
  <xsl:template name="dvt_1.rowview">
    <xsl:param name="thisNode" select="."/>
    <tr>
      <td>
        <table border="0" cellspacing="0" cellpadding="1" class="ms-listviewtable" width="100%">
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Case ID:</b>
            </td>
            <td class="ms-vb2">
              <span id="spanUniqueCaseID">
                <xsl:value-of select="@UniqueCaseID" disable-output-escaping="yes"/>
              </span>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Respondent Name:</b>
            </td>
            <td class="ms-vb2">
              <xsl:value-of select="@RespondentFirstName" disable-output-escaping="yes"/>
              <xsl:text> </xsl:text>
              <xsl:value-of select="@RespondentMiddleName" disable-output-escaping="yes"/>
              <xsl:text> </xsl:text>
              <xsl:value-of select="@RespondentLastName" disable-output-escaping="yes"/>
            </td>
          </tr>
          <!--<tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Respondent Middle Name:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="respondentmiddlename">
                <xsl:value-of select="@RespondentMiddleName" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$respondentmiddlename" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Respondent Last Name:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="respondentlastname">
                <xsl:value-of select="@RespondentLastName" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$respondentlastname" />
              </xsl:call-template>
            </td>
          </tr>-->
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Tax ID:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="taxid">
                <xsl:value-of select="@TaxID" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$taxid" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Referral Date:</b>
            </td>
            <td class="ms-vb2">
              <xsl:value-of select="ddwrt:FormatDate(string(@ReferralDate),1033,1)" />
              <!--<xsl:value-of select="ddwrt:FormatDate(string(@TaskDueDate),1033,1)" />-->
              <!--<xsl:value-of select="@NextDueDateUrl" disable-output-escaping="yes" />-->
              <!--<xsl:if test="@NextDueDateUrl != ''">
                <xsl:variable name="duedateurl">
                  <xsl:value-of select="substring-before(@NextDueDateUrl,',')"/>
                </xsl:variable>
                <a href="{$duedateurl}" onclick="ShowPopupDialog('{$duedateurl}');return false;" target="_self">
                  <xsl:value-of select="substring-after(@NextDueDateUrl,',')"/>
                </a>
              </xsl:if>-->
            </td>
          </tr>
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Assigned Bureau IG Investigator:</b>
            </td>
            <td class="ms-vb2">
              <xsl:value-of select="@AssignedTo" disable-output-escaping="yes"/>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Bureau IG:</b>
            </td>
            <td class="ms-vb2">
              <xsl:value-of select="@BureauIG" disable-output-escaping="yes"/>
            </td>
          </tr>
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Case Step:</b>
            </td>
            <td class="ms-vb2">
              <xsl:value-of select="@CaseStep" disable-output-escaping="yes"/>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Case Status:</b>
            </td>
            <td class="ms-vb2">
              <xsl:value-of select="@CaseStatusLookup" disable-output-escaping="yes"/>
            </td>
          </tr>
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Case Opening Date:</b>
            </td>
            <td class="ms-vb2">
              <!--<xsl:value-of select="ddwrt:FormatDate(@DateofPrimaryInitiatingDocument, 2057, 3)" disable-output-escaping="yes"/>-->
              <xsl:value-of select="ddwrt:FormatDate(string(@CaseOpeningDate),1033,1)" />
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Primary Initiated Document Attached:</b>
            </td>
            <td class="ms-vb2">
              <!--<xsl:variable name="primaryinitiatingdocumentattached">
                <xsl:value-of select="@PrimaryInitiatingDocumentAttached" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$primaryinitiatingdocumentattached" />
              </xsl:call-template>-->
              <xsl:choose>
                <xsl:when test="@PrimaryInitiatingDocumentAttached='1'">Yes</xsl:when>
                <xsl:otherwise>No</xsl:otherwise>
              </xsl:choose>
            </td>
          </tr>
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Keyword(s):</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="taxkeyword">
                <xsl:value-of select="@TaxKeyword" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$taxkeyword" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Keywords:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="keywords">
                <xsl:value-of select="@Keywords" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$keywords" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Case Description:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="description">
                <xsl:value-of select="@Description" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$description" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Tax Debt:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="taxdebt">
                <xsl:value-of select="@TaxDebt" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$taxdebt" />
              </xsl:call-template>
              <!--<xsl:choose>
                <xsl:when test="@TaxDebt='1'">Yes</xsl:when>
                <xsl:otherwise>No</xsl:otherwise>
              </xsl:choose>-->
            </td>
          </tr>
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Intake Source:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="intakesource">
                <xsl:value-of select="@IntakeSource" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$intakesource" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Law Issue List:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="lawissuelist">
                <xsl:value-of select="@LawIssueList" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$lawissuelist" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Law Issue:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="lawissue">
                <xsl:value-of select="@LawIssue" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$lawissue" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Respondent Email:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="respondentemail">
                <xsl:value-of select="@RespondentEmail" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$respondentemail" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Respondent Phone:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="respondentphone">
                <xsl:value-of select="@RespondentPhone" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$respondentphone" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Respondent Address:</b>
            </td>
            <td class="ms-vb2">
              <xsl:value-of select="@RespondentAddress1" disable-output-escaping="yes"/>
              <xsl:choose>
                <xsl:when test="@RespondentAddress2 != ''">
                  <xsl:text>, </xsl:text>
                </xsl:when>
                <xsl:otherwise></xsl:otherwise>
              </xsl:choose>
              <xsl:value-of select="@RespondentAddress2" disable-output-escaping="yes"/>
            </td>
          </tr>
          <!--<tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Respondent Address 2:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="respondentaddress2">
                <xsl:value-of select="@RespondentAddress2" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$respondentaddress2" />
              </xsl:call-template>
            </td>
          </tr>-->
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Respondent City, State, Zip Code:</b>
            </td>
            <td class="ms-vb2">
              <xsl:value-of select="@RespondentCity" disable-output-escaping="yes"/>
              <xsl:choose>
                <xsl:when test="@RespondentState != ''">
                  <xsl:text>, </xsl:text>
                </xsl:when>
                <xsl:otherwise></xsl:otherwise>
              </xsl:choose>
              <xsl:value-of select="@RespondentState" disable-output-escaping="yes"/>
              <xsl:choose>
                <xsl:when test="@RespondentZIPCode != ''">
                  <xsl:text>, </xsl:text>
                </xsl:when>
                <xsl:otherwise></xsl:otherwise>
              </xsl:choose>
              <xsl:value-of select="@RespondentZIPCode" disable-output-escaping="yes"/>
            </td>
          </tr>
          <!--<tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Respondent State:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="respondentstate">
                <xsl:value-of select="@RespondentState" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$respondentstate" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Respondent ZIP Code:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="respondentzipcode">
                <xsl:value-of select="@RespondentZIPCode" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$respondentzipcode" />
              </xsl:call-template>
            </td>
          </tr>-->
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Company Name:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="companyname">
                <xsl:value-of select="@CompanyName" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$companyname" />
              </xsl:call-template>
            </td>
          </tr>                   
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Company Address:</b>
            </td>
            <td class="ms-vb2">
              <xsl:value-of select="@CompanyAddress1" disable-output-escaping="yes"/>
              <xsl:choose>
                <xsl:when test="@CompanyAddress2 != ''"><xsl:text>, </xsl:text></xsl:when>
                <xsl:otherwise></xsl:otherwise>
              </xsl:choose>
              <xsl:value-of select="@CompanyAddress2" disable-output-escaping="yes"/>
            </td>
          </tr>
          <!--<tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Company Address 2:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="companyaddress2">
                <xsl:value-of select="@CompanyAddress2" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$companyaddress2" />
              </xsl:call-template>
            </td>
          </tr>-->
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Company City, State, Zip Code:</b>
            </td>
            <td class="ms-vb2">
              <xsl:value-of select="@CompanyCity" disable-output-escaping="yes"/>
              <xsl:choose>
                <xsl:when test="@CompanyState != ''">
                  <xsl:text>, </xsl:text>
                </xsl:when>
                <xsl:otherwise></xsl:otherwise>
              </xsl:choose>
              <xsl:value-of select="@CompanyState" disable-output-escaping="yes"/>
              <xsl:choose>
                <xsl:when test="@CompanyZIPCode != ''">
                  <xsl:text>, </xsl:text>
                </xsl:when>
                <xsl:otherwise></xsl:otherwise>
              </xsl:choose>
              <xsl:value-of select="@CompanyZIPCode" disable-output-escaping="yes"/>
            </td>
          </tr>
          <!--<tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Company State:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="companystate">
                <xsl:value-of select="@CompanyState" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$companystate" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Company ZIP Code:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="companyzipcode">
                <xsl:value-of select="@CompanyZIPCode" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$companyzipcode" />
              </xsl:call-template>
            </td>
          </tr>-->
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Company Phone:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="companyphone">
                <xsl:value-of select="@CompanyPhone" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$companyphone" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>Affiliates:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="affiliates">
                <xsl:value-of select="@Affiliates" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$affiliates" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>Known Associates:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="knownassociates">
                <xsl:value-of select="@KnownAssociates" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$knownassociates" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>DUNS:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="duns">
                <xsl:value-of select="@DUNS" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$duns" />
              </xsl:call-template>
            </td>
          </tr>
          <tr class="ms-itmhover">
            <td class="ms-vb2">
              <b>CAGE:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="cage">
                <xsl:value-of select="@CAGE" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$cage" />
              </xsl:call-template>
            </td>
          </tr>
          <!--<tr class="ms-alternating ms-itmhover">
            <td class="ms-vb2">
              <b>UIL:</b>
            </td>
            <td class="ms-vb2">
              <xsl:variable name="uil">
                <xsl:value-of select="@LawIssueList" disable-output-escaping="yes"/>
              </xsl:variable>
              <xsl:call-template name="desc_line_break">
                <xsl:with-param name="text" select="$uil" />
              </xsl:call-template>
            </td>
          </tr>-->                    
        </table>
        <br />
        <div style="display: none;">
        FileDirRef: <xsl:value-of select="@FileDirRef" /><br />
        PermMask: <xsl:value-of select="@PermMask" /><br />
        </div>
        <a id="lnkEditCase" class="ecase-long-link" href="../Lists/Cases/EditForm.aspx?ID={@ID}&amp;ContentTypeId=0x01004892F4C3DF0941E89EFB5B5E5615F4B7" onclick="ShowPopupDialog('../Lists/Cases/EditForm.aspx?ID={@ID}&amp;ContentTypeId=0x01004892F4C3DF0941E89EFB5B5E5615F4B7');return false;">Edit Case Details</a>
        <!--
        <xsl:variable name="mask" select="@PermMask"/>
        <xsl:variable name="bit" select="substring($mask, string-length($mask))"/>
        <xsl:variable name="hasRight">
          <xsl:choose>
            <xsl:when test="$bit = '2' or $bit = '3' or $bit = '6' or $bit = '7' or 
                        $bit = 'A' or $bit = 'a' or $bit = 'B' or $bit = 'b' or $bit = 'E' or $bit = 'e' or $bit = 'F' or $bit = 'f'">1</xsl:when>
            <xsl:otherwise>0</xsl:otherwise>
          </xsl:choose>
        </xsl:variable>
        <xsl:if test="$hasRight = '1'">          
          <xsl:variable name="strEditUrl" select="concat(@FileDirRef, '/EditForm.aspx?ID=', @ID)" />          
          <a id="lnkEditCase" class="ecase-long-link" href="{$strEditUrl}" onclick="ShowPopupDialog('{$strEditUrl}');return false;">Edit Case Details</a>
        </xsl:if> 
        -->       
        <xsl:if test="$dvt_1_automode = '1'" ddwrt:cf_ignore="1">
          <br />
          <span ddwrt:amkeyfield="ID" ddwrt:amkeyvalue="ddwrt:EscapeDelims(string(@ID))" ddwrt:ammode="view"></span>
        </xsl:if>
      </td>
    </tr>
  </xsl:template>
  <xsl:template name="desc_line_break">
    <xsl:param name="text"/>
    <xsl:choose>
      <xsl:when test="contains($text, '&#xa;')">
        <xsl:value-of select="substring-before($text, '&#xa;')" disable-output-escaping="yes"/>
        <br/>
        <xsl:call-template name="desc_line_break">
          <xsl:with-param name="text" select="substring-after($text,'&#xa;')" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$text" disable-output-escaping="yes"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="dvt_1.empty">
    <xsl:variable name="dvt_ViewEmptyText">There are no items to show in this view.</xsl:variable>
    <table border="0" width="100%">
      <tr>
        <td class="ms-vb">
          <xsl:value-of select="$dvt_ViewEmptyText"/>
        </td>
      </tr>
    </table>
  </xsl:template>
  <xsl:template name="IfHasRight">
    <xsl:param name="thisNode" select="."/>
    <xsl:variable name="mask" select="$thisNode/@PermMask"/>
    <xsl:variable name="bit" select="substring($mask, string-length($mask))"/>
    <xsl:choose>
      <xsl:when test="$bit = '2' or $bit = '3' or $bit = '6' or $bit = '7' or 
                      $bit = 'A' or $bit = 'a' or $bit = 'B' or $bit = 'b' or $bit = 'E' or $bit = 'e' or $bit = 'F' or $bit = 'f'">1</xsl:when>
      <xsl:otherwise>0</xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>