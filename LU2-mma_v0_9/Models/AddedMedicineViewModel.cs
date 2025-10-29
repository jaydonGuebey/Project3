namespace LU2_software_testen.Models
{
    public class AddedMedicineViewModel
    {
        public int MedicineID { get; set; }
        public string TradeName { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Instructions { get; set; } = string.Empty;
    }
}
