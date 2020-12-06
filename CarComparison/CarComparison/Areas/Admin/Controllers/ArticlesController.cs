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
    public class ArticlesController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();

        // GET: Admin/Articles
        public ActionResult Index()
        {
            var articles = db.Articles.Include(a => a.CategoryArticle).Include(a => a.InfoAccount);
            return View(articles.ToList());
        }

        // GET: Admin/Articles/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // GET: Admin/Articles/Create
        public ActionResult Create()
        {
            ViewBag.id_category = new SelectList(db.CategoryArticles, "id_category", "name_category");
            ViewBag.id_user = new SelectList(db.InfoAccounts, "id_user", "name_user");
            return View();
        }

        // POST: Admin/Articles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_article,id_category,title_article,alias_article,id_user,description_article,time_pulish_article,time_write,state_article,img_article,linkvideo_article")] Article article)
        {
            if (ModelState.IsValid)
            {
                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_category = new SelectList(db.CategoryArticles, "id_category", "name_category", article.id_category);
            ViewBag.id_user = new SelectList(db.InfoAccounts, "id_user", "name_user", article.id_user);
            return View(article);
        }

        // GET: Admin/Articles/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_category = new SelectList(db.CategoryArticles, "id_category", "name_category", article.id_category);
            ViewBag.id_user = new SelectList(db.InfoAccounts, "id_user", "name_user", article.id_user);
            return View(article);
        }

        // POST: Admin/Articles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_article,id_category,title_article,alias_article,id_user,description_article,time_pulish_article,time_write,state_article,img_article,linkvideo_article")] Article article)
        {
            if (ModelState.IsValid)
            {
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_category = new SelectList(db.CategoryArticles, "id_category", "name_category", article.id_category);
            ViewBag.id_user = new SelectList(db.InfoAccounts, "id_user", "name_user", article.id_user);
            return View(article);
        }

        // GET: Admin/Articles/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Admin/Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
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
