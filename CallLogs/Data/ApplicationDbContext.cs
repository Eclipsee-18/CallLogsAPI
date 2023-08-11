using CallLogs.Model;
using Microsoft.EntityFrameworkCore;

namespace CallLogs.Data
{
	public class ApplicationDbContext:DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{

		}

		public DbSet<User> Users { get; set; }

		public DbSet<Calllog> CallLogs { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Define the relationship between User and CallLog
			modelBuilder.Entity<Calllog>()
				.HasOne(c => c.User)
				.WithMany(u => u.CallLogs)
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.Cascade); // Use the appropriate deletion behavior
		}
	}
}
