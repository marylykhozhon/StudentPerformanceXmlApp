using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using StudentPerformanceXmlApp.Services;
using StudentPerformanceXmlApp.Strategies;
using StudentPerformanceXmlApp.ViewModels;

namespace StudentPerformanceXmlApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // дозволяє читати XML у кодуваннях windows-1251, koi8-u тощо
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Сервіси
        builder.Services.AddSingleton<IDialogService, MauiDialogService>();
        builder.Services.AddSingleton<XmlMetadataService>();
        builder.Services.AddSingleton<XslTransformService>();

        // Strategy: три реалізації + контекст
        builder.Services.AddSingleton<IXmlSearchStrategy, SaxSearchStrategy>();
        builder.Services.AddSingleton<IXmlSearchStrategy, DomSearchStrategy>();
        builder.Services.AddSingleton<IXmlSearchStrategy, LinqToXmlSearchStrategy>();
        builder.Services.AddSingleton<XmlSearchContext>();

        // MVVM
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

#if WINDOWS
        // Підтвердження при закритті вікна хрестиком: «Чи дійсно ви хочете завершити роботу з програмою?»
        builder.ConfigureLifecycleEvents(events => events.AddWindows(windows => windows.OnWindowCreated(window =>
        {
            window.AppWindow.Closing += async (sender, args) =>
            {
                var dialogs = IPlatformApplication.Current?.Services.GetService<IDialogService>();
                if (dialogs is null || dialogs.ExitConfirmed) return;

                args.Cancel = true; // скасовуємо закриття до відповіді користувача
                if (await dialogs.ConfirmExitAsync()) window.Close();
            };
        })));
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
