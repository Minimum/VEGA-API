using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VEGA_Data.Database;

namespace VEGA_Data.Files
{
    public class MimeSubType : VegaObject
    {
        [StringLength(128, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String SubType { get; set; }
        [StringLength(128, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String MimeType { get; set; }
        [StringLength(32, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String Desc { get; set; }
    }
}
