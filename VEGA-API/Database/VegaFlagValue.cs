using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VEGA_API.Database
{
    public abstract class FlagValue
    {
        public int Flag { get; set; }
        public bool Value { get; set; }
    }
}
