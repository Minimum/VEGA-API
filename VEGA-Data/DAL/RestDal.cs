using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VEGA_Data.Database;

namespace VEGA_Data.DAL
{
    public abstract class RestDal : VegaDal
    {
        protected String SelectManyQuery { get; set; }
        protected String SelectQuery { get; set; }
        protected String InsertQuery { get; set; }
        protected String UpdateQuery { get; set; }
        protected String UpdateNullsQuery { get; set; }
        protected String DeleteQuery { get; set; }

        protected String GetFlagsQuery { get; set; }
        protected String GetFlagQuery { get; set; }

        protected String GetFlagValuesQuery { get; set; }
        protected String GetFlagValueQuery { get; set; }
        protected String SetFlagValueQuery { get; set; }

        public long SelectManyOffset { get; set; }
        public long SelectManyLimit { get; set; }
        public bool UpdateUseNulls { get; set; }

        protected String Schema { get; set; }
        protected String Table { get; set; }
        protected String FlagsTable { get; set; }
        protected String FlagValuesTable { get; set; }
        protected List<VegaColumn> Columns { get; set; }

        public RestDal(VegaTransaction transaction)
            : base(transaction)
        {

        }

        public ICollection<VegaObject> SelectMany()
        {
            List<VegaObject> objs = new List<VegaObject>();

            if (SelectManyQuery == null)
                SelectManyQuery = BuildSelectManyQuery();

            using (NpgsqlCommand cmd = new NpgsqlCommand(SelectQuery, Connection, Transaction))
            {
                cmd.Parameters.Add(new NpgsqlParameter("SelectManyOffset", SelectManyOffset));
                cmd.Parameters.Add(new NpgsqlParameter("SelectManyLimit", SelectManyLimit));

                SetPrimaryKeyParameters(cmd);

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

        protected String BuildSelectManyQuery()
        {
            String query = "SELECT " + Columns[0].Name;
            int columnCount = Columns.Count;

            for(int i = 1; i < columnCount; i++)
            {
                query += ", " + Columns[i].Name;
            }

            query += " FROM " + Schema + "." + Table + " LIMIT @SelectManyLimit OFFSET @SelectManyOffset;";

            return query;
        }

        public VegaObject Select()
        {
            VegaObject obj = null;

            if (SelectQuery == null)
                SelectQuery = BuildSelectQuery();

            using (NpgsqlCommand cmd = new NpgsqlCommand(SelectQuery, Connection, Transaction))
            {
                SetPrimaryKeyParameters(cmd);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    obj = ReadObject(reader);
                }
            }

            return obj;
        }

        protected String BuildSelectQuery()
        {
            String query = "SELECT " + Columns[0].Name;
            List<VegaColumn> primaryKey = GetPrimaryKey();
            int columnCount = Columns.Count;
            int primaryKeyCount = primaryKey.Count;

            for(int i = 1; i < columnCount; i++)
            {
                query += ", " + Columns[i].Name;
            }

            query += " FROM " + Schema + "." + Table + " WHERE " + primaryKey[0].Name + " = @" + primaryKey[0].Parameter;

            for (int i = 1; i < primaryKeyCount; i++)
            {
                query += " AND " + primaryKey[i].Name + " = @" + primaryKey[i].Parameter;
            }

            query += ";";

            return query;
        }

        public VegaObject Insert()
        {
            VegaObject obj = null;

            if (InsertQuery == null)
                InsertQuery = BuildInsertQuery();

            using (NpgsqlCommand cmd = new NpgsqlCommand(InsertQuery, Connection, Transaction))
            {
                SetPrimaryKeyParameters(cmd);
                SetDataParameters(cmd);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    obj = ReadObject(reader);
                }
            }

            return obj;
        }

        protected String BuildInsertQuery()
        {
            String query = "INSERT INTO " + Schema + "." + Table + " (" + Columns[0].Name;
            int columnCount = Columns.Count;

            for(int i = 1; i < columnCount; i++)
            {
                query += ", " + Columns[i].Name;
            }

            query += ") VALUES (@" + Columns[0].Parameter;

            for (int i = 1; i < columnCount; i++)
            {
                query += ", " + Columns[i].Parameter;
            }

            query += ");";

            return query;
        }

        public VegaObject Update()
        {
            VegaObject obj = null;
            String query;

            if (UpdateUseNulls)
            {
                if (UpdateNullsQuery == null)
                    UpdateNullsQuery = BuildUpdateQuery(UpdateUseNulls);

                query = UpdateNullsQuery;
            }
            else
            {
                if (UpdateQuery == null)
                    UpdateQuery = BuildUpdateQuery(UpdateUseNulls);

                query = UpdateQuery;
            }

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, Connection, Transaction))
            {
                SetPrimaryKeyParameters(cmd);
                SetDataParameters(cmd);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    obj = ReadObject(reader);
                }
            }

            return obj;
        }

        protected String BuildUpdateQuery(bool useNulls)
        {
            String query = "UPDATE " + Schema + "." + Table + " SET ";
            List<VegaColumn> primaryKey = GetPrimaryKey();
            int columnCount = Columns.Count;
            int primaryKeyCount = primaryKey.Count;

            if(useNulls)
            {
                query += Columns[0].Name + " = @" + Columns[0].Parameter;

                for(int i = 1; i < columnCount; i++)
                {
                    query += ", " + Columns[i].Name + " = @" + Columns[i].Parameter;
                }
            }
            else
            {
                query += Columns[0].Name + " = COALESCE(" + Columns[0].Name + ", @" + Columns[0].Parameter + ")";

                for (int i = 1; i < columnCount; i++)
                {
                    query += ", " + Columns[i].Name + " = COALESCE(" + Columns[i].Name + ", @" + Columns[i].Parameter + ")";
                }
            }

            query += " WHERE " + primaryKey[0].Name + " = @" + primaryKey[0].Parameter;

            for(int i = 1; i < primaryKeyCount; i++)
            {
                query += " AND " + primaryKey[i].Name + " = @" + primaryKey[i].Parameter;
            }

            query += ";";

            return query;
        }

        public void Delete()
        {
            if (DeleteQuery == null)
                DeleteQuery = BuildDeleteQuery();

            using (NpgsqlCommand cmd = new NpgsqlCommand(DeleteQuery, Connection, Transaction))
            {
                SetPrimaryKeyParameters(cmd);

                cmd.ExecuteNonQuery();
            }

            return;
        }

        protected String BuildDeleteQuery()
        {
            List<VegaColumn> primaryKey = GetPrimaryKey();
            int primaryKeyCount = primaryKey.Count;
            String query = "DELETE FROM " + Schema + "." + Table + " WHERE " + primaryKey[0].Name + " = " + primaryKey[0].Parameter;

            for(int i = 1; i < primaryKeyCount; i++)
            {
                query += " AND " + primaryKey[i].Name + " = " + primaryKey[i].Parameter;
            }

            query += ";";

            return query;
        }

        public ICollection<VegaFlag> GetFlags()
        {
            List<VegaFlag> flags = new List<VegaFlag>();

            if (GetFlagsQuery == null)
                GetFlagsQuery = BuildGetFlagsQuery();

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

        protected String BuildGetFlagsQuery()
        {
            return "SELECT dbflag, dbdesc FROM " + Schema + "." + FlagsTable + ";";
        }

        public VegaFlag GetFlag(int flagId)
        {
            VegaFlag flag = null;

            if (GetFlagQuery == null)
                GetFlagQuery = BuildGetFlagQuery();

            using (NpgsqlCommand cmd = new NpgsqlCommand(GetFlagQuery, Connection, Transaction))
            {
                cmd.Parameters.Add(new NpgsqlParameter("VegaFlagId", flagId));

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

        protected String BuildGetFlagQuery()
        {
            return "SELECT dbflag, dbdesc FROM " + Schema + "." + FlagsTable + " WHERE dbflag = @VegaFlagId;";
        }

        public ICollection<VegaFlagValue> GetFlagValues()
        {
            List<VegaFlagValue> flags = new List<VegaFlagValue>();

            if (GetFlagValuesQuery == null)
                GetFlagValuesQuery = BuildGetFlagValuesQuery();

            using (NpgsqlCommand cmd = new NpgsqlCommand(GetFlagValuesQuery, Connection, Transaction))
            {
                SetPrimaryKeyParameters(cmd);

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

        protected String BuildGetFlagValuesQuery()
        {
            List<VegaColumn> primaryKey = GetPrimaryKey();
            int primaryKeyCount = primaryKey.Count;
            String query = "SELECT f.dbflag, COALESCE(v.dbvalue, FALSE), COALESCE(v." + primaryKey[0].Name + ", @" + primaryKey[0].Parameter + ")";

            for(int i = 1; i < primaryKeyCount; i++)
            {
                query += ", COALESCE(v." + primaryKey[i].Name + ", @" + primaryKey[i].Parameter + ")";
            }

            query += " FROM " + Schema + "." + FlagsTable + " f LEFT JOIN " + Schema + "." + FlagValuesTable + " v ON v.dbflag = f.dbflag";

            for(int i = 0; i < primaryKeyCount; i++)
            {
                query += " AND v." + primaryKey[i].Name + " = @" + primaryKey[i].Parameter;
            }

            query += ";";

            return query;
        }

        public VegaFlagValue GetFlagValue(int flagId)
        {
            VegaFlagValue flag = null;

            if (GetFlagValueQuery == null)
                GetFlagValueQuery = BuildGetFlagQuery();

            using (NpgsqlCommand cmd = new NpgsqlCommand(GetFlagValueQuery, Connection, Transaction))
            {
                cmd.Parameters.Add(new NpgsqlParameter("VegaFlagId", flagId));
                SetPrimaryKeyParameters(cmd);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    flag = ReadFlagValue(reader);
                }
            }

            return flag;
        }

        protected String BuildGetFlagValueQuery()
        {
            List<VegaColumn> primaryKey = GetPrimaryKey();
            int primaryKeyCount = primaryKey.Count;
            String query = "SELECT COALESCE(v." + primaryKey[0].Name + ", @" + primaryKey[0].Parameter + ")";

            for (int i = 1; i < primaryKeyCount; i++)
            {
                query += ", COALESCE(v." + primaryKey[i].Name + ", @" + primaryKey[i].Parameter + ")";
            }

            query += ", f.dbflag, COALESCE(v.dbvalue, FALSE) FROM " + Schema + "." + FlagsTable + " f LEFT JOIN " + Schema + "." + FlagValuesTable + " v ON v.dbflag = f.dbflag";

            for (int i = 0; i < primaryKeyCount; i++)
            {
                query += " AND v." + primaryKey[i].Name + " = @" + primaryKey[i].Parameter;
            }

            query += " WHERE f.dbflag = @VegaFlagId;";

            return query;
        }

        public void SetFlagValue(int flagId, bool value)
        {
            if (SetFlagValueQuery == null)
                SetFlagValueQuery = BuildSetFlagValueQuery();

            using (NpgsqlCommand cmd = new NpgsqlCommand(SetFlagValueQuery, Connection, Transaction))
            {
                cmd.Parameters.Add(new NpgsqlParameter("VegaFlagId", flagId));
                cmd.Parameters.Add(new NpgsqlParameter("VegaFlagValue", value));
                SetPrimaryKeyParameters(cmd);

                cmd.ExecuteNonQuery();
            }

            return;
        }

        protected String BuildSetFlagValueQuery()
        {
            List<VegaColumn> primaryKey = GetPrimaryKey();
            int primaryKeyCount = primaryKey.Count;
            String query = "MERGE INTO " + Schema + "." + FlagValuesTable + "t USING (SELECT @flag dbflag, @value dbvalue";

            for(int i = 0; i < primaryKeyCount; i++)
            {
                query += ", @" + primaryKey[i].Parameter + " " + primaryKey[i].Name;
            }

            query += ") i ON (t.dbflag = i.dbflag";

            for(int i = 0; i < primaryKeyCount; i++)
            {
                query += " AND t." + primaryKey[i].Name + " = i." + primaryKey[i].Name;
            }

            query += ") WHEN MATCHED THEN UPDATE SET dbvalue = i.dbvalue WHEN NOT MATCHED THEN INSERT (dbflag, dbvalue";

            for(int i = 0; i < primaryKeyCount; i++)
            {
                query += ", " + primaryKey[i].Name;
            }

            query += ") VALUES (i.dbflag, i.dbvalue";

            for(int i = 0; i < primaryKeyCount; i++)
            {
                query += ", i." + primaryKey[i].Name;
            }

            query += ");";

            return query;
        }

        protected virtual void SetPrimaryKeyParameters(NpgsqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        protected virtual void SetDataParameters(NpgsqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        protected virtual VegaObject ReadObject(NpgsqlDataReader reader)
        {
            throw new NotImplementedException();
        }

        protected virtual VegaFlagValue ReadFlagValue(NpgsqlDataReader reader)
        {
            return new VegaFlagValue { Flag = reader.GetInt32(0), Value = reader.GetBoolean(1) };
        }

        protected List<VegaColumn> GetPrimaryKey()
        {
            return (from column in Columns
                    where column.PrimaryKey
                    select column).ToList();
        }
    }
}
