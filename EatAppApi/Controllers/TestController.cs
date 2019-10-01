using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EatAppApi.Models;
using EatAppApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EatAppApi.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITimezoneHelper timezoneHelper;

        public TestController(ITimezoneHelper timezoneHelper) => this.timezoneHelper = timezoneHelper;

        public ActionResult<Test> Test()
        {
            return new Test
            {
                Message = "OK to proceed",
                DateTime = DateTime.Now,
                ServerTimezoneId = timezoneHelper.ServerTimezoneId,
                ServerTimezoneOffset = timezoneHelper.GetServerTimezoneOffset(),
                CurrentUtcTime = DateTimeOffset.Now.ToLocalTime().ToString()
            };
        }
    }
}