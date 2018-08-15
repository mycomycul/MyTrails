using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MyTrails.Models
{
    public class Trail
    {
        public string Id { get; set; }
        public string TrailName { get; set; }
        public string Zone { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Elevation { get; set; }
        public float? Miles { get; set; }
        public string Agency { get; set; }

        public virtual ICollection<Condition> Conditions { get; set; }
        /// <summary>
        /// Link to information page on NPS.gov
        /// </summary>
        public string InfoHTMLLink { get; set; }

        public Trail()
        {
            Id = Guid.NewGuid().ToString();
            Conditions = new List<Condition>();
        }







        /// <summary>
        /// Points along the trail
        /// </summary>
        //public virtual ICollection<Point> Points { get; set; }
    }
}