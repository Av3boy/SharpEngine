
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

        window.Width = 800;
        window.Height = 600;

        return window;
    }
}
