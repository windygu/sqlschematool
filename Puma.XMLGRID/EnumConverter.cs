using System;

namespace Lewis.Xml
{
	/// <summary>
	/// 
	/// </summary>
	public class EnumConverter : System.ComponentModel.TypeConverter
	{
		public EnumConverter(string[] EmumEntities) : base()
		{
			_emumEntities = EmumEntities;
		}


		public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(_emumEntities);
		}	

		private string [] _emumEntities;

	}
}
