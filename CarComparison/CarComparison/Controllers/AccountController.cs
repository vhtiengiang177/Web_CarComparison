using CarComparison.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CarComparison.Controllers
{
    // Chức năng: Sửa thông tin tài khoản sau khi đăng nhập, cả admin và member đều dùng chung
    public class AccountController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();

        //// GET: Account
        //public ActionResult Index()
        //{
        //    return View();
        //}

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
            User_ us = Session["user"] as User_;
            if (us != null)
            {
                return RedirectToAction("Index", "Client");
            }
            return View();
        }

        [HttpPost]
        //xử lý đăng nhập
        public ActionResult Login(FormCollection f)
        {
            string userName = f["username"].ToString();
            string passWord = f["password"].ToString();
            string passWordmd5 = GetMD5(passWord);
            User_ us = (from c in db.User_ where c.name_user == userName && c.password_user == passWordmd5 select c).SingleOrDefault();
            //nếu user nhập đúng mật khẩu
            if (us != null)
            {
                if (us.block_state_user == "0" && us.id_typeuser != "TyUs01") // Không phải admin và trạng thái ngừng hoạt động
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
                        return RedirectToAction("Index", "Admin/Home");
                    }
                    if (us.id_typeuser == "TyUs02")
                    {
                        return RedirectToAction("Index", "Client");
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
            User_ us = Session["user"] as User_;
            if (us != null)
            {
                return RedirectToAction("Index", "Client");
            }
            return View();
        }


        public ActionResult Register(FormCollection f)
        {
            try
            {
                string userName = f["username"].ToString();
                var checkUserName = (from c in db.User_ where c.name_user == userName select c).ToList();
                if (checkUserName.Count() != 0)
                {
                    ModelState.AddModelError("RegisterError", "Tên tài khoản đã tổn tại!");
                }
                else
                {
                    string passWord = f["password"].ToString();
                    string passWordmd5 = GetMD5(passWord);
                    string lastName = f["lname"].ToString();
                    string firstName = f["fname"].ToString();
                    string email = f["email"].ToString();
                    string sex = f["sex"];
                    DateTime birthday = Convert.ToDateTime(f["birthday"]);
                    User_ us_new = new User_();
                    // Tạo id user tự động
                    var createID = (from c in db.User_ select c.id_user).ToList();
                    string id = "";
                    if (createID.Count == 0) // nếu danh sách rỗng
                    {
                        id = "Us01";
                    }
                    else
                    {
                        for (int i = 0; i < createID.Count(); i++)
                        {
                            if (int.Parse(createID[i].Substring(2, 2)) != (i + 1))
                            {
                                if (i + 1 >= 0 && i + 1 < 9)
                                    id = "Us0" + (i + 1).ToString();
                                else if (i + 1 > 9)
                                    id = "Us" + (i + 1).ToString();
                                break;
                            }
                        }
                        if (id == "")
                        {
                            id = createID[createID.Count - 1].Substring(2, 2);
                            if (int.Parse(id) >= 0 && int.Parse(id) < 9)
                            {
                                id = "Us0" + (int.Parse(id) + 1).ToString();
                            }
                            else if (int.Parse(id) >= 9)
                            {
                                id = "Us" + (int.Parse(id) + 1).ToString();
                            }
                        }
                    }
                    us_new.id_user = id;
                    us_new.id_typeuser = "TyUs02"; // Member
                    us_new.lname_user = lastName;
                    us_new.fname_user = firstName;
                    us_new.email_user = email;
                    if (sex == "1")
                    {
                        us_new.sex_user = "Nam";
                    }
                    else if (sex == "2")
                    {
                        us_new.sex_user = "Nữ";
                    }
                    else
                    {
                        us_new.sex_user = "Khác";
                    }
                    us_new.birthday_user = birthday;
                    us_new.block_state_user = "1";
                    us_new.name_user = userName;
                    us_new.password_user = passWordmd5;
                    us_new.registerdate_user = DateTime.Now;
                    us_new.lastvisitdate_user = DateTime.Now;
                    us_new.address_user = us_new.avt_user = us_new.phone_user = "";
                    db.User_.Add(us_new);
                    db.SaveChanges();
                    Session["user"] = us_new;
                    return RedirectToAction("Index", "Client");
                }
                return View();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Lỗi: Không thể tạo được tài khoản, vui lòng đăng ký lại!" + ex.Message + "');</script>");
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Remove("user");
            Session.Clear();
            return RedirectToAction("Index", "Client");
        }

        [AuthorizeController]
        public ActionResult AccountInformation(string id_user = "", string tab = "")
        {
            User_ us = Session["user"] as User_;
            if (us == null)
            {
                return RedirectToAction("Index", "Client");
            }
            ViewBag.tabactive = tab;
            if (id_user != "")
            {
                User_ user = db.User_.Find(id_user);
                return View(user);
            }
            return View(us);
        }

        [HttpPost]
        public ActionResult EditInfo(FormCollection f, [Bind(Include = "imageFile" )] User_ user)
        {
            string action = f["action"];
            string formID = f["id_user"];
            User_ us = db.User_.Find(formID);
            if (us == null)
            {
                return HttpNotFound();
            }
            if (action == "Lưu")
            {
                string lname = f["lname_user"];
                string fname = f["fname_user"];
                string email = f["email_user"];
                string phone = f["phone_user"];
                string sex = f["sex"];
                DateTime birthday = Convert.ToDateTime(f["birthday_us"]);
                string address = f["address_user"];
                us.lname_user = lname;
                us.fname_user = fname;
                us.email_user = email;
                us.phone_user = phone;
                if (sex == "1")
                {
                    us.sex_user = "Nam";
                }
                else if (sex == "2")
                {
                    us.sex_user = "Nữ";
                }
                else
                {
                    us.sex_user = "Khác";
                }
                us.birthday_user = birthday;
                us.address_user = address;
                us.imageFile = user.imageFile;
                if (us.imageFile != null)
                {
                    if (us.avt_user != null && us.avt_user != "" && us.avt_user != "/Asset/Image/User/Default.jpg")
                    {
                        string fullpath = Server.MapPath(us.avt_user);
                        FileInfo fi = new FileInfo(fullpath);
                        if (fi != null)
                        {
                            System.IO.File.Delete(fullpath);
                            fi.Delete();
                        }
                    }
                    string fileName = Path.GetFileNameWithoutExtension(us.imageFile.FileName);
                    string extension = Path.GetExtension(us.imageFile.FileName);
                    us.avt_user = "/Asset/Image/User/" + us.id_user + extension;
                    fileName = Path.Combine(Server.MapPath("/Asset/Image/User/"), us.id_user + extension);
                    us.imageFile.SaveAs(fileName);
                }

                db.Entry(us).State = EntityState.Modified;
                db.SaveChanges();
            }
            else if (action == "Hủy")
            {
                db.Entry(us).State = EntityState.Unchanged;
                db.SaveChanges();
            }
            return RedirectToAction("AccountInformation", "Account", new { us.id_user, tab = "edit" });
        }

        [HttpPost]
        public ActionResult ChangePassword(FormCollection f)
        {
            string formID = f["id_user"];
            User_ us = db.User_.Find(formID);
            if (us == null)
            {
                return HttpNotFound();
            }
            //string ol = f["password_old"];
            //string ne = f["password_new"];
            //string re_ne = f["repassword_new"];
            string olmd5 = GetMD5(f["password_old"]);
            string nemd5 = GetMD5(f["password_new"]);
            string re_nemd5 = GetMD5(f["repassword_new"]);
            if (nemd5 != re_nemd5 || olmd5 != us.password_user)
            {
                if (nemd5 != re_nemd5)
                {
                    ModelState.AddModelError("errorChange", "Mật khẩu mới và nhập lại mật khẩu không trùng nhau!");
                }
                else
                {
                    ModelState.AddModelError("errorChange2", "Mật khẩu cũ không đúng!");
                }
                return RedirectToAction("AccountInformation", "Account", new { us.id_user, tab = "changepassf" });
            }
            us.password_user = nemd5;
            db.Entry(us).State = EntityState.Modified;
            db.SaveChanges();
            Response.Write("<script>alert('Đổi thành công!');</script>");
            return RedirectToAction("AccountInformation", "Account", new { us.id_user, tab = "changepasst" });
        }

        [HttpPost]
        public ActionResult DisableAccount(FormCollection f)
        {
            string action = f["action"];
            string formID = f["id_user"];
            User_ us = db.User_.Find(formID);
            if (us == null)
            {
                return HttpNotFound();
            }
            if (action == "Vô hiệu hóa tài khoản")
            {
                us.block_state_user = "0";
                db.Entry(us).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Logout"); // Thoát khỏi trang
            }
            return RedirectToAction("AccountInformation", "Account", new { us.id_user });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}