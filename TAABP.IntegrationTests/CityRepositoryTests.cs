﻿using AutoFixture;
using TAABP.Infrastructure.Repositories;
using TAABP.Infrastructure;
using Microsoft.EntityFrameworkCore;
using TAABP.Core;

namespace TAABP.IntegrationTests
{
    public class CityRepositoryTests
    {
        private readonly TAABPDbContext _context;
        private readonly CityRepository _cityRepository;
        private readonly IFixture _fixture;

        public CityRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TAABPDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TAABPDbContext(options);

            _cityRepository = new CityRepository(_context);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task CreateCityAsync_ShouldAddCityAsync()
        {
            // Arrange
            var city = _fixture.Create<City>();

            // Act
            await _cityRepository.CreateCityAsync(city);

            // Assert
            var result = await _context.Cities.FirstOrDefaultAsync(c => c.CityId == city.CityId);
            Assert.Equal(city, result);
        }

        [Fact]
        public async Task GetCityByIdAsync_ShouldReturnCityByIdAsync()
        {
            // Arrange
            var city = _fixture.Create<City>();
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cityRepository.GetCityByIdAsync(city.CityId);

            // Assert
            Assert.Equal(city.CityId, result.CityId);
        }

        [Fact]
        public async Task GetCitiesAsync_ShouldReturnAllCitiesAsync()
        {
            // Arrange
            var cities = _fixture.CreateMany<City>(3);
            await _context.Cities.AddRangeAsync(cities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cityRepository.GetCitiesAsync();

            // Assert
            Assert.Equal(cities.Count(), result.Count());
        }

        [Fact]
        public async Task UpdateCityAsync_ShouldUpdateCityAsync()
        {
            // Arrange
            var city = _fixture.Create<City>();
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();
            city.Name = "Updated Name";

            // Act
            await _cityRepository.UpdateCityAsync(city);

            // Assert
            var result = await _context.Cities.FirstOrDefaultAsync(c => c.CityId == city.CityId);
            Assert.Equal(city.Name, result.Name);
        }

        [Fact]
        public async Task DeleteCityAsync_ShouldDeleteCityAsync()
        {
            // Arrange
            var city = _fixture.Create<City>();
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();

            // Act
            await _cityRepository.DeleteCityAsync(city);

            // Assert
            var result = await _context.Cities.FirstOrDefaultAsync(c => c.CityId == city.CityId);
            Assert.Null(result);
        }

        [Fact]
        public async Task IncrementNumberOfHotelsAsync_ShouldIncrementNumberOfHotelsAsync()
        {
            // Arrange
            var city = _fixture.Create<City>();
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();

            // Act
            await _cityRepository.IncrementNumberOfHotelsAsync(city.CityId);

            // Assert
            var result = await _context.Cities.FirstOrDefaultAsync(c => c.CityId == city.CityId);
            Assert.Equal(city.NumberOfHotels, result.NumberOfHotels);
        }

        [Fact]
        public async Task DecrementNumberOfHotelsAsync_ShouldDecrementNumberOfHotelsAsync()
        {
            // Arrange
            int numberOfHotels = 10;
            var city = _fixture.Build<City>()
                .With(c => c.NumberOfHotels, numberOfHotels)
                .Create();
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();
            // Act
            await _cityRepository.DecrementNumberOfHotelsAsync(city.CityId);

            // Assert
            var result = await _context.Cities.FirstOrDefaultAsync(c => c.CityId == city.CityId);
            Assert.Equal(9, result.NumberOfHotels);
        }

        [Fact]
        public async Task IncrementNumberOfVisitsAsync_ShouldIncrementNumberOfVisitsAsync()
        {
            // Arrange
            var city = _fixture.Create<City>();
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();

            // Act
            await _cityRepository.IncrementNumberOfVisitsAsync(city.CityId);

            // Assert
            var result = await _context.Cities.FirstOrDefaultAsync(c => c.CityId == city.CityId);
            Assert.Equal(city.NumberOfVisits, result.NumberOfVisits);
        }

        [Fact]
        public async Task DecrementNumberOfVisitsAsync_ShouldDecrementNumberOfVisitsAsync()
        {
            // Arrange
            int numberOfVisits = 10;
            var city = _fixture.Build<City>()
                .With(c => c.NumberOfVisits, numberOfVisits)
                .Create();
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();

            // Act
            await _cityRepository.DecrementNumberOfVisitsAsync(city.CityId);

            // Assert
            var result = await _context.Cities.FirstOrDefaultAsync(c => c.CityId == city.CityId);
            Assert.Equal(9, result.NumberOfVisits);
        }
    }
}
