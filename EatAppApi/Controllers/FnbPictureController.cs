using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EatAppApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EatAppApi.Controllers
{
    [Route("api/fnb-picture")]
    [ApiController]
    public class FnbPictureController : ControllerBase
    {
        private readonly IDbHelper dbHelper;
        public FnbPictureController(IDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }
    }
}