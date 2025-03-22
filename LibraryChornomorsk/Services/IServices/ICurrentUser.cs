using Microsoft.AspNetCore.Identity;
using LibraryChornomorsk.Models;

namespace LibraryChornomorsk.Services.IServices
{
    public interface ICurrentUser
    {
        Task<LibraryUser?> GetCurrentUserAsync();
        Task<IdentityUser?> GetCurrentIdentityUserAsync();
        string? GetAuthenticationMethod();
        string? GetProfilePictureUrl();
    }
}
