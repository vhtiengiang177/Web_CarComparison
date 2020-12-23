using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
            ViewBag.id_typeuser = new SelectList(db.TypeUsers, "id_typeuser", "name_typeuser");
            return View();
        }

        // POST: Admin/User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_user,name_user,password_user,id_typeuser,lname_user,fname_user,email_user,avt_user,block_state_user,registerdate_user,lastvisitdate_user,phone_user,sex_user,birthday_user,address_user")] User_ user_)
        {
            if (ModelState.IsValid)
            {
                db.User_.Add(user_);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_typeuser = new SelectList(db.TypeUsers, "id_typeuser", "name_typeuser", user_.id_typeuser);
            return View(user_);
        }

        // GET: Admin/User/Edit/5
        public ActionResult Edit(string id)
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
            ViewBag.id_typeuser = new SelectList(db.TypeUsers, "id_typeuser", "name_typeuser", user_.id_typeuser);
            return View(user_);
        }

        // POST: Admin/User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_user,name_user,password_user,id_typeuser,lname_user,fname_user,email_user,avt_user,block_state_user,registerdate_user,lastvisitdate_user,phone_user,sex_user,birthday_user,address_user")] User_ user_)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user_).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_typeuser = new SelectList(db.TypeUsers, "id_typeuser", "name_typeuser", user_.id_typeuser);
            return View(user_);
        }

        // GET: Admin/User/Delete/5
        public ActionResult Delete(string id)
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

        // POST: Admin/User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User_ user_ = db.User_.Find(id);
            db.User_.Remove(user_);
            db.SaveChanges();
            return RedirectToAction("Index");
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
