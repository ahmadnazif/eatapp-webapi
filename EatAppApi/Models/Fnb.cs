using EatAppApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EatAppApi.Models
{
    public class Fnb
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FnbType FnbType { get; set; }
    }
}
