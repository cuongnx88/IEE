using IEE.Infrastructure.DbContext;
using IEE.Web.Areas.ttn_content.Models;
using IEE.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEE.Web.Controllers
{
    [Authorize(Roles = "Examinee")]

    public class SATTestController : Controller
    {
        private ApplicationUserManager userManager;
        private SATEntities db = new SATEntities();
        public ApplicationUserManager UserManager
        {
            get
            {
                return this.userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            private set
            {
                this.userManager = value;
            }
        }

        // GET: SATTest
        public ActionResult Index()
        {

            try
            {
                var categories = db.Categories.Where(t => t.ParentId == 1).OrderBy(t => t.OrderNumber).ToList();
                ViewBag.Categories = categories;
                ViewBag.Facebook = db.Settings.FirstOrDefault(t => t.Id == 2).Value;
                ViewBag.Youtube = db.Settings.FirstOrDefault(t => t.Id == 3).Value;
                ViewBag.Linked = db.Settings.FirstOrDefault(t => t.Id == 6).Value;
                ViewBag.Gplus = db.Settings.FirstOrDefault(t => t.Id == 5).Value;
                ViewBag.Instagram = db.Settings.FirstOrDefault(t => t.Id == 4).Value;

                ViewBag.Hotline = db.Settings.FirstOrDefault(t => t.Id == 1).Value;
                ViewBag.Address = db.Settings.FirstOrDefault(t => t.Key == "address").Value;
                ViewBag.Phone = db.Settings.FirstOrDefault(t => t.Key.Equals("phone")).Value;

                var email = Session["AccountEmail"].ToString();
                //var _user = await UserManager.FindByEmailAsync(email);
                using (var db = new SATEntities())
                {
                    var user = db.Users.Where(u => u.Email == email).OrderByDescending(u => u.Id).FirstOrDefault();
                    var userTestListInfo = db.SATTests.Include("SATTestSections").Where(u => u.UserID == user.Id).ToList();

                    return View(userTestListInfo);
                }
            }
            catch (Exception)
            {

                return RedirectToRoute("SATLogin");
            }
        }

        public ActionResult Test()
        {

            try
            {
                using (var db = new SATEntities())
                {
                    var email = Session["AccountEmail"].ToString();
                    var user = db.Users.Where(u => u.Email == email).FirstOrDefault();
                    var countTest = db.SATTests.Where(t => t.UserID == user.Id);
                    if (countTest != null && countTest.ToList().Count > 0)
                    {
                        var examForm = db.SATExamForms.OrderBy(e => e.Section).Where(e => e.Status == true).ToList();
                        ViewBag.ExamForm = examForm;
                        return View();
                    }
                    return RedirectToRoute("SATTestIndex");

                }
            }
            catch (Exception)
            {

                return RedirectToRoute("SATLogin");
            }

        }

        [HttpPost]
        public ActionResult Test(SATTest model, FormCollection collection)
        {

            var choose = collection.Get("sectionValue");
            var examCode = !string.IsNullOrEmpty(collection.Get("examCode")) ? int.Parse(collection.Get("examCode")) : 1;
            using (var db = new SATEntities())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var email = Session["AccountEmail"].ToString();
                        var user = db.Users.Where(u => u.Email == email).FirstOrDefault();
                        var now = DateTime.Now;
                        var section = int.Parse(choose);

                        var _existTest = db.SATTests.Include("User").Any(u => u.UserID == user.Id && (u.User.IsDeleted == null || u.User.IsDeleted == false) && u.ExamCode == examCode);
                        if (_existTest)
                        {
                            var testData = db.SATTests.FirstOrDefault(t => t.UserID == user.Id && t.Status == false);

                            var sectionData = testData.SATTestSections.FirstOrDefault(s => s.Section == section);
                            sectionData.TotalRightAnswer = 0;
                            //var type = db.SATExamForms.Where(e => e.TypeID == section).FirstOrDefault();
                            //var typeID = type != null ? type.TypeID : 0;
                            var defaultScore = db.SATScores.Where(s => s.RawValue == 0 && s.TypeID == section).FirstOrDefault();
                            sectionData.ScoreID = defaultScore.ID;
                            sectionData.Section = section;
                            sectionData.rowguid = testData.rowguid;
                            sectionData.StartTime = now;
                            sectionData.EndTime = now.AddMinutes(Utils.Instance.GetSectionDuration(section));

                            var entry = db.Entry(sectionData);
                            entry.State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            Session["Section"] = section;
                            TempData["guidTest"] = testData.rowguid.ToString();
                            TempData.Keep("guidTest");
                            transaction.Commit();
                            return RedirectToAction("BeginTest", new { guid = testData.rowguid });
                        }
                        else
                        {
                            //new test
                            #region NewTest
                            var duration = Utils.Instance.GetSectionDuration(section);
                            // add pre define test
                            var beginTest = new SATTest();
                            beginTest.ExamCode = examCode;
                            beginTest.StartTime = now;
                            beginTest.EndTime = now.AddMinutes(duration);
                            beginTest.SubmitTime = beginTest.EndTime;
                            beginTest.TotalScore = 0;
                            beginTest.Status = false;// true is done the test
                            beginTest.rowguid = Guid.NewGuid();
                            beginTest.UserID = user.Id;

                            db.SATTests.Add(beginTest);
                            db.SaveChanges();

                            var currentTest = db.SATTests.Where(t => t.rowguid == beginTest.rowguid).FirstOrDefault();

                            var timeStep = now;
                            var testSectionRedirectGuid = Guid.Empty;

                            //add pre define section
                            for (int i = 1; i < 5; i++)
                            {
                                var testSection = new SATTestSection();
                                testSection.TestID = currentTest.ID;
                                testSection.TotalRightAnswer = 0;
                                //var type = db.SATExamForms.Where(e => e.Section == i).FirstOrDefault().TypeID;
                                var type = i;
                                var defaultScore = db.SATScores.Where(s => s.RawValue == 0 && s.TypeID == type).FirstOrDefault();
                                testSection.ScoreID = defaultScore.ID;
                                testSection.Section = i;
                                testSection.rowguid = beginTest.rowguid;
                                switch (i)
                                {
                                    case 1:
                                        testSection.StartTime = now;
                                        testSection.EndTime = now.AddMinutes(65);
                                        timeStep = testSection.EndTime;


                                        break;
                                    case 2:
                                        testSection.StartTime = timeStep;
                                        testSection.EndTime = timeStep.AddMinutes(35);
                                        timeStep = testSection.EndTime;
                                        break;
                                    case 3:
                                        testSection.StartTime = timeStep;
                                        testSection.EndTime = timeStep.AddMinutes(25);
                                        timeStep = testSection.EndTime;
                                        break;
                                    case 4:
                                        testSection.StartTime = timeStep;
                                        testSection.EndTime = timeStep.AddMinutes(55);
                                        timeStep = testSection.EndTime;
                                        break;
                                    default:
                                        break;
                                }
                                testSection.Status = false;
                                db.SATTestSections.Add(testSection);
                                db.SaveChanges();
                            }
                            Session["Section"] = section;
                            ////add pre define user answer
                            //foreach (var item in db.SATQuestions.ToList())
                            //{
                            //    var userAnswer = new SATUserAnswer();

                            //    userAnswer.rowguid = Guid.NewGuid();
                            //    userAnswer.TestID = currentTest.ID;
                            //    userAnswer.UserAnswer = null;
                            //    db.SATUserAnswers.Add(userAnswer);
                            //    db.SaveChanges();
                            //}
                            TempData["guidTest"] = beginTest.rowguid.ToString();
                            TempData.Keep("guidTest");
                            transaction.Commit();
                            return RedirectToAction("BeginTest", new { guid = beginTest.rowguid });
                            #endregion
                        }


                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ViewBag.Ex = ex.ToString();
                        return View();
                    }
                }



            }


        }


        public ActionResult BeginTest(Guid guid)
        {
            try
            {
                if (TempData["guidTest"] != null)
                {
                    string _guid = (string)TempData["guidTest"];
                    TempData.Keep("guidTest");
                }

                using (var db = new SATEntities())
                {
                    var email = Session["AccountEmail"].ToString();
                    var user = db.Users.Where(u => u.Email == email).FirstOrDefault();

                    var theTest = db.SATTests.Where(t => t.rowguid == guid && t.Status == false).FirstOrDefault();
                    var section = Session["Section"] == null ? 1 : int.Parse(Session["Section"].ToString());
                    var contentIdx = Session["ContentIndex"] == null ? 1 : int.Parse(Session["ContentIndex"].ToString());
                    var lastFormContent = db.SATExamForms.Where(f => f.Section == section && f.ExamCode == theTest.ExamCode && f.Status == true).FirstOrDefault();
                    if (lastFormContent != null)
                    {
                        var contentCount = lastFormContent.SATExamContents.Count;
                        Session["LastContent"] = contentCount;
                    }

                    var testForm = db.SATExamForms.Include("SATType").FirstOrDefault(f => f.Section == section && f.ExamCode == theTest.ExamCode && f.Status == true);
                    if (testForm != null)
                    {
                        var formContent = db.SATExamContents.FirstOrDefault(c => c.ExamContentIndex == contentIdx && c.Status == true && c.ExamFormID == testForm.ID);
                        if (formContent != null)
                        {
                            var contenLines = db.SATContentLines.Where(l => l.ExamContentID == formContent.ID).OrderBy(l => l.LineIndex).ToList();
                            ViewBag.Content = formContent;
                            ViewBag.Lines = contenLines;
                            var questions = db.SATQuestions.Include("SATAnswers").Where(q => q.ExamContentID == formContent.ID && q.Status == true).ToList();
                            ViewBag.Questions = questions;


                        }


                        //update status of test section
                        var sectionEntity = db.SATTestSections.FirstOrDefault(t => t.Section == section && t.rowguid == guid);
                        if (sectionEntity != null)
                        {
                            sectionEntity.Status = true;
                            db.Entry(sectionEntity).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }

                        ViewBag.TestForm = testForm;


                        var viewModel = new TestViewModel(theTest);

                        viewModel.TypeName = testForm.SATType.TypeName;
                        //get userAnswer
                        if (Session["ListAnswer"] != null)
                        {
                            viewModel.ListAnswer = (List<SATUserAnswer>)Session["ListAnswer"];
                            var listUA = viewModel.ListAnswer.Where(a => a != null).Select(a => a.UserAnswer).ToList();
                            if (ViewBag.Questions != null)
                            {
                                while (ViewBag.Questions.Count > listUA.Count)
                                {
                                    listUA.Add(0);

                                }
                                while (viewModel.ListAnswer.Count() < ViewBag.Questions.Count)
                                {
                                    viewModel.ListAnswer.Add(new SATUserAnswer());
                                }

                                ViewBag.UserAnswerId = listUA;
                            }
                        }
                        if (Session["AnsweredQuestion"] != null)
                        {
                            ViewBag.AnsweredQuestion = (List<int>)Session["AnsweredQuestion"];
                        }
                        return View(viewModel);
                    }

                }
                return View("Test");
            }
            catch (Exception ex)
            {
                ViewBag.Ex = ex.ToString();
                return View("Test");
            }
        }

        [HttpPost]
        public ActionResult BeginTest(TestViewModel viewModel, FormCollection collection, string submitBtnPress, string answeredQuestion)
        {

            using (var db = new SATEntities())
            {
                var theTest = db.SATTests.Find(viewModel.Test.ID);
                if (submitBtnPress == "prev" && Session["ContentIndex"] != null && int.Parse(Session["ContentIndex"].ToString()) > 1)
                {
                    Session["ContentIndex"] = int.Parse(Session["ContentIndex"].ToString()) - 1;
                    //if (Session["ListAnswer"] != null)
                    //{
                    //    Session["ListAnswer"] = viewModel.ListAnswer;
                    //}
                    return RedirectToAction("BeginTest", new { guid = theTest.rowguid });
                }
                var section = Session["Section"] == null ? 1 : int.Parse(Session["Section"].ToString());
                var contentIdx = Session["ContentIndex"] == null ? 1 : int.Parse(Session["ContentIndex"].ToString());
                var lastFormContent = 0;
                if (Session["LastContent"] != null)
                {
                    lastFormContent = Convert.ToInt32(Session["LastContent"].ToString());
                }
                else
                {
                    var entity = db.SATExamForms.Where(f => f.Section == section && f.ExamCode == theTest.ExamCode && f.Status == true).FirstOrDefault();
                    if (entity != null)
                    {
                        lastFormContent = entity.SATExamContents.Count;
                    }
                }

                //var lastSection = theTest.SATTestSections.Count;
                var lastSection = 1;
                var disabledSection = new List<int>() { };
                //save user answer
                if (viewModel.ListAnswer != null)
                {
                    Session["ListAnswer"] = viewModel.ListAnswer;
                    if (!string.IsNullOrEmpty(answeredQuestion))
                    {
                        List<int> qId = answeredQuestion.Split(',').Where(a => !string.IsNullOrEmpty(a)).Select(a => int.Parse(a)).ToList();
                        Session["AnsweredQuestion"] = qId;
                    }

                    foreach (var item in viewModel.ListAnswer)
                    {
                        var userAnswer = new SATUserAnswer();
                        userAnswer.UserAnswer = item.UserAnswer;
                        userAnswer.rowguid = Guid.NewGuid();
                        userAnswer.TestID = viewModel.Test.ID;
                        userAnswer.AnswerContent = item.AnswerContent;

                        db.SATUserAnswers.Add(userAnswer);
                        //update section score
                        if (!string.IsNullOrEmpty(item.AnswerContent))
                        {

                            var rightAnsw = db.SATAnswers.Any(a => a.AnswerContent == userAnswer.AnswerContent);
                            if (rightAnsw)
                            {
                                var testSection = db.SATTestSections.Where(s => s.rowguid == theTest.rowguid && s.Section == section).FirstOrDefault();
                                testSection.TotalRightAnswer++;
                                var sectionScore = db.SATScores.Where(s => s.RawValue == testSection.TotalRightAnswer && s.TypeID == testSection.SATExamForm.TypeID).FirstOrDefault();
                                testSection.ScoreID = sectionScore.ID;

                                db.Entry(testSection).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                        else
                        {
                            var isRightAnswer = db.SATAnswers.Any(a => a.IsRightAnswer == true && a.ID == userAnswer.UserAnswer);
                            if (isRightAnswer)
                            {
                                var testSection = db.SATTestSections.Where(s => s.rowguid == theTest.rowguid && s.Section == section).FirstOrDefault();
                                testSection.TotalRightAnswer++;
                                var sectionScore = db.SATScores.Where(s => s.RawValue == testSection.TotalRightAnswer && s.TypeID == testSection.SATExamForm.TypeID).FirstOrDefault();
                                testSection.ScoreID = sectionScore.ID;

                                db.Entry(testSection).State = System.Data.Entity.EntityState.Modified;
                            }
                        }

                        db.SaveChanges();
                    }
                }
                theTest.SubmitTime = DateTime.Now;
                int mathScore = 0;
                var otherScore = 0;
                var mathRightAnswer = 0;
                foreach (var item in theTest.SATTestSections)
                {
                    var sectionScore = db.SATScores.Find(item.ScoreID);
                    if (sectionScore.TypeID != 3 && sectionScore.TypeID != 4)
                    {
                        otherScore += (sectionScore.ScoreValue);
                    }
                    else
                    {
                        mathRightAnswer += item.TotalRightAnswer;
                    }
                }
                var _mathScoreEnt = db.SATScores.Where(s => s.RawValue == mathRightAnswer && s.TypeID == 3).FirstOrDefault();
                if (_mathScoreEnt != null)
                {
                    mathScore = _mathScoreEnt.ScoreValue;
                }
                theTest.TotalScore = otherScore + mathScore;
                db.Entry(theTest).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                if (contentIdx == lastFormContent || lastFormContent == 0)
                {
                    //section = section + 1;
                    Session["Section"] = section;
                    Session["ContentIndex"] = null;

                    if (section >= lastSection)
                    {
                        disabledSection.Add(section);
                        Session["DisabledSection"] = disabledSection;
                        Session["Section"] = null;
                        Session["DisabledInExam"] = theTest.ExamCode;

                        var disableSectionModel = new DisabledSection { Section = disabledSection, DisabledInExam = theTest.ExamCode };
                        if (Session["DisabledSectionModel"] != null)
                        {
                            var _sectionModel = Session["DisabledSectionModel"] as DisabledSection;
                            _sectionModel.Section.AddRange(disableSectionModel.Section);
                            Session["DisabledSectionModel"] = _sectionModel;
                        }
                        else
                        {
                            Session["DisabledSectionModel"] = disableSectionModel;

                        }
                        //Session["DisableSectionRender"] = Utils.Instance.RenderViewToString("~/Views/SATTest/_ChooseSection.cshtml", disableSectionModel);
                        return RedirectToAction("Test");
                    }
                }
                else
                {
                    contentIdx = contentIdx + 1;
                    Session["ContentIndex"] = contentIdx;
                }
                return RedirectToAction("BeginTest", new { guid = theTest.rowguid });
            }


        }
        public string GetAllSection(int examCode)
        {
            if (Session["DisabledSectionModel"] != null)
            {
                var model = Session["DisabledSectionModel"] as DisabledSection;
                if (examCode == model.DisabledInExam)
                {
                    return Utils.Instance.RenderViewToString("~/Views/SATTest/_ChooseSection.cshtml", model);
                }
                else
                {
                    var newModel = new DisabledSection();
                    //model.Section = new List<int>();
                    return Utils.Instance.RenderViewToString("~/Views/SATTest/_ChooseSection.cshtml", newModel);
                }
            }
            return Utils.Instance.RenderViewToString("~/Views/SATTest/_ChooseSection.cshtml", new DisabledSection());
        }


        public string GetRemainingTime(Guid guid)
        {
            var currentTime = DateTime.Now;
            var entity = db.SATTestSections.FirstOrDefault(t => t.Status == true && t.rowguid == guid);
            if (entity != null)
            {
                var remain = entity.EndTime - currentTime;
                return remain.TotalMinutes.ToString();
            }
            return string.Empty;
        }
        public int GetEndTime(Guid guid)
        {
            var currentTime = DateTime.Now;
            var entity = db.SATTestSections.FirstOrDefault(t => t.Status == true && t.rowguid == guid);
            if (entity != null)
            {

                return entity.SATExamForm.Duration * 60 * 1000;
            }
            return 0;
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
