using System.Diagnostics;

namespace MauiApp1;

public partial class App : Application
{
    private Window window = null;
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }
    protected override Window CreateWindow(IActivationState activationState)
    {
        if (window is null)
        {
            window = base.CreateWindow(activationState);
        }
        else
        {
            MainPage = new MainPage();
        }
        return window;
    }
}
