using System.Collections.Generic;
using System.Security.Claims;
using LU2_software_testen.Models;

namespace LU2_software_testen.Services
{
    public interface IUserService
    {
        List<UserViewModel> GetAllUsers();
        string GetCurrentUserId(ClaimsPrincipal user);
        void ChangeUserRole(string userId, string newRole);
        void DeleteUser(string userId);
    }
}