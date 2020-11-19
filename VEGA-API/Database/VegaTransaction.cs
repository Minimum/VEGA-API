using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Net;
using System.Net.Http;
using VEGA_API.DAL;

namespace VEGA_API.Database
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VegaTransaction
    {
        // Constants
        public const String HeaderAddress = "X-VEGA-Address";

        public const String CookieSessionId = "VEGA-SessionId";
        public const String CookieSessionToken = "VEGA-SessionToken";

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
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }

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
            Request = null;
            Response = null;
        }

        public VegaTransaction(HttpRequest request)
        {
            Connection = new NpgsqlConnection(VegaConfig.Instance.DataConnectionString);

            RequireAuthentication = true;
            AuthSystem = null;
            AuthPrivilege = null;

            ErrorCode = "";
            ErrorMessage = "";
            Request = request;
            Response = null;
        }

        public VegaTransaction(HttpRequest request, HttpResponse response)
        {
            Connection = new NpgsqlConnection(VegaConfig.Instance.DataConnectionString);

            RequireAuthentication = true;
            AuthSystem = null;
            AuthPrivilege = null;

            ErrorCode = "";
            ErrorMessage = "";
            Request = request;
            Response = response;
        }

        public VegaTransaction(ControllerBase controller)
        {
            Connection = new NpgsqlConnection(VegaConfig.Instance.DataConnectionString);

            RequireAuthentication = true;
            AuthSystem = null;
            AuthPrivilege = null;

            ErrorCode = "";
            ErrorMessage = "";
            Request = controller.Request;
            Response = controller.Response;
        }

        public VegaTransactionInitStatus Initialize()
        {
            VegaTransactionInitStatus success = VegaTransactionInitStatus.Error;
            long sessionId;
            Guid sessionToken;
            IPAddress address;

            if (Request.Cookies.ContainsKey(CookieSessionId))
            {
                Int64.TryParse(Request.Cookies[CookieSessionId].ToString(), out sessionId);

                SessionId = sessionId;
            }

            if (Request.Cookies.ContainsKey(CookieSessionToken))
            {
                Guid.TryParse(Request.Cookies[CookieSessionToken].ToString(), out sessionToken);

                SessionToken = sessionToken;
            }

            if (Request.Headers.ContainsKey(HeaderAddress))
            {
                // Expecting remote address header from NGINX
                if (IPAddress.TryParse(Request.Headers[HeaderAddress].ToString(), out address))
                {
                    Address = address;
                }
                else
                {
                    Address = Request.HttpContext.Connection.RemoteIpAddress;
                }
            }
            else
            {
                Address = Request.HttpContext.Connection.RemoteIpAddress;
            }

            Connection.Open();

            Transaction = Connection.BeginTransaction();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM pl_system.fn_start_transaction(@session_id, @session_token, @address, @environment_info, @require_auth, @auth_system, @auth_privilege);", Connection, Transaction);

            cmd.Parameters.AddWithValue("session_id", SessionId);
            cmd.Parameters.AddWithValue("session_token", SessionToken);
            cmd.Parameters.AddWithValue("address", Address);
            cmd.Parameters.AddWithValue("environment_info", GetEnvironmentInfo());
            cmd.Parameters.AddWithValue("require_auth", RequireAuthentication);
            cmd.Parameters.AddWithValue("auth_system", NpgsqlTypes.NpgsqlDbType.Integer, AuthSystem);
            cmd.Parameters.AddWithValue("auth_privilege", NpgsqlTypes.NpgsqlDbType.Integer, AuthPrivilege);

            try
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        SetDataError();
                    }
                    else
                    {
                        long newSessionId = reader.GetInt64(0);

                        if (newSessionId != SessionId)
                        {
                            SessionId = newSessionId;
                            SessionToken = reader.GetGuid(1);
                            DateTimeOffset newSessionExpiration = reader.GetDateTime(2);

                            SetClientCookies(newSessionExpiration);
                        }

                        success = reader.GetBoolean(3) ? VegaTransactionInitStatus.Success : VegaTransactionInitStatus.AuthFailed;

                        if (success == VegaTransactionInitStatus.AuthFailed)
                            SetAccessError();
                    }
                }
            }
            catch (PostgresException e)
            {
                SetError(e);
            }
            catch (NpgsqlException)
            {
                SetDataError();
            }

            cmd.Dispose();

            return success;
        }

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

        public void SetClientCookies(DateTimeOffset expiration)
        {
            CookieOptions options = new CookieOptions
            {
                Expires = expiration,
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append(CookieSessionId, SessionId.ToString(), options);
            Response.Cookies.Append(CookieSessionToken, SessionToken.ToString(), options);

            return;
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
