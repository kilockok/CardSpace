using Microsoft.Extensions.DependencyInjection;
using PersonalCardDemo.Components;
using PersonalCardDemo.Components.Templates;
using PersonalCardDemo.Config;
using PersonalCardDemo.Layout;
using PersonalCardDemo.Styles;
using PersonalCardDemo.ViewModels;

namespace PersonalCardDemo.Hosting;

/// <summary>
/// DI 容器注册扩展 - 集中管理所有服务的生命周期
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCardServices(this IServiceCollection services)
    {
        // 配置系统
        services.AddSingleton<ConfigValidator>();
        services.AddSingleton<ConfigMigrator>();
        services.AddSingleton<IConfigService, ConfigService>();

        // 组件模板（通过 IEnumerable<IComponentTemplate> 批量注入到注册表）
        services.AddSingleton<IComponentTemplate, ProfileCardTemplate>();
        services.AddSingleton<IComponentTemplate, MapCardTemplate>();
        services.AddSingleton<IComponentTemplate, TechStackCardTemplate>();
        services.AddSingleton<IComponentTemplate, PhilosophyCardTemplate>();

        // 组件系统
        services.AddSingleton<IComponentRegistry, ComponentRegistry>();
        services.AddSingleton<ComponentFactory>();

        // 布局引擎
        services.AddSingleton<ILayoutEngine, GridLayoutEngine>();

        // 风格管理
        services.AddSingleton<IStyleManager, StyleManager>();

        // ViewModel
        services.AddSingleton<MainViewModel>();

        return services;
    }
}
