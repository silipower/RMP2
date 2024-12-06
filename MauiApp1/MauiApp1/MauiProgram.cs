using NotebookApp;

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
                fonts.AddFont("Caveat-VariableFont_wght.ttf", "Caveat");
                fonts.AddFont("Oswald-VariableFont_wght.ttf", "Oswald");
                fonts.AddFont("Pacifico-Regular.ttf", "Pacifico");
            });

		return builder.Build();
	}
}
