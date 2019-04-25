<xsl:stylesheet xmlns:x="http://www.w3.org/2001/XMLSchema" xmlns:d="http://schemas.microsoft.com/sharepoint/dsp" version="1.0" exclude-result-prefixes="xsl msxsl ddwrt" xmlns:ddwrt="http://schemas.microsoft.com/WebParts/v2/DataView/runtime" xmlns:asp="http://schemas.microsoft.com/ASPNET/20" xmlns:__designer="http://schemas.microsoft.com/WebParts/v2/DataView/designer" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:SharePoint="Microsoft.SharePoint.WebControls" xmlns:ddwrt2="urn:frontpage:internal" xmlns:o="urn:schemas-microsoft-com:office:office">
  <xsl:include href="/_layouts/xsl/main.xsl"/>
  <xsl:include href="/_layouts/xsl/internal.xsl"/>
  <xsl:param name="AllRows" select="/dsQueryResponse/Rows/Row[$EntityName = '' or (position() &gt;= $FirstRow and position() &lt;= $LastRow)]"/>
  <xsl:param name="dvt_apos">&apos;</xsl:param>
  <xsl:template name="FieldHeader.TaxKeyword" ddwrt:dvt_mode="header" ddwrt:ghost="" xmlns:ddwrt2="urn:frontpage:internal">
    <xsl:param name="fieldname" />
    <xsl:param name="fieldtitle" />
    <xsl:param name="displayname" />
    <xsl:param name="fieldtype" select="'0'"/>
    <xsl:param name="thisNode" select="."/>
    <xsl:variable name="sortable">
      <xsl:choose>
        <xsl:when test="../../@BaseViewID='3' and ../../List/@TemplateType='106'">FALSE</xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="./@Sortable"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:choose>
      <xsl:when test="not($sortable='FALSE')">
        <xsl:variable name="sortfield">
          <xsl:choose>
            <xsl:when test="substring($fieldname, string-length($fieldname) - 5) = '(text)'">
              <xsl:value-of select="substring($fieldname, 1, string-length($fieldname) - 6)" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="$fieldname"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>
        <xsl:variable name="linkdir">
          <xsl:choose>
            <xsl:when test="$dvt_sortfield = $sortfield and ($dvt_sortdir = 'ascending' or $dvt_sortdir = 'ASC')">Desc</xsl:when>
            <xsl:otherwise>Asc</xsl:otherwise>
          </xsl:choose>
        </xsl:variable>
        <xsl:variable name="sortText">
          <xsl:choose>
            <xsl:when test="$linkdir='Desc'">&apos; + &apos;descending&apos; + &apos;</xsl:when>
            <xsl:otherwise>&apos; + &apos;ascending&apos; + &apos;</xsl:otherwise>
          </xsl:choose>
        </xsl:variable>
        <xsl:variable name="jsescapeddisplayname">
          <xsl:call-template name="fixQuotes">
            <xsl:with-param name="string" select="$displayname"/>
          </xsl:call-template>
        </xsl:variable>
        <xsl:variable name="separator" select="' '" />
        <xsl:variable name="connector" select="';'" />
        <a id="diidSort{$fieldname}" onfocus="OnFocusFilter(this)">
          <xsl:attribute name="href">
            javascript: <xsl:if test="$NoAJAX">
              <xsl:call-template name="GenFireServerEvent">
                <xsl:with-param name="param" select="concat('dvt_sortfield={',$sortfield,'};dvt_sortdir={',$sortText,'}')"/>
              </xsl:call-template>
            </xsl:if>
          </xsl:attribute>
          <xsl:attribute name="onclick">
            <xsl:choose>
              <xsl:when test="not($NoAJAX)">javascript:return OnClickFilter(this,event);</xsl:when>
              <xsl:otherwise>
                javascript: <xsl:call-template name="GenFireServerEvent">
                  <xsl:with-param name="param" select="concat('dvt_sortfield={',$sortfield,'};dvt_sortdir={',$sortText,'}')"/>
                </xsl:call-template>; event.cancelBubble = true; return false;
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
          <xsl:choose>
            <xsl:when test="not($NoAJAX)">
              <xsl:attribute name="SortingFields">
                <xsl:value-of select ="$RootFolderParam"/><xsl:value-of select ="$FieldSortParam"/>SortField=<xsl:value-of select="@Name"/>&amp;SortDir=<xsl:value-of select="$linkdir"/>
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="FilterString">
                <xsl:value-of select="concat($jsescapeddisplayname,$separator,$fieldname, $separator,$fieldtype, $connector, $LCID, $separator, $WebPartClientID)" />
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
          <xsl:choose>
            <xsl:when test="$fieldtype = 'Attachments'">
              <xsl:value-of select="$fieldtitle" disable-output-escaping="yes"/>
            </xsl:when>
            <xsl:otherwise>
              <!--<xsl:value-of select="$fieldtitle"/>-->
              <xsl:value-of select="'Tags'" />
            </xsl:otherwise>
          </xsl:choose>
          <xsl:if test="$dvt_sortfield = $sortfield">
            <xsl:choose>
              <xsl:when test="$dvt_sortdir = 'ascending'">
                <img border="0" alt="{$Rows/@viewedit_onetidSortAsc}" src="{ddwrt:FieldSortImageUrl('Desc')}" />
              </xsl:when>
              <xsl:when test="$dvt_sortdir = 'descending'">
                <img border="0" alt="{$Rows/@viewedit_onetidSortDesc}" src="{ddwrt:FieldSortImageUrl('Asc')}" />
              </xsl:when>
            </xsl:choose>
          </xsl:if>
          <img src="/_layouts/images/blank.gif" class="ms-hidden" border="0" width="1" height="1" alt="{$OpenMenuKeyAccessible}"/>
        </a>
        <img src="/_layouts/images/blank.gif" alt="" border="0"/>
        <xsl:choose>
          <xsl:when test="contains($dvt_filterfields, concat(';', $fieldname, ';' )) or contains($dvt_filterfields, concat(';@', $fieldname, ';' ))">
            <img src="/_layouts/images/filter.gif" border="0" alt="" />
          </xsl:when>
          <xsl:otherwise>
            <img src="/_layouts/images/blank.gif" border="0" alt=""/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:when test="not(@Filterable='FALSE') and ($sortable='FALSE')">
        <xsl:choose>
          <xsl:when test="$fieldtype = 'Attachments'">
            <xsl:value-of select="$fieldtitle" disable-output-escaping="yes"/>
          </xsl:when>
          <xsl:otherwise>             
            <!--<xsl:value-of select="$fieldtitle" />-->
            <xsl:value-of select="'Tags'" />
          </xsl:otherwise>
        </xsl:choose>
        <xsl:if test="contains($dvt_filterfields, concat(';', $fieldname, ';' )) or contains($dvt_filterfields, concat(';@', $fieldname, ';' ))">
          <img src="/_layouts/images/filter.gif" border="0" alt="" />
        </xsl:if>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="$fieldtype = 'Attachments'">
            <xsl:value-of select="$fieldtitle" disable-output-escaping="yes"/>
          </xsl:when>
          <xsl:otherwise>
            <!--<xsl:value-of select="$fieldtitle"/>-->
            <!--<xsl:call-template name="eCaseDisplayCustomTitle"><xsl:with-param name="fieldtitle" select ="$fieldtitle" /></xsl:call-template>-->
            <xsl:value-of select="'Tags'" />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:if test="($fieldtype='BusinessData') and not($XmlDefinition/List/@ExternalDataList='1')">
      <a style="padding-left:2px;padding-right:12px" onmouseover="" onclick="GoToLink(this);return false;"
        href="{$HttpVDir}/_layouts/BusinessDataSynchronizer.aspx?ListId={$List}&amp;ColumnName={$fieldname}">
        <img border="0" src="/_layouts/images/bdupdate.gif" alt="{$Rows/@resource.wss.BusinessDataField_UpdateImageAlt}" title="{$Rows/@resource.wss.BusinessDataField_UpdateImageAlt}"/>
      </a>
    </xsl:if>
  </xsl:template>
  
  <xsl:template name="eCaseDisplayCustomTitle">
    <xsl:param name="fieldtitle" />
    <xsl:choose>
      <xsl:when test="$fieldtitle = 'Enterprise Keywords'" >
        <xsl:value-of select="'Tags'" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$fieldtitle" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>
