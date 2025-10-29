using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LU2_software_testen.Models;
using System.Threading.Tasks;
using System.Linq;
using LU2_software_testen.Utils; // Add this using for Logger
using System.Security.Cryptography; // <-- Add this line
using System.Text; // <-- Add this line

namespace Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty, Required, MaxLength(100)]
        public string Username { get; set; }

        [BindProperty, Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; }

        [BindProperty, Required, MinLength(6)]
        public string Password { get; set; }

        [BindProperty, Required]
        public string PhoneNumber { get; set; }

        [BindProperty, Required]
        public string Country { get; set; } // Add a dropdown for country in the form

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (_context.Users.Any(u => u.Username == Username))
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return Page();
            }
            if (_context.Users.Any(u => u.Email == Email))
            {
                ModelState.AddModelError("Email", "Email already registered.");
                return Page();
            }

            if (!IsValidPhoneNumber(PhoneNumber, Country))
            {
                ModelState.AddModelError(nameof(PhoneNumber), "Invalid phone number for selected country.");
                return Page();
            }

            var user = new User
            {
                Username = Username,
                Email = Email,
                PasswordHash = HashPassword(Password),
                PhoneNumber = PhoneNumber,
                UserRoleID = UserRole.patient // Set default role to 'patient'
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create a UserViewModel for logging
            var userViewModel = new UserViewModel
            {
                Id = user.UserID.ToString(),
                Name = user.Username,
                Email = user.Email,
                Role = user.UserRoleID.ToString()
            };

            // Log the registration
            Logger.Log(userViewModel, "has registered");

            // Redirect or return as needed
            return RedirectToPage("/Index");
        }

        private bool IsValidPhoneNumber(string phoneNumber, string country)
        {
            // Remove spaces, dashes, etc.
            var cleaned = Regex.Replace(phoneNumber, @"[\s\-()]", "");

            switch (country)
            {
                case "Netherlands":
                    // Dutch numbers: +31 or 0, 9 or 10 digits after country code
                    return Regex.IsMatch(cleaned, @"^(?:\+31|0)6?\d{8}$");
                case "Belgium":
                    // Belgian numbers: +32 or 0, 9 digits after country code
                    return Regex.IsMatch(cleaned, @"^(?:\+32|0)\d{8,9}$");
                case "Germany":
                    // German numbers: +49 or 0, 10-11 digits after country code
                    return Regex.IsMatch(cleaned, @"^(?:\+49|0)\d{10,11}$");
                case "UK":
                case "United Kingdom":
                    // UK numbers: +44 or 0, 10 digits after country code
                    return Regex.IsMatch(cleaned, @"^(?:\+44|0)7\d{9}$");
                default:
                    return false;
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