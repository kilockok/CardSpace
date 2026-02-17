using System.Collections.Generic;
using System.Linq;
using PersonalCardDemo.Config;

namespace PersonalCardDemo.ViewModels;

public sealed class CardViewModel
{
    public AppConfig Config { get; }

    public CardViewModel(AppConfig config)
    {
        Config = config;
    }

    // 个人信息
    public string Name => Config.Profile.Name;
    public string Id => Config.Profile.Id;
    public string AvatarFallback => Config.Profile.AvatarFallback;
    public string Signature => $"\u201C{Config.Profile.Signature}\u201D";
    public List<string> Tags => Config.Profile.Tags;

    // 地点
    public string City => Config.Location.City;
    public string LocationDescription => Config.Location.Description;

    // 社交
    public string XUrl => Config.Social.X;
    public string GitHubUrl => Config.Social.GitHub;

    // 技术栈
    public List<TechStackItem> TechStack => Config.TechStack;

    // 链接
    public List<LinkItem> Links => Config.Links;

    // 理念
    public string PhilosophyTitle => Config.Philosophy.Title;
    public string PhilosophyQuote => string.Join("\n", Config.Philosophy.Quotes);
    public string PhilosophyAttribution => Config.Philosophy.Attribution;
}
