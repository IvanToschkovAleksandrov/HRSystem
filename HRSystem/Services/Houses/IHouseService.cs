using HRSystem.Services.Models;

namespace HRSystem.Services.Houses
{
    public interface IHouseService
    {
        Task<IEnumerable<HouseIndexServiceModel>> LastThreeHousesAsync();
        Task<IEnumerable<HouseCategoryServiceModel>> AllCategoriesAsync();
        Task<bool> CategoryExistAsync(int id);
        Task<int> CreateAsync(
            string title,
            string address,
            string description,
            string imageUrl,
            decimal price,
            int categoryId,
            int agentId);
    }
}
