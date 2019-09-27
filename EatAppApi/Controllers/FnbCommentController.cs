using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EatAppApi.Models;
using EatAppApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EatAppApi.Controllers
{
    [Route("api/fnb-comment")]
    [ApiController]
    public class FnbCommentController : ControllerBase
    {
        private readonly IDbHelper dbHelper;
        public FnbCommentController(IDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        [HttpPost("add")]
        public ActionResult<string> AddComment([FromBody] FnbComment c)
        {
            var fnb = dbHelper.GetFnbById(c.FnbId);
            if (fnb == null)
                return "FnB Not found";

            return dbHelper.AddFnbComment(c.FnbId, c.CommenterName, c.Comment, c.Rating);
        }

        [HttpGet("get-by-fnbid")]
        public ActionResult<List<FnbComment>> ListAllFnbComment()
        {
            var fnbId = Request.Query["fnbid"];
            if (!StringValues.IsNullOrEmpty(fnbId))
            {
                var succ = int.TryParse(fnbId, out int result);
                if (succ)
                    return dbHelper.ListAllFnbComment(result);
            }

            return new List<FnbComment>();
        }

        [HttpGet("delete-by-fnbid")]
        public ActionResult<string> DeleteAll()
        {
            var fnbId = Request.Query["fnbid"];
            if (!StringValues.IsNullOrEmpty(fnbId))
            {
                var succ = int.TryParse(fnbId, out int result);
                if (succ)
                    return dbHelper.DeleteAllComment(result);
            }

            return null;
        }
    }
}