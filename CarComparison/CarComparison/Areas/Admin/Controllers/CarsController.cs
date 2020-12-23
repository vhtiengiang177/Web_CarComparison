using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using CarComparison.Areas.Admin.Models;
using Newtonsoft.Json;

namespace CarComparison.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class CarsController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();
        
        // GET: Admin/Cars

        public ActionResult Index(string searchname)
        {
            var car = from c in db.XemTenXe()
                      select c;
            if(!String.IsNullOrEmpty(searchname))
            {
                car = car.Where(c => (c.name_automaker.ToString() + " " + c.name_model.ToString() + " " + c.name_version.ToString()).Contains(searchname));
            }

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
            // SINH ID XE TỰ ĐỘNG
            var cars = (from c in db.Cars select c.id_car).ToList();
            string id = "";
            if (cars.Count == 0) // nếu danh sách rỗng
            {
                id = "Ca01";
            }
            else
            {
                for (int i = 0; i < cars.Count(); i++)
                {
                    if (int.Parse(cars[i].Substring(2, 2)) != (i + 1))
                    {
                        if (i + 1 >= 0 && i + 1 < 9)
                            id = "Ca0" + (i + 1).ToString();
                        else if (i + 1 > 9)
                            id = "Ca" + (i + 1).ToString();
                        break;
                    }
                }
                if(id == "")
                {
                    id = cars[cars.Count - 1].Substring(2, 2);
                    if (int.Parse(id) >= 0 && int.Parse(id) < 9)
                    {
                        id = "Ca0" + (int.Parse(id) + 1).ToString();
                    }
                    if (int.Parse(id) >= 9)
                    {
                        id = "Ca" + (int.Parse(id) + 1).ToString();
                    }
                }
            }
            Car car = new Car
            {
                id_car = id
            };
            ViewBag.id_version = new SelectList(db.Versions, "id_version", "name_version");
            return View(car);
        }

        // POST: Admin/Cars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
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
