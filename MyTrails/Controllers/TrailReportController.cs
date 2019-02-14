using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System.Text.RegularExpressions;
using MyTrails.ViewModels;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyTrails.Controllers
{
    /// <summary>
    /// This is a first pass at extracting data off the Olympic National Park Website and returning it to a view
    /// </summary>
    /// TODO: Check for any valuable code and delete controller
    public class TrailReportController : Controller
    {


        /// <summary>
        /// Imports the Olympic National Park Trail Condtions website, parses conditions into a View Model and returns them on a page
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Import()
        {
            ///TODO:Make Async
            List<TrailConditions> ConditionsList = new List<TrailConditions>();
            //Setup Browser and download page 
            var baseUrl = new Uri("https://www.nps.gov/olym/planyourvisit/wilderness-trail-conditions.htm");
            HtmlWeb web = new HtmlWeb();
            var PageResult = web.Load(baseUrl.ToString());


            //Get tables from page
            IEnumerable<HtmlNode> TableNode = PageResult.DocumentNode.CssSelect("tbody");

            //Loop through tables skipping the header row
            for (int i = 1; i < TableNode.Count(); i++)
            {
                var tableRows = TableNode.ElementAt(i).CssSelect("tr");
                //Loop Through Rows adding to viewmodel database skipping rows one and 2
                for (int j = 2; j < tableRows.Count(); j++)
                {
                    var rowCells = tableRows.ElementAt(j).CssSelect("td");
                    var rowZone = tableRows.ElementAt(0).InnerText;
                    TrailConditions tc = new TrailConditions()
                    {
                        TrailName = CleanFromHTML(rowCells.ElementAt(0).InnerText),
                        Description = CleanFromHTML(rowCells.ElementAt(1).InnerText),
                        Elevation = ExtractElevations(CleanFromHTML(rowCells.ElementAt(2).InnerText)),
                        Conditions = CleanFromHTML(rowCells.ElementAt(3).InnerText),
                        Updated = DateTime.Parse(CleanFromHTML(rowCells.ElementAt(4).InnerText)),
                        InfoLink = rowCells.ElementAt(0).CssSelect("a").Any() ? baseUrl.Host + CleanFromHTML(rowCells.ElementAt(0).CssSelect("a").First().GetAttributeValue("href")) : null,
                        Zone = CleanFromHTML(rowZone),
                        Miles = ExtractMiles(CleanFromHTML(rowCells.ElementAt(2).InnerText))

                    };
                    ConditionsList.Add(tc);
                }

            }


            return View(ConditionsList);
        }
        ///<summary>
        ///Removes comments and newlines from strings and replaces character codes
        /// </summary>
        string CleanFromHTML(string stringToClean)
        {
            Regex HTMLCommentRegEx = new Regex("<[^>]*>", RegexOptions.IgnoreCase);
            stringToClean = HTMLCommentRegEx.Replace(HttpUtility.HtmlDecode(stringToClean).Replace("\n", String.Empty), "");
            return stringToClean;
        }
        //TODO Need to account for oddities in elevations or mileagelike multiple mileage sets
        float? ExtractMiles(string milesElevation)
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
        //TODO Need to account for oddities in elevations or mileagelike multiple mileage sets
        string ExtractElevations(string milesElevation)
        {
            milesElevation = milesElevation.Remove(0, milesElevation.IndexOf("miles") + 5);
            return milesElevation;
        }


    }



}

