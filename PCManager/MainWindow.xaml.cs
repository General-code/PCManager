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
        private const double GRID_SIZE = 40.0;
        private const double ITEM_SIZE = GRID_SIZE * 2;
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