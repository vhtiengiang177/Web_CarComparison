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
    public class TypeUsersController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();

        // GET: Admin/TypeUsers
        public ActionResult Index()
        {
            return View(db.TypeUsers.ToList());
        }

        // GET: Admin/TypeUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/TypeUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_typeuser,name_typeuser")] TypeUser typeUser)
        {
            if (ModelState.IsValid)
            {
                db.TypeUsers.Add(typeUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(typeUser);
        }

        // GET: Admin/TypeUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeUser typeUser = db.TypeUsers.Find(id);
            if (typeUser == null)
            {
                return HttpNotFound();
            }
            return View(typeUser);
        }

        // POST: Admin/TypeUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_typeuser,name_typeuser")] TypeUser typeUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(typeUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(typeUser);
        }

        // GET: Admin/TypeUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeUser typeUser = db.TypeUsers.Find(id);
            if (typeUser == null)
            {
                return HttpNotFound();
            }
            return View(typeUser);
        }

        // POST: Admin/TypeUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TypeUser typeUser = db.TypeUsers.Find(id);
            db.TypeUsers.Remove(typeUser);
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
