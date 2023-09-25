using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Flatlinq.Data;

public class CoreDbContext: IdentityDbContext<User>
{
	public CoreDbContext(DbContextOptions<CoreDbContext> options): base(options)
	{
		
	}

    public DbSet<Tenant>? Tenants { get; set; }
	public DbSet<House>? Houses { get; set; }
	public DbSet<Landlord>? Landlords { get; set; }
	public DbSet<UserSwipes>? Swipes { get; set; }
    public DbSet<Channel>? Channels { get; set; }
    public DbSet<Message>? Messages { get; set; }
}
