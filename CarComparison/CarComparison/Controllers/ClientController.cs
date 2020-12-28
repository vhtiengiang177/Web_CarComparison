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

            var lstNewBlog = db.Articles;
            //Gán vào ViewBag
            ViewBag.ListNewBlog = lstNewBlog;

            //List video mới nhất
            var lstNewVideo = db.Articles.Where(n=>n.id_category=="2");
            //Gán vào ViewBag
            ViewBag.ListNewVideo = lstNewVideo; 
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Team()
        {
            return View();
        }

        public ActionResult MyAcc()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        
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

        //public JsonResult joinName(string id_car)
        //{
        //    string name = "";
        //    var query = (from s in db.Cars
        //                 join c in db.Versions on s.id_version equals c.id_version
        //                 join t in db.Models on c.id_model equals t.id_model
        //                 join h in db.Automakers on t.id_automaker equals h.id_automaker
                        
        //                 select new
        //                 {
        //                     s.id_car,
        //                     c.name_version,
        //                     t.name_model,
        //                     h.name_automaker

        //                 });
        //    foreach (var item in query)
        //    {
        //        if (item.id_car== id_car)
        //        {
        //            name= item.name_automaker + " " + item.name_model + " " + item.name_version;
        //        }                    
        //    }

        //    return Json(name, JsonRequestBehavior.AllowGet);
        //}


        public JsonResult joinName(string id_version)
        {
            string name = "";
            var query = (
                         from c in db.Versions
                         join t in db.Models on c.id_model equals t.id_model
                         join h in db.Automakers on t.id_automaker equals h.id_automaker
                         where id_version == c.id_version
                         select new
                         {
                             c.id_version,
                             c.name_version,
                             t.name_model,
                             h.name_automaker

                         });
            foreach (var item in query)
            {
                if (item.id_version == id_version)
                {
                    name = item.name_automaker + " " + item.name_model + " " + item.name_version;
                }
            }

            return Json(name, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Add()
        {
            return View();
        }

       
        //[HttpGet]
        //public ActionResult Login()
        //{
        //    return View();
        //}

        
        

        public ActionResult Review()
        {

            
            return View();
        }

        public ActionResult Video()
        {


            return View();
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


    }
}