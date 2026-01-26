using PCManager.Models;

namespace PCManager.ViewModels
{
    public class MachineViewModel : BaseViewModel
    {
        public string Name => Model.Name;
        private int _posX;
        private int _posY;

        public string Status
        {
            get
            {
                var elapsed = DateTime.Now - Model.StartTime;
                return $"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
            }
        }

        public MachineViewModel(Machine model)
        {
            Model = model;
            PosX = model.PosX;
            PosY = model.PosY;
        }

        public Machine Model { get; }

        public int PosX
        {
            get => Model.PosX;
            set
            {
                if(SetProperty(ref _posX, value))
                    Model.PosX = value;
            }
        }

        public int PosY
        {
            get => Model.PosY;
            set
            {
                if (SetProperty(ref _posY, value))
                    Model.PosY = value;
            }
        }
    }
}
