using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using PersonalCardDemo.Hosting;

namespace PersonalCardDemo;

class Program
{
    [System.STAThread]
    public static void Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddCardServices();
        var services = serviceCollection.BuildServiceProvider();

        BuildAvaloniaApp()
            .AfterSetup(_ =>
            {
                // App 实例已创建，注入 DI 容器
                if (Application.Current is App app)
                    app.Services = services;
            })
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
