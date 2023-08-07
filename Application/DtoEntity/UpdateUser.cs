using Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DtoEntity
{
    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? UserName { get; set; }
        [Required]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }

        public Adress? Adress { get; set; }
        public Company? Company { get; set; }
    }
}
