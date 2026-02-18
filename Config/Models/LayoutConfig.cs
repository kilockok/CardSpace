using YamlDotNet.Serialization;

namespace PersonalCardDemo.Config.Models;

/// <summary>
/// 布局配置 - 控制主内容区域的 Grid 参数
/// </summary>
public sealed class LayoutConfig
{
    /// <summary>
    /// Grid 列定义，如 "300,*"
    /// </summary>
    [YamlMember(Alias = "columns")]
    public string Columns { get; set; } = "300,*";

    /// <summary>
    /// Grid 行定义，如 "*,210"
    /// </summary>
    [YamlMember(Alias = "rows")]
    public string Rows { get; set; } = "*,210";

    /// <summary>
    /// 列间距（像素）
    /// </summary>
    [YamlMember(Alias = "column_spacing")]
    public double ColumnSpacing { get; set; } = 16;

    /// <summary>
    /// 行间距（像素）
    /// </summary>
    [YamlMember(Alias = "row_spacing")]
    public double RowSpacing { get; set; } = 16;

    /// <summary>
    /// 内容区域边距，格式 "left,top,right,bottom"
    /// </summary>
    [YamlMember(Alias = "margin")]
    public string Margin { get; set; } = "20,8,20,20";
}
