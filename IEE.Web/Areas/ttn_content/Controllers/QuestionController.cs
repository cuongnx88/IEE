using IEE.Infrastructure.DbContext;
using IEE.Web.Areas.ttn_content.Models;
using IEE.Web.Models;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuestionController : Controller
    {
        // GET: ttn_content/Question
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            try
            {
                using (var dbContext = new SATEntities())
                {
                    var model = dbContext.SATQuestions.Include("SATType").Where(q => q.IsDeleted == null || q.IsDeleted == false).ToList();

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        model = model.Where(t => t.Title.ToLower().Contains(keyword.ToLower())).ToList();
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
                    int pageNumber = (page ?? 1);
                    return View(model.OrderByDescending(t => t.ID).ToPagedList(pageNumber, int.Parse(ConfigurationManager.AppSettings["PageSize"])));

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
                ViewBag.FormValue = JsonConvert.SerializeObject(model);
                var listExamContent = new ExamContentSelectList().GetExamContentSelectList();
                ViewBag.ExamContentID = listExamContent;
                ViewBag.IsInputAnswer = false;
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
                    var listExamContent = db.SATExamContents.Include("SATExamForm").ToList();
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
                    var listExamContent = db.SATExamContents.Include("SATExamForm").ToList();
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
        public ActionResult Create(QuestionCreateViewModel model, List<SATAnswer> Answers, string btnSave)
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
                    ViewBag.FormValue = JsonConvert.SerializeObject(model);
                    if (Session["IsInputAnswer"] != null)
                    {
                        ViewBag.IsInputAnswer = (bool)Session["IsInputAnswer"];
                    }

                    return View("CreateWithAnswer", model);
                }
                using (var dbContext = new SATEntities())
                {
                    var questionModel = new SATQuestion();
                    questionModel.ExamContentID = model.ExamContentID;
                    questionModel.HasInputAnswer = model.HasInputAnswer;
                    questionModel.QuestionContent = model.QuestionContent;
                    questionModel.QuestionNo = model.QuestionNo;
                    questionModel.Status = btnSave == "Tạo mới & Đăng" ? true : false;
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
                ViewBag.EditQuestion = true;
                var hasInput = true;
                var textType = new SelectListItem() { Value = "1", Text = "Nhập vào" };
                var choiceType = new SelectListItem() { Value = "0", Text = "Chọn câu", Disabled = hasInput ? true : false };
                ViewBag.AnswerType = new SelectList(new List<SelectListItem>() { choiceType, textType }, "Value", "Text", hasInput ? "1" : "0");
                var questionEntity = db.SATQuestions.Find(id);


                var model = questionEntity.Map<QuestionEditViewModel>();
                model.Answers = new QuestionEditViewModel(id).Answers;

                ViewBag.FormValue = JsonConvert.SerializeObject(model, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                var listType = new SelectList(db.SATTypes.ToList(), "TypeID", "TypeName", questionEntity.TypeID);
                ViewBag.Type = listType;
                List<SelectListItem> listCalc = UseCalcSelectList();
                //ViewBag.UseCalc = new SelectList(listCalc, "Value", "Text", questionEntity.UserCalculator);
                var examContentID = questionEntity.ExamContentID != null ? questionEntity.ExamContentID.Value : 0;
                var listExamContent = new ExamContentSelectList().GetExamContentSelectList(examContentID);
                ViewBag.SelectedID = examContentID;
                ViewBag.ExamContentID = listExamContent;
                if (model.Answers?.Count > 1)
                {
                    ViewBag.IsInputAnswer = false;
                }
                else
                {
                    ViewBag.IsInputAnswer = true;
                }

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
                    var questionEntity = db.SATQuestions.Find(id);
                    ViewBag.EditQuestion = true;
                    var hasInput = true;
                    var textType = new SelectListItem() { Value = "1", Text = "Nhập vào" };
                    var choiceType = new SelectListItem() { Value = "0", Text = "Chọn câu", Disabled = hasInput ? true : false };
                    ViewBag.AnswerType = new SelectList(new List<SelectListItem>() { choiceType, textType }, "Value", "Text", hasInput ? "1" : "0");
                    var examContentID = questionEntity.ExamContentID != null ? questionEntity.ExamContentID.Value : 0;
                    ViewBag.SelectedID = examContentID;
                    var listExamContent = new ExamContentSelectList().GetExamContentSelectList(examContentID);
                    ViewBag.ExamContentID = listExamContent;
                    var listType = new SelectList(db.SATTypes.ToList(), "TypeID", "TypeName", questionEntity.TypeID);
                    ViewBag.Type = listType;

                    List<SelectListItem> listCalc = UseCalcSelectList();
                    ViewBag.UseCalc = new SelectList(listCalc, "Value", "Text");
                    if (!ModelState.IsValid)
                    {
                        if (Session["IsInputAnswer"] != null)
                        {
                            ViewBag.IsInputAnswer = (bool)Session["IsInputAnswer"];
                        }
                        ViewBag.FormValue = JsonConvert.SerializeObject(model);
                        return View("EditWithAnswer", model);
                    }


                    questionEntity.ExamContentID = model.ExamContentID;
                    questionEntity.HasInputAnswer = model.HasInputAnswer;
                    questionEntity.QuestionContent = model.QuestionContent;
                    questionEntity.QuestionNo = model.QuestionNo;
                    questionEntity.Status = model.Status;
                    questionEntity.Title = model.Title;
                    questionEntity.TypeID = model.TypeID;



                    var entry = db.Entry<SATQuestion>(questionEntity);

                    entry.State = System.Data.Entity.EntityState.Modified;

                    if (Answers.Count == 1)
                    {
                        var qID = Answers[0].QuestionID;
                        var _answ = Answers[0];
                        var other = db.SATAnswers.Where(a => a.QuestionID == qID && a.ID != _answ.ID).ToList();
                        foreach (var item in other)
                        {
                            item.Status = false;
                            var _entry = db.Entry<SATAnswer>(item);
                            _entry.State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    foreach (var item in Answers)
                    {
                        var _entity = db.SATAnswers.Find(item.ID);
                        _entity.AnswerContent = item.AnswerContent;
                        _entity.AnswerType = item.AnswerType;
                        _entity.IsRightAnswer = item.IsRightAnswer;
                        _entity.Mark = item.Mark;
                        _entity.QuestionID = id;
                        _entity.Status = item.Status;
                        _entity.Explanation = item.Explanation;

                        var _entry = db.Entry<SATAnswer>(_entity);
                        _entry.State = System.Data.Entity.EntityState.Modified;
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                if (Session["IsInputAnswer"] != null)
                {
                    ViewBag.IsInputAnswer = (bool)Session["IsInputAnswer"];
                }
                ViewBag.FormValue = JsonConvert.SerializeObject(model);
                return View("EditWithAnswer", model);
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
                        //deleteEntity.Status = false;
                        deleteEntity.IsDeleted = true;
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

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GetAnswerDivEdit( bool isInput, QuestionEditViewModel editFormValue)
        {
            object model = new Object();
            string inputViewPath = "";
            string anserViewPath = "";
            
                var data = editFormValue;
                if (data.Answers?.Count == 1)
                {
                    var qId = data.Answers[0].QuestionID;
                    using (var db = new SATEntities())
                    {
                        var otherAnsw = db.SATAnswers.Where(a => a.QuestionID == qId).ToList();
                        if (otherAnsw?.Count == 1)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                otherAnsw.Add(new SATAnswer { AnswerContent = String.Empty, AnswerType = 0, IsRightAnswer = false, Mark = null, Status = false, QuestionID = qId });
                            }
                        }
                        data.Answers = otherAnsw;
                    }
                }
                model = data;
                inputViewPath = "_InputAnswerDivEdit.cshtml";
                anserViewPath = "_AnswerDivEdit.cshtml";

           

            if (isInput)
            {
                Session["IsInputAnswer"] = true;

                return Json(new { data = Content(Utils.Instance.RenderViewToString(@"~\Areas\ttn_content\Views\Question\" + inputViewPath, model))},JsonRequestBehavior.AllowGet);
            }
            else
            {

                Session["IsInputAnswer"] = false;
                return Json(new { data = Content(Utils.Instance.RenderViewToString(@"~\Areas\ttn_content\Views\Question\" + anserViewPath, model)) }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetAnswerDivCreate(bool isInput, QuestionCreateViewModel createFormValue)
        {
            object model = new Object();
            string inputViewPath = "";
            string anserViewPath = "";


            model = createFormValue;
            inputViewPath = "_InputAnswerDiv.cshtml";
            anserViewPath = "_AnswerDiv.cshtml";


            if (isInput)
            {
                Session["IsInputAnswer"] = true;

                return Json(new { data = Content(Utils.Instance.RenderViewToString(@"~\Areas\ttn_content\Views\Question\" + inputViewPath, model)) }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                Session["IsInputAnswer"] = false;
                return Json(new { data = Content(Utils.Instance.RenderViewToString(@"~\Areas\ttn_content\Views\Question\" + anserViewPath, model)) }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}