using System.Collections.Generic;
using System.Threading.Tasks;

namespace LU2_software_testen.Services
{
    public interface IRoleService
    {
        Task<List<string>> GetAllRolesAsync();
        Task<bool> RoleExistsAsync(string roleName);
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(string roleName);
    }
}