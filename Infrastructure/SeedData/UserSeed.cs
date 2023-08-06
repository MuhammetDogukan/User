using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SeedData
{
    public class UserSeed : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasData(new User
            {
                Id = 1,
                Name = "Leanne Graham",
                UserName = "Bret",
                Email = "Sincere@april.biz",
                AdressId = 1,
                Phone = "1-770-736-8031 x56442",
                Website = "hildegard.org",
                CompanyId = 1,
            },
            new User
            {
                Id = 2,
                Name = "Ervin Howell",
                UserName = "Antonette",
                Email = "Shanna@melissa.tv",
                AdressId = 2,
                Phone = "010-692-6593 x09125",
                Website = "anastasia.net",
                CompanyId = 2,
            },
            new User
            {
                Id = 3,
                Name = "Clementine Bauch",
                UserName = "Samantha",
                Email = "Nathan@yesenia.net",
                AdressId = 3,
                Phone = "1-463-123-4447",
                Website = "ramiro.info",
                CompanyId = 3,
            },
            new User
            {
                Id = 4,
                Name = "Patricia Lebsack",
                UserName = "Karianne",
                Email = "Julianne.OConner@kory.org",
                AdressId = 4,
                Phone = "493-170-9623 x156",
                Website = "kale.biz",
                CompanyId = 4,
            },
            new User
            {
                Id = 5,
                Name = "Chelsey Dietrich",
                UserName = "Kamren",
                Email = "Lucio_Hettinger@annie.ca",
                AdressId = 5,
                Phone = "(254)954-1289",
                Website = "demarco.info",
                CompanyId = 5,
            },
            new User
            {
                Id = 6,
                Name = "Mrs. Dennis Schulist",
                UserName = "Leopoldo_Corkery",
                Email = "Karley_Dach@jasper.info",
                AdressId = 6,
                Phone = "1-477-935-8478 x6430",
                Website = "ola.org",
                CompanyId = 6,
            },
            new User 
            {
                Id = 7,
                Name = "Kurtis Weissnat",
                UserName = "Elwyn.Skiles",
                Email = "Telly.Hoeger@billy.biz",
                AdressId = 7,
                Phone = "210.067.6132",
                Website = "elvis.io",
                CompanyId = 7,
            },
            new User 
            {
                Id = 8,
                Name = "Nicholas Runolfsdottir V",
                UserName = "Maxime_Nienow",
                Email = "Sherwood@rosamond.me",
                AdressId = 8,
                Phone = "586.493.6943 x140",
                Website = "jacynthe.com",
                CompanyId = 8,
            },
            new User
            {
                Id = 9,
                Name = "Glenna Reichert",
                UserName = "Delphine",
                Email = "Chaim_McDermott@dana.io",
                AdressId = 9,
                Phone = "(775)976-6794 x41206",
                Website = "conrad.com",
                CompanyId = 9,
            },
            new User
            {
                Id = 10,
                Name = "Clementina DuBuque",
                UserName = "Moriah.Stanton",
                Email = "Rey.Padberg@karina.biz",
                AdressId = 10,
                Phone = "024-648-3804",
                Website = "ambrose.net",
                CompanyId = 10,
            }
            );

        }
    }
}
