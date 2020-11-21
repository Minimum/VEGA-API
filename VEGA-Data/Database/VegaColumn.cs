using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VEGA_Data.Database
{
    public class VegaColumn
    {
        public int Order { get; set; }
        public String Parameter { get; set; }
        public String Name { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Nullable { get; set; }
    }
}
