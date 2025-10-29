namespace LU2_software_testen.Models
{
    public enum StockStatus
    {
        AVAILABLE = 1,
        LIMITED = 2,
        UNAVAILABLE = 3
    }

    public static class StockStatusExtensions
    {
        public static string ToDisplayString(this StockStatus stockStatus)
        {
            return stockStatus switch
            {
                StockStatus.AVAILABLE => "Op voorraad",
                StockStatus.LIMITED => "Gelimiteerd",
                StockStatus.UNAVAILABLE => "Niet beschikbaar",
                _ => "Unknown"
            };
        }
    }
}