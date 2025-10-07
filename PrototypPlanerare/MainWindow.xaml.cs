using Microsoft.UI.Xaml;
using PrototypPlanerare.Views;

namespace PrototypPlanerare
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            RootFrame.Navigate(typeof(ItemsPage)); // start on the list page
        }
    }
}