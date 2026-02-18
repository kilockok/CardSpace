using System;
using System.IO;
using System.Threading;
using Avalonia.Threading;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PersonalCardDemo.Config;

/// <summary>
/// 配置变更事件参数
/// </summary>
public sealed class ConfigChangedEventArgs : EventArgs
{
    public required AppConfig OldConfig { get; init; }
    public required AppConfig NewConfig { get; init; }
}

/// <summary>
/// 配置服务接口 - 负责加载、监听、通知配置变更
/// </summary>
public interface IConfigService : IDisposable
{
    /// <summary>
    /// 当前生效的配置
    /// </summary>
    AppConfig Current { get; }

    /// <summary>
    /// 配置文件的完整路径
    /// </summary>
    string ConfigPath { get; }

    /// <summary>
    /// 加载配置（首次启动时调用）
    /// </summary>
    AppConfig Load();

    /// <summary>
    /// 配置变更事件（热加载触发，已切换到 UI 线程）
    /// </summary>
    event EventHandler<ConfigChangedEventArgs>? ConfigChanged;

    /// <summary>
    /// 启动文件监听
    /// </summary>
    void StartWatching();

    /// <summary>
    /// 停止文件监听
    /// </summary>
    void StopWatching();
}

/// <summary>
/// 配置服务实现
///
/// 职责：
/// 1. 从 config.yaml 加载配置，解析失败时回退默认值
/// 2. 通过 FileSystemWatcher 监听文件变更
/// 3. 防抖处理避免编辑器多次写入触发重复加载
/// 4. 配置变更后在 UI 线程触发事件
/// </summary>
public sealed class ConfigService : IConfigService
{
    private const string FileName = "config.yaml";

    // 防抖延迟（毫秒），编辑器保存时可能触发多次写入事件
    private const int DebounceDelayMs = 300;

    private readonly ConfigValidator _validator;
    private readonly ConfigMigrator _migrator;
    private readonly IDeserializer _deserializer;
    private readonly ISerializer _serializer;

    private FileSystemWatcher? _watcher;
    private Timer? _debounceTimer;
    private readonly object _lock = new();

    public AppConfig Current { get; private set; } = new();
    public string ConfigPath { get; }

    public event EventHandler<ConfigChangedEventArgs>? ConfigChanged;

    public ConfigService(ConfigValidator validator, ConfigMigrator migrator)
    {
        _validator = validator;
        _migrator = migrator;

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        _serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        ConfigPath = ResolvePath();
    }

    public AppConfig Load()
    {
        AppConfig config;

        if (!File.Exists(ConfigPath))
        {
            // 配置文件不存在，生成默认配置并写入
            config = ConfigDefaults.Create();
            SaveToFile(config);
        }
        else
        {
            config = LoadFromFile();
        }

        // 迁移旧格式 -> 校验 -> 存储
        config = _migrator.Migrate(config);
        config = _validator.Validate(config);
        Current = config;

        return config;
    }

    public void StartWatching()
    {
        var dir = Path.GetDirectoryName(ConfigPath);
        if (string.IsNullOrEmpty(dir)) return;

        _watcher = new FileSystemWatcher(dir, FileName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
            EnableRaisingEvents = true
        };

        _watcher.Changed += OnFileChanged;
        _watcher.Created += OnFileChanged;
    }

    public void StopWatching()
    {
        if (_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Changed -= OnFileChanged;
            _watcher.Created -= OnFileChanged;
            _watcher.Dispose();
            _watcher = null;
        }
    }

    public void Dispose()
    {
        StopWatching();
        _debounceTimer?.Dispose();
    }

    /// <summary>
    /// 文件变更回调，使用防抖避免重复触发
    /// </summary>
    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        lock (_lock)
        {
            _debounceTimer?.Dispose();
            _debounceTimer = new Timer(OnDebounceElapsed, null, DebounceDelayMs, Timeout.Infinite);
        }
    }

    /// <summary>
    /// 防抖结束后执行实际的配置重载
    /// </summary>
    private void OnDebounceElapsed(object? state)
    {
        try
        {
            var newConfig = LoadFromFile();
            newConfig = _migrator.Migrate(newConfig);
            newConfig = _validator.Validate(newConfig);

            var oldConfig = Current;
            Current = newConfig;

            // 切换到 UI 线程触发事件
            Dispatcher.UIThread.Post(() =>
            {
                ConfigChanged?.Invoke(this, new ConfigChangedEventArgs
                {
                    OldConfig = oldConfig,
                    NewConfig = newConfig
                });
            });
        }
        catch (Exception)
        {
            // 热加载失败时静默忽略，保持当前配置不变
            // 避免用户编辑到一半导致程序崩溃
        }
    }

    /// <summary>
    /// 从文件加载配置，解析失败时返回默认配置
    /// </summary>
    private AppConfig LoadFromFile()
    {
        try
        {
            var yaml = File.ReadAllText(ConfigPath);
            return _deserializer.Deserialize<AppConfig>(yaml) ?? new AppConfig();
        }
        catch (Exception)
        {
            return new AppConfig();
        }
    }

    /// <summary>
    /// 将配置序列化写入文件
    /// </summary>
    private void SaveToFile(AppConfig config)
    {
        try
        {
            var yaml = _serializer.Serialize(config);
            File.WriteAllText(ConfigPath, yaml);
        }
        catch (Exception)
        {
            // 写入失败不影响程序运行
        }
    }

    /// <summary>
    /// 解析配置文件路径，优先 exe 同目录，其次工作目录
    /// </summary>
    private static string ResolvePath()
    {
        var exeDir = AppContext.BaseDirectory;
        var exePath = Path.Combine(exeDir, FileName);
        if (File.Exists(exePath))
            return exePath;

        var cwdPath = Path.Combine(Directory.GetCurrentDirectory(), FileName);
        if (File.Exists(cwdPath))
            return cwdPath;

        return exePath;
    }
}

/// <summary>
/// 默认配置工厂 - 集中管理默认配置值
/// </summary>
public static class ConfigDefaults
{
    public static AppConfig Create()
    {
        return new AppConfig
        {
            Style = "fluent",
            Theme = "light",
            Window = new Models.WindowConfig
            {
                Width = 960,
                Height = 680,
                CanResize = false,
                Title = "个人主页"
            },
            Layout = new Models.LayoutConfig
            {
                Columns = "300,*",
                Rows = "*,210",
                ColumnSpacing = 16,
                RowSpacing = 16,
                Margin = "20,8,20,20"
            },
            Profile = new Models.ProfileConfig
            {
                Name = "Kilock",
                Id = "@kilock_1208",
                AvatarFallback = "K",
                Signature = "清凤凤凤凤凤!",
                Tags = ["Dev", "INTP", "Mtx"]
            },
            Location = new Models.LocationConfig
            {
                City = "浙江 · 宁波",
                Description = "qwqwqwq!"
            },
            Social = new Models.SocialConfig
            {
                X = "https://x.com/kilock_1208",
                GitHub = "https://github.com/kilockok"
            },
            TechStack =
            [
                new() { Name = "C++", Icon = "CppIcon", Color = "#00599C" },
                new() { Name = "Python", Icon = "PythonIcon", Color = "#3776AB" },
                new() { Name = "HTML5", Icon = "Html5Icon", Color = "#E34F26" },
                new() { Name = "Vue", Icon = "VueIcon", Color = "#42B883" },
                new() { Name = "Node.js", Icon = "NodeJsIcon", Color = "#339933" },
                new() { Name = "Docker", Icon = "DockerIcon", Color = "#2496ED" }
            ],
            Links =
            [
                new() { Icon = "GitHubIcon", Text = "github.com/kilockok" },
                new() { Icon = "BlogIcon", Text = "清凤.fun" }
            ],
            Philosophy = new Models.PhilosophyConfig
            {
                Title = "Our Philosophy",
                Quotes = ["活下去，去做更多、更优秀的作品"],
                Attribution = "以及陪着清凤"
            }
        };
    }
}
