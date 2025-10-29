using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LU2_software_testen.Models;
using System.Threading.Tasks;

namespace Pages.Medicine
{
    [Authorize(Roles = "apothecary")]
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;
        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }
        public LU2_software_testen.Models.Medicine Medicine { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Medicine = await _context.Medicines.FindAsync(id);
            if (Medicine == null)
                return NotFound();
            return Page();
        }
    }
}
