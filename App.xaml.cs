namespace StudentPerformanceXmlApp;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // MainPage створюється через DI (отримує MainViewModel); AppShell не потрібен
        var page = _services.GetRequiredService<MainPage>();
        return new Window(new NavigationPage(page)) { Title = "Успішність студентів" };
    }
}
