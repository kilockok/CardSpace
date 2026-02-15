using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace PersonalCardDemo;

public partial class MainWindow : Window
{
    // ========== 图片资源说明 ==========
    // 封面图: 将图片放入 Assets/cover.jpg (推荐 600x200 城市夜景)
    //         AXAML 中对应 x:Name="CoverImage" 的 Image 控件
    //         Source 路径: avares://PersonalCardDemo/Assets/cover.jpg
    //
    // 头像: 将图片放入 Assets/avatar.png (推荐 200x200 正方形)
    //       AXAML 中对应 x:Name="AvatarImage" 的 Image 控件
    //       Source 路径: avares://PersonalCardDemo/Assets/avatar.png
    //       外层 Border 已做圆形裁剪，图片会自动变圆
    //
    // 地图截图: 将图片放入 Assets/map.png (推荐 600x400 地图截图)
    //           AXAML 中对应 x:Name="MapImage" 的 Image 控件
    //           Source 路径: avares://PersonalCardDemo/Assets/map.png
    //
    // 替换方式: 放入图片后取消 AXAML 中对应注释，dotnet build 即可
    // ===================================

    public MainWindow()
    {
        InitializeComponent();
    }

    // 窗口任意位置拖拽移动
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }

    // 关闭
    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    // 最小化
    private void MinimizeButton_Click(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    // 最大化/还原
    private void MaximizeButton_Click(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }
}
