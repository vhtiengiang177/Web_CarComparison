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

namespace CarComparison.Controllers
{
    public class ClientController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();

        

        
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

        public ActionResult Comparing()
        {
            CompareCarEntities db = new CompareCarEntities();
            var getAutomakerList = db.Automaker.ToList();
            SelectList list = new SelectList(getAutomakerList, "id_automaker", "name_automaker");
            ViewBag.automakerlistname = list;
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

       
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        
        public ActionResult Register(string username, string password, string password2)
        {
            //if (LoginDAO.Instance.Register(username, password, password2))
            //{
            //    return View("Success");
            //}
            //else
                return View("Error");
           
        }


        public ActionResult RegisterView()
        {
            return View("Register");
        }

        public ActionResult Team()
        {
            return View();
        }

        public ActionResult Review()
        {
            return View();
        }

    }
}