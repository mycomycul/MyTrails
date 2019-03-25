using MyTrails.Libraries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace MyTrails.Models
{
    /// <summary>One of the features extracted from geoJSON that make up a full trail</summary>
    public class TrailSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string ShortDescription { get; set; }
        public DbGeography Geography { get; set; }
        public string Status { get; set; }
        public string TrailID { get; set; }
        public virtual Trail Trail { get; set; }

        public TrailSection()
        {
        }

        public TrailSection(dynamic trail, string wkid)
        {
            Id = trail.attributes.FEATUREID;
            ShortDescription = trail.attributes.TRLNAME;
            Geography = DbGeography.FromText(GeoJSONTools.CreateWktFromJson(trail), Convert.ToInt32(wkid));
            Status = trail.attributes.TRLSTATUS;
        }

    }
}