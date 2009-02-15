using System;
using System.ComponentModel;
using System.Xml;

namespace Lewis.Xml
{
	/// <summary>
	/// 
	/// </summary>
	public class SimpleTypePropertyDescriptor : PropertyDescriptor, IPropertyDescriptorCommon
	{
		public SimpleTypePropertyDescriptor(XmlGridNodeSchemaBinded XmlXsdGridNode) : base ( XmlXsdGridNode.Name, null)
		{
			_xmlXsdGridNode = XmlXsdGridNode;
		}		


		public SimpleTypePropertyDescriptor(XmlGridNodeSchemaBinded XmlXsdGridNode, bool IsParentIsCategory) : base ( XmlXsdGridNode.Name, null)
		{
			_xmlXsdGridNode = XmlXsdGridNode;

			if (IsParentIsCategory)
			{
				base.AttributeArray = new Attribute[] {new CategoryAttribute(((XmlGridNodeSchemaBinded)XmlXsdGridNode.ParentNode).displayedName)};
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
				return _xmlXsdGridNode.displayedName;
			}
		}


		public override string Description
		{
			get
			{
				return _xmlXsdGridNode.Documentation;
			}
		}

	
		public override TypeConverter Converter
		{
			get
			{
				IEnumerationRestriction enumerationRestriction = (IEnumerationRestriction)_xmlXsdGridNode.GetRestriction(XmlNodeSchemaBindedRestrictionsType.Enumeration);
				
				if (enumerationRestriction != null)
				{
					return new EnumConverter(enumerationRestriction.Entities);
				}

				if (_xmlXsdGridNode.dateTimeType != XmlGridNodeSchemaBinded.DateTimeType.NonDateTime)
				{
					_dateTimeConverter._dateTimeType = _xmlXsdGridNode.dateTimeType;
					
					return _dateTimeConverter;					
				}
				
				return base.Converter;
				
			}
		}

		
		public override bool IsReadOnly
		{
			get { return _xmlXsdGridNode.readOnly;  }
		}

		
		public override string Name
		{
			get {return _xmlXsdGridNode.Name;}
		}

		
		public override Type PropertyType
		{
			get
			{
				//Special case when treate "time" data type.
				//If this property return System.DateType when in any case PG will figured out what
				//one should use DateTimeEditor editor but this malicious behaviour for this case.
				if (_xmlXsdGridNode.dateTimeType == XmlGridNodeSchemaBinded.DateTimeType.Time)return typeof(string);

				return (_xmlXsdGridNode.SystemType == null) ? typeof(string) : _xmlXsdGridNode.SystemType;
			}
		}

		
		public override void ResetValue(object component) {}
	
		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}
		
		public override object GetValue(object component)
		{
			#region dateTime case

			if (_xmlXsdGridNode.dateTimeType != XmlGridNodeSchemaBinded.DateTimeType.NonDateTime)
			{
				switch(_xmlXsdGridNode.dateTimeType)
				{
					case XmlGridNodeSchemaBinded.DateTimeType.Date:
						return System.DateTime.ParseExact(_xmlXsdGridNode.Value.ToString(),"yyyy-MM-dd",null);
					case XmlGridNodeSchemaBinded.DateTimeType.Time:
						return _xmlXsdGridNode.Value;
                    case XmlGridNodeSchemaBinded.DateTimeType.DateTime:
                        {
                            DateTime dt = Convert.ToDateTime(_xmlXsdGridNode.Value);
                            return dt; //System.DateTime.ParseExact(dateTime, "yyyy-MM-ddTHH:mm:ss", null);
                        }
				}

			}

			#endregion

			return _xmlXsdGridNode.Value;			
		}

		
		public override void SetValue(object component, object Value)
		{

			IMinMaxLengthRestriction minMaxRestriction = (IMinMaxLengthRestriction)_xmlXsdGridNode.GetRestriction(XmlNodeSchemaBindedRestrictionsType.MinMaxLength);

			if ( minMaxRestriction != null && _xmlXsdGridNode.SystemType == typeof(string))
			{
				if (Value.ToString().Length < minMaxRestriction.MinLength)throw new InvalidOperationException("Length of value should be more than " + (minMaxRestriction.MinLength - 1).ToString() + " characters");

				if (Value.ToString().Length > minMaxRestriction.MaxLength)throw new InvalidOperationException("Length of value should be less than " + (minMaxRestriction.MaxLength + 1).ToString() + " characters");
			}

			if (_xmlXsdGridNode.dateTimeType != XmlGridNodeSchemaBinded.DateTimeType.NonDateTime)
			{
				#region dateTime case

				switch(_xmlXsdGridNode.dateTimeType)
				{
					case XmlGridNodeSchemaBinded.DateTimeType.Date:
						_xmlXsdGridNode.Value = ((System.DateTime)Value).ToString("yyyy-MM-dd");
						break;
					case XmlGridNodeSchemaBinded.DateTimeType.Time:
						_xmlXsdGridNode.Value = ((System.DateTime)Value).ToString("HH:mm:ss");
						break;
					case XmlGridNodeSchemaBinded.DateTimeType.DateTime:
						_xmlXsdGridNode.Value = ((System.DateTime)Value).ToString("yyyy-MM-ddTHH:mm:ss");
						break;
				}

				#endregion				
			}
			else
			{	
				_xmlXsdGridNode.Value = Value.ToString();
			}

			if (_xmlXsdGridNode.parentEntry)
			{
				//_xmlXsdGridNode.xmlGridDocumentSchemaBinded.RefreshXmlGrid(); Update mf here 
			}
		}
		
		#endregion

		private XmlGridNodeSchemaBinded _xmlXsdGridNode;

        private static Lewis.Xml.DateTimeConverter _dateTimeConverter = new Lewis.Xml.DateTimeConverter();


		#region IPropertyDescriptorCommon Members

		public XmlGridNodeSchemaBinded  XmlGridNodeSchemaBinded{ get{return _xmlXsdGridNode;} }

		#endregion
	}
}
