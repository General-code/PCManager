using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCManager.Models
{
    public class WeldingData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        long ID { get; set; }
        public DateTime createdAt { get; }
        public double[] SinData { get; }
        public double[] CosData { get; }
        public int Count { get; set; }

        // DB 에서 조회할 때 필요
        public WeldingData() { }

        public WeldingData(double[] sinData, double[] cosData)
        {
            createdAt = DateTime.UtcNow;
            this.SinData = sinData;
            this.CosData = cosData;
            Count = sinData?.Length ?? 0;
        }
    }
}
