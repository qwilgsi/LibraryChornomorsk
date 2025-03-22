using LibraryChornomorsk.Data;
using LibraryChornomorsk.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Diagnostics;
using Annotation = LibraryChornomorsk.Models.Annotation;

namespace LibraryChornomorsk.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var annotations = _db.Annotations.ToList();
            return View(annotations);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Image")] Annotation annotation)
        {
            if (HttpContext.Request.Form.Files.Count == 0)
            {
                ModelState.AddModelError("Image", "Оберіть файл з картинкою");
            }

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "annotations"); // Правильный путь

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);
                string filePath = Path.Combine(uploadPath, fileName + extension);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await files[0].CopyToAsync(fileStream);
                }

                annotation.Image = $"annotations/{fileName}{extension}"; // Правильный путь

                _db.Annotations.Add(annotation); // Добавляем в БД
                await _db.SaveChangesAsync(); // Сохраняем

                return RedirectToAction(nameof(Index));
            }

            return View(annotation);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var annotation = await _db.Annotations.FindAsync(id);
            if (annotation == null) return NotFound();
            return View(annotation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Image")] Annotation annotation)
        {
            if (id != annotation.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    Annotation oldNews = await _db.Annotations.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
                    if (oldNews == null) return NotFound();

                    var files = HttpContext.Request.Form.Files;

                    if (files.Count > 0)
                    {
                        string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "book");

                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        if (!string.IsNullOrEmpty(oldNews.Image))
                        {
                            string oldFile = Path.Combine(_webHostEnvironment.WebRootPath, oldNews.Image);
                            if (System.IO.File.Exists(oldFile))
                            {
                                System.IO.File.Delete(oldFile);
                            }
                        }

                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        string filePath = Path.Combine(uploadPath, fileName + extension);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await files[0].CopyToAsync(fileStream);
                        }

                        annotation.Image = $"images/book/{fileName}{extension}";
                    }
                    else
                    {
                        annotation.Image = oldNews.Image;
                    }

                    _db.Update(annotation);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnnotationExists(annotation.Id)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(annotation);
        }

        private bool AnnotationExists(int id)
        {
            return _db.Annotations.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var annotation = await _db.Annotations.FindAsync(id);
            if (annotation == null) return NotFound();
            return View(annotation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var annotation = await _db.Annotations.FindAsync(id);
            if (annotation != null)
            {
                if (!string.IsNullOrEmpty(annotation.Image))
                {
                    var file = Path.Combine(_webHostEnvironment.WebRootPath, annotation.Image);
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                }

                _db.Annotations.Remove(annotation);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
