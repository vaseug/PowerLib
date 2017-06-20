using System;
using System.ComponentModel;
using System.Globalization;


namespace PowerLib.System.Numerics
{
	public sealed class GradAngleConverter : TypeConverter
	{
		public GradAngleConverter()
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
						if (sourceType == typeof(GradAngle))
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
						if (destinationType == typeof(GradAngle))
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
						return new GradAngle((double)value);
					case TypeCode.String:
						return GradAngle.Parse((string)value);
					case TypeCode.Object:
						if (value.GetType() == typeof(GradAngle))
							return value ;
						break;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value != null && typeof(GradAngle).Equals(value) && destinationType != null)
			{
				switch (Type.GetTypeCode(destinationType))
				{
					case TypeCode.Int16:
						return (short)((GradAngle)value).ToGrads();
					case TypeCode.Int32:
						return (int)((GradAngle)value).ToGrads();
					case TypeCode.Int64:
						return (long)((GradAngle)value).ToGrads();
					case TypeCode.Decimal:
						return ((GradAngle)value).ToGrads();
					case TypeCode.Single:
						return (float)((GradAngle)value).ToGrads();
					case TypeCode.Double:
						return ((GradAngle)value).ToGrads();
					case TypeCode.String:
						return ((GradAngle)value).ToString();
					case TypeCode.Object:
						if (destinationType == typeof(GradAngle))
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
						return decValue > (decimal)-GradAngle.MaxTotalGrads && decValue< (decimal)GradAngle.MaxTotalGrads;
					case TypeCode.Single:
					case TypeCode.Double:
						double dblValue= (double)value ;
						return dblValue> (double)-GradAngle.MaxTotalGrads && dblValue< (double)GradAngle.MaxTotalGrads;
					case TypeCode.String:
						GradAngle result;
						return GradAngle.TryParse((string)value, out result);
					case TypeCode.Object:
						if (value.GetType() == typeof(GradAngle))
							return true;
						break;
				}
			}
			return false;
		}
	}
}
