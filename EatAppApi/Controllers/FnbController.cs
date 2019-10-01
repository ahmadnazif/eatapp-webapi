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
        private readonly ILiteDbHelper dbHelper;
        public FnbController(ILiteDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        [HttpGet("populate")]
        public ActionResult<string> Populate()
        {
            if (dbHelper.ListAllFnb().Count == 0)
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

                return dbHelper.AddBatchFnb(fnbList);

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
        public ActionResult<Fnb> GetFnbById()
        {
            var id = Request.Query["id"];
            if (!StringValues.IsNullOrEmpty(id))
            {
                var succ = int.TryParse(id, out int result);
                return dbHelper.GetFnbById(result);
            }

            return null;
        }

        [HttpGet("get-by-name")]
        public ActionResult<Fnb> GetFnbByName()
        {
            var name = Request.Query["name"];
            if (!StringValues.IsNullOrEmpty(name))
            {
                return dbHelper.GetFnbByName(name);
            }

            return null;
        }

        [HttpGet("list-all")]
        public ActionResult<List<Fnb>> ListAllFnb()
        {
            return dbHelper.ListAllFnb();
        }

        [HttpPost("add")]
        public ActionResult<string> Add([FromBody] Fnb fnb)
        {
            var exist = dbHelper.IsFnbExist(fnb.Name);
            if (exist)
                return $"{fnb.Name} already exist";
            else
                return dbHelper.AddFnb(fnb);
        }

    }
}