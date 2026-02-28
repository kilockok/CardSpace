using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using PersonalCardDemo.Config.Models;
using PersonalCardDemo.Converters;
using PersonalCardDemo.ViewModels;

namespace PersonalCardDemo.Components.Templates;

/// <summary>
/// 技术栈卡模板 - 展示技术栈列表和链接
///
/// Fluent: 单层 Border（fluent-card 承载 Background/Border），内容直接作为 Child
/// Glass:  双层 Border（outerBorder + Panel + innerBorder，中间夹 AcrylicBorder）
/// </summary>
public sealed class TechStackCardTemplate : TemplateBase, IComponentTemplate
{
    public string TypeName => "tech_stack_card";

    public Control Build(ComponentConfig config, MainViewModel viewModel, string currentStyle)
    {
        var glass = IsGlass(currentStyle);

        var contentStack = BuildContentStack(glass);

        var outerBorder = glass
            ? BuildGlassShell("TechCard", contentStack)
            : BuildFluentShell("TechCard", contentStack);

        outerBorder.DataContext = viewModel;
        return outerBorder;
    }

    public void ApplyOverrides(Control root, ComponentConfig config, MainViewModel viewModel, string currentStyle)
    {
        // 热加载时由 LayoutEngine 重建
    }

    private StackPanel BuildContentStack(bool glass)
    {
        var contentStack = new StackPanel
        {
            Margin = glass ? new Thickness(16, 14) : new Thickness(16, 12),
            Spacing = glass ? 10 : 8,
            ClipToBounds = false
        };

        // 标题行
        var titleRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8
        };

        titleRow.Children.Add(new PathIcon
        {
            Data = TryGetIcon("CodeIcon"),
            Foreground = glass ? TryGetBrush("GlassAccentBrush") : TryGetBrush("AccentBrush"),
            Width = 14,
            Height = 14
        });

        titleRow.Children.Add(new TextBlock
        {
            Text = "Tech Stack",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Foreground = glass ? TryGetBrush("GlassTextPrimary") : TryGetBrush("TextPrimary")
        });

        contentStack.Children.Add(titleRow);

        // 技术栈列表
        var techItems = new ItemsControl
        {
            ClipToBounds = false,
            ItemsPanel = new FuncTemplate<Panel?>(() =>
                new WrapPanel
                {
                    // Glass 间距 10，Fluent 间距 8（和旧 AXAML 一致）
                    ItemSpacing = glass ? 10 : 8,
                    ClipToBounds = false
                }),
            ItemTemplate = new FuncDataTemplate<TechStackItem>((item, _) =>
            {
                var border = new Border
                {
                    Background = glass ? TryGetBrush("GlassTechItemBackground") : TryGetBrush("TechItemBackground"),
                    CornerRadius = new CornerRadius(glass ? 8 : 4),
                    Padding = new Thickness(10, 6),
                    Margin = new Thickness(0, 0, 0, 6),
                    BorderThickness = new Thickness(1),
                    BorderBrush = glass ? ParseBrush("#08FFFFFF") : TryGetBrush("TechItemBorder"),
                    Classes = { glass ? "glass-tech-item" : "fluent-tech-item" }
                };

                var stack = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 5
                };

                var icon = new PathIcon { Width = 12, Height = 12 };
                icon.Bind(PathIcon.DataProperty, new Binding("Icon") { Converter = IconKeyConverter.Instance });
                icon.Bind(PathIcon.ForegroundProperty, new Binding("Color") { Converter = ColorStringConverter.Instance });
                stack.Children.Add(icon);

                var nameText = new TextBlock
                {
                    FontSize = 11,
                    Foreground = glass ? TryGetBrush("GlassTechText") : TryGetBrush("TechItemText"),
                    VerticalAlignment = VerticalAlignment.Center
                };
                nameText.Bind(TextBlock.TextProperty, new Binding("Name"));
                stack.Children.Add(nameText);

                border.Child = stack;
                return border;
            }, supportsRecycling: true)
        };
        techItems.Bind(ItemsControl.ItemsSourceProperty, new Binding("TechStack"));
        contentStack.Children.Add(techItems);

        // 分割线
        var separator = new Border
        {
            Height = 1,
            Margin = new Thickness(0, 2, 0, 0),
            Background = glass ? ParseBrush("#15FFFFFF") : TryGetBrush("SeparatorLight")
        };
        contentStack.Children.Add(separator);

        // 链接列表
        var linksItems = new ItemsControl
        {
            ItemTemplate = new FuncDataTemplate<LinkItem>((item, _) =>
            {
                var stack = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 6,
                    Margin = new Thickness(0, 2)
                };

                var icon = new PathIcon
                {
                    Foreground = glass ? TryGetBrush("GlassLinkForeground") : TryGetBrush("TextTertiary"),
                    Width = 12, Height = 12
                };
                icon.Bind(PathIcon.DataProperty, new Binding("Icon") { Converter = IconKeyConverter.Instance });
                stack.Children.Add(icon);

                var text = new TextBlock
                {
                    FontSize = 11,
                    Foreground = glass ? TryGetBrush("GlassLinkForeground") : TryGetBrush("TextTertiary"),
                    VerticalAlignment = VerticalAlignment.Center
                };
                text.Bind(TextBlock.TextProperty, new Binding("Text"));
                stack.Children.Add(text);

                return stack;
            }, supportsRecycling: true)
        };
        linksItems.Bind(ItemsControl.ItemsSourceProperty, new Binding("Links"));
        contentStack.Children.Add(linksItems);

        return contentStack;
    }
}
