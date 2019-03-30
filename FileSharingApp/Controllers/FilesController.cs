using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FileSharingApp.Data;

namespace FileSharingApp.Controllers
{
    public class FilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Files
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Files.Include(f => f.Subject);
            return View(await applicationDbContext.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Id,Name,Path,Public,UserId,SubjectId")] File file)
        {
            if (ModelState.IsValid)
            {
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
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", file.SubjectId);
            return View(file);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Path,Public,UserId,SubjectId")] File file)
        {
            if (id != file.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(file);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileExists(file.Id))
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

            return View(file);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var file = await _context.Files.FindAsync(id);
            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FileExists(int id)
        {
            return _context.Files.Any(e => e.Id == id);
        }
    }
}
