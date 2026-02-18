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
    /// Fluent 卡片外壳
    /// 视觉层（背景/边框/圆角/阴影）和内容层分离，避免 CornerRadius 裁切 scale 动画
    /// clipContent=true 时内容层也裁切（地图等全出血图片需要）
    /// </summary>
    protected static Panel BuildFluentShell(string name, Control content, bool clipContent = false)
    {
        var root = new Panel { Name = name };

        // 底层：视觉装饰（背景、边框、圆角、阴影）
        var decorBorder = new Border
        {
            Classes = { "fluent-card" },
            IsHitTestVisible = false,
            Effect = new DropShadowEffect
            {
                OffsetX = 0, OffsetY = 2, BlurRadius = 8,
                Color = Colors.Black, Opacity = 0.06
            }
        };
        root.Children.Add(decorBorder);

        // 上层：内容，不受圆角裁切
        if (clipContent)
        {
            // 全出血内容需要裁切（如地图图片）
            var clipBorder = new Border
            {
                CornerRadius = new CornerRadius(8),
                ClipToBounds = true,
                Child = content
            };
            root.Children.Add(clipBorder);
        }
        else
        {
            root.Children.Add(content);
        }

        return root;
    }

    /// <summary>
    /// Glass 卡片外壳
    /// 视觉层（亚克力/背景/边框/圆角/阴影）和内容层分离，避免 CornerRadius 裁切 scale 动画
    /// </summary>
    protected static Panel BuildGlassShell(string name, Control content)
    {
        var root = new Panel
        {
            Name = name,
            Classes = { "glass-card" }
        };

        // 底层：亚克力 + 视觉装饰，用 ClipToBounds 裁切圆角溢出
        var decorBorder = new Border
        {
            CornerRadius = new CornerRadius(10),
            ClipToBounds = true,
            IsHitTestVisible = false
        };
        var decorPanel = new Panel();
        decorPanel.Children.Add(new ExperimentalAcrylicBorder
        {
            Material = new ExperimentalAcrylicMaterial
            {
                BackgroundSource = AcrylicBackgroundSource.Digger,
                TintColor = Color.Parse("#FFFFFF"),
                TintOpacity = 0.02,
                MaterialOpacity = 0.15
            }
        });
        decorPanel.Children.Add(new Border
        {
            BorderThickness = new Thickness(1),
            BorderBrush = TryGetBrush("GlassCardBorder"),
            Background = TryGetBrush("GlassCardGradient")
        });
        decorBorder.Child = decorPanel;
        decorBorder.Effect = new DropShadowEffect
        {
            OffsetX = 0, OffsetY = 8, BlurRadius = 16,
            Color = Colors.Black, Opacity = 0.10
        };
        root.Children.Add(decorBorder);

        // 上层：内容，不受圆角裁切
        root.Children.Add(content);

        return root;
    }

    // -- 资源加载 --

    /// <summary>
    /// 从文件系统加载图片，路径相对于 exe 所在目录
    /// 使用字节流加载，不锁定文件，支持热替换
    /// </summary>
    protected static IImage? LoadImage(string relativePath, string? fallbackAvares = null)
    {
        var basePath = AppContext.BaseDirectory;
        var fullPath = System.IO.Path.Combine(basePath, relativePath);

        if (System.IO.File.Exists(fullPath))
        {
            try
            {
                // 读取字节流后立即释放文件句柄，避免锁定文件
                var bytes = System.IO.File.ReadAllBytes(fullPath);
                using var stream = new System.IO.MemoryStream(bytes);
                return new Bitmap(stream);
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
