using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LU2_software_testen.Models;
using LU2_software_testen.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Pages.Prescriptions
{
    public class NewModel : PageModel
    {
        private readonly AppDbContext _context;
        private const string MedicinesSessionKey = "AddedMedicines";
        private const string PrescriptionDraftSessionKey = "PrescriptionDraft";

        public NewModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Prescription Prescription { get; set; } = new Prescription();

        public List<User> Patients { get; set; } = new List<User>();
        public string PrescriberName { get; set; } = string.Empty;
        public DateTime PrescriptionDate { get; set; }

        public List<AddedMedicineViewModel> AddedMedicines { get; set; } = new();

        public void OnGet()
        {
            Patients = _context.Users.OrderBy(u => u.Username).ToList();
            PrescriberName = User.Identity?.Name ?? "Unknown";
            PrescriptionDate = DateTime.Today;

            // Set default dates
            Prescription.PrescriptionStartDate = DateTime.Today;
            Prescription.PrescriptionEndDate = DateTime.Today.AddDays(1);

            // Load prescription draft from session if available
            var draftJson = HttpContext.Session.GetString(PrescriptionDraftSessionKey);
            if (!string.IsNullOrEmpty(draftJson))
            {
                var draft = JsonSerializer.Deserialize<PrescriptionDraft>(draftJson);
                if (draft != null)
                {
                    Prescription.PatientID = draft.PatientID;
                    Prescription.PrescriptionStartDate = draft.PrescriptionStartDate;
                    Prescription.PrescriptionEndDate = draft.PrescriptionEndDate;
                    Prescription.Description = draft.Description;
                }
            }

            // Load added medicines from session
            var json = HttpContext.Session.GetString(MedicinesSessionKey);
            if (!string.IsNullOrEmpty(json))
                AddedMedicines = JsonSerializer.Deserialize<List<AddedMedicineViewModel>>(json) ?? new();
        }

        public IActionResult OnPost()
        {
            Patients = _context.Users.OrderBy(u => u.Username).ToList();
            PrescriberName = User.Identity?.Name ?? "Unknown";
            PrescriptionDate = DateTime.Today;

            // Load added medicines from session
            var json = HttpContext.Session.GetString(MedicinesSessionKey);
            AddedMedicines = string.IsNullOrEmpty(json)
                ? new List<AddedMedicineViewModel>()
                : JsonSerializer.Deserialize<List<AddedMedicineViewModel>>(json);

            // Patient is mandatory
            if (Prescription.PatientID == 0)
            {
                ModelState.AddModelError("Prescription.PatientID", "No patient selected.");
            }

            // At least one medicine must be added
            if (AddedMedicines == null || !AddedMedicines.Any())
            {
                ModelState.AddModelError(string.Empty, "At least one medicine must be added to the prescription.");
            }

            // Custom validation: End date must be at least one day after start date
            if (Prescription.PrescriptionEndDate <= Prescription.PrescriptionStartDate)
            {
                ModelState.AddModelError("Prescription.PrescriptionEndDate", "The end date must be at least one day after the start date.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Prescription.PrescriberID = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            Prescription.PrescriptionDate = DateTime.Today;

            // Save prescription
            _context.Prescriptions.Add(Prescription);
            _context.SaveChanges();

            // Save medicines to prescription_medicines
            foreach (var med in AddedMedicines)
            {
                var prescriptionMedicine = new PrescriptionMedicine
                {
                    PrescriptionID = Prescription.Id,
                    MedicineID = med.MedicineID,
                    Quantity = med.Quantity,
                    Instructions = med.Instructions
                };
                _context.PrescriptionMedicines.Add(prescriptionMedicine);
            }
            _context.SaveChanges();

            // Log the creation
            var currentUser = new UserViewModel
            {
                Id = Prescription.PrescriberID.ToString(),
                Name = User.Identity?.Name ?? "Unknown"
            };
            string logMessage = $"Prescription {Prescription.Id} was created for {Prescription.PatientID}";
            Logger.Log(currentUser, logMessage);

            // Set success message
            TempData["SuccessMessage"] = "Prescription successfully created";

            // Clear session after saving
            HttpContext.Session.Remove(MedicinesSessionKey);
            HttpContext.Session.Remove(PrescriptionDraftSessionKey);

            return RedirectToPage("Index");
        }

        public IActionResult OnPostAddMedicine()
        {
            // Validate and save the draft information to the session
            var draft = new
            {
                PatientID = Prescription.PatientID,
                PrescriptionStartDate = Prescription.PrescriptionStartDate,
                PrescriptionEndDate = Prescription.PrescriptionEndDate,
                Description = Prescription.Description
            };
            HttpContext.Session.SetString(PrescriptionDraftSessionKey, JsonSerializer.Serialize(draft));

            return RedirectToPage("AddMedicineToPrescription");
        }

        // Helper class for draft
        private class PrescriptionDraft
        {
            public int PatientID { get; set; }
            public DateTime PrescriptionStartDate { get; set; }
            public DateTime PrescriptionEndDate { get; set; }
            public string Description { get; set; }
        }
    }
}
