using CarComparison.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CarComparison.Controllers
{
    // Chức năng: Đăng ký, đăng nhập, kiểm tra tài khoản ở đây
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
        [AllowAnonymous]
        public ActionResult LoginView()
        {
            return View();
        }

        [HttpPost]
        //xử lý đăng nhập
        public ActionResult Login(FormCollection f)
        {
            string userName = f["username"].ToString();
            string passWord = f["password"].ToString();
            string passWordmd5 = GetMD5(passWord);
            User_ us = db.User_.SingleOrDefault(n => n.name_user == userName && n.password_user == passWordmd5);
            //nếu user nhập đúng mật khẩu
            if (us != null)
            {
                if (us.block_state_user == "0" && us.id_typeuser != "1") // Không phải admin và trạng thái ngừng hoạt động
                {
                    ModelState.AddModelError("LoginError", "Tài khoản đang bị khóa");
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
            else
            {
                ModelState.AddModelError("LoginError", "Tài khoản hoặc mật khẩu không đúng, vui lòng nhập lại!");
            }
            return View("LoginView");
        }

        // View đăng ký
        [AllowAnonymous]
        public ActionResult RegisterView()
        {
            return View();
        }


        public ActionResult Register(FormCollection f)
        {
            string userName = f["username"].ToString();
            var checkUserName = (from c in db.User_ where c.name_user == userName select c).ToList();
            if(checkUserName.Count() != 0)
            {
                ModelState.AddModelError("RegisterError", "Tên tài khoản đã tổn tại!");
            }
            else
            {
                string passWord = f["password"].ToString();
                string passWordmd5 = GetMD5(passWord);
                string lastName = f["lname"].ToString();
                string firstName = f["fname"].ToString();
                var sex = Convert.ToInt32(f["sex"]);
            }
            
            return View();
        }

        public ActionResult Logout()
        {
            Session.Remove("user");
            Session.Clear();
            return View("~/Views/Client/Index.cshtml");
        }
    }
}