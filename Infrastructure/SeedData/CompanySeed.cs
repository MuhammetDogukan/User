using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SeedData
{
    public class CompanySeed : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasData(new Company
            {
                Id = 1,
                Name = "Romaguera-Crona",
                CatchPhrase = "Multi-layered client-server neural-net",
                Bs = "harness real-time e-markets"
            },
            new Company
            {
                Id = 2,
                Name = "Deckow-Crist",
                CatchPhrase = "Proactive didactic contingency",
                Bs = "synergize scalable supply-chains"
            },
            new Company
            {
                Id = 3,
                Name = "Romaguera-Jacobson",
                CatchPhrase = "Face to face bifurcated interface",
                Bs = "e-enable strategic applications"
            },
            new Company
            {
                Id = 4,
                Name = "Robel-Corkery",
                CatchPhrase = "Multi-tiered zero tolerance productivity",
                Bs = "transition cutting-edge web services"
            },
            new Company
            {
                Id = 5,
                Name = "Keebler LLC",
                CatchPhrase = "User-centric fault-tolerant solution",
                Bs = "revolutionize end-to-end systems"
            },
            new Company
            {
                Id = 6,
                Name = "Considine-Lockman",
                CatchPhrase = "Synchronised bottom-line interface",
                Bs = "e-enable innovative applications"
            },
            new Company
            {
                Id = 7,
                Name = "Johns Group",
                CatchPhrase = "Configurable multimedia task-force",
                Bs = "generate enterprise e-tailers"
            },
            new Company
            {
                Id = 8,
                Name = "Abernathy Group",
                CatchPhrase = "Implemented secondary concept",
                Bs = "e-enable extensible e-tailers"
            },
            new Company
            {
                Id = 9,
                Name = "Yost and Sons",
                CatchPhrase = "Switchable contextually-based project",
                Bs = "aggregate real-time technologies"
            },
            new Company
            {
                Id = 10,
                Name = "Hoeger LLC",
                CatchPhrase = "Centralized empowering task-force",
                Bs = "target end-to-end models"
            });
        }
    }
}
