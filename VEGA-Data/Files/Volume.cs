using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VEGA_Data.Database;

namespace VEGA_Data.Files
{
    public class Volume : VegaObject
    {
        public long Id { get; set; }
        [StringLength(32, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String Name { get; set; }
        [StringLength(128, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String Desc { get; set; }
        [StringLength(1024, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String Path { get; set; }

        public override VegaObject Mask()
        {
            return new Volume
            {
                Id = Id,
                Name = Name,
                Desc = Desc
            };
        }
    }
}
