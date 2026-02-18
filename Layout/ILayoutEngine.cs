using System.Collections.Generic;
using Avalonia.Controls;
using PersonalCardDemo.Config.Models;
using PersonalCardDemo.ViewModels;

namespace PersonalCardDemo.Layout;

/// <summary>
/// 布局引擎接口 - 负责根据配置构建和更新内容区域
/// </summary>
public interface ILayoutEngine
{
    /// <summary>
    /// 根据配置构建完整的内容区域 Grid
    /// </summary>
    /// <param name="layout">布局配置（行列定义、间距等）</param>
    /// <param name="components">组件配置列表</param>
    /// <param name="viewModel">数据上下文</param>
    /// <param name="currentStyle">当前风格</param>
    /// <returns>构建好的 Grid 控件</returns>
    Grid BuildLayout(LayoutConfig layout, IReadOnlyList<ComponentConfig> components,
        MainViewModel viewModel, string currentStyle);

    /// <summary>
    /// 热加载时重建布局
    /// </summary>
    Grid RebuildLayout(LayoutConfig layout, IReadOnlyList<ComponentConfig> components,
        MainViewModel viewModel, string currentStyle);
}
