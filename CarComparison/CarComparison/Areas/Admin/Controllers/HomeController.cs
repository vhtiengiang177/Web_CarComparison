﻿using CarComparison.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarComparison.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class HomeController : Controller
    {
        //public static User_ adminLogin;
        // GET: Admin/Home
        public ActionResult Index()
        {
            //User_ user = Global.GlobalUser;
            return View();
        }
    }
}