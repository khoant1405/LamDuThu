using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JSN.IdentityServer.Data;

public class JsnIdentityDbContext : IdentityDbContext
{
    public JsnIdentityDbContext(DbContextOptions<JsnIdentityDbContext> options)
        : base(options)
    {
    }
}