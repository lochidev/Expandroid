using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Expandroid.Models;
using Expandroid.Services;
using MudBlazor.Services;

namespace Expandroid;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>().UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif
#if ANDROID
        builder.Services.AddSingleton<ICheckIfActivated, CheckIfActivated>();
#endif
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton(FileSaver.Default);
        builder.Services.AddSingleton(FilePicker.Default);
        return builder.Build();
    }
}
