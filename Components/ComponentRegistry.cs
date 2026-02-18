using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalCardDemo.Components;

/// <summary>
/// 组件模板注册表实现
/// 使用字典映射 typeName -> IComponentTemplate，O(1) 查找
/// </summary>
public sealed class ComponentRegistry : IComponentRegistry
{
    private readonly Dictionary<string, IComponentTemplate> _templates = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 构造时批量注册所有通过 DI 注入的模板
    /// </summary>
    public ComponentRegistry(IEnumerable<IComponentTemplate> templates)
    {
        foreach (var template in templates)
        {
            _templates[template.TypeName] = template;
        }
    }

    public void Register(IComponentTemplate template)
    {
        _templates[template.TypeName] = template;
    }

    public IComponentTemplate? Resolve(string typeName)
    {
        return _templates.GetValueOrDefault(typeName);
    }

    public IReadOnlyList<string> RegisteredTypes => _templates.Keys.ToList().AsReadOnly();
}
