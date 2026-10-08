using System;
using System.ComponentModel;
using System.Globalization;

namespace Shoko.Plugin.GenUiTest;

/// <summary>
///   A plugin-defined colour, converted to and from <c>#rrggbb</c> by a type
///   converter, which also makes its option labels.
/// </summary>
/// <param name="Red">The red channel.</param>
/// <param name="Green">The green channel.</param>
/// <param name="Blue">The blue channel.</param>
[TypeConverter(typeof(TestTintConverter))]
public sealed record TestTint(byte Red, byte Green, byte Blue);

/// <summary>
///   Converts a <see cref="TestTint"/> to and from <c>#rrggbb</c>.
/// </summary>
public sealed class TestTintConverter : TypeConverter
{
    /// <inheritdoc />
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    /// <inheritdoc />
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

    /// <inheritdoc />
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        => value is string { Length: 7 } text
            ? new TestTint(Convert.ToByte(text[1..3], 16), Convert.ToByte(text[3..5], 16), Convert.ToByte(text[5..7], 16))
            : base.ConvertFrom(context, culture, value);

    /// <inheritdoc />
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        => value is TestTint tint && destinationType == typeof(string)
            ? $"#{tint.Red:x2}{tint.Green:x2}{tint.Blue:x2}"
            : base.ConvertTo(context, culture, value, destinationType);
}
