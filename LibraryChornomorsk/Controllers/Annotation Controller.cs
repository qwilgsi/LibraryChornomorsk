using LibraryChornomorsk.Data;
using LibraryChornomorsk.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Annotation = LibraryChornomorsk.Models.Annotation;
using Microsoft.AspNetCore.Authorization;

namespace LibraryChornomorsk.Controllers
{
    public class AnnotationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AnnotationController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
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
                string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "book");

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

                annotation.Image = $"images/book/{fileName}{extension}";

                _db.Add(annotation);
                await _db.SaveChangesAsync();

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
        public async Task<IActionResult> Edit(Annotation annotation, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var annotationFromDb = await _db.Annotations.FindAsync(annotation.Id);
                if (annotationFromDb == null) return NotFound();

                if (file != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "annotations");
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Удаляем старый файл
                    if (!string.IsNullOrEmpty(annotationFromDb.Image))
                    {
                        string oldImagePath = Path.Combine(uploadsFolder, annotationFromDb.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    annotationFromDb.Image = uniqueFileName;
                }

                annotationFromDb.Name = annotation.Name;
                annotationFromDb.Description = annotation.Description;

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(annotation);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var annotation = await _db.Annotations.FindAsync(id);
            if (annotation == null) return NotFound();

            return View(annotation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var annotation = await _db.Annotations.FindAsync(id);
            if (annotation == null) return NotFound();

            if (!string.IsNullOrEmpty(annotation.Image))
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "annotations");
                string imagePath = Path.Combine(uploadsFolder, annotation.Image);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _db.Annotations.Remove(annotation);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
