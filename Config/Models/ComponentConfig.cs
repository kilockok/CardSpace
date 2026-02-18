using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace PersonalCardDemo.Config.Models;

/// <summary>
/// 组件配置 - 描述一个卡片组件的布局和属性
/// </summary>
public sealed class ComponentConfig
{
    /// <summary>
    /// 组件实例名称（唯一标识）
    /// </summary>
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 组件模板类型，对应 IComponentTemplate.TypeName
    /// </summary>
    [YamlMember(Alias = "type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Grid 行位置
    /// </summary>
    [YamlMember(Alias = "grid_row")]
    public int GridRow { get; set; }

    /// <summary>
    /// Grid 列位置
    /// </summary>
    [YamlMember(Alias = "grid_column")]
    public int GridColumn { get; set; }

    /// <summary>
    /// Grid 行跨度
    /// </summary>
    [YamlMember(Alias = "grid_row_span")]
    public int GridRowSpan { get; set; } = 1;

    /// <summary>
    /// Grid 列跨度
    /// </summary>
    [YamlMember(Alias = "grid_column_span")]
    public int GridColumnSpan { get; set; } = 1;

    /// <summary>
    /// 是否可见
    /// </summary>
    [YamlMember(Alias = "visible")]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// 层级（越大越靠前）
    /// </summary>
    [YamlMember(Alias = "z_index")]
    public int ZIndex { get; set; }

    /// <summary>
    /// 子布局列定义（用于同一 Grid 单元格内的多组件排列）
    /// </summary>
    [YamlMember(Alias = "sub_columns")]
    public string? SubColumns { get; set; }

    /// <summary>
    /// 子布局列间距
    /// </summary>
    [YamlMember(Alias = "sub_column_spacing")]
    public double SubColumnSpacing { get; set; } = 16;

    /// <summary>
    /// 在子布局中的列位置
    /// </summary>
    [YamlMember(Alias = "sub_column")]
    public int? SubColumn { get; set; }

    /// <summary>
    /// 元素属性覆写表，key 为元素标识名
    /// </summary>
    [YamlMember(Alias = "overrides")]
    public Dictionary<string, ElementOverride>? Overrides { get; set; }
}
