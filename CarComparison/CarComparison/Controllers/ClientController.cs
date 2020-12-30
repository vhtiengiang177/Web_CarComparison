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



        // GET: Client
        public ActionResult Index()
        {
            //Lần lượt tạo các viewbag để lấy các list từ csdl
            //List bài viết mới nhất

            var lstNewBlog = db.Articles.Where(n => n.id_category == "CaAr02").Where(a => a.state_article == "1");
            //Gán vào ViewBag
            ViewBag.ListNewBlog = lstNewBlog;

            //List video mới nhất
            var lstNewVideo = db.Articles.Where(n=>n.id_category== "CaAr01").Where(a => a.state_article == "1");
            //Gán vào ViewBag
            ViewBag.ListNewVideo = lstNewVideo; 
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

        
        //Trang bài viết
        public ActionResult Review(int? page)
        {
            //3 bài viết gợi ý
            var lstnew = (from c in db.Articles where c.id_category != "CaAr01" && c.state_article == "1" orderby c.time_pulish_article descending select c).ToList();
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
            var lstBlog = db.Articles.Where(n => n.id_category != "CaAr01" && n.state_article == "1");
            //Gán vào ViewBag
            ViewBag.ListBlog= lstBlog;

            //Thực hiện chức năng phân trang
            //Tạo biến số bài viết trên trang
            int PageSize = 4;
            //Tạo biến thứ 2: Số trang hiện tại
            int PageNumber = (page ?? 1); // gán bằng 1


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
            //Nếu không thì truy xuất csdl lấy ra bài viết tương ứng
            Article art = db.Articles.SingleOrDefault(n=>n.id_article==id && n.id_category!="CaAr01");
            if (art == null)
            {
                //Thông báo không có bài viết đó
                return HttpNotFound();
            }
            return View(art);
            
        }

        //Trang video
        public ActionResult Video(int? page)
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
            //Nếu không thì truy xuất csdl lấy ra bài viết tương ứng
            Article art = db.Articles.SingleOrDefault(n => n.id_article == id && n.id_category == "CaAr01");
            if (art == null)
            {
                //Thông báo không có bài viết đó
                return HttpNotFound();
            }
            return View(art);

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


        //Tìm kiếm theo ajax
        public ActionResult resultSearchPartial(string key)
        {

            //Tìm kiếm theo tên bài viết
            var lstBlog = db.Articles.Where(n => n.title_article.Contains(key) && n.id_category!="CaAr01"); //contains tìm kiếm gần đúng
            ViewBag.Key = key;
            return PartialView(lstBlog.OrderBy(n => n.time_pulish_article));
        }

    }
}