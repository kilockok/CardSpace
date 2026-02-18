using System;
using Avalonia;
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
    /// <summary>
    /// DI 容器，由 Program.Main 在 AfterSetup 中注入
    /// </summary>
    public IServiceProvider? Services { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
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
}
