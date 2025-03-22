using LibraryChornomorsk.Data;
using LibraryChornomorsk.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryChornomorsk.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NewsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _db = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var newsList = await _db.News.ToListAsync();
            return View(newsList);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var news = await _db.News.FirstOrDefaultAsync(n => n.Id == id);
            if (news == null) return NotFound();

            return View(news);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Image")] News news)
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

                news.Image = $"images/book/{fileName}{extension}";

                _db.Add(news);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(news);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var news = await _db.News.FindAsync(id);
            if (news == null) return NotFound();
            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Image")] News news)
        {
            if (id != news.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    News oldNews = await _db.News.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
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

                        news.Image = $"images/book/{fileName}{extension}";
                    }
                    else
                    {
                        news.Image = oldNews.Image;
                    }

                    _db.Update(news);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(news);
        }

        private bool NewsExists(int id)
        {
            return _db.News.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var news = await _db.News.FindAsync(id);
            if (news == null) return NotFound();
            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _db.News.FindAsync(id);
            if (news != null)
            {
                if (!string.IsNullOrEmpty(news.Image))
                {
                    var file = Path.Combine(_webHostEnvironment.WebRootPath, news.Image);
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                }

                _db.News.Remove(news);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
