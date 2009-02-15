using System;
using System.Xml;
using System.ComponentModel;

namespace Lewis.Xml
{
	public class XmlGridNodesCollectionPropertyDescriptor : PropertyDescriptor
	{
		public XmlGridNodesCollectionPropertyDescriptor(XmlGridNodeSchemaBinded[] XmlGridNodesSchemaBinded) : base ("Collection", null)
		{

			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);

			foreach (XmlGridNodeSchemaBinded xmlGridNodeSchemaBinded in XmlGridNodesSchemaBinded)
			{
				propertyDescriptorCollection.Add(XmlGridNode.GetPropertyDescriptor(xmlGridNodeSchemaBinded));
			}
			
			_xmlGridNodesCollection = new XmlGridNodesCollection(propertyDescriptorCollection);		

			_collectionEditor = new CollectionEditor(propertyDescriptorCollection);

			_templateNode = XmlGridNodesSchemaBinded[0];

		}

		public XmlGridNodesCollectionPropertyDescriptor(XmlGridNodeSchemaBinded[] XmlGridNodesSchemaBinded, bool IsParentIsCategory) : base ("Collection", null)
		{

			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);

			foreach (XmlGridNodeSchemaBinded xmlGridNodeSchemaBinded in XmlGridNodesSchemaBinded)
			{
				propertyDescriptorCollection.Add(XmlGridNode.GetPropertyDescriptor(xmlGridNodeSchemaBinded));
			}
			
			_xmlGridNodesCollection = new XmlGridNodesCollection(propertyDescriptorCollection);		

			_collectionEditor = new CollectionEditor(propertyDescriptorCollection);

			_templateNode = XmlGridNodesSchemaBinded[0];

			if (IsParentIsCategory)
			{
				base.AttributeArray = new Attribute[] {new CategoryAttribute(((XmlGridNodeSchemaBinded)XmlGridNodesSchemaBinded[0].ParentNode).displayedName)};
			}
		}

		#region PropertyDescriptor overridens

		public override void ResetValue(object component)
		{}

		public override bool CanResetValue(object component)
		{
			return true;
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
				return _templateNode.arrayCaption != null ? _templateNode.arrayCaption : _templateNode.displayedName + " [Collection]";
			}
		}


		public override string Description
		{
			get
			{
				return "Collection of " + _templateNode.displayedName + " elements";
			}
		}

		
		public override bool IsReadOnly
		{
			get { return true;  }
		}

		
		public override TypeConverter Converter
		{
			get
			{
				return XmlGridNode.converter;
			}
		}


		public override string Name
		{
			get {return _templateNode.Name;}
		}

		public override Type PropertyType
		{
			get
			{
				return _xmlGridNodesCollection.GetType();
			}
		}

		
		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}

		
		public override object GetValue(object component)
		{	
			return _xmlGridNodesCollection;			
		}

		
		public override void SetValue(object component, object Value)
		{
			_xmlGridNodesCollection = (XmlGridNodesCollection)Value;
		}

		public override object GetEditor(Type editorBaseType)
		{
			return (_templateNode.xmlGridDocumentSchemaBinded._xmlGrid.EditCollectionMode == EditCollectionModes.BothEditors || _templateNode.xmlGridDocumentSchemaBinded._xmlGrid.EditCollectionMode == EditCollectionModes.CollectionEditor) ? _collectionEditor : null ;
		}


		#endregion

		private CollectionEditor _collectionEditor;

		private XmlGridNodesCollection _xmlGridNodesCollection;

		private XmlGridNodeSchemaBinded _templateNode;

	}
}
