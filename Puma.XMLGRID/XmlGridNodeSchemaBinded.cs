using System;
using System.Xml;
using System.Windows.Forms;

namespace Lewis.Xml
{
	/// <summary>
	/// 
	/// </summary>
	public class XmlGridNodeSchemaBinded : XmlNodeSchemaBindedStatic
	{
		public bool bindAsArray;
		public bool readOnly;
		public bool omit;
		public bool gap;
		public bool bindAsCategory;
		public bool parentEntry;
		private bool _isNodeFreezed = false;
		public string caption;
		public string arrayCaption;
		public string displayedName;
		public DateTimeType dateTimeType;
		public readonly XmlGridDocumentSchemaBinded xmlGridDocumentSchemaBinded;
		private object _freezedValue;

		public enum DateTimeType
		{
			NonDateTime,
			Date,
			Time,
			DateTime
		}

		private System.Collections.ArrayList _activatedChildrenNodes;

		//Main XmlGrid
		#region Constructors

		public XmlGridNodeSchemaBinded(XmlDocument XmlDocument, string SchemaFileName) : base(XmlDocument, SchemaFileName)
		{
			xmlGridDocumentSchemaBinded = (XmlGridDocumentSchemaBinded) this;
		}

		protected XmlGridNodeSchemaBinded(string XmlFileName, string SchemaFileName) : base (XmlFileName, SchemaFileName)
		{
			xmlGridDocumentSchemaBinded = (XmlGridDocumentSchemaBinded) this;
		}

		protected XmlGridNodeSchemaBinded(XmlNodeSchemaBinded ParentNode, int ChildNodeIndex) : base(ParentNode, ChildNodeIndex)
		{
			xmlGridDocumentSchemaBinded = ((XmlGridNodeSchemaBinded)ParentNode).xmlGridDocumentSchemaBinded;

			displayedName = xmlGridDocumentSchemaBinded.captionIsName && caption != null ? caption : Name;

			if (((XmlGridNodeSchemaBinded)ParentNode).IzNodeFreezed) FreezeNode();
		}

		protected XmlGridNodeSchemaBinded(XmlNode XmlNode, XmlNodeSchemaBinded TemplateNode) : base (XmlNode, TemplateNode)
		{
			xmlGridDocumentSchemaBinded= ((XmlGridNodeSchemaBinded)TemplateNode).xmlGridDocumentSchemaBinded;

			displayedName = xmlGridDocumentSchemaBinded.captionIsName && caption != null ? caption : Name;

			if (((XmlGridNodeSchemaBinded)TemplateNode).IzNodeFreezed) FreezeNode();
		}

		protected override XmlNodeSchemaBinded CreateChildNode(XmlNodeSchemaBinded ParentNode, int ChildNodeIndex)
		{
			return new XmlGridNodeSchemaBinded(ParentNode, ChildNodeIndex);
		}

		protected override XmlNodeSchemaBinded CloneNode(XmlNode XmlNode, XmlNodeSchemaBinded TemplateNode)
		{
			return new XmlGridNodeSchemaBinded(XmlNode, TemplateNode);
		}

		#endregion

		internal void ActivateChildNodes() 
		{
			XmlNodeSchemaBinded[] xmlNodesSchemaBinded = base.ChildNodes;

			xmlNodesSchemaBinded = base.ChildNodes;
			
			if (xmlNodesSchemaBinded == null || !IsComplexType)
			{
				_activatedChildrenNodes = null;

				return;
			}

			_activatedChildrenNodes = new System.Collections.ArrayList(xmlNodesSchemaBinded);
		}

		public override XmlNodeSchemaBinded AppendChild(XmlNodeSchemaBinded TemplateXmlNodeSchemaBinded)
		{
			XmlNodeSchemaBinded appendedNode =  base.AppendChild (TemplateXmlNodeSchemaBinded);

			if (_activatedChildrenNodes != null)
			{
				_activatedChildrenNodes.Add(appendedNode);
			}

			if(((XmlGridNodeSchemaBinded)TemplateXmlNodeSchemaBinded).IzNodeFreezed)
				((XmlGridNodeSchemaBinded)appendedNode).FreezeNode();
			
			return appendedNode;
		}

		public override void RemoveChild(XmlNodeSchemaBinded XmlNodeSchemaBinded)
		{
			if (_activatedChildrenNodes != null)
			{
				_activatedChildrenNodes.Remove(XmlNodeSchemaBinded);
			}

			base.RemoveChild (XmlNodeSchemaBinded);
		}

		/// <summary>
		/// Activate childs nodes if still not activated, but not initialized internal 
		/// activatedChildrenNodes array cause of in next time will be initialize again.
		/// For persrsist initialization children nodes use ActivateChildrenNodes method.
		/// </summary>
		public override XmlNodeSchemaBinded[] ChildNodes
		{
			get
			{
				if (_activatedChildrenNodes != null)return (XmlNodeSchemaBinded[])_activatedChildrenNodes.ToArray(typeof(XmlNodeSchemaBinded));

				return base.ChildNodes;
			}
		}

		protected override void InitializePersist()
		{	
			if (XmlSchemaElement == null) return;

			if (XmlSchemaElement.UnhandledAttributes != null)
			{
				foreach (System.Xml.XmlAttribute xmlAttribute in XmlSchemaElement.UnhandledAttributes)
				{
					switch (xmlAttribute.LocalName)
					{
						case "BindAsArray":
							bindAsArray = xmlAttribute.Value == "true" ? true : false;
							break;
						case "Omit":
							omit = xmlAttribute.Value == "true" ? true : false;
							break;
						case "BindAsCategory":
							bindAsCategory = xmlAttribute.Value == "true" ? true : false;
							break;
						case "Caption":
							caption = xmlAttribute.Value;
							break;
						case "Gap":
							gap = xmlAttribute.Value == "true" ? true : false;
							break;
						case "ReadOnly":
							readOnly = xmlAttribute.Value == "true" ? true : false;
							break;
						case "ParentEntry":
							parentEntry = xmlAttribute.Value == "true" ? true : false;
							break;
						case "ArrayCaption":
							arrayCaption = xmlAttribute.Value;
							break;
					}
				}
			}

			base.InitializePersist();

			if (SystemType == typeof(System.DateTime))
			{
				switch(XmlSchemaElement.SchemaTypeName.Name)
				{
					case "time":
						dateTimeType = DateTimeType.Time;
						break;
					case "date":
						dateTimeType = DateTimeType.Date;
						break;
					case "dateTime":
						dateTimeType = DateTimeType.DateTime;
						break;
				}
			}
			else
			{
				dateTimeType = DateTimeType.NonDateTime;
			}

		}

		public string PropertyString
		{
			get
			{
				System.Text.StringBuilder str = new System.Text.StringBuilder();

				foreach (XmlGridNodeSchemaBinded xmlNodeSchemaBinded in ChildNodes)
				{
					if (xmlNodeSchemaBinded.parentEntry)str.Append(xmlNodeSchemaBinded.Value + " ; ");
				}

				return str.Length == 0 ? "" : str.ToString().Substring(0,str.Length - 3);
			}
		}

		public override object Value
		{
			get
			{
				if (_isNodeFreezed)
					return _freezedValue;
				else
					return base.Value;
			}
			set
			{
				if (_isNodeFreezed)
					_freezedValue = value;
				else
					base.Value = value;
			}
		}

		#region freeze node

		public void FreezeNode()
		{
			_freezedValue = base.Value;

			_isNodeFreezed = true;

			if (_activatedChildrenNodes != null)
			{
				foreach (XmlGridNodeSchemaBinded childNode in _activatedChildrenNodes)
				{
					childNode.FreezeNode();
				}
			}
		}

		public void UnfreezeNode(bool CancelChangest)
		{
			if (!CancelChangest && !IsComplexType) base.Value = _freezedValue;

			_isNodeFreezed = false;

			_freezedValue = null;

			if (_activatedChildrenNodes != null)
			{
				foreach (XmlGridNodeSchemaBinded childNode in _activatedChildrenNodes)
				{
					childNode.UnfreezeNode(CancelChangest);
				}
			}
		}

		public bool IzNodeFreezed{get{return _isNodeFreezed;}}

		public object FreezedValue{get{return _freezedValue;}}

		#endregion
	}
}
