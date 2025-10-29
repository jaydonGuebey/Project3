using System.Security.Claims;

namespace LU2_software_testen.Models
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsInUserRole(this ClaimsPrincipal user, UserRole role)
        {
            var roleClaim = user.FindFirst("UserRole")?.Value;
            var test = -1;

            if (roleClaim != null) {
                // Successfully retrieved the role claim
                // Converting string to the right Enum value and then get the int value
                test = (int) (UserRole) Enum.Parse(typeof(UserRole), roleClaim);
            }
            else
            {
                // Role claim not found
                return false;
            }
            
            return roleClaim != null && test == (int)role;
        }
    }
}