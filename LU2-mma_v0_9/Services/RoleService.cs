using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LU2_software_testen.Models;

namespace LU2_software_testen.Services
{
    public class RoleService : IRoleService
    {
        public Task<List<string>> GetAllRolesAsync()
        {
            var roles = System.Enum.GetNames(typeof(UserRole)).ToList();
            return Task.FromResult(roles);
        }

        public Task<bool> RoleExistsAsync(string roleName)
        {
            var exists = System.Enum.TryParse(typeof(UserRole), roleName, out _);
            return Task.FromResult(exists);
        }

        public Task<bool> CreateRoleAsync(string roleName)
        {
            // Roles are defined in the UserRole enum, so you can't add new ones at runtime.
            return Task.FromResult(false);
        }

        public Task<bool> DeleteRoleAsync(string roleName)
        {
            // Roles are defined in the UserRole enum, so you can't remove them at runtime.
            return Task.FromResult(false);
        }
    }
}