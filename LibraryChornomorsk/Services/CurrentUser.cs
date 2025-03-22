using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LibraryChornomorsk.Data;
using LibraryChornomorsk.Models;
using LibraryChornomorsk.Services.IServices;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace ShopITCourses.Services
{
    public class CurrentUser : ICurrentUser
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(UserManager<IdentityUser> userManager, ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IdentityUser?> GetCurrentIdentityUserAsync()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return null;
            }
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<LibraryUser?> GetCurrentUserAsync()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return null;
            }
            return await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public string? GetAuthenticationMethod()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.AuthenticationMethod);
        }

        private string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string? GetProfilePictureUrl()
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext?.User;
            if (claimsPrincipal == null) return null;
            var loginProvider = claimsPrincipal.FindFirstValue(ClaimTypes.AuthenticationMethod) ?? claimsPrincipal.FindFirstValue("provider");
            if ((loginProvider?.Equals("Google", StringComparison.OrdinalIgnoreCase)) == true)
            {
                foreach (var claim in claimsPrincipal.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }
                return claimsPrincipal.FindFirstValue("picture");
            }
            return null;
        }
    }
}
