using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TAABP.Core;
using TAABP.Core.PaymentEntities;
using TAABP.Core.ShoppingEntities;

namespace TAABP.Infrastructure
{
    public class TAABPDbContext : IdentityDbContext<User>
    {
        public TAABPDbContext(DbContextOptions<TAABPDbContext> options) : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<HotelImage> HotelImages { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<FeaturedDeal> FeaturedDeals { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<PayPal> PayPals { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Cart>()
            //    .HasOne(c => c.PaymentMethod)
            //    .WithMany()
            //    .HasForeignKey(c => c.PaymentMethodId)
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_Hotels_Rating", "Rating >= 0 AND Rating <= 5"));
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_Hotel_NumberOfRooms_Positive", "[NumberOfRooms] > 0"));
                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_Hotel_NumberOfVisits_Positive", "[NumberOfVisits] >= 0"));
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_Room_Number_Positive", "[RoomNumber] > 0"));
                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_Room_Adault_Capacity_Positive", "[AdultsCapacity] >= 0"));
                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_Room_Children_Capacity_Positive", "[ChildrenCapacity] >= 0"));
                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_Room_Price_Positive", "[PricePerNight] > 0"));
            });
            modelBuilder.Entity<Room>()
               .Property(r => r.Type)
               .HasConversion<string>();

            modelBuilder.Entity<FeaturedDeal>(entity =>
            {
                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_FeaturedDeal_Discount_Positive", "[Discount] >= 0"));
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_FeaturedDeal_NumberOfHotels_Positive", "[NumberOfHotels] >= 0"));
                entity.ToTable(t =>
                   t.HasCheckConstraint("CK_FeaturedDeal_NumberOfVists_Positive", "[NumberOfVisits] >= 0"));
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_Reservation_Price_Positive", "[Price] > 0"));
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 0 AND [Rating] <= 5"));
            });

            modelBuilder.Entity<PayPal>()
                .HasOne(p => p.PaymentMethod)
                .WithOne()
                .HasForeignKey<PayPal>(p => p.PaymentMethodId);

            modelBuilder.Entity<CreditCard>()
                .HasOne(cc => cc.PaymentMethod)
                .WithOne()
                .HasForeignKey<CreditCard>(cc => cc.PaymentMethodId);
        }
    }
}
