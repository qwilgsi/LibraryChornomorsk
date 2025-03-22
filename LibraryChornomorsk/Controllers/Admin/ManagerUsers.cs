using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryChornomorsk.Data;
using LibraryChornomorsk.Models;

namespace LibraryChornomorsk.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    public class ManagerUsers : Controller
    {
        private readonly UserManager<LibraryUser> _usersManager;
        private readonly ApplicationDbContext _db;

        public ManagerUsers(UserManager<LibraryUser> usersManager, ApplicationDbContext context)
        {
            _usersManager = usersManager;
            _db = context;
        }

        //GET: Users
        [HttpGet]
        public async Task<IActionResult> Index(string searchBy, string searchValue)
        {
           var users = _usersManager.Users.ToList();
           if (!string.IsNullOrEmpty(searchValue))
           {
               if (searchBy == "Username")
               {
                   users = users.Where(x => x.UserName.Contains(searchValue)).ToList();
               }
               else if (searchBy == "Role")
               {
                   var usersInRole = new List<LibraryUser>();
                   foreach (var user in users)
                   {
                       var roles = await _usersManager.GetRolesAsync(user);
                       if (roles.Contains(searchValue))
                       {
                           usersInRole.Add(user);
                       }
                   }
                   users = usersInRole;
               }
           }
           var userRoles = new Dictionary<string, IList<string> > ();
           foreach (var user in users)
           {
               userRoles[user.Id] = await _usersManager.GetRolesAsync(user);
           }
           ViewBag.UserRoles = userRoles;
           return View(users);
        }

        //GET: Users/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return BadRequest("�� ������� ID �����������");
            }
            var user = await _usersManager.FindByIdAsync(id);
            if (user == null)
            { 
                return NotFound("����������� �� ��������");
            }
            var roles = await _usersManager.GetRolesAsync(user);
            ViewBag.Roles = roles;
            LibraryUser? libraryUser = await _db.Users.FindAsync(user.Id);
            ViewBag.ShopUser = libraryUser;
            return View(user);
        }

        // GET: Users/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return BadRequest("Не вказано ID користувача");
            }
            var user = await _usersManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Користувача не знайдено");
            }
            var roles = await _usersManager.GetRolesAsync(user);
            ViewBag.Roles = roles;
            LibraryUser? libraryUser = await _db.Users.FindAsync(user.Id);
            ViewBag.LibraryUser = libraryUser ?? new LibraryUser();
            return View(user);
        }

        // POST: Users/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, LibraryUser user)
        {
            if (id != user.Id)
            {
                return BadRequest("ID не співпадає");
            }
            var existingUser = await _usersManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound("Користувача не знайдено");
            }
            existingUser.Email = user.Email;
            existingUser.UserName = user.UserName;
            var result = await _usersManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Помилка при оновленні користувача");
                return View(user);
            }
            var existingLibraryUser = await _db.Users.FindAsync(id);
            if (existingLibraryUser != null)
            {
                existingLibraryUser.FullName = user.FullName;
                existingLibraryUser.Age = user.Age;
            }
            else
            {
                user.Id = id;
                _db.Users.Add(user);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
