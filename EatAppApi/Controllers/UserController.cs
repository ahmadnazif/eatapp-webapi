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
        private readonly IMysqlDbHelper dbHelper;
        public UserController(IMysqlDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        [HttpGet("get-by-id")]
        public async Task<ActionResult<User>> GetUserById()
        {
            var id = Request.Query["id"];
            if (!StringValues.IsNullOrEmpty(id))
            {
                var succ = int.TryParse(id, out int result);
                if (succ)
                    return await dbHelper.GetUserByIdAsync(result);
            }

            return null;
        }

        [HttpPost("add")]
        public async Task<ActionResult<string>> AddUser([FromBody] User user)
        {
            var exist = await dbHelper.IsUsernameExistAsync(user.Username);
            if (!exist)
            {
                var r = await dbHelper.AddUserAsync(user.Username, user.PasswordHash, user.Email);
                return r.Message;
            }
            else
                return $"Username '{user.Username}' already exist. Choose another one";
        }

        [HttpGet("list-all")]
        public async Task<ActionResult<List<User>>> ListAllUser()
        {
            return await dbHelper.ListAllUserAsync();
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserAuthResponse>> Login([FromBody] UserAuth userAuth)
        {
            var user = await dbHelper.GetUserByUsernameAsync(userAuth.Username);
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
        public async Task<ActionResult<UserAuthResponse>> ChangePassword([FromBody] UserChangePassword ucp)
        {
            var user = await dbHelper.GetUserByUsernameAsync(ucp.Username);
            if (user != null)
            {
                var valid = PasswordHasher.IsEqual(user.PasswordHash, ucp.OldPassword);
                if (valid)
                {
                    var hash = PasswordHasher.GenerateHash(ucp.NewPassword);
                    var resp = await dbHelper.ChangePasswordAsync(user.Id, hash);
                    return new UserAuthResponse
                    {
                        IsSuccess = resp.IsSuccess,
                        Message = resp.Message
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
        public async Task<ActionResult<UserAuthResponse>> UpdateUserInfo([FromBody] User u)
        {
            var user = await dbHelper.GetUserByUsernameAsync(u.Username);
            if (user != null)
            {
               var resp = await dbHelper.UpdateUserAsync(user.Id, u.Email, u.Avatar);
                return new UserAuthResponse
                {
                    IsSuccess = resp.IsSuccess,
                    Message = resp.Message
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
