using System.Collections.ObjectModel;
using System.Windows;

namespace PCManager.Views
{
    public class DataItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public string Description { get; set; }
    }

    public partial class MachineMonitoringWindow : Window
    {
        // [최적화 1] 링버퍼: 고정된 크기의 배열을 딱 한 번만 만듭니다. (새 배열 생성 X)
        private readonly double[] dataSin = new double[1000];
        private readonly double[] dataCos = new double[1000];

        public ObservableCollection<DataItem> DataItems { get; set; }

        private int nextIndex = 0;              // 데이터를 채워넣을 현재 위치
        private double theta = 0;               // 수식에 사용할 각도 변수
        ScottPlot.Plottables.Signal mySignal;

        // [최적화 2] Signal 객체를 미리 선언해둡니다.
        private ScottPlot.Plottables.Signal mySignalSin;
        private ScottPlot.Plottables.Signal mySignalCos;
        private ScottPlot.Plottables.VerticalLine vLine;

        public MachineMonitoringWindow()
        {
            InitializeComponent();

            SetUpChart();
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
            vLine.LineWidth = 2;

            // X 축은 배열 크기 1000 Y축은 파형 범위(-1.5, 1.5)로 고정
            BasicPlot.Plot.Axes.SetLimits(0, 1000, -1.5, 1.5);
            NextPlot.Plot.Axes.SetLimits(0, 1000, -1.5, 1.5);
            BasicPlot.Plot.ShowLegend();
            NextPlot.Plot.ShowLegend();
        }

        private void GenerateLargeDummy()
        {
            for (int i = 0; i < dataSin.Length; i++)
            {
                theta += Random.Shared.NextDouble() * 0.1;
                dataSin[i] = Math.Sin(3 * theta) / 3 + Math.Sin(theta * 2) / 4;
                dataCos[i] = Math.Cos(theta) + Math.Sin(theta * 2) / 2.0;
            }

            //WeldingData data = new WeldingData(dataSin, dataCos);
            //WeldingDataRepository repo = new WeldingDataRepository();
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

        private ObservableCollection<DataItem> GenerateLargeData(int count)
        {
            var data = new ObservableCollection<DataItem>();
            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                data.Add(new DataItem
                {
                    Id = i + 1,
                    Name = $"Item {i + 1}",
                    Value = random.Next(1, 1000),
                    Description = $"This is a sample description for item {i + 1}."
                });
            }
            return data;
        }

    }
}
