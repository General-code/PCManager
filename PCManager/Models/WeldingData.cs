using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCManager.Models
{
    [Table("machine_welding_history")]
    public class WeldingData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [Column("count")]
        public int Count { get; set; }

        [Column("machine_id")]
        public ulong machineId { get; set; }

        [Column("created_at")]
        public DateTime createdAt { get; set; } = DateTime.Now;

        [Column("sin_raw_data")]
        public double[] sin_raw_data { get; set; }

        [Column("cos_raw_data")]
        public double[] cos_raw_data { get; set; }

        // DB 에서 조회할 때 필요
        public WeldingData() { }

        public WeldingData(ulong machineId, double[] sinData, double[] cosData)
        {
            createdAt = DateTime.Now;
            this.machineId = machineId;
            this.sin_raw_data = sinData;
            this.cos_raw_data = cosData;
            Count = sinData?.Length ?? 0;
        }
    }
}
