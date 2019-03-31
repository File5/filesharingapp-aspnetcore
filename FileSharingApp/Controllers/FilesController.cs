using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FileSharingApp.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FileSharingApp.Areas.Identity.Data;

namespace FileSharingApp.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public FilesController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Files
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Files.Include(f => f.Subject).Include(f => f.User);
            AppUser currentUser = await GetCurrentUser();
            bool isAdmin = await _userManager.IsInRoleAsync(currentUser, AdminController.adminRoleName);
            IEnumerable<Data.File> result = applicationDbContext.ToList();
            if (!isAdmin) {
                result = result.Where(i => CheckFileVisibleFor(currentUser, i));
            }
            return View(result.ToList());
        }

        // GET: Files/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.Files
                .Include(f => f.Subject)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }

        // GET: Files/Create
        public IActionResult Create()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name");
            return View();
        }

        // POST: Files/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Public,SubjectId")] Data.File file, IFormFile filedata)
        {
            if (ModelState.IsValid)
            {
                if (file == null)
                    return Content("file not selected");

                var extension = filedata.FileName.Split(".").LastOrDefault();
                var filename = Path.GetRandomFileName() + "." + extension;
                var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot",
                            "Files", filename);

                using (var stream = new FileStream(path, FileMode.Create)) {
                    await filedata.CopyToAsync(stream);
                }

                file.Path = filename;
                AppUser currentUser = await GetCurrentUser();
                file.UserId = currentUser.Id;
                file.User = currentUser;

                _context.Add(file);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", file.SubjectId);
            return View(file);
        }

        // GET: Files/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.Files.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }
            bool canEdit = await CanEditFile(file);
            if (!canEdit) {
                return Forbid();
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", file.SubjectId);
            return View(file);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Public,SubjectId")] Data.File file)
        {
            var origFile = await _context.Files.FindAsync(id);
            if (origFile == null) {
                return NotFound();
            }
            if (id != file.Id)
            {
                return NotFound();
            }
            bool canEdit = await CanEditFile(origFile);
            if (!canEdit) {
                return Forbid();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    origFile.Name = file.Name;
                    origFile.Public = file.Public;
                    origFile.SubjectId = file.SubjectId;
                    origFile.Subject = file.Subject;

                    _context.Update(origFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileExists(origFile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", file.SubjectId);
            return View(file);
        }

        // GET: Files/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.Files
                .Include(f => f.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (file == null)
            {
                return NotFound();
            }
            bool canEdit = await CanEditFile(file);
            if (!canEdit) {
                return Forbid();
            }


            return View(file);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var file = await _context.Files.FindAsync(id);
            bool canEdit = await CanEditFile(file);
            if (!canEdit) {
                return Forbid();
            }


            if (file.Path != null) {
                var path = Path.Combine(
                               Directory.GetCurrentDirectory(),
                               "wwwroot", "Files", file.Path);
                System.IO.File.Delete(path);
            }

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Download(string filename) {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", "Files", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open)) {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private async Task<AppUser> GetCurrentUser() {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        private bool CheckFileVisibleFor(AppUser user, Data.File file) {
            if (file.UserId.Equals(user.Id)) {
                return true;
            }

            if (file.Public) {
                var userSpecialityId = user.SpecialityId;
                var fileSpecialityId = file.Subject.SpecialityId;
                var userSemester = user.Semester;
                var fileSemester = file.Subject.Semester;
                if (userSpecialityId != null && userSpecialityId == fileSpecialityId || fileSpecialityId == null) {
                    if (userSemester == null || userSemester == fileSemester) {
                        return true; 
                    }
                }
            }

            return false;
        }

        private async Task<bool> CanEditFile(Data.File file) {
            var currentUser = await GetCurrentUser();
            bool isAdmin = await _userManager.IsInRoleAsync(currentUser, AdminController.adminRoleName);
            return file.UserId == currentUser.Id || isAdmin;
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

        private bool FileExists(int id)
        {
            return _context.Files.Any(e => e.Id == id);
        }
    }
}
