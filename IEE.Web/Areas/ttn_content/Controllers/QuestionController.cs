using IEE.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IEE.Model;
using IEE.Infrastructure.DbContext;
using AutoMapper;
using IEE.Web.Areas.ttn_content.Models;
using IEE.Web.Models;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuestionController : Controller
    {
        // GET: ttn_content/Question
        public ActionResult Index()
        {
            try
            {

                using (var dbContext = new SATEntities())
                {
                    var model = dbContext.SATQuestions.Include("SATType").Where(q => q.Status == true).ToList();
                    return View(model);
                }
            }
            catch (Exception)
            {

                return View(new List<SATQuestion>());
            }

        }

        public ActionResult Create()
        {
            using (var dbContext = new SATEntities())
            {
                var hasInput = true;
                var textType = new SelectListItem() { Value = "1", Text = "Nhập vào" };
                var choiceType = new SelectListItem() { Value = "0", Text = "Chọn câu", Disabled = hasInput ? true : false };
                ViewBag.AnswerType = new SelectList(new List<SelectListItem>() { choiceType, textType }, "Value", "Text", hasInput ? "1" : "0");
                var listType = new SelectList(dbContext.SATTypes.ToList(), "TypeID", "TypeName");
                ViewBag.Type = listType;
                List<SelectListItem> listCalc = UseCalcSelectList();
                ViewBag.UseCalc = new SelectList(listCalc, "Value", "Text");
                var model = new QuestionCreateViewModel();
                model.Status = false;
                var listExamContent = new ExamContentSelectList().GetExamContentSelectList();
                ViewBag.ExamContentID = listExamContent;
                return View("CreateWithAnswer", model);
            }

        }
        public class ExamContentSelectList
        {
            [System.ComponentModel.DataAnnotations.Display(Name = "Thuộc nội dung")]
            public string ExamContentID { get; set; }
            public string Title { get; set; }

            public SelectList GetExamContentSelectList()
            {
                using (var db = new SATEntities())
                {
                    var listExamContent = db.SATExamContents.Include("SATExamForm").Where(e => e.SATQuestions.Count() < 10).ToList();
                    var examContentSelectList = new List<ExamContentSelectList>();
                    examContentSelectList.Add(new ExamContentSelectList { ExamContentID = "", Title = "Thêm vào sau" });
                    foreach (var item in listExamContent)
                    {
                        examContentSelectList.Add(new ExamContentSelectList { ExamContentID = item.ID.ToString(), Title = item.SATExamForm.SATType.TypeName + "--" + item.Title });
                    }
                    var selectList = new SelectList(examContentSelectList, "ExamContentID", "Title");
                    return selectList;
                }
            }
            public SelectList GetExamContentSelectList(int selectedID)
            {
                using (var db = new SATEntities())
                {
                    var listExamContent = db.SATExamContents.Include("SATExamForm").Where(e => e.SATQuestions.Count() < 10).ToList();
                    object selectedValue;
                    if (selectedID != 0)
                    {
                        var current = db.SATExamContents.Find(selectedID);
                        listExamContent.Add(current);
                        selectedValue = selectedID;
                    }
                    else
                    {
                        selectedValue = "";
                    }

                    var examContentSelectList = new List<ExamContentSelectList>();
                    examContentSelectList.Add(new ExamContentSelectList { ExamContentID = "", Title = "Thêm vào sau" });
                    foreach (var item in listExamContent)
                    {
                        examContentSelectList.Add(new ExamContentSelectList { ExamContentID = item.ID.ToString(), Title = item.SATExamForm.SATType.TypeName + "--" + item.Title });
                    }
                    var selectList = new SelectList(examContentSelectList, "ExamContentID", "Title", selectedValue);
                    return selectList;
                }
            }
        }

        private static List<SelectListItem> UseCalcSelectList()
        {
            var use = new SelectListItem() { Text = "Có", Value = "true" };
            var noUse = new SelectListItem() { Text = "Không", Value = "false" };
            var notSet = new SelectListItem() { Text = "không thiết lập", Value = null };
            var listCalc = new List<SelectListItem>() { notSet, use, noUse };
            return listCalc;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(QuestionCreateViewModel model, List<SATAnswer> Answers)
        {

            var db = new SATEntities();
            var listType = new SelectList(db.SATTypes.ToList(), "TypeID", "TypeName");
            ViewBag.Type = listType;
            List<SelectListItem> listCalc = UseCalcSelectList();
            ViewBag.UseCalc = new SelectList(listCalc, "Value", "Text");
            var listExamContent = new ExamContentSelectList().GetExamContentSelectList();
            ViewBag.ExamContentID = listExamContent;
            db.Dispose();
            var hasInput = true;
            var textType = new SelectListItem() { Value = "1", Text = "Nhập vào" };
            var choiceType = new SelectListItem() { Value = "0", Text = "Chọn câu", Disabled = hasInput ? true : false };
            ViewBag.AnswerType = new SelectList(new List<SelectListItem>() { choiceType, textType }, "Value", "Text", hasInput ? "1" : "0");
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("CreateWithAnswer", model);
                }
                using (var dbContext = new SATEntities())
                {
                    var questionModel = new SATQuestion();
                    questionModel.ExamContentID = model.ExamContentID;
                    questionModel.HasInputAnswer = model.HasInputAnswer;
                    questionModel.QuestionContent = model.QuestionContent;
                    questionModel.QuestionNo = model.QuestionNo;
                    questionModel.Status = model.Status;
                    questionModel.Title = model.Title;
                    questionModel.TypeID = model.TypeID;



                    dbContext.SATQuestions.Add(questionModel);
                    dbContext.SaveChanges();

                    var lastest = dbContext.SATQuestions.Max(q => q.ID);
                    foreach (var item in Answers)
                    {
                        item.QuestionID = lastest;
                    }
                    dbContext.SATAnswers.AddRange(Answers);
                    dbContext.SaveChanges();

                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {

                return View("CreateWithAnswer", model);
            }

        }

        public ActionResult Edit(int id)
        {
            using (var db = new SATEntities())
            {
                var hasInput = true;
                var textType = new SelectListItem() { Value = "1", Text = "Nhập vào" };
                var choiceType = new SelectListItem() { Value = "0", Text = "Chọn câu", Disabled = hasInput ? true : false };
                ViewBag.AnswerType = new SelectList(new List<SelectListItem>() { choiceType, textType }, "Value", "Text", hasInput ? "1" : "0");
                var questionEntity = db.SATQuestions.Find(id);


                var model = questionEntity.Map<QuestionEditViewModel>();
                model.Answers = new QuestionEditViewModel(id).Answers;
                var listType = new SelectList(db.SATTypes.ToList(), "TypeID", "TypeName", questionEntity.TypeID);
                ViewBag.Type = listType;
                List<SelectListItem> listCalc = UseCalcSelectList();
                //ViewBag.UseCalc = new SelectList(listCalc, "Value", "Text", questionEntity.UserCalculator);
                var examContentID = questionEntity.ExamContentID != null ? questionEntity.ExamContentID.Value : 0;
                var listExamContent = new ExamContentSelectList().GetExamContentSelectList(examContentID);
                ViewBag.SelectedID = examContentID;
                ViewBag.ExamContentID = listExamContent;
                return View("EditWithAnswer", model);
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, QuestionEditViewModel model, List<SATAnswer> Answers)
        {
            try
            {
                using (var db = new SATEntities())
                {
                    var hasInput = true;
                    var textType = new SelectListItem() { Value = "1", Text = "Nhập vào" };
                    var choiceType = new SelectListItem() { Value = "0", Text = "Chọn câu", Disabled = hasInput ? true : false };
                    ViewBag.AnswerType = new SelectList(new List<SelectListItem>() { choiceType, textType }, "Value", "Text", hasInput ? "1" : "0");

                    var questionEntity = db.SATQuestions.Find(id);
                    questionEntity.ExamContentID = model.ExamContentID;
                    questionEntity.HasInputAnswer = model.HasInputAnswer;
                    questionEntity.QuestionContent = model.QuestionContent;
                    questionEntity.QuestionNo = model.QuestionNo;
                    questionEntity.Status = model.Status;
                    questionEntity.Title = model.Title;
                    questionEntity.TypeID = model.TypeID;

                    var listType = new SelectList(db.SATTypes.ToList(), "TypeID", "TypeName", questionEntity.TypeID);
                    ViewBag.Type = listType;
                    List<SelectListItem> listCalc = UseCalcSelectList();
                    ViewBag.UseCalc = new SelectList(listCalc, "Value", "Text");
                    var listExamContent = new ExamContentSelectList().GetExamContentSelectList();
                    ViewBag.ExamContentID = listExamContent;
                    var entry = db.Entry<SATQuestion>(questionEntity);

                    entry.State = System.Data.Entity.EntityState.Modified;

                    foreach (var item in Answers)
                    {
                        var _entity = db.SATAnswers.Find(item.ID);
                        _entity.AnswerContent = item.AnswerContent;
                        _entity.AnswerType = item.AnswerType;
                        _entity.IsRightAnswer = item.IsRightAnswer;
                        _entity.Mark = item.Mark;
                        _entity.QuestionID = id;
                        _entity.Status = item.Status;

                        var _entry = db.Entry<SATAnswer>(_entity);
                        _entry.State = System.Data.Entity.EntityState.Modified;
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

                return View(model);
            }

        }

        public ActionResult Delete(int id)
        {

            using (var db = new SATEntities())
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var deleteEntity = db.SATQuestions.Find(id);
                        deleteEntity.Status = false;
                        //disable relate answers
                        var answers = db.SATAnswers.Where(a => a.QuestionID == id).ToList();
                        //delete underline text
                        var underlines = db.SATWritingUnderlineTexts.Where(u => u.QuestionID == id).ToList();

                        foreach (var item in answers)
                        {
                            item.Status = false;
                            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }

                        db.SATWritingUnderlineTexts.RemoveRange(underlines);
                        db.SaveChanges();

                        db.Entry(deleteEntity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        dbTransaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        return View("Index");
                    }
                }
            }

        }
    }
}