using PCManager.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


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
            this.Loaded += (s, e) =>
            {
                var canvas = FindVisualChild<Canvas>(this);

                if (this.DataContext is MainWindowViewModel vm)
                {
                    vm.CanvasWidth = canvas?.ActualWidth ?? 0;
                    vm.CanvasHeight = canvas?.ActualHeight ?? 0;
                }
            };
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

        private T? FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T t)
                    return t;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }
    }
}
