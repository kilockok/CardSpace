using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PersonalCardDemo.Config;
using PersonalCardDemo.Config.Models;

namespace PersonalCardDemo.ViewModels;

/// <summary>
/// 主 ViewModel - 替代旧的 CardViewModel
/// 所有属性支持变更通知，配合热加载实时刷新 UI
/// </summary>
public sealed class MainViewModel : ViewModelBase
{
    // === 个人信息 ===

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        private set => SetField(ref _name, value);
    }

    private string _id = string.Empty;
    public string Id
    {
        get => _id;
        private set => SetField(ref _id, value);
    }

    private string _avatarFallback = string.Empty;
    public string AvatarFallback
    {
        get => _avatarFallback;
        private set => SetField(ref _avatarFallback, value);
    }

    private string _signature = string.Empty;
    public string Signature
    {
        get => _signature;
        private set => SetField(ref _signature, value);
    }

    private ObservableCollection<string> _tags = [];
    public ObservableCollection<string> Tags
    {
        get => _tags;
        private set => SetField(ref _tags, value);
    }

    // === 地点 ===

    private string _city = string.Empty;
    public string City
    {
        get => _city;
        private set => SetField(ref _city, value);
    }

    private string _locationDescription = string.Empty;
    public string LocationDescription
    {
        get => _locationDescription;
        private set => SetField(ref _locationDescription, value);
    }

    // === 社交 ===

    private string _xUrl = string.Empty;
    public string XUrl
    {
        get => _xUrl;
        private set => SetField(ref _xUrl, value);
    }

    private string _gitHubUrl = string.Empty;
    public string GitHubUrl
    {
        get => _gitHubUrl;
        private set => SetField(ref _gitHubUrl, value);
    }

    // === 技术栈 ===

    private ObservableCollection<TechStackItem> _techStack = [];
    public ObservableCollection<TechStackItem> TechStack
    {
        get => _techStack;
        private set => SetField(ref _techStack, value);
    }

    // === 链接 ===

    private ObservableCollection<LinkItem> _links = [];
    public ObservableCollection<LinkItem> Links
    {
        get => _links;
        private set => SetField(ref _links, value);
    }

    // === 理念 ===

    private string _philosophyTitle = string.Empty;
    public string PhilosophyTitle
    {
        get => _philosophyTitle;
        private set => SetField(ref _philosophyTitle, value);
    }

    private string _philosophyQuote = string.Empty;
    public string PhilosophyQuote
    {
        get => _philosophyQuote;
        private set => SetField(ref _philosophyQuote, value);
    }

    private string _philosophyAttribution = string.Empty;
    public string PhilosophyAttribution
    {
        get => _philosophyAttribution;
        private set => SetField(ref _philosophyAttribution, value);
    }

    /// <summary>
    /// 从配置更新所有属性（热加载时调用）
    /// </summary>
    public void UpdateFromConfig(AppConfig config)
    {
        Name = config.Profile.Name;
        Id = config.Profile.Id;
        AvatarFallback = config.Profile.AvatarFallback;
        Signature = $"\u201C{config.Profile.Signature}\u201D";
        Tags = new ObservableCollection<string>(config.Profile.Tags);

        City = config.Location.City;
        LocationDescription = config.Location.Description;

        XUrl = config.Social.X;
        GitHubUrl = config.Social.GitHub;

        TechStack = new ObservableCollection<TechStackItem>(config.TechStack);
        Links = new ObservableCollection<LinkItem>(config.Links);

        PhilosophyTitle = config.Philosophy.Title;
        PhilosophyQuote = string.Join("\n", config.Philosophy.Quotes);
        PhilosophyAttribution = config.Philosophy.Attribution;
    }
}
