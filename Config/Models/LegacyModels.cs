using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace PersonalCardDemo.Config.Models;

// 保留旧配置模型，确保向后兼容
// 这些模型对应 config.yaml 中的数据源节点

public sealed class ProfileConfig
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = "Kilock";

    [YamlMember(Alias = "id")]
    public string Id { get; set; } = "@kilock_1208";

    [YamlMember(Alias = "avatar_fallback")]
    public string AvatarFallback { get; set; } = "K";

    [YamlMember(Alias = "signature")]
    public string Signature { get; set; } = string.Empty;

    [YamlMember(Alias = "tags")]
    public List<string> Tags { get; set; } = [];
}

public sealed class LocationConfig
{
    [YamlMember(Alias = "city")]
    public string City { get; set; } = string.Empty;

    [YamlMember(Alias = "description")]
    public string Description { get; set; } = string.Empty;
}

public sealed class SocialConfig
{
    [YamlMember(Alias = "x")]
    public string X { get; set; } = string.Empty;

    [YamlMember(Alias = "github")]
    public string GitHub { get; set; } = string.Empty;
}

public sealed class TechStackItem
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "icon")]
    public string Icon { get; set; } = string.Empty;

    [YamlMember(Alias = "color")]
    public string Color { get; set; } = "#808080";
}

public sealed class LinkItem
{
    [YamlMember(Alias = "icon")]
    public string Icon { get; set; } = string.Empty;

    [YamlMember(Alias = "text")]
    public string Text { get; set; } = string.Empty;
}

public sealed class PhilosophyConfig
{
    [YamlMember(Alias = "title")]
    public string Title { get; set; } = "Our Philosophy";

    [YamlMember(Alias = "quotes")]
    public List<string> Quotes { get; set; } = [];

    [YamlMember(Alias = "attribution")]
    public string Attribution { get; set; } = string.Empty;
}
