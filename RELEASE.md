# v2.0.0 - 架构重构

## 变更概要

从单文件硬编码架构重构为组件化、配置驱动的架构，支持双风格切换和配置热加载。

## 新增

- Fluent / Glass 双风格，运行时通过 `config.yaml` 切换
- 亮色 / 暗色主题切换（Fluent 模式）
- `config.yaml` 热加载，保存即刷新 UI
- 组件化卡片系统（ProfileCard / MapCard / TechStackCard / PhilosophyCard）
- Grid 布局引擎，支持通过配置自定义卡片排列
- 配置校验与旧格式自动迁移
- DI 容器管理所有服务生命周期
- 入场交错动画

## 架构改进

- 拆分 FluentWindow / GlassWindow 为统一的 MainWindow + StyleManager
- 引入 IComponentTemplate / ComponentFactory / ComponentRegistry 组件体系
- 引入 IConfigService / ConfigValidator / ConfigMigrator 配置体系
- 引入 ILayoutEngine 布局引擎抽象
- 引入 ResourceHelper 统一资源查找
- 卡片外壳构建（BuildFluentShell / BuildGlassShell）提取到 TemplateBase 基类
- Program.Services 静态全局状态改为 App 实例属性注入

## 修复

- Fluent 模式下标签 hover 动画被卡片 ClipToBounds 截断
- Glass 模式下标签 hover 动画被裁剪

## 下载

| 文件 | 说明 |
|------|------|
| `PersonalCardDemo-win-x64.zip` | Windows x64 自包含单文件，无需安装 .NET 运行时 |

解压后将 `PersonalCardDemo.exe` 和 `config.yaml` 放在同一目录，双击运行。
