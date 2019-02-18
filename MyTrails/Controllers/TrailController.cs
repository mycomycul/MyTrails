using HtmlAgilityPack;
using MyTrails.Libraries;
using MyTrails.Models;
using MyTrails.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static MyTrails.Libraries.GeoJSONTools;

namespace MyTrails.Controllers
{
    /// <summary>   A controller for handling trails. </summary>
    /// TODO: Isolate this controller
    /// <remarks>   Michael, 2/10/2019. </remarks>

    public class TrailController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();        
        private readonly JObject trailData = new JObject(JObject.Parse(System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/OlympicTrailData.Json"))) as JObject);

        // GET: Trail
        public ActionResult Index()
        {
            var vm = db.Trails.ToList();
            return View(vm);
        
        }


        /// <summary>
        /// IN the event that automatic pairing between trail names on the website and JSON Data fails
        /// THe Method selects Trails from the database without geospatial data and geospatial data without 
        /// matching trailnames in the db so they can be paired up on a manual pairing page
        /// </summary>
        /// <returns>View and CombineViewModel</returns>
        [HttpGet]
        public ActionResult ManuallyAddJsonToDb()
        {
            dynamic features = trailData["features"];

            //Get the names of all trails in the db without a trailsection.  If no trailsection, Automatically adding Geometry to the Db didn't work
            var Trails = db.Trails.Where(x => x.TrailSections.Count < 1).OrderBy(q => q.TrailName).Select(n => n.TrailName).ToList();

            //Gets the names of features in the JSON without db entries by checking for each name in the db
            List<string> trailNames = new List<string>();
            for (int i = 0; i < features.Count; i++)
            {
                string currentFeature = features[i].attributes.TRLNAME.Value;
                if (!db.Trails.Any(x => x.TrailName == currentFeature))
                {
                    trailNames.Add(features[i].attributes.TRLNAME.Value);
                }
            }


            return View(new CombineViewModel(Trails, trailNames.Distinct().OrderBy(x => x).ToList()));
        }

        /// TODO: COmplete method for combining json trail section s and saving them under a trail
        /// <summary>
        /// Incomplete method for combining the selected trail form the db and trail sections from JSON
        /// </summary>
        /// <param name="trailNameInDb"></param>
        /// <param name="trailFeatureNames"></param>
        /// <returns></returns>
        //[HttpPost]
        //public ActionResult ManuallyAddJsonToDb(string trailNameInDb, string[] trailFeatureNames)
        //{
        //    dynamic features = trailData["features"];
        //    var Trail = db.Trails.Where(x => x.TrailName.ToUpper() == trailNameInDb);

        //    foreach (var trailFeatureName in trailFeatureNames)
        //    {
        //        var newTrailFeature = features;
        //    }
        //    var ts = new TrailSection(trail, wkid);
        //    var currentTrail = db.Trails.Where(x => x.TrailName.ToUpper() == trailName).First();
        //    if (currentTrail.TrailSections.Count < 1)
        //    {
        //        currentTrail.Status = ts.Status;
        //    }
        //    currentTrail.TrailSections.Add(ts);
        //    return new EmptyResult();
        //}





        // POST: Trail/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TrailName,Zone,Description,Elevation,Miles,InfoHTMLLink")] Trail trail)
        {
            if (ModelState.IsValid)
            {
                db.Trails.Add(trail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(trail);
        }

        // GET: Trail/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trail trail = db.Trails.Find(id);
            if (trail == null)
            {
                return HttpNotFound();
            }
            return View(trail);
        }

        // POST: Trail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TrailName,Zone,Description,Elevation,Miles,InfoHTMLLink")] Trail trail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trail);
        }

        // GET: Trail/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trail trail = db.Trails.Find(id);
            if (trail == null)
            {
                return HttpNotFound();
            }
            return View(trail);
        }

        // POST: Trail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trail trail = db.Trails.Find(id);
            db.Trails.Remove(trail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Trails/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trail trail = await db.Trails.FindAsync(id);
            if (trail == null)
            {
                return HttpNotFound();
            }
            return View(trail);
        }


        public ActionResult Map()
        {
            return View();
        }

        public ActionResult ImportJSONtrail()
        {
            GeoJSONTools geoTools = new GeoJSONTools();
            geoTools.ImportGeoJsonDataToDB();

            return new EmptyResult();
        }

    
        /// <summary>
        /// Imports trails and conditions to the db from the Olympic Naitonal Park trail conditions for first time run
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportOlympicTrails()
        {

            //Setup Browser and download page 
            Uri baseUrl = new Uri("https://www.nps.gov/olym/planyourvisit/wilderness-trail-conditions.htm");
            HtmlWeb web = new HtmlWeb();
            var pageResult = web.Load(baseUrl.ToString());


            //Get tables from page
            IEnumerable<HtmlNode> tableNode = pageResult.DocumentNode.CssSelect("tbody");

            //Loop through tables skipping table one
            for (int table = 1; table < tableNode.Count(); table++)
            {
                var tableRows = tableNode.ElementAt(table).CssSelect("tr");
                var trailZone = tableRows.ElementAt(0).InnerText;

                //Loop Through Rows adding to database skipping rows one and 2
                for (int tablerow = 2; tablerow < tableRows.Count(); tablerow++)
                {

                    var rowCells = tableRows.ElementAt(tablerow).CssSelect("td");
                    string trailName = Scraper.CleanFromHTML(rowCells.ElementAt(0).InnerText);

                    Trail trail;
                    //If the current row on the website doesn't exist in the database, create the trail
                    if (!db.Trails.Where(x => x.TrailName == trailName).Any())
                    {
                        trail = new Trail(rowCells, trailZone, baseUrl);
                        db.Trails.Add(trail);
                        db.SaveChanges();
                    }
                    //else retrieve the trail in the database so it can be updated with new conditions
                    else
                    {
                        trail = db.Trails.Where(x => x.TrailName == trailName).First();
                    };

                    //DateTime? conditionDate = DateTime.Parse(Scraper.CleanFromHTML(rowCells.ElementAt(4).InnerText));
                    //If there are no
                    if (trail.Conditions.Count() == 0 || trail.Conditions.Last().Description != Scraper.CleanFromHTML(rowCells.ElementAt(3).InnerText))
                    {
                        Condition condition = new Condition()
                        {
                            Description = Scraper.CleanFromHTML(rowCells.ElementAt(3).InnerText),
                            Date = DateTime.Parse(Scraper.CleanFromHTML(rowCells.ElementAt(4).InnerText))
                        };

                        trail.Conditions.Add(condition);
                    }
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        public string GetJsonTrailDataFromJson(string trailSectionName)
        {
            var trailSections = GetTrailDataFromJson(trailSectionName);
            return (JsonConvert.SerializeObject(trailSections));
        }

        ///TODO:Convert system to RESTFul Api Output, client and server
        /// <summary>
        /// Extracts geometry and notes for all trail sections in the GeoJson data that 
        /// have the same name and returns them as Json
        /// </summary>
        /// <param name="trailSectionName"></param>
        /// <returns></returns>
        public List<SingleTrailSection> GetTrailDataFromJson(string trailSectionName)
        {
            dynamic features = trailData["features"];
            try
            {
                //Select the geometry and notes from all sections in the GeoJson data with the same name as the received string
                dynamic trailSections = (from s in features as IEnumerable<dynamic>
                                     where s.attributes.TRLNAME == trailSectionName
                                     select new { Geometry = s.geometry.paths[0],
                                                    Note = s.attributes.NOTES}).ToList();
                List<SingleTrailSection> st = new List<SingleTrailSection>();
                foreach (var feature in trailSections)
                {
                    SingleTrailSection t = new SingleTrailSection();
                    t.Geometry = feature.Geometry.ToObject<decimal[,]>();
                        t.Note = feature.Note.ToString();
                    var r = feature.Geometry.ToObject<decimal[,]>();
                    st.Add(t);
                }

                return st;
            }
            catch (Exception e)
            {
                //return e.Message;
                //return new EmptyResult()
                return new List<SingleTrailSection>();
            }
        }


        /// <summary>
        /// Extracts all of the features by a name from a geoJSON set
        /// </summary>
        /// <param name="trailFeatureName">Trail Feature to look retrieve</param>
        /// <param name="returnJson">Set true to return json string instead of dynamic object: Default false</param>
        /// <returns></returns>
        public dynamic GetJsonTrailFromGeoJson(string trailFeatureName, bool returnJson = false)
        {
            dynamic features = trailData["features"];
            //Convert received <trailSectionName> case to match JSON feature's Uppercase name
            trailFeatureName = trailFeatureName.ToUpper();
            try
            {
                //Select the geometry and notes from all sections in the GeoJson data with the same name as the received string
                dynamic trailSections = (from s in features as IEnumerable<dynamic>
                                         where s.attributes.TRLNAME == trailFeatureName
                                         select s);
                if (returnJson)
                {
                    return JsonConvert.SerializeObject(trailSections);
                }
                return trailSections;
            }
            catch (Exception e)
            {
                var r = e.Message;
                return "Failed to Get trail";
            }
        }

        /// <summary>
        /// Finds a Trail in the DB with the same TrailName as the received trailSectionName parameter
        /// and returns its notes and geometry as JSON.
        /// </summary>
        ///
        /// <remarks>   Michael, 2/15/2019. </remarks>
        ///
        /// <param name="trailSectionName"> . </param>
        ///
        /// <returns>   The database trail data. </returns>

        public string GetDbTrailData(string trailSectionName)
        {
            //For testing without a name parameter
            //var dbTrail = db.Trails.Where(m => m.TrailSections.Count > 0).First();

            //Query the db for trails using the received trailSectionName
            var dbTrail = db.Trails.Where(m => m.TrailName == trailSectionName).First();
            List<SingleTrailSection> FullTrail = new List<SingleTrailSection>();
            foreach (var geometrySection in dbTrail.TrailSections)
            {
                //Save Notes and geometry from the current section of the trail to full trail
                //Geography uses extension method provided by GeoJSONTools
                FullTrail.Add(new SingleTrailSection(geometrySection.Geography.AsArray(), geometrySection.ShortDescription));
            }
            return JsonConvert.SerializeObject(FullTrail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
