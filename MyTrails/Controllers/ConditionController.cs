using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using Microsoft.AspNet.Identity;
using MyTrails.Libraries;
using MyTrails.Models;
using ScrapySharp.Extensions;

namespace MyTrails.Controllers
{
    public class ConditionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Condition
        public ActionResult Index()
        {
            var model = db.Conditions.ToList();
            return View(model);
        }

        /// <summary>   (An Action that handles HTTP POST requests) updates this object. </summary>
        ///
        /// <remarks>   Michael, 2/15/2019. </remarks>
        ///
        /// <returns>   A response stream to send to the Success View. </returns>
        /// TODO: Separate Update method from Olympic Page Schema
        [HttpPost]
        public ActionResult UpdateNPSConditions()
        {
            List<Condition> model = new List<Condition>();
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

                //Loop Through Rows adding to Trails skipping header rows 1 and 2
                for (int tablerow = 2; tablerow < tableRows.Count(); tablerow++)
                {
                    var rowCells = tableRows.ElementAt(tablerow).CssSelect("td");
                    ///Grab the trailname for the online condition
                    string trailName = Scraper.CleanFromHTML(rowCells.ElementAt(0).InnerText);
                    //Find the updated trail in the database so it can be updated with new conditions
                    Trail trail = db.Trails.Where(x => x.TrailName == trailName).First();



                    var trailcondition = trail.Conditions.OrderBy(x => x.Date).Last();

                    var scrapedConditionDescription = Scraper.CleanFromHTML(rowCells.ElementAt(3).InnerText);                    
                    var scrapedConditionDate = DateTime.Parse(Scraper.CleanFromHTML(rowCells.ElementAt(4).InnerText));

                    if (scrapedConditionDate != trailcondition.Date)
                    {

                        Condition condition = new Condition()
                        {
                            Description = scrapedConditionDescription,
                            Date = scrapedConditionDate
                        };
                        model.Add(condition);

                        trail.Conditions.Add(condition);
                    }
                    db.SaveChanges();
                }

            }
            ViewBag.Operation = "Added";

            return View("Success", model.OrderBy(x => x.Date));
        }

        ///TODO: Account for source. Only remove duplicates if from scraper
        /// <summary>   Removes conditions with the same dates </summary>
        ///
        /// <remarks>   Michael, 2/16/2019. </remarks>
        ///
        /// <returns>   A response stream to send to the Success View</returns>

        public ActionResult RemoveDuplicates()
        {
            var successModel = new List<Condition>();

            foreach (var trail in db.Trails.ToList())
            {
                HashSet<DateTime?> conditionDates = new HashSet<DateTime?>();
                foreach (var condition in trail.Conditions.ToList())
                {
                    if (conditionDates.Contains(condition.Date))
                    {                     
                        successModel.Add(condition);
                    }
                    else
                    {
                        conditionDates.Add(condition.Date);
                    }
                }
            }
            foreach (var item in successModel)
            {
                db.Conditions.Remove(item);
            }
            db.SaveChanges();
            ViewBag.Operation = "Deleted";
            return View("success", successModel);
        }

        [Authorize]
        // GET: Condition/Create
        public ActionResult Create()
        {

            ViewBag.Trails = db.Trails.OrderBy(t => t.TrailName).Select(x => new SelectListItem { Text = x.TrailName, Value = x.Id });
            return View();
        }

        // POST: Condition/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PercentSnowCover,Description,Date,TrailId")] Condition condition)
        {
            condition.UserId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                db.Conditions.Add(condition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Trails = db.Trails.OrderBy(t => t.TrailName).Select(x => new SelectListItem { Text = x.TrailName, Value = x.Id });

            return View(condition);
        }

        // GET: Condition/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condition condition = db.Conditions.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            return View(condition);
        }

        // POST: Condition/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PercentSnowCover,Description,Date")] Condition condition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(condition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(condition);
        }

        // GET: Condition/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condition condition = db.Conditions.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            return View(condition);
        }

        // POST: Condition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Condition condition = db.Conditions.Find(id);
            db.Conditions.Remove(condition);
            db.SaveChanges();
            return RedirectToAction("Index");
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

