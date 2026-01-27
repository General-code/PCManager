using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCManager.Models
{
    [Table("machine")]
    public class Machine
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
        public ulong Id { get; set; }
        [Column("name"), Required]
        public string Name { get; set; }
        [Column("start_time")]
        public DateTime StartTime { get; set; }
        [Column("pos_x"), Required]
        public int PosX { get; set; }
        [Column("pos_y"), Required]
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
