using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LU2_software_testen.Models;
using System.Threading.Tasks;

namespace Pages.Medicine
{
    [Authorize(Roles = "apothecary")]
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;
        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public LU2_software_testen.Models.Medicine Medicine { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Medicine = await _context.Medicines.FindAsync(id);
            if (Medicine == null)
                return NotFound();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var medicine = await _context.Medicines.FindAsync(Medicine.MedicineID);
            if (medicine == null)
                return NotFound();
            _context.Medicines.Remove(medicine);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
