using HRSystem.Services.Models;

namespace HRSystem.Services.Houses
{
    public interface IHouseService
    {
        Task<IEnumerable<HouseIndexServiceModel>> LastThreeHousesAsync();
    }
}
