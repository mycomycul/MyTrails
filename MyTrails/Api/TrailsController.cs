using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MyTrails.Models;

namespace MyTrails.Api
{
    public class TrailsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Trails
        public IQueryable<Trail> GetTrails()
        {
            return db.Trails;
        }

        // GET: api/Trails/5
        [ResponseType(typeof(Trail))]
        public IHttpActionResult GetTrail(string id)
        {
            Trail trail = db.Trails.Find(id);
            if (trail == null)
            {
                return NotFound();
            }

            return Ok(trail);
        }

        // PUT: api/Trails/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTrail(string id, Trail trail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != trail.Id)
            {
                return BadRequest();
            }

            db.Entry(trail).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Trails
        [ResponseType(typeof(Trail))]
        public IHttpActionResult PostTrail(Trail trail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Trails.Add(trail);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TrailExists(trail.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = trail.Id }, trail);
        }

        // DELETE: api/Trails/5
        [ResponseType(typeof(Trail))]
        public IHttpActionResult DeleteTrail(string id)
        {
            Trail trail = db.Trails.Find(id);
            if (trail == null)
            {
                return NotFound();
            }

            db.Trails.Remove(trail);
            db.SaveChanges();

            return Ok(trail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TrailExists(string id)
        {
            return db.Trails.Count(e => e.Id == id) > 0;
        }
    }
}