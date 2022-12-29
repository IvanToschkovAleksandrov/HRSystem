using HRSystem.Data;
using HRSystem.Data.Models;
using HRSystem.Services.Agents.Models;
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
            var houses = await context.Houses
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
            return houses;
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

        public async Task EditAsync(int id, string title, string address, string description, string imageUrl, decimal price, int categoryId)
        {
            var house = await context.Houses
                .FirstAsync(h => h.Id == id);
            house.Title = title;
            house.Address = address;
            house.Description = description;
            house.ImageUrl = imageUrl;
            house.PricePerMonth = price;
            house.CategoryId = categoryId;

            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistAsync(int id)
        {
            return await context.Houses
                .AnyAsync(h => h.Id == id);
        }

        public async Task<int> GetHouseCategoryIdAsync(int houseId)
        {
            var house = await context.Houses
                .FirstAsync(h => h.Id == houseId);

            return house.CategoryId;
        }

        public async Task<bool> HasAgentWithIdAsync(int houseId, string currentUserId)
        {
            var house = await context.Houses
                .FindAsync(houseId);
            var agent = await context.Agents
                .FirstOrDefaultAsync(a => a.Id == house.AgentId);

            if (agent == null)
            {
                return false;
            }

            if (agent.UserId != currentUserId)
            {
                return false;
            }

            return true;
        }

        public async Task<HouseDetailsServiceModel> HouseDetailsByIdAsync(int id)
        {
            return await context.Houses
                .Where(h => h.Id == id)
                .Select(h => new HouseDetailsServiceModel()
                {
                    Id = h.Id,
                    Title = h.Title,
                    Address = h.Address,
                    Description = h.Description,
                    ImageUrl = h.ImageUrl,
                    PricePerMonth = h.PricePerMonth,
                    IsRented = h.RenterId != null,
                    Category = h.Category.Name,
                    Agent = new AgentServiceModel()
                    {
                        Email = h.Agent.User.Email,
                        PhoneNumber = h.Agent.PhoneNumber
                    }
                })
                .FirstAsync();
        }

        public async Task<bool> IsRentedAsync(int id)
        {
            return await context.Houses
                .AnyAsync(h => h.Id == id && h.RenterId != null);
        }

        public async Task<bool> IsRentedByUserIdAsync(int houseId, string userId)
        {
            return await context.Houses
                .AnyAsync(h => h.Id == houseId && h.RenterId == userId);
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

        public async Task RentAsync(int houseId, string userId)
        {
            var house = await context.Houses.FirstAsync(h => h.Id == houseId);
            house.RenterId = userId;

            await context.SaveChangesAsync();
        }
    }
}
