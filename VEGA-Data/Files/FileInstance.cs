﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VEGA_Data.Database;

namespace VEGA_Data.Files
{
    public class FileInstance : VegaObject
    {
        public long File { get; set; }
        public long Volume { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
