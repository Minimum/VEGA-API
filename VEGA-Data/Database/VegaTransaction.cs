using Newtonsoft.Json;
using Npgsql;
using System;
using System.Net;
using System.Net.Http;
using VEGA_Data.DAL;

namespace VEGA_Data.Database
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class VegaTransaction
    {
        // System Variables
        public NpgsqlConnection Connection { get; set; }
        public NpgsqlTransaction Transaction { get; set; }
        [JsonProperty]
        public Object Data { get; set; }

        // User Variables
        public long SessionId { get; set; }
        public Guid SessionToken { get; set; }

        // Network Variables
        public IPAddress Address { get; set; }

        // Action Variables
        public bool RequireAuthentication { get; set; }
        public int? AuthSystem { get; set; }
        public long? AuthPrivilege { get; set; }

        // Error Variables
        [JsonProperty]
        public String ErrorCode { get; set; }
        [JsonProperty]
        public String ErrorMessage { get; set; }

        public VegaTransaction()
        {
            Connection = new NpgsqlConnection(VegaConfig.Instance.DataConnectionString);

            RequireAuthentication = true;
            AuthSystem = null;
            AuthPrivilege = null;

            ErrorCode = "";
            ErrorMessage = "";
        }

        public abstract VegaTransactionInitStatus Initialize();

        public String RestSelectMany(RestDal dal)
        {
            if (Initialize() != VegaTransactionInitStatus.Success)
            {
                return EndTransaction();
            }

            try
            {
                Data = dal.SelectMany();
            }
            catch (PostgresException e)
            {
                SetError(e);
            }
            catch (Exception)
            {
                SetDataError();
            }

            return EndTransaction();
        }

        public String RestSelect(RestDal dal)
        {
            if (Initialize() != VegaTransactionInitStatus.Success)
            {
                return EndTransaction();
            }

            try
            {
                Data = dal.Select();
            }
            catch (PostgresException e)
            {
                SetError(e);
            }
            catch (Exception)
            {
                SetDataError();
            }

            return EndTransaction();
        }

        public String RestInsert(RestDal dal)
        {
            if (Initialize() != VegaTransactionInitStatus.Success)
            {
                return EndTransaction();
            }

            try
            {
                Data = dal.Insert();
            }
            catch (PostgresException e)
            {
                SetError(e);
            }
            catch (Exception)
            {
                SetDataError();
            }

            return EndTransaction();
        }

        public String RestUpdate(RestDal dal)
        {
            if (Initialize() != VegaTransactionInitStatus.Success)
            {
                return EndTransaction();
            }

            try
            {
                Data = dal.Update();
            }
            catch (PostgresException e)
            {
                SetError(e);
            }
            catch (Exception)
            {
                SetDataError();
            }

            return EndTransaction();
        }

        public String RestDelete(RestDal dal)
        {
            if (Initialize() != VegaTransactionInitStatus.Success)
            {
                return EndTransaction();
            }

            try
            {
                dal.Delete();
            }
            catch (PostgresException e)
            {
                SetError(e);
            }
            catch (Exception)
            {
                SetDataError();
            }

            return EndTransaction();
        }

        public String RestGetFlags(RestDal dal)
        {
            if (Initialize() != VegaTransactionInitStatus.Success)
            {
                return EndTransaction();
            }

            try
            {
                Data = dal.GetFlags();
            }
            catch (PostgresException e)
            {
                SetError(e);
            }
            catch (Exception)
            {
                SetDataError();
            }

            return EndTransaction();
        }

        public String RestGetFlag(RestDal dal, int flagId)
        {
            if (Initialize() != VegaTransactionInitStatus.Success)
            {
                return EndTransaction();
            }

            try
            {
                Data = dal.GetFlag(flagId);
            }
            catch (PostgresException e)
            {
                SetError(e);
            }
            catch (Exception)
            {
                SetDataError();
            }

            return EndTransaction();
        }

        public String RestGetFlagValues(RestDal dal)
        {
            if (Initialize() != VegaTransactionInitStatus.Success)
            {
                return EndTransaction();
            }

            try
            {
                Data = dal.GetFlagValues();
            }
            catch (PostgresException e)
            {
                SetError(e);
            }
            catch (Exception)
            {
                SetDataError();
            }

            return EndTransaction();
        }

        public String RestGetFlagValue(RestDal dal, int flagId)
        {
            if (Initialize() != VegaTransactionInitStatus.Success)
            {
                return EndTransaction();
            }

            try
            {
                Data = dal.GetFlagValue(flagId);
            }
            catch (PostgresException e)
            {
                SetError(e);
            }
            catch (Exception)
            {
                SetDataError();
            }

            return EndTransaction();
        }

        public String RestSetFlagValue(RestDal dal, int flagId, bool value)
        {
            if (Initialize() != VegaTransactionInitStatus.Success)
            {
                return EndTransaction();
            }

            try
            {
                dal.SetFlagValue(flagId, value);
            }
            catch (PostgresException e)
            {
                SetError(e);
            }
            catch (Exception)
            {
                SetDataError();
            }

            return EndTransaction();
        }

        public String EndTransaction()
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("CALL pl_system.sp_end_transaction();", Connection, Transaction))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception) { }

            try
            {
                Transaction.Commit();
                Transaction.Dispose();
            }
            catch (Exception) { }

            try
            {
                Connection.Close();
                Connection.Dispose();
            }
            catch (Exception) { }

            return GetResponse();
        }

        public String Close()
        {
            Connection.Close();
            Connection.Dispose();

            return GetResponse();
        }

        public void SetError(String errorCode, String errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;

            return;
        }

        public void SetError(PostgresException exception)
        {
            ErrorCode = exception.SqlState;
            ErrorMessage = exception.MessageText;

            return;
        }

        public void SetDataError()
        {
            SetError(VegaRules.ErrorGeneral, "Sorry, the API is currently experiencing issues.");

            return;
        }

        public void SetAccessError()
        {
            SetError(VegaRules.ErrorAccess, "Access denied.");

            return;
        }

        public String GetEnvironmentInfo()
        {
            return "Address: " + Address.ToString();
        }

        public String GetResponse()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
