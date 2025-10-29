using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Pages.LogFiles
{
    public class IndexModel : PageModel
    {
        public List<string> LogFiles { get; set; } = new();

        private string LogsDirectory =>
            Path.Combine(AppContext.BaseDirectory, "Logs");

        public void OnGet()
        {
            if (Directory.Exists(LogsDirectory))
            {
                LogFiles = Directory.GetFiles(LogsDirectory)
                    .Select(Path.GetFileName)
                    .OrderBy(f => f)
                    .ToList();
            }
        }

        public IActionResult OnGetView(string fileName)
        {
            var filePath = Path.Combine(LogsDirectory, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var content = System.IO.File.ReadAllText(filePath);
            return Content(content, "text/plain");
        }

        public IActionResult OnGetDownload(string fileName)
        {
            var filePath = Path.Combine(LogsDirectory, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "text/plain", fileName);
        }
    }
}
