using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VEGA_Data.Database;

namespace VEGA_Data.Users.Access
{
    public class AccessAttempt : VegaObject
    {
        public long Id { get; set; }
        public long Privilege { get; set; }
        public long Session { get; set; }
        public bool Success { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public IPAddress Address { get; set; }
        [StringLength(4096, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String EnvironmentInfo { get; set; }
        [StringLength(128, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public String Message { get; set; }

        public override VegaObject Mask()
        {
            return new AccessAttempt
            {
                Id = Id,
                Privilege = Privilege,
                Session = Session,
                Success = Success,
                Timestamp = Timestamp
            };
        }
    }
}
