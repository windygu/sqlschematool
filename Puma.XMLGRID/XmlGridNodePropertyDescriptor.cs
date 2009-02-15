using System;
using System.ComponentModel;
using System.Xml;

namespace Lewis.Xml
{
	/// <summary>
	/// 
	/// </summary>
	public class XmlGridNodePropertyDescriptor : PropertyDescriptor , IPropertyDescriptorCommon
	{
		public XmlGridNodePropertyDescriptor(XmlGridNode XmlGridNode) : base (XmlGridNode.xmlGridNodeSchemaBinded.Name, null)
		{
			_xmlGridNode = XmlGridNode;
		}

		public XmlGridNodePropertyDescriptor(XmlGridNode XmlGridNode, bool IsParentIsCategory) : base (XmlGridNode.xmlGridNodeSchemaBinded.Name, null)
		{
			_xmlGridNode = XmlGridNode;

			if (IsParentIsCategory)
			{
				base.AttributeArray = new Attribute[] {new CategoryAttribute(((XmlGridNodeSchemaBinded)_xmlGridNode.xmlGridNodeSchemaBinded.ParentNode).displayedName)};
			}
		}

		#region PropertyDescriptor overridens

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get 
			{ 
				return null;
			}
		}

		public override string DisplayName
		{
			get 
			{
				return _xmlGridNode.xmlGridNodeSchemaBinded.displayedName;
			}
		}

		public override string Description
		{
			get
			{
				return _xmlGridNode.xmlGridNodeSchemaBinded.Documentation;
			}
		}

		public override object GetValue(object component)
		{
			return _xmlGridNode;
		}

		public override bool IsReadOnly
		{
			get { return _xmlGridNode.xmlGridNodeSchemaBinded.readOnly;}
		}

		public override string Name
		{
			get {return _xmlGridNode.xmlGridNodeSchemaBinded.Name;}
		}

		public override Type PropertyType
		{
			get { return _xmlGridNode.xmlGridNodeSchemaBinded.GetType(); }
		}

		public override void ResetValue(object component) {}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}

		public override void SetValue(object component, object value)
		{
			//_xmlGridNode.xmlGridNodeSchemaBinded.ChildNodes
			//TODO: add Setvalue for child properties
		}

		public override TypeConverter Converter
		{
			get
			{
				return XmlGridNode.converter;
			}
		}

		#endregion

		private readonly XmlGridNode _xmlGridNode;

		#region IPropertyDescriptorCommon Members

		public XmlGridNodeSchemaBinded  XmlGridNodeSchemaBinded{ get{return _xmlGridNode.xmlGridNodeSchemaBinded;} }

		#endregion
	}
}
