namespace Expandroid;

public partial class App : Application
{
    private Window window = null;
    public App()
    {
        InitializeComponent();
    }
    protected override Window CreateWindow(IActivationState activationState)
    {
        window ??= new Window(new MainPage());
        return window;
    }
}
