using PCManager.Models;
using PCManager.Repositories;

namespace PCManager.ViewModels
{
    public class WeldingDataViewModel : BaseViewModel
    {
        private readonly WeldingDataRepository _repo = new();
        public WeldingData Model { get; private set; }
        public ulong Id => Model.Id;
        public DateTime CreatedAt => Model.createdAt;
        public int Count => Model.Count;

        // BLOB 데이터 (처음에는 null, 필요할 때 로딩)
        private double[]? _sinData;
        private double[]? _cosData;

        public double[]? SinData
        {   get => _sinData;
            set => SetProperty(ref _sinData, value);
        }
        public double[]? CosData
        {
            get => _cosData;
            set => SetProperty(ref _cosData, value);
        }

        public WeldingDataViewModel(WeldingData model) => Model = model;

        // 데이터가 이미 로드되었는지 확인
        public bool IsLoaded => _sinData != null && _cosData != null;

        // 상세 데이터 로드 메서드 (Lazy Loading)
        public async Task EnsureLoadedAsync()
        {
            if (IsLoaded) return;

            var fullData = await _repo.GetWeldingDataByIdAsync(this.Id);
            if (fullData != null)
            {
                SinData = fullData.sin_raw_data;
                CosData = fullData.cos_raw_data;

                OnPropertyChanged(nameof(IsLoaded));
            }
        }
    }
}
