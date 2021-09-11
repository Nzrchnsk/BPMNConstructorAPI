using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BPMNConstructorAPI.Models
{
    public class BPMNConstructorContext : IdentityDbContext<User, IdentityRole<int>,int>
    {
        public BPMNConstructorContext(DbContextOptions<BPMNConstructorContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }
    }
}