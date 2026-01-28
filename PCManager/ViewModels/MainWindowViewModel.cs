using PCManager.Command;
using PCManager.Models;
using PCManager.Repositories;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PCManager.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly MachineRepository _machineRepository;
        private const double GRID_SIZE = 40.0;
        private const double ITEM_SIZE = GRID_SIZE * 2;

        private double _canvasWidth = 0;
        private double _canvasHeight = 0;

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public double CanvasWidth
        {
            get => _canvasWidth;
            set => SetProperty(ref _canvasWidth, value);
        }
        public double CanvasHeight
        {
            get => _canvasHeight;
            set => SetProperty(ref _canvasHeight, value);
        }

        public ObservableCollection<MachineViewModel> Machines { get; }

        public ICommand AddMachineCommand { get; }
        public ICommand SaveAllCommand { get; }
        public ICommand OpenMonitoringCommand { get; }

        public MainWindowViewModel()
        {
            CurrentView = this;
            _machineRepository = new MachineRepository();
            Machines = new ObservableCollection<MachineViewModel>();

            SaveAllCommand = new RelayCommand(SaveAll);
            AddMachineCommand = new RelayCommand(AddMachine);
            OpenMonitoringCommand = new RelayCommand<MachineViewModel>(OnOpenMonitoring);
            LoadMachines();
        }

        private void LoadMachines()
        {
            var machines = _machineRepository.GetAll();
            foreach (var machine in machines)
            {
                Machines.Add(new MachineViewModel(machine));
            }
        }

        private void AddMachine()
        {
            // 빈 공간 찾기
            var (x, y) = FindEmptyPosition();

            var newMachine = new Machine(
                name: $"PC-{Machines.Count + 1}",
                posX: x,
                posY: y
            );

            var viewModel = new MachineViewModel(newMachine);
            Machines.Add(viewModel);

            // 즉시 DB에 저장
            _machineRepository.Save(newMachine);
            viewModel.Model.Id = _machineRepository.GetByName(newMachine.Name).Id;
        }

        private (int x, int y) FindEmptyPosition()
        {
            for (int y = 0; y < CanvasHeight; y += (int)GRID_SIZE)
            {
                for (int x = 0; x < CanvasWidth; x += (int)GRID_SIZE)
                {
                    if (!IsPositionOccupied(x, y))
                    {
                        return (x, y);
                    }
                }
            }

            return (0, 0); // 기본값
        }

        public bool IsPositionOccupied(int x, int y)
        {
            foreach (var machine in Machines)
            {
                if (IsOverlapping(x, y, machine.PosX, machine.PosY))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanPlaceAt(int x, int y, MachineViewModel? excludeVM = null)
        {
            foreach (var machine in Machines)
            {
                if (machine == excludeVM) continue;

                if (IsOverlapping(x, y, machine.PosX, machine.PosY))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsOverlapping(int x1, int y1, int x2, int y2)
        {
            return !(x1 + (int)ITEM_SIZE <= x2 ||
                     x2 + (int)ITEM_SIZE <= x1 ||
                     y1 + (int)ITEM_SIZE <= y2 ||
                     y2 + (int)ITEM_SIZE <= y1);
        }

        private void SaveAll()
        {
            var models = Machines.Select(vm => vm.Model).ToList();
            bool success = _machineRepository.SaveAll(models);

            if (success)
            {
                MessageBox.Show("모든 위치가 저장되었습니다.", "저장 완료");
            }
            else
            {
                MessageBox.Show("저장에 실패했습니다.", "저장 실패");
            }
        }

        #region MonitoringWindow 관련
        private void OnOpenMonitoring(MachineViewModel machine)
        {
            if (machine == null) return;

            var monitoringVM = new MachineMonitoringViewModel(machine.Id);
            monitoringVM.BackRequested += (s, e) => CurrentView = this;
            CurrentView = monitoringVM;

        }

        public void CloseMonitoring()
        {
            CurrentView = this; // 다시 나(지도 로직)로 돌아오면 끝!
        }

        #endregion
    }
}
