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
using PersonalCardDemo.ViewModels;

namespace PersonalCardDemo.Views;

public partial class FluentWindow : Window
{
    private bool _isDarkTheme;

    public FluentWindow(bool isDark)
    {
        _isDarkTheme = isDark;
        InitializeComponent();
    }

    // 需要无参构造供 AXAML 设计器使用
    public FluentWindow() : this(false) { }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        UpdateThemeIcon();
        await PlayEntranceAsync();
    }

    private async Task PlayEntranceAsync()
    {
        var cards = new Border[] { ProfileCard, MapCard, TechCard, PhilosophyCard };

        foreach (var card in cards)
        {
            card.Opacity = 0;
            card.RenderTransform = new TranslateTransform(0, 20);
        }

        await Task.Delay(80);

        foreach (var card in cards)
        {
            card.Opacity = 1;
            card.RenderTransform = new TranslateTransform(0, 0);
            await Task.Delay(70);
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }

    private void ThemeToggle_Click(object? sender, RoutedEventArgs e)
    {
        _isDarkTheme = !_isDarkTheme;
        if (Application.Current is { } app)
            app.RequestedThemeVariant = _isDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
        UpdateThemeIcon();
    }

    private void UpdateThemeIcon()
    {
        if (ThemeToggleIcon is null) return;
        var key = _isDarkTheme ? "SunIcon" : "MoonIcon";
        if (this.TryFindResource(key, out var resource) && resource is StreamGeometry geometry)
            ThemeToggleIcon.Data = geometry;
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e) => Close();

    private void MinimizeButton_Click(object? sender, RoutedEventArgs e)
        => WindowState = WindowState.Minimized;

    private void MaximizeButton_Click(object? sender, RoutedEventArgs e)
        => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void XButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is CardViewModel vm)
            OpenUrl(vm.XUrl);
    }

    private void GitHubButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is CardViewModel vm)
            OpenUrl(vm.GitHubUrl);
    }

    private static void OpenUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            Process.Start("open", url);
        else
            Process.Start("xdg-open", url);
    }
}
