using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VEGA_API.Database;

namespace VEGA_API.Files
{
    public class MimeType : VegaObject
    {
        [StringLength(128, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String Type { get; set; }
        [StringLength(32, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String Desc { get; set; }
    }
}
