using System;
using System.Xml;
using System.ComponentModel;

namespace Lewis.Xml
{
	/// <summary>
	/// 
	/// </summary>
	[TypeConverter(typeof(XmlGridNodeConverter))]
	public class CollectionEditorEntry :  System.ComponentModel.ICustomTypeDescriptor
	{
		public CollectionEditorEntry(PropertyDescriptor PropertyDescriptor)
		{
			_propertyDescriptorCollection = new PropertyDescriptorCollection(new PropertyDescriptor[] {PropertyDescriptor});
		}

		internal readonly PropertyDescriptorCollection _propertyDescriptorCollection;

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
			return XmlGridNode.converter;
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
