using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HiringFunnel.DAL;
using HiringFunnel.Models;

namespace HiringFunnel.Controllers
{
    public class AnnotationsController : Controller
    {
        private HFContext db = new HFContext();

        // GET: Annotations
        public ActionResult Index()
        {
            var annotations = db.Annotations.Include(a => a.Author).Include(a => a.InstanceOfProcess).Include(a => a.Receiver);
            return View(annotations.ToList());
        }

        // GET: Annotations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Annotation annotation = db.Annotations.Find(id);
            if (annotation == null)
            {
                return HttpNotFound();
            }
            return View(annotation);
        }

        // GET: Annotations/Create
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.PInsId = new SelectList(db.ProcessInstances, "Id", "Id");
            ViewBag.ContactId = new SelectList(db.Contacts, "Id", "FirstName");
            return View();
        }

        // POST: Annotations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Message,Time,Type,Hidden,PInsId,ContactId,AuthorId")] Annotation annotation)
        {
            if (ModelState.IsValid)
            {
                db.Annotations.Add(annotation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", annotation.AuthorId);
            ViewBag.PInsId = new SelectList(db.ProcessInstances, "Id", "Id", annotation.PInsId);
            ViewBag.ContactId = new SelectList(db.Contacts, "Id", "FirstName", annotation.ContactId);
            return View(annotation);
        }

        // GET: Annotations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Annotation annotation = db.Annotations.Find(id);
            if (annotation == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", annotation.AuthorId);
            ViewBag.PInsId = new SelectList(db.ProcessInstances, "Id", "Id", annotation.PInsId);
            ViewBag.ContactId = new SelectList(db.Contacts, "Id", "FirstName", annotation.ContactId);
            return View(annotation);
        }

        // POST: Annotations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Message,Time,Type,Hidden,PInsId,ContactId,AuthorId")] Annotation annotation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(annotation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", annotation.AuthorId);
            ViewBag.PInsId = new SelectList(db.ProcessInstances, "Id", "Id", annotation.PInsId);
            ViewBag.ContactId = new SelectList(db.Contacts, "Id", "FirstName", annotation.ContactId);
            return View(annotation);
        }

        // GET: Annotations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Annotation annotation = db.Annotations.Find(id);
            if (annotation == null)
            {
                return HttpNotFound();
            }
            return View(annotation);
        }

        // POST: Annotations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Annotation annotation = db.Annotations.Find(id);
            db.Annotations.Remove(annotation);
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
