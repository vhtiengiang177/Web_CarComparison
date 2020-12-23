﻿using CarComparison.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CarComparison.Controllers
{
    // Đăng ký, đăng nhập, kiểm tra tài khoản ở đây
    public class LoginController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        // Hash mật khẩu
        public string GetMD5(string str)
        {
            str = str + "md5";
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            StringBuilder sbHash = new StringBuilder();

            foreach (byte b in bHash)
            {

                sbHash.Append(String.Format("{0:x2}", b));

            }
            return sbHash.ToString();
        }

        // View đăng nhập
        public ActionResult LoginView()
        {
            return View();
        }

        [HttpPost]
        //xử lý đăng nhập
        public ActionResult Login(FormCollection f)
        {

            string taikhoan = f["username"].ToString();
            string matkhau = f["password"].ToString();
            string matkhaumd5 = GetMD5(matkhau);
            User_ us = db.User_.SingleOrDefault(n => n.name_user == taikhoan && n.password_user == matkhaumd5);
            //nếu user nhập đúng mật khẩu
            if (us != null)
            {
                if (us.block_state_user == "0" && us.id_typeuser != "1") // Không phải admin và trạng thái ngừng hoạt động
                {
                    return Content("er_block"); // Trả về trang thông báo
                }
                else
                {
                    Session["user"] = us;
                    try
                    { //lấy thời gian đăng nhập lưu vào hệ thống
                        us.lastvisitdate_user = DateTime.Now;
                        db.SaveChanges();
                    }
                    catch { };

                    if (us.id_typeuser == "TyUs01")
                    {
                        return View("~/Areas/Admin/Views/Home/Index.cshtml");
                    }
                    if (us.id_typeuser == "TyUs02")
                    {
                        return View("~/Views/Client/Index.cshtml");
                    }
                }
            }
            return Content("false");
            //if (us != null)
            //{
            //    //Global.SetGlobalUser(us);
            //    //return Redirect("Home", "Areas/Admin/")
            //    return View("~/Areas/Admin/Views/Home/Index.cshtml");
            //    //    if (us.block == false && us.usertype != "1")
            //    //    {
            //    //        return Content("er_block");
            //    //    }
            //    //    else
            //    //    {
            //    //        Session["user"] = us;
            //    //        try
            //    //        { //lấy thời gian đăng nhập lưu vào hệ thống
            //    //            us.lastvisitdate = DateTime.Now;
            //    //            db.SaveChanges();
            //    //        }
            //    //        catch { };

            //    //        if (us.usertype == "3")
            //    //        {
            //    //            return Content("/SinhVien/QuanLyTaiKhoan");
            //    //        }
            //    //        if (us.usertype == "1")
            //    //        {
            //    //            return Content("/Home/Index");
            //    //        }
            //    //        if (us.usertype == "2")
            //    //        {
            //    //            return Content("/GiangVien/QuanLyTaiKhoan");
            //    //        }
            //    //        if (us.usertype == "4")
            //    //        {
            //    //            return Content("/TruongBoMon/QuanLyTaiKhoan");
            //    //        }
            //    //    }
            //    //}
            //}
            //return View("~/Views/Client/Index.cshtml");
        }

        // View đăng ký
        public ActionResult RegisterView()
        {
            return View();
        }


    }
}