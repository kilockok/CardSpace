using System;
using Avalonia.Controls;
using PersonalCardDemo.Config.Models;
using PersonalCardDemo.ViewModels;

namespace PersonalCardDemo.Components;

/// <summary>
/// 组件工厂 - 根据 ComponentConfig 从注册表中查找模板并创建控件
///
/// 职责：
/// 1. 从注册表解析模板
/// 2. 调用模板的 Build 方法创建控件
/// 3. 设置 Grid 附加属性（行列位置、跨度）
/// 4. 设置可见性和 ZIndex
/// </summary>
public sealed class ComponentFactory
{
    private readonly IComponentRegistry _registry;

    public ComponentFactory(IComponentRegistry registry)
    {
        _registry = registry;
    }

    /// <summary>
    /// 根据配置创建组件控件
    /// </summary>
    /// <returns>创建的控件，模板未找到时返回 null</returns>
    public Control? Create(ComponentConfig config, MainViewModel viewModel, string currentStyle)
    {
        if (string.IsNullOrWhiteSpace(config.Type))
            return null;

        var template = _registry.Resolve(config.Type);
        if (template == null)
            return null;

        try
        {
            var control = template.Build(config, viewModel, currentStyle);

            // 设置 Grid 附加属性
            Grid.SetRow(control, config.GridRow);
            Grid.SetColumn(control, config.GridColumn);
            Grid.SetRowSpan(control, config.GridRowSpan);
            Grid.SetColumnSpan(control, config.GridColumnSpan);

            // 可见性
            control.IsVisible = config.Visible;

            // 层级
            control.ZIndex = config.ZIndex;

            // 存储组件名称，方便后续查找
            control.Tag = config.Name;

            return control;
        }
        catch (Exception)
        {
            // 单个组件创建失败不影响其他组件
            return null;
        }
    }

    /// <summary>
    /// 更新已有组件的属性（热加载用）
    /// </summary>
    public void Update(Control control, ComponentConfig config, MainViewModel viewModel, string currentStyle)
    {
        var template = _registry.Resolve(config.Type);
        if (template == null) return;

        try
        {
            // 更新 Grid 位置
            Grid.SetRow(control, config.GridRow);
            Grid.SetColumn(control, config.GridColumn);
            Grid.SetRowSpan(control, config.GridRowSpan);
            Grid.SetColumnSpan(control, config.GridColumnSpan);

            control.IsVisible = config.Visible;
            control.ZIndex = config.ZIndex;

            // 委托模板处理内部属性覆写
            template.ApplyOverrides(control, config, viewModel, currentStyle);
        }
        catch (Exception)
        {
            // 更新失败静默忽略
        }
    }
}
