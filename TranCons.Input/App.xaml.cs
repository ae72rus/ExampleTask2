using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using TranCons.EventEmitter.TCP;
using TranCons.Input.ViewModels;
using TranCons.Input.Windows;

namespace TranCons.Input
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
            if (string.IsNullOrWhiteSpace(portStr))
                throw new InvalidOperationException("Server Port is not set. use appsettings.json to set server port (key: Server:Port)");
            services.AddTcpEventEmitter(x => x.ServerPort = int.Parse(portStr));

            var logger = LoggerFactory.Create(x =>
            {
                x.AddDebug();
#if DEBUG
                x.SetMinimumLevel(LogLevel.Debug);
#else
                x.SetMinimumLevel(LogLevel.Error);
#endif
            }).CreateLogger("Input Module logger");

            services.AddSingleton(logger);
            services.AddSingleton<InputWindowViewModel>();

            var serviceProvider = services.BuildServiceProvider();


            var mainWindow = new InputWindow
            {
                DataContext = serviceProvider.GetService<InputWindowViewModel>()
            };

            MainWindow = mainWindow;
            mainWindow.Show();
        }
    }
}
