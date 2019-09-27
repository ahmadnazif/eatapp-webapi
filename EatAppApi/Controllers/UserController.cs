using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EatAppApi.Helpers;
using EatAppApi.Models;
using EatAppApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EatAppApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IDbHelper dbHelper;
        public UserController(IDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        [HttpGet("get-by-id")]
        public ActionResult<User> GetUserById()
        {
            var id = Request.Query["id"];
            if (!StringValues.IsNullOrEmpty(id))
            {
                var succ = int.TryParse(id, out int result);
                if (succ)
                    return dbHelper.GetUserById(result);
            }

            return null;
        }

        [HttpPost("add")]
        public ActionResult<string> AddUser([FromBody] User user)
        {
            var exist = dbHelper.IsUsernameExist(user.Username);
            if (!exist)
            {
                return dbHelper.AddUser(user.Username, user.PasswordHash, user.Email);
            }
            else
                return $"Username '{user.Username}' already exist. Choose another one";
        }

        [HttpGet("list-all")]
        public ActionResult<List<User>> ListAllUser()
        {
            return dbHelper.ListAllUser();
        }

        [HttpPost("login")]
        public ActionResult<UserAuthResponse> Login([FromBody] UserAuth userAuth)
        {
            var user = dbHelper.GetUserByUsername(userAuth.Username);
            if (user != null)
            {
                var valid = PasswordHasher.IsEqual(user.PasswordHash, userAuth.Password);
                if (valid)
                    return new UserAuthResponse
                    {
                        IsSuccess = PasswordHasher.IsEqual(user.PasswordHash, userAuth.Password),
                        Message = "User authenticated"
                    };
            }

            return new UserAuthResponse
            {
                IsSuccess = false,
                Message = "User not authenticated"
            };
        }

        [HttpPost("change-password")]
        public ActionResult<UserAuthResponse> ChangePassword([FromBody] UserChangePassword ucp)
        {
            var user = dbHelper.GetUserByUsername(ucp.Username);
            if (user != null)
            {
                var valid = PasswordHasher.IsEqual(user.PasswordHash, ucp.OldPassword);
                if (valid)
                {
                    var hash = PasswordHasher.GenerateHash(ucp.NewPassword);
                    var (success, message) = dbHelper.ChangePassword(user.Id, hash);
                    return new UserAuthResponse
                    {
                        IsSuccess = success,
                        Message = message
                    };
                }
                else
                {
                    return new UserAuthResponse
                    {
                        IsSuccess = false,
                        Message = "Old password doesn't match"
                    };
                }
            }

            return new UserAuthResponse
            {
                IsSuccess = false,
                Message = "User not authenticated"
            };
        }

        [HttpPost("update")]
        public ActionResult<UserAuthResponse> UpdateUserInfo([FromBody] User u)
        {
            var user = dbHelper.GetUserByUsername(u.Username);
            if (user != null)
            {
               var (success, message) = dbHelper.UpdateUser(user.Id, u.Email, u.Avatar);
                return new UserAuthResponse
                {
                    IsSuccess = success,
                    Message = message
                };                
            }

            return new UserAuthResponse
            {
                IsSuccess = false,
                Message = "User not authenticated"
            };
        }

        [HttpGet("generate-hash")]
        public ActionResult<string> GenerateHash()
        {
            var str = Request.Query["password"];
            return PasswordHasher.GenerateHash(str);
        }

        [HttpGet("is-equal")]
        public ActionResult<bool> IsEqual()
        {
            var pass = Request.Query["password"];
            var hash = Request.Query["hash"];

            return PasswordHasher.IsEqual(hash, pass);
        }
    }
}
