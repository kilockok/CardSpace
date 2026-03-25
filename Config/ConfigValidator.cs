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
        "light", "dark"
    };

    /// <summary>
    /// 校验并修正配置，返回修正后的配置（原地修改）
    /// </summary>
    public AppConfig Validate(AppConfig config)
    {
        // 根节点为 null 时直接返回全默认
        config ??= new AppConfig();

        ValidateStyleAndTheme(config);
        ValidateWindow(config.Window);
        ValidateLayout(config.Layout);
        ValidateComponents(config.Components);
        ValidateProfile(config.Profile);
        ValidatePhilosophy(config.Philosophy);

        return config;
    }

    private static void ValidateStyleAndTheme(AppConfig config)
    {
        if (!ValidStyles.Contains(config.Style))
            config.Style = "fluent";

        if (!ValidThemes.Contains(config.Theme))
            config.Theme = "light";
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
    }

    private static void ValidatePhilosophy(PhilosophyConfig philosophy)
    {
        if (string.IsNullOrWhiteSpace(philosophy.Title))
            philosophy.Title = "Our Philosophy";
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
