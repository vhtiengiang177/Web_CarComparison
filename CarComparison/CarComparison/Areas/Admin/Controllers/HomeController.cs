using CarComparison.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarComparison.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        //public static User_ adminLogin;
        [AuthorizeController]
        // GET: Admin/Home

        public ActionResult Index()
        {
            //User_ user = Global.GlobalUser;
            return View();
        }
    }
}