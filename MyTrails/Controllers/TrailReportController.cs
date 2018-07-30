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
    public class TrailReportController : Controller
    {
        // GET: TrailReport
        public ActionResult Index()
        {
            return View();
        }

        // GET: TrailReport/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TrailReport/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrailReport/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TrailReport/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TrailReport/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TrailReport/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrailReport/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult Import()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file)
        {
            XmlTextReader reader = new XmlTextReader("Trails.xml");
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        Console.Write("<" + reader.Name);

                        while (reader.MoveToNextAttribute()) // Read the attributes.
                            Console.Write(" " + reader.Name + "='" + reader.Value + "'");
                        Console.WriteLine(">");
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        Console.WriteLine(reader.Value);
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        Console.Write("</" + reader.Name);
                        Console.WriteLine(">");
                        break;
                }
            }
            return View();
        }
        public ActionResult Conditions()
        {
            List<TrailConditions> ConditionsList = new List<TrailConditions>();
            //Setup Browser and download page 
            var baseUrl = new Uri("https://www.nps.gov/olym/planyourvisit/wilderness-trail-conditions.htm");
            HtmlWeb web = new HtmlWeb();
            var PageResult = web.Load(baseUrl.ToString());
            //var PageResult = web.NavigateToPageAsync(new Uri("https://www.nps.gov/olym/planyourvisit/wilderness-trail-conditions.htm"));


            //Get tables from page
            IEnumerable<HtmlNode> TableNode = PageResult.DocumentNode.CssSelect("tbody");

            //Loop through tables skipping table one
            for (int i = 1; i < TableNode.Count(); i++)
            {
                var tableRows = TableNode.ElementAt(i).CssSelect("tr");
                //Loop Through Rows adding to database skipping rows one and 2
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
        //Need to account for oddities in elevations or mileagelike multiple mileage sets
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
        //Need to account for oddities in elevations or mileagelike multiple mileage sets
        string ExtractElevations(string milesElevation)
        {
            milesElevation = milesElevation.Remove(0, milesElevation.IndexOf("miles") + 5);
            return milesElevation;
        }


    }



}

