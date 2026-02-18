using YamlDotNet.Serialization;

namespace PersonalCardDemo.Config.Models;

/// <summary>
/// 窗口配置 - 控制窗口尺寸、标题等属性
/// </summary>
public sealed class WindowConfig
{
    [YamlMember(Alias = "width")]
    public int Width { get; set; } = 960;

    [YamlMember(Alias = "height")]
    public int Height { get; set; } = 680;

    [YamlMember(Alias = "can_resize")]
    public bool CanResize { get; set; } = false;

    [YamlMember(Alias = "title")]
    public string Title { get; set; } = "个人主页";
}
