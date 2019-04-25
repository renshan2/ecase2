<xsl:stylesheet xmlns:x="http://www.w3.org/2001/XMLSchema" xmlns:d="http://schemas.microsoft.com/sharepoint/dsp" version="1.0" exclude-result-prefixes="xsl msxsl ddwrt" xmlns:ddwrt="http://schemas.microsoft.com/WebParts/v2/DataView/runtime" xmlns:asp="http://schemas.microsoft.com/ASPNET/20" xmlns:__designer="http://schemas.microsoft.com/WebParts/v2/DataView/designer" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:SharePoint="Microsoft.SharePoint.WebControls" xmlns:ddwrt2="urn:frontpage:internal" xmlns:o="urn:schemas-microsoft-com:office:office">
  <xsl:include href="/_layouts/xsl/main.xsl"/>
  <xsl:include href="/_layouts/xsl/internal.xsl"/>
  <xsl:template match="View" mode="full">
    <xsl:param name="ShowSelectAllCheckbox" select="'False'"/>
    <xsl:variable name="ViewStyleID">
      <xsl:value-of select="ViewStyle/@ID"/>
    </xsl:variable>
    <xsl:apply-templates select="." mode="RenderView" />
    <xsl:apply-templates mode="footer" select="." />
  </xsl:template>
  <xsl:template mode="Item" match="Row">
    <tr>
      <td class="ms-vb2" style="vertical-align: middle;" valign="middle">
        <i class="icon-user usericon"></i>
        <a href="_layouts/userdisp.aspx?ID={@GroupMember.id}" onclick="ShowPopupDialog('_layouts/userdisp.aspx?ID={@GroupMember.id}');return false;" target="_self">
          <xsl:value-of select='@GroupMember.title' disable-output-escaping ='yes'/>
        </a>
        <xsl:if test="$ListRight_AddListItems = '1'">
          <div class="deleteicon">
            <a href="Lists/Investigator/EditForm.aspx?ID={@ID}&amp;ContentTypeID=0x01005886DC9B59F8491E91DADDC768E67BAF00563F97DA064D2449B72B4BCAEB5215F7" onclick="javascript:DeleteItem({@ID},'Participants');return false;">
              <i class="icon-remove-sign" title="Delete"></i>
            </a>
          </div>
          <div class="clearfloat"></div>
        </xsl:if>
      </td>
    </tr>
  </xsl:template>
  <xsl:template name="Freeform">
    <xsl:param name="AddNewText"/>
    <xsl:param name="ID"/>
    <xsl:variable name="Url">
      <xsl:value-of select="$ENCODED_FORM_NEW"/>
    </xsl:variable>
    <xsl:variable name="HeroStyle">
      <xsl:choose>
        <xsl:when test="Toolbar[@Type='Standard']">display:none</xsl:when>
        <xsl:otherwise></xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:if test="$ListRight_AddListItems = '1' and (not($InlineEdit) or $IsDocLib)">
      <table id="Hero-{$WPQ}" width="100%" cellpadding="0" cellspacing="0" border="0" style="{$HeroStyle}">
        <tr>
          <td colspan="2" class="ms-partline">
            <img src="/_layouts/images/blank.gif" width="1" height="1" alt="" />
          </td>
        </tr>
        <tr>
          <td class="ms-addnew" style="padding-bottom: 3px">
            <span style="height:10px;width:10px;position:relative;display:inline-block;overflow:hidden;" class="s4-clust">
              <img src="/_layouts/images/fgimg.png" alt="" style="left:-0px !important;top:-128px !important;position:absolute;"  />
            </span>
            <xsl:text disable-output-escaping="yes" ddwrt:nbsp-preserve="yes">&amp;nbsp;</xsl:text>
            <a class="ms-addnew" id="{$ID}"
                   href="{$Url}"
                   onclick="javascript:NewItem2(event, &quot;{$Url}&quot;);javascript:return false;"
                   target="_self">Add New Investigator</a>
          </td>
        </tr>
        <tr>
          <td>
            <img src="/_layouts/images/blank.gif" width="1" height="5" alt="" />
          </td>
        </tr>
      </table>
      <xsl:choose>
        <xsl:when test="Toolbar[@Type='Standard']">
          <script type='text/javascript'>
            if (typeof(heroButtonWebPart<xsl:value-of select="$WPQ"/>) != "undefined")
            {
            <xsl:value-of select="concat('  var eleHero = document.getElementById(&quot;Hero-', $WPQ, '&quot;);')"/>
            if (eleHero != null)
            eleHero.style.display = "";
            }
          </script>
        </xsl:when>
        <xsl:otherwise>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
  </xsl:template>
  <xsl:template name="EmptyTemplate">
    <tr>
      <td class="ms-vb" colspan="99">
        <xsl:value-of select="$NoAnnouncements"/>
        <xsl:if test="$ListRight_AddListItems = '1'">
          <xsl:text ddwrt:whitespace-preserve="yes" xml:space="preserve"> </xsl:text>
          <xsl:text>To add a new item, click "Add New Investigator".</xsl:text>
        </xsl:if>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>