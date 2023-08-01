using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Adress
    {
        public int Id { get; set; }
        public string? Street { get; set; }
        public string? Suite { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        [ForeignKey("Geo")]
        public int? GeoId { get; set; }

        public Geo? Geo { get; set; } 

    }
}
