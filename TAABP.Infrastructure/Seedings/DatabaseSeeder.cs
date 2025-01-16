using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TAABP.Core;

namespace TAABP.Infrastructure.Seedings
{
    public static class DatabaseSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TAABPDbContext>();

            context.Database.Migrate();
            SeedCities(context);
            SeedHotels(context);
            SeedRooms(context);
        }

        private static void SeedCities(TAABPDbContext context)
        {
            if (!context.Cities.Any())
            {
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Cities', RESEED, 0);");
                context.Cities.AddRange(
                    new City { Name = "New York", Country = "USA", Thumbnail = "newyork.jpg", Description = "The city that never sleeps.", PostOffice = "10001", NumberOfHotels = 20, NumberOfVisits = 5000000 },
                    new City { Name = "London", Country = "UK", Thumbnail = "london.jpg", Description = "Home to Big Ben and the Thames.", PostOffice = "E1 6AN", NumberOfHotels = 30, NumberOfVisits = 7000000 },
                    new City { Name = "Paris", Country = "France", Thumbnail = "paris.jpg", Description = "The city of love.", PostOffice = "75001", NumberOfHotels = 25, NumberOfVisits = 6000000 },
                    new City { Name = "Tokyo", Country = "Japan", Thumbnail = "tokyo.jpg", Description = "A blend of tradition and technology.", PostOffice = "100-0001", NumberOfHotels = 50, NumberOfVisits = 8000000 },
                    new City { Name = "Sydney", Country = "Australia", Thumbnail = "sydney.jpg", Description = "Famous for the Opera House.", PostOffice = "2000", NumberOfHotels = 15, NumberOfVisits = 3000000 },
                    new City { Name = "Dubai", Country = "UAE", Thumbnail = "dubai.jpg", Description = "Home to the Burj Khalifa.", PostOffice = "12345", NumberOfHotels = 40, NumberOfVisits = 10000000 },
                    new City { Name = "Cape Town", Country = "South Africa", Thumbnail = "capetown.jpg", Description = "Known for Table Mountain.", PostOffice = "8001", NumberOfHotels = 10, NumberOfVisits = 2000000 },
                    new City { Name = "Rio de Janeiro", Country = "Brazil", Thumbnail = "rio.jpg", Description = "Famous for its Carnival.", PostOffice = "20000-000", NumberOfHotels = 18, NumberOfVisits = 4000000 },
                    new City { Name = "Berlin", Country = "Germany", Thumbnail = "berlin.jpg", Description = "A city steeped in history.", PostOffice = "10115", NumberOfHotels = 22, NumberOfVisits = 3500000 },
                    new City { Name = "Rome", Country = "Italy", Thumbnail = "rome.jpg", Description = "Home to the Colosseum.", PostOffice = "00100", NumberOfHotels = 28, NumberOfVisits = 4500000 }
                );
                context.SaveChanges();
            }
        }

        private static void SeedHotels(TAABPDbContext context)
        {
            if (!context.Hotels.Any())
            {
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Hotels', RESEED, 0);");
                context.Hotels.AddRange(
                    new Hotel { Name = "Grand Palace", Address = "123 Palace St, New York", PhoneNumber = "123-456-7890", Description = "Luxury at its finest.", Thumbnail = "grandpalace.jpg", Owner = "John Doe", Rating = 5, NumberOfRooms = 100, NumberOfVisits = 5000, CityId = 1 },
                    new Hotel { Name = "The Ritz", Address = "456 Ritz Rd, London", PhoneNumber = "234-567-8901", Description = "Elegance in every corner.", Thumbnail = "theritz.jpg", Owner = "Jane Smith", Rating = 5, NumberOfRooms = 80, NumberOfVisits = 4000, CityId = 2 },
                    new Hotel { Name = "Parisian Paradise", Address = "789 Eiffel Blvd, Paris", PhoneNumber = "345-678-9012", Description = "A true Parisian experience.", Thumbnail = "parisianparadise.jpg", Owner = "Pierre Dupont", Rating = 4, NumberOfRooms = 70, NumberOfVisits = 3500, CityId = 3 },
                    new Hotel { Name = "Tokyo Towers", Address = "321 Shinjuku St, Tokyo", PhoneNumber = "456-789-0123", Description = "Modern luxury in the heart of Tokyo.", Thumbnail = "tokyotowers.jpg", Owner = "Kenji Tanaka", Rating = 5, NumberOfRooms = 150, NumberOfVisits = 6000, CityId = 4 },
                    new Hotel { Name = "Harbor View", Address = "654 Ocean Dr, Sydney", PhoneNumber = "567-890-1234", Description = "Wake up to the ocean breeze.", Thumbnail = "harborview.jpg", Owner = "Emily Watson", Rating = 4, NumberOfRooms = 90, NumberOfVisits = 3000, CityId = 5 },
                    new Hotel { Name = "Desert Oasis", Address = "987 Dunes Ln, Dubai", PhoneNumber = "678-901-2345", Description = "A luxurious escape in the desert.", Thumbnail = "desertoasis.jpg", Owner = "Ahmed Al-Farsi", Rating = 5, NumberOfRooms = 120, NumberOfVisits = 8000, CityId = 6 },
                    new Hotel { Name = "Table Mountain Retreat", Address = "159 Summit Ave, Cape Town", PhoneNumber = "789-012-3456", Description = "Relax with stunning mountain views.", Thumbnail = "tablemountainretreat.jpg", Owner = "Sipho Ndlovu", Rating = 4, NumberOfRooms = 60, NumberOfVisits = 2000, CityId = 7 },
                    new Hotel { Name = "Carnival Getaway", Address = "753 Samba St, Rio de Janeiro", PhoneNumber = "890-123-4567", Description = "Celebrate in the heart of Rio.", Thumbnail = "carnivalgetaway.jpg", Owner = "Carlos Silva", Rating = 4, NumberOfRooms = 85, NumberOfVisits = 4000, CityId = 8 },
                    new Hotel { Name = "Berlin Boutique", Address = "258 Berlin St, Berlin", PhoneNumber = "901-234-5678", Description = "Boutique hotel with a rich history.", Thumbnail = "berlinboutique.jpg", Owner = "Anna Müller", Rating = 4, NumberOfRooms = 50, NumberOfVisits = 3500, CityId = 9 },
                    new Hotel { Name = "Roman Holiday", Address = "852 Coliseum Ln, Rome", PhoneNumber = "123-345-6789", Description = "Experience the charm of Rome.", Thumbnail = "romanholiday.jpg", Owner = "Luigi Rossi", Rating = 4, NumberOfRooms = 75, NumberOfVisits = 4500, CityId = 10 }
                );
                context.SaveChanges();
            }

        }

        private static void SeedRooms(TAABPDbContext context)
        {
            if (!context.Rooms.Any())
            {
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Rooms', RESEED, 0);");
                context.Rooms.AddRange(
                    new Room { Name = "Luxury Suite", Description = "A luxurious suite with ocean view.", Thumbnail = "luxurysuite.jpg", AdultsCapacity = 2, ChildrenCapacity = 2, PricePerNight = 500, IsAvailable = true, RoomNumber = 101, Type = RoomType.Luxury, HotelId = 1 },
                    new Room { Name = "Deluxe Room", Description = "A spacious room with modern amenities.", Thumbnail = "deluxeroom.jpg", AdultsCapacity = 2, ChildrenCapacity = 1, PricePerNight = 300, IsAvailable = true, RoomNumber = 102, Type = RoomType.Deluxe, HotelId = 1 },
                    new Room { Name = "Family Room", Description = "Perfect for family vacations.", Thumbnail = "familyroom.jpg", AdultsCapacity = 2, ChildrenCapacity = 3, PricePerNight = 400, IsAvailable = true, RoomNumber = 103, Type = RoomType.Family, HotelId = 2 },
                    new Room { Name = "Single Room", Description = "A cozy room for one person.", Thumbnail = "singleroom.jpg", AdultsCapacity = 1, ChildrenCapacity = 0, PricePerNight = 150, IsAvailable = true, RoomNumber = 201, Type = RoomType.Single, HotelId = 3 },
                    new Room { Name = "Suite", Description = "A spacious suite with city view.", Thumbnail = "suite.jpg", AdultsCapacity = 2, ChildrenCapacity = 2, PricePerNight = 450, IsAvailable = true, RoomNumber = 202, Type = RoomType.Suite, HotelId = 4 },
                    new Room { Name = "Double Room", Description = "Comfortable room for two.", Thumbnail = "doubleroom.jpg", AdultsCapacity = 2, ChildrenCapacity = 0, PricePerNight = 200, IsAvailable = true, RoomNumber = 301, Type = RoomType.Double, HotelId = 5 },
                    new Room { Name = "Economy Room", Description = "Affordable and convenient.", Thumbnail = "economyroom.jpg", AdultsCapacity = 2, ChildrenCapacity = 0, PricePerNight = 100, IsAvailable = true, RoomNumber = 302, Type = RoomType.Single, HotelId = 6 },
                    new Room { Name = "Luxury Suite", Description = "Premium luxury with private pool.", Thumbnail = "luxuryroom.jpg", AdultsCapacity = 2, ChildrenCapacity = 2, PricePerNight = 700, IsAvailable = true, RoomNumber = 401, Type = RoomType.Luxury, HotelId = 7 },
                    new Room { Name = "Family Suite", Description = "Spacious suite for the whole family.", Thumbnail = "familysuite.jpg", AdultsCapacity = 4, ChildrenCapacity = 4, PricePerNight = 600, IsAvailable = true, RoomNumber = 402, Type = RoomType.Family, HotelId = 8 },
                    new Room { Name = "Penthouse", Description = "Exclusive penthouse with panoramic view.", Thumbnail = "penthouse.jpg", AdultsCapacity = 2, ChildrenCapacity = 1, PricePerNight = 1000, IsAvailable = true, RoomNumber = 501, Type = RoomType.Luxury, HotelId = 9 }
                );
                context.SaveChanges();
            }
        }
    }
}
