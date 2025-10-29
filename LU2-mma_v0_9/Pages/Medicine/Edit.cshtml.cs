using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LU2_software_testen.Models;

namespace Pages.Medicine
{
    [Authorize(Roles = "apothecary")]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        public EditModel(AppDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public LU2_software_testen.Models.Medicine Medicine { get; set; }
        [BindProperty]
        public string ActiveSubstancesInput { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Medicine = await _context.Medicines.FindAsync(id);
            if (Medicine == null)
                return NotFound();
            ActiveSubstancesInput = string.Join(", ", Medicine.ActiveSubstances);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Medicine.ActiveSubstances = ActiveSubstancesInput?
               .Split(',')
               .Select(s => s.Trim())
               .Where(s => !string.IsNullOrEmpty(s))
               .ToList() ?? new System.Collections.Generic.List<string>();

            // Manual validation
            if (Medicine.ActiveSubstances == null || !Medicine.ActiveSubstances.Any())
            {
                ModelState.AddModelError("ActiveSubstancesInput", "At least one active substance is required.");
            }
            else
            {
                // Remove any previous error for this field
                ModelState.Remove("Medicine.ActiveSubstances");
                ModelState.Remove("ActiveSubstances");
            }

            if (!ModelState.IsValid)
                return Page();

            _context.Attach(Medicine).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
