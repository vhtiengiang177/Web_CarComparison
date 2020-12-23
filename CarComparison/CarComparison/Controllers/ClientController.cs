using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
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
            return View();
        }

        public ActionResult About()
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

        //public ActionResult Comparing()
        //{
        //    CompareCarEntities db = new CompareCarEntities();
        //    var getAutomakerList = db.Automakers.ToList();
        //    SelectList list = new SelectList(getAutomakerList, "id_automaker", "name_automaker");
        //    ViewBag.automakerlistname = list;
        //    return View();


        //}

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

            //from a in db.Models
            //join c in db.Versions on a.id_model equals c.id_model
            //join b in db.Automakers on a.id_automaker equals b.id_automaker
            //join d in db.Cars on c.id_version equals d.id_version
            //where d.id_car == idCar
            //select new
            //{
            //    tenxe = a.name_model + " " + c.name_version,

            //};
            return Json(CarList, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult getCar(string idver)
        //{
        //    List<Car> CarList = new List<Car>();
        //    CarList = db.Cars.ToList();
        //    return Json(CarList, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //public ActionResult Comparing(string automakerId, string modelId, string versionId)
        //{
        //    cascadingmodel model = new cascadingmodel();
        //    foreach (var auto in db.Automakers)
        //    {
        //        model.Automaker.Add(new SelectListItem { Text = auto.name_automaker, Value = auto.id_automaker.ToString() });
        //    }

        //    if (automakerId != null)
        //    {
        //        var mol = (from mo in db.Models
        //                      where mo.id_automaker == automakerId
        //                   select mo).ToList();
        //        foreach (var mo in mol)
        //        {
        //            model.Model.Add(new SelectListItem { Text = mo.name_model, Value = mo.id_model });
        //        }

        //        if (modelId != null)
        //        {
        //            var version = (from ver in db.Versions
        //                          where ver.id_model == modelId
        //                           select ver).ToList();
        //            foreach (var ver in version)
        //            {
        //                model.Version.Add(new SelectListItem { Text = ver.name_version, Value = ver.id_version });
        //            }
        //        }
        //    }

        //    return View(model);
        //}

        public ActionResult Add()
        {
            return View();
        }

       
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        
        //public ActionResult Register(string username, string password, string password2)
        //{
        //    //if (LoginDAO.Instance.Register(username, password, password2))
        //    //{
        //    //    return View("Success");
        //    //}
        //    //else
        //        return View("Error");
           
        //}


        //public ActionResult RegisterView()
        //{
        //    return View("Register");
        //}

        //public ActionResult Team()
        //{
        //    return View();
        //}

        public ActionResult Review(int? page)
        {

            
            return View();
        }
    }
}