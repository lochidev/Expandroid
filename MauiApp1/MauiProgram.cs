using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using MauiApp1.Models;
using MauiApp1.Services;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace MauiApp1;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
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
		builder.Logging.AddDebug();
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
