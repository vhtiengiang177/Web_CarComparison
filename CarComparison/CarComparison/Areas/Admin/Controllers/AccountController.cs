using CarComparison.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CarComparison.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {

        private CompareCarEntities db = new CompareCarEntities();
        // GET: Admin/Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
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

        [HttpPost]
        //xử lý đăng nhập
        public ActionResult DangNhap(FormCollection f)
        {

            string taikhoan = f["username"].ToString();
            string matkhau = f["password"].ToString();
            string matkhaumd5 = GetMD5(matkhau);
            InfoAccount us = db.InfoAccounts.SingleOrDefault(n => n.name_user == taikhoan && n.password_user == matkhau);
            //nếu user nhập đúng mật khẩu
            if (us != null)
            {
                //Global.SetGlobalUser(us);
                //return Redirect("Home", "Areas/Admin/")
                return View("~/Areas/Admin/Views/Home/Index.cshtml");
                //    if (us.block == false && us.usertype != "1")
                //    {
                //        return Content("er_block");
                //    }
                //    else
                //    {
                //        Session["user"] = us;
                //        try
                //        { //lấy thời gian đăng nhập lưu vào hệ thống
                //            us.lastvisitdate = DateTime.Now;
                //            db.SaveChanges();
                //        }
                //        catch { };

                //        if (us.usertype == "3")
                //        {
                //            return Content("/SinhVien/QuanLyTaiKhoan");
                //        }
                //        if (us.usertype == "1")
                //        {
                //            return Content("/Home/Index");
                //        }
                //        if (us.usertype == "2")
                //        {
                //            return Content("/GiangVien/QuanLyTaiKhoan");
                //        }
                //        if (us.usertype == "4")
                //        {
                //            return Content("/TruongBoMon/QuanLyTaiKhoan");
                //        }
                //    }
                //}
            }
            return View("~/Views/Client/Index.cshtml");
        }
    }
}