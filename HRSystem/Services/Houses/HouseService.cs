using HRSystem.Data;
using HRSystem.Data.Models;
using HRSystem.Services.Models;
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
