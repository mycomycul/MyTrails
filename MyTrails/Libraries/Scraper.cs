﻿using HtmlAgilityPack;
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



        /// <summary>
        ///Removes HTML or XML ELements and newlines from inner content
        /// </summary>
        /// <param name="stringToClean">String of XML or HTML content</param>
        /// <returns>Inner text content cleaned of XML or HTML markup</returns>
        public static string CleanFromHTML(string stringToClean)
        {
            /*Remove Element Tags*/
            Regex HTMLCommentRegEx = new Regex("<[^>]*>", RegexOptions.IgnoreCase);
            /*Remove any newlines*/
            stringToClean = HTMLCommentRegEx.Replace(HttpUtility.HtmlDecode(stringToClean).Replace("\n", String.Empty), "");
            /*Remove any null character. Was having issues with \0 not equalling \0 in two separate strings*/
            stringToClean = stringToClean.Replace("\0", string.Empty);
            return stringToClean;
        }

        //TODO:Need to account for oddities in elevations or mileagelike multiple mileage sets
        /// <summary>
        /// Extracts "miles" from a text  string and return
        /// </summary>
        /// <param name="milesElevation"></param>
        /// <returns></returns>
        public static float? ExtractMiles(string milesElevation)
        {
            try
            {
                var numbers = new String(milesElevation.Where(c => Char.IsDigit(c)).ToArray());
                return float.Parse(String.Join(null,numbers));
                
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
                        trail.TotalMiles = ExtractMiles(CleanFromHTML(rowCells.ElementAt(2).InnerText));
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