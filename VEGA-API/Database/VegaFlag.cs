using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VEGA_API.Database
{
    public class VegaFlag
    {
        public int Id { get; set; }
        [StringLength(32, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String Desc { get; set; }
    }
}
