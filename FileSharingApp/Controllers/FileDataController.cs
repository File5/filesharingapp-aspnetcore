using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileSharingApp.Controllers
{
    public class FileDataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file) {
            if (file == null)
                return Content("file not selected");

            var extension = file.FileName.Split(".").LastOrDefault();
            var filename = Path.GetRandomFileName() + "." + extension;
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        "Files", filename);

            using (var stream = new FileStream(path, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Download(string filename) {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", "Files", filename);

            //var memory = new MemoryStream();
            //using (var stream = new FileStream(path, FileMode.Open)) {
            //    await stream.CopyToAsync(memory);
            //}
            //memory.Position = 0;
            //string fileType = GetContentType(path);
            //return File(path, fileType, filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open)) {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));

            //Response.Headers["Content-Disposition"] = $"inline; filename={filename}.pdf";
            //var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(path), GetContentType(path)) {
            //    FileDownloadName = $"{filename}.pdf"
            //};

            //return fileContentResult;
        }

        private string GetContentType(string path) {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.GetValueOrDefault(ext, "application/octet-stream");
        }

        private Dictionary<string, string> GetMimeTypes() {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},  
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}