# v1.0.0 - 架构重构

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
# Changelog

## 2026-02-18

### Assets 外置

图片资源不再嵌入 exe 内部，改为独立存放于 exe 同级的 `Assets/` 文件夹。

变更内容：

- `Assets/cover.jpg`、`Assets/avatar.png`、`Assets/map.png` 发布时自动复制到输出目录，与 exe 平级
- `config.yaml` 中图片路径更新为 `Assets/xxx` 格式，并附带注释说明每张图片的名称与用途
- 移除了嵌入资源（`AvaloniaResource`）回退逻辑，图片统一从文件系统加载
- 找不到图片时会在 Debug 输出中打印路径提示

影响：

- 从旧版本升级时，需要将图片从 exe 同级根目录移动到 `Assets/` 子文件夹下
- `config.yaml` 中的图片路径需要加上 `Assets/` 前缀（如 `cover.jpg` -> `Assets/cover.jpg`）
