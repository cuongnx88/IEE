using IEE.Infrastructure.DbContext;
using IEE.Web.Areas.ttn_content.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    public class AnswerController : Controller
    {
        private SATEntities db = new SATEntities();

        // GET: ttn_content/SATAnswers
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            int pageNumber = (page ?? 1);
            using (var db = new SATEntities())
            {
                var model = new AnswerViewModel();
                model.ListQuestion = db.SATQuestions.Include("SATAnswers").ToList().Where(q => q.Status == true).ToPagedList(pageNumber, int.Parse(ConfigurationManager.AppSettings["PageSize"]));


                if (!string.IsNullOrEmpty(keyword))
                {
                    model.ListQuestion = model.ListQuestion.ToList().Where(t => t.Title.ToLower().Contains(keyword.ToLower())).ToPagedList(pageNumber, int.Parse(ConfigurationManager.AppSettings["PageSize"]));
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    page = 1;
                }
                else
                {
                    keyword = CurrentFilter;
                }

                ViewBag.CurrentFilter = keyword;

                model.ListQuestion = model.ListQuestion.ToList().OrderByDescending(t => t.ID).ToPagedList(pageNumber, int.Parse(ConfigurationManager.AppSettings["PageSize"]));
                return View(model);

            }

        }


        // GET: ttn_content/SATAnswers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SATAnswer sATAnswer = db.SATAnswers.Find(id);
            if (sATAnswer == null)
            {
                return HttpNotFound();
            }
            return View(sATAnswer);
        }

        // GET: ttn_content/SATAnswers/Create
        public ActionResult Create(int questionId)
        {
            var listQuestion = db.SATQuestions.Where(q => q.SATAnswers.Count < 4).ToList();
            ViewBag.QuestionID = new SelectList(listQuestion, "ID", "Title", questionId);
            var hasInput = db.SATQuestions.Find(questionId).HasInputAnswer;
            var textType = new SelectListItem() { Value = "1", Text = "Nhập vào" };
            var choiceType = new SelectListItem() { Value = "0", Text = "Chọn câu", Disabled = hasInput.HasValue && hasInput.Value ? true : false };

            var disable = new List<string>() { "0" };
            ViewBag.AnswerType = new SelectList(new List<SelectListItem>() { choiceType, textType }, "Value", "Text", hasInput.HasValue && hasInput.Value ? "1" : "0");

            var model = new SATAnswer();
            model.Status = false;
            return View(model);
        }

        // POST: ttn_content/SATAnswers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,AnswerContent,IsRightAnswer,Status,QuestionID,AnswerType,Mark")] SATAnswer sATAnswer)
        {
            try
            {
                ViewBag.QuestionID = new SelectList(db.SATQuestions, "ID", "Title", sATAnswer.QuestionID);
                var textType = new SelectListItem() { Value = "1", Text = "Nhập vào" };
                var choiceType = new SelectListItem() { Value = "0", Text = "Chọn câu" };
                ViewBag.AnswerType = new SelectList(new List<SelectListItem>() { choiceType, textType }, "Value", "Text");
                if (ModelState.IsValid)
                {
                    db.SATAnswers.Add(sATAnswer);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }


                return View(sATAnswer);
            }
            catch (Exception ex)
            {

                return View(sATAnswer);
            }

        }

        // GET: ttn_content/SATAnswers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SATAnswer sATAnswer = db.SATAnswers.Find(id);
            if (sATAnswer == null)
            {
                return HttpNotFound();
            }

            var listQuestion = db.SATQuestions.ToList();
            ViewBag.QuestionID = new SelectList(listQuestion, "ID", "Title", sATAnswer.QuestionID);
            var textType = new SelectListItem() { Value = "1", Text = "Nhập vào" };
            var choiceType = new SelectListItem() { Value = "0", Text = "Chọn câu" };
            ViewBag.AnswerType = new SelectList(new List<SelectListItem>() { choiceType, textType }, "Value", "Text", sATAnswer.AnswerType);
            ViewBag.Mark = sATAnswer.Mark;
            ViewBag.SelectedQuestID = sATAnswer.QuestionID;
            return View(sATAnswer);
        }

        // POST: ttn_content/SATAnswers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,AnswerContent,Mark,IsRightAnswer,Status,QuestionID,AnswerType")] SATAnswer sATAnswer)
        {
            try
            {
                var listQuestion = db.SATQuestions.ToList();
                ViewBag.QuestionID = new SelectList(listQuestion, "ID", "Title", sATAnswer.QuestionID);
                var textType = new SelectListItem() { Value = "1", Text = "Nhập vào" };
                var choiceType = new SelectListItem() { Value = "0", Text = "Chọn câu" };
                ViewBag.AnswerType = new SelectList(new List<SelectListItem>() { choiceType, textType }, "Value", "Text", sATAnswer.AnswerType);
                ViewBag.QuestionID = new SelectList(db.SATQuestions, "ID", "Title", sATAnswer.QuestionID);
                if (ModelState.IsValid)
                {
                    var answEntity = db.SATAnswers.Find(sATAnswer.ID);
                    var maxAnswerQuestion = db.SATQuestions.Find(sATAnswer.QuestionID).SATAnswers.Count();
                    if (maxAnswerQuestion >= 4)
                    {
                        if (answEntity.QuestionID != sATAnswer.QuestionID)
                        {
                            ModelState.AddModelError("MaxAnswerError", "Câu hỏi này đã có đủ đáp án cần thiết, hãy chọn lại");
                            return View(sATAnswer);
                        }


                    }
                    db.Entry(answEntity).CurrentValues.SetValues(sATAnswer);
                    db.Entry(answEntity).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(sATAnswer);
            }
            catch (Exception ex)
            {

                return View(sATAnswer);
            }

        }

        // GET: ttn_content/SATAnswers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SATAnswer sATAnswer = db.SATAnswers.Find(id);
            if (sATAnswer == null)
            {
                return HttpNotFound();
            }
            return View(sATAnswer);
        }

        // POST: ttn_content/SATAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SATAnswer sATAnswer = db.SATAnswers.Find(id);
            db.SATAnswers.Remove(sATAnswer);
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
