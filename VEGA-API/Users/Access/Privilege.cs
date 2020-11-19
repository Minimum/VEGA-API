using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VEGA_API.Database;

namespace VEGA_API.Users.Access
{
    public class Privilege : VegaObject
    {
        public long Id { get; set; }
        public long Version { get; set; }
        public long System { get; set; }
        public bool Deleted { get; set; }
        [StringLength(32, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String Name { get; set; }
        [StringLength(256, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String Desc { get; set; }
    }
}
