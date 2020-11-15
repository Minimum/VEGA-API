using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VEGA_API.Database;

namespace VEGA_API.Files
{
    public class FileExtensionFlagValue : VegaFlagValue
    {
        public long ExtensionId { get; set; }
    }
}
