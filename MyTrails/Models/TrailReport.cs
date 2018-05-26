using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrails.Models
{
    public class TrailReport
    {
        public int Id { get; set; }             
        public string LongReport { get; set; }
        public ApplicationUser Author { get; set; }
        public DateTime AuthoredDate { get; set; }
        public DateTime TripDate { get; set; }
        public DateTime MyProperty { get; set; }
        public List<Trail> TrailsTravelled { get; set; }
    }
}