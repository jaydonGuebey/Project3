using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LU2_software_testen.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Pages.Prescriptions
{
    public class AddMedicineToPrescriptionModel : PageModel
    {
        private readonly AppDbContext _context;
        private const string MedicinesSessionKey = "AddedMedicines";

        public AddMedicineToPrescriptionModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int SelectedMedicineId { get; set; }

        [BindProperty]
        public int Quantity { get; set; }

        [BindProperty]
        public string Instructions { get; set; }

        public List<LU2_software_testen.Models.Medicine> Medicines { get; set; }

        public void OnGet()
        {
            Medicines = _context.Medicines.ToList();
        }

        public IActionResult OnPost(string action)
        {
            Medicines = _context.Medicines.ToList();

            if (action == "cancel")
            {
                // Skip validation and redirect immediately
                return RedirectToPage("New");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var med = Medicines.FirstOrDefault(m => m.MedicineID == SelectedMedicineId);
            if (med != null)
            {
                var addedMedicine = new AddedMedicineViewModel
                {
                    MedicineID = med.MedicineID,
                    TradeName = med.TradeName,
                    Strength = med.Strength,
                    Quantity = Quantity,
                    Instructions = Instructions
                };

                var json = HttpContext.Session.GetString(MedicinesSessionKey);
                var list = string.IsNullOrEmpty(json)
                    ? new List<AddedMedicineViewModel>()
                    : JsonSerializer.Deserialize<List<AddedMedicineViewModel>>(json);

                list.Add(addedMedicine);
                HttpContext.Session.SetString(MedicinesSessionKey, JsonSerializer.Serialize(list));
            }

            return RedirectToPage("New");
        }
    }
}
