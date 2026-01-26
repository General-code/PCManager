using PCManager.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shell;

namespace PCManager.AttachedProperty
{    /// <summary>
     /// Thumb에 드래그 기능을 추가하는 AttachedProperty
     /// </summary>

    class DraggableHelper
    {
        private const double GRID_SIZE = 40.0;
        private const double ITEM_SIZE = GRID_SIZE * 2;

        #region EnableDrag AttachedProperty

        public static readonly DependencyProperty EnableDragProperty =
            DependencyProperty.RegisterAttached(
                "EnableDrag",
                typeof(bool),
                typeof(DraggableHelper),
                new PropertyMetadata(false, OnEnableDragChanged));

        public static bool GetEnableDrag(DependencyObject obj) => (bool)obj.GetValue(EnableDragProperty);
        public static void SetEnableDrag(DependencyObject obj, bool value) => obj.SetValue(EnableDragProperty, value);

        private static void OnEnableDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Thumb thumb)
            {
                if ((bool)e.NewValue)
                {
                    thumb.Loaded += OnThumbLoaded;
                    thumb.DragStarted += OnDragStarted;
                    thumb.DragDelta += OnDragDelta;
                    thumb.DragCompleted += OnDragCompleted;
                }
                else
                {
                    thumb.Loaded -= OnThumbLoaded;
                    thumb.DragStarted -= OnDragStarted;
                    thumb.DragDelta -= OnDragDelta;
                    thumb.DragCompleted -= OnDragCompleted;
                }
            }
        }

        #endregion

        #region ParentCanvas AttachedProperty (캐싱용)

        private static readonly DependencyProperty ParentCanvasProperty =
            DependencyProperty.RegisterAttached(
                "ParentCanvas",
                typeof(Canvas),
                typeof(DraggableHelper),
                new PropertyMetadata(null));

        private static Canvas? GetParentCanvas(DependencyObject obj)
        {
            return (Canvas?)obj.GetValue(ParentCanvasProperty);
        }

        private static void SetParentCanvas(DependencyObject obj, Canvas? value)
        {
            obj.SetValue(ParentCanvasProperty, value);
        }

        #endregion

        #region ViewModel AttachedProperty (Caching)

        private static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.RegisterAttached(
                "ViewModel",
                typeof(MainWindowViewModel),
                typeof(DraggableHelper),
                new PropertyMetadata(null));

        private static MainWindowViewModel? GetViewModel(DependencyObject obj)
        {
            return (MainWindowViewModel?)obj.GetValue(ViewModelProperty);
        }

        private static void SetViewModel(DependencyObject obj, MainWindowViewModel? value)
        {
            obj.SetValue(ViewModelProperty, value);
        }

        #endregion

        #region Event Handlers

        private static void OnThumbLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                // 부모 Canvas 찾아서 캐싱
                var canvas = FindParent<Canvas>(thumb);
                SetParentCanvas(thumb, canvas);

                // ViewModel 찾아서 캐싱
                var window = FindParent<Window>(thumb);
                var viewModel = window?.DataContext as MainWindowViewModel;
                SetViewModel(thumb, viewModel);
            }
        }

        private static void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                Panel.SetZIndex(thumb, 999);
            }
        }

        private static void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not Thumb thumb) return;

            var canvas = GetParentCanvas(thumb);
            if (canvas == null) return;

            // parent Cavas찾았는데 이미 ..
            FrameworkElement elementToMove = thumb;
            DependencyObject parent = VisualTreeHelper.GetParent(thumb);
            while (parent != null && parent != canvas)
            {
                if (parent is FrameworkElement fe)
                    elementToMove = fe;

                parent = VisualTreeHelper.GetParent(parent);
            }

            double currentLeft = Canvas.GetLeft(elementToMove);
            double currentTop = Canvas.GetTop(elementToMove);

            // NaN 처리 DB 상 그럴일은 없는데 .. 불안
            if (double.IsNaN(currentLeft)) currentLeft = 0;
            if (double.IsNaN(currentTop)) currentTop = 0;

            double newLeft = currentLeft + e.HorizontalChange;
            double newTop = currentTop + e.VerticalChange;

            // 그리드에 스냅
            newLeft = Math.Round(newLeft / GRID_SIZE) * GRID_SIZE;
            newTop = Math.Round(newTop / GRID_SIZE) * GRID_SIZE;

            // 캔버스 경계 체크
            newLeft = Math.Max(0, Math.Min(newLeft, canvas.ActualWidth - ITEM_SIZE));
            newTop = Math.Max(0, Math.Min(newTop, canvas.ActualHeight - ITEM_SIZE));

            // 충돌 감지
            var machineVM = thumb.DataContext as MachineViewModel;
            var viewModel = GetViewModel(thumb);

            if (machineVM != null && viewModel != null)
            {
                if (viewModel.CanPlaceAt((int)newLeft, (int)newTop, machineVM))
                {
                    Canvas.SetLeft(elementToMove, newLeft);
                    Canvas.SetTop(elementToMove, newTop);
                    //Canvas.SetLeft(thumb, newLeft);
                    //Canvas.SetTop(thumb, newTop);
                }
            }
            else
            {
                Canvas.SetLeft(thumb, newLeft);
                Canvas.SetTop(thumb, newTop);
            }
        }

        private static void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender is not Thumb thumb) return;

            Panel.SetZIndex(thumb, 0);

            // ViewModel 업데이트
            if (thumb.DataContext is MachineViewModel machineVM)
            {
                double left = Canvas.GetLeft(thumb);
                double top = Canvas.GetTop(thumb);

                if (!double.IsNaN(left) && !double.IsNaN(top))
                {
                    machineVM.PosX = (int)left;
                    machineVM.PosY = (int)top;
                }
            }
        }

        #endregion

        private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject? parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            if (parentObject is T parent)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

    }
}
