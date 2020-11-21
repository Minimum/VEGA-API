using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VEGA_Data.Database;

namespace VEGA_Data.DAL
{
    public abstract class VegaDal
    {
        protected readonly VegaTransaction VegaTransaction;
        protected readonly NpgsqlConnection Connection;
        protected readonly NpgsqlTransaction Transaction;

        public VegaDal(VegaTransaction transaction)
        {
            VegaTransaction = transaction;
            Connection = transaction.Connection;
            Transaction = transaction.Transaction;
        }
    }
}
