using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using CarComparison.Areas.Admin.Models;
using Newtonsoft.Json;
using Version = CarComparison.Areas.Admin.Models.Version;

namespace CarComparison.Areas.Admin.Controllers
{
    // Chức năng: Quản lý xe
    //[AuthorizeController]
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
                    else if (int.Parse(id) >= 9)
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
        public ActionResult Create([Bind(Include = "id_car,overview_car,length_car,wheels_car,tireparameters_car,weight_car,capacity_car,horsepower_car,gear_car,torque_car,drivetype_car,frontbrakesystem_car,rearbrakesystem_car,maximumspeed_car,accelerationtime_car,capacitytank_car,numberseat_car,seat_car,airconditioner_car,sunroof_car,charge_car,img_car,imageFile")] Car car)
        {
            if (ModelState.IsValid)
            {
                // SINH ID CHO AUTOMAKER
                var auto = (from c in db.Automakers select c.id_automaker).ToList();
                string idauto = "";
                if (auto.Count == 0) // nếu danh sách rỗng
                {
                    idauto = "Au01";
                }
                else
                {
                    for (int i = 0; i < auto.Count(); i++)
                    {
                        if (int.Parse(auto[i].Substring(2, 2)) != (i + 1))
                        {
                            if (i + 1 >= 0 && i + 1 < 9)
                                idauto = "Au0" + (i + 1).ToString();
                            else if (i + 1 > 9)
                                idauto = "Au" + (i + 1).ToString();
                            break;
                        }
                    }
                    if (idauto == "")
                    {
                        idauto = auto[auto.Count - 1].Substring(2, 2);
                        if (int.Parse(idauto) >= 0 && int.Parse(idauto) < 9)
                        {
                            idauto = "Au0" + (int.Parse(idauto) + 1).ToString();
                        }
                        else if (int.Parse(idauto) >= 9)
                        {
                            idauto = "Au" + (int.Parse(idauto) + 1).ToString();
                        }
                    }
                }
                Automaker automaker = new Automaker();
                automaker.id_automaker = idauto;
                automaker.name_automaker = Request["Version.Model.Automaker.name_automaker"];
                // SINH ID CHO MODEL
                var mols = (from c in db.Models select c.id_model).ToList();
                string idmol = "";
                if (mols.Count == 0) // nếu danh sách rỗng
                {
                    idmol = "Mo01";
                }
                else
                {
                    for (int i = 0; i < mols.Count(); i++)
                    {
                        if (int.Parse(mols[i].Substring(2, 2)) != (i + 1))
                        {
                            if (i + 1 >= 0 && i + 1 < 9)
                                idmol = "Mo0" + (i + 1).ToString();
                            else if (i + 1 > 9)
                                idmol = "Mo" + (i + 1).ToString();
                            break;
                        }
                    }
                    if (idmol == "")
                    {
                        idmol = mols[mols.Count - 1].Substring(2, 2);
                        if (int.Parse(idmol) >= 0 && int.Parse(idmol) < 9)
                        {
                            idmol = "Mo0" + (int.Parse(idmol) + 1).ToString();
                        }
                        else if (int.Parse(idmol) >= 9)
                        {
                            idmol = "Mo" + (int.Parse(idmol) + 1).ToString();
                        }
                    }
                }
                Model model = new Model();
                model.id_model = idmol;
                model.name_model = Request["Version.Model.name_model"];
                model.id_automaker = automaker.id_automaker;
                // SINH ID CHO VERSION
                var vers = (from c in db.Versions select c.id_version).ToList();
                string id = "";
                if (vers.Count == 0) // nếu danh sách rỗng
                {
                    id = "Ve01";
                }
                else
                {
                    for (int i = 0; i < vers.Count(); i++)
                    {
                        if (int.Parse(vers[i].Substring(2, 2)) != (i + 1))
                        {
                            if (i + 1 >= 0 && i + 1 < 9)
                                id = "Ve0" + (i + 1).ToString();
                            else if (i + 1 > 9)
                                id = "Ve" + (i + 1).ToString();
                            break;
                        }
                    }
                    if (id == "")
                    {
                        id = vers[vers.Count - 1].Substring(2, 2);
                        if (int.Parse(id) >= 0 && int.Parse(id) < 9)
                        {
                            id = "Ve0" + (int.Parse(id) + 1).ToString();
                        }
                        else if (int.Parse(id) >= 9)
                        {
                            id = "Ve" + (int.Parse(id) + 1).ToString();
                        }
                    }
                }
                Version version = new Version();
                version.id_version = id;
                version.name_version = Request["Version.name_version"];
                version.id_model = model.id_model;

                
                string fileName = Path.GetFileNameWithoutExtension(car.imageFile.FileName);
                string extension = Path.GetExtension(car.imageFile.FileName);
                car.img_car = "~/Asset/Image/Car/" + car.id_car + extension;
                fileName = Path.Combine(Server.MapPath("~/Asset/Image/Car/"), car.id_car + extension);
                car.imageFile.SaveAs(fileName);
                //if (car.img_car != null && car.img_car != "")
                //{
                //    Bitmap bmp = new Bitmap(car.img_car);
                //    bmp.Save(Path.Combine("~/Asset/Image/Car/" + car.id_car + Path.GetExtension(car.img_car)));
                //}

                db.Automakers.Add(automaker);
                db.Models.Add(model);
                db.Versions.Add(version);

                car.id_version = version.id_version;
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
            Version ver = db.Versions.Find(car.id_version);
            Model mol = db.Models.Find(ver.id_model);
            Automaker au = db.Automakers.Find(mol.id_automaker);
            if(car.img_car != null && car.img_car != "")
            {
                string fullpath = Server.MapPath(car.img_car);
                FileInfo fi = new FileInfo(fullpath);
                if (fi != null)
                {
                    System.IO.File.Delete(fullpath);
                    fi.Delete();
                }
            }
            db.Cars.Remove(car);
            db.Versions.Remove(ver);
            db.Models.Remove(mol);
            db.Automakers.Remove(au);
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
