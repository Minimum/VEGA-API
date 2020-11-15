using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VEGA_API.Database;

namespace VEGA_API.DAL
{
    public abstract class RestDal : VegaDal
    {
        protected String SelectManyQuery;
        protected String SelectQuery;
        protected String InsertQuery;
        protected String UpdateQuery;
        protected String DeleteQuery;

        protected String GetFlagsQuery;
        protected String GetFlagQuery;

        protected String GetFlagValuesQuery;
        protected String GetFlagValueQuery;
        protected String SetFlagValueQuery;

        public RestDal(VegaTransaction transaction)
            : base(transaction)
        {

        }

        public virtual ICollection<VegaObject> SelectMany()
        {
            List<VegaObject> objs = new List<VegaObject>();

            using (NpgsqlCommand cmd = new NpgsqlCommand(SelectQuery, Connection, Transaction))
            {
                SetSelectManyParameters(cmd);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        objs.Add(ReadObject(reader));
                    }
                }
            }

            return objs;
        }

        protected virtual void SetSelectManyParameters(NpgsqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        public virtual VegaObject Select()
        {
            VegaObject obj = null;

            using (NpgsqlCommand cmd = new NpgsqlCommand(SelectQuery, Connection, Transaction))
            {
                SetSelectParameters(cmd);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    obj = ReadObject(reader);
                }
            }

            return obj;
        }

        protected virtual void SetSelectParameters(NpgsqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        public virtual VegaObject Insert()
        {
            VegaObject obj = null;

            using (NpgsqlCommand cmd = new NpgsqlCommand(InsertQuery, Connection, Transaction))
            {
                SetInsertParameters(cmd);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    obj = ReadObject(reader);
                }
            }

            return obj;
        }

        protected virtual void SetInsertParameters(NpgsqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        public virtual VegaObject Update()
        {
            VegaObject obj = null;

            using (NpgsqlCommand cmd = new NpgsqlCommand(UpdateQuery, Connection, Transaction))
            {
                SetUpdateParameters(cmd);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    obj = ReadObject(reader);
                }
            }

            return obj;
        }

        protected virtual void SetUpdateParameters(NpgsqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete()
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand(DeleteQuery, Connection, Transaction))
            {
                SetDeleteParameters(cmd);

                cmd.ExecuteNonQuery();
            }

            return;
        }

        protected virtual void SetDeleteParameters(NpgsqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        public virtual ICollection<VegaFlag> GetFlags()
        {
            List<VegaFlag> flags = new List<VegaFlag>();

            using (NpgsqlCommand cmd = new NpgsqlCommand(GetFlagsQuery, Connection, Transaction))
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    VegaFlag flag = new VegaFlag
                    {
                        Id = reader.GetInt32(0),
                        Desc = reader.GetString(1)
                    };

                    flags.Add(flag);
                }
            }

            return flags;
        }

        public virtual VegaFlag GetFlag(int flagId)
        {
            VegaFlag flag = null;

            using (NpgsqlCommand cmd = new NpgsqlCommand(GetFlagQuery, Connection, Transaction))
            {
                SetGetFlagParameters(cmd, flagId);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    flag = new VegaFlag
                    {
                        Id = reader.GetInt32(0),
                        Desc = reader.GetString(1)
                    };
                }
            }

            return flag;
        }

        protected virtual void SetGetFlagParameters(NpgsqlCommand cmd, int flagId)
        {
            throw new NotImplementedException();
        }

        public virtual ICollection<VegaFlagValue> GetFlagValues()
        {
            List<VegaFlagValue> flags = new List<VegaFlagValue>();

            using (NpgsqlCommand cmd = new NpgsqlCommand(GetFlagValuesQuery, Connection, Transaction))
            {
                SetGetFlagValuesParameters(cmd);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        flags.Add(ReadFlagValue(reader));
                    }
                }
            }

            return flags;
        }

        protected virtual void SetGetFlagValuesParameters(NpgsqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        public virtual VegaFlagValue GetFlagValue(int flagId)
        {
            VegaFlagValue flag = null;

            using (NpgsqlCommand cmd = new NpgsqlCommand(GetFlagValueQuery, Connection, Transaction))
            {
                SetGetFlagValueParameters(cmd, flagId);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    flag = ReadFlagValue(reader);
                }
            }

            return flag;
        }

        protected virtual void SetGetFlagValueParameters(NpgsqlCommand cmd, int flagId)
        {
            throw new NotImplementedException();
        }

        public virtual void SetFlagValue(int flagId, bool value)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand(SetFlagValueQuery, Connection, Transaction))
            {
                SetSetFlagValueParameters(cmd, flagId, value);

                cmd.ExecuteNonQuery();
            }

            return;
        }

        protected virtual void SetSetFlagValueParameters(NpgsqlCommand cmd, int flagId, bool value)
        {
            throw new NotImplementedException();
        }

        protected virtual VegaObject ReadObject(NpgsqlDataReader reader)
        {
            throw new NotImplementedException();
        }

        protected virtual VegaFlagValue ReadFlagValue(NpgsqlDataReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
