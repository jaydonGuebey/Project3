using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LU2_software_testen.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pages.Prescriptions
{
    [Authorize(Roles = "patient")]
    public class MyPrescriptionsModel : PageModel
    {
        public List<Prescription> CurrentPrescriptions { get; set; }
        // Maps PrescriptionID to a list of medicine details for that prescription
        public Dictionary<int, List<MedicineInfo>> Medicines { get; set; }
        private readonly AppDbContext _context;

        public MyPrescriptionsModel(AppDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var today = DateTime.Today;

            CurrentPrescriptions = _context.Prescriptions
                .Where(p => p.PatientID == userId
                    && p.PrescriptionStartDate <= today
                    && p.PrescriptionEndDate >= today)
                .ToList();

            // Build the Medicines dictionary
            var prescriptionIds = CurrentPrescriptions.Select(p => p.Id).ToList();

            var prescriptionMedicines = _context.PrescriptionMedicines
                .Where(pm => prescriptionIds.Contains(pm.PrescriptionID))
                .ToList();

            var medicineIds = prescriptionMedicines.Select(pm => pm.MedicineID).Distinct().ToList();

            var medicines = _context.Medicines
                .Where(m => medicineIds.Contains(m.MedicineID))
                .ToList();

            Medicines = prescriptionIds.ToDictionary(
                pid => pid,
                pid => (from pm in prescriptionMedicines
                        where pm.PrescriptionID == pid
                        join m in medicines on pm.MedicineID equals m.MedicineID
                        select new MedicineInfo
                        {
                            TradeName = m.TradeName,
                            Strength = m.Strength,
                            Instructions = pm.Instructions
                        }).ToList()
            );
        }

        public class MedicineInfo
        {
            public string TradeName { get; set; }
            public string Strength { get; set; }
            public string Instructions { get; set; }
        }
    }
}
