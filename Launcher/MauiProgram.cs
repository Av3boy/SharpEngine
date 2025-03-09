using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace Launcher;

/// <summary>
///		Represents the entry point of the application.
/// </summary>
public static class MauiProgram
{
	/// <summary>
	///		Creates and builds the application.
	/// </summary>
	/// <returns>The created built application.</returns>
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
