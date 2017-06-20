using System;
using System.ComponentModel;
using System.Globalization;


namespace PowerLib.System.Numerics
{
	public sealed class HourAngleConverter : TypeConverter
	{
		public HourAngleConverter()
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType != null)
				switch (Type.GetTypeCode(sourceType))
				{
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Decimal:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.String:
						return true;
					case TypeCode.Object:
						if (sourceType == typeof(HourAngle))
							return true;
						break;
				}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType != null)
				switch (Type.GetTypeCode(destinationType))
				{
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Decimal:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.String:
						return true;
					case TypeCode.Object:
						if (destinationType == typeof(HourAngle))
							return true;
						break;
				}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value != null)
			{
				switch (Type.GetTypeCode(value.GetType()))
				{
      		case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Decimal:
					case TypeCode.Single:
					case TypeCode.Double:
						return new HourAngle((double)value);
					case TypeCode.String:
						return HourAngle.Parse((string)value);
					case TypeCode.Object:
						if (value.GetType() == typeof(HourAngle))
							return value ;
						break;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value != null && typeof(HourAngle).Equals(value) && destinationType != null)
			{
				switch (Type.GetTypeCode(destinationType))
				{
					case TypeCode.Int16:
						return (short)((HourAngle)value).ToDegree();
					case TypeCode.Int32:
						return (int)((HourAngle)value).ToDegree();
					case TypeCode.Int64:
						return (long)((HourAngle)value).ToDegree();
					case TypeCode.Decimal:
						return ((HourAngle)value).ToDegree();
					case TypeCode.Single:
						return (float)((HourAngle)value).ToDegree();
					case TypeCode.Double:
						return ((HourAngle)value).ToDegree();
					case TypeCode.String:
						return ((HourAngle)value).ToString();
					case TypeCode.Object:
						if (destinationType == typeof(HourAngle))
							return value ;
						break;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (value != null)
			{
				switch (Type.GetTypeCode(value.GetType()))
				{
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Decimal:
						decimal decValue = (decimal)value ;
						return decValue > (decimal)-HourAngle.MaxTotalHours && decValue < (decimal)HourAngle.MaxTotalHours;
					case TypeCode.Single:
					case TypeCode.Double:
						double dblValue = (double)value ;
						return dblValue > (double)-HourAngle.MaxTotalHours && dblValue < (double)HourAngle.MaxTotalHours;
					case TypeCode.String:
						HourAngle result;
						return HourAngle.TryParse((string)value, out result);
					case TypeCode.Object:
						if (value.GetType() == typeof(HourAngle))
							return true;
						break;
				}
			}
			return false;
		}
	}
}
