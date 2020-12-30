using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarComparison.Areas.Admin.Models;

namespace CarComparison.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        private CompareCarEntities db = new CompareCarEntities();
        public ActionResult Index()
        {
            return View();
        }

        //Tìm kiếm thông thường
        public ActionResult resultSearch(string key)
        {
            //Tìm kiếm theo tên bài viết
            var lstBlog = db.Articles.Where(n => n.title_article.Contains(key)); //contains tìm kiếm gần đúng
            return View(lstBlog.OrderBy(n=>n.title_article));
        }

        public ActionResult resultSearchPartial(string key)
        {

            //Tìm kiếm theo tên bài viết
            var lstBlog = db.Articles.Where(n => n.title_article.Contains(key)); //contains tìm kiếm gần đúng
            ViewBag.Key = key;
            return PartialView(lstBlog.OrderBy(n=>n.time_pulish_article));
        }


    }
}