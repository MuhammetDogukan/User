using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SeedData
{
    public class RoleSeed : IEntityTypeConfiguration<IdentityRole<int>>
    {

        public void Configure(EntityTypeBuilder<IdentityRole<int>> builder)
        {
            builder.HasData(new IdentityRole<int>
            {
                Id = 1,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new IdentityRole<int>
            {
                Id = 2,
                Name = "SuperUser",
                NormalizedName = "SUPERUSER",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new IdentityRole<int>
            {
                Id = 3,
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
            );
        }
    }
}
