using Avalonia.Controls;
using PersonalCardDemo.Config.Models;
using PersonalCardDemo.ViewModels;

namespace PersonalCardDemo.Components;

/// <summary>
/// 组件模板接口 - 每种卡片类型实现一个
///
/// 设计思路：
/// - TypeName 与 config.yaml 中 component.type 对应
/// - Build 创建完整控件树
/// - ApplyOverrides 热加载时更新已有控件的属性，避免重建
/// </summary>
public interface IComponentTemplate
{
    /// <summary>
    /// 模板类型标识，如 "profile_card"、"map_card"
    /// </summary>
    string TypeName { get; }

    /// <summary>
    /// 构建控件树
    /// </summary>
    /// <param name="config">组件配置（含布局参数和属性覆写）</param>
    /// <param name="viewModel">数据上下文</param>
    /// <param name="currentStyle">当前风格（fluent/glass），模板据此选择不同的视觉表现</param>
    /// <returns>构建好的控件</returns>
    Control Build(ComponentConfig config, MainViewModel viewModel, string currentStyle);

    /// <summary>
    /// 应用属性覆写（热加载时调用）
    /// </summary>
    /// <param name="root">之前 Build 返回的控件</param>
    /// <param name="config">新的组件配置</param>
    /// <param name="viewModel">数据上下文</param>
    /// <param name="currentStyle">当前风格</param>
    void ApplyOverrides(Control root, ComponentConfig config, MainViewModel viewModel, string currentStyle);
}
