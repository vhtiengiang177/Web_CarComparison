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
    public class CarsController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();

        // GET: Admin/Cars
        public ActionResult Index()
        {
            var car = db.XemTenXe();
            return View(car.ToList());
        }

        // GET: Admin/Cars/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // GET: Admin/Cars/Create
        public ActionResult Create()
        {
            ViewBag.id_version = new SelectList(db.Versions, "id_version", "name_version");
            return View();
        }

        // POST: Admin/Cars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_car,overview_car,length_car,wheels_car,tireparameters_car,weight_car,capacity_car,horsepower_car,gear_car,torque_car,drivetype_car,frontbrakesystem_car,rearbrakesystem_car,maximumspeed_car,accelerationtime_car,capacitytank_car,numberseat_car,seat_car,airconditioner_car,sunroof_car,charge_car,img_car,id_version")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Cars.Add(car);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_version = new SelectList(db.Versions, "id_version", "name_version", car.id_version);
            return View(car);
        }

        // GET: Admin/Cars/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_version = new SelectList(db.Versions, "id_version", "name_version", car.id_version);
            return View(car);
        }

        // POST: Admin/Cars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_car,overview_car,length_car,wheels_car,tireparameters_car,weight_car,capacity_car,horsepower_car,gear_car,torque_car,drivetype_car,frontbrakesystem_car,rearbrakesystem_car,maximumspeed_car,accelerationtime_car,capacitytank_car,numberseat_car,seat_car,airconditioner_car,sunroof_car,charge_car,img_car,id_version")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_version = new SelectList(db.Versions, "id_version", "name_version", car.id_version);
            return View(car);
        }

        // GET: Admin/Cars/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Admin/Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Car car = db.Cars.Find(id);
            db.Cars.Remove(car);
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
