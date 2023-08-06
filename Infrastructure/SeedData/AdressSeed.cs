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
    public class AdressSeed : IEntityTypeConfiguration<Adress>
    {
        public void Configure(EntityTypeBuilder<Adress> builder)
        {
            builder.HasData(new Adress
            {
                Id = 1,
                Street = "Kulas Light",
                Suite = "Apt. 556",
                City = "Gwenborough",
                ZipCode = "92998-3874",
                GeoId = 1,
            },
            new Adress
            {
                Id = 2,
                Street = "Victor Plains",
                Suite = "Suite 879",
                City = "Wisokyburgh",
                ZipCode = "90566-7771",
                GeoId = 2,
            },
            new Adress
            {
                Id = 3,
                Street = "Douglas Extension",
                Suite = "Suite 847",
                City = "McKenziehaven",
                ZipCode = "59590-4157",
                GeoId = 3,
            },
            new Adress
            {
                Id = 4,
                Street = "Hoeger Mall",
                Suite = "Apt. 692",
                City = "South Elvis",
                ZipCode = "53919-4257",
                GeoId = 4,
            },
            new Adress
            {
                Id = 5,
                Street = "Skiles Walks",
                Suite = "Suite 351",
                City = "Roscoeview",
                ZipCode = "33263",
                GeoId = 5,
            },
            new Adress
            {
                Id = 6,
                Street = "Norberto Crossing",
                Suite = "Apt. 950",
                City = "South Christy",
                ZipCode = "23505-1337",
                GeoId = 6,
            },
            new Adress
            {
                Id = 7,
                Street = "Rex Trail",
                Suite = "Suite 280",
                City = "Howemouth",
                ZipCode = "58804-1099",
                GeoId = 7,
            },
            new Adress
            {
                Id = 8,
                Street = "Ellsworth Summit",
                Suite = "Suite 729",
                City = "Aliyaview",
                ZipCode = "45169",
                GeoId = 8,
            },
            new Adress
            {
                Id = 9,
                Street = "Dayna Park",
                Suite = "Suite 449",
                City = "Bartholomebury",
                ZipCode = "76495-3109",
                GeoId = 9,
            },
            new Adress
            {
                Id = 10,
                Street = "Kattie Turnpike",
                Suite = "Suite 198",
                City = "Lebsackbury",
                ZipCode = "31428-2261",
                GeoId = 10,
            }
            );
        }
    }
}
