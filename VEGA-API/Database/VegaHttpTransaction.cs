using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Net;
using System.Net.Http;
using VEGA_Data.DAL;
using VEGA_Data.Database;

namespace VEGA_API.Database
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VegaHttpTransaction : VegaTransaction
    {
        // Constants
        public const String HeaderAddress = "X-VEGA-Address";

        public const String CookieSessionId = "VEGA-SessionId";
        public const String CookieSessionToken = "VEGA-SessionToken";

        // Network Variables
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }

        public VegaHttpTransaction()
            : base()
        {
            Request = null;
            Response = null;
        }

        public VegaHttpTransaction(HttpRequest request)
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

        public VegaHttpTransaction(HttpRequest request, HttpResponse response)
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

        public VegaHttpTransaction(ControllerBase controller)
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

        public override VegaTransactionInitStatus Initialize()
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
    }
}
