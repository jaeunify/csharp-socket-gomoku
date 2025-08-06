using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebView.Wpf;

namespace GomokuClient
{
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            services.AddWpfBlazorWebView();
#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif
            // DI
            services.AddSingleton<ServerClient>();
            services.AddSingleton<LogState>();

            Services = services.BuildServiceProvider();
            base.OnStartup(e);
        }
    }
}