using System.Collections.Generic;

namespace PersonalCardDemo.Components;

/// <summary>
/// 组件模板注册表接口
/// </summary>
public interface IComponentRegistry
{
    /// <summary>
    /// 注册一个模板
    /// </summary>
    void Register(IComponentTemplate template);

    /// <summary>
    /// 根据类型名解析模板，未找到返回 null
    /// </summary>
    IComponentTemplate? Resolve(string typeName);

    /// <summary>
    /// 所有已注册的类型名
    /// </summary>
    IReadOnlyList<string> RegisteredTypes { get; }
}
