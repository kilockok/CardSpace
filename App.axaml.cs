using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PersonalCardDemo.Config;
using PersonalCardDemo.Layout;
using PersonalCardDemo.Styles;
using PersonalCardDemo.ViewModels;
using PersonalCardDemo.Views;

namespace PersonalCardDemo;

public partial class App : Application
{
    private const string IconsFileName = "Icons.axaml";

    /// <summary>
    /// DI 容器，由 Program.Main 在 AfterSetup 中注入
    /// </summary>
    public IServiceProvider? Services { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        LoadExternalIcons();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && Services is not null)
        {
            var configService = Services.GetRequiredService<IConfigService>();
            var config = configService.Load();

            var viewModel = Services.GetRequiredService<MainViewModel>();
            viewModel.UpdateFromConfig(config);

            var layoutEngine = Services.GetRequiredService<ILayoutEngine>();
            var styleManager = Services.GetRequiredService<IStyleManager>();

            desktop.MainWindow = new MainWindow(configService, layoutEngine, styleManager, viewModel);
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// 从 exe 同级目录加载外部 Icons.axaml，解析为 ResourceDictionary 合并到应用资源
    /// </summary>
    private void LoadExternalIcons()
    {
        var iconsPath = ResolveIconsPath();
        if (iconsPath is null) return;

        try
        {
            var xaml = File.ReadAllText(iconsPath);
            var parsed = AvaloniaRuntimeXamlLoader.Parse<ResourceDictionary>(xaml);
            Resources.MergedDictionaries.Add(parsed);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Icons] 加载外部图标失败: {iconsPath} - {ex.Message}");
        }
    }

    /// <summary>
    /// 解析 Icons.axaml 路径，优先 exe 同目录，其次工作目录
    /// </summary>
    private static string? ResolveIconsPath()
    {
        var exePath = Path.Combine(AppContext.BaseDirectory, IconsFileName);
        if (File.Exists(exePath))
            return exePath;

        var cwdPath = Path.Combine(Directory.GetCurrentDirectory(), IconsFileName);
        if (File.Exists(cwdPath))
            return cwdPath;

        Console.Error.WriteLine($"[Icons] 未找到 {IconsFileName}，图标将不可用");
        return null;
    }
}
