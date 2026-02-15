# PersonalCardDemo

基于 Avalonia UI 的个人主页卡片桌面应用，采用液态玻璃风格设计，支持跨平台运行。

## 技术栈

- .NET 10
- Avalonia UI 11.3
- ExperimentalAcrylicMaterial (液态玻璃效果)

## 布局结构

```
+------------------------------------------+
|  [红绿灯]      个人主页                    |  标题栏
+------------------------------------------+
|              |                            |
|   左侧       |   右上：地图卡              |
|   资料卡      |   (地图截图 + 地点信息)      |
|   (封面图     |                            |
|    头像       +-------------+-------------+
|    名字       |  右下左      |  右下右      |
|    标签       |  技术栈卡    |  预留卡      |
|    签名       |             |             |
|    社交)      |             |             |
+--------------+-------------+-------------+
```

## 运行

```bash
dotnet run
```

## 自定义图片

将图片放入 `Assets/` 目录，然后在 `MainWindow.axaml` 中取消对应 Image 控件的注释即可。

| 图片 | 文件名 | 推荐尺寸 | 说明 |
|------|--------|----------|------|
| 封面图 | `Assets/cover.jpg` | 600x200 | 资料卡顶部背景 |
| 头像 | `Assets/avatar.png` | 200x200 | 圆形裁剪，正方形即可 |
| 地图截图 | `Assets/map.png` | 600x400 | 右上地图卡背景 |

详细路径说明见 `MainWindow.axaml.cs` 顶部注释。

## 项目结构

```
PersonalCardDemo/
  App.axaml            # 应用入口 XAML
  App.axaml.cs         # 应用初始化
  MainWindow.axaml     # 主窗口布局与样式
  MainWindow.axaml.cs  # 窗口交互逻辑
  Program.cs           # 程序入口
  Assets/              # 图片资源目录
```

## 平台支持

- Windows
- macOS
- Linux

## License

MIT
