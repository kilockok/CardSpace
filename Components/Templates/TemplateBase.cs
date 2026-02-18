using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using PersonalCardDemo.Config.Models;
using PersonalCardDemo.Helpers;

namespace PersonalCardDemo.Components.Templates;

/// <summary>
/// 模板基类 - 提供通用的控件构建、属性覆写、卡片外壳构建
/// </summary>
public abstract class TemplateBase
{
    // -- 资源查找（委托到 ResourceHelper） --

    protected static IBrush? TryGetBrush(string key) => ResourceHelper.FindBrush(key);
    protected static StreamGeometry? TryGetIcon(string key) => ResourceHelper.FindIcon(key);
    protected static SolidColorBrush? ParseBrush(string colorStr) => ResourceHelper.ParseBrush(colorStr);

    // -- 覆写 --

    protected static ElementOverride? GetOverride(ComponentConfig config, string elementName)
    {
        if (config.Overrides == null) return null;
        return config.Overrides.GetValueOrDefault(elementName);
    }

    /// <summary>
    /// 应用文本元素的覆写属性
    /// </summary>
    protected static void ApplyTextOverride(TextBlock textBlock, ElementOverride? ov)
    {
        if (ov == null) return;

        if (ov.FontSize.HasValue)
            textBlock.FontSize = ov.FontSize.Value;

        if (ov.FontWeight != null)
            textBlock.FontWeight = ParseFontWeight(ov.FontWeight);

        if (ov.FontStyle != null)
            textBlock.FontStyle = ParseFontStyle(ov.FontStyle);

        if (ov.Foreground != null)
            textBlock.Foreground = ParseBrush(ov.Foreground);

        if (ov.Visible.HasValue)
            textBlock.IsVisible = ov.Visible.Value;

        if (ov.Opacity.HasValue)
            textBlock.Opacity = ov.Opacity.Value;

        if (ov.Text != null)
            textBlock.Text = ov.Text;
    }

    /// <summary>
    /// 应用 Border 元素的覆写属性
    /// </summary>
    protected static void ApplyBorderOverride(Border border, ElementOverride? ov)
    {
        if (ov == null) return;

        if (ov.Width.HasValue)
            border.Width = ov.Width.Value;

        if (ov.Height.HasValue)
            border.Height = ov.Height.Value;

        if (ov.CornerRadius.HasValue)
            border.CornerRadius = new CornerRadius(ov.CornerRadius.Value);

        if (ov.Background != null)
            border.Background = ParseBrush(ov.Background);

        if (ov.Visible.HasValue)
            border.IsVisible = ov.Visible.Value;

        if (ov.Opacity.HasValue)
            border.Opacity = ov.Opacity.Value;

        if (ov.Margin != null)
            border.Margin = ParseThickness(ov.Margin);
    }

    // -- 卡片外壳构建（通用实现，子类不再重复） --

    /// <summary>
    /// Fluent 单层结构：Border 承载 Background/Border/Shadow，由 AXAML fluent-card 样式控制
    /// </summary>
    /// <param name="clipContent">是否裁剪子内容（地图等全出血图片需要，有 scale 动画的卡片不要）</param>
    protected static Border BuildFluentShell(string name, Control content, bool clipContent = false)
    {
        return new Border
        {
            Name = name,
            ClipToBounds = clipContent,
            Classes = { "fluent-card" },
            Effect = new DropShadowEffect
            {
                OffsetX = 0, OffsetY = 2, BlurRadius = 8,
                Color = Colors.Black, Opacity = 0.06
            },
            Child = content
        };
    }

    /// <summary>
    /// Glass 双层结构：outerBorder -> Panel -> [AcrylicBorder, innerBorder -> content]
    /// </summary>
    protected static Border BuildGlassShell(string name, Control content)
    {
        var outerBorder = new Border
        {
            CornerRadius = new CornerRadius(10),
            ClipToBounds = true,
            Name = name,
            Classes = { "glass-card" }
        };

        var panel = new Panel();

        panel.Children.Add(new ExperimentalAcrylicBorder
        {
            IsHitTestVisible = false,
            Material = new ExperimentalAcrylicMaterial
            {
                BackgroundSource = AcrylicBackgroundSource.Digger,
                TintColor = Color.Parse("#FFFFFF"),
                TintOpacity = 0.02,
                MaterialOpacity = 0.15
            }
        });

        var innerBorder = new Border
        {
            BorderThickness = new Thickness(1),
            BorderBrush = TryGetBrush("GlassCardBorder"),
            Background = TryGetBrush("GlassCardGradient"),
            Effect = new DropShadowEffect
            {
                OffsetX = 0, OffsetY = 8, BlurRadius = 16,
                Color = Colors.Black, Opacity = 0.10
            },
            Child = content
        };

        panel.Children.Add(innerBorder);
        outerBorder.Child = panel;
        return outerBorder;
    }

    // -- 资源加载 --

    /// <summary>
    /// 从文件系统加载图片，路径相对于 exe 所在目录
    /// Assets 不再嵌入程序集，统一从外部文件加载
    /// </summary>
    protected static IImage? LoadImage(string relativePath, string? fallbackAvares = null)
    {
        var basePath = AppContext.BaseDirectory;
        var fullPath = System.IO.Path.Combine(basePath, relativePath);

        if (System.IO.File.Exists(fullPath))
        {
            try
            {
                return new Bitmap(fullPath);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"[LoadImage] 文件损坏或格式不支持: {fullPath}");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[LoadImage] 文件不存在: {fullPath}，请检查 Assets 文件夹是否在 exe 同级目录");
        }

        return null;
    }

    /// <summary>
    /// 加载 avares:// 协议的嵌入图片资源
    /// </summary>
    protected static IImage? LoadAvaloniaResource(string uri)
    {
        try
        {
            return new Bitmap(Avalonia.Platform.AssetLoader.Open(new Uri(uri)));
        }
        catch
        {
            return null;
        }
    }

    // -- 解析工具 --

    protected static Thickness ParseThickness(string value)
    {
        try
        {
            var parts = value.Split(',');
            return parts.Length switch
            {
                1 => new Thickness(double.Parse(parts[0])),
                2 => new Thickness(double.Parse(parts[0]), double.Parse(parts[1])),
                4 => new Thickness(
                    double.Parse(parts[0]), double.Parse(parts[1]),
                    double.Parse(parts[2]), double.Parse(parts[3])),
                _ => default
            };
        }
        catch
        {
            return default;
        }
    }

    protected static FontWeight ParseFontWeight(string value)
    {
        return value.ToLowerInvariant() switch
        {
            "thin" => FontWeight.Thin,
            "light" => FontWeight.Light,
            "regular" or "normal" => FontWeight.Regular,
            "medium" => FontWeight.Medium,
            "semibold" => FontWeight.SemiBold,
            "bold" => FontWeight.Bold,
            "extrabold" => FontWeight.ExtraBold,
            "black" => FontWeight.Black,
            _ => FontWeight.Regular
        };
    }

    protected static Avalonia.Media.FontStyle ParseFontStyle(string value)
    {
        return value.ToLowerInvariant() switch
        {
            "italic" => Avalonia.Media.FontStyle.Italic,
            "oblique" => Avalonia.Media.FontStyle.Oblique,
            _ => Avalonia.Media.FontStyle.Normal
        };
    }

    protected static bool IsGlass(string style)
        => string.Equals(style, "glass", StringComparison.OrdinalIgnoreCase);
}
