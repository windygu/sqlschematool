<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

<xsl:template match="TablesFields">
	<xsl:apply-templates select="MyDatabase"/>
</xsl:template>

<xsl:template match="MyDatabase">
<HTML><HEAD></HEAD><STYLE>
  BODY {font-family:tahoma;font-size:8pt}
  TD   {font-size:8pt}
  TABLE {font-family:tahoma;font-size:8pt}
  .Guion {	font-family: Webdings;}
  </STYLE><BODY>
 <H1><xsl:value-of select="Database/Name"/> database</H1><H4> (<xsl:value-of select="Database/Date"/>)</H4>
<xsl:apply-templates select="Tables"/>
</BODY></HTML>
</xsl:template>


<xsl:template match="TABLE_VIEW">
    <BR/><BR/><B>
      <xsl:value-of select="TABLE_NAME"/></B><BR/><HR/>
        <TABLE BORDER="0" width="100%">
			<TR>
			<TD width="150px"><u>Field name</u></TD>
		<TD width="100px"><u>Data type</u></TD>
		<TD width="100px"><u>Default value</u></TD>
		<TD width="50px"><u>Allow nulls</u></TD>
		<TD width="*"><u>Description</u></TD>
		</TR>
	</TABLE>
	<TABLE BORDER="0" width="100%">
      <xsl:apply-templates select="COLUMN"/>
      </TABLE>
</xsl:template>

<xsl:template match="COLUMN">
    <TR>
		<TD width="150px"><i><xsl:value-of select="Column_name"/></i></TD>
		<TD width="100px"><xsl:value-of select="Type"/> (<xsl:value-of select="CHARACTER_MAXIMUM_LENGTH"/>)</TD>
		<TD width="100px"><xsl:value-of select="COLUMN_DEFAULT"/></TD>
		<TD width="50px"><xsl:if test="NULLABLE = 'YES' or (NULLABLE = 1)"><SPAN class="Guion">a</SPAN></xsl:if></TD>
		<TD width="*"><xsl:value-of select="Column_name"/></TD>
    </TR>
 </xsl:template>

</xsl:stylesheet>

  