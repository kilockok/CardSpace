using YamlDotNet.Serialization;

namespace PersonalCardDemo.Config.Models;

/// <summary>
/// 元素属性覆写 - 控制卡片内部元素的外观属性
/// 所有字段可选，null 表示使用模板默认值
/// </summary>
public sealed class ElementOverride
{
    [YamlMember(Alias = "font_size")]
    public double? FontSize { get; set; }

    [YamlMember(Alias = "font_weight")]
    public string? FontWeight { get; set; }

    [YamlMember(Alias = "font_style")]
    public string? FontStyle { get; set; }

    [YamlMember(Alias = "foreground")]
    public string? Foreground { get; set; }

    [YamlMember(Alias = "background")]
    public string? Background { get; set; }

    [YamlMember(Alias = "width")]
    public double? Width { get; set; }

    [YamlMember(Alias = "height")]
    public double? Height { get; set; }

    [YamlMember(Alias = "visible")]
    public bool? Visible { get; set; }

    [YamlMember(Alias = "margin")]
    public string? Margin { get; set; }

    [YamlMember(Alias = "padding")]
    public string? Padding { get; set; }

    [YamlMember(Alias = "corner_radius")]
    public double? CornerRadius { get; set; }

    [YamlMember(Alias = "opacity")]
    public double? Opacity { get; set; }

    [YamlMember(Alias = "text")]
    public string? Text { get; set; }

    [YamlMember(Alias = "image_source")]
    public string? ImageSource { get; set; }
}
