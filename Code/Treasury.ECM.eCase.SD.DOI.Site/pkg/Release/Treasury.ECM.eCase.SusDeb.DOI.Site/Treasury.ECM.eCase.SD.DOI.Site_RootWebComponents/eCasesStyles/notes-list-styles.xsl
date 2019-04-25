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
    <!--<tr>
      <td class="note-heading">Case Note</td>
    </tr>-->
    <tr>
      <td class="note-cell" style="vertical-align: top;" valign="top">        
        <!--<div class="note-title">
          <a href="Lists/CaseNotes/DispForm.aspx?ID={@ID}&amp;ContentTypeID={@ContentTypeId}" onclick="ShowPopupDialog('Lists/CaseNotes/DispForm.aspx?ID={@ID}&amp;ContentTypeID={@ContentTypeId}');return false;" target="_self">
            <xsl:value-of select='@Title' disable-output-escaping ='yes'/>
          </a>
        </div>-->
        <div class="note-truncated">
          <a href="Lists/CaseNotes/DispForm.aspx?ID={@ID}" onclick="ShowPopupDialog('Lists/CaseNotes/DispForm.aspx?ID={@ID}');return false;" target="_self">
            <xsl:value-of select='@CaseNote' disable-output-escaping ='yes'/>
          </a>
        </div>
        <div class="note-author-date">
          <xsl:text>Author: </xsl:text>
          <!--<xsl:value-of select='@Editor' disable-output-escaping ='yes'/>-->
          <xsl:value-of select="substring-after(substring-before(substring-after(@Editor, 'ID='), '&lt;'), '&gt;')" disable-output-escaping="yes" />
          <br/>
          <xsl:text>Last Edited: </xsl:text>
          <xsl:value-of select='ddwrt:FormatDate(string(@Modified),1033,1)' />          
        </div>
        <xsl:if test="$ListRight_AddListItems = '1'">
          <div class="note-deleteicon">
            <a href="Lists/CaseNotes/EditForm.aspx?ID={@ID}" onclick="javascript:RemoveNote({@ID},'Case Notes');return false;">
              <i class="icon-remove-sign" title="Remove"></i>
              Remove
            </a>
          </div>
        </xsl:if>
      </td>
    </tr>
    <tr>
      <td class="note-spacer">
        <xsl:text> </xsl:text>
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
                   target="_self">Add New Case Note</a>
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
    <!--<tr>
      <td class="note-heading">Case Note</td>
    </tr>-->
    <tr>
      <td class="note-cell">
        <xsl:value-of select="$NoAnnouncements"/>
        <xsl:if test="$ListRight_AddListItems = '1'">
          <xsl:text ddwrt:whitespace-preserve="yes" xml:space="preserve"> </xsl:text>
          <xsl:text>To add a new item, click "Add New Case Note".</xsl:text>
        </xsl:if>
        <br/>
        <img src="/_layouts/images/blank.gif" width="1" height="15" alt="" />      
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
