using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VEGA_API.Database;

namespace VEGA_API.Files
{
    public class FileExtension : VegaObject
    {
        public long Id { get; set; }
        [StringLength(64, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String Extension { get; set; }
        [StringLength(128, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String MimeType { get; set; }
        [StringLength(128, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String MimeSubType { get; set; }
        [StringLength(32, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String Desc { get; set; }
    }
}
