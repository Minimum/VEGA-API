using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VEGA_API.Database;
using VEGA_API.Users.Access;

namespace VEGA_API.Users
{
    public class User : VegaObject
    {
        public long Id { get; set; }
        public int Type { get; set; }
        public bool Deleted { get; set; }
        public List<Role> Roles { get; set; }

        public override VegaObject Mask()
        {
            return new User
            {
                Id = Id,
                Type = Type,
                Deleted = Deleted
            };
        }
    }
}
