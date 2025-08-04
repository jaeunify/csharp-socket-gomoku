using System.Windows;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using EchoWpfBlazorClient.Pages;

namespace EchoWpfBlazorClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            blazorView.Services = App.Current.Services;
            blazorView.RootComponents.Add(new RootComponent()
            {
                Selector = "#app",
                ComponentType = typeof(Main)
            });
        }
    }
}
