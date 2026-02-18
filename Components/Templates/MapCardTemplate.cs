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
/// 地图卡模板 - 右上区域，展示地理位置信息
///
/// Fluent: 单层 Border（fluent-card 承载 Background/Border/Shadow）
/// Glass:  双层 Border（outerBorder + Panel + innerBorder，中间夹 AcrylicBorder）
/// </summary>
public sealed class MapCardTemplate : TemplateBase, IComponentTemplate
{
    public string TypeName => "map_card";

    public Control Build(ComponentConfig config, MainViewModel viewModel, string currentStyle)
    {
        var glass = IsGlass(currentStyle);

        var contentGrid = BuildContentGrid(viewModel, glass);

        var outerBorder = glass
            ? BuildGlassShell("MapCard", contentGrid)
            : BuildFluentShell("MapCard", contentGrid, clipContent: true);

        outerBorder.DataContext = viewModel;
        return outerBorder;
    }

    public void ApplyOverrides(Control root, ComponentConfig config, MainViewModel viewModel, string currentStyle)
    {
        // 热加载时由 LayoutEngine 重建
    }

    private Grid BuildContentGrid(MainViewModel viewModel, bool glass)
    {
        var contentGrid = new Grid();

        // 地图背景
        contentGrid.Children.Add(new Border
        {
            Background = glass ? TryGetBrush("GlassMapGradient") : TryGetBrush("MapGradientBrush")
        });

        // 地图图片
        contentGrid.Children.Add(new Image
        {
            Source = LoadImage(viewModel.MapImage),
            Stretch = Stretch.UniformToFill
        });

        // 底部信息叠加层
        var overlayBorder = new Border
        {
            VerticalAlignment = VerticalAlignment.Bottom,
            Padding = glass ? new Thickness(24, 16) : new Thickness(20, 14)
        };

        if (glass)
        {
            overlayBorder.Background = TryGetBrush("GlassMapOverlayGradient");
        }
        else
        {
            overlayBorder.Background = TryGetBrush("MapOverlayBackground");
            overlayBorder.BorderBrush = TryGetBrush("MapOverlayBorder");
            overlayBorder.BorderThickness = new Thickness(0, 1, 0, 0);
            overlayBorder.CornerRadius = new CornerRadius(0, 0, 8, 8);
        }

        var infoStack = new StackPanel { Spacing = glass ? 6 : 4 };

        // 城市行
        var cityRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8
        };

        cityRow.Children.Add(new PathIcon
        {
            Data = TryGetIcon("LocationIcon"),
            Foreground = glass ? TryGetBrush("GlassLocationForeground") : TryGetBrush("LocationIconForeground"),
            Width = 18,
            Height = 18
        });

        var cityText = new TextBlock
        {
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            Foreground = glass ? TryGetBrush("GlassTextPrimary") : TryGetBrush("TextPrimary")
        };
        cityText.Bind(TextBlock.TextProperty, new Binding("City"));
        cityRow.Children.Add(cityText);
        infoStack.Children.Add(cityRow);

        // 描述
        var descText = new TextBlock
        {
            FontSize = 13,
            Foreground = glass ? TryGetBrush("GlassTextSecondary") : TryGetBrush("TextSecondary"),
            Margin = new Thickness(26, 0, 0, 0)
        };
        descText.Bind(TextBlock.TextProperty, new Binding("LocationDescription"));
        infoStack.Children.Add(descText);

        overlayBorder.Child = infoStack;
        contentGrid.Children.Add(overlayBorder);

        return contentGrid;
    }
}
