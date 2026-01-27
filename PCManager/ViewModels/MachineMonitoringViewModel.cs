
using PCManager.Repositories;
using System.Collections.ObjectModel;

namespace PCManager.ViewModels
{
    public class MachineMonitoringViewModel : BaseViewModel
    {
        private readonly WeldingDataRepository _repository;
        private readonly ulong _machineId; // 창이 닫힐 때까지 변하지 않는 주인 ID

        // DataGrid에 뿌려질 이력 목록
        public ObservableCollection<WeldingDataViewModel> WeldingHistory { get; } = new();

        private WeldingDataViewModel? _selectedItem;
        public WeldingDataViewModel? SelectedItem
        {
            get => _selectedItem;
            set
            {
                // SetProperty가 PropertyChanged 이벤트를 자동으로 발생시킵니다.
                if (SetProperty(ref _selectedItem, value) && value != null)
                {
                    // 2. 선택된 항목의 상세 데이터(BLOB)가 없으면 가져오기
                    _ = LoadDetailAsync(value);
                }
            }
        }

        public MachineMonitoringViewModel(ulong machineId)
        {
            _machineId = machineId;
            _repository = new WeldingDataRepository();
            _ = LoadSummaryListAsync(); // 생성되자마자 목록 조회 시작
        }

        private async Task LoadSummaryListAsync()
        {
            // 목록은 가볍게 (BLOB 제외) 가져옴
            var summaries = await _repository.GetSummaryByMachineIdAsync(_machineId);

            WeldingHistory.Clear();
            foreach (var s in summaries)
            {
                WeldingHistory.Add(new WeldingDataViewModel(s));
            }
        }

        private async Task LoadDetailAsync(WeldingDataViewModel item)
        {
            if (!item.IsLoaded)
            {
                await item.EnsureLoadedAsync();
                // 데이터 로드가 끝나면 item 내부의 IsLoaded가 바뀌며
                // 필요한 경우 여기서 한 번 더 알림을 줄 수 있습니다.
            }
        }
    }
}
