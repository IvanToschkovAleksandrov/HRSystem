using HRSystem.Services.Models;

namespace HRSystem.Services.Houses
{
    public interface IHouseService
    {
        Task<IEnumerable<HouseIndexServiceModel>> LastThreeHousesAsync();

        Task<HouseQueryServiceModel> AllAsync(
            string? category = null,
            string? serchTerm = null,
            HouseSorting sorting = HouseSorting.Newest,
            int currentPage = 1,
            int housesPerPage = 1);

        Task<IEnumerable<HouseCategoryServiceModel>> AllCategoriesAsync();

        Task<IEnumerable<string>> AllCategoriesNamesAsync();

        Task<IEnumerable<HouseServiceModel>> AllHousesByUserIdAsync(string userId);

        Task<IEnumerable<HouseServiceModel>> AllHousesByAgentId(int id);
        
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
