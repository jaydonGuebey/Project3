using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI; 
using System;
using System.Collections.ObjectModel;
using System.Threading;
using Tests; 

namespace Tests.Beheerder
{
    [TestFixture]
    public class BE_2_4_log_autorisatie : BaseTest // Nieuwe klasse BE_2_4
    {
        // Gebruikersgegevens
        private const string AdminUsername = "admin"; // Geautoriseerde gebruiker (Auditor)
        private const string AdminPassword = "admin1"; 
        
        // Ongeautoriseerde gebruiker (Arts/Zorgverzekeraar)
        private const string UnauthorizedUsername = "healthinsurer"; 
        private const string UnauthorizedPassword = "healthinsurer1"; // Wachtwoord (aangenomen uit db info)
        
        // Locators & URLs
        private static readonly By LoginUsernameField = By.Name("Username");
        private static readonly By LoginPasswordField = By.Name("Password");
        private static readonly By LoginButton = By.Name("btn-login");

        private const string LogFilesUrl = "/LogFiles"; 
        private const string UnauthorizedLandingUrl = "/"; // Verwachte redirect bij blokkade
        
        // Elementen op de LogFiles pagina om succesvolle toegang te verifiëren
        private static readonly By LogFileDropdownInput = By.Id("logFileDropdown");
        private static readonly By LogContentTextarea = By.Id("logContent");
        private static readonly By AuditTrailMenuLink = By.CssSelector("a[href='/LogFiles']"); // Menu item om te zoeken

        private const string ForbiddenUrl = "/admin/logs"; // Conceptuele verboden URL, voor BE-2.4.2


        // --- HELPERFUNCTIES ---

        private void PerformLogin(string username, string password, string role)
        {
            log.Info($"Logging in as {role} user: {username}");
            NavigateToLogin();
            Type(LoginUsernameField, username, $"Entering username ({username})");
            Type(LoginPasswordField, password, "Entering password");
            Click(LoginButton, "Clicking login button");
            _wait.Until(d => !d.Url.Contains("/Account/Login"));
        }

        private void NavigateAndWaitForUrlChange(string targetUrl)
        {
            log.Info($"Navigating to: {targetUrl}");
            _driver.Navigate().GoToUrl(_baseUrl + targetUrl);
            Thread.Sleep(500); 
        }

        // ------------------------------------------------------------------
        // --- TC BE-2.4.1: HAPPY PATH (Geautoriseerde toegang) ---
        // ------------------------------------------------------------------

        [Test]
        [Category("Happy Path")]
        [Category("BE-2.4")]
        public void BE_2_4_1_AuthorizedAdminHasAccessToLogs()
        {
            log.Info("=== Starting BE-2.4.1: Geautoriseerde beheerder (Admin) heeft toegang tot logs ===");

            try
            {
                // Stap 1: Log in als 'Admin Jansen'
                PerformLogin(AdminUsername, AdminPassword, "Admin");

                // Stap 2: Zoek in het navigatiemenu naar 'Audittrail'
                log.Info("Stap 2: Zoek naar 'Audittrail' menu item.");
                IWebElement menuLink = FindWithWait(AuditTrailMenuLink);
                
                // Expected Result 1: Het menu-item 'Audittrail' is zichtbaar
                Assert.That(menuLink.Displayed, Is.True, "FAILURE: Het 'Audittrail' menu-item is niet zichtbaar.");
                
                // Stap 3: Klik op het menu-item
                menuLink.Click();

                // Stap 4 & Expected Result 2: De Audittrail-module laadt succesvol
                log.Info("Stap 4: Controleer of de logviewer elementen geladen zijn.");
                
                // Controleer de URL
                _wait.Until(d => d.Url.Contains(LogFilesUrl));
                
                // Controleer op een uniek element op de pagina
                FindWithWait(LogFileDropdownInput); 
                FindWithWait(LogContentTextarea);
                
                log.Info("✓ Assertion passed: Audittrail pagina is succesvol geladen en toont logbestanden.");

                log.Info("=== BE-2.4.1 PASSED: Positieve toegangscontrole is OK ===");
            }
            catch (Exception ex)
            {
                log.Error($"BE-2.4.1 FAILED: {ex.Message}");
                TakeScreenshot("BE_2_4_1_Failed");
                throw;
            }
        }


        // ------------------------------------------------------------------
        // --- TC BE-2.4.2: UNHAPPY PATH (Ongeautoriseerde toegang) ---
        // ------------------------------------------------------------------

        [Test]
        [Category("Unhappy Path")]
        [Category("BE-2.4")]
        public void BE_2_4_2_UnauthorizedUserCannotAccessLogs()
        {
            log.Info("=== Starting BE-2.4.2: Ongeautoriseerde gebruiker (Zorgverzekeraar) heeft geen toegang tot logs ===");

            try
            {
                // Stap 1: Log in als 'Dokter Willems' (Zorgverzekeraar)
                PerformLogin(UnauthorizedUsername, UnauthorizedPassword, "Zorgverzekeraar");

                // Stap 2: Zoek in het navigatiemenu naar 'Audittrail'
                log.Info("Stap 2: Controleer of het 'Audittrail' menu-item NIET zichtbaar is.");
                ReadOnlyCollection<IWebElement> menuElements = _driver.FindElements(AuditTrailMenuLink);
                
                // Expected Result 1: Het menu-item 'Audittrail' is niet zichtbaar
                Assert.That(menuElements.Count, Is.EqualTo(0), 
                    "FAILURE: Het 'Audittrail' menu-item is onterecht zichtbaar.");
                log.Info("✓ 'Audittrail' menu is niet zichtbaar.");


                // Stap 3, 4 & 5: Voer de directe URL in en observeer de reactie
                log.Info($"Stap 3: Probeer de directe URL {LogFilesUrl} te benaderen.");
                NavigateAndWaitForUrlChange(LogFilesUrl);
                
                // De URL moet NIET de LogFiles pagina zijn, maar de Index of Login (afhankelijk van de redirect-logica)
                _wait.Until(d => !d.Url.Contains(LogFilesUrl));
                
                string currentUrl = _driver.Url;
                
                // Expected Result 2: Toegang geweigerd (403 Forbidden, resulteert in redirect)
                Assert.That(
                    currentUrl, 
                    Does.Not.Contain(LogFilesUrl).IgnoreCase,
                    $"FAILURE: Gebruiker kreeg onterecht toegang tot de Verboden URL. Huidige URL: {currentUrl}");
                
                // Valideer dat de gebruiker naar een 'veilige' pagina is geredirect
                Assert.That(currentUrl, 
                    Does.Contain(UnauthorizedLandingUrl).IgnoreCase.Or.Contains("/Home/Index").IgnoreCase, 
                    $"FAILURE: Gebruiker werd niet naar een veilige landingspagina geredirect. Huidige URL: {currentUrl}");
                
                log.Info($"✓ Assertion passed: Directe URL-toegang is correct geblokkeerd en geredirect naar {currentUrl}.");

                log.Info("=== BE-2.4.2 PASSED: Negatieve toegangscontrole is OK ===");
            }
            catch (Exception ex)
            {
                log.Error($"BE-2.4.2 FAILED: {ex.Message}");
                TakeScreenshot("BE_2_4_2_Failed");
                throw;
            }
        }
    }
}