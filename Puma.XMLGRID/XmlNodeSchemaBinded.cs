using System;
using System.Xml;
using System.Xml.Schema;

namespace Lewis.Xml
{
    #region Enums
    public enum XmlNodeSchemaBindedType
	{
		Undefined,
		SimpleType,
		ComplexType
	}

	public enum XmlNodeSchemaBindedRestrictionsType
	{
		NoRestrictions = 0x0,
		Occurency = 0x1,
		MinMaxLength = 0x10,
		//MinMaxValue = 0x100, 
		Enumeration = 0x1000
    }
    #endregion
    #region Interfaces
    public interface IOccurencyRestriction
	{
		decimal MaxOccur {get;}
		decimal MinOccur {get;}
	}

	public interface IMinMaxLengthRestriction
	{
		int MaxLength {get;}
		int MinLength {get;}
	}

	public interface IMinMaxValueRestriction
	{
		int MinValue {get;}
		int MaxValue {get;}
	}

	public interface IEnumerationRestriction
	{
		string [] Entities{get;}
    }
    #endregion
    /// <summary>
	/// Represent XML node binded with schema.
	/// </summary>
	public class XmlNodeSchemaBinded
	{
        private System.Xml.Schema.XmlSchemaSet xss = new XmlSchemaSet();
        private XmlSchemaElement _xmlSchemaElement;
        private XmlNode _xmlNode;
        private readonly XmlSchema _xmlSchema;
        private int _indexInParentNode = -1;
        private XmlNodeSchemaBinded _parentNode;

        #region Constructors

		/// <summary>
		/// Create new node for particular document node.
		/// </summary>
		/// <param name="XmlFileName"></param>
		/// <param name="SchemaFileName"></param>
		public XmlNodeSchemaBinded(string XmlFileName, string SchemaFileName)
		{
			XmlDocument xmlDocument = new System.Xml.XmlDocument();			
			xmlDocument.Load(XmlFileName);
			System.IO.StreamReader reader = new System.IO.StreamReader(SchemaFileName);
			_xmlSchema = new XmlSchema();
			_xmlSchema = XmlSchema.Read(reader,null);
            xss.Add(_xmlSchema);
            xss.Compile();
			_xmlNode = xmlDocument;
            _indexInParentNode = _parentNode != null ? _parentNode.ChildNodes.Length : 0;
		}

		public XmlNodeSchemaBinded(XmlDocument XmlDocument, string SchemaFileName)
		{
			System.IO.StreamReader reader = new System.IO.StreamReader(SchemaFileName);
			_xmlSchema = new XmlSchema();
			_xmlSchema = XmlSchema.Read(reader,null);
            xss.Add(_xmlSchema);
            xss.Compile();
            _xmlNode = XmlDocument;
            _indexInParentNode = _parentNode != null ? _parentNode.ChildNodes.Length : 0;
        }


		protected XmlNodeSchemaBinded(XmlNodeSchemaBinded ParentNode, int ChildNodeIndex)
		{
			_xmlNode = ParentNode._xmlNode.ChildNodes[ChildNodeIndex];
			_xmlSchema = ParentNode._xmlSchema;
			_xmlSchemaElement = GetChildXmlSchemaElement(ParentNode._xmlSchemaElement, _xmlNode.Name);
			_parentNode = ParentNode;
			_indexInParentNode = ChildNodeIndex;
		}

		protected XmlNodeSchemaBinded(XmlNode XmlNode, XmlNodeSchemaBinded TemplateNode)
		{
			_xmlNode = XmlNode;
			_xmlSchema = TemplateNode._xmlSchema;
			_xmlSchemaElement = TemplateNode._xmlSchemaElement;
			_parentNode = TemplateNode._parentNode;
            _indexInParentNode = _parentNode != null ? _parentNode.ChildNodes.Length : 0;
		}

		/// <summary>
		/// This method need to be override in each derived class for proper instantiation
		/// children nodes in ChildNode property.
		/// </summary>
		/// <param name="ParentNode"></param>
		/// <param name="ChildNodeIndex"></param>
		/// <returns></returns>
		protected virtual XmlNodeSchemaBinded CreateChildNode(XmlNodeSchemaBinded ParentNode, int ChildNodeIndex)
		{
			return new XmlNodeSchemaBinded(ParentNode, ChildNodeIndex);
		}

		protected virtual XmlNodeSchemaBinded CloneNode(XmlNode XmlNode, XmlNodeSchemaBinded TemplateNode)
		{
			return new XmlNodeSchemaBinded(XmlNode, TemplateNode);
		}
		#endregion

		public virtual XmlNodeSchemaBinded AppendChild(XmlNodeSchemaBinded TemplateXmlNodeSchemaBinded)
		{
			_xmlNode.AppendChild(TemplateXmlNodeSchemaBinded.XmlNode.Clone());
			int childsCnt = this.XmlNode.ChildNodes.Count;
			XmlNodeSchemaBinded appendedNode = CreateChildNode(this, childsCnt - 1);
			return appendedNode;
		}

		public virtual void RemoveChild(XmlNodeSchemaBinded XmlNodeSchemaBinded)
		{
			_xmlNode.RemoveChild(XmlNodeSchemaBinded._xmlNode);
		}

		public XmlNodeSchemaBinded Clone()
		{
			return CloneNode(_xmlNode.Clone(), this);
		}

		public XmlNodeSchemaBinded ParentNode
		{
			get { return _parentNode; }
		}

		public virtual XmlNodeSchemaBinded[] ChildNodes
		{
			get
			{
				int childsCount = _xmlNode.ChildNodes.Count;
				if (childsCount == 0) return null;
				if (childsCount == 1 && _xmlNode.ChildNodes[0].NodeType == XmlNodeType.Text)return null;
				XmlNodeSchemaBinded[] childNodes = new XmlNodeSchemaBinded[childsCount];
				for(int i = 0; i <= childsCount - 1; i++)
				{
					childNodes[i] = CreateChildNode(this, i);
				}
				return childNodes;
			}
		}

		private XmlSchemaElement GetChildXmlSchemaElement(XmlSchemaElement ParentXmlSchemaNode, string XmlSchemaNodeName)
		{
			if (ParentXmlSchemaNode == null || ParentXmlSchemaNode.SchemaType == null)
			{
				//Parent node is document
				XmlSchemaObjectTable xmlSchemaElements = _xmlSchema.Elements;
				foreach(XmlSchemaElement xmlSchemaElement in xmlSchemaElements.Values)
				{
					if (xmlSchemaElement.Name == XmlSchemaNodeName)
					{
						return xmlSchemaElement;
					}
				}
			}
			else
			{			
				XmlSchemaComplexType complexType = ParentXmlSchemaNode.SchemaType as XmlSchemaComplexType;
				//if (complexType == null)Exception unexpeceted
				XmlSchemaSequence xmlSchemaSequency = complexType.Particle as XmlSchemaSequence;
				//if (xmlSchemaSequency == null)Exception unexpeceted
				foreach(XmlSchemaElement xmlSchemaSubElement in xmlSchemaSequency.Items)
				{
					if (XmlSchemaNodeName == xmlSchemaSubElement.Name)return  xmlSchemaSubElement;
				}
			}
			//Exception corresponded schema element undefined
			return null;
		}

		public XmlNode XmlNode
		{
			get { return _xmlNode; }
			set { _xmlNode = value; }
		}

		public XmlSchema XmlSchema
		{
			get { return _xmlSchema; }
		}

		public XmlSchemaElement XmlSchemaElement
		{
			get { return _xmlSchemaElement; }
		}

		#region Utils 

        public int Index
        {
            get { return _indexInParentNode; }
        }

		public string Name
		{
			get { return _xmlNode.Name; }
		}

		/// <summary>
		/// Retun value of simple type node (if node contain subnode with text type then return one value)
		/// If node had complex type then return  null.
		/// </summary>
		public virtual object Value
		{
			get 
            {
				if (_xmlNode.ChildNodes.Count == 1 && _xmlNode.FirstChild.NodeType != XmlNodeType.Text) return null;
				if (_xmlNode.ChildNodes.Count == 1) return _xmlNode.FirstChild.Value;
				return _xmlNode.Value;
			}
			set
			{
				string sValue = value.ToString();
				#region Validate property value

				if (_xmlNode.ChildNodes.Count == 1 && _xmlNode.FirstChild.NodeType != XmlNodeType.Text)
				{
					throw new System.InvalidOperationException("Can't set value on complex type");
				}

				IMinMaxLengthRestriction minMaxLengthRestriction = (IMinMaxLengthRestriction)GetRestriction(XmlNodeSchemaBindedRestrictionsType.MinMaxLength);

				if (minMaxLengthRestriction != null)
				{
					if (sValue.Length < minMaxLengthRestriction.MinLength)
					{
						throw new System.InvalidOperationException("Property out of range : min length = " + minMaxLengthRestriction.MinLength.ToString());
					}
					if (sValue.Length > minMaxLengthRestriction.MaxLength)
					{
						throw new System.InvalidOperationException("Property out of range : max length = " + minMaxLengthRestriction.MaxLength.ToString());
					}

				}

				IEnumerationRestriction enumerationRestriction = (IEnumerationRestriction)GetRestriction(XmlNodeSchemaBindedRestrictionsType.Enumeration);

				if (enumerationRestriction != null)
				{
					System.Text.StringBuilder sEnumEntities = new System.Text.StringBuilder();
					bool fFound = false;
					foreach (string s in enumerationRestriction.Entities){sEnumEntities.Append(s + "; "); if (s == sValue) {fFound = true; break;}}
					if (!fFound)throw new System.InvalidOperationException("Property out of enumeration : " + sEnumEntities.ToString());
				}

				#endregion
				if (_xmlNode.ChildNodes.Count == 1)
					_xmlNode.FirstChild.InnerText = sValue;	
				else _xmlNode.InnerText = sValue;
			}
		}

		public bool IsComplexType
		{
			get
			{
				if ( (_xmlNode.ChildNodes.Count == 1 && _xmlNode.ChildNodes[0].NodeType == XmlNodeType.Text) || _xmlNode.ChildNodes.Count == 0)return false;
				return true;
			}
		}
		
		#region virtuals

		public virtual Type SystemType
		{
			get
			{
				if (_xmlSchemaElement == null) return null;
				XmlSchemaSimpleType simpleType = _xmlSchemaElement.SchemaType as XmlSchemaSimpleType;
                XmlSchemaDatatype xmlSchemaDatatype = (simpleType != null) ? simpleType.Datatype : _xmlSchemaElement.ElementSchemaType.Datatype;
				return (xmlSchemaDatatype == null) ? null : xmlSchemaDatatype.ValueType;
			}
		}


		/// <summary>
		/// Return documentation string composed from Annotation element of schema file 
		/// for current node.
		/// </summary>
		public virtual string Documentation
		{
			get
			{
				if (_xmlSchemaElement == null )return null;
				System.Text.StringBuilder docs = new System.Text.StringBuilder();
				if (_xmlSchemaElement.Annotation != null)
				{
					foreach (object annotItem in _xmlSchemaElement.Annotation.Items)
					{
						XmlSchemaDocumentation  schemaDocs = annotItem as  XmlSchemaDocumentation;
						if (schemaDocs != null && schemaDocs.Markup[0] != null) docs.Append(schemaDocs.Markup[0].Value + "\n");	
					}
				}
				return docs.Length == 0 ? null : docs.ToString().TrimEnd(new char[] {'\n'});
			}
		}


		#region RestrictionObjects

		private class OccurencyRestriction : IOccurencyRestriction	
		{
			public OccurencyRestriction(decimal MinOccur, decimal MaxOccur) 
			{
				_minOccur = MinOccur;
				_maxOccur = MaxOccur;
			}

			public decimal MaxOccur {get{return _maxOccur;}}
			public decimal MinOccur {get{return _minOccur;}}

			private decimal _minOccur = -1;
			private decimal _maxOccur = -1;
		}


		private class MinMaxLengthRestriction : IMinMaxLengthRestriction
		{
			public MinMaxLengthRestriction(int MinLength, int MaxLength)
			{
				_minLength = MinLength;
				_maxLength = MaxLength;
			}

			private int _minLength = -1;
			private int _maxLength = -1;

			public int MaxLength{get{return _maxLength;}}
			public int MinLength{get{return _minLength;}}
		}


		private class MinMaxValueRestriction : IMinMaxValueRestriction
		{
			public MinMaxValueRestriction(int MinValue, int MaxValue)
			{
				_minValue = MinValue;
				_maxValue = MaxValue;
			}

			public int MinValue{get{return _minValue;}}
			public int MaxValue{get{return _maxValue;}}

			private int _minValue = -1;
			private int _maxValue = -1;
		}


		private class EnumerationRestriction : IEnumerationRestriction
		{
            public string[] Entities { get { return _entities; } }
            private string[] _entities = null;

            public EnumerationRestriction(string[] entities)
			{
				_entities = entities;
			}
		}

		#endregion

		public virtual object GetRestriction(XmlNodeSchemaBindedRestrictionsType XmlNodeSchemaBindedRestriction)
		{
			if (_xmlSchemaElement == null )return null;
			int min = -1;
			int max = -1;
			#region OccurencyRestriction

			if (XmlNodeSchemaBindedRestriction == XmlNodeSchemaBindedRestrictionsType.Occurency)
			{
				return new OccurencyRestriction(_xmlSchemaElement.MinOccurs, _xmlSchemaElement.MaxOccurs);
			}
			#endregion
			XmlSchemaSimpleType xmlSchemaSimpleType = _xmlSchemaElement.ElementSchemaType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType != null)
			{
				#region MinMaxLengthRestriction

				if (XmlNodeSchemaBindedRestriction == XmlNodeSchemaBindedRestrictionsType.MinMaxLength)
				{
					XmlSchemaSimpleTypeRestriction  simpleTypeRestriction = xmlSchemaSimpleType.Content as XmlSchemaSimpleTypeRestriction;

					if (simpleTypeRestriction != null)
					{
						foreach(XmlSchemaFacet facet in simpleTypeRestriction.Facets)
						{
							XmlSchemaMinLengthFacet xmlSchemaMinLengthFacet = facet as XmlSchemaMinLengthFacet;

							if (xmlSchemaMinLengthFacet != null)min = int.Parse(xmlSchemaMinLengthFacet.Value);

							XmlSchemaMaxLengthFacet xmlSchemaMaxLengthFacet = facet as XmlSchemaMaxLengthFacet;

							if (xmlSchemaMaxLengthFacet != null)max = int.Parse(xmlSchemaMaxLengthFacet.Value);

							if (min != -1 && max != -1)break;
						}

						if (min != -1 || max != -1)return new MinMaxLengthRestriction(min, max);
							
						return null;
					}

					return null;
				}

				#endregion
				#region MinMaxValueRestriction

				/*
				if (XmlNodeSchemaBindedRestriction == XmlNodeSchemaBindedRestrictionsType.MinMaxValue)
				{
					XmlSchemaSimpleTypeRestriction  simpleTypeRestriction = xmlSchemaSimpleType.Content as XmlSchemaSimpleTypeRestriction;

					if (simpleTypeRestriction != null)
					{
						int iMin = simpleTypeRestriction.Facets.IndexOf(new XmlSchemaMinInclusiveFacet());

						int iMax = simpleTypeRestriction.Facets.IndexOf(new XmlSchemaMaxInclusiveFacet());

						if (iMin != -1) min = int.Parse(((XmlSchemaMinInclusiveFacet)simpleTypeRestriction.Facets[iMin]).Value);

						if (iMax != -1) max = int.Parse(((XmlSchemaMaxInclusiveFacet)simpleTypeRestriction.Facets[iMax]).Value);

						if (min != -1 || max != -1)return new MinMaxValueRestriction(min, max);
					}

					return null;
			
				}
				*/

				#endregion
				#region EnumerationRestriction

				if (XmlNodeSchemaBindedRestriction == XmlNodeSchemaBindedRestrictionsType.Enumeration)
				{
					XmlSchemaSimpleTypeRestriction  simpleTypeRestriction = xmlSchemaSimpleType.Content as XmlSchemaSimpleTypeRestriction;

					System.Collections.Specialized.StringCollection enumVals = new System.Collections.Specialized.StringCollection();

					foreach(XmlSchemaFacet facet in simpleTypeRestriction.Facets)
					{
						XmlSchemaEnumerationFacet xmlSchemaEnumerationFacet = facet as XmlSchemaEnumerationFacet;

						if (xmlSchemaEnumerationFacet != null)enumVals.Add(xmlSchemaEnumerationFacet.Value);
					}

					string [] sEnumVals = null;
					
					if (enumVals.Count != 0)
					{
						sEnumVals = new string [enumVals.Count];

						enumVals.CopyTo(sEnumVals, 0);
					}

					return (sEnumVals == null) ? null : new EnumerationRestriction(sEnumVals);
				}

				#endregion
			}
			//Exception unrecognized restriction type 
			return null;
		}
		
		#endregion
		#endregion
			
		private static string TrimString(string String)
		{
			return String.Trim(new char[] {'\n','\r','\t',' '});
		}
	}

	public class XmlNodeSchemaBindedStatic : XmlNodeSchemaBinded
	{
        private string _documentation;
        private System.Collections.Specialized.ListDictionary _restrictions = new System.Collections.Specialized.ListDictionary();
        private Type _systemType;

        #region Constructors

		/// <summary>
		/// Create new node for particular document node.
		/// </summary>
		/// <param name="XmlFileName"></param>
		/// <param name="SchemaFileName"></param>
		public XmlNodeSchemaBindedStatic(string XmlFileName, string SchemaFileName) : base (XmlFileName, SchemaFileName)
		{
			InitializePersist();
		}

		public XmlNodeSchemaBindedStatic(XmlDocument XmlDocument, string SchemaFileName) : base (XmlDocument, SchemaFileName)
		{
			InitializePersist();
		}

		protected XmlNodeSchemaBindedStatic(XmlNodeSchemaBinded ParentNode, int ChildNodeIndex) : base (ParentNode, ChildNodeIndex)
		{
			InitializePersist();
		}

		protected XmlNodeSchemaBindedStatic(XmlNode XmlNode, XmlNodeSchemaBinded TemplateNode) : base (XmlNode, TemplateNode)
		{
			InitializePersist();
		}

		protected override XmlNodeSchemaBinded CreateChildNode(XmlNodeSchemaBinded ParentNode, int ChildNodeIndex)
		{
			return new XmlNodeSchemaBindedStatic(ParentNode, ChildNodeIndex);
		}

		protected override XmlNodeSchemaBinded CloneNode(XmlNode XmlNode, XmlNodeSchemaBinded TemplateNode)
		{
			return new XmlNodeSchemaBindedStatic(XmlNode, TemplateNode);
		}

		#endregion
		protected virtual void InitializePersist()
		{
			_documentation = base.Documentation;
			_systemType = base.SystemType;
			object obj = base.GetRestriction(XmlNodeSchemaBindedRestrictionsType.Enumeration);
			if (obj != null)_restrictions.Add(XmlNodeSchemaBindedRestrictionsType.Enumeration, obj);
			obj = base.GetRestriction(XmlNodeSchemaBindedRestrictionsType.MinMaxLength);
			if (obj != null)_restrictions.Add(XmlNodeSchemaBindedRestrictionsType.MinMaxLength, obj);
			obj = base.GetRestriction(XmlNodeSchemaBindedRestrictionsType.Occurency);
			if (obj != null)_restrictions.Add(XmlNodeSchemaBindedRestrictionsType.Occurency, obj);
		}

		#region overridens

		public override Type SystemType
		{
			get
			{
				return _systemType;
			}
		}


		public override string Documentation
		{
			get
			{
				return _documentation;
			}
		}

		public override object GetRestriction(XmlNodeSchemaBindedRestrictionsType XmlNodeSchemaBindedRestriction)
		{
			return _restrictions[XmlNodeSchemaBindedRestriction];
		}

		#endregion
	}	

}
