using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace home_work_66.Models;

public class HeadHunterContext: IdentityDbContext<User, IdentityRole<int>, int>
{
    
    
    public HeadHunterContext(DbContextOptions<HeadHunterContext> options) : base(options) {}
}