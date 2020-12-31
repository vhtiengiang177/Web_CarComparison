using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using CarComparison.Areas.Admin.Models;

namespace CarComparison.Areas.Admin.Controllers
{
    public class ContactsController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();

        // GET: Admin/Contacts
        public ActionResult Index()
        {
            var model = db.Contacts
                 .OrderByDescending(p => p.date_contact)
                 .ToList();
            return View(model);
        }

        // GET: Admin/Contacts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // GET: Admin/Contacts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_contact,name_contact,email_contact,state_contact,date_contact,subject_contact")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Contacts.Add(contact);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contact);
        }

        // GET: Admin/Contacts/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Admin/Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Contact contact = db.Contacts.Find(id);
            db.Contacts.Remove(contact);
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



        [HttpPost]
        public ActionResult ExportToExcel()
        {
           
            var gv = new System.Web.UI.WebControls.GridView();
            gv.DataSource = db.Contacts
                //.Where(p => p.state_contact == "1")
                .Select(r => new {
                    Names = r.name_contact,
                    Emails = r.email_contact,
                    Date = r.date_contact,
                    Subject = r.subject_contact
                })
                .OrderByDescending(p => p.Date)
                .ToList();
            gv.DataBind();
            Response.Clear();
            Response.Buffer = true;
            //Response.AddHeader("content-disposition",
            // "attachment;filename=GridViewExport.xls");
            Response.Charset = "utf-8";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.xls");
            //Mã hóa chữa sang UTF8
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());

            System.IO.StringWriter sw = new StringWriter();
            System.Web.UI.HtmlTextWriter hw = new HtmlTextWriter(sw);

            if (gv.Rows.Count >0)
            {
                for (int i = 0; i < gv.Rows.Count; i++)
                {
                    //Apply text style to each Row
                    gv.Rows[i].Attributes.Add("class", "textmode");
                }


                //Add màu nền cho header của file excel
                gv.HeaderRow.BackColor = System.Drawing.Color.DarkBlue;
                //Màu chữ cho header của file excel
                gv.HeaderStyle.ForeColor = System.Drawing.Color.White;

                gv.HeaderRow.Cells[0].Text = "Họ Tên";
                gv.HeaderRow.Cells[1].Text = "Email";
                gv.HeaderRow.Cells[2].Text = "Ngày nhận";
                gv.HeaderRow.Cells[3].Text = "Nội dung";
                gv.RenderControl(hw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                var model = db.Contacts
                    .OrderByDescending(p => p.date_contact)
                    .ToList();
                return View("View", model);
            }
            return View();
            
        }
    }
}
