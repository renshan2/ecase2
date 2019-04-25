<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet xmlns:x="http://www.w3.org/2001/XMLSchema" xmlns:d="http://schemas.microsoft.com/sharepoint/dsp" version="1.0" exclude-result-prefixes="xsl msxsl ddwrt" xmlns:ddwrt="http://schemas.microsoft.com/WebParts/v2/DataView/runtime" xmlns:asp="http://schemas.microsoft.com/ASPNET/20" xmlns:__designer="http://schemas.microsoft.com/WebParts/v2/DataView/designer" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:SharePoint="Microsoft.SharePoint.WebControls" xmlns:ddwrt2="urn:frontpage:internal" xmlns:o="urn:schemas-microsoft-com:office:office">
  <xsl:output method="html" indent="no"/>
  <xsl:include href="/_layouts/xsl/main.xsl"/> 
  <xsl:include href="/_layouts/xsl/internal.xsl"/> 
  <xsl:param name="AllRows" select="/dsQueryResponse/Rows/Row[$EntityName = '' or (position() &gt;= $FirstRow and position() &lt;= $LastRow)]"/>
  <xsl:param name="dvt_apos">&apos;</xsl:param>
  <xsl:template match="FieldRef[(@Encoded) and @Name='CaseStatusLookup']" ddwrt:dvt_mode="body" mode="Lookup_body" ddwrt:ghost="" xmlns:ddwrt2="urn:frontpage:internal">
    <xsl:param name="thisNode" select="."/>
    <xsl:value-of select="substring-before(substring-after($thisNode/@*[name()=current()/@Name],'&gt;'), '&lt;')" disable-output-escaping="yes" />
  </xsl:template>
</xsl:stylesheet>