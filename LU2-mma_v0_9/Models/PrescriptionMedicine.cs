using System.ComponentModel.DataAnnotations.Schema;

namespace LU2_software_testen.Models
{
    [Table("prescription_medicines")]
    public class PrescriptionMedicine
    {
        public int PrescriptionID { get; set; }
        public int MedicineID { get; set; }
        public int Quantity { get; set; }
        public string Instructions { get; set; } = string.Empty;
    }
}
