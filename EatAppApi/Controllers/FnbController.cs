using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EatAppApi.Common;
using EatAppApi.Models;
using EatAppApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EatAppApi.Controllers
{
    [Route("api/fnb")]
    [ApiController]
    public class FnbController : ControllerBase
    {
        private readonly IMysqlDbHelper dbHelper;
        public FnbController(IMysqlDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        [HttpGet("populate")]
        public async Task<ActionResult<string>> Populate()
        {
            if ((await dbHelper.ListAllFnbAsync()).Count == 0)
            {
                List<Fnb> fnbList = new List<Fnb>();

                Add("Nasi Lemak", FnbType.Food);
                Add("Nasi Goreng", FnbType.Food);
                Add("Mee Goreng", FnbType.Food);
                Add("Bihun Goreng", FnbType.Food);
                Add("Spagetti", FnbType.Food);
                Add("Milo Ais", FnbType.Baverage);
                Add("Teh Ais Limau", FnbType.Baverage);
                Add("Sirap Bandung", FnbType.Baverage);
                Add("Kopi O", FnbType.Baverage);

                var r = await dbHelper.AddBatchFnbAsync(fnbList);
                return r.Message;

                void Add(string name, FnbType type)
                {
                    fnbList.Add(new Fnb
                    {
                        Name = name,
                        FnbType = type
                    });
                }
            }
            else
            {
                return "Population not needed";
            }
        }

        [HttpGet("get-by-id")]
        public async Task<ActionResult<Fnb>> GetFnbById()
        {
            var id = Request.Query["id"];
            if (!StringValues.IsNullOrEmpty(id))
            {
                var succ = int.TryParse(id, out int result);
                return await dbHelper.GetFnbByIdAsync(result);
            }

            return null;
        }

        [HttpGet("get-by-name")]
        public async Task<ActionResult<Fnb>> GetFnbByName()
        {
            var name = Request.Query["name"];
            if (!StringValues.IsNullOrEmpty(name))
            {
                return await dbHelper.GetFnbByNameAsync(name);
            }

            return null;
        }

        [HttpGet("list-all")]
        public async Task<ActionResult<List<Fnb>>> ListAllFnb()
        {
            return await dbHelper.ListAllFnbAsync();
        }

        [HttpPost("add")]
        public async Task<ActionResult<string>> Add([FromBody] Fnb fnb)
        {
            var exist = await dbHelper.IsFnbExistAsync(fnb.Name);
            if (exist)
                return $"{fnb.Name} already exist";
            else
            {
                var r = await dbHelper.AddFnbAsync(fnb);
                return r.Message;
            }
        }

    }
}