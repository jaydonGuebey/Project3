using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LU2_software_testen.Models;
using System;
using System.Linq;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;

    public EditModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Medicine Medicine { get; set; }

    public DosageForms[] AllDosageForms { get; set; }

    public IActionResult OnGet(int id)
    {
        Medicine = _context.Medicines.Find(id);
        if (Medicine == null)
            return NotFound();

        AllDosageForms = Enum.GetValues(typeof(DosageForms)).Cast<DosageForms>().ToArray();
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            AllDosageForms = Enum.GetValues(typeof(DosageForms)).Cast<DosageForms>().ToArray();
            return Page();
        }

        // Find the existing medicine by ID
        var existingMedicine = _context.Medicines.Find(Medicine.MedicineID);
        if (existingMedicine == null)
            return NotFound();

        // Update properties
        existingMedicine.TradeName = Medicine.TradeName;
        existingMedicine.ActiveSubstances = Request.Form["Medicine.ActiveSubstances"]
            .ToString()
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .ToList();
        existingMedicine.DosageForm = Medicine.DosageForm;
        existingMedicine.Strength = Medicine.Strength;
        existingMedicine.StockStatus = Medicine.StockStatus; // Preserved via hidden field

        _context.SaveChanges();
        return RedirectToPage("Index");
    }
}