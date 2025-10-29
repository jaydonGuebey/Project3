using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LU2_software_testen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty, Required]
        public string Username { get; set; }

        [BindProperty, Required]
        public string Password { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var passwordHash = HashPassword(Password);
            var user = _context.Users.FirstOrDefault(u => u.Username == Username && u.PasswordHash == passwordHash);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim("UserRole", user.UserRoleID.ToString()),
                    new Claim(ClaimTypes.Role, user.UserRoleID.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirect based on role
                if (user.UserRoleID == UserRole.patient)
                {
                    return RedirectToPage("/Prescriptions/MyPrescriptions");
                }
                else
                {
                    return RedirectToPage("/Index");
                }
            }
            else
            {
                return RedirectToPage("/Index", new { loginError = true });
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}