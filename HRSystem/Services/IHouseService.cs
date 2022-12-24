using HRSystem.Services.Models;

namespace HRSystem.Services
{
    public interface IHouseService
    {
        Task<IEnumerable<HouseIndexServiceModel>> LastThreeHousesAsync();
    }
}
