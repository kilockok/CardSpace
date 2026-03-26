using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace PersonalCardDemo.Styles;

/// <summary>
/// 风格管理实现
///
/// 职责：
/// 1. 管理 Fluent/Glass 风格资源字典的加载和切换
/// 2. 管理亮/暗主题切换
/// 3. 运行时动态替换资源字典
///
/// 实现思路：
/// - 维护当前加载的风格资源字典引用
/// - 切换时先移除旧资源，再加载新资源
/// - Glass 风格强制暗色主题
/// </summary>
public sealed class StyleManager : IStyleManager
{
    // 风格资源字典的 avares 路径
    private const string FluentStyleUri = "avares://PersonalCardDemo/Styles/FluentStyle.axaml";
    private const string GlassStyleUri = "avares://PersonalCardDemo/Styles/GlassStyle.axaml";

    // 当前加载的风格资源字典
    private ResourceDictionary? _currentStyleResource;

    public string CurrentStyle { get; private set; } = "fluent";
    public string CurrentTheme { get; private set; } = "light";

    public event EventHandler<StyleChangedEventArgs>? StyleChanged;

    /// <summary>
    /// 应用风格和主题
    /// </summary>
    public void ApplyStyle(string style, string theme)
    {
        var app = Application.Current;
        if (app == null) return;

        var oldStyle = CurrentStyle;
        var isGlass = string.Equals(style, "glass", StringComparison.OrdinalIgnoreCase);
        var normalizedTheme = NormalizeTheme(theme);

        // 移除旧的风格资源
        if (_currentStyleResource != null)
        {
            app.Resources.MergedDictionaries.Remove(_currentStyleResource);
            _currentStyleResource = null;
        }

        // 加载新的风格资源
        var uri = isGlass ? GlassStyleUri : FluentStyleUri;
        _currentStyleResource = LoadResourceDictionary(uri);
        if (_currentStyleResource != null)
        {
            app.Resources.MergedDictionaries.Add(_currentStyleResource);
        }

        // 设置主题
        if (isGlass)
        {
            // Glass 风格固定暗色
            app.RequestedThemeVariant = ThemeVariant.Dark;
            CurrentTheme = "dark";
        }
        else
        {
            var isDark = string.Equals(normalizedTheme, "dark", StringComparison.OrdinalIgnoreCase);
            app.RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;
            CurrentTheme = isDark ? "dark" : "light";
        }

        CurrentStyle = isGlass ? "glass" : "fluent";

        // 风格变化时触发事件
        if (!string.Equals(oldStyle, CurrentStyle, StringComparison.OrdinalIgnoreCase))
        {
            StyleChanged?.Invoke(this, new StyleChangedEventArgs
            {
                OldStyle = oldStyle,
                NewStyle = CurrentStyle,
                Theme = CurrentTheme
            });
        }
    }

    /// <summary>
    /// 加载资源字典
    /// </summary>
    private static ResourceDictionary? LoadResourceDictionary(string uri)
    {
        try
        {
            return (ResourceDictionary)AvaloniaXamlLoader.Load(new Uri(uri));
        }
        catch
        {
            return null;
        }
    }

    private static string NormalizeTheme(string? theme)
    {
        return string.Equals(theme, "black", StringComparison.OrdinalIgnoreCase)
            ? "dark"
            : theme ?? "light";
    }
}
