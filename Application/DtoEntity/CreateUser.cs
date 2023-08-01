using Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DtoEntity
{
    public class CreateUser
    {
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public string? Role { get; set; }

        [ForeignKey("Company")]
        public int? CompanyId { get; set; }
        [ForeignKey("Adress")]
        public int? AdressId { get; set; }

        public Adress? Adress { get; set; }
        public Company? Company { get; set; }
    }
}
