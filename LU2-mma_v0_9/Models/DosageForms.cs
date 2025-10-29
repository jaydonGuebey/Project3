namespace LU2_software_testen.Models
{
    public enum DosageForms
    {
        TABLET = 1,
        CAPSULE = 2,
        INJECTION = 3,
        SALVE = 4
    }

    public static class DosageFormsExtensions
    {
        public static string ToDisplayString(this DosageForms dosageForm)
        {
            return dosageForm switch
            {
                DosageForms.TABLET => "Tablet",
                DosageForms.CAPSULE => "Capsule",
                DosageForms.INJECTION => "Injectie",
                DosageForms.SALVE => "Zalf",
                _ => "Unknown"
            };
        }
    }
}