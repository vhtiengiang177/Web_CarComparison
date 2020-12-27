using CarComparison.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            User_ us = Session["user"] as User_;
            if(us== null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                return Content(us.name_user.ToString());
            }
        }
        
    }
}