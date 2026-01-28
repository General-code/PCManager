using PCManager.Repositories;
using PCManager.ViewModels;
using System.Windows;

namespace PCManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int MachineCounter => ViewModel.Machines.Count;

        private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

        public MainWindow()
        {
            MachineRepository machineRepository = new MachineRepository();
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }
    }
}