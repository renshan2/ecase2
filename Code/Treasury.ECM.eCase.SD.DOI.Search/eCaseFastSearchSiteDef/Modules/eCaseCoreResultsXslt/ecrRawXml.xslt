<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>
  <!-- XSL transformation starts here -->
  <xsl:template match="/">
    <xmp>
      <xsl:copy-of select="*"/>
    </xmp>
  </xsl:template>

  <!-- End of Stylesheet -->
</xsl:stylesheet>