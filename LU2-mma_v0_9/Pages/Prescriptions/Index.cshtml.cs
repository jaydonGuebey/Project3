using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LU2_software_testen.Models;
using System.Collections.Generic;
using System.Linq;

namespace Pages.Prescriptions
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<LU2_software_testen.Models.User> Users { get; set; }

        public void OnGet()
        {
            Users = _context.Users.ToList();
        }
    }
}
