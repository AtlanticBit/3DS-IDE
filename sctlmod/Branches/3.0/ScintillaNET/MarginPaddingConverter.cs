#region Using Directives

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

#endregion Using Directives


namespace ScintillaNet
{
	/// <summary>
	/// Provides a type converter to convert <see cref="MarginPadding"/> values
	/// to and from various other representations.
	/// </summary>
	public class MarginPaddingConverter : TypeConverter
	{
		#region Methods

		/// <summary>
		/// Overridden. See <see cref="TypeConverter.CanConvertFrom"/>.
		/// </summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}


		/// <summary>
		/// Overridden. See <see cref="TypeConverter.CanConvertTo"/>.
		/// </summary>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;

			return base.CanConvertTo(context, destinationType);
		}


		/// <summary>
		/// Overridden. See <see cref="TypeConverter.ConvertFrom"/>.
		/// </summary>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string str = value as string;
			if (str == null)
				return base.ConvertFrom(context, culture, value);

			str = str.Trim();
			if (str.Length == 0)
				return null;

			if (culture == null)
				culture = CultureInfo.CurrentCulture;

			char ch = culture.TextInfo.ListSeparator[0];
			string[] tokens = str.Split(ch);
			int[] values = new int[tokens.Length];

			TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
			for (int i = 0; i < values.Length; i++)
				values[i] = (int)converter.ConvertFromString(context, culture, tokens[i]);

			if (values.Length != 2)
				throw new ArgumentException("Text cannot be parsed. The expected text format is \"Left,Right\".");

			return new MarginPadding(values[0], values[1]);
		}


		/// <summary>
		/// Overridden. See <see cref="TypeConverter.ConvertTo"/>.
		/// </summary>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
				throw new ArgumentNullException("destinationType");

			if (value is MarginPadding)
			{
				if (destinationType == typeof(string))
				{
					MarginPadding p = (MarginPadding)value;
					if (culture == null)
						culture = CultureInfo.CurrentCulture;

					string separator = culture.TextInfo.ListSeparator + " ";
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));

					string[] args = new string[2];
					args[0] = converter.ConvertToString(context, culture, p.Left);
					args[1] = converter.ConvertToString(context, culture, p.Right);
					return String.Join(separator, args);
				}

				if (destinationType == typeof(InstanceDescriptor))
				{
					MarginPadding p = (MarginPadding)value;
					if (p.ShouldSerializeBoth())
					{
						return new InstanceDescriptor(
							typeof(MarginPadding).GetConstructor(new Type[] { typeof(int) }),
							new object[] { p.Both });
					}
					else
					{
						return new InstanceDescriptor(
							typeof(MarginPadding).GetConstructor(new Type[] { typeof(int), typeof(int) }),
							new object[] { p.Left, p.Right });
					}
				}
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}


		/// <summary>
		/// Overridden. See <see cref="TypeConverter.CreateInstance"/>.
		/// </summary>
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			if (propertyValues == null)
				throw new ArgumentNullException("propertyValues");

			MarginPadding original = (MarginPadding)context.PropertyDescriptor.GetValue(context.Instance);

			int both = (int)propertyValues["Both"];
			if (original.Both != both)
			{
				return new MarginPadding(both);
			}
			else
			{
				return new MarginPadding(
					(int)propertyValues["Left"],
					(int)propertyValues["Right"]);
			}
		}


		/// <summary>
		/// Overridden. See <see cref="TypeConverter.GetCreateInstanceSupported"/>.
		/// </summary>
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}


		/// <summary>
		/// Overridden. See <see cref="TypeConverter.GetProperties"/>.
		/// </summary>
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(MarginPadding), attributes);
			return props.Sort(new string[] { "Left", "Right" });
		}


		/// <summary>
		/// Overridden. See <see cref="TypeConverter.GetPropertiesSupported"/>.
		/// </summary>
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		#endregion Methods


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MarginPaddingConverter"/> class.
		/// </summary>
		public MarginPaddingConverter()
		{
		}

		#endregion Constructors
	}
}
