using System;

namespace PersonalCardDemo.Styles;

/// <summary>
/// 风格变更事件参数
/// </summary>
public sealed class StyleChangedEventArgs : EventArgs
{
    public required string OldStyle { get; init; }
    public required string NewStyle { get; init; }
    public required string Theme { get; init; }
}

/// <summary>
/// 风格管理接口 - 负责 Fluent/Glass 风格的切换
/// </summary>
public interface IStyleManager
{
    /// <summary>
    /// 当前风格名称
    /// </summary>
    string CurrentStyle { get; }

    /// <summary>
    /// 当前主题（light/dark）
    /// </summary>
    string CurrentTheme { get; }

    /// <summary>
    /// 应用风格和主题
    /// </summary>
    /// <param name="style">fluent 或 glass</param>
    /// <param name="theme">light 或 dark（仅 fluent 生效）</param>
    void ApplyStyle(string style, string theme);

    /// <summary>
    /// 风格变更事件
    /// </summary>
    event EventHandler<StyleChangedEventArgs>? StyleChanged;
}
