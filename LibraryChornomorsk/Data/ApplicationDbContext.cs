using LibraryChornomorsk.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryChornomorsk.Data
{
    public class ApplicationDbContext : IdentityDbContext<LibraryUser>

    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
    
        public DbSet<News> News { get; set; }
        public DbSet<Annotation> Annotations { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
