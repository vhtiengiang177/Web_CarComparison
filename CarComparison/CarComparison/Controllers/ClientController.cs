using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarComparison.Areas.Admin.Models;
using Version = CarComparison.Areas.Admin.Models.Version;
using PagedList;

namespace CarComparison.Controllers
{
    // Chức năng: Mọi thứ mà bất kỳ ai được truy cập vào
    public class ClientController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();
        
        cascadingmodel mol = new cascadingmodel();
        Article_Comment ac = new Article_Comment();



        // GET: Client
        public ActionResult Index()
        {
            //Lần lượt tạo các viewbag để lấy các list từ csdl


            //List video mới nhất
            var lstNewVideo = db.Articles.Where(n => n.id_category == "CaAr01").Where(a => a.state_article == "1");
            //Gán vào ViewBag
            ViewBag.ListNewVideo = lstNewVideo;


            //List bài đánh giá mới nhất
            var lstNewBlog = db.Articles.Where(n => n.id_category == "CaAr02").Where(a => a.state_article == "1");
            //Gán vào ViewBag
            ViewBag.ListNewBlog = lstNewBlog;


            //List bài so sánh mới nhất
            var lstNewArcCompare = db.Articles.Where(n => n.id_category == "CaAr03").Where(a => a.state_article == "1");
            //Gán vào ViewBag
            ViewBag.lstNewArcCompare = lstNewArcCompare;



            return View();
        }

        //Truyền dữ liệu xe vào dropdownlist
        public ActionResult Comparing()
        {
            cascadingmodel mol = new cascadingmodel();
            List<Automaker> AutoList = db.Automakers.ToList();
            ViewBag.AutoList = new SelectList(AutoList, "id_automaker", "name_automaker");
            return View();


        }

        public JsonResult GetModelList(string idAuto)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Model> ModelList = db.Models.Where(x => x.id_automaker == idAuto).ToList();
            return Json(ModelList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetVersionList(string idModel)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Version> VersionList = db.Versions.Where(x => x.id_model == idModel).ToList();
            return Json(VersionList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCarList(string idVersion)
        {
           
            db.Configuration.ProxyCreationEnabled = false;
            List<Car> CarList = db.Cars.Where(x => x.id_version == idVersion).ToList();

           
            return Json(CarList, JsonRequestBehavior.AllowGet);

        }


        //Trang bài đánh giá
        public ActionResult Review(int? page, string key)
        {
            //3 bài viết gợi ý
            var lstnew = (from c in db.Articles where c.id_category == "CaAr02" && c.state_article == "1" orderby c.time_pulish_article descending select c).ToList();
            var lstnew3 = new List<Article>();
            if (lstnew.Count() > 3)
            {
                lstnew3.Add(lstnew[0]);
                lstnew3.Add(lstnew[1]);
                lstnew3.Add(lstnew[2]);
                ViewBag.lstnew3 = lstnew3;
            }
            else if (lstnew.Count() == 2)
            {
                lstnew3.Add(lstnew[0]);
                lstnew3.Add(lstnew[1]);
                ViewBag.lstnew3 = lstnew3;
            }
            else if (lstnew.Count() == 1)
            {
                lstnew3.Add(lstnew[0]);
                ViewBag.lstnew3 = lstnew3;
            }
            else if (lstnew.Count() == 0)
            {
                ViewBag.lstnew3 = null;
            }

            //Toàn bộ bài viết
            var lstBlog = db.Articles.Where(n => n.id_category == "CaAr02" && n.state_article == "1");
            //Gán vào ViewBag
            ViewBag.ListBlog= lstBlog;

            //Thực hiện chức năng phân trang
            //Tạo biến số bài viết trên trang
            int PageSize = 4;
            //Tạo biến thứ 2: Số trang hiện tại
            int PageNumber = (page ?? 1); // gán bằng 1

            //Tìm kiếm theo tên bài viết
            if (key!=null)
            {
                var lstsearchBlog = db.Articles.Where(n => n.title_article.Contains(key) && n.id_category == "CaAr02"); //contains tìm kiếm gần đúng
                return View(lstsearchBlog.OrderBy(n => n.id_article).ToPagedList(PageNumber, PageSize));
            }    
            

            return View(lstBlog.OrderBy(n=>n.id_article).ToPagedList(PageNumber,PageSize));
        }

        //Chi tiết bài viết
        public ActionResult DetailBlog(string id)
        {
            //3 bài viết gợi ý
            var lstnew = (from c in db.Articles where c.id_category != "CaAr01" && c.state_article == "1" orderby c.time_pulish_article descending select c).ToList();

            var lstnew3 = new List<Article>();
            if(lstnew.Count() > 3)
            {
                lstnew3.Add(lstnew[0]);
                lstnew3.Add(lstnew[1]);
                lstnew3.Add(lstnew[2]);
                ViewBag.lstnew3 = lstnew3;
            }
            else if(lstnew.Count() == 2)
            {
                lstnew3.Add(lstnew[0]);
                lstnew3.Add(lstnew[1]);
                ViewBag.lstnew3 = lstnew3;
            }
            else if(lstnew.Count() == 1)
            {
                lstnew3.Add(lstnew[0]);
                ViewBag.lstnew3 = lstnew3;
            }
            else if(lstnew.Count() == 0)
            {
                ViewBag.lstnew3 = null;
            }

            //Kiểm tra tham số truyền vào
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article_Comment arc = new Article_Comment();


            arc.Article= (from c in db.Articles where c.id_article == id && c.id_category != "CaAr01"  select c).ToList();
            arc.Comments= (from c in db.Comments where c.id_article == id select c).ToList();
            ////Nếu không thì truy xuất csdl lấy ra bài viết tương ứng
            //var art = (from c in arc.Article where c.id_article == id select c).ToList();
            //ViewBag.art = art;
            if (arc== null)
            {
                //Thông báo không có bài viết đó
                return HttpNotFound();
            }
            else
            {
                Article art = db.Articles.Find(id);
                art.view_article += 1;
                db.Entry(art).State = EntityState.Modified;
                db.SaveChanges();
            }
            return View(arc);
            
        }

        //Trang video
        public ActionResult Video(int? page, string key)
        {
            //3 video mới nhất gợi ý
            var lstnew = (from c in db.Articles where c.id_category == "CaAr01" && c.state_article == "1" orderby c.time_pulish_article descending select c).ToList();
            var lstnew3video = new List<Article>();
            if (lstnew.Count() > 3)
            {
                lstnew3video.Add(lstnew[0]);
                lstnew3video.Add(lstnew[1]);
                lstnew3video.Add(lstnew[2]);
                ViewBag.lstnew3video = lstnew3video;
            }
            else if (lstnew.Count() == 2)
            {
                lstnew3video.Add(lstnew[0]);
                lstnew3video.Add(lstnew[1]);
                ViewBag.lstnew3video = lstnew3video;
            }
            else if (lstnew.Count() == 1)
            {
                lstnew3video.Add(lstnew[0]);
                ViewBag.lstnew3video = lstnew3video;
            }
            else if (lstnew.Count() == 0)
            {
                ViewBag.lstnew3video = null;
            }

            //video lớn ở đầu trang video
            var videofirst = new List<Article>();
            videofirst.Add(lstnew[0]);
            ViewBag.videofirst = videofirst;
            //Danh sách video có trạng thái đã xuất bản
            var lstNewVideo = db.Articles.Where(n => n.id_category == "CaAr01").Where(a => a.state_article == "1"); 
            //Gán vào ViewBag
            ViewBag.ListNewVideo = lstNewVideo;

            //Thực hiện chức năng phân trang
            //Tạo biến số bài viết trên trang
            int PageSize = 4;
            //Tạo biến thứ 2: Số trang hiện tại
            int PageNumber = (page ?? 1); // gán bằng 1

            //Tìm kiếm theo tên video
            if (key != null)
            {
                var lstsearchVideo = db.Articles.Where(n => n.title_article.Contains(key) && n.id_category == "CaAr01" && n.state_article == "1"); //contains tìm kiếm gần đúng
                return View(lstsearchVideo.OrderBy(n => n.id_article).ToPagedList(PageNumber, PageSize));
            }


            return View(lstNewVideo.OrderBy(n => n.id_article).ToPagedList(PageNumber, PageSize));
        }

        //Chi tiết video
        public ActionResult DetailVideo(string id)
        {
            //3 video gợi ý
            var lstnew = (from c in db.Articles where c.id_category == "CaAr01" && c.state_article == "1" orderby c.time_pulish_article descending select c).ToList();
            var lstnew3video = new List<Article>();
            if (lstnew.Count() > 3)
            {
                lstnew3video.Add(lstnew[0]);
                lstnew3video.Add(lstnew[1]);
                lstnew3video.Add(lstnew[2]);
                ViewBag.lstnew3video = lstnew3video;
            }
            else if (lstnew.Count() == 2)
            {
                lstnew3video.Add(lstnew[0]);
                lstnew3video.Add(lstnew[1]);
                ViewBag.lstnew3video = lstnew3video;
            }
            else if (lstnew.Count() == 1)
            {
                lstnew3video.Add(lstnew[0]);
                ViewBag.lstnew3video = lstnew3video;
            }
            else if (lstnew.Count() == 0)
            {
                ViewBag.lstnew3video = null;
            }
            //Kiểm tra tham số truyền vào
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Article_Comment arc = new Article_Comment();


            arc.Article = (from c in db.Articles where c.id_article == id && c.id_category== "CaAr01" && c.state_article == "1" select c).ToList();
            arc.Comments = (from c in db.Comments where c.id_article == id select c).ToList();
            ////Nếu không thì truy xuất csdl lấy ra bài viết tương ứng
            //var art = (from c in arc.Article where c.id_article == id select c).ToList();
            //ViewBag.art = art;
            if (arc == null)
            {
                //Thông báo không có bài viết đó
                return HttpNotFound();
            }
            else
            {
                Article art = db.Articles.Find(id);
                art.view_article += 1;
                db.Entry(art).State = EntityState.Modified;
                db.SaveChanges();
            }
            return View(arc);
        }

        //Trang bài so sánh
        public ActionResult ArcCompare(int? page, string key)
        {
            //3 bài viết gợi ý
            var lstnew = (from c in db.Articles where c.id_category == "CaAr03" && c.state_article == "1" orderby c.time_pulish_article descending select c).ToList();
            var lstnew3 = new List<Article>();
            if (lstnew.Count() > 3)
            {
                lstnew3.Add(lstnew[0]);
                lstnew3.Add(lstnew[1]);
                lstnew3.Add(lstnew[2]);
                ViewBag.lstnew3 = lstnew3;
            }
            else if (lstnew.Count() == 2)
            {
                lstnew3.Add(lstnew[0]);
                lstnew3.Add(lstnew[1]);
                ViewBag.lstnew3 = lstnew3;
            }
            else if (lstnew.Count() == 1)
            {
                lstnew3.Add(lstnew[0]);
                ViewBag.lstnew3 = lstnew3;
            }
            else if (lstnew.Count() == 0)
            {
                ViewBag.lstnew3 = null;
            }

            //Toàn bộ bài viết
            var lstBlog = db.Articles.Where(n => n.id_category == "CaAr03" && n.state_article == "1");
            //Gán vào ViewBag
            ViewBag.ListBlog = lstBlog;

            //Thực hiện chức năng phân trang
            //Tạo biến số bài viết trên trang
            int PageSize = 4;
            //Tạo biến thứ 2: Số trang hiện tại
            int PageNumber = (page ?? 1); // gán bằng 1

            //Tìm kiếm theo tên bài viết
            if (key != null)
            {
                var lstsearchBlog = db.Articles.Where(n => n.title_article.Contains(key) && n.id_category == "CaAr03"); //contains tìm kiếm gần đúng
                return View(lstsearchBlog.OrderBy(n => n.id_article).ToPagedList(PageNumber, PageSize));
            }


            return View(lstBlog.OrderBy(n => n.id_article).ToPagedList(PageNumber, PageSize));
        }


        [ChildActionOnly] //để người dùng không get được view này
        public ActionResult blogPartial()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult videoPartial()
        {
            return PartialView();
        }

        //Trang team
        public ActionResult Team()
        {
            return View();
        }


        //TRang contact
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(FormCollection f)
        {
            string name = f["name"];
            string email = f["email"];
            string subject = f["message"];
            // Sinh id tự động
            var contact = (from c in db.Contacts select c.id_contact).ToList();
            string id = "";
            if (contact.Count == 0) // nếu danh sách rỗng
            {
                id = "Co01";
            }
            else
            {
                for (int i = 0; i < contact.Count(); i++)
                {
                    if (int.Parse(contact[i].Substring(2, 2)) != (i + 1))
                    {
                        if (i + 1 >= 0 && i + 1 < 9)
                            id = "Co0" + (i + 1).ToString();
                        else if (i + 1 > 9)
                            id = "Co" + (i + 1).ToString();
                        break;
                    }
                }
                if (id == "")
                {
                    id = contact[contact.Count - 1].Substring(2, 2);
                    if (int.Parse(id) >= 0 && int.Parse(id) < 9)
                    {
                        id = "Co0" + (int.Parse(id) + 1).ToString();
                    }
                    else if (int.Parse(id) >= 9)
                    {
                        id = "Co" + (int.Parse(id) + 1).ToString();
                    }
                }
            }
            Contact con = new Contact();
            con.id_contact = id;
            con.name_contact = name;
            con.email_contact = email;
            con.subject_contact = subject;
            con.state_contact = "0"; // Chưa đọc
            con.date_contact = DateTime.Now;

            db.Contacts.Add(con);
            db.SaveChanges();
            Response.Write("<script>alert('Gửi thành công!');</script>");
            return View();
        }

        public ActionResult Comment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DetailVideo(FormCollection f, string id_art = "")
        {
            id_art = f["id_art"];
            string text = f["comment"];
            User_ us = Session["user"] as User_;

            // Sinh id
            var com = (from c in db.Comments select c.id_comment).ToList();
            string id = "";
            if (com.Count == 0) // nếu danh sách rỗng
            {
                id = "Cm001";
            }
            else
            {
                for (int i = 0; i < com.Count(); i++)
                {
                    if (int.Parse(com[i].Substring(2, 3)) != (i + 1))
                    {
                        if (i + 1 >= 0 && i + 1 < 9)
                            id = "Cm00" + (i + 1).ToString();
                        else if (i + 1 > 9)
                            id = "Cm0" + (i + 1).ToString();
                        break;
                    }
                }
                if (id == "")
                {
                    id = com[com.Count - 1].Substring(2, 3);
                    if (int.Parse(id) >= 0 && int.Parse(id) < 9)
                    {
                        id = "Cm00" + (int.Parse(id) + 1).ToString();
                    }
                    else if (int.Parse(id) >= 9)
                    {
                        id = "Cm0" + (int.Parse(id) + 1).ToString();
                    }
                }
            }
            Comment dbcom = new Comment();
            dbcom.id_comment = id;
            dbcom.id_article = id_art;
            dbcom.id_commenter = us.id_user;
            dbcom.day_comment = DateTime.Now;
            dbcom.text_comment = text;
            dbcom.count_like = dbcom.count_dislike = 0;

            db.Comments.Add(dbcom);
            db.SaveChanges();

            Article_Comment arc = new Article_Comment();


            arc.Article = (from c in db.Articles where c.id_article == id_art && c.id_category == "CaAr01" && c.state_article == "1" select c).ToList();
            arc.Comments = (from c in db.Comments where c.id_article == id select c).ToList();
            return RedirectToAction("DetailVideo", "Client", new { id = id_art});
        }

        [HttpPost]
        public ActionResult DetailBlog(FormCollection f, string id_art = "")
        {
            id_art = f["id_art"];
            string text = f["comment"];
            User_ us = Session["user"] as User_;

            // Sinh id
            var com = (from c in db.Comments select c.id_comment).ToList();
            string id = "";
            if (com.Count == 0) // nếu danh sách rỗng
            {
                id = "Cm001";
            }
            else
            {
                for (int i = 0; i < com.Count(); i++)
                {
                    if (int.Parse(com[i].Substring(2, 3)) != (i + 1))
                    {
                        if (i + 1 >= 0 && i + 1 < 9)
                            id = "Cm00" + (i + 1).ToString();
                        else if (i + 1 > 9)
                            id = "Cm0" + (i + 1).ToString();
                        break;
                    }
                }
                if (id == "")
                {
                    id = com[com.Count - 1].Substring(2, 3);
                    if (int.Parse(id) >= 0 && int.Parse(id) < 9)
                    {
                        id = "Cm00" + (int.Parse(id) + 1).ToString();
                    }
                    else if (int.Parse(id) >= 9)
                    {
                        id = "Cm0" + (int.Parse(id) + 1).ToString();
                    }
                }
            }
            Comment dbcom = new Comment();
            dbcom.id_comment = id;
            dbcom.id_article = id_art;
            dbcom.id_commenter = us.id_user;
            dbcom.day_comment = DateTime.Now;
            dbcom.text_comment = text;
            dbcom.count_like = dbcom.count_dislike = 0;

            db.Comments.Add(dbcom);
            db.SaveChanges();

            Article_Comment arc = new Article_Comment();


            arc.Article = (from c in db.Articles where c.id_article == id_art && c.id_category == "CaAr01" && c.state_article == "1" select c).ToList();
            arc.Comments = (from c in db.Comments where c.id_article == id select c).ToList();
            return RedirectToAction("DetailBlog", "Client", new { id = id_art });
        }

        //Tìm kiếm theo ajax
        public ActionResult resultSearchPartial(string key)
        {

            //Tìm kiếm theo tên bài viết
            var lstBlog = db.Articles.Where(n => n.title_article.Contains(key) && n.id_category!="CaAr01"); //contains tìm kiếm gần đúng
            ViewBag.Key = key;
            return PartialView(lstBlog.OrderBy(n => n.time_pulish_article));
        }


        // GET: Contacts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id_contact,name_contact,email_contact,subject_contace,state_contact,date_contact")] Contact contact)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Contacts.Add(contact);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(contact);
        //}

        [HttpPost]
        public ActionResult DeleteComment(FormCollection f)
        {
            string id_comment = f["id_comment"];
            string cat = f["category"];
            Comment cmt = db.Comments.Find(id_comment);
            db.Comments.Remove(cmt);
            db.SaveChanges();

            if (cat == "Video")
            {
                return RedirectToAction("DetailVideo", "Client", new { id = cmt.id_article });
            }
            else if(cat == "Đánh giá")
            {
                return RedirectToAction("DetailVideo", "Client", new { id = cmt.id_article });
            }
            else
            {
                return RedirectToAction("DetailVideo", "Client", new { id = cmt.id_article });
            }
        }


       

       

    }
}