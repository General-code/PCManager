using PCManager.ViewModels;
using System.Windows;
using System.Windows.Controls;


namespace PCManager
{
    /// <summary>
    /// MachineMapControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MachineMapControl : UserControl
    {
        public MachineMapControl()
        {
            InitializeComponent();
        }
        private void DragCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // DataContext가 MainWindowViewModel이라고 가정하고 캐스팅
            if (this.DataContext is MainWindowViewModel vm)
            {
                vm.CanvasWidth = e.NewSize.Width;
                vm.CanvasHeight = e.NewSize.Height;
            }
        }
    }
}
