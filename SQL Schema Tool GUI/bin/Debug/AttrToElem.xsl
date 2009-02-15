<?xml version='1.0' encoding='utf-8' ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml"/>
	<xsl:template match="DocumentElement">
		<xsl:element name="Data">
			<xsl:apply-templates select="*"/>
		</xsl:element>
	</xsl:template>
		
	<xsl:template match="*">
		<xsl:element name="{local-name()}">
			<xsl:for-each select="*">
				<xsl:element name="{local-name()}">
					<xsl:for-each select="@*">
						<xsl:text> </xsl:text>
					</xsl:for-each>
					<xsl:value-of select="."/>
				</xsl:element>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
</xsl:stylesheet>
