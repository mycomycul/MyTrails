using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
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
        public float? TotalMiles { get; set; }
        public string Status { get; set; }
        public string Agency { get; set; }
        /// <summary>
        /// Link to information page on NPS.gov
        /// </summary>
        public string InfoHTMLLink { get; set; }

        //NPS Properties
        public string Elevation { get; set; }
        public string ShortDescription { get; set; }


        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<TrailSection> TrailSections { get; set; }
        public virtual ICollection<Condition> Conditions { get; set; }

        public Trail()
        {
            Id = Guid.NewGuid().ToString();
            Conditions = new List<Condition>();
            this.Posts = new HashSet<Post>();
        }
    }

    public class TrailSection
    {
        public string Id { get; set; }
        public string ShortDescription { get; set; }
        public DbGeography Geography { get; set; }
        public string Status { get; set; }


        public string  TrailID { get; set; }
        public virtual Trail Trail { get; set; }


    }
}