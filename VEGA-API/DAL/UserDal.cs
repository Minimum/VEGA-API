using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VEGA_API.Database;
using VEGA_API.Users;

namespace VEGA_API.DAL
{
    public class UserDal : RestDal
    {
        public UserDal(VegaTransaction transaction) : base(transaction)
        {
        }

        public User Select(long id)
        {
            User user = null;

            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM pl_user.api_rest_user(@id);", Connection, Transaction))
            {
                cmd.Parameters.AddWithValue("id", id);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    user = new User
                    {
                        Id = reader.GetInt64(0),
                        Type = reader.GetInt32(1),
                        Deleted = reader.GetBoolean(2)
                    };
                }
            }

            return user;
        }
    }
}
