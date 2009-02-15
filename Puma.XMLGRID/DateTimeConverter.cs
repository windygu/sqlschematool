using System;

namespace Lewis.Xml
{
	public class DateTimeConverter : System.ComponentModel.DateTimeConverter
	{
		public DateTimeConverter()
		{
			
		}


		public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (_dateTimeType == XmlGridNodeSchemaBinded.DateTimeType.DateTime)
			{
				return ((System.DateTime)value).ToString("dd.MM.yyyy  HH:mm:ss");
			}

			return base.ConvertTo (context, culture, value, destinationType);
		}

		public XmlGridNodeSchemaBinded.DateTimeType _dateTimeType = XmlGridNodeSchemaBinded.DateTimeType.NonDateTime;

	}
}
