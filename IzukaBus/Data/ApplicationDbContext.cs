using IzukaBus.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IzukaBus.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
		public DbSet<Topic> Topics { get; set; }
		public DbSet<TopicSubscriber> Subscribers { get; set; }

		public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
		{
			var result =  await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
			if(result > 0 )
			{
				return result;
			}
			else
			{
				throw new ArgumentOutOfRangeException("Unable to persist the current data to database");
			}
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
		
			var result =  base.SaveChangesAsync(cancellationToken);
			if (result.Result > 0)
			{
				return result;
			}
			else
			{
				throw new ArgumentOutOfRangeException("Unable to persist the current data to database");
			}
		}

	}
}
