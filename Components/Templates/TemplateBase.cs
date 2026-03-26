using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using PersonalCardDemo.Config.Models;
using PersonalCardDemo.Helpers;

namespace PersonalCardDemo.Components.Templates;

public abstract class TemplateBase
{
    private static readonly string[] SupportedImageExtensions =
    [
        ".png",
        ".jpg",
        ".jpeg",
        ".webp",
        ".bmp",
        ".gif",
        ".tiff",
        ".tif"
    ];

    protected static IBrush? TryGetBrush(string key) => ResourceHelper.FindBrush(key);
    protected static StreamGeometry? TryGetIcon(string key) => ResourceHelper.FindIcon(key);
    protected static SolidColorBrush? ParseBrush(string colorStr) => ResourceHelper.ParseBrush(colorStr);

    protected static ElementOverride? GetOverride(ComponentConfig config, string elementName)
    {
        if (config.Overrides == null) return null;
        return config.Overrides.GetValueOrDefault(elementName);
    }

    protected static void ApplyControlOverride(Control control, ElementOverride? ov)
    {
        if (ov == null) return;

        if (ov.Width.HasValue)
            control.Width = ov.Width.Value;

        if (ov.Height.HasValue)
            control.Height = ov.Height.Value;

        if (ov.Visible.HasValue)
            control.IsVisible = ov.Visible.Value;

        if (ov.Opacity.HasValue)
            control.Opacity = ov.Opacity.Value;

        if (ov.Margin != null)
            control.Margin = ParseThickness(ov.Margin);
    }

    protected static void ApplyTextOverride(TextBlock textBlock, ElementOverride? ov)
    {
        ApplyControlOverride(textBlock, ov);
        if (ov == null) return;

        if (ov.FontSize.HasValue)
            textBlock.FontSize = ov.FontSize.Value;

        if (ov.FontWeight != null)
            textBlock.FontWeight = ParseFontWeight(ov.FontWeight);

        if (ov.FontStyle != null)
            textBlock.FontStyle = ParseFontStyle(ov.FontStyle);

        if (ov.Foreground != null)
            textBlock.Foreground = ParseBrush(ov.Foreground);

        if (ov.Text != null)
            textBlock.Text = ov.Text;
    }

    protected static void ApplyBorderOverride(Border border, ElementOverride? ov)
    {
        ApplyControlOverride(border, ov);
        if (ov == null) return;

        if (ov.CornerRadius.HasValue)
            border.CornerRadius = new CornerRadius(ov.CornerRadius.Value);

        if (ov.Background != null)
            border.Background = ParseBrush(ov.Background);

        if (ov.Padding != null)
            border.Padding = ParseThickness(ov.Padding);
    }

    protected static void ApplyImageOverride(Image image, ElementOverride? ov)
    {
        ApplyControlOverride(image, ov);
        if (ov == null || string.IsNullOrWhiteSpace(ov.ImageSource))
            return;

        var overrideSource = LoadImage(ov.ImageSource);
        if (overrideSource != null)
            image.Source = overrideSource;
    }

    protected static Panel BuildFluentShell(string name, Control content, bool clipContent = false)
    {
        var root = new Panel { Name = name };

        var decorBorder = new Border
        {
            Classes = { "fluent-card" },
            IsHitTestVisible = false,
            Effect = new DropShadowEffect
            {
                OffsetX = 0,
                OffsetY = 2,
                BlurRadius = 8,
                Color = Colors.Black,
                Opacity = 0.06
            }
        };
        root.Children.Add(decorBorder);

        if (clipContent)
        {
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

    protected static Panel BuildGlassShell(string name, Control content, bool clipContent = false)
    {
        var root = new Panel
        {
            Name = name,
            Classes = { "glass-card" }
        };

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
            OffsetX = 0,
            OffsetY = 8,
            BlurRadius = 16,
            Color = Colors.Black,
            Opacity = 0.10
        };
        root.Children.Add(decorBorder);

        if (clipContent)
        {
            var clipBorder = new Border
            {
                CornerRadius = new CornerRadius(10),
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

    protected static IImage? LoadImage(string relativePath, string? fallbackAvares = null)
    {
        var fullPath = ResolveImagePath(relativePath);
        if (string.IsNullOrWhiteSpace(fullPath))
            return null;

        try
        {
            var bytes = File.ReadAllBytes(fullPath);
            using var stream = new MemoryStream(bytes);
            return new Bitmap(stream);
        }
        catch
        {
            System.Diagnostics.Debug.WriteLine($"[LoadImage] 文件损坏或格式不支持: {fullPath}");
            return null;
        }
    }

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

    private static string? ResolveImagePath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return null;

        var fullPath = Path.IsPathRooted(relativePath)
            ? relativePath
            : Path.Combine(AppContext.BaseDirectory, relativePath);

        var resolvedPath = FindCompatibleImagePath(fullPath);
        if (!string.IsNullOrWhiteSpace(resolvedPath))
            return resolvedPath;

        System.Diagnostics.Debug.WriteLine($"[LoadImage] 文件不存在: {fullPath}");
        return null;
    }

    private static string? FindCompatibleImagePath(string path)
    {
        if (File.Exists(path))
            return path;

        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            return null;

        var stem = Path.GetFileNameWithoutExtension(path);
        if (string.IsNullOrWhiteSpace(stem))
            return null;

        foreach (var extension in GetExtensionSearchOrder(Path.GetExtension(path)))
        {
            var candidate = FindCaseInsensitiveFile(directory, stem + extension);
            if (candidate == null)
                continue;

            System.Diagnostics.Debug.WriteLine($"[LoadImage] 使用兼容图片: {path} -> {candidate}");
            return candidate;
        }

        return null;
    }

    private static IEnumerable<string> GetExtensionSearchOrder(string configuredExtension)
    {
        if (!string.IsNullOrWhiteSpace(configuredExtension))
            yield return configuredExtension;

        foreach (var extension in SupportedImageExtensions)
        {
            if (string.Equals(extension, configuredExtension, StringComparison.OrdinalIgnoreCase))
                continue;

            yield return extension;
        }
    }

    private static string? FindCaseInsensitiveFile(string directory, string fileName)
    {
        foreach (var candidate in Directory.EnumerateFiles(directory))
        {
            if (string.Equals(Path.GetFileName(candidate), fileName, StringComparison.OrdinalIgnoreCase))
                return candidate;
        }

        return null;
    }

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
