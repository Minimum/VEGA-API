using System;
using System.Collections.Generic;
using System.Text;
using VEGA_Data.Database;

namespace VEGA_Data.Users
{
    public class UserFlagValue : VegaFlagValue
    {
        public UserFlagValue(VegaFlagValue value)
            : base(value)
        {

        }

        public long Id { get; set; }
    }
}
