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
        public async Task<ActionResult<UserProfile>> GetUserById()
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

        [HttpGet("get-by-username")]
        public async Task<ActionResult<UserProfile>> GetUserByUsername()
        {
            var username = Request.Query["username"];
            if (!StringValues.IsNullOrEmpty(username))
            {
                return await dbHelper.GetUserByUsernameAsync(username);
            }

            return null;
        }

        [HttpPost("add")]
        public async Task<ActionResult<string>> AddUser([FromBody] UserInit user)
        {
            var exist = await dbHelper.IsUsernameExistAsync(user.Username);
            if (!exist)
            {
                var passwordSalt = Guid.NewGuid().ToString("N").ToUpper();
                var passwordHash = PasswordHasher.GeneratePasswordHash(user.Password, passwordSalt);
                var r = await dbHelper.AddUserAsync(user.Username, passwordHash, passwordSalt, user.Email, user.Role);
                return r.Message;
            }
            else
                return $"Username '{user.Username}' already exist. Choose another one";
        }

        [HttpGet("list-all")]
        public async Task<ActionResult<List<UserProfile>>> ListAllUser()
        {
            return await dbHelper.ListAllUserAsync();
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserAuthResponse>> Login([FromBody] UserLogin ul)
        {
            var ua = await dbHelper.GetUserAuthAsync(ul.Username);
            if (ua != null)
            {
                var valid = PasswordHasher.IsEqual(ua.PasswordHash, ua.PasswordSalt, ul.Password);
                if (valid)
                {
                    await dbHelper.UpdateLoginTimeAsync(ua.UserId);
                    return new UserAuthResponse
                    {
                        IsSuccess = true,
                        Message = "User authenticated"
                    };
                }
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
            var ua = await dbHelper.GetUserAuthAsync(ucp.Username);
            if (ua != null)
            {
                var oldValid = PasswordHasher.IsEqual(ua.PasswordHash, ua.PasswordSalt, ucp.OldPassword);
                if (oldValid)
                {
                    var newPasswordHash = PasswordHasher.GeneratePasswordHash(ucp.NewPassword, ua.PasswordSalt);
                    var resp = await dbHelper.ChangePasswordAsync(ua.UserId, newPasswordHash);
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
        public async Task<ActionResult<UserAuthResponse>> UpdateUserInfo([FromBody] UserProfile u)
        {
            var user = await dbHelper.GetUserByUsernameAsync(u.Username);
            if (user != null)
            {
                var resp = await dbHelper.UpdateUserAsync(user.Id, u.Email, u.Fullname, u.Avatar);
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
            var pass = Request.Query["password"];
            var salt = Request.Query["salt"];
            return PasswordHasher.GeneratePasswordHash(pass, salt);
        }

        [HttpGet("is-equal")]
        public ActionResult<bool> IsEqual()
        {
            var pass = Request.Query["password"];
            var salt = Request.Query["salt"];
            var hash = Request.Query["hash"];

            return PasswordHasher.IsEqual(hash, salt, pass);
        }
    }
}
