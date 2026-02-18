# PersonalCardDemo

基于 Avalonia UI 的个人主页卡片桌面应用，支持 Fluent / Glass 双风格切换、亮暗主题、配置热加载。

## 功能

- Fluent 风格（Windows 11 设计语言）和 Glass 毛玻璃风格
- 亮色 / 暗色主题切换（Fluent 模式下）
- `config.yaml` 热加载，保存即刷新
- 组件化卡片布局，支持自定义排列
- 跨平台：Windows / macOS / Linux

## 快速开始

### 从 Release 下载

下载 `PersonalCardDemo-win-x64.zip`，解压后双击 `PersonalCardDemo.exe` 即可运行，无需安装任何运行时。

### 从源码构建

```bash
# 需要 .NET 8.0 SDK
dotnet run
```

### 发布自包含 exe

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish/win-x64
```

## 配置

编辑 `config.yaml` 自定义个人信息、风格、布局等，保存后自动刷新。

```yaml
style: fluent          # fluent 或 glass
theme: light           # light 或 dark（仅 fluent 生效）

profile:
  name: Kilock
  id: "@kilock_1208"
  signature: "清凤凤凤凤凤!"
  tags: [Dev, INTP, Mtx]
```

完整配置项见 `config.yaml` 文件。

## 自定义图片

将图片放入 `Assets/` 目录：

| 图片 | 文件名 | 推荐尺寸 | 说明 |
|------|--------|----------|------|
| 封面图 | `Assets/cover.jpg` | 600x200 | 资料卡顶部背景 |
| 头像 | `Assets/avatar.png` | 200x200 | 圆形裁剪，正方形即可 |
| 地图截图 | `Assets/map.png` | 600x400 | 右上地图卡背景 |

## 项目结构

```
PersonalCardDemo/
  Program.cs                    # 程序入口，DI 容器构建
  App.axaml / App.axaml.cs      # 应用初始化
  Views/
    MainWindow.axaml(.cs)       # 主窗口，标题栏/内容区/动画
  ViewModels/
    MainViewModel.cs            # 数据绑定，属性变更通知
    ViewModelBase.cs            # INPC 基类
  Components/
    Templates/                  # 四种卡片模板（Profile/Map/TechStack/Philosophy）
    ComponentFactory.cs         # 组件工厂
    ComponentRegistry.cs        # 模板注册表
  Config/
    ConfigService.cs            # 配置加载/热加载/文件监听
    ConfigValidator.cs          # 配置校验
    ConfigMigrator.cs           # 旧格式迁移
    Models/                     # 配置数据模型
  Layout/
    GridLayoutEngine.cs         # Grid 布局引擎
  Styles/
    FluentStyle.axaml           # Fluent 风格资源
    GlassStyle.axaml            # Glass 风格资源
    StyleManager.cs             # 风格/主题切换
  Helpers/
    ResourceHelper.cs           # 资源查找工具
  Converters/
    IconKeyConverter.cs         # 图标/颜色转换器
  Hosting/
    ServiceCollectionExtensions.cs  # DI 注册
  Assets/                       # 图片资源
```

## 技术栈

- [Avalonia UI](https://avaloniaui.net/) 11.3
- .NET 8.0
- YamlDotNet
- Microsoft.Extensions.DependencyInjection

## 平台支持

- Windows
- macOS
- Linux

## License

MIT
