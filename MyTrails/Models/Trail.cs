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
        //NPS Properties scraped from website
        public string TrailName { get; set; }
        public string Zone { get; set; }
        public string Description { get; set; }
        public string Elevation { get; set; }
        public float? TotalMiles { get; set; }
        /// <summary>
        /// Link to information page on NPS.gov
        /// </summary>
        public string InfoHTMLLink { get; set; }


        public string Status { get; set; }
        public string Agency { get; set; }
        public string ShortDescription { get; set; }


        public virtual ICollection<Post> Posts { get; set; }
        /// <summary>
        /// Sections of spatial data that have the same trailname and make up a single trail defined by the Federal Register
        /// </summary>
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

}