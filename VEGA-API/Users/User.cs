using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VEGA_API.Database;

namespace VEGA_API.Users
{
    public class User : VegaObject
    {
        public long Id { get; set; }
        public int Type { get; set; }
        public bool Deleted { get; set; }
    }
}
