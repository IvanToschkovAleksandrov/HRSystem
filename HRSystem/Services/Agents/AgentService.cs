using HRSystem.Data;
using HRSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Services.Agents
{
    public class AgentService : IAgentService
    {
        private readonly HRSystemDbContext context;

        public AgentService(HRSystemDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(string userId, string phoneNumber)
        {
            var agent = new Agent()
            {
                UserId = userId,
                PhoneNumber = phoneNumber
            };

            await context.Agents.AddAsync(agent);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistByIdAsync(string userId)
        {
            return await context.Agents
                .AnyAsync(x => x.UserId == userId);
        }

        public async Task<bool> UserHasRentsAsync(string userId)
        {
            return await context.Houses
                .AnyAsync(h => h.RenterId == userId);
        }

        public async Task<bool> UserWithPhoneNumberExistAsync(string phoneNumber)
        {
            return await context.Agents
                .AnyAsync(u => u.PhoneNumber == phoneNumber);
        }
    }
}
