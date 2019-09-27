using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EatAppApi.Models
{
    public class FnbPicture
    {
        public int Id { get; set; }
        public int FnbId { get; set; }
        public string AddedBy { get; set; }
        public DateTime TimeAdded { get; set; }
        public string PicturePath { get; set; }
    }
}
