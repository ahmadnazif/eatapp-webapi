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
        private readonly IMysqlDbHelper dbHelper;
        public FnbCommentController(IMysqlDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        [HttpPost("add")]
        public async Task<ActionResult<string>> AddComment([FromBody] FnbComment c)
        {
            var fnb = await dbHelper.GetFnbByIdAsync(c.FnbId);
            if (fnb == null)
                return "FnB Not found";

            var r = await dbHelper.AddFnbCommentAsync(c.FnbId, c.CommenterId, c.Comment, c.Rating, c.BaseRating);
            return r.Message;
        }

        [HttpGet("list-all")]
        public async Task<ActionResult<List<FnbComment>>> ListAllFnbComment()
        {
            var fnbId = Request.Query["fnbid"];
            if (!StringValues.IsNullOrEmpty(fnbId))
            {
                var succ = int.TryParse(fnbId, out int result);
                if (succ)
                    return await dbHelper.ListAllFnbCommentAsync(result);
            }

            return new List<FnbComment>();
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountAllFnbComment()
        {
            var fnbId = Request.Query["fnbid"];
            if (!StringValues.IsNullOrEmpty(fnbId))
            {
                var succ = int.TryParse(fnbId, out int result);
                if (succ)
                    return await dbHelper.CountAllFnbCommentAsync(result);
            }

            return 0;
        }

        [HttpGet("delete-by-fnbid")]
        public async Task<ActionResult<string>> DeleteAll()
        {
            var fnbId = Request.Query["fnbid"];
            if (!StringValues.IsNullOrEmpty(fnbId))
            {
                var succ = int.TryParse(fnbId, out int result);
                if (succ)
                {
                    var r = await dbHelper.DeleteAllCommentAsync(result);
                    return r.Message;
                }
            }

            return null;
        }
    }
}