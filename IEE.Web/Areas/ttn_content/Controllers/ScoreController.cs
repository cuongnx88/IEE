using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEE.Infrastructure.DbContext;
using IEE.Web.Areas.ttn_content.Models;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    [Authorize(Roles="Admin")]
    public class ScoreController : Controller
    {
        private SATEntities db = new SATEntities();

        // GET: ttn_content/Score
        public ActionResult Index()
        {
            
            var sATScores = db.SATScores.Include(s => s.SATType).ToList();
            var viewModel = new ScoreViewModel(sATScores);
            return View(viewModel);
        }

        // GET: ttn_content/Score/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SATScore sATScore = db.SATScores.Find(id);
            if (sATScore == null)
            {
                return HttpNotFound();
            }
            return View(sATScore);
        }

        // GET: ttn_content/Score/Create
        public ActionResult Create()
        {
            ViewBag.TypeID = new SelectList(db.SATTypes, "TypeID", "TypeName");
            return View();
        }

        // POST: ttn_content/Score/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,RawValue,ScoreValue,TypeID")] SATScore sATScore)
        {
            if (ModelState.IsValid)
            {
                db.SATScores.Add(sATScore);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TypeID = new SelectList(db.SATTypes, "TypeID", "TypeName", sATScore.TypeID);
            return View(sATScore);
        }

        // GET: ttn_content/Score/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SATScore sATScore = db.SATScores.Find(id);
            if (sATScore == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypeID = new SelectList(db.SATTypes, "TypeID", "TypeName", sATScore.TypeID);
            return View(sATScore);
        }

        // POST: ttn_content/Score/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,RawValue,ScoreValue,TypeID")] SATScore sATScore)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sATScore).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypeID = new SelectList(db.SATTypes, "TypeID", "TypeName", sATScore.TypeID);
            return View(sATScore);
        }

        // GET: ttn_content/Score/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SATScore sATScore = db.SATScores.Find(id);
            if (sATScore == null)
            {
                return HttpNotFound();
            }
            return View(sATScore);
        }

        // POST: ttn_content/Score/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SATScore sATScore = db.SATScores.Find(id);
            db.SATScores.Remove(sATScore);
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
