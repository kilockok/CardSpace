using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;

namespace PersonalCardDemo.Helpers;

/// <summary>
/// 应用资源查找工具 - 集中管理画刷、图标、颜色的资源查找逻辑
/// </summary>
public static class ResourceHelper
{
    /// <summary>
    /// 从应用资源中查找画刷，未找到返回 null
    /// </summary>
    public static IBrush? FindBrush(string key)
    {
        var app = Application.Current;
        if (app is null) return null;

        if (app.TryGetResource(key, app.ActualThemeVariant, out var res) && res is IBrush brush)
            return brush;

        return null;
    }

    /// <summary>
    /// 从应用资源中查找画刷，未找到时返回 fallback
    /// </summary>
    public static IBrush FindBrush(string key, IBrush fallback)
    {
        return FindBrush(key) ?? fallback;
    }

    /// <summary>
    /// 从应用资源中查找 StreamGeometry 图标，未找到返回 null
    /// </summary>
    public static StreamGeometry? FindIcon(string key)
    {
        var app = Application.Current;
        if (app is null) return null;

        if (app.TryGetResource(key, app.ActualThemeVariant, out var res) && res is StreamGeometry geo)
            return geo;

        return null;
    }

    /// <summary>
    /// 解析颜色字符串为 SolidColorBrush，解析失败返回 null
    /// </summary>
    public static SolidColorBrush? ParseBrush(string colorStr)
    {
        try
        {
            return new SolidColorBrush(Color.Parse(colorStr));
        }
        catch
        {
            return null;
        }
    }
}
