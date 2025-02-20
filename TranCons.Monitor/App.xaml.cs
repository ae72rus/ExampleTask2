using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TranCons.EventListener.TCP;
using TranCons.EventProcessors.Implementation;
using TranCons.Monitor.ViewModels;
using TranCons.Monitor.Windows;

namespace TranCons.Monitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var configuration = new ConfigurationBuilder()
                .AddCommandLine(e.Args)
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);

            var portStr = configuration["Server:Port"];
            var ipAddress = configuration["Server:Address"];
            if (string.IsNullOrWhiteSpace(portStr))
                throw new InvalidOperationException("Server Port is not set. use appsettings.json to set server port (key: Server:Port)");
            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new InvalidOperationException("Server IP address is not set. use appsettings.json to set server IP address (key: Server:Address)");
            services.AddTcpEventListener(x =>
            {
                x.ServerPort = int.Parse(portStr);
                x.ServerAddress = ipAddress;
            });

            services.AddBasicEventProcessors();

            var logger = LoggerFactory.Create(x =>
            {
                x.AddDebug();
#if DEBUG
                x.SetMinimumLevel(LogLevel.Debug);
#else
                x.SetMinimumLevel(LogLevel.Error);
#endif
            }).CreateLogger("Monitor Module logger");

            services.AddSingleton(logger);
            services.AddSingleton<MonitorWindowViewModel>();

            var serviceProvider = services.BuildServiceProvider();


            var mainWindow = new MonitorWindow
            {
                DataContext = serviceProvider.GetService<MonitorWindowViewModel>()
            };

            MainWindow = mainWindow;
            mainWindow.Show();
        }
    }
}
