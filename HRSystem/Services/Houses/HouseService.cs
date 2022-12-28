using HRSystem.Data;
using HRSystem.Data.Models;
using HRSystem.Services.Models;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Services.Houses
{
    public class HouseService : IHouseService
    {
        private readonly HRSystemDbContext context;

        public HouseService(HRSystemDbContext context)
        {
            this.context = context;
        }

        public async Task<HouseQueryServiceModel> AllAsync(
            string? category = null,
            string? serchTerm = null, 
            HouseSorting sorting = HouseSorting.Newest,
            int currentPage = 1,
            int housesPerPage = 1)
        {
            var housesQuery = context.Houses.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                housesQuery = context.Houses
                    .Where(h => h.Category.Name == category);
            }

            if (!string.IsNullOrEmpty(serchTerm))
            {
                housesQuery = housesQuery.Where(h =>
                h.Title.ToLower().Contains(serchTerm.ToLower()) ||
                h.Address.ToLower().Contains(serchTerm.ToLower()) ||
                h.Description.ToLower().Contains(serchTerm.ToLower()));
            }

            housesQuery = sorting switch
            {
                HouseSorting.Priced => housesQuery.OrderBy(h => h.PricePerMonth),
                HouseSorting.NotRentedFirst => housesQuery
                    .OrderBy(h => h.RenterId == null)
                    .ThenByDescending(h => h.Id),
                _ => housesQuery.OrderByDescending(h => h.Id)
            };

            var houses = await housesQuery
                .Skip((currentPage - 1) * housesPerPage)
                .Take(housesPerPage)
                .Select(h => new HouseServiceModel()
                {
                    Id = h.Id,
                    Title = h.Title,
                    Address = h.Address,
                    ImageUrl = h.ImageUrl,
                    PricePerMonth = h.PricePerMonth,
                    IsRented = h.RenterId != null,
                })
                .ToListAsync();

            int totalHouses = housesQuery.Count();

            return new HouseQueryServiceModel()
            {
                Houses = houses,
                TotalHousesCount = totalHouses
            };
        }

        public async Task<IEnumerable<HouseCategoryServiceModel>> AllCategoriesAsync()
        {
            return await context.Categories
                .Select(c => new HouseCategoryServiceModel()
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> AllCategoriesNamesAsync()
        {
            return await context.Categories
                .Select(c => c.Name)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<HouseServiceModel>> AllHousesByAgentId(int id)
        {
            return await context.Houses
                .Where(h => h.AgentId == id)
                .Select(h => new HouseServiceModel()
                {
                    Id = h.Id,
                    Address = h.Address,
                    Title = h.Title,
                    ImageUrl = h.ImageUrl,
                    IsRented = h.RenterId != null,
                    PricePerMonth = h.PricePerMonth
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<HouseServiceModel>> AllHousesByUserIdAsync(string userId)
        {
            return await context.Houses
                .Where(h => h.RenterId == userId)
                .Select(h => new HouseServiceModel()
                {
                    Id = h.Id,
                    Address = h.Address,
                    Title = h.Title,
                    ImageUrl = h.ImageUrl,
                    IsRented = h.RenterId != null,
                    PricePerMonth = h.PricePerMonth
                })
                .ToListAsync();
        }

        public async Task<bool> CategoryExistAsync(int id)
        {
            return await context.Categories
                .AnyAsync(c => c.Id == id);
        }

        public async Task<int> CreateAsync(
            string title,
            string address,
            string description,
            string imageUrl, 
            decimal price, 
            int categoryId,
            int agentId)
        {
            var house = new House()
            {
                Title = title,
                Address = address,
                Description = description,
                ImageUrl = imageUrl,
                PricePerMonth = price,
                CategoryId = categoryId,
                AgentId = agentId
            };

            await context.Houses.AddAsync(house);
            await context.SaveChangesAsync();

            return house.Id;
        }

        public async Task<IEnumerable<HouseIndexServiceModel>> LastThreeHousesAsync()
        {
            return await context.Houses
                .OrderByDescending(h => h.Id)
                .Take(3)
                .Select(h => new HouseIndexServiceModel()
                {
                    Id = h.Id,
                    Title = h.Title,
                    ImageUrl = h.ImageUrl
                })
                .ToListAsync();
        }
    }
}
