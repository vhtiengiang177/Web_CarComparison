﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CarComparison.Areas.Admin.Models;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace CarComparison.Areas.Admin.Controllers
{
    // Chức năng: Quản lý bài viết
    [AuthorizeController]
    public class ArticlesController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();

        // GET: Admin/Articles
        public ActionResult Index(string searchname)
        {
            var articles = db.Articles.Include(a => a.CategoryArticle).Include(a => a.User_);
            if (!String.IsNullOrEmpty(searchname))
            {
                articles = (from a in db.Articles where a.CategoryArticle.name_category.Contains(searchname) || a.title_article.Contains(searchname) select a);
            }
            return View(articles.ToList());
        }

        // GET: Admin/Articles/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // GET: Admin/Articles/Create
        public ActionResult Create()
        {
            var articles = (from c in db.Articles select c.id_article).ToList();
            string id = "";
            if (articles.Count == 0) // nếu danh sách rỗng
            {
                id = "Ar01";
            }
            else
            {
                for (int i = 0; i < articles.Count(); i++)
                {
                    if (int.Parse(articles[i].Substring(2, 2)) != (i + 1))
                    {
                        if (i + 1 >= 0 && i + 1 < 9)
                            id = "Ar0" + (i + 1).ToString();
                        else if (i + 1 > 9)
                            id = "Ar" + (i + 1).ToString();
                        break;
                    }
                }
                if (id == "")
                {
                    id = articles[articles.Count - 1].Substring(2, 2);
                    if (int.Parse(id) >= 0 && int.Parse(id) < 9)
                    {
                        id = "Ar0" + (int.Parse(id) + 1).ToString();
                    }
                    else if (int.Parse(id) >= 9)
                    {
                        id = "Ar" + (int.Parse(id) + 1).ToString();
                    }
                }
            }
            Article art = new Article
            {
                id_article = id
            };
            ViewBag.id_category = new SelectList(db.CategoryArticles, "id_category", "name_category");
            ViewBag.id_user = new SelectList(db.User_, "id_user", "name_user");
            return View(art);
        }

        // POST: Admin/Articles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_article,id_category,title_article,description_article,img_article,linkvideo_article,imageFile")] Article article)
        {
            string value = Request["action"]; // Lấy nút submit
            if ((article.id_category == "CaAr01" && article.linkvideo_article == "") || article.title_article == "" || article.description_article == "" || article.title_article == null)
            {
                if (article.linkvideo_article == "")
                {
                    ModelState.AddModelError("linkError", "Không để trống link video");
                }
                if (article.title_article == "" || article.title_article == null)
                {
                    ModelState.AddModelError("titleError", "Không để trống tiêu đề");
                }
                if (article.description_article == "")
                {
                    ModelState.AddModelError("desError", "Không để trống nội dung");
                }
                ViewBag.id_category = new SelectList(db.CategoryArticles, "id_category", "name_category", article.id_category);
                return View(article);
            }
            if (article.linkvideo_article != "" && article.linkvideo_article != null)
            {
                var uri = new Uri(article.linkvideo_article);
                if (uri.Host != "www.youtube.com" && uri.Host != "https://youtu.be/")
                {
                    ModelState.AddModelError("linkError", "Lưu ý: Chỉ nhận đường dẫn youtube!");
                    article.id_category = "CaAr01";
                    ViewBag.id_category = new SelectList(db.CategoryArticles, "id_category", "name_category", article.id_category);
                    return View(article);
                }
                var query = HttpUtility.ParseQueryString(uri.Query);

                var videoId = string.Empty;

                if (query.AllKeys.Contains("v"))
                {
                    videoId = query["v"];
                }
                else
                {
                    videoId = uri.Segments.Last();
                }
                string link_new = @"//www.youtube.com/embed/" + videoId;
                article.linkvideo_article = link_new;
            }
            var user = Session["user"] as User_;
            article.id_user = user.id_user;
            if (value == "Lưu nháp")
            {
                article.state_article = "0"; // Trạng thái bằng không, chưa đc hiển thị trên web
                article.time_write = DateTime.Now;
                article.view_article = 0;
            }
            if (value == "Đăng bài")
            {
                article.state_article = "1"; // Trạng thái bằng 1, được hiển thị trên web
                article.time_write = DateTime.Now;
                article.time_pulish_article = DateTime.Now;
                article.view_article = 0;
            }
            string removeUnicode = RemoveUnicode(article.title_article);
            string cleanString = removeUnicode.ToLower().Replace(" ", "-"); // ToLower() on the string thenreplaces spaces with hyphens
            cleanString = Regex.Replace(cleanString, @"[^a-zA-Z0-9\/_|+ -]", ""); // removes all non-alphanumerics/underscore/hyphens
            article.alias_article = cleanString;

            string fileName = Path.GetFileNameWithoutExtension(article.imageFile.FileName);
            string extension = Path.GetExtension(article.imageFile.FileName);
            article.img_article = "/Asset/Image/Article/" + article.id_article + extension;
            fileName = Path.Combine(Server.MapPath("/Asset/Image/Article/"), article.id_article + extension);
            article.imageFile.SaveAs(fileName);

            db.Articles.Add(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
    "đ",
    "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
    "í","ì","ỉ","ĩ","ị",
    "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
    "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
    "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
    "d",
    "e","e","e","e","e","e","e","e","e","e","e",
    "i","i","i","i","i",
    "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
    "u","u","u","u","u","u","u","u","u","u","u",
    "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

        // GET: Admin/Articles/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_category = new SelectList(db.CategoryArticles, "id_category", "name_category", article.id_category);
            ViewBag.id_user = new SelectList(db.User_, "id_user", "name_user", article.id_user);
            return View(article);
        }

        // POST: Admin/Articles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "id_article,id_category,title_article,description_article,img_article,linkvideo_article,imageFile")] Article article)
        {
            string value = Request["action"]; // Lấy nút submit
            if ((article.id_category == "CaAr01" && article.linkvideo_article == "") || article.title_article == "" || article.description_article == "")
            {
                if (article.linkvideo_article == "")
                {
                    ModelState.AddModelError("linkError", "Không để trống link video");
                }
                if (article.title_article == "")
                {
                    ModelState.AddModelError("titleError", "Không để trống tiêu đề");
                }
                if (article.description_article == "")
                {
                    ModelState.AddModelError("desError", "Không để trống nội dung");
                }
                //ViewBag.id_category = new SelectList(db.CategoryArticles, "id_category", "name_category", article.id_category);
                //return View(article);
            }
            if (ModelState.IsValid)
            {
                Article article_old = db.Articles.AsNoTracking().Where(c => c.id_article == article.id_article).SingleOrDefault(); // tránh xung đột khi đang sửa Ar tại id 1
                if (article.linkvideo_article != "" && article.linkvideo_article != null)
                {
                    var uri = new Uri(article.linkvideo_article);
                    if (uri.Host != "www.youtube.com")
                    {
                        ModelState.AddModelError("linkError", "Lưu ý: Chỉ nhận đường dẫn youtube!");
                        ViewBag.id_category = new SelectList(db.CategoryArticles, "id_category", "name_category", article.id_category);
                        return View(article);
                    }
                    var query = HttpUtility.ParseQueryString(uri.Query);

                    var videoId = string.Empty;

                    if (query.AllKeys.Contains("v"))
                    {
                        videoId = query["v"];
                    }
                    else
                    {
                        videoId = uri.Segments.Last();
                    }
                    string link_new = @"//www.youtube.com/embed/" + videoId;
                    article.linkvideo_article = link_new;
                }
                article.id_user = article_old.id_user;
                if (value == "Cập nhật")
                {
                    article.state_article = "1"; // Trạng thái bằng 1, đc hiển thị trên web
                    article.time_pulish_article = DateTime.Now;
                }
                if (value == "Chuyển sang nháp")
                {
                    article.state_article = "0"; // Trạng thái bằng 0, k được hiển thị trên web
                    article.time_pulish_article = article_old.time_pulish_article;
                }
                if (value == "Lưu nháp")
                {
                    article.state_article = "0"; // Trạng thái bằng không, chưa đc hiển thị trên web
                }
                if (value == "Đăng bài")
                {
                    article.state_article = "1"; // Trạng thái bằng 1, được hiển thị trên web
                    article.time_pulish_article = DateTime.Now;
                }
                article.time_write = DateTime.Now;
                article.view_article = article_old.view_article;
                string removeUnicode = RemoveUnicode(article.title_article);
                string cleanString = removeUnicode.ToLower().Replace(" ", "-"); // ToLower() on the string thenreplaces spaces with hyphens
                cleanString = Regex.Replace(cleanString, @"[^a-zA-Z0-9\/_|+ -]", ""); // removes all non-alphanumerics/underscore/hyphens
                article.alias_article = cleanString;

                if (article.imageFile != null)
                {
                    if (article.img_article != null && article.img_article != "")
                    {
                        string fullpath = Server.MapPath(article.img_article);
                        FileInfo fi = new FileInfo(fullpath);
                        if (fi != null)
                        {
                            System.IO.File.Delete(fullpath);
                            fi.Delete();
                        }
                    }
                    string fileName = Path.GetFileNameWithoutExtension(article.imageFile.FileName);
                    string extension = Path.GetExtension(article.imageFile.FileName);
                    article.img_article = "/Asset/Image/Article/" + article.id_article + extension;
                    fileName = Path.Combine(Server.MapPath("/Asset/Image/Article/"), article.id_article + extension);
                    article.imageFile.SaveAs(fileName);
                }

                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_category = new SelectList(db.CategoryArticles, "id_category", "name_category", article.id_category);
            ViewBag.id_user = new SelectList(db.User_, "id_user", "name_user", article.id_user);
            return View(article);
        }

        // GET: Admin/Articles/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Admin/Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Article article = db.Articles.Find(id);
            var lstCom = (from c in db.Comments where c.id_article == id select c).ToList();
            for (int i = 0; i < lstCom.Count; i++)
            {
                db.Comments.Remove(lstCom[i]);
                db.SaveChanges();
            }
            // Xóa hình của bài viết
            if (article.img_article != null && article.img_article != "")
            {
                string fullpath = Server.MapPath(article.img_article);
                FileInfo fi = new FileInfo(fullpath);
                if (fi != null)
                {
                    System.IO.File.Delete(fullpath);
                    fi.Delete();
                }
            }
            db.Articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Publish(FormCollection f)
        {
            string id = f["id_art"];
            Article art = db.Articles.Find(id);
            art.state_article = "1";

            db.Entry(art).State = EntityState.Modified;
            db.SaveChanges();
            return View("Index", "Articles");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(HttpPostedFileBase file)
        {
            string filePath = string.Empty;
            if (file != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                file.SaveAs(filePath);

                string conString = string.Empty;

                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = "Provider=Microsoft.ACE.OLEDB.16.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                conString = @"Server=localhost;Database=CompareCar;Trusted_Connection=True;";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.Article";

                        // Map the Excel columns with that of the database table, this is optional but good if you do
                        // 
                        sqlBulkCopy.ColumnMappings.Add("id_article", "id_article");
                        sqlBulkCopy.ColumnMappings.Add("id_category", "id_category");
                        sqlBulkCopy.ColumnMappings.Add("title_article", "title_article");
                        sqlBulkCopy.ColumnMappings.Add("alias_article", "alias_article");
                        sqlBulkCopy.ColumnMappings.Add("id_user", "id_user");
                        sqlBulkCopy.ColumnMappings.Add("description_article", "description_article");
                        sqlBulkCopy.ColumnMappings.Add("time_pulish_article", "time_pulish_article");
                        sqlBulkCopy.ColumnMappings.Add("time_write", "time_write");

                        sqlBulkCopy.ColumnMappings.Add("state_article", "state_article");
                        sqlBulkCopy.ColumnMappings.Add("linkvideo_article", "linkvideo_article");
                        sqlBulkCopy.ColumnMappings.Add("img_article", "img_article");
                        sqlBulkCopy.ColumnMappings.Add("view_article", "view_article");

                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
            }
            //if the code reach here means everthing goes fine and excel data is imported into database
            ViewBag.Success = "File Imported and excel data saved into database";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index2(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                        break;
                }

                DataTable dtEmployee = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dtEmployee);
                            connExcel.Close();
                        }
                    }
                }

                conString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.Article";

                        //[OPTIONAL]: Map the Excel columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("id_article", "id_article");
                        sqlBulkCopy.ColumnMappings.Add("id_category", "id_category");
                        sqlBulkCopy.ColumnMappings.Add("title_article", "title_article");
                        sqlBulkCopy.ColumnMappings.Add("alias_article", "alias_article");
                        sqlBulkCopy.ColumnMappings.Add("id_user", "id_user");
                        sqlBulkCopy.ColumnMappings.Add("description_article", "description_article");
                        sqlBulkCopy.ColumnMappings.Add("time_pulish_article", "time_pulish_article");
                        sqlBulkCopy.ColumnMappings.Add("time_write", "time_write");
                        sqlBulkCopy.ColumnMappings.Add("state_article", "state_article");
                        sqlBulkCopy.ColumnMappings.Add("linkvideo_article", "linkvideo_article");
                        sqlBulkCopy.ColumnMappings.Add("img_article", "img_article");
                        sqlBulkCopy.ColumnMappings.Add("view_article", "view_article");

                        con.Open();
                        sqlBulkCopy.WriteToServer(dtEmployee);
                        con.Close();
                    }
                }
            }
            ViewBag.Success = "File Imported and excel data saved into database";

            return View();
        }




    }
}
