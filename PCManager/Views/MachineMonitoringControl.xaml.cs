using PCManager.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PCManager.Views
{
    /// <summary>
    /// MachineMonitoringControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MachineMonitoringControl : UserControl
    {
        private MachineMonitoringViewModel? _viewModel;
        private static readonly HashSet<string> ExcludedProperties = new()
        {
            "SinData", "CosData", "IsLoaded", "Model", "OtherHiddenField"
        };
        // [최적화 1] 링버퍼: 고정된 크기의 배열을 딱 한 번만 만듭니다. (새 배열 생성 X)
        private readonly double[] dataSin = new double[1000];
        private readonly double[] dataCos = new double[1000];

        private int nextIndex = 0;              // 데이터를 채워넣을 현재 위치
        private double theta = 0;               // 수식에 사용할 각도 변수
        ScottPlot.Plottables.Signal mySignal;

        // [최적화 2] Signal 객체를 미리 선언해둡니다.
        private ScottPlot.Plottables.Signal mySignalSin;
        private ScottPlot.Plottables.Signal mySignalCos;
        private ScottPlot.Plottables.VerticalLine vLine;

        public MachineMonitoringControl()
        {
            InitializeComponent();

            this.Loaded += OnControlLoaded;
            this.Unloaded += OnControlUnloaded;
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            // Loaded가 호출되는 시점에는 부모가 DataContext를 꽂아준 상태
            if (this.DataContext is MachineMonitoringViewModel vm)
            {
                _viewModel = vm; // 이제야 비로소 연결!

                // 데이터가 확실히 있으니 그리드에 연결
                WeldingGrid.ItemsSource = _viewModel.WeldingHistory;

                // 차트 설정 및 렌더링 시작
                SetUpChart();
                CompositionTarget.Rendering += OnRendering;
            }
        }

        private void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            // 메모리 누수 방지
            CompositionTarget.Rendering -= OnRendering;
        }

        private void SetUpChart()
        {
            mySignalSin = BasicPlot.Plot.Add.Signal(dataSin);
            mySignalSin.LegendText = "Current (Sin)";
            mySignalSin.Color = ScottPlot.Colors.DodgerBlue;
            BasicPlot.Plot.DataBackground.Color = ScottPlot.Colors.WhiteSmoke;

            mySignalCos = NextPlot.Plot.Add.Signal(dataCos);
            mySignalCos.LegendText = "Voltage (Cos)";
            mySignalCos.Color = ScottPlot.Colors.FloralWhite;
            NextPlot.Plot.DataBackground.Color = ScottPlot.Colors.DimGray;

            vLine = BasicPlot.Plot.Add.VerticalLine(0);
            vLine.Color = ScottPlot.Colors.Blue;
            vLine.LineWidth = 1;

            // X 축은 배열 크기 1000 Y축은 파형 범위(-1.5, 1.5)로 고정
            BasicPlot.Plot.Axes.SetLimits(0, 1000, -1.5, 1.5);
            NextPlot.Plot.Axes.SetLimits(0, 1000, -1.5, 1.5);
            BasicPlot.Plot.ShowLegend();
            NextPlot.Plot.ShowLegend();
        }

        private void OnRendering(object sender, EventArgs e)
        {
            // [최적화 4] 더미 데이터 생성 및 링버퍼 쓰기
            // 한 번 그릴 때마다 5개씩 데이터를 채워 넣어 속도감을 준다.
            for (int i = 0; i < 5; i++)
            {
                theta += Random.Shared.NextDouble() * 0.1;
                dataSin[nextIndex] = Math.Sin(3 * theta) / 3 + Math.Sin(theta * 2) / 4;
                dataCos[nextIndex] = Math.Cos(theta) + Math.Sin(theta * 2) / 2.0;

                nextIndex = (nextIndex + 1) % dataSin.Length;
            }

            vLine.X = nextIndex;
            BasicPlot.Refresh();
            NextPlot.Refresh();
        }

        private void WeldingGrid_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(double[]))
            {
                e.Cancel = true;
                return;
            }

            if (ExcludedProperties.Contains(e.PropertyName))
            {
                e.Cancel = true;
                return;
            }

            e.Column.Header = e.PropertyName switch
            {
                "Id" => "ID",
                "machineId" => "Machine ID",
                "CreatedAt" => "Created At",
                "Count" => "Data Count",
                _ => e.PropertyName
            };

            if (String.Compare(e.PropertyName, "createdAt", ignoreCase: true) == 0)
            {
                e.Column.Width = DataGridLength.SizeToCells;
                return;
            }

            e.Column.Width = DataGridLength.SizeToHeader;
        }
    }
}
