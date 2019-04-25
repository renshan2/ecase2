<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="xsl msxsl ddwrt" xmlns:ddwrt="http://schemas.microsoft.com/WebParts/v2/DataView/runtime" xmlns:asp="http://schemas.microsoft.com/ASPNET/20" xmlns:__designer="http://schemas.microsoft.com/WebParts/v2/DataView/designer" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:SharePoint="Microsoft.SharePoint.WebControls" xmlns:ddwrt2="urn:frontpage:internal">
  <xsl:output method="html" indent="no"/>
  <xsl:decimal-format NaN=""/>
  <xsl:param name="dvt_apos">'</xsl:param>
  <xsl:param name="ManualRefresh"></xsl:param>
  <xsl:param name="FileName" />
  <xsl:param name="dvt_groupfield" />
  <xsl:param name="IdPrefix" />
  <xsl:variable name="dvt_1_automode">0</xsl:variable>

  <xsl:template match="/" xmlns:asp="http://schemas.microsoft.com/ASPNET/20" xmlns:__designer="http://schemas.microsoft.com/WebParts/v2/DataView/designer" xmlns:SharePoint="Microsoft.SharePoint.WebControls">
    <xsl:choose>
      <xsl:when test="($ManualRefresh = 'True')">
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
          <tr>
            <td valign="top">
              <xsl:call-template name="dvt_1"/>
            </td>
            <td width="1%" class="ms-vb" valign="top">
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
  <xsl:template name="dvt_1">
    <xsl:variable name="dvt_StyleName">RepForm3</xsl:variable>
    <xsl:variable name="Rows" select="/All_Results/Result" />
    <xsl:variable name="dvt_RowCount" select="count($Rows)" />
    <xsl:variable name="dvt_IsEmpty" select="$dvt_RowCount = 0" />
    <xsl:choose>
      <xsl:when test="$dvt_IsEmpty">
        <xsl:call-template name="dvt_1.empty" />
      </xsl:when>
      <xsl:otherwise>
        <table border="0" width="100%">
          <xsl:call-template name="dvt_1.body">
            <xsl:with-param name="Rows" select="$Rows" />
            <xsl:with-param name="FirstRow" select="1" />
            <xsl:with-param name="LastRow" select="$dvt_RowCount" />
          </xsl:call-template>
        </table>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="dvt_1.body">
    <xsl:param name="Rows" />
    <xsl:param name="FirstRow" />
    <xsl:param name="LastRow" />
    <xsl:variable name="dvt_LastRowValue">
      <xsl:for-each select="$Rows">
        <!--<xsl:sort select="not(assignedto)" order="ascending" />-->
        <xsl:sort select="assignedto=''" order="ascending" />
        <xsl:sort select="assignedto" order="ascending" />
        <!--<xsl:sort select="concat( substring('1', 1, boolean(assignedto[text()]) ), substring('0', 1, not(boolean(assignedto[text()]))) )" order="ascending" />-->        
        <xsl:if test="position()=$LastRow">
          <xsl:value-of select="assignedto" />
        </xsl:if>
      </xsl:for-each>
    </xsl:variable>
    <xsl:variable name="dvt_Rows">
      <root>
        <xsl:for-each select="$Rows">
          <!--<xsl:sort select="not(assignedto)" order="ascending" />-->
          <xsl:sort select="assignedto=''" order="ascending" />
          <xsl:sort select="assignedto" order="ascending" />
          <!--<xsl:sort select="concat( substring('1', 1, boolean(assignedto[text()]) ), substring('0', 1, not(boolean(assignedto[text()]))) )" order="ascending" />-->
          <xsl:if test="(position() &gt;= $FirstRow and position() &lt;= $LastRow) or (position() &gt; $LastRow and assignedto = $dvt_LastRowValue)">
            <xsl:copy-of select="." />
          </xsl:if>
        </xsl:for-each>
      </root>
    </xsl:variable>
    <xsl:for-each select="$Rows">
      <!--<xsl:sort select="not(assignedto)" order="ascending" />-->
      <xsl:sort select="assignedto=''" order="ascending" />
      <xsl:sort select="assignedto" order="ascending" />
      <!--<xsl:sort select="concat( substring('1', 1, boolean(assignedto[text()]) ), substring('0', 1, not(boolean(assignedto[text()]))) )" order="ascending" />-->
      <xsl:variable name="NewGroup_0">
        <xsl:choose>
          <xsl:when test="not ($dvt_groupfield)">
            <xsl:value-of select="ddwrt:NameChanged(string(assignedto), 0)" />
          </xsl:when>
          <xsl:otherwise></xsl:otherwise>
        </xsl:choose>
      </xsl:variable>
      <xsl:choose>
        <xsl:when test="0" />
        <xsl:when test="not($dvt_groupfield) and (not($NewGroup_0='') and position() &gt;= $FirstRow and position() &lt;= $LastRow or ($FirstRow = position()))">
          <xsl:variable name="groupheader0">
            <xsl:choose>
              <xsl:when test="not (assignedto) and (assignedto) != false()">
                <xsl:value-of select="' '" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="assignedto" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:variable>
          <xsl:if test="not ((position()=1) or (position()=$FirstRow))"></xsl:if>
          <xsl:call-template name="dvt_1.groupheader0">
            <xsl:with-param name="fieldtitle">assignedto</xsl:with-param>
            <xsl:with-param name="fieldname">assignedto</xsl:with-param>
            <xsl:with-param name="fieldvalue" select="$groupheader0" />
            <xsl:with-param name="fieldtype" select="'text'" />
            <xsl:with-param name="nodeset" select="msxsl:node-set($dvt_Rows)/root//Result[((assignedto)=$groupheader0 or ((not(assignedto) or assignedto='') and $groupheader0=' '))]" />
            <xsl:with-param name="groupid" select="'0'" />
            <xsl:with-param name="displaystyle" select="'auto'" />
            <xsl:with-param name="imagesrc" select="'/_layouts/images/minus.gif'" />
            <xsl:with-param name="alttext" select="'collapse'" />
            <xsl:with-param name="altname" select="'expand'" />
            <xsl:with-param name="hidedetail" select="false()" />
            <xsl:with-param name="showheader" select="true()" />
            <xsl:with-param name="showheadercolumn" select="false()" />
          </xsl:call-template>
        </xsl:when>
      </xsl:choose>
      <xsl:variable name="BreakOut">
        <xsl:choose>
          <xsl:when test="not($dvt_groupfield) and position() &gt; $LastRow and not($NewGroup_0='' or $NewGroup_0=' ')">
            <xsl:value-of select="ddwrt:NameChanged('', -1)" />
          </xsl:when>
          <xsl:otherwise>BreakOut</xsl:otherwise>
        </xsl:choose>
      </xsl:variable>
      <xsl:variable name="dvt_KeepItemsTogether" select="$NewGroup_0='' and not($dvt_groupfield) and position() &gt;= $FirstRow" />
      <xsl:variable name="dvt_HideGroupDetail" select="false()" />
      <xsl:if test="(position() &gt;= $FirstRow and position() &lt;= $LastRow) or $dvt_KeepItemsTogether">
        <xsl:if test="not($dvt_HideGroupDetail)" ddwrt:cf_ignore="1">
          <xsl:call-template name="dvt_1.rowview" />
        </xsl:if>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="0" />
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>
  <xsl:template name="dvt_1.rowview">
    <xsl:variable name="id" select="id"/>
    <xsl:variable name="currentId" select="concat($IdPrefix,$id)"/>
    <xsl:variable name="url" select="url"/>
    <tr>
      <td>
        <div style="clear: both;">
          <div style="float: left;">
            <input type="hidden" id="hdnIsDoc{generate-id()}" value="{isdocument}" />
            <input type="checkbox" id="chkSelect{generate-id()}" value="{url}" />
          </div>
          <div class="srch-Icon" id="{concat($currentId,'_Icon')}">
            <img align="absmiddle" src="{imageurl}" border="0" alt="{imageurl/@imageurldescription}" />
          </div>
          <div class="srch-Title2">
            <div class="srch-Title3">
              <a href="{$url}" id="{concat($currentId,'_Title')}" title="{title}" target="_blank">
                <xsl:choose>
                  <xsl:when test="hithighlightedproperties/HHTitle[. != '']">
                    <xsl:call-template name="HitHighlighting">
                      <xsl:with-param name="hh" select="hithighlightedproperties/HHTitle" />
                    </xsl:call-template>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="title"/>
                  </xsl:otherwise>
                </xsl:choose>
              </a>
            </div>
          </div>
          <div id="{concat('DOC_DETAIL_',$currentId)}" >
            <div class="srch-Description2">
              <xsl:choose>
                <xsl:when test="hithighlightedsummary[. != '']">
                  <xsl:call-template name="HitHighlighting">
                    <xsl:with-param name="hh" select="hithighlightedsummary" />
                  </xsl:call-template>
                </xsl:when>
                <xsl:when test="description[. != '']">
                  <xsl:value-of select="description"/>
                </xsl:when>
                <xsl:otherwise>
                  <img alt="" src="/_layouts/images/blank.gif" height="0" width="0"/>
                </xsl:otherwise>
              </xsl:choose>
            </div>
            <div class="srch-Metadata2">
              <xsl:call-template name="DisplayAuthors">
                <xsl:with-param name="author" select="author" />
              </xsl:call-template>
              <xsl:call-template name="DisplayDate">
                <xsl:with-param name="write" select="write" />
              </xsl:call-template>
              <xsl:call-template name="DisplaySize">
                <xsl:with-param name="size" select="size" />
              </xsl:call-template>
              <img style="display:none;" alt="" src="/_layouts/images/blank.gif"/>
            </div>
            <div style="margin-bottom:20px;" class="srch-Metadata2">
              <span class="srch-URL2" id="{concat($currentId,'_Url')}">
                <xsl:choose>
                  <xsl:when test="hithighlightedproperties/HHUrl[. != '']">
                    <xsl:call-template name="HitHighlighting">
                      <xsl:with-param name="hh" select="hithighlightedproperties/HHUrl" />
                    </xsl:call-template>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="url"/>
                  </xsl:otherwise>
                </xsl:choose>
              </span>
            </div>
          </div>
        </div>
      </td>
    </tr>
  </xsl:template>

  <xsl:template name="DisplayAuthors">
    <xsl:param name="author" />
    <xsl:if test="string-length($author) &gt; 0">
      <xsl:value-of select="'Authors: '" />
      <xsl:value-of select="author"/>
    </xsl:if>
  </xsl:template>

  <xsl:template name="DisplayDate">
    <xsl:param name="write" />
    <xsl:if test="string-length($write) &gt; 0">
      <xsl:if test="string-length(author) &gt; 0">
        <xsl:text disable-output-escaping="yes">&#8195;</xsl:text>
      </xsl:if>
      <xsl:value-of select="'Date: '" />
      <xsl:value-of select="$write"/>
    </xsl:if>
  </xsl:template>

  <xsl:template name="DisplaySize">
    <xsl:param name="size" />
    <xsl:if test="string-length($size) &gt; 0">
      <xsl:if test="number($size) &gt; 0">
        <xsl:if test="string-length(write) &gt; 0 or string-length(author) &gt; 0">
          <xsl:text disable-output-escaping="yes">&#8195;</xsl:text>
        </xsl:if>
        <xsl:value-of select="'Size: '" />
        <xsl:choose>
          <xsl:when test="round($size div 1024) &lt; 1">
            <xsl:value-of select="$size" /> Bytes
          </xsl:when>
          <xsl:when test="round($size div (1024 *1024)) &lt; 1">
            <xsl:value-of select="round($size div 1024)" />KB
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="round($size div (1024 * 1024))"/>MB
          </xsl:otherwise>
        </xsl:choose>
      </xsl:if>
    </xsl:if>
  </xsl:template>

  <xsl:template name="HitHighlighting">
    <xsl:param name="hh" />
    <xsl:apply-templates select="$hh"/>
  </xsl:template>

  <xsl:template match="ddd">
    &#8230;
  </xsl:template>
  <xsl:template match="c0">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c1">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c2">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c3">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c4">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c5">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c6">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c7">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c8">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c9">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>

  <xsl:template name="dvt_1.groupheader0">
    <xsl:param name="fieldtitle" />
    <xsl:param name="fieldname" />
    <xsl:param name="fieldvalue" />
    <xsl:param name="fieldtype" />
    <xsl:param name="nodeset" />
    <xsl:param name="groupid" />
    <xsl:param name="displaystyle" />
    <xsl:param name="imagesrc" />
    <xsl:param name="alttext" />
    <xsl:param name="altname" />
    <xsl:param name="hidedetail" />
    <xsl:param name="showheader" />
    <xsl:param name="showheadercolumn" />
    <tr id="group{$groupid}" style="display:{$displaystyle}">
      <td class="ms-gb" colspan="99">
        <xsl:choose>
          <xsl:when test="$groupid='0' or $groupid='9'">
            <xsl:text></xsl:text>
          </xsl:when>
          <xsl:when test="$groupid='1'">
            <xsl:text disable-output-escaping="yes" ddwrt:nbsp-preserve="yes">&amp;nbsp;</xsl:text>
          </xsl:when>
          <xsl:when test="$groupid='2'">
            <xsl:text disable-output-escaping="yes" ddwrt:nbsp-preserve="yes">&amp;nbsp;</xsl:text>
            <xsl:text disable-output-escaping="yes" ddwrt:nbsp-preserve="yes">&amp;nbsp;</xsl:text>
          </xsl:when>
          <xsl:otherwise>
            <xsl:text disable-output-escaping="yes" ddwrt:nbsp-preserve="yes">&amp;nbsp;</xsl:text>
            <xsl:text disable-output-escaping="yes" ddwrt:nbsp-preserve="yes">&amp;nbsp;</xsl:text>
            <xsl:text disable-output-escaping="yes" ddwrt:nbsp-preserve="yes">&amp;nbsp;</xsl:text>
          </xsl:otherwise>
        </xsl:choose>
        <xsl:if test="not($hidedetail)" ddwrt:cf_ignore="1">
          <a href="javascript:" onclick="javascript:ExpGroupBy(this);return false;">
            <img src="{$imagesrc}" border="0" alt="{$alttext}" name="{$altname}" />
          </a>
        </xsl:if>
        <xsl:text disable-output-escaping="yes" ddwrt:nbsp-preserve="yes" xmlns:ddwrt="http://schemas.microsoft.com/WebParts/v2/DataView/runtime">&amp;nbsp;</xsl:text>
        <strong>
          <!--<xsl:value-of select="$fieldtitle" />-->
          <xsl:value-of select="'Assigned To'" />
        </strong>
        <xsl:if test="$fieldtitle">: </xsl:if>
        <xsl:choose>
          <xsl:when test="$fieldtype='url'">
            <a href="{$fieldvalue}">
              <xsl:value-of select="$fieldvalue" />
            </a>
          </xsl:when>
          <xsl:when test="$fieldtype='user'">
            <xsl:value-of select="$fieldvalue" disable-output-escaping="yes" />
          </xsl:when>
          <xsl:when test="$fieldvalue=' '">
						<xsl:value-of select="'Undefined'" />
					</xsl:when>
          <xsl:when test="$fieldvalue=''">
						<xsl:value-of select="'Undefined'" />
					</xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="$fieldvalue" />
          </xsl:otherwise>
        </xsl:choose>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
