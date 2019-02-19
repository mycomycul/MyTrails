using HtmlAgilityPack;
using MyTrails.Libraries;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;


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
        public virtual List<TrailSection> TrailSections { get; set; }
        public virtual List<Condition> Conditions { get; set; }

        public Trail()
        {
            Id = Guid.NewGuid().ToString();
            Conditions = new List<Condition>();
            this.Posts = new HashSet<Post>();
        }
        public Trail(IEnumerable<HtmlNode> rowCells, string trailZone, Uri baseUrl)
        {
            Id = Guid.NewGuid().ToString();
            Conditions = new List<Condition>();
            this.Posts = new HashSet<Post>();
            this.TrailName = Scraper.CleanFromHTML(rowCells.ElementAt(0).InnerText);
            this.TotalMiles = Scraper.ExtractMiles(Scraper.CleanFromHTML(rowCells.ElementAt(2).InnerText));
            this.ShortDescription = Scraper.CleanFromHTML(rowCells.ElementAt(1).InnerText);
            this.Elevation = Scraper.ExtractElevations(Scraper.CleanFromHTML(rowCells.ElementAt(2).InnerText));
            this.InfoHTMLLink = rowCells.ElementAt(0).CssSelect("a").Any() ? baseUrl.Host + Scraper.CleanFromHTML(rowCells.ElementAt(0).CssSelect("a").First().GetAttributeValue("href")) : null;
            this.Agency = "Olympic National Park";
            this.Zone = trailZone;
        }
    }

    /// <summary>One of the features extracted from geoJSON that makes up a full trail</summary>
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