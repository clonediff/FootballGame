using Microsoft.Extensions.Logging;
using UIApplication.ViewModels;
using UIApplication.Views;

namespace UIApplication;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<LobbyViewModel>();
        builder.Services.AddTransient<GameViewModel>();

		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<LobbyPage>();
		builder.Services.AddTransient<GamePage>();

		return builder.Build();
	}
}
