using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EatAppApi.Models;
using EatAppApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EatAppApi.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITimezoneHelper timezoneHelper;
        private readonly IHostingEnvironment env;

        public TestController(ITimezoneHelper timezoneHelper, IHostingEnvironment env)
        {
            this.timezoneHelper = timezoneHelper;
            this.env = env;
        }

        public ActionResult<Test> Test()
        {
            return new Test
            {
                AppName = env.ApplicationName,
                EnvironmentName = env.EnvironmentName,
                Message = "OK to proceed",
                DateTime = DateTime.Now,
                ServerTimezoneId = timezoneHelper.ServerTimezoneId,
                ServerTimezoneOffset = timezoneHelper.GetServerTimezoneOffset(),
                CurrentUtcTime = DateTimeOffset.Now.ToLocalTime().ToString(),
                ContentRootPath = env.ContentRootPath
            };
        }
    }
}