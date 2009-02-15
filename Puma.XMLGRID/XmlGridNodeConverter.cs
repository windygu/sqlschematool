using System;
using System.ComponentModel;

namespace Lewis.Xml
{
	/// <summary>
	/// 
	/// </summary>
	public class XmlGridNodeConverter : ExpandableObjectConverter
	{
		public XmlGridNodeConverter()
		{
			// 
			// TODO: Add constructor logic here
			//
		}

		public override object ConvertTo(ITypeDescriptorContext context, 
			System.Globalization.CultureInfo culture, 
			object Value, Type destType )
		{			

			if( destType == typeof(string) && Value is XmlGridNodesCollection)
			{
				return "";
			}

			if( destType == typeof(string) && Value is CollectionEditorEntry)
			{
				return "";
			}


			if( destType == typeof(string) && Value is XmlGridNode)
			{
				return ((XmlGridNode)Value).xmlGridNodeSchemaBinded.PropertyString;
			}
			

			return base.ConvertTo(context,culture,Value,destType);
		}


	}
}
