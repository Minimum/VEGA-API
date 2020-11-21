using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VEGA_Data.Users.Access
{
    public class RoleGroup
    {
        public long Id { get; set; }
        public long Version { get; set; }
        public bool Deleted { get; set; }
        public String Name { get; set; }
        public String Desc { get; set; }
        public List<Role> Roles { get; set; }
    }
}
