using PCManager.AttachedProperty;
using PCManager.Repositories;
using PCManager.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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
            DataContext = new MainWindowViewModel();
            InitializeComponent();
            InitGridMachines();

            // 시험용으로 작성한 것 데이터가 없어서 초기 값만든 것
            //machineRepository.Save(new Models.Machine(name: "PC-1", posX: 160, posY: 220));
        }

        private void InitGridMachines()
        {
            List<MachineViewModel> machines = ViewModel.Machines?.ToList() ?? new List<MachineViewModel>();

            // 리소스로 정의한 템플릿 찾아두기
            var pcTemplate = (ControlTemplate)this.FindResource("PCItem");

            foreach (var machineVM in machines)
            {
                Thumb thumb = new Thumb
                {
                    Width = 80,
                    Height = 80,
                    Template = pcTemplate,
                    DataContext = machineVM,
                };


                DraggableHelper.SetEnableDrag(thumb, true);
                Canvas.SetLeft(thumb, machineVM.PosX);
                Canvas.SetTop(thumb, machineVM.PosY);

                DragCanvas.Children.Add(thumb);
            }
        }


        private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.DataContext is MainWindowViewModel vm)
            {
                vm.CanvasWidth = e.NewSize.Width;
                vm.CanvasHeight = e.NewSize.Height;
            }
        }
    }
}