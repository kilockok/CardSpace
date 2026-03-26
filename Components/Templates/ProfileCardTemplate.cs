using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using PersonalCardDemo.Config.Models;
using PersonalCardDemo.ViewModels;
using PersonalCardDemo.Views;

namespace PersonalCardDemo.Components.Templates;

public sealed class ProfileCardTemplate : TemplateBase, IComponentTemplate
{
    public string TypeName => "profile_card";

    public Control Build(ComponentConfig config, MainViewModel viewModel, string currentStyle)
    {
        var glass = IsGlass(currentStyle);
        var contentGrid = BuildContentGrid(config, viewModel, glass);
        contentGrid.ClipToBounds = false;

        var outerBorder = glass
            ? BuildGlassShell("ProfileCard", contentGrid)
            : BuildFluentShell("ProfileCard", contentGrid);

        outerBorder.DataContext = viewModel;
        return outerBorder;
    }

    public void ApplyOverrides(Control root, ComponentConfig config, MainViewModel viewModel, string currentStyle)
    {
    }

    private Grid BuildContentGrid(ComponentConfig config, MainViewModel viewModel, bool glass)
    {
        var grid = new Grid
        {
            RowDefinitions = new RowDefinitions(glass ? "160,Auto,*" : "170,Auto,*")
        };

        var coverBorder = new Border
        {
            CornerRadius = new CornerRadius(glass ? 10 : 8, glass ? 10 : 8, 0, 0),
            ClipToBounds = true
        };
        var coverPanel = new Panel();
        coverPanel.Children.Add(new Border
        {
            Background = glass
                ? TryGetBrush("GlassCoverGradient")
                : TryGetBrush("CoverGradientBrush")
        });
        var coverImage = new Image
        {
            Source = LoadImage(viewModel.CoverImage),
            Stretch = Stretch.UniformToFill
        };
        ApplyImageOverride(coverImage, GetOverride(config, "cover_image"));
        coverPanel.Children.Add(coverImage);
        coverBorder.Child = coverPanel;
        ApplyBorderOverride(coverBorder, GetOverride(config, "cover_block"));
        Grid.SetRow(coverBorder, 0);
        grid.Children.Add(coverBorder);

        var avatarSource = LoadImage(viewModel.AvatarImage);
        if (avatarSource != null)
        {
            var avatarSize = glass ? 90.0 : 96.0;
            var avatarRadius = avatarSize / 2;
            var avatarOverride = GetOverride(config, "avatar_image");
            if (!string.IsNullOrWhiteSpace(avatarOverride?.ImageSource))
            {
                var overrideAvatarSource = LoadImage(avatarOverride.ImageSource);
                if (overrideAvatarSource != null)
                    avatarSource = overrideAvatarSource;
            }

            var avatarBorder = new Border
            {
                Width = avatarSize,
                Height = avatarSize,
                ZIndex = 1,
                CornerRadius = new CornerRadius(avatarRadius),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, -avatarRadius, 0, 0),
                BorderThickness = new Thickness(glass ? 1.5 : 4),
                BorderBrush = glass
                    ? TryGetBrush("GlassAvatarBorder")
                    : TryGetBrush("AvatarBorder"),
                ClipToBounds = true,
                Background = new ImageBrush
                {
                    Source = (IImageBrushSource)avatarSource,
                    Stretch = Stretch.UniformToFill,
                    AlignmentX = AlignmentX.Center,
                    AlignmentY = AlignmentY.Top
                }
            };

            if (!glass)
            {
                avatarBorder.Effect = new DropShadowEffect
                {
                    OffsetX = 0,
                    OffsetY = 2,
                    BlurRadius = 8,
                    Color = Colors.Black,
                    Opacity = 0.12
                };
            }

            ApplyBorderOverride(avatarBorder, GetOverride(config, "avatar_block"));
            Grid.SetRow(avatarBorder, 1);
            grid.Children.Add(avatarBorder);
        }

        var contentStack = new StackPanel
        {
            Spacing = glass ? 16 : 14,
            Margin = new Thickness(24, glass ? 8 : 6, 24, 24),
            ClipToBounds = false
        };

        var nameStack = new StackPanel
        {
            Spacing = glass ? 4 : 2,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var nameText = new TextBlock
        {
            FontSize = 22,
            FontWeight = FontWeight.SemiBold,
            Foreground = glass ? TryGetBrush("GlassTextPrimary") : TryGetBrush("TextPrimary"),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        nameText.Bind(TextBlock.TextProperty, new Binding("Name"));
        ApplyTextOverride(nameText, GetOverride(config, "name_text"));
        nameStack.Children.Add(nameText);

        var idText = new TextBlock
        {
            FontSize = 13,
            Foreground = glass ? TryGetBrush("GlassTextTertiary") : TryGetBrush("TextTertiary"),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        idText.Bind(TextBlock.TextProperty, new Binding("Id"));
        ApplyTextOverride(idText, GetOverride(config, "id_text"));
        nameStack.Children.Add(idText);

        contentStack.Children.Add(nameStack);

        var tagsControl = new ItemsControl
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            ClipToBounds = false,
            ItemsPanel = new FuncTemplate<Panel?>(() =>
                new WrapPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    ItemSpacing = 8,
                    ClipToBounds = false
                }),
            ItemTemplate = new FuncDataTemplate<string>((tag, _) =>
            {
                var border = new Border
                {
                    Background = glass ? TryGetBrush("GlassTagBackground") : TryGetBrush("TagBackground"),
                    CornerRadius = new CornerRadius(glass ? 14 : 4),
                    Padding = glass ? new Thickness(14, 6) : new Thickness(12, 5),
                    Classes = { glass ? "glass-tag" : "fluent-tag" }
                };
                border.Child = new TextBlock
                {
                    Text = tag,
                    FontSize = 12,
                    Foreground = glass ? TryGetBrush("GlassTagForeground") : TryGetBrush("TagForeground")
                };
                return border;
            }, supportsRecycling: true)
        };
        tagsControl.Bind(ItemsControl.ItemsSourceProperty, new Binding("Tags"));
        ApplyControlOverride(tagsControl, GetOverride(config, "tags_list"));
        contentStack.Children.Add(tagsControl);

        var separator = new Border
        {
            Height = 1,
            Margin = new Thickness(8, 2),
            Background = glass
                ? ParseBrush("#15FFFFFF")
                : TryGetBrush("Separator")
        };
        ApplyBorderOverride(separator, GetOverride(config, "separator"));
        contentStack.Children.Add(separator);

        var sigText = new TextBlock
        {
            FontStyle = Avalonia.Media.FontStyle.Italic,
            FontSize = 13,
            Foreground = glass ? TryGetBrush("GlassTextSecondary") : TryGetBrush("TextSecondary"),
            TextAlignment = TextAlignment.Center
        };
        sigText.Bind(TextBlock.TextProperty, new Binding("Signature"));
        ApplyTextOverride(sigText, GetOverride(config, "signature_text"));
        contentStack.Children.Add(sigText);

        var socialStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = glass ? 16 : 12,
            Margin = new Thickness(0, glass ? 4 : 2, 0, 0)
        };

        var xButton = CreateSocialButton("XIcon", glass);
        xButton.Name = "XButton";
        xButton.Click += OnSocialButtonClick;
        socialStack.Children.Add(xButton);

        var ghButton = CreateSocialButton("GitHubIcon", glass);
        ghButton.Name = "GitHubButton";
        ghButton.Click += OnSocialButtonClick;
        socialStack.Children.Add(ghButton);

        ApplyControlOverride(socialStack, GetOverride(config, "social_row"));
        contentStack.Children.Add(socialStack);

        Grid.SetRow(contentStack, 2);
        grid.Children.Add(contentStack);

        return grid;
    }

    private static void OnSocialButtonClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Name: { } name } button) return;
        var mainWindow = button.FindAncestorOfType<MainWindow>();
        mainWindow?.HandleSocialClick(name);
    }

    private static Button CreateSocialButton(string iconKey, bool glass)
    {
        var button = new Button
        {
            Width = glass ? 42 : 40,
            Height = glass ? 42 : 40,
            CornerRadius = new CornerRadius(glass ? 21 : 20),
            Background = glass
                ? TryGetBrush("GlassSocialBackground")
                : TryGetBrush("SocialBackground"),
            BorderThickness = new Thickness(glass ? 0 : 1),
            Classes = { glass ? "glass-social" : "fluent-social" }
        };

        if (!glass)
            button.BorderBrush = TryGetBrush("SocialBorder");

        button.Content = new PathIcon
        {
            Data = TryGetIcon(iconKey),
            Foreground = glass ? Brushes.White : TryGetBrush("SocialIconForeground"),
            Width = glass ? 18 : 16,
            Height = glass ? 18 : 16
        };

        return button;
    }
}
