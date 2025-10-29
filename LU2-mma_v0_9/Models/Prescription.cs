using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LU2_software_testen.Models
{
    [Table("prescriptions")]
    public class Prescription
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("patientID")]
        public int PatientID { get; set; }

        [Column("prescriberID")]
        public int PrescriberID { get; set; }

        [Column("prescription_date")]
        [DataType(DataType.Date)]
        public DateTime PrescriptionDate { get; set; }

        [Column("prescription_start_date")]
        [DataType(DataType.Date)]
        public DateTime PrescriptionStartDate { get; set; }

        [Column("prescription_end_date")]
        [DataType(DataType.Date)]
        public DateTime PrescriptionEndDate { get; set; }

        [Column("description")]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}