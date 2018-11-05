using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEE.Infrastructure.DbContext;
using IEE.Infrastructure;
using System.IO;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    public class TutorialVideoController : Controller
    {
        private SATEntities db = new SATEntities();

        // GET: ttn_content/TutorialVideo
        public ActionResult Index()
        {
            var tutorialVideos = db.TutorialVideos.Include(t => t.Category);
            return View(tutorialVideos.ToList());
        }

        // GET: ttn_content/TutorialVideo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TutorialVideo tutorialVideo = db.TutorialVideos.Find(id);
            if (tutorialVideo == null)
            {
                return HttpNotFound();
            }
            return View(tutorialVideo);
        }

        // GET: ttn_content/TutorialVideo/Create
        public ActionResult Create()
        {
            SetCatInfo();
            return View();
        }

        private void SetCatInfo()
        {
            UnitOfWork dbContext = new UnitOfWork();
            var _categoryRepo = dbContext.GetRepository<Category>();
            var ieltCat = _categoryRepo.GetMany(c => c.Name.Contains("IELT")&&c.IsDeleted==false).Where(c => c.Name == "IELTS Foundation" || c.Name == "IELTS 5.5" || c.Name == "IELTS 6.5" || c.Name == "IELTS >7.5").OrderBy(c => c.OrderNumber).ToList();
            var satCat = _categoryRepo.GetMany(c => c.Name.Contains("SAT")&& c.IsDeleted == false).Where(c => c.Name == "SAT Beginner" || c.Name == "SAT Intermediate" || c.Name == "SAT Advanced" || c.Name == "SAT Super Advanced").OrderBy(c => c.OrderNumber).ToList();
            ieltCat.AddRange(satCat);
            ViewBag.CatId = new SelectList(ieltCat, "Id", "Name");
        }

        // POST: ttn_content/TutorialVideo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Link,Title,Status,CatId,Description")] TutorialVideo tutorialVideo, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    var uploadFile = Request.Files[0];

                    var fileName = Path.GetFileName(uploadFile.FileName);
                    fileName = Guid.NewGuid() + Path.GetExtension(fileName);
                    var file_src = Path.Combine(Server.MapPath("~/photo/Uploads/"), fileName);
                    tutorialVideo.Thumbnail = fileName;
                    tutorialVideo.CreateDate = DateTime.Now;
                    uploadFile.SaveAs(file_src);
                }

                db.TutorialVideos.Add(tutorialVideo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            SetCatInfo();

            return View(tutorialVideo);
        }

        // GET: ttn_content/TutorialVideo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TutorialVideo tutorialVideo = db.TutorialVideos.Find(id);
            if (tutorialVideo == null)
            {
                return HttpNotFound();
            }
            SetCatInfo();

            return View(tutorialVideo);
        }

        // POST: ttn_content/TutorialVideo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Link,Title,Status,CatId,Description,CreateDate,Thumbnail")] TutorialVideo tutorialVideo)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    var uploadFile = Request.Files[0];
                    if (uploadFile.ContentLength > 0)
                    {


                        var fileName = Path.GetFileName(uploadFile.FileName);
                        fileName = Guid.NewGuid() + Path.GetExtension(fileName);
                        var file_src = Path.Combine(Server.MapPath("~/photo/Uploads/"), fileName);
                        tutorialVideo.Thumbnail = fileName;

                        uploadFile.SaveAs(file_src);
                    }
                }

                db.Entry(tutorialVideo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            SetCatInfo();
            return View(tutorialVideo);
        }

        // GET: ttn_content/TutorialVideo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TutorialVideo tutorialVideo = db.TutorialVideos.Find(id);
            if (tutorialVideo == null)
            {
                return HttpNotFound();
            }
            return View(tutorialVideo);
        }

        // POST: ttn_content/TutorialVideo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TutorialVideo tutorialVideo = db.TutorialVideos.Find(id);
            db.TutorialVideos.Remove(tutorialVideo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult ShowContent(int? id)
        {
            ViewBag.Facebook = db.Settings.FirstOrDefault(t => t.Id == 2).Value;
            ViewBag.Youtube = db.Settings.FirstOrDefault(t => t.Id == 3).Value;
            ViewBag.Linked = db.Settings.FirstOrDefault(t => t.Id == 6).Value;
            ViewBag.Gplus = db.Settings.FirstOrDefault(t => t.Id == 5).Value;
            ViewBag.Instagram = db.Settings.FirstOrDefault(t => t.Id == 4).Value;

            ViewBag.Hotline = db.Settings.FirstOrDefault(t => t.Id == 1).Value;
            ViewBag.Address = db.Settings.FirstOrDefault(t => t.Key == "address").Value;
            ViewBag.Phone = db.Settings.FirstOrDefault(t => t.Key.Equals("phone")).Value;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TutorialVideo tutorialVideo = db.TutorialVideos.Find(id);
            if (tutorialVideo == null)
            {
                return HttpNotFound();
            }
            return View("~/Areas/ttn_content/Views/TutorialVideo/ShowContent.cshtml", tutorialVideo);
        }
        public ActionResult AllVideo()
        {

            ViewBag.Facebook = db.Settings.FirstOrDefault(t => t.Id == 2).Value;
            ViewBag.Youtube = db.Settings.FirstOrDefault(t => t.Id == 3).Value;
            ViewBag.Linked = db.Settings.FirstOrDefault(t => t.Id == 6).Value;
            ViewBag.Gplus = db.Settings.FirstOrDefault(t => t.Id == 5).Value;
            ViewBag.Instagram = db.Settings.FirstOrDefault(t => t.Id == 4).Value;

            ViewBag.Hotline = db.Settings.FirstOrDefault(t => t.Id == 1).Value;
            ViewBag.Address = db.Settings.FirstOrDefault(t => t.Key == "address").Value;
            ViewBag.Phone = db.Settings.FirstOrDefault(t => t.Key.Equals("phone")).Value;

                return View("~/Areas/ttn_content/Views/TutorialVideo/AllVideo.cshtml", db.TutorialVideos.ToList());
            
        }

        public ActionResult ListVideo(string catName)
        {

            ViewBag.Facebook = db.Settings.FirstOrDefault(t => t.Id == 2).Value;
            ViewBag.Youtube = db.Settings.FirstOrDefault(t => t.Id == 3).Value;
            ViewBag.Linked = db.Settings.FirstOrDefault(t => t.Id == 6).Value;
            ViewBag.Gplus = db.Settings.FirstOrDefault(t => t.Id == 5).Value;
            ViewBag.Instagram = db.Settings.FirstOrDefault(t => t.Id == 4).Value;

            ViewBag.Hotline = db.Settings.FirstOrDefault(t => t.Id == 1).Value;
            ViewBag.Address = db.Settings.FirstOrDefault(t => t.Key == "address").Value;
            ViewBag.Phone = db.Settings.FirstOrDefault(t => t.Key.Equals("phone")).Value;


            return View("~/Areas/ttn_content/Views/TutorialVideo/AllVideo.cshtml", db.TutorialVideos.Where(v => v.Category.SEOTitle == catName && v.Status).ToList());
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
