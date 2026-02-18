using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using PersonalCardDemo.Config;
using PersonalCardDemo.Helpers;
using PersonalCardDemo.Layout;
using PersonalCardDemo.Styles;
using PersonalCardDemo.ViewModels;

namespace PersonalCardDemo.Views;

/// <summary>
/// 统一主窗口 - 合并原 FluentWindow 和 GlassWindow
///
/// 职责：
/// 1. 根据风格动态构建标题栏（macOS 红绿灯 / Windows 按钮）
/// 2. 通过 LayoutEngine 动态构建内容区域
/// 3. 响应配置热加载事件，刷新 UI
/// 4. 管理入场动画
/// </summary>
public partial class MainWindow : Window
{
    private readonly IConfigService? _configService;
    private readonly ILayoutEngine? _layoutEngine;
    private readonly IStyleManager? _styleManager;
    private readonly MainViewModel? _viewModel;

    private PathIcon? _themeToggleIcon;

    public MainWindow(
        IConfigService configService,
        ILayoutEngine layoutEngine,
        IStyleManager styleManager,
        MainViewModel viewModel)
    {
        _configService = configService;
        _layoutEngine = layoutEngine;
        _styleManager = styleManager;
        _viewModel = viewModel;

        InitializeComponent();
        DataContext = _viewModel;
    }

    // 设计器用，不注入任何依赖
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        // 设计器模式下跳过运行时初始化
        if (_configService is null || _styleManager is null) return;

        var config = _configService.Current;
        ApplyWindowConfig(config);
        _styleManager.ApplyStyle(config.Style, config.Theme);

        BuildTitleBar();
        ApplyRootStyle();
        BuildContent();

        await PlayEntranceAsync();

        _configService.ConfigChanged += OnConfigChanged;
        _configService.StartWatching();
    }

    protected override void OnClosed(EventArgs e)
    {
        if (_configService is not null)
        {
            _configService.ConfigChanged -= OnConfigChanged;
            _configService.StopWatching();
        }
        base.OnClosed(e);
    }

    // === 热加载回调 ===

    private async void OnConfigChanged(object? sender, ConfigChangedEventArgs e)
    {
        _viewModel!.UpdateFromConfig(e.NewConfig);
        ApplyWindowConfig(e.NewConfig);

        var styleChanged = !string.Equals(e.OldConfig.Style, e.NewConfig.Style, StringComparison.OrdinalIgnoreCase);
        var themeChanged = !string.Equals(e.OldConfig.Theme, e.NewConfig.Theme, StringComparison.OrdinalIgnoreCase);

        if (styleChanged || themeChanged)
            _styleManager!.ApplyStyle(e.NewConfig.Style, e.NewConfig.Theme);

        if (styleChanged)
        {
            BuildTitleBar();
            ApplyRootStyle();
        }

        BuildContent();

        if (styleChanged)
            await PlayEntranceAsync();
    }

    // === 窗口配置 ===

    private void ApplyWindowConfig(AppConfig config)
    {
        Width = config.Window.Width;
        Height = config.Window.Height;
        CanResize = config.Window.CanResize;
        Title = config.Window.Title;
    }

    // === 根容器样式 ===

    private void ApplyRootStyle()
    {
        var glass = IsGlass();

        if (glass)
        {
            RootBorder.Background = Brushes.Transparent;
            RootBorder.BorderBrush = null;
            RootBorder.BorderThickness = new Thickness(0);
            RootBorder.CornerRadius = new CornerRadius(16);
            TransparencyLevelHint = new[] { WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.Transparent };
        }
        else
        {
            RootBorder.CornerRadius = new CornerRadius(8);
            RootBorder.BorderThickness = new Thickness(1);
            TransparencyLevelHint = new[] { WindowTransparencyLevel.None };
            RootBorder.Background = ResourceHelper.FindBrush("WindowBackground") ?? Brushes.White;
            RootBorder.BorderBrush = ResourceHelper.FindBrush("WindowBorder");
        }
    }

    // === 标题栏 ===

    private void BuildTitleBar()
    {
        if (IsGlass())
            BuildGlassTitleBar();
        else
            BuildFluentTitleBar();
    }

    private void BuildGlassTitleBar()
    {
        var rootGrid = (Grid)RootBorder.Child!;

        // 清除旧的 Glass 背景层
        RemoveGlassBackgroundLayers(rootGrid);

        // 添加 Glass 背景
        var bgBorder = new Border
        {
            Name = "GlassBg",
            IsHitTestVisible = false,
            Background = ResourceHelper.FindBrush("GlassBackgroundGradient")
        };
        Grid.SetRowSpan(bgBorder, 2);
        rootGrid.Children.Insert(0, bgBorder);

        // 光晕
        var glow = new Avalonia.Controls.Shapes.Ellipse
        {
            Width = 500, Height = 260, Opacity = 0.15,
            IsHitTestVisible = false,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0, -80, 0, 0),
            Fill = new RadialGradientBrush
            {
                GradientStops =
                {
                    new GradientStop(Color.Parse("#FFE8A849"), 0),
                    new GradientStop(Color.Parse("#00E8A849"), 1)
                }
            },
            Name = "GlassGlow"
        };
        Grid.SetRowSpan(glow, 2);
        rootGrid.Children.Insert(1, glow);

        // 标题栏内容
        var border = new Border { Background = Brushes.Transparent, Height = 40 };
        var grid = new Grid();

        var trafficGroup = new StackPanel
        {
            Classes = { "traffic-group" },
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Margin = new Thickness(14, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left
        };

        trafficGroup.Children.Add(CreateTrafficButton("traffic-close", "\u00d7", (_, _) => Close()));
        trafficGroup.Children.Add(CreateTrafficButton("traffic-minimize", "\u2013",
            (_, _) => WindowState = WindowState.Minimized));
        trafficGroup.Children.Add(CreateTrafficButton("traffic-maximize", "+",
            (_, _) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized));

        grid.Children.Add(trafficGroup);

        grid.Children.Add(new TextBlock
        {
            Text = _configService!.Current.Window.Title,
            FontSize = 13,
            FontWeight = FontWeight.Medium,
            Foreground = ResourceHelper.FindBrush("GlassTitleForeground") ?? Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        });

        border.Child = grid;
        TitleBarHost.Content = border;
    }

    private void BuildFluentTitleBar()
    {
        var rootGrid = (Grid)RootBorder.Child!;
        RemoveGlassBackgroundLayers(rootGrid);

        var border = new Border
        {
            Height = 32,
            Background = ResourceHelper.FindBrush("TitleBarBackground"),
            BorderBrush = ResourceHelper.FindBrush("TitleBarBorder"),
            BorderThickness = new Thickness(0, 0, 0, 1)
        };

        var grid = new Grid();

        // 左侧 Logo
        var leftStack = new StackPanel
        {
            Orientation = Orientation.Horizontal, Spacing = 10,
            Margin = new Thickness(16, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left
        };

        var logoBorder = new Border
        {
            Width = 18, Height = 18, CornerRadius = new CornerRadius(4),
            Background = ResourceHelper.FindBrush("LogoGradientBrush"),
            Child = new TextBlock
            {
                Text = "C", FontSize = 10, FontWeight = FontWeight.Bold,
                Foreground = ResourceHelper.FindBrush("TextOnAccent") ?? Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        leftStack.Children.Add(logoBorder);

        leftStack.Children.Add(new TextBlock
        {
            Text = "CardSpace", FontSize = 12, FontWeight = FontWeight.SemiBold,
            Foreground = ResourceHelper.FindBrush("TitleBarForeground") ?? Brushes.Black,
            VerticalAlignment = VerticalAlignment.Center
        });

        grid.Children.Add(leftStack);

        // 右侧按钮
        var rightStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top
        };

        // 主题切换
        var isDark = string.Equals(_styleManager!.CurrentTheme, "dark", StringComparison.OrdinalIgnoreCase);
        _themeToggleIcon = new PathIcon
        {
            Width = 14, Height = 14,
            Foreground = ResourceHelper.FindBrush("ThemeToggleForeground") ?? Brushes.Gray,
            Data = ResourceHelper.FindIcon(isDark ? "SunIcon" : "MoonIcon")
        };
        var themeBtn = new Button { Classes = { "win-caption" }, Content = _themeToggleIcon };
        themeBtn.Click += OnThemeToggleClick;
        rightStack.Children.Add(themeBtn);

        rightStack.Children.Add(CreateCaptionButton("\u2014", false, (_, _) => WindowState = WindowState.Minimized));
        rightStack.Children.Add(CreateCaptionButton("\u25A1", false,
            (_, _) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized));
        rightStack.Children.Add(CreateCaptionButton("\u2715", true, (_, _) => Close()));

        grid.Children.Add(rightStack);
        border.Child = grid;
        TitleBarHost.Content = border;
    }

    // === 内容区域 ===

    private void BuildContent()
    {
        var config = _configService!.Current;
        var layout = config.GetLayoutForStyle(_styleManager!.CurrentStyle);
        var contentGrid = _layoutEngine!.BuildLayout(
            layout, config.Components, _viewModel!, _styleManager.CurrentStyle);
        ContentHost.Content = contentGrid;
    }

    // === 入场动画 ===

    private async Task PlayEntranceAsync()
    {
        if (ContentHost.Content is not Grid contentGrid) return;

        var glass = IsGlass();

        // 入场参数与旧代码一致
        // Fluent: translateY(20), 初始 80ms, 间隔 70ms
        // Glass:  translateY(24), 初始 100ms, 间隔 80ms
        var translateY = glass ? 24.0 : 20.0;
        var initialDelay = glass ? 100 : 80;
        var staggerDelay = glass ? 80 : 70;

        // 按 config.yaml 中组件声明顺序收集卡片
        // 旧代码入场顺序: ProfileCard -> MapCard -> TechCard -> PhilosophyCard
        var configOrder = _configService!.Current.Components
            .Select(c => c.Name)
            .ToList();

        var allCards = new System.Collections.Generic.List<Control>();
        CollectCards(contentGrid, allCards);

        // 按配置声明顺序排序
        allCards.Sort((a, b) =>
        {
            var ia = a.Tag is string ta ? configOrder.IndexOf(ta) : int.MaxValue;
            var ib = b.Tag is string tb ? configOrder.IndexOf(tb) : int.MaxValue;
            return ia.CompareTo(ib);
        });

        // 初始状态：透明 + 向下偏移
        foreach (var card in allCards)
        {
            card.Opacity = 0;
            card.RenderTransform = new TranslateTransform(0, translateY);
        }

        await Task.Delay(initialDelay);

        // 依次入场
        foreach (var card in allCards)
        {
            card.Opacity = 1;
            card.RenderTransform = new TranslateTransform(0, 0);
            await Task.Delay(staggerDelay);
        }
    }

    /// <summary>
    /// 递归收集所有卡片控件（跳过子 Grid 容器本身）
    /// </summary>
    private static void CollectCards(Grid grid, System.Collections.Generic.List<Control> cards)
    {
        foreach (var child in grid.Children)
        {
            if (child is Grid subGrid)
                CollectCards(subGrid, cards);
            else if (child is Control control)
                cards.Add(control);
        }
    }

    // === 主题切换 ===

    private void OnThemeToggleClick(object? sender, RoutedEventArgs e)
    {
        var isDark = string.Equals(_styleManager!.CurrentTheme, "dark", StringComparison.OrdinalIgnoreCase);
        var newTheme = isDark ? "light" : "dark";

        _styleManager!.ApplyStyle(_styleManager.CurrentStyle, newTheme);
        ApplyRootStyle();
        BuildTitleBar();
        BuildContent();
    }

    // === 社交按钮 ===

    public void HandleSocialClick(string buttonName)
    {
        var url = buttonName switch
        {
            "XButton" => _viewModel?.XUrl,
            "GitHubButton" => _viewModel?.GitHubUrl,
            _ => null
        };
        if (!string.IsNullOrEmpty(url))
            OpenUrl(url);
    }

    // === 窗口拖拽 ===

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }

    // === 辅助方法 ===

    private bool IsGlass()
        => string.Equals(_styleManager?.CurrentStyle, "glass", StringComparison.OrdinalIgnoreCase);

    private static void RemoveGlassBackgroundLayers(Grid grid)
    {
        var toRemove = grid.Children
            .Where(c => (c is Border b && b.Name is "GlassBg") ||
                        (c is Avalonia.Controls.Shapes.Ellipse el && el.Name is "GlassGlow"))
            .ToList();
        foreach (var item in toRemove)
            grid.Children.Remove(item);
    }

    private static Button CreateTrafficButton(string className, string icon, EventHandler<RoutedEventArgs> click)
    {
        var btn = new Button { Classes = { "traffic", className } };
        btn.Content = new TextBlock { Classes = { "icon" }, Text = icon };
        btn.Click += click;
        return btn;
    }

    private static Button CreateCaptionButton(string text, bool isClose, EventHandler<RoutedEventArgs> click)
    {
        var btn = new Button { Classes = { "win-caption" } };
        if (isClose) btn.Classes.Add("win-caption-close");
        btn.Content = new TextBlock { Text = text };
        btn.Click += click;
        return btn;
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
