using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using LU2_software_testen.Models;
using Microsoft.AspNetCore.Authorization;

namespace Pages.Medicine
{
    [Authorize(Roles = "apothecary")]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        public CreateModel(AppDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public LU2_software_testen.Models.Medicine Medicine { get; set; }
        [BindProperty]
        public string ActiveSubstancesInput { get; set; }
        public void OnGet()
        {
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

            _context.Medicines.Add(Medicine);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
