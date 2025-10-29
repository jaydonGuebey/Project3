using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LU2_software_testen.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        [Required]
        public UserRole UserRoleID { get; set; }
    }
}