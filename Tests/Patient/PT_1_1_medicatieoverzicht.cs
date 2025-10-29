using NUnit.Framework;
using OpenQA.Selenium;
using System;
using Tests; // Zorgt dat deze klasse de BaseTest kan vinden

namespace Tests.Patient
{
    [TestFixture]
    public class PT_1_1_medicatieoverzicht : BaseTest
    {
        [Test]
        [Category("Happy Path")]
        [Category("PT-1.1")]
        public void PT_1_1_1_ViewActualMedicationSchedule_Success()
        {
            log.Info("=== Starting PT-1.1.1: Inzien van het actuele medicatieschema ===");

            // Arrange
            string username = "patient"; 
            string password = "Patient1";
            
            // Testdata uit de HTML die je stuurde
            string expectedMed1_Name = "Panadol";
            string expectedMed1_Dose = "500mg";
            string expectedMed2_Name = "Crestor";
            string expectedMed2_Dose = "10mg";

            log.Info($"Test data - Username: {username}");

            try
            {
                // Act
                NavigateToLogin();
                Type(By.Name("Username"), username, "Step 1: Entering username");
                Type(By.Name("Password"), password, "Step 1: Entering password");
                Click(By.Name("btn-login"), "Step 1: Clicking login button");
                log.Info("Step 2 & 3: Verifying redirect to medication schedule");
                
                // Assert
                LogStep("Step 5: Verifying medication details are visible on page load");

                // Zoekt op class (Oplossing 2)
                log.Info("Waiting for medication table body to load...");
                FindWithWait(By.CssSelector(".table-striped tbody")); 
                log.Info("Table body found.");

                // Vindt de tabel op basis van zijn class-naam
                var medicationContainer = FindWithWait(By.ClassName("table-striped")); 
                string containerText = medicationContainer.Text;

                // Controleer de data
                Assert.That(containerText, Does.Contain(expectedMed1_Name), $"Medicatie 1 (Naam) '{expectedMed1_Name}' niet gevonden.");
                Assert.That(containerText, Does.Contain(expectedMed1_Dose), $"Medicatie 1 (Dosis) '{expectedMed1_Dose}' niet gevonden.");
                Assert.That(containerText, Does.Contain(expectedMed2_Name), $"Medicatie 2 (Naam) '{expectedMed2_Name}' niet gevonden.");
                Assert.That(containerText, Does.Contain(expectedMed2_Dose), $"Medicatie 2 (Dosis) '{expectedMed2_Dose}' niet gevonden.");

                log.Info("✓ Assertion passed: Correct medication, dose, and times are displayed.");
                log.Info("=== PT-1.1.1 PASSED ===");
            }
            catch (Exception ex)
            {
                log.Error($"PT-1.1.1 FAILED: {ex.Message}");
                log.Error($"Stack trace: {ex.StackTrace}");
                TakeScreenshot("PT_1_1_1_Failed");
                throw;
            }
        }

        [Test]
        [Category("Unhappy Path")]
        [Category("PT-1.1")]
        public void PT_1_1_2_ViewEmptyMedicationSchedule_ShowsMessage()
        {
            log.Info("=== Starting PT-1.1.2: Systeem toont melding bij geen medicatieschema ===");

            // Arrange
            string username = "patient2"; 
            string password = "Patient1"; // (Pas aan als het wachtwoord anders is)
            
            string expectedMessage = "You have no current prescriptions"; 
            
            log.Info($"Test data - Username: {username}");

            try
            {
                // Act
                NavigateToLogin();
                Type(By.Name("Username"), username, "Step 1: Entering username (Patient2)");
                Type(By.Name("Password"), password, "Step 1: Entering password");
                Click(By.Name("btn-login"), "Step 1: Clicking login button");
                log.Info("Step 2: Verifying redirect to medication schedule page");

                // Assert
                LogStep("Step 3: Observing system response for empty state");

                // --- HIER IS DE CORRECTIE ---
                // Wacht tot de 'geen medicatie' melding verschijnt, gezocht op CLASS
                var messageElement = FindWithWait(By.ClassName("alert-info"));

                // Controleer de melding
                Assert.That(messageElement.Displayed, Is.True, "Het 'geen medicatie' bericht is niet zichtbaar.");
                Assert.That(messageElement.Text, Does.Contain(expectedMessage).IgnoreCase, 
                    $"Verwachte tekst '{expectedMessage}' niet gevonden in de melding. Gevonden: '{messageElement.Text}'");
                log.Info("✓ Assertion passed: Correct 'empty state' message is displayed.");
                
                // Controleer dat de tabel (met class 'table-striped') NIET bestaat
                var tableElements = _driver.FindElements(By.ClassName("table-striped"));
                Assert.That(tableElements.Count, Is.EqualTo(0), "Er werd een medicatietabel gevonden, maar er werd geen verwacht.");
                log.Info("✓ Assertion passed: No medication table/container was found.");

                log.Info("=== PT-1.1.2 PASSED ===");
            }
            catch (Exception ex)
            {
                log.Error($"PT-1.1.2 FAILED: {ex.Message}");
                log.Error($"Stack trace: {ex.StackTrace}");
                TakeScreenshot("PT_1_1_2_Failed");
                throw;
            }
        }
    }
}