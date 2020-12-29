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

            var lstNewBlog = db.Articles.Where(n => n.id_category == "CaAr02");
            //Gán vào ViewBag
            ViewBag.ListNewBlog = lstNewBlog;

            //List video mới nhất
            var lstNewVideo = db.Articles.Where(n=>n.id_category== "CaAr01");
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
        public ActionResult Review()
        {
            //3 bài viết gợi ý
            var lstnew = (from c in db.Articles where c.id_category != "CaAr01" orderby c.time_pulish_article descending select c).ToList();
            var lstnew3 = new List<Article>();
            lstnew3.Add(lstnew[0]);
            lstnew3.Add(lstnew[1]);
            lstnew3.Add(lstnew[2]);
            ViewBag.lstnew3 = lstnew3;

            //Toàn bộ bài viết
            var lstBlog = db.Articles.Where(n => n.id_category != "CaAr01" );
            //Gán vào ViewBag
            ViewBag.ListBlog= lstBlog;


            return View(lstBlog);
        }

        //Chi tiết bài viết
        public ActionResult DetailBlog(string id)
        {
            //3 bài viết gợi ý
            var lstnew = (from c in db.Articles where c.id_category != "CaAr01" orderby c.time_pulish_article descending select c).ToList();

            var lstnew3 = new List<Article>();
            lstnew3.Add(lstnew[0]);
            lstnew3.Add(lstnew[1]);
            lstnew3.Add(lstnew[2]);
            ViewBag.lstnew3 = lstnew3;

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
        public ActionResult Video()
        {
            //3 video mới nhất gợi ý
            var lstnew = (from c in db.Articles where c.id_category == "CaAr01" orderby c.time_pulish_article descending select c).ToList();
            var lstnew3video = new List<Article>();
            lstnew3video.Add(lstnew[0]);
            lstnew3video.Add(lstnew[1]);
            lstnew3video.Add(lstnew[2]);
            ViewBag.lstnew3video = lstnew3video;

            //video lớn ở đầu trang video
            var videofirst = new List<Article>();
            videofirst.Add(lstnew[0]);
            ViewBag.videofirst = videofirst;
            //Danh sách video
            var lstNewVideo = db.Articles.Where(n => n.id_category == "CaAr01");
            //Gán vào ViewBag
            ViewBag.ListNewVideo = lstNewVideo;


            return View(lstNewVideo);
        }

        //Chi tiết video
        public ActionResult DetailVideo(string id)
        {
            //3 video gợi ý
            var lstnew = (from c in db.Articles where c.id_category == "CaAr01" orderby c.time_pulish_article descending select c).ToList();
            var lstnew3video = new List<Article>();
            lstnew3video.Add(lstnew[0]);
            lstnew3video.Add(lstnew[1]);
            lstnew3video.Add(lstnew[2]);
            ViewBag.lstnew3video = lstnew3video;
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

        public ActionResult Comment()
        {
            return View();
        }


    }
}