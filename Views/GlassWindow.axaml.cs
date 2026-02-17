using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using PersonalCardDemo.ViewModels;

namespace PersonalCardDemo.Views;

public partial class GlassWindow : Window
{
    public GlassWindow()
    {
        InitializeComponent();
    }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        await PlayEntranceAsync();
    }

    private async Task PlayEntranceAsync()
    {
        var cards = new Border[] { ProfileCard, MapCard, TechCard, PhilosophyCard };

        foreach (var card in cards)
        {
            card.Opacity = 0;
            card.RenderTransform = new TranslateTransform(0, 24);
        }

        await Task.Delay(100);

        foreach (var card in cards)
        {
            card.Opacity = 1;
            card.RenderTransform = new TranslateTransform(0, 0);
            await Task.Delay(80);
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
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
