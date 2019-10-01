using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EatAppApi.Models
{
    public class DbCommitResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
