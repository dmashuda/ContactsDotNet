using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.Security;

namespace Contacts.Models
{
    public class ContactsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Contacts
        public ActionResult Index(string sortOrder)
        {

            ViewBag.FirstNameSortParam = String.IsNullOrEmpty(sortOrder) ? "firstName_dsc" : "firstName_asc";

            ViewBag.MiddleNameSortParam = String.IsNullOrEmpty(sortOrder) ? "middleName_dsc" : "middleName_asc";

            ViewBag.LastNameSortParam = String.IsNullOrEmpty(sortOrder) ? "lastName_dsc" : "lastName_asc";

            string userId = User.Identity.GetUserId();


            if (userId == null)
            {
                return new HttpUnauthorizedResult();
            }


            ApplicationUser user = db.Users.Find(userId);

            var contacts = user.Contacts.OrderBy(s=> s.ContactId);

            switch (sortOrder)
            {
                case "firstName_asc":
                    contacts = contacts.OrderBy(s => s.FirstName);
                    break;
                case "lastName_asc":
                    contacts = contacts.OrderBy(s => s.LastName);
                    break;
                case "firstName_dsc":
                    contacts = contacts.OrderByDescending(s => s.FirstName);
                    break;
                case "lastName_dsc":
                    contacts = contacts.OrderByDescending(s => s.LastName);
                    break;
            }

            

            return View(contacts.ToList());
        }

        // GET: Contacts/Details/5
        public ActionResult Details(int? id)
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
            
            string userId = User.Identity.GetUserId();
            if (!contact.ApplicationUser.Id.Equals(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "no hacking....");
            }
             
            return View(contact);
        }

        // GET: Contacts/Create
        public ActionResult Create()
        {
            string userId = User.Identity.GetUserId();

            if (userId == null)
            {
                return new HttpUnauthorizedResult();
            }

            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ContactId,FirstName,MiddleName,LastName,PhoneNumber,Email")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                string userid = User.Identity.GetUserId();

                if (userid == null)
                {
                    return new HttpUnauthorizedResult();
                }
                
                ApplicationUser user =  db.Users.Find(userid);

                user.Contacts.Add(contact);



                //db.Contacts.Add(contact);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contact);
        }

        // GET: Contacts/Edit/5
        public ActionResult Edit(int? id)
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

            string userId = User.Identity.GetUserId();
            if (!contact.ApplicationUser.Id.Equals(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "no hacking....");
            }

            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ContactId,FirstName,MiddleName,LastName,PhoneNumber,Email")] Contact contact)
        {


            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contact);
        }

        // GET: Contacts/Delete/5
        public ActionResult Delete(int? id)
        {
            string userId = User.Identity.GetUserId();


            if (userId == null)
            {
                return new HttpUnauthorizedResult();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);


            if (contact == null)
            {
                return HttpNotFound();
            }

            if (!contact.ApplicationUser.Id.Equals(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "no hacking....");
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string userId = User.Identity.GetUserId();


            if (userId == null)
            {
                return new HttpUnauthorizedResult();
            }

            Contact contact = db.Contacts.Find(id);
            if (!contact.ApplicationUser.Id.Equals(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "no hacking....");
            }
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
    }
}
