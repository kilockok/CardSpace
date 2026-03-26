using System;
using System.Collections.Generic;
using PersonalCardDemo.Config.Models;

namespace PersonalCardDemo.Config;

/// <summary>
/// 配置校验器 - 对加载的配置进行合法性检查，非法值回退到默认值
/// 设计原则：宁可使用默认值也不让程序崩溃
/// </summary>
public sealed class ConfigValidator
{
    // 允许的风格值
    private static readonly HashSet<string> ValidStyles = new(StringComparer.OrdinalIgnoreCase)
    {
        "fluent", "glass"
    };

    // 允许的主题值
    private static readonly HashSet<string> ValidThemes = new(StringComparer.OrdinalIgnoreCase)
    {
        "light", "dark", "black"
    };

    /// <summary>
    /// 校验并修正配置，返回修正后的配置（原地修改）
    /// </summary>
    public AppConfig Validate(AppConfig config)
    {
        // 根节点为 null 时直接返回全默认
        config ??= new AppConfig();

        config.Window ??= new WindowConfig();
        config.Layout ??= new LayoutConfig();
        config.Profile ??= new ProfileConfig();
        config.Location ??= new LocationConfig();
        config.Social ??= new SocialConfig();
        config.TechStack ??= [];
        config.Links ??= [];
        config.Philosophy ??= new PhilosophyConfig();
        config.Components ??= [];

        ValidateStyleAndTheme(config);
        ValidateWindow(config.Window);
        ValidateLayout(config.Layout);
        if (config.LayoutGlass != null)
            ValidateLayout(config.LayoutGlass);
        ValidateComponents(config.Components);
        ValidateProfile(config.Profile);
        ValidateLocation(config.Location);
        ValidateSocial(config.Social);
        ValidateTechStack(config.TechStack);
        ValidateLinks(config.Links);
        ValidatePhilosophy(config.Philosophy);

        return config;
    }

    private static void ValidateStyleAndTheme(AppConfig config)
    {
        if (!ValidStyles.Contains(config.Style))
            config.Style = "fluent";

        config.Theme = NormalizeTheme(config.Theme);
    }

    private static string NormalizeTheme(string? theme)
    {
        if (string.IsNullOrWhiteSpace(theme) || !ValidThemes.Contains(theme))
            return "light";

        return string.Equals(theme, "black", StringComparison.OrdinalIgnoreCase)
            ? "dark"
            : theme.ToLowerInvariant();
    }

    private static void ValidateWindow(WindowConfig window)
    {
        // 窗口尺寸限制在合理范围
        window.Width = Clamp(window.Width, 400, 4000, 960);
        window.Height = Clamp(window.Height, 300, 4000, 680);

        if (string.IsNullOrWhiteSpace(window.Title))
            window.Title = "个人主页";
    }

    private static void ValidateLayout(LayoutConfig layout)
    {
        if (string.IsNullOrWhiteSpace(layout.Columns))
            layout.Columns = "300,*";

        if (string.IsNullOrWhiteSpace(layout.Rows))
            layout.Rows = "*,210";

        layout.ColumnSpacing = ClampDouble(layout.ColumnSpacing, 0, 100, 16);
        layout.RowSpacing = ClampDouble(layout.RowSpacing, 0, 100, 16);

        if (string.IsNullOrWhiteSpace(layout.Margin))
            layout.Margin = "20,8,20,20";
    }

    private static void ValidateComponents(List<ComponentConfig> components)
    {
        if (components == null) return;

        foreach (var comp in components)
        {
            comp.Name ??= string.Empty;

            if (string.IsNullOrWhiteSpace(comp.Type))
                comp.Type = comp.Name;

            comp.GridRow = Math.Max(0, comp.GridRow);
            comp.GridColumn = Math.Max(0, comp.GridColumn);
            comp.GridRowSpan = Math.Max(1, comp.GridRowSpan);
            comp.GridColumnSpan = Math.Max(1, comp.GridColumnSpan);
            comp.ZIndex = Math.Max(0, comp.ZIndex);
            comp.SubColumnSpacing = ClampDouble(comp.SubColumnSpacing, 0, 100, 16);
        }
    }

    private static void ValidateProfile(ProfileConfig profile)
    {
        if (string.IsNullOrWhiteSpace(profile.Name))
            profile.Name = "User";

        profile.Id ??= string.Empty;
        profile.Signature ??= string.Empty;
        profile.Tags ??= [];

        if (string.IsNullOrWhiteSpace(profile.CoverImage))
            profile.CoverImage = "Assets/cover.jpg";

        if (string.IsNullOrWhiteSpace(profile.AvatarImage))
            profile.AvatarImage = "Assets/avatar.png";
    }

    private static void ValidateLocation(LocationConfig location)
    {
        location.City ??= string.Empty;
        location.Description ??= string.Empty;

        if (string.IsNullOrWhiteSpace(location.MapImage))
            location.MapImage = "Assets/map.png";
    }

    private static void ValidateSocial(SocialConfig social)
    {
        social.X ??= string.Empty;
        social.GitHub ??= string.Empty;
    }

    private static void ValidateTechStack(List<TechStackItem> techStack)
    {
        if (techStack == null) return;

        foreach (var item in techStack)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                item.Name = "Tech";

            if (string.IsNullOrWhiteSpace(item.Icon))
                item.Icon = "CodeIcon";

            if (string.IsNullOrWhiteSpace(item.Color))
                item.Color = "#808080";
        }
    }

    private static void ValidateLinks(List<LinkItem> links)
    {
        if (links == null) return;

        foreach (var item in links)
        {
            if (string.IsNullOrWhiteSpace(item.Icon))
                item.Icon = "CodeIcon";

            item.Text ??= string.Empty;
        }
    }

    private static void ValidatePhilosophy(PhilosophyConfig philosophy)
    {
        if (string.IsNullOrWhiteSpace(philosophy.Title))
            philosophy.Title = "Our Philosophy";

        philosophy.Quotes ??= [];
        philosophy.Attribution ??= string.Empty;
    }

    /// <summary>
    /// 整数范围钳制，超出范围时使用 fallback
    /// </summary>
    private static int Clamp(int value, int min, int max, int fallback)
    {
        if (value < min || value > max)
            return fallback;
        return value;
    }

    /// <summary>
    /// 浮点范围钳制
    /// </summary>
    private static double ClampDouble(double value, double min, double max, double fallback)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < min || value > max)
            return fallback;
        return value;
    }
}
