﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarComparison.Controllers
{
    // Chức năng: Sửa thông tin tài khoản sau khi đăng nhập, cả admin và member đều dùng chung
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
    }
}