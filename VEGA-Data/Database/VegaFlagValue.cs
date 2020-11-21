using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VEGA_Data.Database
{
    public class VegaFlagValue
    {
        public VegaFlagValue()
        {
            
        }

        public VegaFlagValue(VegaFlagValue value)
        {
            Flag = value.Flag;
            Value = value.Value;
        }

        public int Flag { get; set; }
        public bool Value { get; set; }
    }
}
