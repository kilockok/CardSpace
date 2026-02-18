using System.Collections.Generic;
using PersonalCardDemo.Config.Models;

namespace PersonalCardDemo.Config;

/// <summary>
/// 配置迁移器 - 将旧格式配置自动补全为新格式
/// 当 config.yaml 中没有 components 节点时，根据旧数据源自动生成默认组件列表
/// 确保旧配置文件无需修改即可正常运行
/// </summary>
public sealed class ConfigMigrator
{
    /// <summary>
    /// 检查并迁移配置，如果 components 为空则自动生成默认组件声明
    /// </summary>
    public AppConfig Migrate(AppConfig config)
    {
        if (config.Components is not { Count: > 0 })
        {
            config.Components = CreateDefaultComponents();
        }

        return config;
    }

    /// <summary>
    /// 生成默认的四卡片组件布局，与原始硬编码布局一致
    /// </summary>
    private static List<ComponentConfig> CreateDefaultComponents()
    {
        return
        [
            // 左侧资料卡，跨两行
            new ComponentConfig
            {
                Name = "profile_card",
                Type = "profile_card",
                GridRow = 0,
                GridColumn = 0,
                GridRowSpan = 2,
                GridColumnSpan = 1,
                Visible = true,
                ZIndex = 0
            },

            // 右上地图卡
            new ComponentConfig
            {
                Name = "map_card",
                Type = "map_card",
                GridRow = 0,
                GridColumn = 1,
                Visible = true,
                ZIndex = 0
            },

            // 右下技术栈卡（子布局左列）
            new ComponentConfig
            {
                Name = "tech_card",
                Type = "tech_stack_card",
                GridRow = 1,
                GridColumn = 1,
                Visible = true,
                ZIndex = 0,
                SubColumns = "*,*",
                SubColumnSpacing = 16,
                SubColumn = 0
            },

            // 右下理念卡（子布局右列）
            new ComponentConfig
            {
                Name = "philosophy_card",
                Type = "philosophy_card",
                GridRow = 1,
                GridColumn = 1,
                Visible = true,
                ZIndex = 0,
                SubColumn = 1
            }
        ];
    }
}
