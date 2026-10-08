using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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

/// <summary>
///   A plugin-defined version, parsable from <c>major.minor</c> and written
///   as that text, with no type converter.
/// </summary>
/// <param name="Major">The major part.</param>
/// <param name="Minor">The minor part.</param>
[Newtonsoft.Json.JsonConverter(typeof(TestVersionJsonConverter))]
public readonly record struct TestVersion(int Major, int Minor) : IParsable<TestVersion>
{
    /// <inheritdoc />
    public static TestVersion Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException($"\"{s}\" is not a version.");

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out TestVersion result)
    {
        result = default;
        var parts = s?.Split('.');
        if (parts is not { Length: 2 } || !int.TryParse(parts[0], provider, out var major) || !int.TryParse(parts[1], provider, out var minor))
            return false;

        result = new(major, minor);
        return true;
    }

    /// <inheritdoc />
    public override string ToString()
        => $"{Major}.{Minor}";
}

/// <summary>
///   Writes a <see cref="TestVersion"/> as its text.
/// </summary>
public sealed class TestVersionJsonConverter : Newtonsoft.Json.JsonConverter<TestVersion>
{
    /// <inheritdoc />
    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, TestVersion value, Newtonsoft.Json.JsonSerializer serializer)
        => writer.WriteValue(value.ToString());

    /// <inheritdoc />
    public override TestVersion ReadJson(
        Newtonsoft.Json.JsonReader reader,
        Type objectType,
        TestVersion existingValue,
        bool hasExistingValue,
        Newtonsoft.Json.JsonSerializer serializer
    ) => TestVersion.Parse((string)reader.Value!, CultureInfo.InvariantCulture);
}
