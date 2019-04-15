using IEE.Infrastructure;
using IEE.Infrastructure.DbContext;
using IEE.Web.Models;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    public class MediaController : BaseController
    {
        private readonly IRepository<Medium> _mediaRepo;
        public MediaController()
        {
            var unitOfWork = new UnitOfWork();
            _mediaRepo = unitOfWork.GetRepository<Medium>();
        }

        // GET: ttn_content/Media
        public ActionResult Index()
        {
            var medias = _mediaRepo.GetAll().OrderByDescending(t => t.CreatedDate);
            ViewBag.Medias = medias;
            var listSource = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("SAT","SAT"),
                new KeyValuePair<string, string>("IELTS","IELTS"),
                //new KeyValuePair<string, string>("SAT","SAT"),
            };
            ViewBag.ListSource = new SelectList(listSource, "Key", "Value");
            return View();
        }

        [HttpPost]
        public ActionResult Index(Medium model, HttpPostedFileBase file, string submit)
        {
            var listSource = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("SAT","SAT"),
                new KeyValuePair<string, string>("IELTS","IELTS"),
                //new KeyValuePair<string, string>("SAT","SAT"),
            };

            ViewBag.ListSource = new SelectList(listSource, "Key", "Value");
            var medias = _mediaRepo.GetAll().OrderByDescending(t => t.CreatedDate);
            if (ModelState.IsValidField("DisplayName"))
            {
                string file_name = string.Empty, file_src = string.Empty;
                if (submit == "Hủy")
                {
                    return RedirectToAction("Index");

                }
                else if (submit == "Thêm")
                {
                    //for (int i = 0; i < Request.Files.Count; i++)
                    //{
                    var _file = Request.Files[0];

                    if (_file != null && _file.ContentLength > 0)
                    {
                        if (_file.ContentLength > 2000000)
                        {
                            ModelState.AddModelError("DisplayName", "Tên tài liệu không được để trống");
                            ViewBag.ErrorMessage = "Dung lượng file quá giới hạn";

                            ViewBag.Medias = medias;
                            return View(model);
                        }
                        var fileName = Path.GetFileName(_file.FileName);
                        file_name = fileName.Replace(Path.GetExtension(fileName), string.Empty) + DateTime.Now.Ticks + Path.GetExtension(fileName);
                        if (_file.ContentType.Contains("image"))
                        {
                            model.Type = 1;
                            file_src = Path.Combine(Server.MapPath("~/photo/post/"), file_name);
                            model.Name = file_name;
                            model.Link = "/photo/post/" + file_name;
                        }
                        else
                        {
                            model.Type = 2;
                            file_src = Path.Combine(Utils.Instance.CreateFolderIfNeeded(Server.MapPath("~/Uploads/")), file_name);
                            model.Name = file_name;
                            model.Link = "/Uploads/" + file_name;
                        }


                        try
                        {
                            _file.SaveAs(file_src);
                        }
                        catch (Exception)
                        {

                            ModelState.AddModelError("SavePath", "Không thể lưu file!");
                            ViewBag.ErrorMessage = "Không thể lưu file!";

                            ViewBag.Medias = medias;
                            return View(model);
                        }
                        //break;
                    }
                    //}

                    model.IsDeleted = false;
                    model.CreatedBy = User.UserId;
                    model.CreatedDate = DateTime.Now;
                    _mediaRepo.InsertAndSubmit(model);
                }
                else
                {
                    var entity = _mediaRepo.GetById(model.Id);
                    var _file = Request.Files[0];
                    if (_file != null && _file.ContentLength > 0)
                    {
                        if (_file.ContentLength > 2000000)
                        {
                            ModelState.AddModelError("DisplayName", "Tên tài liệu không được để trống");
                            ViewBag.ErrorMessage = "Dung lượng file quá giới hạn";

                            ViewBag.Medias = medias;
                            return View(model);
                        }
                        var fileName = Path.GetFileName(_file.FileName);
                        file_name = fileName.Replace(Path.GetExtension(fileName), string.Empty) + DateTime.Now.Ticks + Path.GetExtension(fileName);
                        if (_file.ContentType.Contains("image"))
                        {
                            model.Type = 1;
                            file_src = Path.Combine(Server.MapPath("~/photo/post/"), file_name);
                            model.Name = file_name;
                            model.Link = "/photo/post/" + file_name;
                        }
                        else
                        {
                            model.Type = 2;
                            file_src = Path.Combine(Utils.Instance.CreateFolderIfNeeded(Server.MapPath("~/Uploads/")), file_name);
                            model.Name = file_name;
                            model.Link = "/Uploads/" + file_name;
                        }


                        try
                        {
                            _file.SaveAs(file_src);
                        }
                        catch (Exception)
                        {

                            ModelState.AddModelError("SavePath", "Không thể lưu file!");
                            ViewBag.ErrorMessage = "Không thể lưu file!";

                            ViewBag.Medias = medias;
                            return View(model);
                        }

                        entity = model.MapUpdateWithStatus<Medium>(User.UserId, entity);

                        entity.ModifiedDate = model.ModifiedDate = DateTime.Now;
                        entity.ModifiedBy = User.UserId;
                        _mediaRepo.Update(entity);
                        _mediaRepo.SaveChanges();

                    }
                    else
                    {

                        entity.DisplayName = model.DisplayName;
                        entity.FileSource = model.FileSource;
                        //entity.Type = model.Type;
                        entity.ModifiedDate = model.ModifiedDate = DateTime.Now;
                        entity.ModifiedBy = User.UserId;
                        _mediaRepo.Update(entity);
                        _mediaRepo.SaveChanges();
                    }
                }



                //var mediaModel = new IEE.Infrastructure.Media();
                //mediaModel.Name = model.Name;
                //mediaModel.Link = model.Link;
                //mediaModel.IsDeleted = model.IsDeleted;
                //mediaModel.CreatedBy = model.CreatedBy;
                //mediaModel.CreatedDate = model.CreatedDate;

                //using (var db= new IEE.Infrastructure.nhieevwp_webEntities())
                //{
                //    db.Media.Add(mediaModel);
                //    db.SaveChanges();
                //}



                return RedirectToAction("Index");
            }
            ModelState.AddModelError("DisplayName", "Tên tài liệu không được để trống");
            ViewBag.ErrorMessage = "Tên tài liệu không được để trống";

            ViewBag.Medias = medias;
            return View(model);
        }

        public ActionResult ListMedia()
        {
            var model = _mediaRepo.GetAll().OrderByDescending(t => t.CreatedDate).ToList();
            return View("~/Areas/ttn_content/Views/Media/ListMedia.cshtml", model);
        }

        [HttpDelete]
        public JsonResult Delete(int id)
        {
            try
            {

                var entity = _mediaRepo.GetById(id);
                var filePath = entity.Link;
                _mediaRepo.Delete(entity);
                _mediaRepo.SaveChanges();

                FileInfo fileInfo = new FileInfo(HostingEnvironment.MapPath(filePath));
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }

        }

        public async Task<JsonResult> GetMedia(int id)
        {
            var fileTypes = new List<string>() { ".jpg", ".jpeg", ".png" };
            var media = _mediaRepo.GetById(id);
            if (fileTypes.IndexOf(Path.GetExtension(media.Name)) > -1)
            {
                using (Image image = Image.FromFile(HostingEnvironment.MapPath(media.Link)))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        string base64String = Convert.ToBase64String(imageBytes);
                        var dataUrl = "data:image/jpeg;base64," + base64String;
                        return Json(new { media, dataUrl }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            else
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var file = new FileInfo(HostingEnvironment.MapPath(media.Link));
                if (file.Exists)
                {
                    var roundSize = decimal.Round(file.Length / 1024);
                    List<string> content = new List<string>();

                    if (file.Extension.Contains("doc"))
                    {
                        var docApp = new Application();
                        var doc = await System.Threading.Tasks.Task.Run(() => docApp.Documents.Open(file.FullName));

                        String read = string.Empty;
                        foreach (Paragraph objParagraph in doc.Paragraphs)
                        {
                            await System.Threading.Tasks.Task.Run(() => content.Add(objParagraph.Range.Text.Trim() + "</br>"));
                        }

                        doc.Close();
                        docApp.Quit();
                        stopwatch.Stop();
                        return Json(new { media, roundSize, docContent = content, sw = stopwatch.ElapsedMilliseconds }, JsonRequestBehavior.AllowGet);
                    }
                    var text = System.IO.File.ReadAllText(file.FullName);
                    text = text.Replace(System.Environment.NewLine, "</br>");
                    content.Add(text);

                    return Json(new { media, roundSize, content }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { media }, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult Download(int file)
        {
            var media = _mediaRepo.GetById(file);
            if (media != null)
            {
                var path = Path.GetFullPath(HostingEnvironment.MapPath(media.Link));
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                    string fileName = media.DisplayName+fileInfo.Extension;
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
               
            }
            return RedirectToRoute("HomePage");

        }

    }
}