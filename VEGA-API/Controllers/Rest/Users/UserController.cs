using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VEGA_API.Database;
using VEGA_Data.Database;
using VEGA_Data.Users;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VEGA_API.Controllers.Rest.User
{
    [Route("rest/users/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public const long PvSelectUser = 1;
        public const long PvInsertUser = 2;
        public const long PvUpdateUser = 3;
        public const long PvDeleteUser = 4;

        // GET: api/<UserController>
        [HttpGet]
        public String Get()
        {
            VegaHttpTransaction transaction = new VegaHttpTransaction(this)
            {
                AuthSystem = UserService.System,
                AuthPrivilege = PvSelectUser
            };

            if(transaction.Initialize() == VegaTransactionInitStatus.Success)
            {
                UserService service = new UserService(transaction);
            }

            return transaction.EndTransaction();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(long id)
        {
            VegaHttpTransaction transaction = new VegaHttpTransaction(this)
            {
                AuthSystem = UserService.System,
                AuthPrivilege = PvSelectUser
            };
            
            if (transaction.Initialize() == VegaTransactionInitStatus.Success)
            {
                UserService service = new UserService(transaction);

                if (service.ValidateUserId(id))
                {
                    service.GetUser(id);
                }
                else
                {
                    transaction.SetError(VegaRules.ErrorValidation, "Invalid user id.");
                }
            }

            return transaction.EndTransaction();
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(long id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(long id)
        {
        }
    }
}
