
namespace Launcher;

/// <summary>
///     Represents the application window.
/// </summary>
public partial class App : Application
{
    /// <summary>
    ///     Initializes a new instance of <see cref="App" />.
    /// </summary>
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }

    /// <inheritdoc />
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

        window.Title = "SharpEngine Launcher";
        window.Width = 700;
        window.Height = 500;

        return window;
    }
}
