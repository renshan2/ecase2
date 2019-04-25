<xsl:stylesheet xmlns:x="http://www.w3.org/2001/XMLSchema" xmlns:d="http://schemas.microsoft.com/sharepoint/dsp" version="1.0" exclude-result-prefixes="xsl msxsl ddwrt" xmlns:ddwrt="http://schemas.microsoft.com/WebParts/v2/DataView/runtime" xmlns:asp="http://schemas.microsoft.com/ASPNET/20" xmlns:__designer="http://schemas.microsoft.com/WebParts/v2/DataView/designer" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:SharePoint="Microsoft.SharePoint.WebControls" xmlns:ddwrt2="urn:frontpage:internal">
  <xsl:output method="html" indent="no"/>
  <xsl:decimal-format NaN=""/>
  <xsl:param name="dvt_apos">&apos;</xsl:param>
  <xsl:param name="Today">CurrentDate</xsl:param>
  <xsl:variable name="dvt_1_automode">0</xsl:variable>

  <xsl:template match="/" xmlns:x="http://www.w3.org/2001/XMLSchema" xmlns:d="http://schemas.microsoft.com/sharepoint/dsp" xmlns:asp="http://schemas.microsoft.com/ASPNET/20" xmlns:__designer="http://schemas.microsoft.com/WebParts/v2/DataView/designer" xmlns:SharePoint="Microsoft.SharePoint.WebControls">
    <xsl:call-template name="dvt_1"/>
  </xsl:template>

  <xsl:template name="dvt_1">
    <xsl:variable name="dvt_StyleName">BulTitl</xsl:variable>
    <xsl:variable name="Rows" select="/dsQueryResponse/Rows/Row" />
    <xsl:variable name="RowLimit" select="1000" />
    <xsl:variable name="RowCount" select="count($Rows)" />
    <xsl:variable name="Events">
      <xsl:call-template name="dvt_1.body">
        <xsl:with-param name="Rows" select="$Rows" />
        <xsl:with-param name="FirstRow" select="1" />
        <xsl:with-param name="LastRow" select="$RowLimit" />
        <xsl:with-param name="RowCount" select="$RowCount" />
      </xsl:call-template>
    </xsl:variable>
    <script type="text/javascript">
      function myCalendar() {
      $(&apos;#calendar&apos;).fullCalendar({
      header: {
      left: &apos;prev,next today&apos;,
      center: &apos;title&apos;,
      right: &apos;month,agendaWeek,agendaDay&apos;
      },
      contentHeight: 550,
      editable: false,
      events: [
      <xsl:value-of select="$Events" />
      ],
      eventClick: function(event) {
      if (event.url) {
      ShowPopupDialog(event.url);
      return false;
      }
      }
      });
      }
      $(document).ready(function(){
      // Execute the calendar function
      myCalendar();
      });
    </script>
    <h3 class="detailsheading">Activities &amp; Tasks</h3>
    <div id="calendar">
    </div>
    <!-- #calendar -->
  </xsl:template>

  <xsl:template name="dvt_1.body">
    <xsl:param name="Rows" />
    <xsl:param name="FirstRow" />
    <xsl:param name="LastRow" />
    <xsl:param name="RowCount" />
    <xsl:for-each select="$Rows">
      <xsl:if test="(position() &gt;= $FirstRow and position() &lt;= $LastRow)">
        <xsl:call-template name="dvt_1.rowview">

          <xsl:with-param name="RowCount" select="$RowCount" />
        </xsl:call-template>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>
  <xsl:template name="dvt_1.rowview">
    <xsl:param name="RowCount" />
    <xsl:variable name="strListName" select="substring-after(@FileDirRef, 'Lists/')" />
    <xsl:variable name="fullTitle">
      <xsl:choose>
        <xsl:when test="$strListName = 'CaseRelatedDates'">
          <xsl:value-of select="concat('Event: ', translate(@Title, $dvt_apos, ''))" />
        </xsl:when>
        <xsl:when test="$strListName = 'TasksAndActivities'">
          <xsl:value-of select="concat('Task: ', translate(@Title, $dvt_apos, ''))" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="translate(@Title, $dvt_apos, '')" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="startDate">
      <xsl:choose>
        <xsl:when test="@EventDate != ''">
          <xsl:value-of select="@EventDate" />
        </xsl:when>
        <xsl:when test="@StartDate != ''">
          <xsl:value-of select="@StartDate" />
        </xsl:when>
        <xsl:when test="(@StartDate = '') and (@DueDate != '')">
          <xsl:value-of select="@DueDate" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$Today" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="endDate">
      <xsl:choose>
        <xsl:when test="@EndDate != ''">
          <xsl:value-of select="@EndDate" />
        </xsl:when>
        <xsl:when test="@DueDate != ''">
          <xsl:value-of select="@DueDate" />
        </xsl:when>
        <xsl:when test="(@DueDate = '') and (@StartDate != '')">
          <xsl:value-of select="@StartDate" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$Today"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="startYear" select="ddwrt:FormatDateTime(string($startDate), 1033, 'yyyy')" />
    <xsl:variable name="startMonth" select="number(ddwrt:FormatDateTime(string($startDate), 1033, 'MM'))-1" />
    <xsl:variable name="startDay" select="ddwrt:FormatDateTime(string($startDate), 1033, 'dd')" />
    <xsl:variable name="startHour" select="substring-before(ddwrt:FormatDateTime(string($startDate), 1033, 'HH:mm'), ':')" />
    <xsl:variable name="startMinute" select="substring-after(ddwrt:FormatDateTime(string($startDate), 1033, 'HH:mm'), ':')" />
    <xsl:variable name="endYear" select="ddwrt:FormatDateTime(string($endDate), 1033, 'yyyy')" />
    <xsl:variable name="endMonth" select="number(ddwrt:FormatDateTime(string($endDate), 1033, 'MM'))-1" />
    <xsl:variable name="endDay" select="ddwrt:FormatDateTime(string($endDate), 1033, 'dd')" />
    <xsl:variable name="endHour" select="substring-before(ddwrt:FormatDateTime(string($endDate), 1033, 'HH:mm'), ':')" />
    <xsl:variable name="endMinute" select="substring-after(ddwrt:FormatDateTime(string($endDate), 1033, 'HH:mm'), ':')" />
    <xsl:choose>
      <xsl:when test="position() != $RowCount">
        <xsl:call-template name="dvt_EventInfo">
          <xsl:with-param name="fullTitle" select="$fullTitle" />
          <xsl:with-param name="startYear" select="$startYear" />
          <xsl:with-param name="startMonth" select="$startMonth" />
          <xsl:with-param name="startDay" select="$startDay" />
          <xsl:with-param name="startHour" select="$startHour" />
          <xsl:with-param name="startMinute" select="$startMinute" />
          <xsl:with-param name="endYear" select="$endYear" />
          <xsl:with-param name="endMonth" select="$endMonth" />
          <xsl:with-param name="endDay" select="$endDay" />
          <xsl:with-param name="endHour" select="$endHour" />
          <xsl:with-param name="endMinute" select="$endMinute" />
        </xsl:call-template>,
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="dvt_EventInfo">
          <xsl:with-param name="fullTitle" select="$fullTitle" />
          <xsl:with-param name="startYear" select="$startYear" />
          <xsl:with-param name="startMonth" select="$startMonth" />
          <xsl:with-param name="startDay" select="$startDay" />
          <xsl:with-param name="startHour" select="$startHour" />
          <xsl:with-param name="startMinute" select="$startMinute" />
          <xsl:with-param name="endYear" select="$endYear" />
          <xsl:with-param name="endMonth" select="$endMonth" />
          <xsl:with-param name="endDay" select="$endDay" />
          <xsl:with-param name="endHour" select="$endHour" />
          <xsl:with-param name="endMinute" select="$endMinute" />
        </xsl:call-template>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="dvt_EventInfo">
    <xsl:param name="fullTitle" />
    <xsl:param name="startYear" />
    <xsl:param name="startMonth" />
    <xsl:param name="startDay" />
    <xsl:param name="startHour" />
    <xsl:param name="startMinute" />
    <xsl:param name="endYear" />
    <xsl:param name="endMonth" />
    <xsl:param name="endDay" />
    <xsl:param name="endHour" />
    <xsl:param name="endMinute" />
    <xsl:variable name="strPath" select="substring-after(@FileDirRef, '#')" />
    {
    title: &apos;<xsl:value-of select="$fullTitle" />111&apos;,
    start: new Date(<xsl:value-of select="concat($startYear, ', ', $startMonth, ', ', $startDay, ', ' , $startHour, ', ', $startMinute)" />),
    end: new Date(<xsl:value-of select="concat($endYear, ', ', $endMonth, ', ', $endDay, ', ', $endHour, ', ', $endMinute)" />),
    url: &apos;<xsl:value-of select="concat('/', $strPath, '/DispForm.aspx?ID=', @ID)" />&apos;,
    <xsl:choose>
      <xsl:when test="$startHour != '00' and $endHour !='00'">
        allDay: false
      </xsl:when>
      <xsl:otherwise>
        allDay: true
      </xsl:otherwise>
    </xsl:choose>
    }
  </xsl:template>

</xsl:stylesheet>