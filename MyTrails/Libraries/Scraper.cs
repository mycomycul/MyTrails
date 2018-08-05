using HtmlAgilityPack;
using MyTrails.Models;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MyTrails.Libraries
{
    public class Scraper
    {




        ///<summary>
        ///Removes comments and newlines from strings and replaces character codes
        /// </summary>
        public static string CleanFromHTML(string stringToClean)
        {
            Regex HTMLCommentRegEx = new Regex("<[^>]*>", RegexOptions.IgnoreCase);
            stringToClean = HTMLCommentRegEx.Replace(HttpUtility.HtmlDecode(stringToClean).Replace("\n", String.Empty), "");
            return stringToClean;
        }
        //Need to account for oddities in elevations or mileagelike multiple mileage sets
        public static float? ExtractMiles(string milesElevation)
        {
            try
            {
                milesElevation = milesElevation.Remove(milesElevation.IndexOf(" miles"));
                return float.Parse(milesElevation);
            }
            catch
            {
                return null;
            }
        }
        //Need to account for oddities in elevations or mileagelike multiple mileage sets
        public static string ExtractElevations(string milesElevation)
        {
            milesElevation = milesElevation.Remove(0, milesElevation.IndexOf("miles") + 5);
            return milesElevation;
        }

        public  void OlympicTrailConditions()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            //Setup Browser and download page 
            var baseUrl = new Uri("https://www.nps.gov/olym/planyourvisit/wilderness-trail-conditions.htm");
            HtmlWeb web = new HtmlWeb();
            var PageResult = web.Load(baseUrl.ToString());


            //Get tables from page
            IEnumerable<HtmlNode> TableNode = PageResult.DocumentNode.CssSelect("tbody");

            //Loop through tables skipping table one
            for (int table = 1; table < TableNode.Count(); table++)
            {
                var tableRows = TableNode.ElementAt(table).CssSelect("tr");
                var TrailZone = tableRows.ElementAt(0).InnerText;
                //Loop Through Rows adding to database skipping rows one and 2
                for (int tablerow = 2; tablerow < tableRows.Count(); tablerow++)
                {

                    var rowCells = tableRows.ElementAt(table).CssSelect("td");
                    string trailName = CleanFromHTML(rowCells.ElementAt(0).InnerText);

                    Trail trail = new Trail();
                    if (!db.Trails.Where(x => x.TrailName == trailName).Any())
                    {
                        trail.TrailName = CleanFromHTML(rowCells.ElementAt(0).InnerText);
                        trail.Miles = ExtractMiles(CleanFromHTML(rowCells.ElementAt(2).InnerText));
                        trail.ShortDescription = CleanFromHTML(rowCells.ElementAt(1).InnerText);
                        trail.Elevation = ExtractElevations(CleanFromHTML(rowCells.ElementAt(2).InnerText));
                        trail.InfoHTMLLink = rowCells.ElementAt(0).CssSelect("a").Any() ? baseUrl.Host + CleanFromHTML(rowCells.ElementAt(0).CssSelect("a").First().GetAttributeValue("href")) : null;

                        db.Trails.Add(trail);
                    }
                    else
                    {
                        trail = db.Trails.Where(x => x.TrailName == trailName).FirstOrDefault();
                    };
                    DateTime? conditionDate = DateTime.Parse(CleanFromHTML(rowCells.ElementAt(4).InnerText));

                    if (!trail.Conditions.Where(x => x.Date == conditionDate).Any()) {
                        Condition condition = new Condition()
                        {
                            Description = CleanFromHTML(rowCells.ElementAt(3).InnerText),
                            Date = DateTime.Parse(CleanFromHTML(rowCells.ElementAt(4).InnerText))
                        };
                        trail.Conditions.Add(condition);
                    }
                }
                db.SaveChanges();
            }
        }

        //static List<TrailConditions> RainierTrailConditions()
        //{
        //    List<TrailConditions> ConditionsList = new List<TrailConditions>();
        //    //Setup Browser and download page 
        //    var baseUrl = new Uri("https://www.nps.gov/mora/planyourvisit/trails-and-backcountry-camp-conditions.htm");
        //    HtmlWeb web = new HtmlWeb();
        //    var PageResult = web.Load(baseUrl.ToString());
        //    //Get tables from page
        //    IEnumerable<HtmlNode> TableNode = PageResult.DocumentNode.CssSelect("tbody");

        //    //Loop through tables skipping table one
        //    for (int i = 0; i < TableNode.Count(); i++)
        //    {
        //        var tableRows = TableNode.ElementAt(i).CssSelect("tr");
        //        //Loop Through Rows adding to database skipping rows one and 2
        //        for (int j = 2; j < tableRows.Count(); j++)
        //        {
        //            var rowCells = tableRows.ElementAt(j).CssSelect("td");
        //            var rowZone = tableRows.ElementAt(0).InnerText;

        //            TrailConditions tc = new TrailConditions()
        //            {
        //                TrailName = CleanFromHTML(rowCells.ElementAt(0).InnerText),
        //                PercentSnowCover = int.TryParse(CleanFromHTML(rowCells.ElementAt(1).InnerText), out int snow) ? snow : 0,
        //                Conditions = CleanFromHTML(rowCells.ElementAt(2).InnerText),
        //                Updated = DateTime.TryParse(CleanFromHTML(rowCells.ElementAt(3).InnerText), out DateTime update) ? update : (DateTime?)null
        //                //Zone = CleanFromHTML(rowZone),
        //            };
        //            ConditionsList.Add(tc);
        //        }
        //    }
        //    return ConditionsList;

        //}



        public class TrailConditions
        {
            public string TrailName { get; set; }


        }
    }
}