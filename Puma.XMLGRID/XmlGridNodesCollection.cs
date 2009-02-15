using System;
using System.ComponentModel;
using System.Collections;
using System.Xml;

namespace Lewis.Xml
{
	/// <summary>
	/// 
	/// </summary>
	public class XmlGridNodesCollection :  ICustomTypeDescriptor
	{
		public XmlGridNodesCollection(PropertyDescriptorCollection PropertyDescriptorCollection)
		{
			_propertyDescriptorCollection = PropertyDescriptorCollection;

		}

        public int IndexOf(PropertyDescriptor value)
        {
            return this._propertyDescriptorCollection.IndexOf(value);
        }

        private PropertyDescriptorCollection _propertyDescriptorCollection;
		
		#region ICustomTypeDescriptor

		public String GetClassName()
		{
			return TypeDescriptor.GetClassName(this,true);
		}

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this,true);
		}

		public String GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
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

		public PropertyDescriptorCollection GetProperties() 
		{
			return _propertyDescriptorCollection;
		}
		
		#endregion

	}
}
