using NUnit.Framework;
using OpenQA.Selenium;
using System;
using Tests; // Zorgt dat deze klasse de BaseTest kan vinden

namespace Tests.Patient
{
    [TestFixture]
    public class PT_1_2_medicatie_details : BaseTest
    {
        [Test]
        [Category("Happy Path")]
        [Category("PT-1.2")]
        public void PT_1_2_1_CorrectDisplayOfAllDetails()
        {
            log.Info("=== Starting PT-1.2.1: Correcte weergave van alle vereiste medicatiedetails ===");

            // Arrange
            string username = "patient"; 
            string password = "Patient1";
            
            // "Checklist" voor de 'patient' gebruiker
            string med1_name = "Panadol";
            string med1_strength = "500mg";
            string med1_instructions = "take every 2 days";

            string med2_name = "Crestor";
            string med2_strength = "10mg";
            string med2_instructions = "take 1 per day";

            log.Info($"Test data - Username: {username}");

            try
            {
                // Act
                NavigateToLogin();
                Type(By.Name("Username"), username, "Step 1: Entering username (Patient)");
                Type(By.Name("Password"), password, "Step 1: Entering password");
                Click(By.Name("btn-login"), "Step 1: Clicking login button");
                log.Info("Step 2: Verifying redirect to medication schedule");
                
                // Assert
                LogStep("Step 3-6: Verifying details for all medications");
                
                FindWithWait(By.CssSelector(".table-striped tbody"));
                log.Info("Table body found.");

                // --- HIER IS DE CORRECTIE ---
                // We zoeken nu specifiek in de TBODY om de headers te negeren.

                // Controleer Medicijn 1 (Panadol)
                log.Info("Checking details for Panadol...");
                // 1. Vind de RIJ (tr) in de TBODY die "Panadol" bevat
                var row1 = FindWithWait(By.XPath("//tbody/tr[contains(., '" + med1_name + "')]"));
                
                // 2. Controleer of ALLE data in de tekst van die RIJ voorkomt
                Assert.That(row1.Text, Does.Contain(med1_name), "Naam 'Panadol' niet gevonden in de rij.");
                Assert.That(row1.Text, Does.Contain(med1_strength), "Sterkte '500mg' niet gevonden in de rij.");
                Assert.That(row1.Text, Does.Contain(med1_instructions), "Instructie 'take every 2 days' niet gevonden in de rij.");
                log.Info("✓ Panadol details are correct.");

                // Controleer Medicijn 2 (Crestor)
                log.Info("Checking details for Crestor...");
                // 1. Vind de RIJ (tr) in de TBODY die "Crestor" bevat
                var row2 = FindWithWait(By.XPath("//tbody/tr[contains(., '" + med2_name + "')]"));
                
                // 2. Controleer of ALLE data in de tekst van die RIJ voorkomt
                Assert.That(row2.Text, Does.Contain(med2_name), "Naam 'Crestor' niet gevonden in de rij.");
                Assert.That(row2.Text, Does.Contain(med2_strength), "Sterkte '10mg' niet gevonden in de rij.");
                Assert.That(row2.Text, Does.Contain(med2_instructions), "Instructie 'take 1 per day' niet gevonden in de rij.");
                log.Info("✓ Crestor details are correct.");

                log.Info("✓ Assertion passed: All medication details are displayed correctly.");
                log.Info("=== PT-1.2.1 PASSED ===");
            }
            catch (Exception ex)
            {
                log.Error($"PT-1.2.1 FAILED: {ex.Message}");
                log.Error($"Stack trace: {ex.StackTrace}");
                TakeScreenshot("PT_1_2_1_Failed");
                throw;
            }
        }
    }
}