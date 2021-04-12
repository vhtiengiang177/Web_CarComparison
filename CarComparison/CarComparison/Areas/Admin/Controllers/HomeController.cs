using CarComparison.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace CarComparison.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class HomeController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();
        //public static User_ adminLogin;
        // GET: Admin/Home
        public ActionResult Index()
        {
            //User_ user = Global.GlobalUser;
            //User_ us = Session["user"] as User_;
            List<DataPoint> dataPoints = new List<DataPoint>();

            var query = from p in db.Automakers
                        join c in db.Cars on p.id_automaker equals c.Version.Model.id_automaker
                        group p by p.name_automaker into g
                        select new
                        {
                            name = g.Key,
                            count = g.Count()
                        };

            double total = 0;
            foreach (var item in query)
            {
                total += item.count;
            }
            foreach (var item in query)
            {
                double pecent = Math.Round((item.count/total)*100);
                dataPoints.Add(new DataPoint(item.name, pecent));
            }
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);


          
            List<DataPoint> dataPointsArticle = new List<DataPoint>();
            var lst = from p in db.Articles
                      where p.state_article=="1"
                      orderby p.view_article descending

                      select new
                      {
                          name = p.title_article,
                          view = p.view_article
                       };
            int count = 0;
            foreach (var item in lst)
            {
                count += 1;
                if (count > 9) break;
                dataPointsArticle.Add(new DataPoint(item.name, Convert.ToDouble(item.view)));
            }
            ViewBag.dataPointsArticle = JsonConvert.SerializeObject(dataPointsArticle);

            return View();
        }

        [ChildActionOnly] // action cannot be requested directly via URL
        public ActionResult CountContact()
        {
            var con = db.Contacts.Where(c => c.state_contact == "0").ToList();
            return Content(con.Count().ToString());
        }

        [ChildActionOnly] // action cannot be requested directly via URL
        public ActionResult Notification()
        {
            var lstnew = (from c in db.Contacts orderby c.date_contact descending select c).ToList();
            var lstnew3 = new List<Contact>();
            if (lstnew.Count() >= 4)
            {
                lstnew3.Add(lstnew[0]);
                lstnew3.Add(lstnew[1]);
                lstnew3.Add(lstnew[2]);
                lstnew3.Add(lstnew[3]);
                ViewBag.lstnew3 = lstnew3;
            }
            else if (lstnew.Count() == 3)
            {
                lstnew3.Add(lstnew[0]);
                lstnew3.Add(lstnew[1]);
                lstnew3.Add(lstnew[2]);
                ViewBag.lstnew3 = lstnew3;
            }
            else if (lstnew.Count() == 2)
            {
                lstnew3.Add(lstnew[0]);
                lstnew3.Add(lstnew[1]);
                ViewBag.lstnew3 = lstnew3;
            }
            else if (lstnew.Count() == 1)
            {
                lstnew3.Add(lstnew[0]);
                ViewBag.lstnew3 = lstnew3;
            }
            else if (lstnew.Count() == 0)
            {
                ViewBag.lstnew3 = null;
            }
            return PartialView("~/Areas/Admin/Views/Home/NotificationPartialView.cshtml", ViewBag.lstnew3);
        }
    }
}