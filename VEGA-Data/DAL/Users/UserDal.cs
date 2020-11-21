using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VEGA_Data.Database;
using VEGA_Data.Users;

namespace VEGA_Data.DAL.Users
{
    public class UserDal : RestDal
    {
        public User Input { get; set; }

        public UserDal(VegaTransaction transaction) 
            : base(transaction)
        {
            Schema = "pl_user";
            Table = "users";
            FlagsTable = "user_flags";
            FlagValuesTable = "user_flag_values";

            Columns = new List<VegaColumn>
            {
                new VegaColumn { Order = 0, Name = "dbid", Parameter = "id", PrimaryKey = true, Nullable = false },
                new VegaColumn { Order = 1, Name = "dbuser_type", Parameter = "type", PrimaryKey = false, Nullable = false },
                new VegaColumn { Order = 2, Name = "dbdeleted", Parameter = "deleted", PrimaryKey = false, Nullable = false }
            };
        }
        
        protected override void SetPrimaryKeyParameters(NpgsqlCommand cmd)
        {
            cmd.Parameters.Add(new NpgsqlParameter("id", Input.Id));
        }

        protected override void SetDataParameters(NpgsqlCommand cmd)
        {
            cmd.Parameters.Add(new NpgsqlParameter("id", Input.Id));
            cmd.Parameters.Add(new NpgsqlParameter("type", Input.Type));
            cmd.Parameters.Add(new NpgsqlParameter("deleted", Input.Deleted));
        }

        protected override VegaObject ReadObject(NpgsqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt64(0),
                Type = reader.GetInt32(1),
                Deleted = reader.GetBoolean(2)
            };
        }

        protected override VegaFlagValue ReadFlagValue(NpgsqlDataReader reader)
        {
            return new UserFlagValue(base.ReadFlagValue(reader)) { Id = reader.GetInt64(2) };
        }
    }
}
