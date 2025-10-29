using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LU2_software_testen.Models
{
    [Table("Medicines")]
    public class Medicine
    {
        [Key]
        public int MedicineID { get; set; }

        [Required]
        public string TradeName { get; set; } = string.Empty;

        [Required]
        public List<string> ActiveSubstances { get; set; } = new List<string>();

        [Required]
        public StockStatus StockStatus { get; set; }

        [Required]
        public DosageForms DosageForm { get; set; }

        [Required]
        public string Strength { get; set; } = string.Empty;
    }
}
