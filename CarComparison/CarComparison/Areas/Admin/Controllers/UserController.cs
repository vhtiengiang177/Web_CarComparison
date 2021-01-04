using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CarComparison.Areas.Admin.Models;

namespace CarComparison.Areas.Admin.Controllers
{
    // Chức năng: Quản lý các tài khoản trên Web
    [AuthorizeController]
    public class UserController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();

        // GET: Admin/User
        public ActionResult Index()
        {
            var user_ = db.User_.Include(u => u.TypeUser);
            return View(user_.ToList());
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

        // GET: Admin/User/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_ user_ = db.User_.Find(id);
            if (user_ == null)
            {
                return HttpNotFound();
            }
            return View(user_);
        }

        // GET: Admin/User/Create
        public ActionResult Create()
        {
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
            User_ us = new User_
            {
                id_user = id
            };
            ViewBag.id_typeuser = new SelectList(db.TypeUsers, "id_typeuser", "name_typeuser");
            return View(us);
        }

        // POST: Admin/User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_user,name_user,password_user,id_typeuser,lname_user,fname_user,email_user,avt_user,block_state_user,registerdate_user,lastvisitdate_user,phone_user,sex_user,birthday_user,address_user")] User_ user_)
        {
            string bday = Request["birthday"];
            if (user_.name_user == "" || user_.lname_user == "" || user_.fname_user == "" || user_.password_user == "" || bday == null || user_.id_typeuser == "")
            {
                if(user_.name_user == "")
                {
                    ModelState.AddModelError("usname", "Không được để trống tên tài khoản");
                }
                if(user_.lname_user == "")
                {
                    ModelState.AddModelError("lname", "Không được để trống họ");
                }
                if (user_.fname_user == "")
                {
                    ModelState.AddModelError("fname", "Không được để trống tên");
                }
                if (user_.password_user == "")
                {
                    ModelState.AddModelError("pass", "Không được để trống mật khẩu");
                }
                if (bday == null)
                {
                    ModelState.AddModelError("bday", "Không được để trống ngày sinh");
                }
                if (user_.id_typeuser == "")
                {
                    ModelState.AddModelError("typeus", "Không được để trống loại tài khoản");
                }
                ViewBag.id_typeuser = new SelectList(db.TypeUsers, "id_typeuser", "name_typeuser", user_.id_typeuser);
                return View(user_);
            }
            var checkUserName = (from c in db.User_ where c.name_user == user_.name_user select c).ToList();
            if (checkUserName.Count() != 0)
            {
                ModelState.AddModelError("RegisterError", "Tên tài khoản đã tổn tại!");
            }
            if (ModelState.IsValid)
            {
                user_.password_user = GetMD5(user_.password_user);
                string sex = Request["sex"];
                DateTime birthday = Convert.ToDateTime(Request["birthday"]);
                if (sex == "1")
                {
                    user_.sex_user = "Nam";
                }
                else if (sex == "2")
                {
                    user_.sex_user = "Nữ";
                }
                else
                {
                    user_.sex_user = "Khác";
                }
                user_.birthday_user = birthday;
                user_.block_state_user = "1";
                user_.registerdate_user = DateTime.Now;
                user_.avt_user = "/Asset/Image/User/Default.jpg";
                db.User_.Add(user_);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_typeuser = new SelectList(db.TypeUsers, "id_typeuser", "name_typeuser", user_.id_typeuser);
            return View(user_);
        }

        [HttpPost]
        public ActionResult Block()
        {
            string id = Request["id"];
            User_ us = db.User_.Find(id);
            string action = Request["action"];
            if(action == "Khóa")
            {
                us.block_state_user = "0";
            }
            else if(action == "Mở khóa")
            {
                us.block_state_user = "1";
            }
            db.Entry(us).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        //// GET: Admin/User/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    User_ user_ = db.User_.Find(id);
        //    if (user_ == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user_);
        //}

        //// POST: Admin/User/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    User_ user_ = db.User_.Find(id);
        //    db.User_.Remove(user_);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
