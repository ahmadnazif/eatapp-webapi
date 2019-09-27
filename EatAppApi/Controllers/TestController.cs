using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EatAppApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EatAppApi.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public ActionResult<Test> Test()
        {
            return new Test
            {
                Message = "OK to proceed",
                DateTime = DateTime.Now
            };
        }
    }
}