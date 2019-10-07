using EatAppApi.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EatAppApi.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Avatar { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? LastLoginTime { get; set; }
    }
}
