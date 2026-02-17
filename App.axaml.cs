using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using PersonalCardDemo.Config;
using PersonalCardDemo.ViewModels;
using PersonalCardDemo.Views;

namespace PersonalCardDemo;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var config = ConfigLoader.Load();
            var viewModel = new CardViewModel(config);

            var isGlass = string.Equals(config.Style, "glass", StringComparison.OrdinalIgnoreCase);

            if (isGlass)
            {
                // Glass 风格固定暗色
                RequestedThemeVariant = ThemeVariant.Dark;
                LoadStyleResource("avares://PersonalCardDemo/Styles/GlassStyle.axaml");
                desktop.MainWindow = new GlassWindow { DataContext = viewModel };
            }
            else
            {
                // Fluent 风格支持亮/暗
                var isDark = string.Equals(config.Theme, "dark", StringComparison.OrdinalIgnoreCase);
                RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;
                LoadStyleResource("avares://PersonalCardDemo/Styles/FluentStyle.axaml");
                desktop.MainWindow = new FluentWindow(isDark) { DataContext = viewModel };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void LoadStyleResource(string uri)
    {
        var resource = (ResourceDictionary)AvaloniaXamlLoader.Load(new Uri(uri));
        Resources.MergedDictionaries.Add(resource);
    }
}
