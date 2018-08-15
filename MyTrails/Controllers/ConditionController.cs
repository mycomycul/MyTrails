using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyTrails.Models;

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

        // GET: Condition/Details/5
        public ActionResult Details(string id)
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

        // GET: Condition/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Condition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PercentSnowCover,Description,Date")] Condition condition)
        {
            if (ModelState.IsValid)
            {
                db.Conditions.Add(condition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

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
