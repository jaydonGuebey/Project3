namespace LU2_software_testen.Models
{
    public enum UserRole
    {
        apothecary = 1,
        administrator = 2,
        healthInsurer = 3,
        patient = 4,
        generalPractitioner = 5,
        specialist = 6
    }

    public static class UserRoleExtensions
    {
        public static string ToDisplayString(this UserRole role)
        {
            return role switch
            {
                UserRole.apothecary => "Apotheek",
                UserRole.administrator => "Administrator",
                UserRole.healthInsurer => "Verzekeraar",
                UserRole.patient => "Patient",
                UserRole.generalPractitioner => "Huisarts",
                UserRole.specialist => "Specialist",
                _ => "Unknown"
            };
        }
    }
}