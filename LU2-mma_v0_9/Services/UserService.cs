using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using LU2_software_testen.Models;

namespace LU2_software_testen.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public List<UserViewModel> GetAllUsers()
        {
            return _context.Users.Select(user => new UserViewModel
            {
                Id = user.UserID.ToString(),
                Name = user.Username,
                Email = user.Email,
                Role = user.UserRoleID.ToString()
            }).ToList();
        }

        public string GetCurrentUserId(ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public void ChangeUserRole(string userId, string newRole)
        {
            if (!int.TryParse(userId, out var id)) return;
            if (!System.Enum.TryParse<UserRole>(newRole, out var role)) return;

            var user = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (user == null) return;

            user.UserRoleID = role;
            _context.SaveChanges();
        }

        public void DeleteUser(string userId)
        {
            if (!int.TryParse(userId, out var id)) return;

            var user = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}