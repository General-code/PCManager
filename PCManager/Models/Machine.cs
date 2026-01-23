using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCManager.Models
{
    public class Machine
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }

        public Machine() { }

        public Machine(string name, int posX, int posY)
        {
            Name = name;
            StartTime = DateTime.Now;
            PosX = posX;
            PosY = posY;
        }
    }
}
