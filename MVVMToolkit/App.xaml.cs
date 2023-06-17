using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MVVMToolkit.Models;
using MVVMToolkit.Services;
using MVVMToolkit.ViewModels;
using MVVMToolkit.Views;

namespace MVVMToolkit;

public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        //Dependency Injection set up using Microsoft.Extensions.DependencyInjection nugget package
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<MainWindow>(provider => new MainWindow
                {
                    DataContext = provider.GetRequiredService<MainWindowViewModel>()
                });
                services.AddSingleton<LoginViewModel>();
                services.AddSingleton<DatabaseViewModel>();
                services.AddSingleton<DatabaseModel>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<Func<Type, BaseViewModel>>(serviceProvider => viewModelType => (BaseViewModel)serviceProvider.GetRequiredService(viewModelType));
                services.AddSingleton<IDatabaseConnectionService, DatabaseConnectionService>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();
        var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
        startupForm.Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        base.OnExit(e);
    }
}