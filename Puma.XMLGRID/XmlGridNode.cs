using Lewis.Xml.FileConverters;

using System;
using System.ComponentModel;
using System.Xml;
using System.Collections;
using System.Windows.Forms;


namespace Lewis.Xml
{
	/// <summary>
	/// 
	/// </summary>
	public class XmlGridNode : System.ComponentModel.ICustomTypeDescriptor
	{
		public readonly XmlGridNodeSchemaBinded xmlGridNodeSchemaBinded;		
		public static readonly XmlGridNodeConverter converter = new XmlGridNodeConverter();
		
		public XmlGridNode(string XmlFileName, string XsdFileName, XmlGrid XmlGrid)
		{
			if (XmlFileName != null)
			{
				xmlGridNodeSchemaBinded = new XmlGridDocumentSchemaBinded(XmlFileName, XsdFileName, XmlGrid);
			}
			else
			{
				Xsd2Xml  xsd2Xml = new Xsd2Xml();

				XmlDocument xmlDocument = xsd2Xml.GetXmlDocument(XsdFileName);

				xmlGridNodeSchemaBinded = new XmlGridDocumentSchemaBinded(xmlDocument, XsdFileName, XmlGrid);
			}
		}

        public int Index
        {
            get { return this._propertyDescriptorCollection.IndexOf(GetPropertyDescriptor(xmlGridNodeSchemaBinded, false)); }
        }

		private XmlGridNode(XmlGridNodeSchemaBinded XmlGridNodeSchemaBinded)
		{
			xmlGridNodeSchemaBinded = XmlGridNodeSchemaBinded;
		}

		#region ICustomTypeDescriptor implementation
		
		public virtual String GetClassName()
		{
			return TypeDescriptor.GetClassName(this,true);
		}

		public AttributeCollection GetAttributes()
		{

			return TypeDescriptor.GetAttributes(this,true);
		}

		public virtual String GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public virtual TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent() 
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty() 
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType) 
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes) 
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public object GetPropertyOwner(PropertyDescriptor pd) 
		{
			return this;
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return GetProperties();
		}

		private PropertyDescriptorCollection _propertyDescriptorCollection;

		public PropertyDescriptorCollection GetProperties() 
		{
			XmlNodeSchemaBinded[] childNodes = xmlGridNodeSchemaBinded.ChildNodes;

			bool isParentIsCategory = false;

			if (xmlGridNodeSchemaBinded is XmlGridDocumentSchemaBinded)
			{
				childNodes = PillOutGapped(childNodes);

				isParentIsCategory = ((XmlGridNodeSchemaBinded)childNodes[0].ParentNode).bindAsCategory;
			}

			//if (_propertyDescriptorCollection != null && _propertyDescriptorCollection.Count == childNodes.Length)return _propertyDescriptorCollection;
			
			_propertyDescriptorCollection = new PropertyDescriptorCollection(null);

			System.Collections.Specialized.ListDictionary collectedGroups = new System.Collections.Specialized.ListDictionary();

			foreach (XmlGridNodeSchemaBinded childNode in childNodes)
			{
				if (childNode.omit)continue;

				if (childNode.XmlNode.NodeType == XmlNodeType.XmlDeclaration && childNode.xmlGridDocumentSchemaBinded.omitXmlDeclaration)continue;

				#region pickup collected 

				if (childNode.bindAsArray)
				{
					if (collectedGroups.Contains(childNode.Name))
					{
						((ArrayList)collectedGroups[childNode.Name]).Add(childNode);
					}
					else
					{
						ArrayList newGroup = new ArrayList();

						newGroup.Add(childNode);

						collectedGroups.Add(childNode.Name, newGroup);
					}

					continue;
				}

				#endregion

				_propertyDescriptorCollection.Add(GetPropertyDescriptor(childNode, isParentIsCategory));
			}

			foreach(DictionaryEntry de in collectedGroups)
			{
				_propertyDescriptorCollection.Add(new XmlGridNodesCollectionPropertyDescriptor((XmlGridNodeSchemaBinded[])((ArrayList)de.Value).ToArray(typeof(XmlGridNodeSchemaBinded)), isParentIsCategory));
			}

			return _propertyDescriptorCollection;
		}

		#endregion

		private static PropertyDescriptor GetPropertyDescriptor(XmlGridNodeSchemaBinded XmlGridNodeSchemaBinded, bool IsParentIsCategory)
		{
			if (XmlGridNodeSchemaBinded.IsComplexType)
			{
				XmlGridNodeSchemaBinded.ActivateChildNodes();

				return new XmlGridNodePropertyDescriptor(new XmlGridNode(XmlGridNodeSchemaBinded), IsParentIsCategory);
			}
			else
			{
				return new SimpleTypePropertyDescriptor(XmlGridNodeSchemaBinded, IsParentIsCategory);
			}
		}

		internal static PropertyDescriptor GetPropertyDescriptor(XmlGridNodeSchemaBinded XmlGridNodeSchemaBinded)
		{
			return GetPropertyDescriptor(XmlGridNodeSchemaBinded, false);
		}

		/// <summary>
		/// Suppose "gapped" is elements which has: Gap or BindAsCategory attribute
		/// </summary>
		private XmlNodeSchemaBinded[] PillOutGapped(XmlNodeSchemaBinded[] documentTopChilds)
		{
			//First may present xml declaration 
			XmlGridNodeSchemaBinded firstContentChild = (documentTopChilds[0].XmlNode.NodeType != XmlNodeType.XmlDeclaration) ? (XmlGridNodeSchemaBinded) documentTopChilds[0] : (XmlGridNodeSchemaBinded) documentTopChilds[1];

			XmlNodeSchemaBinded[] childNodes = null;

			XmlGridNodeSchemaBinded firstSubNode = firstContentChild;

			while (firstSubNode != null && (firstSubNode.gap || firstSubNode.bindAsCategory))
			{
				firstSubNode.ActivateChildNodes();
				
				childNodes = firstSubNode.ChildNodes;

				if (childNodes == null)
					firstSubNode = null;
				else
					firstSubNode = (XmlGridNodeSchemaBinded)childNodes[0];
			}

			if (firstSubNode != null){/*Throw exception: All nodes in document can't be gapped*/}

			//Document hadn't gapped nodes
			if (childNodes == null)return documentTopChilds;

			//Peekup cildren of parent siling's 
			ArrayList al = new ArrayList();

			foreach(XmlNodeSchemaBinded parentSibling in firstSubNode.ParentNode.ParentNode.ChildNodes)
			{
				if (parentSibling.XmlNode.NodeType == XmlNodeType.XmlDeclaration)continue;

				foreach(XmlNodeSchemaBinded parentSiblingChild in parentSibling.ChildNodes)
				{
					al.Add(parentSiblingChild);
				}				
			}

			return (XmlNodeSchemaBinded[]) al.ToArray(typeof(XmlNodeSchemaBinded));
		}
	}
}
