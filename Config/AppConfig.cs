using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace PersonalCardDemo.Config;

public sealed class AppConfig
{
    [YamlMember(Alias = "style")]
    public string Style { get; set; } = "fluent";

    [YamlMember(Alias = "theme")]
    public string Theme { get; set; } = "light";

    [YamlMember(Alias = "profile")]
    public ProfileConfig Profile { get; set; } = new();

    [YamlMember(Alias = "location")]
    public LocationConfig Location { get; set; } = new();

    [YamlMember(Alias = "social")]
    public SocialConfig Social { get; set; } = new();

    [YamlMember(Alias = "tech_stack")]
    public List<TechStackItem> TechStack { get; set; } = new();

    [YamlMember(Alias = "links")]
    public List<LinkItem> Links { get; set; } = new();

    [YamlMember(Alias = "philosophy")]
    public PhilosophyConfig Philosophy { get; set; } = new();
}

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
    public List<string> Tags { get; set; } = new();
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
    public List<string> Quotes { get; set; } = new();

    [YamlMember(Alias = "attribution")]
    public string Attribution { get; set; } = string.Empty;
}
