using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using PersonalCardDemo.Components;
using PersonalCardDemo.Config.Models;
using PersonalCardDemo.ViewModels;

namespace PersonalCardDemo.Layout;

/// <summary>
/// Grid 布局引擎实现
///
/// 职责：
/// 1. 解析 LayoutConfig 中的行列定义字符串，构建 Grid
/// 2. 处理子布局（同一 Grid 单元格内多个组件共享子 Grid）
/// 3. 通过 ComponentFactory 创建组件并放入 Grid
/// </summary>
public sealed class GridLayoutEngine : ILayoutEngine
{
    private readonly ComponentFactory _factory;

    public GridLayoutEngine(ComponentFactory factory)
    {
        _factory = factory;
    }

    public Grid BuildLayout(LayoutConfig layout, IReadOnlyList<ComponentConfig> components,
        MainViewModel viewModel, string currentStyle)
    {
        return CreateGrid(layout, components, viewModel, currentStyle);
    }

    public Grid RebuildLayout(LayoutConfig layout, IReadOnlyList<ComponentConfig> components,
        MainViewModel viewModel, string currentStyle)
    {
        // 热加载时完全重建（组件树结构可能变化，增量更新复杂度过高）
        return CreateGrid(layout, components, viewModel, currentStyle);
    }

    private Grid CreateGrid(LayoutConfig layout, IReadOnlyList<ComponentConfig> components,
        MainViewModel viewModel, string currentStyle)
    {
        var grid = new Grid
        {
            ColumnDefinitions = ColumnDefinitions.Parse(layout.Columns),
            RowDefinitions = RowDefinitions.Parse(layout.Rows),
            ColumnSpacing = layout.ColumnSpacing,
            RowSpacing = layout.RowSpacing,
            Margin = ParseThickness(layout.Margin)
        };

        // 按 z_index 排序，低的先添加
        var sorted = components
            .Where(c => c.Visible)
            .OrderBy(c => c.ZIndex)
            .ToList();

        // 识别需要子布局的组件组（同一 Grid 单元格内有多个组件）
        var subGroups = sorted
            .Where(c => c.SubColumn.HasValue)
            .GroupBy(c => (c.GridRow, c.GridColumn))
            .ToDictionary(g => g.Key, g => g.ToList());

        // 已处理的子布局组件，避免重复添加
        var handledSubComponents = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var group in subGroups)
        {
            var subGrid = CreateSubGrid(group.Value, viewModel, currentStyle);
            Grid.SetRow(subGrid, group.Key.GridRow);
            Grid.SetColumn(subGrid, group.Key.GridColumn);
            grid.Children.Add(subGrid);

            foreach (var comp in group.Value)
                handledSubComponents.Add(comp.Name);
        }

        // 添加非子布局的普通组件
        foreach (var comp in sorted)
        {
            if (handledSubComponents.Contains(comp.Name))
                continue;

            var control = _factory.Create(comp, viewModel, currentStyle);
            if (control != null)
                grid.Children.Add(control);
        }

        return grid;
    }

    /// <summary>
    /// 为同一 Grid 单元格内的多个组件创建子 Grid
    /// </summary>
    private Grid CreateSubGrid(List<ComponentConfig> components, MainViewModel viewModel, string currentStyle)
    {
        // 从第一个有 SubColumns 定义的组件获取子布局参数
        var subColDef = components.FirstOrDefault(c => !string.IsNullOrEmpty(c.SubColumns));
        var subColumns = subColDef?.SubColumns ?? "*,*";
        var subSpacing = subColDef?.SubColumnSpacing ?? 16;

        var subGrid = new Grid
        {
            ColumnDefinitions = ColumnDefinitions.Parse(subColumns),
            ColumnSpacing = subSpacing
        };

        foreach (var comp in components.OrderBy(c => c.SubColumn ?? 0))
        {
            var control = _factory.Create(comp, viewModel, currentStyle);
            if (control == null) continue;

            // 子布局中使用 SubColumn 而非 GridColumn
            Grid.SetColumn(control, comp.SubColumn ?? 0);
            // 子布局中行列跨度重置
            Grid.SetRow(control, 0);
            Grid.SetRowSpan(control, 1);
            Grid.SetColumnSpan(control, 1);

            subGrid.Children.Add(control);
        }

        return subGrid;
    }

    /// <summary>
    /// 解析 Thickness 字符串
    /// </summary>
    private static Thickness ParseThickness(string value)
    {
        try
        {
            var parts = value.Split(',');
            return parts.Length switch
            {
                1 => new Thickness(double.Parse(parts[0])),
                2 => new Thickness(double.Parse(parts[0]), double.Parse(parts[1])),
                4 => new Thickness(
                    double.Parse(parts[0]), double.Parse(parts[1]),
                    double.Parse(parts[2]), double.Parse(parts[3])),
                _ => new Thickness(20, 8, 20, 20)
            };
        }
        catch
        {
            return new Thickness(20, 8, 20, 20);
        }
    }
}
