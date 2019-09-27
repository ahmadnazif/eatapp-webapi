using EatAppApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EatAppApi.Models
{
    public class FnbComment
    {
        public int Id { get; set; }
        public int FnbId { get; set; }
        public string CommenterName { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public BaseRating BaseRating { get; set; }
        public DateTime TimeAdded { get; set; }
    }
}
