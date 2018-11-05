using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEE.Infrastructure.DbContext;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExamFormController : Controller
    {
        private SATEntities db = new SATEntities();

        // GET: ttn_content/ExamForm
        public ActionResult Index()
        {
            var sATExamForms = db.SATExamForms.Include(s => s.SATType).Where(e => e.Status == true);
            return View(sATExamForms.ToList());
        }

        // GET: ttn_content/ExamForm/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SATExamForm sATExamForm = db.SATExamForms.Find(id);
            if (sATExamForm == null)
            {
                return HttpNotFound();
            }
            return View(sATExamForm);
        }

        // GET: ttn_content/ExamForm/Create
        public ActionResult Create()
        {
            ViewBag.TypeID = new SelectList(db.SATTypes, "TypeID", "TypeName");
            return View();
        }

        // POST: ttn_content/ExamForm/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Direction,Title,Duration,NumberQuestion,TypeID,ExamCode")] SATExamForm sATExamForm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    sATExamForm.Section = sATExamForm.TypeID;
                    sATExamForm.Status = true;
                    db.SATExamForms.Add(sATExamForm);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.TypeID = new SelectList(db.SATTypes, "TypeID", "TypeName", sATExamForm.TypeID);
                return View(sATExamForm);
            }
            catch (Exception)
            {
                ViewBag.TypeID = new SelectList(db.SATTypes, "TypeID", "TypeName", sATExamForm.TypeID);
                return View(sATExamForm);
            }
        }

        // GET: ttn_content/ExamForm/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SATExamForm sATExamForm = db.SATExamForms.Find(id);
            if (sATExamForm == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypeID = new SelectList(db.SATTypes, "TypeID", "TypeName", sATExamForm.TypeID);
            return View(sATExamForm);
        }

        // POST: ttn_content/ExamForm/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Direction,Title,Duration,NumberQuestion,TypeID,ExamCode")] SATExamForm sATExamForm)
        {
            if (ModelState.IsValid)
            {
                sATExamForm.Section = sATExamForm.TypeID;
                db.Entry(sATExamForm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypeID = new SelectList(db.SATTypes, "TypeID", "TypeName", sATExamForm.TypeID);
            return View(sATExamForm);
        }

        // GET: ttn_content/ExamForm/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SATExamForm sATExamForm = db.SATExamForms.Find(id);
            if (sATExamForm == null)
            {
                return HttpNotFound();
            }
            return View(sATExamForm);
        }

        // POST: ttn_content/ExamForm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SATExamForm sATExamForm = db.SATExamForms.Find(id);
            sATExamForm.Status = false;
            db.Entry(sATExamForm).State = EntityState.Modified;
            //db.SATExamForms.Remove(sATExamForm);
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
