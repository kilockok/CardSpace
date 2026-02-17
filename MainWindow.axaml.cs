using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;

namespace PersonalCardDemo;

public partial class MainWindow : Window
{
    private bool _isDarkTheme;

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        await PlayEntranceAsync();
    }

    // 利用 Transition 属性实现入场动画，只需设置目标值
    private async Task PlayEntranceAsync()
    {
        var cards = new Border[] { ProfileCard, MapCard, TechCard, PhilosophyCard };

        // 初始状态：透明 + 下移
        foreach (var card in cards)
        {
            card.Opacity = 0;
            card.RenderTransform = new TranslateTransform(0, 20);
        }

        // 等待布局完成
        await Task.Delay(80);

        // 依次触发，Transition 会自动平滑过渡
        foreach (var card in cards)
        {
            card.Opacity = 1;
            card.RenderTransform = new TranslateTransform(0, 0);
            await Task.Delay(70);
        }
    }

    // 窗口任意位置拖拽移动
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }

    // 主题切换
    private void ThemeToggle_Click(object? sender, RoutedEventArgs e)
    {
        _isDarkTheme = !_isDarkTheme;

        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = _isDarkTheme
                ? ThemeVariant.Dark
                : ThemeVariant.Light;
        }

        UpdateThemeIcon();
    }

    private void UpdateThemeIcon()
    {
        if (ThemeToggleIcon is null) return;

        var key = _isDarkTheme ? "SunIcon" : "MoonIcon";
        if (this.TryFindResource(key, out var resource) && resource is StreamGeometry geometry)
        {
            ThemeToggleIcon.Data = geometry;
        }
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void MinimizeButton_Click(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void XButton_Click(object? sender, RoutedEventArgs e)
    {
        OpenUrl("https://x.com/kilock_1208");
    }

    private void GitHubButton_Click(object? sender, RoutedEventArgs e)
    {
        OpenUrl("https://github.com/kilockok");
    }

    private static void OpenUrl(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }
        else
        {
            Process.Start("xdg-open", url);
        }
    }
}
