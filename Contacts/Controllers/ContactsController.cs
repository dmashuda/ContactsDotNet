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
using PagedList;

namespace Contacts.Models
{
    public class ContactsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Contacts
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = searchString;
            ViewBag.FirstNameSortParam = String.IsNullOrEmpty(sortOrder) ? "firstName_dsc" : "firstName_asc";
            ViewBag.MiddleNameSortParam = String.IsNullOrEmpty(sortOrder) ? "middleName_dsc" : "middleName_asc";
            ViewBag.LastNameSortParam = String.IsNullOrEmpty(sortOrder) ? "lastName_dsc" : "lastName_asc";

            string userId = User.Identity.GetUserId();

            if (userId == null)
            {
                return new HttpUnauthorizedResult();
            }

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ApplicationUser user = db.Users.Find(userId);

            var contacts = user.Contacts.OrderBy(s => s.ContactId).AsEnumerable();

            if (!String.IsNullOrEmpty(searchString))
            {
                contacts = contacts.Where(c => c.LastName.Contains(searchString)
                                       || c.FirstName.Contains(searchString) || c.MiddleName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "firstName_asc":
                    contacts = contacts.OrderBy(s => s.FirstName);
                    break;
                case "lastName_asc":
                    contacts = contacts.OrderBy(s => s.LastName);
                    break;
                case "middleName_asc":
                    contacts = contacts.OrderBy(s => s.MiddleName);
                    break;
                case "firstName_dsc":
                    contacts = contacts.OrderByDescending(s => s.FirstName);
                    break;
                case "lastName_dsc":
                    contacts = contacts.OrderByDescending(s => s.LastName);
                    break;
                case "middleName_dsc":
                    contacts = contacts.OrderByDescending(s => s.MiddleName);
                    break;
                
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(contacts.ToPagedList(pageNumber, pageSize));
        }

        // GET: Contacts
        public ActionResult Duplicates(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = searchString;
            ViewBag.FirstNameSortParam = String.IsNullOrEmpty(sortOrder) ? "firstName_dsc" : "firstName_asc";
            ViewBag.MiddleNameSortParam = String.IsNullOrEmpty(sortOrder) ? "middleName_dsc" : "middleName_asc";
            ViewBag.LastNameSortParam = String.IsNullOrEmpty(sortOrder) ? "lastName_dsc" : "lastName_asc";

            string userId = User.Identity.GetUserId();

            if (userId == null)
            {
                return new HttpUnauthorizedResult();
            }

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ApplicationUser user = db.Users.Find(userId);
            var duplicateContacts = user.Contacts.GroupBy(c => new { c.LastName, c.FirstName }).SelectMany(c => c.Skip(1)).Distinct();

            if (!String.IsNullOrEmpty(searchString))
            {
                duplicateContacts = duplicateContacts.Where(c => c.LastName.Contains(searchString)
                                       || c.FirstName.Contains(searchString) || c.MiddleName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "firstName_asc":
                    duplicateContacts = duplicateContacts.OrderBy(s => s.FirstName);
                    break;
                case "lastName_asc":
                    duplicateContacts = duplicateContacts.OrderBy(s => s.LastName);
                    break;
                case "middleName_asc":
                    duplicateContacts = duplicateContacts.OrderBy(s => s.MiddleName);
                    break;
                case "firstName_dsc":
                    duplicateContacts = duplicateContacts.OrderByDescending(s => s.FirstName);
                    break;
                case "lastName_dsc":
                    duplicateContacts = duplicateContacts.OrderByDescending(s => s.LastName);
                    break;
                case "middleName_dsc":
                    duplicateContacts = duplicateContacts.OrderByDescending(s => s.MiddleName);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(duplicateContacts.ToPagedList(pageNumber, pageSize));
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
