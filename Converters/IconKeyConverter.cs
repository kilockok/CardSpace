using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Styling;

namespace PersonalCardDemo.Converters;

/// <summary>
/// 将图标资源 key（如 "CppIcon"）转换为对应的 StreamGeometry
/// </summary>
public sealed class IconKeyConverter : IValueConverter
{
    public static readonly IconKeyConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string key || string.IsNullOrEmpty(key))
            return null;

        var app = Application.Current;
        if (app is null) return null;

        if (app.TryGetResource(key, app.ActualThemeVariant, out var resource) && resource is StreamGeometry geometry)
            return geometry;

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
/// 将颜色字符串（如 "#00599C"）转换为 SolidColorBrush
/// </summary>
public sealed class ColorStringConverter : IValueConverter
{
    public static readonly ColorStringConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string colorStr || string.IsNullOrEmpty(colorStr))
            return null;

        try
        {
            var color = Color.Parse(colorStr);
            return new SolidColorBrush(color);
        }
        catch
        {
            return null;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
