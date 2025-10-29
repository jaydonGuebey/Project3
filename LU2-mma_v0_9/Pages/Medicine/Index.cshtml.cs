using LU2_software_testen.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Pages.Medicine
{
    [Authorize(Roles = "apothecary")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        public IndexModel(AppDbContext context)
        {
            _context = context;
        }
        public IList<LU2_software_testen.Models.Medicine> Medicines { get; set; }
        public async Task OnGetAsync()
        {
            Medicines = await _context.Medicines.AsNoTracking().ToListAsync();
        }
    }
}
