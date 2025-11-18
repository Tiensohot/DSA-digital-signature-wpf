using System.Windows;
using CryptoLib.Services;
using DsaWpfApp.ViewModels;

namespace DsaWpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(new DsaService());
        }
    }
}
