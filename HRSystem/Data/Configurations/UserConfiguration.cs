using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRSystem.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<IdentityUser>
    {
        public void Configure(EntityTypeBuilder<IdentityUser> builder)
        {
            builder.HasData(CreateUsers());
        }

        private List<IdentityUser> CreateUsers()
        {
            var users = new List<IdentityUser>();
            var hasher = new PasswordHasher<IdentityUser>();

            var user = new IdentityUser
            {
                Id = "dea12856-c198-4129-b3f3-b893d8395082",
                UserName = "AgentUser",
                NormalizedUserName = "agentuser",
                Email = "agentUser@mail.com",
                NormalizedEmail = "agentuser@mail.com"
            };

            user.PasswordHash = hasher.HashPassword(user, "Agent13");
            users.Add(user);

            user = new IdentityUser
            {
                Id = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                UserName = "GuestUser",
                NormalizedUserName = "guestuser",
                Email = "guestUser@mail.com",
                NormalizedEmail = "guestuser@mail.com"
            };

            user.PasswordHash = hasher.HashPassword(user, "Guest13");
            users.Add(user);

            return users;
        }
    }
}
