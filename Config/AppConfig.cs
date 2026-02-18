using System.Collections.Generic;
using PersonalCardDemo.Config.Models;
using YamlDotNet.Serialization;

namespace PersonalCardDemo.Config;

/// <summary>
/// 应用根配置 - 统一管理所有配置节点
/// 同时保留旧字段（profile/location 等）确保向后兼容
/// </summary>
public sealed class AppConfig
{
    // === 风格与主题 ===

    [YamlMember(Alias = "style")]
    public string Style { get; set; } = "fluent";

    [YamlMember(Alias = "theme")]
    public string Theme { get; set; } = "light";

    // === 新增：窗口配置 ===

    [YamlMember(Alias = "window")]
    public WindowConfig Window { get; set; } = new();

    // === 布局配置（按风格区分） ===

    /// <summary>
    /// 默认布局（Fluent 风格使用）
    /// </summary>
    [YamlMember(Alias = "layout")]
    public LayoutConfig Layout { get; set; } = new();

    /// <summary>
    /// Glass 风格专用布局覆写，缺失时回退到 Layout
    /// </summary>
    [YamlMember(Alias = "layout_glass")]
    public LayoutConfig? LayoutGlass { get; set; }

    // === 新增：组件声明列表 ===

    [YamlMember(Alias = "components")]
    public List<ComponentConfig> Components { get; set; } = [];

    // === 数据源（保留旧结构，向后兼容）===

    [YamlMember(Alias = "profile")]
    public ProfileConfig Profile { get; set; } = new();

    [YamlMember(Alias = "location")]
    public LocationConfig Location { get; set; } = new();

    [YamlMember(Alias = "social")]
    public SocialConfig Social { get; set; } = new();

    [YamlMember(Alias = "tech_stack")]
    public List<TechStackItem> TechStack { get; set; } = [];

    [YamlMember(Alias = "links")]
    public List<LinkItem> Links { get; set; } = [];

    [YamlMember(Alias = "philosophy")]
    public PhilosophyConfig Philosophy { get; set; } = new();

    /// <summary>
    /// 根据当前风格返回对应的布局配置
    /// Glass 风格优先使用 layout_glass，缺失时回退到 layout
    /// </summary>
    public LayoutConfig GetLayoutForStyle(string style)
    {
        if (string.Equals(style, "glass", System.StringComparison.OrdinalIgnoreCase)
            && LayoutGlass != null)
            return LayoutGlass;

        return Layout;
    }
}
