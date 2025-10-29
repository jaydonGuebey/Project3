using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LU2_software_testen.Models; // Adjust namespace as needed
using LU2_software_testen.Services;
using LU2_software_testen.Utils; // For Logger
using System.Linq;
using System.Collections.Generic;
using System;

[Authorize(Roles = "administrator")]
public class UsersIndexModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly AppDbContext _context;

    public List<UserViewModel> Users { get; set; }
    public List<UserRole> AllRoles { get; set; }
    public string CurrentUserId { get; set; }

    public UsersIndexModel(IUserService userService, AppDbContext context)
    {
        _userService = userService;
        _context = context;
    }

    public void OnGet()
    {
        AllRoles = Enum.GetValues(typeof(UserRole)).Cast<UserRole>().ToList();
        Users = _userService.GetAllUsers();
        CurrentUserId = _userService.GetCurrentUserId(User);
    }

    public IActionResult OnPostChangeRole(string userId, string newRole)
    {
        // 1. Get the user whose role is being changed
        var user = _context.Users.FirstOrDefault(u => u.UserID.ToString() == userId);
        if (user == null) return NotFound();

        // 2. Get previous role
        var previousRole = user.UserRoleID;

        // 3. Parse new role
        if (!Enum.TryParse<UserRole>(newRole, out var newRoleEnum))
            return BadRequest();

        // 4. Only proceed if the role is actually changing
        if (previousRole == newRoleEnum)
            return RedirectToPage();

        // 5. Change the role
        user.UserRoleID = newRoleEnum;
        _context.SaveChanges();

        // 6. Get the current user (the one performing the change)
        var currentUserName = User.Identity?.Name ?? "Unknown";
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        var currentUser = new UserViewModel
        {
            Id = currentUserId,
            Name = currentUserName
        };

        // 7. Log the change
        string logMessage = $"Role of {user.UserID} was changed from {previousRole} to {newRoleEnum}";
        Logger.Log(currentUser, logMessage);

        return RedirectToPage();
    }

    public IActionResult OnPostDelete(string userId)
    {
        var currentUserId = _userService.GetCurrentUserId(User);
        if (userId != null && userId != currentUserId)
        {
            // Get the user to be deleted (for logging)
            var userToDelete = _context.Users.FirstOrDefault(u => u.UserID.ToString() == userId);

            // Delete the user
            _userService.DeleteUser(userId);

            // Get the current user (the one performing the delete)
            var currentUserName = User.Identity?.Name ?? "Unknown";
            var currentUser = new UserViewModel
            {
                Id = currentUserId,
                Name = currentUserName
            };

            // Log the deletion if the user existed
            if (userToDelete != null)
            {
                string logMessage = $"{userToDelete.UserID} was deleted";
                Logger.Log(currentUser, logMessage);
            }
        }
        return RedirectToPage();
    }
}