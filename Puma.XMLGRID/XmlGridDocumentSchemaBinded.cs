using System;
using System.Xml;

namespace Lewis.Xml
{
	/// <summary>
	/// 
	/// </summary>
    public class XmlGridDocumentSchemaBinded : XmlGridNodeSchemaBinded
	{
		public readonly bool omitXmlDeclaration = false;
		public readonly bool captionIsName = false;

		internal readonly XmlGrid _xmlGrid;

		#region Constructors
		
		public XmlGridDocumentSchemaBinded(XmlDocument XmlDocument, string SchemaFileName, XmlGrid XmlGrid) : base(XmlDocument, SchemaFileName)
		{
			if (XmlSchema.UnhandledAttributes != null)
			{
				foreach(System.Xml.XmlAttribute xmlAttribute in XmlSchema.UnhandledAttributes)
				{
					switch (xmlAttribute.LocalName)
					{
						case "CaptionIsName":
							if (xmlAttribute.Value == "true")captionIsName = true;
							break;
						case "OmitXmlDeclaration":
							if (xmlAttribute.Value == "true")omitXmlDeclaration = true;
							break;
					}
				}
			}
			_xmlGrid = XmlGrid;
		}
		
		public XmlGridDocumentSchemaBinded(string XmlFileName, string SchemaFileName, XmlGrid XmlGrid) : base(XmlFileName, SchemaFileName)
		{
			if (XmlSchema.UnhandledAttributes != null)
			{
				foreach(System.Xml.XmlAttribute xmlAttribute in XmlSchema.UnhandledAttributes)
				{
					switch (xmlAttribute.LocalName)
					{
						case "CaptionIsName":
							if (xmlAttribute.Value == "true")captionIsName = true;
							break;
						case "OmitXmlDeclaration":
							if (xmlAttribute.Value == "true")omitXmlDeclaration = true;
							break;
					}
				}
			}
			_xmlGrid = XmlGrid;
		}

		#endregion

		public void RefreshXmlGrid()
		{
			_xmlGrid.Refresh();
		}
	}
}
