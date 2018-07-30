using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MyTrails.Models
{
    public class Trail
    {
        public int Id { get; set; }
        public string TrailName { get; set; }
        /// <summary>
        /// Points along the trail
        /// </summary>
        public virtual ICollection<Point> Points { get; set; }
    }
}