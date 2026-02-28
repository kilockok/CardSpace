using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using PersonalCardDemo.Config.Models;
using PersonalCardDemo.ViewModels;
using PersonalCardDemo.Views;

namespace PersonalCardDemo.Components.Templates;

/// <summary>
/// 资料卡模板 - 左侧个人信息卡片
/// 包含封面图、头像、姓名、ID、标签、签名、社交按钮
///
/// Fluent: 单层 Border（fluent-card 承载 Background/Border/Shadow）
/// Glass:  双层 Border（outerBorder + Panel + innerBorder，中间夹 AcrylicBorder）
/// </summary>
public sealed class ProfileCardTemplate : TemplateBase, IComponentTemplate
{
    public string TypeName => "profile_card";

    public Control Build(ComponentConfig config, MainViewModel viewModel, string currentStyle)
    {
        var glass = IsGlass(currentStyle);

        // 内容 Grid：三行布局（封面、头像、内容区）
        var contentGrid = BuildContentGrid(config, viewModel, glass);
        contentGrid.ClipToBounds = false;

        // 根据风格构建不同的卡片外壳
        var outerBorder = glass
            ? BuildGlassShell("ProfileCard", contentGrid)
            : BuildFluentShell("ProfileCard", contentGrid);

        outerBorder.DataContext = viewModel;
        return outerBorder;
    }

    public void ApplyOverrides(Control root, ComponentConfig config, MainViewModel viewModel, string currentStyle)
    {
        // 热加载时由 LayoutEngine 重建
    }

    private Grid BuildContentGrid(ComponentConfig config, MainViewModel viewModel, bool glass)
    {
        // Glass 封面 160px，Fluent 封面 170px
        var grid = new Grid
        {
            RowDefinitions = new RowDefinitions(glass ? "160,Auto,*" : "170,Auto,*")
        };

        // 封面区域（顶部圆角裁剪，不影响下方标签的 scale 动画）
        var coverBorder = new Border
        {
            CornerRadius = new CornerRadius(glass ? 10 : 8, glass ? 10 : 8, 0, 0),
            ClipToBounds = true
        };
        var coverPanel = new Panel();
        coverPanel.Children.Add(new Border
        {
            Background = glass
                ? TryGetBrush("GlassCoverGradient")
                : TryGetBrush("CoverGradientBrush")
        });
        coverPanel.Children.Add(new Image
        {
            Source = LoadImage(viewModel.CoverImage),
            Stretch = Stretch.UniformToFill
        });
        coverBorder.Child = coverPanel;
        Grid.SetRow(coverBorder, 0);
        grid.Children.Add(coverBorder);

        // 头像
        var avatarSize = glass ? 90.0 : 96.0;
        var avatarRadius = avatarSize / 2;

        var avatarBorder = new Border
        {
            Width = avatarSize,
            Height = avatarSize,
            CornerRadius = new CornerRadius(avatarRadius),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, -avatarRadius, 0, 0),
            BorderThickness = new Thickness(glass ? 1.5 : 4),
            BorderBrush = glass
                ? TryGetBrush("GlassAvatarBorder")
                : TryGetBrush("AvatarBorder"),
            ClipToBounds = true
        };

        if (!glass)
        {
            avatarBorder.Effect = new DropShadowEffect
            {
                OffsetX = 0, OffsetY = 2, BlurRadius = 8,
                Color = Colors.Black, Opacity = 0.12
            };
        }

        var avatarPanel = new Panel();

        // 头像回退背景
        var fallbackBorder = new Border
        {
            CornerRadius = new CornerRadius(avatarRadius - 3),
            Background = glass
                ? TryGetBrush("GlassAvatarGradient")
                : TryGetBrush("AvatarFallbackBackground")
        };
        var fallbackText = new TextBlock
        {
            FontSize = glass ? 34 : 36,
            FontWeight = FontWeight.SemiBold,
            Foreground = glass ? Brushes.White : TryGetBrush("TextOnAccent"),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        fallbackText.Bind(TextBlock.TextProperty, new Binding("AvatarFallback"));
        fallbackBorder.Child = fallbackText;
        avatarPanel.Children.Add(fallbackBorder);

        // 头像图片
        var avatarImgBorder = new Border
        {
            CornerRadius = new CornerRadius(avatarRadius - 3),
            ClipToBounds = true
        };
        avatarImgBorder.Child = new Image
        {
            Source = LoadImage(viewModel.AvatarImage),
            Stretch = Stretch.UniformToFill
        };
        avatarPanel.Children.Add(avatarImgBorder);

        avatarBorder.Child = avatarPanel;
        Grid.SetRow(avatarBorder, 1);
        grid.Children.Add(avatarBorder);

        // 内容区域
        var contentStack = new StackPanel
        {
            Spacing = glass ? 16 : 14,
            Margin = new Thickness(24, glass ? 8 : 6, 24, 24),
            ClipToBounds = false
        };

        // 姓名 + ID
        var nameStack = new StackPanel
        {
            Spacing = glass ? 4 : 2,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var nameText = new TextBlock
        {
            FontSize = 22,
            FontWeight = FontWeight.SemiBold,
            Foreground = glass ? TryGetBrush("GlassTextPrimary") : TryGetBrush("TextPrimary"),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        nameText.Bind(TextBlock.TextProperty, new Binding("Name"));
        ApplyTextOverride(nameText, GetOverride(config, "name_text"));
        nameStack.Children.Add(nameText);

        var idText = new TextBlock
        {
            FontSize = 13,
            Foreground = glass ? TryGetBrush("GlassTextTertiary") : TryGetBrush("TextTertiary"),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        idText.Bind(TextBlock.TextProperty, new Binding("Id"));
        ApplyTextOverride(idText, GetOverride(config, "id_text"));
        nameStack.Children.Add(idText);

        contentStack.Children.Add(nameStack);

        // 标签
        var tagsControl = new ItemsControl
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            ClipToBounds = false,
            ItemsPanel = new FuncTemplate<Panel?>(() =>
                new WrapPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    ItemSpacing = 8,
                    ClipToBounds = false
                }),
            ItemTemplate = new FuncDataTemplate<string>((tag, _) =>
            {
                var border = new Border
                {
                    Background = glass ? TryGetBrush("GlassTagBackground") : TryGetBrush("TagBackground"),
                    CornerRadius = new CornerRadius(glass ? 14 : 4),
                    Padding = glass ? new Thickness(14, 6) : new Thickness(12, 5),
                    Classes = { glass ? "glass-tag" : "fluent-tag" }
                };
                border.Child = new TextBlock
                {
                    Text = tag,
                    FontSize = 12,
                    Foreground = glass ? TryGetBrush("GlassTagForeground") : TryGetBrush("TagForeground")
                };
                return border;
            }, supportsRecycling: true)
        };
        tagsControl.Bind(ItemsControl.ItemsSourceProperty, new Binding("Tags"));
        contentStack.Children.Add(tagsControl);

        // 分割线
        var separator = new Border
        {
            Height = 1,
            Margin = new Thickness(8, 2),
            Background = glass
                ? ParseBrush("#15FFFFFF")
                : TryGetBrush("Separator")
        };
        contentStack.Children.Add(separator);

        // 签名
        var sigText = new TextBlock
        {
            FontStyle = Avalonia.Media.FontStyle.Italic,
            FontSize = 13,
            Foreground = glass ? TryGetBrush("GlassTextSecondary") : TryGetBrush("TextSecondary"),
            TextAlignment = TextAlignment.Center
        };
        sigText.Bind(TextBlock.TextProperty, new Binding("Signature"));
        ApplyTextOverride(sigText, GetOverride(config, "signature_text"));
        contentStack.Children.Add(sigText);

        // 社交按钮
        var socialStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = glass ? 16 : 12,
            Margin = new Thickness(0, glass ? 4 : 2, 0, 0)
        };

        var xButton = CreateSocialButton("XIcon", glass);
        xButton.Name = "XButton";
        xButton.Click += OnSocialButtonClick;
        socialStack.Children.Add(xButton);

        var ghButton = CreateSocialButton("GitHubIcon", glass);
        ghButton.Name = "GitHubButton";
        ghButton.Click += OnSocialButtonClick;
        socialStack.Children.Add(ghButton);

        contentStack.Children.Add(socialStack);

        Grid.SetRow(contentStack, 2);
        grid.Children.Add(contentStack);

        return grid;
    }

    private static void OnSocialButtonClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Name: { } name } button) return;
        var mainWindow = button.FindAncestorOfType<MainWindow>();
        mainWindow?.HandleSocialClick(name);
    }

    private static Button CreateSocialButton(string iconKey, bool glass)
    {
        var button = new Button
        {
            Width = glass ? 42 : 40,
            Height = glass ? 42 : 40,
            CornerRadius = new CornerRadius(glass ? 21 : 20),
            // Glass: Background 在代码中设置；Fluent: Background 由 AXAML 样式设置但这里也设初始值
            Background = glass
                ? TryGetBrush("GlassSocialBackground")
                : TryGetBrush("SocialBackground"),
            BorderThickness = new Thickness(glass ? 0 : 1),
            Classes = { glass ? "glass-social" : "fluent-social" }
        };

        if (!glass)
            button.BorderBrush = TryGetBrush("SocialBorder");

        button.Content = new PathIcon
        {
            Data = TryGetIcon(iconKey),
            Foreground = glass ? Brushes.White : TryGetBrush("SocialIconForeground"),
            Width = glass ? 18 : 16,
            Height = glass ? 18 : 16
        };

        return button;
    }
}
