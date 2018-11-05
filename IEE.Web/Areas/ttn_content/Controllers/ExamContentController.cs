using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEE.Infrastructure.DbContext;
using System.IO;
using System.Web.Hosting;
using IEE.Web.Areas.ttn_content.Models;
using Newtonsoft.Json;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExamContentController : Controller
    {
        private SATEntities db = new SATEntities();

        // GET: ttn_content/ExamContent
        public ActionResult Index()
        {
            var sATExamContents = db.SATExamContents.Include(s => s.SATExamForm);
            return View(sATExamContents.ToList());
        }

        // GET: ttn_content/ExamContent/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SATExamContent sATExamContent = db.SATExamContents.Find(id);
            if (sATExamContent == null)
            {
                return HttpNotFound();
            }
            return View(sATExamContent);
        }

        // GET: ttn_content/ExamContent/Create
        public ActionResult Create()
        {
            ViewBag.ExamFormID = new SelectList(db.SATExamForms.Where(ef => ef.Status == true), "ID", "Title");
            var listUnSetExamContentIDQuestion = new SelectList(db.SATQuestions.Where(q => q.ExamContentID == null && q.SATWritingUnderlineTexts.Any(u => u.QuestionID == q.ID) == false).ToList(), "ID", "Title");
            ViewBag.ListQuestion = listUnSetExamContentIDQuestion;
            return View();
        }

        private class QuestionList
        {
            public int ID { get; set; }
            public string Title { get; set; }

            public QuestionList()
            {

            }
            public QuestionList(int id, string title)
            {
                ID = id;
                Title = title;
            }
        }
        public ActionResult GetListUnsetQuestion()
        {
            var listUnSetExamContentIDQuestion = db.SATQuestions.Where(q => q.ExamContentID == null && q.Status == true).ToList();
            var listQuestion = new List<QuestionList>();
            foreach (var item in listUnSetExamContentIDQuestion)
            {
                listQuestion.Add(new QuestionList(item.ID, item.Title));
            }

            return Json(new { list = listQuestion }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public ActionResult AddQuestionToLine(int questionID, string selectedText, int? examContentId = 0)
        {

            var underlineText = new SATWritingUnderlineText();
            underlineText.QuestionID = questionID;
            underlineText.UnderlineText = selectedText;
            underlineText.Number = db.SATQuestions.Find(questionID).QuestionNo;
            db.SATWritingUnderlineTexts.Add(underlineText);
            var _question = db.SATQuestions.Find(questionID);
            if (examContentId != 0)
            {
                _question.ExamContentID = examContentId;
            }
            else
            {
                if (Session["QuestionID"] != null)
                {
                    Session["QuestionID"] = Session["QuestionID"].ToString() + questionID + ",";
                }
                else
                {
                    Session["QuestionID"] = questionID;
                }

            }

            db.Entry(_question).State = EntityState.Modified;
            db.SaveChanges();

            var question = db.SATQuestions.Where(q => q.ID == questionID);
            var countListQuestion = db.SATQuestions.Where(q => q.ExamContentID == null && q.SATWritingUnderlineTexts.Any(u => u.QuestionID == q.ID) == false).Except(question).Count();
            var inputHidden = "<input type = 'hidden' id = 'questionID' name='questionToAdd' value = '" + questionID + "' />";

            return Json(new { countListQuestion = countListQuestion, inputHidden = inputHidden, number = underlineText.Number }, JsonRequestBehavior.AllowGet);
        }

        // POST: ttn_content/ExamContent/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "ID,ExamContentIndex,AttachImage,Contents,Status,ExamFormID,Title,Intro")] SATExamContent sATExamContent, FormCollection collection)
        {
            var contentLines = collection.GetValues("inputLine") != null ? collection.GetValues("inputLine").ToList() : new List<string>();
            var questionToAdd = collection.GetValues("questionToAdd") != null ? collection.GetValues("questionToAdd").ToList() : new List<string>();
            if (ModelState.IsValid)
            {

                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //add exam content
                        sATExamContent.rowguid = Guid.NewGuid();
                        db.SATExamContents.Add(sATExamContent);
                        if (db.SaveChanges() > 0)
                        {
                            //add conten lines
                            var newId = db.SATExamContents.FirstOrDefault(e => e.rowguid == sATExamContent.rowguid);
                            if (newId != null)
                            {
                                var index = 1;
                                foreach (var item in contentLines)
                                {
                                    var line = new SATContentLine();
                                    line.LineIndex = index;
                                    index = index++;
                                    line.LineText = item;
                                    line.ExamContentID = newId.ID;
                                    line.rowguid = Guid.NewGuid();
                                    db.SATContentLines.Add(line);
                                    db.SaveChanges();
                                }
                            }
                            //update question's ExamContentID
                            foreach (var item in questionToAdd)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    var questionID = int.Parse(item);
                                    var question = db.SATQuestions.Find(questionID);
                                    question.ExamContentID = newId.ID;
                                    db.Entry(question).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }



                        // db.SaveChanges();
                        dbTransaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ViewBag.ExamFormID = new SelectList(db.SATExamForms.Where(ef => ef.Status == true), "ID", "Title", sATExamContent.ExamFormID);
                        ViewBag.ContentLines = contentLines;

                        var listUnSetExamContentIDQuestion = new SelectList(db.SATQuestions.Where(q => q.ExamContentID == null && q.SATWritingUnderlineTexts.Any(u => u.QuestionID == q.ID) == false).ToList(), "ID", "Title");
                        ViewBag.ListQuestion = listUnSetExamContentIDQuestion;
                        ViewBag.ListInputHidden = questionToAdd;
                        return View(sATExamContent);
                    }

                }

            }

            ViewBag.ExamFormID = new SelectList(db.SATExamForms.Where(ef => ef.Status == true), "ID", "Title", sATExamContent.ExamFormID);
            ViewBag.ContentLines = contentLines;
            return View(sATExamContent);
        }

        // GET: ttn_content/ExamContent/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SATExamContent sATExamContent = db.SATExamContents.Find(id);
            if (sATExamContent == null)
            {
                return HttpNotFound();
            }
            ViewBag.ExamFormID = new SelectList(db.SATExamForms.Where(ef => ef.Status == true), "ID", "Title", sATExamContent.ExamFormID);
            var listUnSetExamContentIDQuestion = new SelectList(db.SATQuestions.Where(q => q.ExamContentID == null && q.SATWritingUnderlineTexts.Any(u => u.QuestionID == q.ID) == false).ToList(), "ID", "Title");
            ViewBag.ListQuestion = listUnSetExamContentIDQuestion;
            var viewModel = new ExamContentEditViewModel(sATExamContent);
            return View(viewModel);
        }

        // POST: ttn_content/ExamContent/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(ExamContentEditViewModel viewModel, int id, FormCollection collection)
        {
            var contentLines = collection.GetValues("inputLine") != null ? collection.GetValues("inputLine").ToList() : new List<string>();
            ViewBag.ContentLines = contentLines;
            ViewBag.ExamFormID = new SelectList(db.SATExamForms, "ID", "Title", viewModel.ExamContent.ExamFormID);
            var listUnSetExamContentIDQuestion = new SelectList(db.SATQuestions.Where(q => q.ExamContentID == null && q.SATWritingUnderlineTexts.Any(u => u.QuestionID == q.ID) == false).ToList(), "ID", "Title");
            ViewBag.ListQuestion = listUnSetExamContentIDQuestion;
            if (ModelState.IsValid)
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (viewModel.ContentLines == null && contentLines.Count > 0)
                        {
                            //delete old lines
                            var oldLines = db.SATContentLines.Where(l => l.ExamContentID == id);
                            db.SATContentLines.RemoveRange(oldLines);
                            db.SaveChanges();
                            //add new lines
                            var index = 0;
                            foreach (var item in contentLines)
                            {
                                var line = new SATContentLine();
                                line.LineIndex = index++;
                                line.LineText = item;
                                line.ExamContentID = id;
                                line.rowguid = Guid.NewGuid();
                                db.SATContentLines.Add(line);
                                db.SaveChanges();
                            }
                        }
                        else if (viewModel.ContentLines != null)

                        {
                            foreach (var item in viewModel.ContentLines)
                            {
                                db.Entry(item).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                        db.Entry(viewModel.ExamContent).State = EntityState.Modified;
                        db.SaveChanges();
                        dbTransaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        dbTransaction.Rollback();
                        ViewBag.ExamFormID = new SelectList(db.SATExamForms.Where(ef => ef.Status == true), "ID", "Title", viewModel.ExamContent.ExamFormID);
                        ViewBag.ContentLines = contentLines;

                        return View(viewModel);
                    }

                }
            }

            return View(viewModel);
        }

        // GET: ttn_content/ExamContent/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SATExamContent sATExamContent = db.SATExamContents.Find(id);
            if (sATExamContent == null)
            {
                return HttpNotFound();
            }
            return View(sATExamContent);
        }

        // POST: ttn_content/ExamContent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SATExamContent sATExamContent = db.SATExamContents.Find(id);
            var lines = db.SATContentLines.Where(l => l.ExamContentID == id).ToList();
            db.SATContentLines.RemoveRange(lines);
            db.SATExamContents.Remove(sATExamContent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // [Route("type/GetType")]
        public ActionResult GetType(int formID)
        {
            var typeName = db.SATExamForms.Include("SATType").Where(f => f.ID == formID).Select(t => t.SATType).FirstOrDefault().TypeName;
            return Json(new { typeName = typeName });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadContentFile()
        {
            var myFile = Request.Files[0];
            var texts = "";
            if (myFile != null && myFile.ContentLength != 0)
            {
                string pathForSaving = Server.MapPath("~/Content/Uploads/");

                try
                {
                    string ext = Path.GetExtension(myFile.FileName.ToString().ToUpper()).ToLower();
                    long size = myFile.ContentLength;//get size of image

                    if (size < 512000)
                    {
                        if (ext == ".txt" || ext == ".doc" || ext == ".docx")
                        {

                            var stream = myFile.InputStream;

                            var fileName = pathForSaving + Guid.NewGuid().ToString();
                            myFile.SaveAs(fileName);
                            object miss = System.Reflection.Missing.Value;
                            object readOnly = true;
                            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                            Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(fileName, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);

                            for (int i = 0; i < docs.Paragraphs.Count; i++)
                            {
                                texts += docs.Paragraphs[i + 1].Range.Text.ToString() + ";";
                            }
                            docs.Close();
                            word.Quit();
                            myFile.InputStream.Dispose();
                            System.IO.File.Delete(fileName);
                            return Json(new { texts = texts });


                        }

                    }

                }
                catch (Exception)
                {
                    return Json(new { texts = texts }, JsonRequestBehavior.AllowGet);
                }

            }


            return Json(new { texts = texts }, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadImage()
        {


            var myFile = Request.Files[0];
            if (myFile != null && myFile.ContentLength != 0)
            {
                string pathForSaving = Server.MapPath("~/Content/Uploads/");

                try
                {
                    string ext = Path.GetExtension(myFile.FileName.ToString().ToUpper()).ToLower();
                    long size = myFile.ContentLength;//get size of image
                    if (size < 1524000)
                    {
                        if (ext == ".jpg" || ext == ".png" || ext == ".gif")
                        {
                            var guid = Guid.NewGuid().ToString() + ext;
                            var fileName = pathForSaving + guid;
                            var url = "~/Content/Uploads/" + guid;
                            myFile.SaveAs(fileName);
                            return Json(new { url = url });


                        }

                    }

                }
                catch (Exception)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

            }


            return Json(false, JsonRequestBehavior.AllowGet);

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
