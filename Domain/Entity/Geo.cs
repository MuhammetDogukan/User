﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Geo
    {
        public int Id { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
