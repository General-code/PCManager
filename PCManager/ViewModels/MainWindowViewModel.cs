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
        private const double ITEM_SIZE = GRID_SIZE * 2; // 80
        private double _canvasWidth;
        public double CanvasWidth
        {
            get => _canvasWidth;
            set { _canvasWidth = value; OnPropertyChanged(); }
        }

        private double _canvasHeight;
        public double CanvasHeight
        {
            get => _canvasHeight;
            set { _canvasHeight = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MachineViewModel> Machines { get; }

        public ICommand AddMachineCommand { get; }
        public ICommand SaveAllCommand { get; }

        public MainWindowViewModel()
        {
            _machineRepository = new MachineRepository();
            Machines = new ObservableCollection<MachineViewModel>();

            AddMachineCommand = new RelayCommand(AddMachine);
            SaveAllCommand = new RelayCommand(SaveAll);

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
        }

        private (int x, int y) FindEmptyPosition()
        {
            // 단순한 배치 알고리즘: 그리드 순서대로 빈 공간 찾기 .. 어차피 몇 개 안되서 이렇게 구현해도 상관 x 1만개 넘어가면 문제 될 수 있음
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
                // 80x80 아이템이므로 겹치는지 확인
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
    }

}
