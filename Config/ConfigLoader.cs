using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PersonalCardDemo.Config;

public static class ConfigLoader
{
    private const string FileName = "config.yaml";

    public static AppConfig Load()
    {
        var path = ResolvePath();

        if (!File.Exists(path))
        {
            var defaultConfig = CreateDefault();
            Save(defaultConfig, path);
            return defaultConfig;
        }

        var yaml = File.ReadAllText(path);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        try
        {
            return deserializer.Deserialize<AppConfig>(yaml) ?? CreateDefault();
        }
        catch (Exception)
        {
            // 配置文件格式错误时回退到默认配置
            return CreateDefault();
        }
    }

    private static void Save(AppConfig config, string path)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        var yaml = serializer.Serialize(config);
        File.WriteAllText(path, yaml);
    }

    private static string ResolvePath()
    {
        // 优先查找 exe 同目录
        var exeDir = AppContext.BaseDirectory;
        var exePath = Path.Combine(exeDir, FileName);
        if (File.Exists(exePath))
            return exePath;

        // 其次查找工作目录
        var cwdPath = Path.Combine(Directory.GetCurrentDirectory(), FileName);
        if (File.Exists(cwdPath))
            return cwdPath;

        // 都不存在时，在 exe 同目录创建
        return exePath;
    }

    private static AppConfig CreateDefault()
    {
        return new AppConfig
        {
            Style = "fluent",
            Theme = "light",
            Profile = new ProfileConfig
            {
                Name = "Kilock",
                Id = "@kilock_1208",
                AvatarFallback = "K",
                Signature = "清凤凤凤凤凤!",
                Tags = ["Dev", "INTP", "Mtx"]
            },
            Location = new LocationConfig
            {
                City = "浙江 · 宁波",
                Description = "qwqwqwq!"
            },
            Social = new SocialConfig
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
            Philosophy = new PhilosophyConfig
            {
                Title = "Our Philosophy",
                Quotes = ["活下去，去做更多、更优秀的作品"],
                Attribution = "以及陪着清凤"
            }
        };
    }
}
