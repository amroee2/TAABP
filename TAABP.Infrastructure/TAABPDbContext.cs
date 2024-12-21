using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TAABP.Core;

namespace TAABP.Infrastructure
{
    public class TAABPDbContext : IdentityDbContext<User>
    {
        public TAABPDbContext(DbContextOptions<TAABPDbContext> options) : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_Hotels_Rating", "Rating >= 0 AND Rating <= 5"));
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_Hotel_NumberOfRooms_Positive", "[NumberOfRooms] > 0"));
            });
        }
    }
}
