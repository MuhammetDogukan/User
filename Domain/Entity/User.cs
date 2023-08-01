using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class User : IdentityUser<int>
    {
        [Key]
        public override int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public override string? UserName { get; set; }
        [Required]
        public override string? Email { get; set; }
        public override string? PasswordHash { get; set; }
        [Required]
        [ForeignKey("Adress")]
        public int? AdressId { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        [ForeignKey("Company")]
        public int? CompanyId { get; set; }
        
        public string? Role { get; set; }
        public bool IsDeleted { get; set; }


        public Adress? Adress { get; set; }
        public Company? Company { get; set; }

    }
}
