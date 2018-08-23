using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyTrails.Models;
using MyTrails.Libraries;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.IO;
using System.Web.Script.Serialization;

namespace MyTrails.Controllers
{
    public class TrailController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Trail
        public ActionResult Index()
        {
            return View(db.Trails.ToList());
        }

        // GET: Trail/Details/5
        public ActionResult Details(int? id)
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

        // GET: Trail/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Trail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        public ActionResult ImportOlympicTrails()
        {



            //Setup Browser and download page 
            var baseUrl = new Uri("https://www.nps.gov/olym/planyourvisit/wilderness-trail-conditions.htm");
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

                    Trail trail = new Trail();
                    if (!db.Trails.Where(x => x.TrailName == trailName).Any())
                    {
                        trail.TrailName = Scraper.CleanFromHTML(rowCells.ElementAt(0).InnerText);
                        trail.TotalMiles = Scraper.ExtractMiles(Scraper.CleanFromHTML(rowCells.ElementAt(2).InnerText));
                        trail.ShortDescription = Scraper.CleanFromHTML(rowCells.ElementAt(1).InnerText);
                        trail.Elevation = Scraper.ExtractElevations(Scraper.CleanFromHTML(rowCells.ElementAt(2).InnerText));
                        trail.InfoHTMLLink = rowCells.ElementAt(0).CssSelect("a").Any() ? baseUrl.Host + Scraper.CleanFromHTML(rowCells.ElementAt(0).CssSelect("a").First().GetAttributeValue("href")) : null;
                        trail.Agency = "Olympic National Park";
                        trail.Zone = trailZone;
                        db.Trails.Add(trail);
                        db.SaveChanges();
                    }
                    else
                    {
                        trail = db.Trails.Where(x => x.TrailName == trailName).First();
                    };

                    DateTime? conditionDate = DateTime.Parse(Scraper.CleanFromHTML(rowCells.ElementAt(4).InnerText));

                    if (trail.Conditions.Count() == 0)
                    {

                        Condition condition = new Condition()
                        {
                            Description = Scraper.CleanFromHTML(rowCells.ElementAt(3).InnerText),
                            Date = DateTime.Parse(Scraper.CleanFromHTML(rowCells.ElementAt(4).InnerText))
                        };

                        trail.Conditions.Add(condition);
                    }
                    else if (trail.Conditions.Last().Description != Scraper.CleanFromHTML(rowCells.ElementAt(3).InnerText))
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

        public ActionResult ImportJSONtrail()
        {
            GeoJSONTools geoTools = new GeoJSONTools();
            geoTools.InputGeoJson2();

            return new EmptyResult();
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
