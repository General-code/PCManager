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
        private int _pcCounter = 1;

        private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

        public MainWindow()
        {
            MachineRepository machineRepository = new MachineRepository();
            DataContext = new MainWindowViewModel();
            InitializeComponent();

            // 시험용으로 작성한 것 데이터가 없어서 초기 값만든 것
            //machineRepository.Save(new Models.Machine(name: "PC-1", posX: 160, posY: 220));
        }

        private void HandleDrag(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb is null) return;

            double currentLeft = Canvas.GetLeft(thumb);
            double currentTop = Canvas.GetTop(thumb);

            currentLeft = (double.IsNaN(currentLeft)) ? 0 : currentLeft;
            currentTop = (double.IsNaN(currentTop)) ? 0 : currentTop;

            double newLeft = currentLeft + e.HorizontalChange;
            double newTop = currentTop + e.VerticalChange;

            newLeft = Math.Round(newLeft / GRID_SIZE) * GRID_SIZE;
            newTop = Math.Round(newTop / GRID_SIZE) * GRID_SIZE;

            if (newLeft >= 0 && newLeft <= DragCanvas.ActualWidth - thumb.ActualWidth)
                Canvas.SetLeft(thumb, newLeft);

            if (newTop >= 0 && newTop <= DragCanvas.ActualHeight - thumb.ActualHeight)
                Canvas.SetTop(thumb, newTop);
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