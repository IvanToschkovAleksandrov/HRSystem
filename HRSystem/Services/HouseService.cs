using HRSystem.Data;
using HRSystem.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Services
{
    public class HouseService : IHouseService
    {
        private readonly HRSystemDbContext context;

        public HouseService(HRSystemDbContext context)
        {
            this.context = context;
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
