using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VEGA_Data.Database;

namespace VEGA_Data.Files
{
    public class File : VegaObject
    {
        public long Id { get; set; }
        [StringLength(256, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String Filename { get; set; }
        public long ExtensionId { get; set; }
        public long User { get; set; }
        public DateTimeOffset UploadTime { get; set; }
        public long Size { get; set; }
        public byte[] Hash { get; set; }
    }
}
