using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VEGA_Data.DAL.Users;
using VEGA_Data.Database;

namespace VEGA_Data.Users
{
    public sealed class UserService
    {
        public const int System = 2;

        private UserDal Dal { get; set; }

        public UserService(VegaTransaction transaction)
        {
            Dal = new UserDal(transaction);
        }

        public bool ValidateUserId(long id)
        {
            return id > 0;
        }

        public User GetUser(long id)
        {
         

            return (User) Dal.Select();
        }
    }
}
