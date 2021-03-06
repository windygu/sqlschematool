using System;
using System.IO;
using System.Reflection;

namespace Lewis.Xml
{
    /// <summary>
    /// Class to read embedded files from the EXE or DLL file assembly.
    /// </summary>
    public class ResourceReader
    {
        /// <summary>
        /// Reads embedded XSLT files and returns their contents as a string.  Keep in mind that .Net Framework v1.1 only supports XSLT v1.0.
        /// You can of course use EXSLT to extend the XSL functions provided by the .Net Framework.
        /// <para/>
        /// <remarks>This is a sample of an XSL file used for the transformation process.
        /// <code escaped="true">
        /// 
        /// <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
        ///
        ///    <!-- Purpose: XSLT to convert SQL schema XML into DROP/CREATE of DB objects.
        ///		Known limitations: no support for indexed views or the index fillfactor. 
        ///		Author: Lindsey Lewis, 08-26-2005 -->
        ///		<xsl:output method="xml" omit-xml-declaration="yes"/>
        ///
        ///		<xsl:template match="/">
        ///			 <xsl:apply-templates/>
        ///		</xsl:template>
        ///
        ///		<!-- template to output ALL SQL statement types -->
        ///		<xsl:template match="DataBase_Schema">
        ///			<xsl:apply-templates select="Database"/>
        ///		</xsl:template>
        ///			
        ///		<!-- template to output comment header -->
        ///		<xsl:template match="Database">
        ///		-- DB Name: <xsl:value-of select="Name"/>
        ///		-- Output Date: <xsl:value-of select="Date"/>
        ///		-- Output Time: <xsl:value-of select="Time"/>
        ///		-- AutoGenerated SQL: using the SQL Schema Tool.
        ///		</xsl:template>
        ///	</xsl:stylesheet>
        ///	
        /// </code>
        /// </remarks>
        /// <para/>
        /// </summary>
        /// <param name="resourceName">string value representing the embedded resouce locator path.</param>
        /// <returns>returns an XSL document as a string.</returns>
        public static string ReadFromResource(string resourceName)
        {
            string result = String.Empty;
            Assembly a = Assembly.GetCallingAssembly();
            Stream s = a.GetManifestResourceStream(resourceName);
            if (s != null)
            {
                StreamReader sr = new StreamReader(s);
                result = sr.ReadToEnd();
                sr.Close();
                s.Close();
            }
            return result;
        }
    }
}
