using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarComparison.Areas.Admin.Models;

namespace CarComparison.Areas.Admin.Controllers
{
    // Chức năng: Quản lý các bình luận
    [AuthorizeController]
    public class CommentsController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();

        // GET: Admin/Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Article).Include(c => c.Comment2).Include(c => c.User_);
            return View(comments.ToList());
        }

        // GET: Admin/Comments/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        //// GET: Admin/Comments/Create
        //public ActionResult Create()
        //{
        //    ViewBag.id_article = new SelectList(db.Articles, "id_article", "id_category");
        //    ViewBag.id_reply_comment = new SelectList(db.Comments, "id_comment", "id_reply_comment");
        //    ViewBag.id_commenter = new SelectList(db.User_, "id_user", "name_user");
        //    return View();
        //}

        //// POST: Admin/Comments/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id_comment,id_reply_comment,id_article,id_commenter,day_comment,count_like,count_dislike,text_comment")] Comment comment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Comments.Add(comment);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.id_article = new SelectList(db.Articles, "id_article", "id_category", comment.id_article);
        //    ViewBag.id_reply_comment = new SelectList(db.Comments, "id_comment", "id_reply_comment", comment.id_reply_comment);
        //    ViewBag.id_commenter = new SelectList(db.User_, "id_user", "name_user", comment.id_commenter);
        //    return View(comment);
        //}

        //// GET: Admin/Comments/Edit/5
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Comment comment = db.Comments.Find(id);
        //    if (comment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.id_article = new SelectList(db.Articles, "id_article", "id_category", comment.id_article);
        //    ViewBag.id_reply_comment = new SelectList(db.Comments, "id_comment", "id_reply_comment", comment.id_reply_comment);
        //    ViewBag.id_commenter = new SelectList(db.User_, "id_user", "name_user", comment.id_commenter);
        //    return View(comment);
        //}

        //// POST: Admin/Comments/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "id_comment,id_reply_comment,id_article,id_commenter,day_comment,count_like,count_dislike,text_comment")] Comment comment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(comment).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.id_article = new SelectList(db.Articles, "id_article", "id_category", comment.id_article);
        //    ViewBag.id_reply_comment = new SelectList(db.Comments, "id_comment", "id_reply_comment", comment.id_reply_comment);
        //    ViewBag.id_commenter = new SelectList(db.User_, "id_user", "name_user", comment.id_commenter);
        //    return View(comment);
        //}

        // GET: Admin/Comments/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Admin/Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
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
