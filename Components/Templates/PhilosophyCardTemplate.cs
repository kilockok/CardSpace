using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using PersonalCardDemo.Config.Models;
using PersonalCardDemo.ViewModels;

namespace PersonalCardDemo.Components.Templates;

/// <summary>
/// 理念卡模板 - 展示哲学理念/引用
/// 包含蓝色装饰线、标题、引用边框、引文和出处
///
/// Fluent: 单层 Border（fluent-card 承载 Background/Border）
/// Glass:  双层 Border（outerBorder + Panel + innerBorder，中间夹 AcrylicBorder）
/// </summary>
public sealed class PhilosophyCardTemplate : TemplateBase, IComponentTemplate
{
    public string TypeName => "philosophy_card";

    public Control Build(ComponentConfig config, MainViewModel viewModel, string currentStyle)
    {
        var glass = IsGlass(currentStyle);

        var contentStack = BuildContentStack(config, viewModel, glass);

        var outerBorder = glass
            ? BuildGlassShell("PhilosophyCard", contentStack)
            : BuildFluentShell("PhilosophyCard", contentStack);

        outerBorder.DataContext = viewModel;
        return outerBorder;
    }

    public void ApplyOverrides(Control root, ComponentConfig config, MainViewModel viewModel, string currentStyle)
    {
        // 热加载时由 LayoutEngine 重建
    }

    private StackPanel BuildContentStack(ComponentConfig config, MainViewModel viewModel, bool glass)
    {
        var contentStack = new StackPanel
        {
            Margin = new Thickness(20, 18),
            Spacing = 10,
            VerticalAlignment = VerticalAlignment.Center
        };

        // 蓝色装饰线
        contentStack.Children.Add(new Border
        {
            Width = 32,
            Height = 4,
            CornerRadius = new CornerRadius(2),
            Background = glass ? TryGetBrush("GlassAccentBrush") : TryGetBrush("AccentBrush"),
            HorizontalAlignment = HorizontalAlignment.Left
        });

        // 标题
        var titleText = new TextBlock
        {
            FontSize = 15,
            FontWeight = FontWeight.SemiBold,
            Foreground = glass ? TryGetBrush("GlassTextPrimary") : TryGetBrush("TextPrimary"),
            Margin = new Thickness(0, 2, 0, 0)
        };
        titleText.Bind(TextBlock.TextProperty, new Binding("PhilosophyTitle"));
        ApplyTextOverride(titleText, GetOverride(config, "title_text"));
        contentStack.Children.Add(titleText);

        // 引用边框
        var quoteBorder = new Border
        {
            BorderThickness = new Thickness(3, 0, 0, 0),
            BorderBrush = glass ? ParseBrush("#40FFFFFF") : TryGetBrush("QuoteBorder"),
            Padding = new Thickness(12, 4, 0, 4)
        };

        var quoteStack = new StackPanel { Spacing = 6 };

        // 引文
        var quoteText = new TextBlock
        {
            FontSize = 12.5,
            Foreground = glass ? TryGetBrush("GlassTextSecondary") : TryGetBrush("TextSecondary"),
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 20
        };
        quoteText.Bind(TextBlock.TextProperty, new Binding("PhilosophyQuote"));
        ApplyTextOverride(quoteText, GetOverride(config, "quote_text"));
        quoteStack.Children.Add(quoteText);

        // 出处
        var attrText = new TextBlock
        {
            FontSize = 12,
            Foreground = glass ? TryGetBrush("GlassTextTertiary") : TryGetBrush("TextTertiary"),
            FontStyle = Avalonia.Media.FontStyle.Italic
        };
        attrText.Bind(TextBlock.TextProperty, new Binding("PhilosophyAttribution"));
        ApplyTextOverride(attrText, GetOverride(config, "attribution_text"));
        quoteStack.Children.Add(attrText);

        quoteBorder.Child = quoteStack;
        contentStack.Children.Add(quoteBorder);

        return contentStack;
    }
}
