using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrails.Models
{
    public class Condition
    {
        public string Id { get; set; }        
        public int PercentSnowCover { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public string TrailId { get; set; }

        public virtual Trail Trail { get; set; }

        public Condition()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}