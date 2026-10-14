using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace home_work_66.Models;

public class HeadHunterContext: IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<User> Users { get; set; }
    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    public HeadHunterContext(DbContextOptions<HeadHunterContext> options) : base(options) {}
}